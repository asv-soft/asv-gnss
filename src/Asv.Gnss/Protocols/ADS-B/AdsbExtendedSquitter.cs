using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>Read-only decoded ADS-B ExtendedSquitter fields.</summary>
public sealed class AdsbExtendedSquitter : IVisitable
{
    /// <summary>ADS-B extended squitter type code.</summary>
    public byte TypeCode { get; internal init; }
    /// <summary>Unmodified seven-byte message element as hexadecimal text.</summary>
    public string MessageElement { get; internal init; } = string.Empty;
    /// <summary>Decoded Identification payload.</summary>
    public AdsbIdentification? Identification { get; internal init; }
    /// <summary>Decoded AirbornePosition payload.</summary>
    public AdsbAirbornePosition? AirbornePosition { get; internal init; }
    /// <summary>Decoded SurfacePosition payload.</summary>
    public AdsbSurfacePosition? SurfacePosition { get; internal init; }
    /// <summary>Decoded Velocity payload.</summary>
    public AdsbVelocity? Velocity { get; internal init; }

    private static readonly Field TypeCodeField = new Field.Builder().Name(nameof(TypeCode)).Title(nameof(TypeCode)).Description("ADS-B extended squitter type code.").DataType(UInt8Type.Default).Metadata("readOnly", true).Build();
    private static readonly Field MessageElementField = new Field.Builder().Name(nameof(MessageElement)).Title(nameof(MessageElement)).Description("Unmodified seven-byte message element as hexadecimal text.").DataType(StringType.Ascii).Metadata("readOnly", true).Build();
    private static readonly Field IdentificationField = new Field.Builder().Name(nameof(Identification)).Title(nameof(Identification)).Description("Decoded Identification payload.").DataType(AdsbIdentification.VisitorType).Metadata("readOnly", true).Build();
    private static readonly Field AirbornePositionField = new Field.Builder().Name(nameof(AirbornePosition)).Title(nameof(AirbornePosition)).Description("Decoded AirbornePosition payload.").DataType(AdsbAirbornePosition.VisitorType).Metadata("readOnly", true).Build();
    private static readonly Field SurfacePositionField = new Field.Builder().Name(nameof(SurfacePosition)).Title(nameof(SurfacePosition)).Description("Decoded SurfacePosition payload.").DataType(AdsbSurfacePosition.VisitorType).Metadata("readOnly", true).Build();
    private static readonly Field VelocityField = new Field.Builder().Name(nameof(Velocity)).Title(nameof(Velocity)).Description("Decoded Velocity payload.").DataType(AdsbVelocity.VisitorType).Metadata("readOnly", true).Build();
    internal static readonly StructType VisitorType = new([TypeCodeField, MessageElementField, IdentificationField, AirbornePositionField, SurfacePositionField, VelocityField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueTypeCode = TypeCode;
        UInt8Type.Accept(visitor, TypeCodeField, TypeCodeField.DataType, ref valueTypeCode);
        var valueMessageElement = MessageElement;
        StringType.Accept(visitor, MessageElementField, MessageElementField.DataType, ref valueMessageElement);
        if (Identification is { } valueIdentification) StructType.Accept(visitor, IdentificationField, IdentificationField.DataType, valueIdentification);
        if (AirbornePosition is { } valueAirbornePosition) StructType.Accept(visitor, AirbornePositionField, AirbornePositionField.DataType, valueAirbornePosition);
        if (SurfacePosition is { } valueSurfacePosition) StructType.Accept(visitor, SurfacePositionField, SurfacePositionField.DataType, valueSurfacePosition);
        if (Velocity is { } valueVelocity) StructType.Accept(visitor, VelocityField, VelocityField.DataType, valueVelocity);
    }
    /// <summary>Decodes the message element of a complete DF17 or supported DF18 frame. CRC validation belongs to the framing layer.</summary>
    public static AdsbExtendedSquitter Decode(ReadOnlySpan<byte> raw)
    {
        if (raw.Length != 14) throw new ArgumentException("An extended squitter frame contains 14 bytes.", nameof(raw));
        var df = raw[0] >> 3;
        var control = raw[0] & 7;
        if (df != 17 && (df != 18 || control is not (0 or 1 or 2 or 5 or 6)))
            throw new ArgumentException("The frame does not carry a supported ADS-B message element.", nameof(raw));
        var me = raw.Slice(4, 7);
        var tc = (byte)Bits(me, 0, 5);
        // TIS-B/ADS-R repurpose ADS-B flag positions for IMF and other relay semantics.
        // Retain their message element without advertising ordinary ADS-B flag meanings.
        if (df == 18 && control is 2 or 5 or 6)
            return new() { TypeCode = tc, MessageElement = Convert.ToHexString(me) };
        AdsbIdentification? identification = null;
        AdsbAirbornePosition? airborne = null;
        AdsbSurfacePosition? surface = null;
        AdsbVelocity? velocity = null;
        if (tc is >= 1 and <= 4)
        {
            Span<char> callsign = stackalloc char[8];
            for (var i = 0; i < callsign.Length; i++)
            {
                var code = Bits(me, 8 + i * 6, 6);
                callsign[i] = code switch { >= 1 and <= 26 => (char)('A' + code - 1), 32 => ' ', >= 48 and <= 57 => (char)code, _ => '#' };
            }
            identification = new() { Category = (byte)Bits(me, 5, 3), Callsign = new string(callsign).TrimEnd() };
        }
        else if (tc is >= 9 and <= 18 or >= 20 and <= 22)
        {
            var altitude = (ushort)Bits(me, 8, 12);
            airborne = new()
            {
                SurveillanceStatus = (byte)Bits(me, 5, 2), NicSupplementB = Bits(me, 7, 1) != 0,
                AltitudeCode = altitude,
                BarometricAltitudeFeet = tc <= 18 && altitude != 0 && (altitude & 0x10) != 0
                    ? (((altitude & 0xFE0) >> 1) | (altitude & 0xF)) * 25 - 1000 : null,
                GnssHeightMeters = tc >= 20 ? altitude : null,
                TimeSynchronized = Bits(me, 20, 1) != 0, IsOddCpr = Bits(me, 21, 1) != 0,
                LatitudeCpr = Bits(me, 22, 17), LongitudeCpr = Bits(me, 39, 17)
            };
        }
        else if (tc is >= 5 and <= 8)
        {
            var movement = (byte)Bits(me, 5, 7);
            var valid = Bits(me, 12, 1) != 0;
            surface = new()
            {
                MovementCode = movement, GroundSpeedIsLowerBound = movement == 124,
                GroundSpeedKnots = movement switch
                {
                    1 => 0, >= 2 and <= 8 => (movement - 1) * 0.125,
                    <= 12 and >= 9 => 1 + (movement - 9) * 0.25,
                    <= 38 and >= 13 => 2 + (movement - 13) * 0.5,
                    <= 93 and >= 39 => 15 + movement - 39,
                    <= 108 and >= 94 => 70 + (movement - 94) * 2,
                    <= 123 and >= 109 => 100 + (movement - 109) * 5,
                    124 => 175, _ => null
                },
                TrackValid = valid, TrackDegrees = valid ? Bits(me, 13, 7) * 360.0 / 128 : null,
                TimeSynchronized = Bits(me, 20, 1) != 0, IsOddCpr = Bits(me, 21, 1) != 0,
                LatitudeCpr = Bits(me, 22, 17), LongitudeCpr = Bits(me, 39, 17)
            };
        }
        else if (tc == 19)
        {
            var subtype = (byte)Bits(me, 5, 3);
            if (subtype is >= 1 and <= 4)
            {
                var scale = subtype is 2 or 4 ? 4 : 1;
                double? east = null, north = null, speed = null, track = null, heading = null, airspeed = null;
                if (subtype <= 2)
                {
                    east = SignedMagnitude(me, 13, 14, 10, scale);
                    north = SignedMagnitude(me, 24, 25, 10, scale);
                    if (east.HasValue && north.HasValue)
                    {
                        speed = Math.Sqrt(east.Value * east.Value + north.Value * north.Value);
                        if (speed != 0) track = (Math.Atan2(east.Value, north.Value) * 180 / Math.PI + 360) % 360;
                    }
                }
                else
                {
                    heading = Bits(me, 13, 1) != 0 ? Bits(me, 14, 10) * 360.0 / 1024 : null;
                    var code = Bits(me, 25, 10);
                    airspeed = code == 0 ? null : (code - 1) * scale;
                }
                velocity = new()
                {
                    Subtype = subtype, IntentChange = Bits(me, 8, 1) != 0, IfrCapability = Bits(me, 9, 1) != 0,
                    NavigationUncertainty = (byte)Bits(me, 10, 3), EastVelocityKnots = east, NorthVelocityKnots = north,
                    GroundSpeedKnots = speed, TrackDegrees = track, HeadingDegrees = heading,
                    TrueAirspeed = subtype >= 3 && Bits(me, 24, 1) != 0, AirspeedKnots = airspeed,
                    VerticalRateBarometric = Bits(me, 35, 1) != 0,
                    VerticalRateFeetPerMinute = SignedMagnitude(me, 36, 37, 9, 64),
                    GnssMinusBarometricFeet = Bits(me, 49, 7) == 127 ? null : SignedMagnitude(me, 48, 49, 7, 25)
                };
            }
        }
        return new() { TypeCode = tc, MessageElement = Convert.ToHexString(me), Identification = identification,
            AirbornePosition = airborne, SurfacePosition = surface, Velocity = velocity };
    }

    private static double? SignedMagnitude(ReadOnlySpan<byte> me, int sign, int start, int count, int scale)
    {
        var magnitude = Bits(me, start, count);
        return magnitude == 0 ? null : (double)(magnitude - 1) * scale * (Bits(me, sign, 1) == 0 ? 1 : -1);
    }

    private static uint Bits(ReadOnlySpan<byte> data, int start, int count)
    {
        uint result = 0;
        for (var i = start; i < start + count; i++) result = (result << 1) | (uint)((data[i >> 3] >> (7 - (i & 7))) & 1);
        return result;
    }

}

/// <summary>Read-only decoded ADS-B Identification fields.</summary>
public sealed class AdsbIdentification : IVisitable
{
    /// <summary>Aircraft category within the type-code category set.</summary>
    public byte Category { get; internal init; }
    /// <summary>Aircraft identification; not a unique aircraft identifier.</summary>
    public string Callsign { get; internal init; } = string.Empty;

