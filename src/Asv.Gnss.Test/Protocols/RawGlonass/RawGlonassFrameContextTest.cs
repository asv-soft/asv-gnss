using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class RawGlonassFrameContextTest
{
    // Fields and check bits are independent transcriptions of ICD 5.1 tables 4.6/4.9/4.11 and section 4.7.
    private const string Frame14 = "0EA0040755555555555504";
    private const string EditedFrame14 = "0E40180555555555555535";
    private const string Frame15 = "0FAA2468ACF13579BDE13D";
    private const string EditedFrame15 = "0FA82468ACF13579BDE0F7";

    [Theory]
    [InlineData(7, "0700000000000000000150")]
    [InlineData(9, "0900000000000000000180")]
    [InlineData(11, "0B000000000000000001D1")]
    [InlineData(13, "0D000000000000000001D2")]
    [InlineData(15, "0F00000000000000000183")]
    public void OddAlmanac_HealthBitNine_IsDecodedVisitedAndSerialized(byte id, string hex)
    {
        var model = new GlonassWordOdd();
        Decode(model, hex);
        Assert.Equal(id, model.Id);
        Assert.Equal(1, model.ln);
        Assert.Equal(1602000000, model.Frequency);
        Assert.Equal(Convert.FromHexString(hex), Encode(model));
        var editor = new HealthEditor();
        model.Accept(editor);
        Assert.True(editor.Visited);
        var bytes = Encode(model);
        Assert.Equal(0, bytes[9] & 1);
        Assert.True(GlonassWordChecksum.IsValid(bytes));
        Assert.Equal(Convert.FromHexString(hex)[..9], bytes[..9]);
        Decode(model, Convert.ToHexString(bytes));
        Assert.Equal(0, model.ln);
        model.ln = 2;
        Assert.Throws<ArgumentOutOfRangeException>(() => Encode(model));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Frame14_ContextBeforeOrAfterDecode_UsesUt1LayoutAndPreservesReservedBits(
        bool contextFirst
    )
    {
        var model = new GlonassWord14();
        if (contextFirst)
            model.FrameNumber = 5;
        Decode(model, Frame14);
        model.FrameNumber = 5;
        Assert.Equal(-0.25, model.B1);
        Assert.Equal(1.0 / 512, model.B2);
        Assert.Equal(3, model.KP);
        Assert.Equal(Convert.FromHexString(Frame14), Encode(model));
        model.B1 = 0.5;
        model.B2 = -1.0 / 256;
        model.KP = 2;
        // Inactive almanac edits must not replace the special frame-five payload.
        model.i = double.NaN;
        model.Prn = 255;
        model.FrameNumber = 5;
        Assert.Equal(Convert.FromHexString(EditedFrame14), Encode(model));
        Assert.True(GlonassWordChecksum.IsValid(Encode(model)));
    }

    [Theory]
    [InlineData("0E7FFFF80000000000005E", 1023.0 / 1024, -511.0 / 65536, 0)]
    [InlineData("0EFFEFFA000000000000ED", -1023.0 / 1024, 511.0 / 65536, 1)]
    [InlineData("0E801004000000000000E2", -0.0, -0.0, 2)]
    [InlineData("0E00000600000000000051", 0.0, 0.0, 3)]
    public void Frame14_SignedBoundsNegativeZeroAndEveryLeapNotification_RoundTripLiteralBits(
        string hex,
        double b1,
        double b2,
        byte kp
    )
    {
        var model = new GlonassWord14 { FrameNumber = 5 };
        Decode(model, hex);
        Assert.Equal(BitConverter.DoubleToInt64Bits(b1), BitConverter.DoubleToInt64Bits(model.B1));
        Assert.Equal(BitConverter.DoubleToInt64Bits(b2), BitConverter.DoubleToInt64Bits(model.B2));
        Assert.Equal(kp, model.KP);
        Assert.Equal(Convert.FromHexString(hex), Encode(model));
    }

    [Fact]
    public void Frame14_UnrepresentableValuesAndInvalidContext_DoNotWritePartialOutput()
    {
        var model = new GlonassWord14 { FrameNumber = 5 };
        Decode(model, Frame14);
        Assert.Throws<ArgumentOutOfRangeException>(() => model.FrameNumber = 6);
        Assert.Equal(5, model.FrameNumber);
        Assert.Equal(Convert.FromHexString(Frame14), Encode(model));
        foreach (var invalid in new[] { -1.0, 1.0, double.NaN, double.PositiveInfinity })
        {
            model.B1 = invalid;
            AssertAtomicFailure(model);
        }
        model.B1 = -0.25;
        model.B2 = 512.0 / 65536;
        AssertAtomicFailure(model);
        model.B2 = -512.0 / 65536;
        AssertAtomicFailure(model);
        model.B2 = 1.0 / 512;
        model.KP = 4;
        AssertAtomicFailure(model);
    }

    [Fact]
    public void ContextSwitch_EncodesActiveEditsAndReinterpretsSameBitsWithoutResettingSameLayout()
    {
        var model = new GlonassWord14 { FrameNumber = 5 };
        Decode(model, Frame14);
        model.B1 = 0.5;
        model.B2 = -1.0 / 256;
        model.KP = 2;
        model.FrameNumber = 4;
        Assert.Equal(Convert.FromHexString(EditedFrame14), Encode(model));
        model.С = 1;
        model.FrameNumber = 3;
        Assert.Equal(1, model.С);
        model.FrameNumber = 5;
        Assert.Equal(-0.5, model.B1);
        Assert.Equal(-1.0 / 256, model.B2);
        Assert.Equal(2, model.KP);
        Decode(model, Frame14);
        Assert.Equal(5, model.FrameNumber);
        Assert.Equal(-0.25, model.B1);
        model.FrameNumber = 0;
        Assert.Equal(Convert.FromHexString(Frame14), Encode(model));
    }

    [Fact]
    public void ContextSwitch_InvalidActiveField_KeepsContextAndRawPayloadUnchanged()
    {
        var model = new GlonassWord14 { FrameNumber = 5 };
        Decode(model, Frame14);
        var original = model.RawData.ToArray();
        model.B1 = double.NaN;
        Assert.Throws<ArgumentOutOfRangeException>(() => model.FrameNumber = 4);
        Assert.Equal(5, model.FrameNumber);
        Assert.Equal(original, model.RawData);
        Assert.True(double.IsNaN(model.B1));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Frame15_ContextBeforeOrAfterDecode_ExposesAllReservedBitsAndHealth(
        bool contextFirst
    )
    {
        var model = new GlonassWord15();
        if (contextFirst)
            model.FrameNumber = 5;
        Decode(model, Frame15);
        model.FrameNumber = 5;
        Assert.Equal(Convert.FromHexString("55123456789ABCDEF0"), model.ReservedData);
        Assert.Equal(1, model.ln);
        Assert.Equal(Convert.FromHexString(Frame15), Encode(model));
        var reserved = model.ReservedData;
        reserved[0] ^= 1;
        Assert.Equal(Convert.FromHexString(Frame15), Encode(model));
        model.ReservedData = reserved;
        model.ln = 0;
        model.Frequency = 0;
        model.omega = double.NaN;
        model.FrameNumber = 5;
        Assert.Equal(Convert.FromHexString(EditedFrame15), Encode(model));
        Assert.True(GlonassWordChecksum.IsValid(Encode(model)));
        model.FrameNumber = 2;
        Assert.Equal(Convert.FromHexString(EditedFrame15), Encode(model));
        model.FrameNumber = 5;
        Assert.Equal(Convert.FromHexString("54123456789ABCDEF0"), model.ReservedData);
        Decode(model, Frame15);
        Assert.Equal(Convert.FromHexString("55123456789ABCDEF0"), model.ReservedData);
        Assert.Equal(1, model.ln);
    }

    [Fact]
    public void Frame15_ReservedDataAndContextValidation_PreservePayloadOnFailure()
    {
        var model = new GlonassWord15 { FrameNumber = 5 };
        Decode(model, Frame15);
        Assert.Throws<ArgumentException>(() => model.ReservedData = null!);
        Assert.Throws<ArgumentException>(() => model.ReservedData = new byte[8]);
        Assert.Throws<ArgumentException>(() => model.ReservedData = new byte[10]);
        Assert.Throws<ArgumentException>(() => model.ReservedData = [0x80, 0, 0, 0, 0, 0, 0, 0, 0]);
        Assert.Throws<ArgumentOutOfRangeException>(() => model.FrameNumber = 255);
        Assert.Equal(5, model.FrameNumber);
        Assert.Equal(Convert.FromHexString(Frame15), Encode(model));
        model.FrameNumber = 0;
        Assert.Throws<InvalidOperationException>(() => model.ReservedData = new byte[9]);
        Assert.Equal(Convert.FromHexString(Frame15), Encode(model));
    }

    [Fact]
    public void Visitor_SelectsContextBeforeVisitingOnlyActiveLayoutAndEditsWireFields()
    {
        var word14 = new GlonassWord14();
        Decode(word14, Frame14);
        var editor14 = new FrameEditor();
        word14.Accept(editor14);
        Assert.Equal(new[] { "FrameNumber", "B1", "B2", "KP" }, editor14.Fields);
        Assert.Equal(Convert.FromHexString(EditedFrame14), Encode(word14));
        var word15 = new GlonassWord15();
        Decode(word15, Frame15);
        var editor15 = new FrameEditor();
        word15.Accept(editor15);
        Assert.Equal(new[] { "FrameNumber", "ReservedData", "ln" }, editor15.Fields);
        Assert.Equal(Convert.FromHexString(EditedFrame15), Encode(word15));
        Assert.Equal(9, editor15.ArrayElements);
    }

    [Fact]
    public void Frame15_HealthVisitorEditsBackingFieldImmediately()
    {
        var model = new GlonassWord15 { FrameNumber = 5 };
        Decode(model, Frame15);
        var visitor = new ImmediateHealthEditor(model);
        model.Accept(visitor);
        Assert.True(visitor.Visited);
        Assert.Equal(0, Encode(model)[9] & 1);
    }

    [Fact]
    public void NewModels_ExplicitFrameFiveDoesNotRequireLegacyAlmanacDefaults()
    {
        var word14 = new GlonassWord14();
        var word15 = new GlonassWord15();
        Assert.Equal(0, word14.i);
        Assert.Equal(0, word15.Frequency);
        word14.FrameNumber = 5;
        word15.FrameNumber = 5;
        Assert.Equal(Convert.FromHexString("0E000000000000000000D0"), Encode(word14));
        Assert.Equal(Convert.FromHexString("0F00000000000000000000"), Encode(word15));
        Assert.Throws<ArgumentNullException>(() => word14.Accept(null!));
        Assert.Throws<ArgumentNullException>(() => word15.Accept(null!));
    }

    [Fact]
    public void LegacyReceiverFactory_StillReturnsSharedAlmanacClassesAndPreservesAllBits()
    {
        var even = GlonassWordFactory.Create(ToReceiverWords(Frame14));
        var odd = GlonassWordFactory.Create(ToReceiverWords(Frame15));
        Assert.IsType<GlonassWordEven>(even);
        Assert.IsType<GlonassWordOdd>(odd);
        Assert.Equal(Convert.FromHexString(Frame14), Encode(even));
        Assert.Equal(Convert.FromHexString(Frame15), Encode(odd));
        Assert.Equal(1, ((GlonassWordOdd)odd).ln);
    }

    private static void Decode(GlonassWordBase model, string hex)
    {
        ReadOnlySpan<byte> data = Convert.FromHexString(hex);
        model.Deserialize(ref data);
        Assert.True(data.IsEmpty);
    }

    private static byte[] Encode(GlonassWordBase model)
    {
        var data = new byte[11];
        Span<byte> destination = data;
        model.Serialize(ref destination);
        Assert.True(destination.IsEmpty);
        return data;
    }

    private static void AssertAtomicFailure(GlonassWordBase model)
    {
        var bytes = Enumerable.Repeat((byte)0xCC, 14).ToArray();
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Span<byte> destination = bytes;
            model.Serialize(ref destination);
        });
        Assert.All(bytes, value => Assert.Equal(0xCC, value));
    }

    private static uint[] ToReceiverWords(string hex)
    {
        var bytes = Convert.FromHexString(hex);
        var result = new uint[3];
        for (var bit = 0; bit < 85; bit++)
            result[bit / 32] |=
                (uint)((bytes[(bit + 3) / 8] >> (7 - (bit + 3) % 8)) & 1) << (31 - bit % 32);
        return result;
    }

    private sealed class HealthEditor : UInt8Type.IVisitor
    {
        public bool Visited { get; private set; }

        public void VisitUnknown(Field field, IFieldType type) { }

        public void Visit(Field field, UInt8Type type, ref byte value)
        {
            if (field.Name == "ln")
            {
                Visited = true;
                value = 0;
            }
        }
    }

    private sealed class FrameEditor : UInt8Type.IVisitor, DoubleType.IVisitor, ArrayType.IVisitor
    {
        public List<string> Fields { get; } = [];
        public int ArrayElements { get; private set; }

        public void VisitUnknown(Field field, IFieldType type) =>
            throw new InvalidOperationException(field.Name);

        public void BeginArray(Field field, ArrayType type) => Fields.Add(field.Name);

        public void EndArray() { }

        public void Visit(Field field, UInt8Type type, ref byte value)
        {
            if (field.Name == "ReservedData")
            {
                if (ArrayElements++ == 0)
                    value ^= 1;
                return;
            }
            Fields.Add(field.Name);
            value = field.Name switch
            {
                "FrameNumber" => 5,
                "KP" => 2,
                "ln" => 0,
                _ => throw new InvalidOperationException(field.Name),
            };
        }

        public void Visit(Field field, DoubleType type, ref double value)
        {
            Fields.Add(field.Name);
            value = field.Name switch
            {
                "B1" => 0.5,
                "B2" => -1.0 / 256,
                _ => throw new InvalidOperationException(field.Name),
            };
        }
    }

    private sealed class ImmediateHealthEditor(GlonassWord15 model) : UInt8Type.IVisitor
    {
        public bool Visited { get; private set; }

        public void VisitUnknown(Field field, IFieldType type) { }

        public void Visit(Field field, UInt8Type type, ref byte value)
        {
            if (field.Name != "ln")
                return;
            value = 0;
            Assert.Equal(0, model.ln);
            Visited = true;
        }
    }
}
