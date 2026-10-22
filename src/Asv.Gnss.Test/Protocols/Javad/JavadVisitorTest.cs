using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class JavadVisitorTest
{
    [Fact]
    public void Accept_AllRegisteredMessagesExposeCachedFieldsAndPreserveWireBytes()
    {
        var builder = new ProtocolMessageFactoryBuilder<JavadMessageBase, string>(
            JavadProtocol.Info
        );
        JavadBinaryMessageFactory.Register(builder);
        JavadTextMessageFactory.Register(builder);
        var factory = builder.Build();
        var ids = factory.GetSupportedIds().ToArray();
        Assert.Equal(303, ids.Length);
        foreach (var id in ids)
        {
            var message = factory.Create(id)!;
            var visitable = Assert.IsAssignableFrom<IVisitable>(message);
            var original = Serialize(message);
            var first = new FieldCollector();
            var second = new FieldCollector();

            visitable.Accept(first);
            visitable.Accept(second);
            visitable.Accept(new PrintValueVisitor(new StringBuilder(), false));

            Assert.Equal(original, Serialize(message));
            Assert.NotEmpty(first.Fields);
            Assert.Equal(first.Fields.Count, second.Fields.Count);
            Assert.Equal(
                first.Fields.Count,
                first.Fields.Select(field => field.Name).Distinct().Count()
            );
            for (var i = 0; i < first.Fields.Count; i++)
            {
                Assert.Same(first.Fields[i], second.Fields[i]);
                Assert.False(string.IsNullOrWhiteSpace(first.Fields[i].GetTitle()), id);
                Assert.False(string.IsNullOrWhiteSpace(first.Fields[i].GetDescription()), id);
            }
            Assert.Contains(
                first.Fields,
                field => field.Name == nameof(JavadMessageBase.ExtensionData)
            );
            Assert.Throws<ArgumentNullException>(() => visitable.Accept(null!));
        }
    }

    [Fact]
    public void Accept_RawPayloadAndExtensionsApplyRefEditsAndResizeBeforeSerialization()
    {
        var message = new JavadRawMessage("zz", JavadChecksumKind.Binary8)
        {
            Payload = [1, 2],
            ExtensionData = [0xAA],
        };
        message.Accept(new ByteEditor(message, 4));
        Assert.Equal(new byte[] { 2, 3, 1, 1 }, message.Payload);
        Assert.Equal(new byte[] { 0xAB }, message.ExtensionData);
        var bytes = Serialize(message);
        Assert.Equal("zz006", Encoding.ASCII.GetString(bytes, 0, 5));
        Assert.Equal(new byte[] { 2, 3, 1, 1, 0xAB }, bytes[5..^1]);
        Assert.Equal(JavadChecksum.Compute8(bytes.AsSpan(0, bytes.Length - 1)), bytes[^1]);
    }

    [Fact]
    public void Accept_InvalidListSizeRejectsBeforeChangingPayloadStorage()
    {
        var message = new JavadRawMessage("zz") { Payload = [4, 5] };
        var original = message.Payload;
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            message.Accept(new ByteEditor(message, 4096))
        );
        Assert.Same(original, message.Payload);
        Assert.Equal(new byte[] { 4, 5 }, message.Payload);
    }

    [Fact]
    public void Accept_NullEmptyStorageRemainsAbsentDuringReadOnlyTraversal()
    {
        var message = new JavadRawMessage("zz") { Payload = null!, ExtensionData = null! };
        message.Accept(new ByteEditor(message, 0));
        Assert.Null(message.Payload);
        Assert.Null(message.ExtensionData);
    }

    private sealed class FieldCollector : IVisitor
    {
        public List<Field> Fields { get; } = [];

        public void VisitUnknown(Field field, IFieldType type) => Fields.Add(field);
    }

    private sealed class ByteEditor(JavadRawMessage message, uint requestedSize)
        : ListType.IVisitor,
            UInt8Type.IVisitor
    {
        private int _index;

        public void VisitUnknown(Field field, IFieldType type) { }

        public void BeginList(Field field, ListType type, ref uint size)
        {
            _index = 0;
            if (field.Name == nameof(JavadRawMessage.Payload))
                size = requestedSize;
        }

        public void EndList() { }

        public void Visit(Field field, UInt8Type type, ref byte value)
        {
            value++;
            if (field.Name == nameof(JavadRawMessage.Payload))
                Assert.Equal(value, message.Payload[_index++]);
            else
                Assert.Equal(value, message.ExtensionData[_index++]);
        }
    }

    private static byte[] Serialize(JavadMessageBase message)
    {
        var bytes = new byte[message.GetByteSize()];
        Span<byte> buffer = bytes;
        message.Serialize(ref buffer);
        Assert.True(buffer.IsEmpty);
        return bytes;
    }
}