    private static readonly Field CategoryField = new Field.Builder().Name(nameof(Category)).Title(nameof(Category)).Description("Aircraft category within the type-code category set.").DataType(UInt8Type.Default).Metadata("readOnly", true).Build();
    private static readonly Field CallsignField = new Field.Builder().Name(nameof(Callsign)).Title(nameof(Callsign)).Description("Aircraft identification; not a unique aircraft identifier.").DataType(StringType.Ascii).Metadata("readOnly", true).Build();
    internal static readonly StructType VisitorType = new([CategoryField, CallsignField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueCategory = Category;
        UInt8Type.Accept(visitor, CategoryField, CategoryField.DataType, ref valueCategory);
        var valueCallsign = Callsign;
        StringType.Accept(visitor, CallsignField, CallsignField.DataType, ref valueCallsign);
    }
}

/// <summary>Read-only decoded ADS-B AirbornePosition fields.</summary>
public sealed class AdsbAirbornePosition : IVisitable
{
    /// <summary>Surveillance status bits.</summary>
    public byte SurveillanceStatus { get; internal init; }
    /// <summary>Navigation integrity category supplement B (ADS-B v1/v2), or single-antenna flag (v0).</summary>
    public bool NicSupplementB { get; internal init; }
    /// <summary>Unmodified 12-bit altitude code. Barometric Q=0 Gillham decoding is not supported.</summary>
    public ushort AltitudeCode { get; internal init; }
    /// <summary>Pressure altitude; unavailable for zero code or unsupported Gillham encoding.</summary>
    public double? BarometricAltitudeFeet { get; internal init; }
    /// <summary>Geometric GNSS height; distinct from pressure altitude.</summary>
    public double? GnssHeightMeters { get; internal init; }
    /// <summary>Time synchronization bit.</summary>
    public bool TimeSynchronized { get; internal init; }
    /// <summary>Odd CPR frame flag. Coordinates require a reference or paired frame.</summary>
    public bool IsOddCpr { get; internal init; }
    /// <summary>Unmodified encoded CPR latitude; not a geographic coordinate.</summary>
    public uint LatitudeCpr { get; internal init; }
    /// <summary>Unmodified encoded CPR longitude; not a geographic coordinate.</summary>
    public uint LongitudeCpr { get; internal init; }

