using System;
using System.Linq;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class UbxRefVisitorTest
{
    [Fact]
    public void Accept_ScalarMutation_ShouldBeVisibleDuringVisitorCallback()
    {
        var message = new UbxCfgRate { RateHz = 1 };
        var visitor = new DirectDoubleVisitor(message);

        message.Accept(visitor);

        Assert.True(visitor.Visited);
        Assert.Equal(2, message.RateHz);
    }

    [Fact]
    public void Accept_VariableArrayMutation_ShouldBeVisibleDuringVisitorCallback()
    {
        var words = new uint[] { 0x80000000, 0xFFFFFFFF };
        var message = new UbxRxmSfrbx { RawData = words };
        var visitor = new DirectWordVisitor(message);

        message.Accept(visitor);

        Assert.Equal(2, visitor.Visits);
        Assert.Same(words, message.RawData);
        Assert.Equal(new uint[] { 0x80000001, 0 }, words);
    }

    [Fact]
    public void Accept_FixedArrayMutation_ShouldBeVisibleDuringVisitorCallback()
    {
        var message = new UbxMonRxbuf();
        var visitor = new DirectCounterVisitor(message);

        message.Accept(visitor);

        Assert.Equal(6, visitor.Visits);
        Assert.All(message.Pending, value => Assert.Equal(1, value));
    }

    [Fact]
    public void Accept_ListElementMutation_ShouldBeVisibleDuringVisitorCallback()
    {
        var message = new UbxMonVer { Extensions = ["ROM", "PROTVER=27"] };
        var extensions = message.Extensions;
        var visitor = new DirectExtensionVisitor(message);

        message.Accept(visitor);

        Assert.Equal(2, visitor.Visits);
        Assert.Same(extensions, message.Extensions);
        Assert.Equal(new[] { "ROM edited", "PROTVER=27 edited" }, extensions);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    public void Accept_ReusedMessage_ShouldNotAllocateDuringTraversal(int kind)
    {
        UbxMessageBase message = kind switch
        {
            0 => new UbxCfgNav5(),
            1 => new UbxNavPvt(),
            2 => new UbxNavSat { NumSvs = 32, Items = Enumerable.Range(0, 32).Select(_ => new UbxNavSatelliteItem()).ToArray() },
            3 => new UbxMonHw { VP = new byte[17] },
            4 => new UbxMonRxbuf(),
            5 => new UbxMonMsgpp(),
            6 => new UbxRxmSfrbx { NumWords = 10, RawData = new uint[10] },
            7 => new UbxMonVer { Software = "FW", Hardware = "HW", Extensions = ["ROM", "PROTVER=27"] },
            8 => new UbxCfgPrt { Config = new UbxCfgPrtConfigUart() },
            _ => new UbxMonHw()
        };
        var visitor = new NoOpVisitor();
        for (var index = 0; index < 1_000; index++)
        {
            message.Accept(visitor);
        }

        var before = GC.GetAllocatedBytesForCurrentThread();
        for (var index = 0; index < 100; index++)
        {
            message.Accept(visitor);
        }
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(0, allocated);
        GC.KeepAlive(message);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(6)]
    [InlineData(8)]
    public void Accept_ReadOnlyFixedArray_ShouldPreserveNullLengthAndIdentity(int length)
    {
        var ports = length < 0 ? null : Enumerable.Range(1, length).Select(value => (byte)value).ToArray();
        var message = new UbxCfgMsg { Ports = ports! };

        message.Accept(new NoOpVisitor());

        Assert.Same(ports, message.Ports);
        if (ports != null)
        {
            Assert.Equal(Enumerable.Range(1, length).Select(value => (byte)value), ports);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(2)]
    public void Accept_EditedShortFixedArray_ShouldMaterializeMissingElements(int length)
    {
        var ports = length < 0 ? null : new byte[length];
        var message = new UbxCfgMsg { Ports = ports! };

        message.Accept(new IncrementByteVisitor());

        Assert.NotSame(ports, message.Ports);
        Assert.Equal(6, message.Ports.Length);
        Assert.All(message.Ports, value => Assert.Equal(1, value));
        if (ports != null)
        {
            Assert.All(ports, value => Assert.Equal(0, value));
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void Accept_ResizeWords_ShouldRetainPrefixAndInitializeNewElements(int requestedSize)
    {
        var words = new uint[] { 123, 456 };
        var message = new UbxRxmSfrbx { RawData = words, NumWords = 2 };

        message.Accept(new ResizeWordsVisitor((uint)requestedSize));

        Assert.NotSame(words, message.RawData);
        Assert.Equal(requestedSize == 0 ? [] : new uint[] { 123, 456, 0 }, message.RawData);
        Assert.Equal(2, message.NumWords);
    }

    [Fact]
    public void Accept_InvalidRequestedArraySize_ShouldRejectBeforeChangingStorage()
    {
        var words = new uint[] { 123, 456 };
        var message = new UbxRxmSfrbx { RawData = words };

        Assert.Throws<ArgumentOutOfRangeException>(() => message.Accept(new ResizeWordsVisitor(17)));

        Assert.Same(words, message.RawData);
        Assert.Equal(new uint[] { 123, 456 }, words);
    }

    private sealed class DirectDoubleVisitor(UbxCfgRate message) : DoubleType.IVisitor
    {
        public bool Visited { get; private set; }
        public void VisitUnknown(Field field, IFieldType type) { }
        public void Visit(Field field, DoubleType type, ref double value)
        {
            value = 2;
            Assert.Equal(2, message.RateHz);
            Visited = true;
        }
    }

    private sealed class DirectWordVisitor(UbxRxmSfrbx message) : UInt32Type.IVisitor, ListType.IVisitor
    {
        public int Visits { get; private set; }
        public void VisitUnknown(Field field, IFieldType type) { }
        public void BeginList(Field field, ListType type, ref uint size) { }
        public void EndList() { }
        public void Visit(Field field, UInt32Type type, ref uint value)
        {
            value = unchecked(value + 1);
            Assert.Equal(value, message.RawData[Visits]);
            Visits++;
        }
    }

    private sealed class DirectCounterVisitor(UbxMonRxbuf message) : UInt16Type.IVisitor, ArrayType.IVisitor
    {
        public int Visits { get; private set; }
        public void VisitUnknown(Field field, IFieldType type) { }
        public void BeginArray(Field field, ArrayType fieldType) { }
        public void EndArray() { }
        public void Visit(Field field, UInt16Type type, ref ushort value)
        {
            value++;
            Assert.Equal(value, message.Pending[Visits]);
            Visits++;
        }
    }

    private sealed class IncrementByteVisitor : UInt8Type.IVisitor, ArrayType.IVisitor
    {
        public void VisitUnknown(Field field, IFieldType type) { }
        public void BeginArray(Field field, ArrayType fieldType) { }
        public void EndArray() { }
        public void Visit(Field field, UInt8Type type, ref byte value) => value++;
    }

    private sealed class DirectExtensionVisitor(UbxMonVer message) : StringType.IVisitor, ListType.IVisitor
    {
        public int Visits { get; private set; }
        public void VisitUnknown(Field field, IFieldType type) { }
        public void BeginList(Field field, ListType type, ref uint size) { }
        public void EndList() { }
        public void Visit(Field field, StringType type, ref string value)
        {
            value += " edited";
            Assert.Equal(value, message.Extensions[Visits]);
            Visits++;
        }
    }

    private sealed class ResizeWordsVisitor(uint requestedSize) : ListType.IVisitor
    {
        public void VisitUnknown(Field field, IFieldType type) { }
        public void BeginList(Field field, ListType type, ref uint size) => size = requestedSize;
        public void EndList() { }
    }

    private sealed class NoOpVisitor() : FullVisitorBase(true)
    {
        public override void Visit(Field field, DoubleOptionalType type, ref double? value) {  }
        public override void Visit(Field field, FloatOptionalType type, ref float? value) {  }
        public override void Visit(Field field, HalfFloatOptionalType type, ref Half? value) {  }
        public override void Visit(Field field, Int8OptionalType type, ref sbyte? value) {  }
        public override void Visit(Field field, Int16OptionalType type, ref short? value) {  }
        public override void Visit(Field field, Int32OptionalType type, ref int? value) {  }
        public override void Visit(Field field, Int64OptionalType type, ref long? value) {  }
        public override void Visit(Field field, UInt8OptionalType type, ref byte? value) {  }
        public override void Visit(Field field, UInt16OptionalType type, ref ushort? value) {  }
        public override void Visit(Field field, UInt32OptionalType type, ref uint? value) {  }
        public override void Visit(Field field, UInt64OptionalType type, ref ulong? value) {  }
        public override void Visit(Field field, StringOptionalType type, ref string? value) {  }
        public override void Visit(Field field, BoolOptionalType type, ref bool? value) {  }
        public override void Visit(Field field, CharOptionalType type, ref char? value) {  }
        public override void Visit(Field field, DateTimeType type, ref DateTime value) {  }
        public override void Visit(Field field, DateTimeOptionalType type, ref DateTime? value) {  }
        public override void Visit(Field field, TimeSpanType type, ref TimeSpan value) {  }
        public override void Visit(Field field, TimeSpanOptionalType type, ref TimeSpan? value) {  }
        public override void Visit(Field field, DateOnlyType type, ref DateOnly value) {  }
        public override void Visit(Field field, DateOnlyOptionalType type, ref DateOnly? value) {  }
        public override void Visit(Field field, TimeOnlyType type, ref TimeOnly value) {  }
        public override void Visit(Field field, TimeOnlyOptionalType type, ref TimeOnly? value) {  }
        public override void Visit(Field field, DoubleType type, ref double value) {  }
        public override void Visit(Field field, FloatType type, ref float value) {  }
        public override void Visit(Field field, HalfFloatType type, ref Half value) {  }
        public override void Visit(Field field, Int8Type type, ref sbyte value) {  }
        public override void Visit(Field field, Int16Type type, ref short value) {  }
        public override void Visit(Field field, Int32Type type, ref int value) {  }
        public override void Visit(Field field, Int64Type type, ref long value) {  }
        public override void Visit(Field field, UInt8Type type, ref byte value) {  }
        public override void Visit(Field field, UInt16Type type, ref ushort value) {  }
        public override void Visit(Field field, UInt32Type type, ref uint value) {  }
        public override void Visit(Field field, UInt64Type type, ref ulong value) {  }
        public override void Visit(Field field, StringType type, ref string value) {  }
        public override void Visit(Field field, BoolType type, ref bool value) {  }
        public override void Visit(Field field, CharType type, ref char value) {  }
        public override void BeginList(Field field, ListType type, ref uint size) {  }
        public override void EndList() {  }
        public override void BeginArray(Field field, ArrayType fieldType) {  }
        public override void EndArray() {  }
        public override void BeginStruct(Field field, StructType type) {  }
        public override void EndStruct() {  }
        public override void BeginOptionalStruct(
            Field field,
            OptionalStructType type,
            bool isPresent,
            out bool createNew
        ) { createNew = false; }
        public override void EndOptionalStruct(bool isPresent) {  }
    }
}
