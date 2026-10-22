using System;
using System.Collections.Generic;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class AdsbPayloadTest
{
    // Published vectors: https://mode-s.org/1090mhz/content/ads-b/
    [Fact]
    public void Decode_PublishedIdentification_ReturnsKlm1023()
    {
        var value = Decode("8D4840D6202CC371C32CE0576098");
        Assert.Equal((byte)4, value.TypeCode);
        Assert.Equal("KLM1023", value.Identification!.Callsign);
        Assert.Equal((byte)0, value.Identification.Category);
        Assert.Null(value.Velocity);
    }

    [Fact]
    public void Decode_PublishedAirbornePosition_PreservesCprAndPressureAltitude()
    {
        var value = Decode("8D40621D58C382D690C8AC2863A7").AirbornePosition!;
        Assert.Equal(38000, value.BarometricAltitudeFeet);
        Assert.Null(value.GnssHeightMeters);
        Assert.Equal(93000U, value.LatitudeCpr);
        Assert.Equal(51372U, value.LongitudeCpr);
        Assert.False(value.IsOddCpr);
    }

    [Fact]
    public void Decode_PublishedSurfacePosition_ReturnsQuantizedMovementAndTrack()
    {
        var value = Decode("8C4841753AAB238733C8CD4020B1").SurfacePosition!;
        Assert.Equal((byte)42, value.MovementCode);
        Assert.Equal(18, value.GroundSpeedKnots);
        Assert.Equal(140.625, value.TrackDegrees);
        Assert.Equal(115609U, value.LatitudeCpr);
        Assert.Equal(116941U, value.LongitudeCpr);
        Assert.False(value.GroundSpeedIsLowerBound);
    }

    [Fact]
    public void Decode_PublishedVelocity_ReturnsSignedComponentsAndUnavailableDifference()
    {
        var a = Decode("8D485020994409940838175B284F").Velocity!;
        Assert.Equal(-8, a.EastVelocityKnots);
        Assert.Equal(-159, a.NorthVelocityKnots);
        Assert.Equal(-832, a.VerticalRateFeetPerMinute);
        Assert.Equal(550, a.GnssMinusBarometricFeet);
        Assert.Equal(Math.Sqrt(8 * 8 + 159 * 159), a.GroundSpeedKnots);
        Assert.InRange(a.TrackDegrees!.Value, 182.88, 182.89);
        var b = Decode("8DA05F219B06B6AF189400CBC33F").Velocity!;
        Assert.Equal(375, b.AirspeedKnots);
        Assert.True(b.TrueAirspeed);
        Assert.Equal(243.984375, b.HeadingDegrees);
        Assert.Equal(-2304, b.VerticalRateFeetPerMinute);
        Assert.Null(b.GnssMinusBarometricFeet);
    }

    [Theory]
    [InlineData(0, null)] [InlineData(1, 0.0)] [InlineData(2, 0.125)] [InlineData(8, 0.875)]
    [InlineData(9, 1.0)] [InlineData(12, 1.75)] [InlineData(13, 2.0)] [InlineData(38, 14.5)]
    [InlineData(39, 15.0)] [InlineData(93, 69.0)] [InlineData(94, 70.0)] [InlineData(108, 98.0)]
    [InlineData(109, 100.0)] [InlineData(123, 170.0)] [InlineData(124, 175.0)] [InlineData(125, null)]
    [InlineData(127, null)]
    public void Decode_SurfaceMovementBoundaries_RetainsUnavailableAndLowerBound(int code, double? expected)
    {
        var raw = Frame(5); Set(raw, 5, 7, code);
        var value = AdsbExtendedSquitter.Decode(raw).SurfacePosition!;
        Assert.Equal(expected, value.GroundSpeedKnots);
        Assert.Equal(code == 124, value.GroundSpeedIsLowerBound);
        Assert.Null(value.TrackDegrees);
    }

    [Fact]
    public void Decode_AltitudeDistinguishesMetersQBitAndUnavailableGillham()
    {
        var raw = Frame(20); Set(raw, 8, 12, 1234);
        Assert.Equal(1234, AdsbExtendedSquitter.Decode(raw).AirbornePosition!.GnssHeightMeters);
        raw = Frame(9);
        Assert.Null(AdsbExtendedSquitter.Decode(raw).AirbornePosition!.BarometricAltitudeFeet);
        Set(raw, 8, 12, 0x100);
        Assert.Null(AdsbExtendedSquitter.Decode(raw).AirbornePosition!.BarometricAltitudeFeet);
        Assert.Equal((ushort)0x100, AdsbExtendedSquitter.Decode(raw).AirbornePosition!.AltitudeCode);
        Set(raw, 8, 12, 0x10);
        Assert.Equal(-1000, AdsbExtendedSquitter.Decode(raw).AirbornePosition!.BarometricAltitudeFeet);
    }

    [Theory]
    [InlineData(1, 1)] [InlineData(2, 4)] [InlineData(3, 1)] [InlineData(4, 4)]
    public void Decode_VelocitySubtypes_RespectScaleAndAvailability(int subtype, int scale)
    {
        var raw = Frame(19); Set(raw, 5, 3, subtype);
        var empty = AdsbExtendedSquitter.Decode(raw).Velocity!;
        Assert.Null(empty.EastVelocityKnots); Assert.Null(empty.NorthVelocityKnots);
        Assert.Null(empty.GroundSpeedKnots); Assert.Null(empty.AirspeedKnots);
        Assert.Null(empty.HeadingDegrees); Assert.Null(empty.VerticalRateFeetPerMinute);
        Set(raw, 14, 10, 11); Set(raw, 25, 10, 21);
        var value = AdsbExtendedSquitter.Decode(raw).Velocity!;
        if (subtype <= 2)
        {
            Assert.Equal(10 * scale, value.EastVelocityKnots);
            Assert.Equal(20 * scale, value.NorthVelocityKnots);
        }
        else Assert.Equal(20 * scale, value.AirspeedKnots);
    }

    [Theory]
    [InlineData(0)] [InlineData(4095)]
    public void Decode_GnssHeight_UsesUnsignedMetersIncludingZero(int code)
    {
        var raw = Frame(20); Set(raw, 8, 12, code);
        var value = AdsbExtendedSquitter.Decode(raw).AirbornePosition!;
        Assert.Equal((double)code, value.GnssHeightMeters);
        Assert.Null(value.BarometricAltitudeFeet);
    }

    [Theory]
    [InlineData(1)] [InlineData(2)] [InlineData(3)] [InlineData(4)]
    public void Decode_VerticalRate_RespectsSourceSignZeroAndMagnitudeBounds(int subtype)
    {
        var raw = Frame(19); Set(raw, 5, 3, subtype);
        foreach (var source in new[] { 0, 1 })
        foreach (var sign in new[] { 0, 1 })
        foreach (var magnitude in new[] { 0, 1, 511 })
        {
            Set(raw, 35, 1, source); Set(raw, 36, 1, sign); Set(raw, 37, 9, magnitude);
            var value = AdsbExtendedSquitter.Decode(raw).Velocity!;
            Assert.Equal(source == 1, value.VerticalRateBarometric);
            double? expected = magnitude == 0 ? null : (magnitude - 1) * 64 * (sign == 1 ? -1 : 1);
            Assert.Equal(expected, value.VerticalRateFeetPerMinute);
        }
    }

    [Theory]
    [InlineData(0)] [InlineData(127)]
    public void Decode_AltitudeDifferenceUnavailableCodes_ReturnNull(int code)
    {
        var raw = Frame(19); Set(raw, 5, 3, 1); Set(raw, 49, 7, code);
        Assert.Null(AdsbExtendedSquitter.Decode(raw).Velocity!.GnssMinusBarometricFeet);
        Set(raw, 48, 1, 1);
        Assert.Null(AdsbExtendedSquitter.Decode(raw).Velocity!.GnssMinusBarometricFeet);
    }

    [Fact]
    public void Decode_UnsupportedTypesAndDf18Control_RetainRawWithoutInventingValues()
    {
        var raw = Frame(31);
        var value = AdsbExtendedSquitter.Decode(raw);
        Assert.Equal(Convert.ToHexString(raw.AsSpan(4, 7)), value.MessageElement);
        Assert.Null(value.Identification); Assert.Null(value.Velocity); Assert.Null(value.SurfacePosition); Assert.Null(value.AirbornePosition);
        foreach (var cf in new[] { 0, 1, 2, 5, 6 })
        {
            raw[0] = (byte)((18 << 3) | cf);
            Assert.Equal((byte)31, AdsbExtendedSquitter.Decode(raw).TypeCode);
        }
        foreach (var cf in new[] { 3, 4, 7 })
        {
            raw[0] = (byte)((18 << 3) | cf);
            Assert.Throws<ArgumentException>(() => AdsbExtendedSquitter.Decode(raw));
        }
        Assert.Throws<ArgumentException>(() => AdsbExtendedSquitter.Decode(new byte[7]));
        raw[0] = 0;
        Assert.Throws<ArgumentException>(() => AdsbExtendedSquitter.Decode(raw));
    }

    [Fact]
    public void Accept_UsesCachedTypedMetadataAndCannotMutateDecodedStorage()
    {
        var value = Decode("8D485020994409940838175B284F");
        var reader = new Editor(); value.Accept(reader);
        Assert.Equal(-8d, reader.Values[nameof(AdsbVelocity.EastVelocityKnots)]);
        var eastField = Assert.Single(reader.Fields, x => x.Name == nameof(AdsbVelocity.EastVelocityKnots));
        Assert.Equal("kt", eastField.GetUnits());
        Assert.True((bool)eastField.Metadata["readOnly"]!);
        Assert.False(string.IsNullOrWhiteSpace(eastField.GetDescription()));
        Assert.Equal(-8, value.Velocity!.EastVelocityKnots);
        Assert.Equal((byte)19, value.TypeCode);
        var again = new Editor(); value.Accept(again);
        Assert.Equal(reader.Fields.Count, again.Fields.Count);
        for (var i = 0; i < reader.Fields.Count; i++) Assert.Same(reader.Fields[i], again.Fields[i]);
        Assert.Throws<ArgumentNullException>(() => value.Accept(null!));
    }

    [Theory]
    [InlineData("8D4840D6202CC371C32CE0576098")]
    [InlineData("8D40621D58C382D690C8AC2863A7")]
    [InlineData("8C4841753AAB238733C8CD4020B1")]
    public void Accept_AllNestedPayloads_PreserveRawAndExposeTypedMetadata(string frame)
    {
        var value = Decode(frame);
        var before = value.MessageElement;
        var reader = new Editor(); value.Accept(reader);
        Assert.Equal(before, value.MessageElement);
        Assert.NotEmpty(reader.Fields);
        Assert.All(reader.Fields, field => Assert.False(string.IsNullOrWhiteSpace(field.GetDescription())));
        if (value.Identification is { } identification) Assert.Equal("KLM1023", identification.Callsign);
        if (value.AirbornePosition is { } position) Assert.Equal(93000U, position.LatitudeCpr);
        if (value.SurfacePosition is { } surface) Assert.Equal(18, surface.GroundSpeedKnots);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(6)]
    public void Decode_RelayedMessageElements_RetainsRawWithoutMislabelingImfAsAdsbFlags(byte control)
    {
        var raw = Convert.FromHexString("8D40621D58C382D690C8AC2863A7");
        raw[0] = (byte)((18 << 3) | control);
        var decoded = AdsbExtendedSquitter.Decode(raw);
        Assert.Equal((byte)11, decoded.TypeCode);
        Assert.Equal("58C382D690C8AC", decoded.MessageElement);
        Assert.Null(decoded.Identification);
        Assert.Null(decoded.AirbornePosition);
        Assert.Null(decoded.SurfacePosition);
        Assert.Null(decoded.Velocity);
    }

    private static AdsbExtendedSquitter Decode(string raw) => AdsbExtendedSquitter.Decode(Convert.FromHexString(raw));
    private static byte[] Frame(int tc) { var raw = new byte[14]; raw[0] = 0x8D; Set(raw, 0, 5, tc); return raw; }
    private static void Set(byte[] raw, int start, int count, int value)
    {
        for (var i = 0; i < count; i++)
        {
            var bit = 32 + start + i; var mask = 1 << (7 - (bit & 7));
            raw[bit >> 3] = (byte)((raw[bit >> 3] & ~mask) | (((value >> (count - i - 1)) & 1) != 0 ? mask : 0));
        }
    }

    private sealed class Editor : UInt8Type.IVisitor, UInt16Type.IVisitor, UInt32Type.IVisitor, StringType.IVisitor, BoolType.IVisitor,
        DoubleOptionalType.IVisitor, StructType.IVisitor
    {
        public List<Field> Fields { get; } = [];
        public Dictionary<string, object?> Values { get; } = [];
        public void Visit(Field f, UInt8Type t, ref byte v) { Fields.Add(f); v = 0; }
        public void Visit(Field f, UInt16Type t, ref ushort v) { Fields.Add(f); v = 0; }
        public void Visit(Field f, UInt32Type t, ref uint v) { Fields.Add(f); v = 0; }
        public void Visit(Field f, StringType t, ref string v) { Fields.Add(f); v = "changed"; }
        public void Visit(Field f, BoolType t, ref bool v) { Fields.Add(f); v = !v; }
        public void Visit(Field f, DoubleOptionalType t, ref double? v) { Fields.Add(f); Values[f.Name] = v; v = 123; }
        public void BeginStruct(Field f, StructType t) => Fields.Add(f);
        public void EndStruct() { }
        public void VisitUnknown(Field f, IFieldType t) => throw new InvalidOperationException(f.Name);
    }
}