    private static readonly Field SurveillanceStatusField = new Field.Builder().Name(nameof(SurveillanceStatus)).Title(nameof(SurveillanceStatus)).Description("Surveillance status bits.").DataType(UInt8Type.Default).Metadata("readOnly", true).Build();
    private static readonly Field NicSupplementBField = new Field.Builder().Name(nameof(NicSupplementB)).Title(nameof(NicSupplementB)).Description("Navigation integrity category supplement B (ADS-B v1/v2), or single-antenna flag (v0).").DataType(BoolType.Default).Metadata("readOnly", true).Build();
    private static readonly Field AltitudeCodeField = new Field.Builder().Name(nameof(AltitudeCode)).Title(nameof(AltitudeCode)).Description("Unmodified 12-bit altitude code. Barometric Q=0 Gillham decoding is not supported.").DataType(UInt16Type.Default).Metadata("readOnly", true).Build();
    private static readonly Field BarometricAltitudeFeetField = new Field.Builder().Name(nameof(BarometricAltitudeFeet)).Title(nameof(BarometricAltitudeFeet)).Description("Pressure altitude; unavailable for zero code or unsupported Gillham encoding.").DataType(DoubleOptionalType.Default).Metadata("readOnly", true).Units("ft").Build();
    private static readonly Field GnssHeightMetersField = new Field.Builder().Name(nameof(GnssHeightMeters)).Title(nameof(GnssHeightMeters)).Description("Geometric GNSS height; distinct from pressure altitude.").DataType(DoubleOptionalType.Default).Metadata("readOnly", true).Units("m").Build();
    private static readonly Field TimeSynchronizedField = new Field.Builder().Name(nameof(TimeSynchronized)).Title(nameof(TimeSynchronized)).Description("Time synchronization bit.").DataType(BoolType.Default).Metadata("readOnly", true).Build();
    private static readonly Field IsOddCprField = new Field.Builder().Name(nameof(IsOddCpr)).Title(nameof(IsOddCpr)).Description("Odd CPR frame flag. Coordinates require a reference or paired frame.").DataType(BoolType.Default).Metadata("readOnly", true).Build();
    private static readonly Field LatitudeCprField = new Field.Builder().Name(nameof(LatitudeCpr)).Title(nameof(LatitudeCpr)).Description("Unmodified encoded CPR latitude; not a geographic coordinate.").DataType(UInt32Type.Default).Metadata("readOnly", true).Build();
    private static readonly Field LongitudeCprField = new Field.Builder().Name(nameof(LongitudeCpr)).Title(nameof(LongitudeCpr)).Description("Unmodified encoded CPR longitude; not a geographic coordinate.").DataType(UInt32Type.Default).Metadata("readOnly", true).Build();
    internal static readonly StructType VisitorType = new([SurveillanceStatusField, NicSupplementBField, AltitudeCodeField, BarometricAltitudeFeetField, GnssHeightMetersField, TimeSynchronizedField, IsOddCprField, LatitudeCprField, LongitudeCprField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueSurveillanceStatus = SurveillanceStatus;
        UInt8Type.Accept(visitor, SurveillanceStatusField, SurveillanceStatusField.DataType, ref valueSurveillanceStatus);
        var valueNicSupplementB = NicSupplementB;
        BoolType.Accept(visitor, NicSupplementBField, NicSupplementBField.DataType, ref valueNicSupplementB);
        var valueAltitudeCode = AltitudeCode;
        UInt16Type.Accept(visitor, AltitudeCodeField, AltitudeCodeField.DataType, ref valueAltitudeCode);
        var valueBarometricAltitudeFeet = BarometricAltitudeFeet;
        DoubleOptionalType.Accept(visitor, BarometricAltitudeFeetField, BarometricAltitudeFeetField.DataType, ref valueBarometricAltitudeFeet);
        var valueGnssHeightMeters = GnssHeightMeters;
        DoubleOptionalType.Accept(visitor, GnssHeightMetersField, GnssHeightMetersField.DataType, ref valueGnssHeightMeters);
        var valueTimeSynchronized = TimeSynchronized;
        BoolType.Accept(visitor, TimeSynchronizedField, TimeSynchronizedField.DataType, ref valueTimeSynchronized);
        var valueIsOddCpr = IsOddCpr;
        BoolType.Accept(visitor, IsOddCprField, IsOddCprField.DataType, ref valueIsOddCpr);
        var valueLatitudeCpr = LatitudeCpr;
        UInt32Type.Accept(visitor, LatitudeCprField, LatitudeCprField.DataType, ref valueLatitudeCpr);
        var valueLongitudeCpr = LongitudeCpr;
        UInt32Type.Accept(visitor, LongitudeCprField, LongitudeCprField.DataType, ref valueLongitudeCpr);
    }
}

/// <summary>Read-only decoded ADS-B SurfacePosition fields.</summary>
public sealed class AdsbSurfacePosition : IVisitable
{
    /// <summary>Unmodified movement code; 0 unavailable, 125 through 127 reserved.</summary>
    public byte MovementCode { get; internal init; }
    /// <summary>Lower edge of the movement quantization interval; code 124 means at least 175 knots.</summary>
    public double? GroundSpeedKnots { get; internal init; }
    /// <summary>True for movement code 124, which has no finite upper bound.</summary>
    public bool GroundSpeedIsLowerBound { get; internal init; }
    /// <summary>Validity of the ground track.</summary>
    public bool TrackValid { get; internal init; }
    /// <summary>Ground track clockwise from true north when valid.</summary>
    public double? TrackDegrees { get; internal init; }
    /// <summary>Time synchronization bit.</summary>
    public bool TimeSynchronized { get; internal init; }
    /// <summary>Odd CPR frame flag; position needs a reference.</summary>
    public bool IsOddCpr { get; internal init; }
    /// <summary>Unmodified encoded CPR latitude.</summary>
    public uint LatitudeCpr { get; internal init; }
    /// <summary>Unmodified encoded CPR longitude.</summary>
    public uint LongitudeCpr { get; internal init; }

