using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class UbxVisitableTest
{
    private static readonly HashSet<string> HeaderProperties =
    [
        nameof(UbxMessageBase.Class), nameof(UbxMessageBase.SubClass), nameof(UbxMessageBase.Name),
        nameof(UbxMessageBase.Id), nameof(UbxMessageBase.Protocol), nameof(UbxMessageBase.Tags)
    ];

    public static IEnumerable<object[]> MessageTypes => typeof(UbxMessageBase).Assembly.GetTypes()
        .Where(type => !type.IsAbstract && typeof(UbxMessageBase).IsAssignableFrom(type))
        .Where(type => !typeof(UbxX20MessageBase).IsAssignableFrom(type))
        .OrderBy(type => type.FullName).Select(type => new object[] { type });

    public static IEnumerable<object[]> NestedTypes =>
    [
        [typeof(UbxCfgPrtConfigUart)], [typeof(UbxCfgPrtConfigUsb)], [typeof(UbxCfgPrtConfigSpi)],
        [typeof(UbxNavSatelliteItem)], [typeof(SvData)]
    ];

    [Theory]
    [MemberData(nameof(MessageTypes))]
    [MemberData(nameof(NestedTypes))]
    public void Accept_ExistingUbxType_ShouldExposeTypedPayloadMetadataAndPreserveDefaults(Type type)
    {
        var instance = Activator.CreateInstance(type)!;
        var visitable = Assert.IsAssignableFrom<IVisitable>(instance);
        var properties = type.GetProperties().Where(property => property.GetMethod is { IsStatic: false }
            && !HeaderProperties.Contains(property.Name)).ToArray();
        var expected = properties.Select(property => property.Name)
            .Where(name => type != typeof(UbxRxmSfrbx) || name is not (nameof(UbxRxmSfrbx.GpsSubFrame) or nameof(UbxRxmSfrbx.GlonassWord)))
            .OrderBy(name => name).ToArray();
        var snapshots = properties.Select(property => (Property: property, Value: property.GetValue(instance),
            Elements: (property.GetValue(instance) as IEnumerable)?.Cast<object?>().ToArray())).ToArray();
        var visitor = new UnknownFieldCollector();

        visitable.Accept(visitor);

        Assert.Equal(expected, visitor.Fields.Select(field => field.Name).OrderBy(name => name));
        Assert.All(visitor.Fields, field =>
        {
            Assert.NotNull(field.DataType);
            Assert.False(string.IsNullOrWhiteSpace(field.GetTitle()));
            Assert.False(string.IsNullOrWhiteSpace(field.GetDescription()));
        });
        AssertPreserved();

        visitable.PrintValues();

        AssertPreserved();
        return;

        void AssertPreserved()
        {
            foreach (var snapshot in snapshots)
            {
                var current = snapshot.Property.GetValue(instance);
                if (snapshot.Property.PropertyType.IsValueType)
                {
                    Assert.Equal(snapshot.Value, current);
                }
                else
                {
                    Assert.Same(snapshot.Value, current);
                }
                if (snapshot.Elements != null)
                {
                    Assert.Equal(snapshot.Elements, ((IEnumerable)current!).Cast<object?>());
                }
            }
        }
    }

    [Theory]
    [MemberData(nameof(MessageTypes))]
    public void Accept_NullVisitor_ShouldThrowArgumentNullException(Type type)
    {
        var message = (UbxMessageBase)Activator.CreateInstance(type)!;

        Assert.Throws<ArgumentNullException>(() => message.Accept(null!));
    }

    [Theory]
    [InlineData(typeof(UbxAckAck))]
    [InlineData(typeof(UbxAckNak))]
    [InlineData(typeof(UbxInfDebug))]
    [InlineData(typeof(UbxInfError))]
    [InlineData(typeof(UbxInfNotice))]
    [InlineData(typeof(UbxInfTest))]
    [InlineData(typeof(UbxCfgAnt))]
    [InlineData(typeof(UbxCfgCfg))]
    [InlineData(typeof(UbxCfgMsg))]
    [InlineData(typeof(UbxCfgNav5))]
    [InlineData(typeof(UbxCfgPrt))]
    [InlineData(typeof(UbxCfgRate))]
    [InlineData(typeof(UbxCfgRst))]
    [InlineData(typeof(UbxCfgTMode3))]
    public void Accept_ReadOnlyVisitor_ShouldPreserveExistingBinarySerialization(Type type)
    {
        var message = (UbxMessageBase)Activator.CreateInstance(type)!;
        if (message is UbxCfgPrt port)
        {
            port.Config = new UbxCfgPrtConfigUart { PortId = 1, BoundRate = 115200, Threshold = 12, Pin = 5 };
        }
        else
        {
            message.Randomize(new Random(1234));
        }
        var before = UbxTestHelper.Serialize(message);

        message.PrintValues();

        Assert.Equal(before, UbxTestHelper.Serialize(message));
    }

    [Fact]
    public void Accept_ExternalSubclassWithoutOverride_ShouldRemainUsable()
    {
        var message = new CustomMessage();
        var visitor = new UnknownFieldCollector();

        ((IVisitable)message).Accept(visitor);

        Assert.Empty(visitor.Fields);
        Assert.Equal(new byte[] { 0xB5, 0x62, 0x99, 0x01, 0, 0, 0x9A, 0x67 }, UbxTestHelper.Serialize(message));
    }

    private sealed class UnknownFieldCollector : IVisitor
    {
        public List<Field> Fields { get; } = [];
        public void VisitUnknown(Field field, IFieldType type) => Fields.Add(field);
    }

    private sealed class CustomMessage : UbxMessageBase
    {
        public override byte Class => 0x99;
        public override byte SubClass => 1;
        public override string Name => "CUSTOM";
        protected override void SerializeContent(ref Span<byte> buffer) { }
        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer) { }
        protected override int GetContentByteSize() => 0;
        public override void Randomize(Random random) { }
    }
}