    private static readonly Field MovementCodeField = new Field.Builder().Name(nameof(MovementCode)).Title(nameof(MovementCode)).Description("Unmodified movement code; 0 unavailable, 125 through 127 reserved.").DataType(UInt8Type.Default).Metadata("readOnly", true).Build();
    private static readonly Field GroundSpeedKnotsField = new Field.Builder().Name(nameof(GroundSpeedKnots)).Title(nameof(GroundSpeedKnots)).Description("Lower edge of the movement quantization interval; code 124 means at least 175 knots.").DataType(DoubleOptionalType.Default).Metadata("readOnly", true).Units("kt").Build();
    private static readonly Field GroundSpeedIsLowerBoundField = new Field.Builder().Name(nameof(GroundSpeedIsLowerBound)).Title(nameof(GroundSpeedIsLowerBound)).Description("True for movement code 124, which has no finite upper bound.").DataType(BoolType.Default).Metadata("readOnly", true).Build();
    private static readonly Field TrackValidField = new Field.Builder().Name(nameof(TrackValid)).Title(nameof(TrackValid)).Description("Validity of the ground track.").DataType(BoolType.Default).Metadata("readOnly", true).Build();
    private static readonly Field TrackDegreesField = new Field.Builder().Name(nameof(TrackDegrees)).Title(nameof(TrackDegrees)).Description("Ground track clockwise from true north when valid.").DataType(DoubleOptionalType.Default).Metadata("readOnly", true).Units("deg").Build();
    private static readonly Field TimeSynchronizedField = new Field.Builder().Name(nameof(TimeSynchronized)).Title(nameof(TimeSynchronized)).Description("Time synchronization bit.").DataType(BoolType.Default).Metadata("readOnly", true).Build();
    private static readonly Field IsOddCprField = new Field.Builder().Name(nameof(IsOddCpr)).Title(nameof(IsOddCpr)).Description("Odd CPR frame flag; position needs a reference.").DataType(BoolType.Default).Metadata("readOnly", true).Build();
    private static readonly Field LatitudeCprField = new Field.Builder().Name(nameof(LatitudeCpr)).Title(nameof(LatitudeCpr)).Description("Unmodified encoded CPR latitude.").DataType(UInt32Type.Default).Metadata("readOnly", true).Build();
    private static readonly Field LongitudeCprField = new Field.Builder().Name(nameof(LongitudeCpr)).Title(nameof(LongitudeCpr)).Description("Unmodified encoded CPR longitude.").DataType(UInt32Type.Default).Metadata("readOnly", true).Build();
    internal static readonly StructType VisitorType = new([MovementCodeField, GroundSpeedKnotsField, GroundSpeedIsLowerBoundField, TrackValidField, TrackDegreesField, TimeSynchronizedField, IsOddCprField, LatitudeCprField, LongitudeCprField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueMovementCode = MovementCode;
        UInt8Type.Accept(visitor, MovementCodeField, MovementCodeField.DataType, ref valueMovementCode);
        var valueGroundSpeedKnots = GroundSpeedKnots;
        DoubleOptionalType.Accept(visitor, GroundSpeedKnotsField, GroundSpeedKnotsField.DataType, ref valueGroundSpeedKnots);
        var valueGroundSpeedIsLowerBound = GroundSpeedIsLowerBound;
        BoolType.Accept(visitor, GroundSpeedIsLowerBoundField, GroundSpeedIsLowerBoundField.DataType, ref valueGroundSpeedIsLowerBound);
        var valueTrackValid = TrackValid;
        BoolType.Accept(visitor, TrackValidField, TrackValidField.DataType, ref valueTrackValid);
        var valueTrackDegrees = TrackDegrees;
        DoubleOptionalType.Accept(visitor, TrackDegreesField, TrackDegreesField.DataType, ref valueTrackDegrees);
        var valueTimeSynchronized = TimeSynchronized;
        BoolType.Accept(visitor, TimeSynchronizedField, TimeSynchronizedField.DataType, ref valueTimeSynchronized);
        var valueIsOddCpr = IsOddCpr;
        BoolType.Accept(visitor, IsOddCprField, IsOddCprField.DataType, ref valueIsOddCpr);
        var valueLatitudeCpr = LatitudeCpr;
        UInt32Type.Accept(visitor, LatitudeCprField, LatitudeCprField.DataType, ref valueLatitudeCpr);
        var valueLongitudeCpr = LongitudeCpr;
        UInt32Type.Accept(visitor, LongitudeCprField, LongitudeCprField.DataType, ref valueLongitudeCpr);
    }
}

/// <summary>Read-only decoded ADS-B Velocity fields.</summary>
public sealed class AdsbVelocity : IVisitable
{
    /// <summary>Velocity subtype: 1/2 ground vector, 3/4 airspeed.</summary>
    public byte Subtype { get; internal init; }
    /// <summary>Intent change flag.</summary>
    public bool IntentChange { get; internal init; }
    /// <summary>IFR capability flag.</summary>
    public bool IfrCapability { get; internal init; }
    /// <summary>Raw velocity navigation uncertainty category.</summary>
    public byte NavigationUncertainty { get; internal init; }
    /// <summary>Signed eastward ground velocity; zero code is unavailable.</summary>
    public double? EastVelocityKnots { get; internal init; }
    /// <summary>Signed northward ground velocity; zero code is unavailable.</summary>
    public double? NorthVelocityKnots { get; internal init; }
    /// <summary>Magnitude of available ground vector components.</summary>
    public double? GroundSpeedKnots { get; internal init; }
    /// <summary>Ground track clockwise from true north; undefined for a zero vector.</summary>
    public double? TrackDegrees { get; internal init; }
    /// <summary>Magnetic heading when the heading status is valid.</summary>
    public double? HeadingDegrees { get; internal init; }
    /// <summary>True for true airspeed, false for indicated airspeed; applies to subtypes 3 and 4.</summary>
    public bool TrueAirspeed { get; internal init; }
    /// <summary>Indicated or true airspeed; zero code is unavailable.</summary>
    public double? AirspeedKnots { get; internal init; }
    /// <summary>True for barometric source, false for GNSS source.</summary>
    public bool VerticalRateBarometric { get; internal init; }
    /// <summary>Vertical rate positive upwards; zero code is unavailable.</summary>
    public double? VerticalRateFeetPerMinute { get; internal init; }
    /// <summary>GNSS height minus pressure altitude; magnitude codes 0 and 127 are unavailable.</summary>
    public double? GnssMinusBarometricFeet { get; internal init; }

    private static readonly Field SubtypeField = new Field.Builder().Name(nameof(Subtype)).Title(nameof(Subtype)).Description("Velocity subtype: 1/2 ground vector, 3/4 airspeed.").DataType(UInt8Type.Default).Metadata("readOnly", true).Build();
    private static readonly Field IntentChangeField = new Field.Builder().Name(nameof(IntentChange)).Title(nameof(IntentChange)).Description("Intent change flag.").DataType(BoolType.Default).Metadata("readOnly", true).Build();
    private static readonly Field IfrCapabilityField = new Field.Builder().Name(nameof(IfrCapability)).Title(nameof(IfrCapability)).Description("IFR capability flag.").DataType(BoolType.Default).Metadata("readOnly", true).Build();
    private static readonly Field NavigationUncertaintyField = new Field.Builder().Name(nameof(NavigationUncertainty)).Title(nameof(NavigationUncertainty)).Description("Raw velocity navigation uncertainty category.").DataType(UInt8Type.Default).Metadata("readOnly", true).Build();
    private static readonly Field EastVelocityKnotsField = new Field.Builder().Name(nameof(EastVelocityKnots)).Title(nameof(EastVelocityKnots)).Description("Signed eastward ground velocity; zero code is unavailable.").DataType(DoubleOptionalType.Default).Metadata("readOnly", true).Units("kt").Build();
    private static readonly Field NorthVelocityKnotsField = new Field.Builder().Name(nameof(NorthVelocityKnots)).Title(nameof(NorthVelocityKnots)).Description("Signed northward ground velocity; zero code is unavailable.").DataType(DoubleOptionalType.Default).Metadata("readOnly", true).Units("kt").Build();
    private static readonly Field GroundSpeedKnotsField = new Field.Builder().Name(nameof(GroundSpeedKnots)).Title(nameof(GroundSpeedKnots)).Description("Magnitude of available ground vector components.").DataType(DoubleOptionalType.Default).Metadata("readOnly", true).Units("kt").Build();
    private static readonly Field TrackDegreesField = new Field.Builder().Name(nameof(TrackDegrees)).Title(nameof(TrackDegrees)).Description("Ground track clockwise from true north; undefined for a zero vector.").DataType(DoubleOptionalType.Default).Metadata("readOnly", true).Units("deg").Build();
    private static readonly Field HeadingDegreesField = new Field.Builder().Name(nameof(HeadingDegrees)).Title(nameof(HeadingDegrees)).Description("Magnetic heading when the heading status is valid.").DataType(DoubleOptionalType.Default).Metadata("readOnly", true).Units("deg").Build();
    private static readonly Field TrueAirspeedField = new Field.Builder().Name(nameof(TrueAirspeed)).Title(nameof(TrueAirspeed)).Description("True for true airspeed, false for indicated airspeed; applies to subtypes 3 and 4.").DataType(BoolType.Default).Metadata("readOnly", true).Build();
    private static readonly Field AirspeedKnotsField = new Field.Builder().Name(nameof(AirspeedKnots)).Title(nameof(AirspeedKnots)).Description("Indicated or true airspeed; zero code is unavailable.").DataType(DoubleOptionalType.Default).Metadata("readOnly", true).Units("kt").Build();
    private static readonly Field VerticalRateBarometricField = new Field.Builder().Name(nameof(VerticalRateBarometric)).Title(nameof(VerticalRateBarometric)).Description("True for barometric source, false for GNSS source.").DataType(BoolType.Default).Metadata("readOnly", true).Build();
    private static readonly Field VerticalRateFeetPerMinuteField = new Field.Builder().Name(nameof(VerticalRateFeetPerMinute)).Title(nameof(VerticalRateFeetPerMinute)).Description("Vertical rate positive upwards; zero code is unavailable.").DataType(DoubleOptionalType.Default).Metadata("readOnly", true).Units("ft/min").Build();
    private static readonly Field GnssMinusBarometricFeetField = new Field.Builder().Name(nameof(GnssMinusBarometricFeet)).Title(nameof(GnssMinusBarometricFeet)).Description("GNSS height minus pressure altitude; magnitude codes 0 and 127 are unavailable.").DataType(DoubleOptionalType.Default).Metadata("readOnly", true).Units("ft").Build();
    internal static readonly StructType VisitorType = new([SubtypeField, IntentChangeField, IfrCapabilityField, NavigationUncertaintyField, EastVelocityKnotsField, NorthVelocityKnotsField, GroundSpeedKnotsField, TrackDegreesField, HeadingDegreesField, TrueAirspeedField, AirspeedKnotsField, VerticalRateBarometricField, VerticalRateFeetPerMinuteField, GnssMinusBarometricFeetField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueSubtype = Subtype;
        UInt8Type.Accept(visitor, SubtypeField, SubtypeField.DataType, ref valueSubtype);
        var valueIntentChange = IntentChange;
        BoolType.Accept(visitor, IntentChangeField, IntentChangeField.DataType, ref valueIntentChange);
        var valueIfrCapability = IfrCapability;
        BoolType.Accept(visitor, IfrCapabilityField, IfrCapabilityField.DataType, ref valueIfrCapability);
        var valueNavigationUncertainty = NavigationUncertainty;
        UInt8Type.Accept(visitor, NavigationUncertaintyField, NavigationUncertaintyField.DataType, ref valueNavigationUncertainty);
        var valueEastVelocityKnots = EastVelocityKnots;
        DoubleOptionalType.Accept(visitor, EastVelocityKnotsField, EastVelocityKnotsField.DataType, ref valueEastVelocityKnots);
        var valueNorthVelocityKnots = NorthVelocityKnots;
        DoubleOptionalType.Accept(visitor, NorthVelocityKnotsField, NorthVelocityKnotsField.DataType, ref valueNorthVelocityKnots);
        var valueGroundSpeedKnots = GroundSpeedKnots;
        DoubleOptionalType.Accept(visitor, GroundSpeedKnotsField, GroundSpeedKnotsField.DataType, ref valueGroundSpeedKnots);
        var valueTrackDegrees = TrackDegrees;
        DoubleOptionalType.Accept(visitor, TrackDegreesField, TrackDegreesField.DataType, ref valueTrackDegrees);
        var valueHeadingDegrees = HeadingDegrees;
        DoubleOptionalType.Accept(visitor, HeadingDegreesField, HeadingDegreesField.DataType, ref valueHeadingDegrees);
        var valueTrueAirspeed = TrueAirspeed;
        BoolType.Accept(visitor, TrueAirspeedField, TrueAirspeedField.DataType, ref valueTrueAirspeed);
        var valueAirspeedKnots = AirspeedKnots;
        DoubleOptionalType.Accept(visitor, AirspeedKnotsField, AirspeedKnotsField.DataType, ref valueAirspeedKnots);
        var valueVerticalRateBarometric = VerticalRateBarometric;
        BoolType.Accept(visitor, VerticalRateBarometricField, VerticalRateBarometricField.DataType, ref valueVerticalRateBarometric);
        var valueVerticalRateFeetPerMinute = VerticalRateFeetPerMinute;
        DoubleOptionalType.Accept(visitor, VerticalRateFeetPerMinuteField, VerticalRateFeetPerMinuteField.DataType, ref valueVerticalRateFeetPerMinute);
        var valueGnssMinusBarometricFeet = GnssMinusBarometricFeet;
        DoubleOptionalType.Accept(visitor, GnssMinusBarometricFeetField, GnssMinusBarometricFeetField.DataType, ref valueGnssMinusBarometricFeet);
    }
}
