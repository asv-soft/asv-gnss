using System;
using Asv.IO;
using Geodesy;

namespace Asv.Gnss;

public class UbxNavPvtPool : UbxMessageBase
{
    public override string Name => "UBX-NAV-PVT-POOL";
    public override byte Class => 0x01;
    public override byte SubClass => 0x07;

    protected override void SerializeContent(ref Span<byte> buffer)
    {

    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
    }

    protected override int GetContentByteSize() => 0;


    public override void Randomize(Random random)
    {
    }
}

// [SerializationNotSupported]
/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.17.17. Receiver and firmware support must be checked against the applicable interface description.</remarks>
/// <remarks>See UBX-18010802 R08, sections 3.1.7 and 3.10.5, for fix and time validity. A fix type or an RTK ambiguity status alone does not establish a valid position.</remarks>
public partial class UbxNavPvt : UbxMessageBase
{
    private double _magneticDeclinationAccuracy;
    private double _magneticDeclination;
    private double _headingOfVehicle2D;
    private bool _isValidLLH;
    private ushort _flags3;
    private double _positionDOP;
    private double _headingAccuracyEstimate;
    private double _speedAccuracyEstimate;
    private double _headingOfMotion2D;
    private double _groundSpeed2D;
    private double _velocityDown;
    private double _velocityEast;
    private double _velocityNorth;
    private double _verticalAccuracyEstimate;
    private double _horizontalAccuracyEstimate;
    private double _altMsl;
    private double _altElipsoid;
    private double _latitude;
    private double _longitude;
    private byte _numberOfSatellites;
    private bool _utcConfirmedTime;
    private bool _utcConfirmedDate;
    private bool _utcConfirmedAvailable;
    private byte _psmState;
    private UbxCarrierSolutionStatus _carrierSolution;
    private bool _gnssFixOK;
    private bool _isAppliedDifferentialCorrections;
    private bool _isValidVehicleHeading;
    private UbxGnssFixType _fixType;
    private double _utcFractionOfSecond;
    private double _utcTimeAccuracyEstimate;
    private bool _isValidMagneticDeclination;
    private bool _utcTimeOfDayIsFullyResolved;
    private bool _utcTimeIsConfirmation;
    private bool _utcDateIsConfirmation;
    private byte _sec;
    private byte _min;
    private byte _hour;
    private byte _day;
    private byte _month;
    private ushort _year;
    private ulong _iTOW;
    private GlobalPosition _movingBaseLocation;
    private DateTime _gpsTime;
    private DateTime _utcTime;

    public override string Name => "UBX-NAV-PVT";
    public override byte Class => 0x01;
    public override byte SubClass => 0x07;

    /// <summary>
    /// Magnetic declination accuracy. Only supported in ADR 4.10 and later. deg
    /// </summary>
    public double MagneticDeclinationAccuracy
    {
        get => _magneticDeclinationAccuracy;
        set => _magneticDeclinationAccuracy = value;
    }

    /// <summary>
    /// Magnetic declination. Only supported in ADR 4.10 and later. deg
    /// </summary>
    public double MagneticDeclination
    {
        get => _magneticDeclination;
        set => _magneticDeclination = value;
    }

    /// <summary>
    /// Vehicle heading in degrees. This deserializer sets the property to NaN when IsValidVehicleHeading is false.
    /// </summary>
    public double HeadingOfVehicle2D
    {
        get => _headingOfVehicle2D;
        set => _headingOfVehicle2D = value;
    }

    /// <summary>
    /// Whether longitude, latitude and both height fields are valid; this is the inverse of the wire invalidLlh bit.
    /// </summary>
    public bool IsValidLLH
    {
        get => _isValidLLH;
        set => _isValidLLH = value;
    }

    /// <summary>
    /// Raw additional navigation flags. X20 HPG 2.11 defines invalidLlh, lastCorrectionAge,
    /// authTime and nmaFixStatus; unused bits are retained without interpretation.
    /// </summary>
    public ushort Flags3
    {
        get => _flags3;
        set => _flags3 = value;
    }

    /// <summary>
    /// Differential-correction age bucket, not seconds: 0 means unavailable, 1 through 11
    /// represent increasing intervals, and 12 through 15 mean at least 120 seconds.
    /// </summary>
    public byte LastCorrectionAge
    {
        get => (byte)((_flags3 >> 1) & 0x0F);
        set
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, (byte)15);
            _flags3 = (ushort)((_flags3 & ~0x001E) | (value << 1));
        }
    }

    /// <summary>Whether the output time has been validated against an external trusted time source.</summary>
    public bool IsTimeAuthenticated
    {
        get => (_flags3 & 0x2000) != 0;
        set => _flags3 = value ? (ushort)(_flags3 | 0x2000) : (ushort)(_flags3 & ~0x2000);
    }

    /// <summary>
    /// Whether the mixed navigation solution agrees with NMA-authenticated data.
    /// A false value alone does not establish a spoofing attack; consult UBX-SEC-SIG.
    /// </summary>
    public bool IsNmaFixVerified
    {
        get => (_flags3 & 0x4000) != 0;
        set => _flags3 = value ? (ushort)(_flags3 | 0x4000) : (ushort)(_flags3 & ~0x4000);
    }

    /// <summary>
    /// Position dilution of precision, dimensionless; wire resolution is 0.01.
    /// </summary>
    public double PositionDOP
    {
        get => _positionDOP;
        set => _positionDOP = value;
    }


    /// <summary>
    /// Heading accuracy estimate (both motion and vehicle), deg
    /// </summary>
    public double HeadingAccuracyEstimate
    {
        get => _headingAccuracyEstimate;
        set => _headingAccuracyEstimate = value;
    }


    /// <summary>
    /// Speed accuracy estimate, m\s
    /// </summary>
    public double SpeedAccuracyEstimate
    {
        get => _speedAccuracyEstimate;
        set => _speedAccuracyEstimate = value;
    }

    /// <summary>
    /// Heading of motion (2-D), deg
    /// </summary>
    public double HeadingOfMotion2D
    {
        get => _headingOfMotion2D;
        set => _headingOfMotion2D = value;
    }


    /// <summary>
    /// Ground Speed (2-D), m\s
    /// </summary>
    public double GroundSpeed2D
    {
        get => _groundSpeed2D;
        set => _groundSpeed2D = value;
    }

    /// <summary>
    /// NED down velocity, m\s
    /// </summary>
    public double VelocityDown
    {
        get => _velocityDown;
        set => _velocityDown = value;
    }

    /// <summary>
    /// NED east velocity, m\s
    /// </summary>
    public double VelocityEast
    {
        get => _velocityEast;
        set => _velocityEast = value;
    }

    /// <summary>
    /// NED north velocity, m\s
    /// </summary>
    public double VelocityNorth
    {
        get => _velocityNorth;
        set => _velocityNorth = value;
    }

    /// <summary>
    /// Vertical accuracy estimate, m
    /// </summary>
    public double VerticalAccuracyEstimate
    {
        get => _verticalAccuracyEstimate;
        set => _verticalAccuracyEstimate = value;
    }

    /// <summary>
    /// Horizontal accuracy estimate, m
    /// </summary>
    public double HorizontalAccuracyEstimate
    {
        get => _horizontalAccuracyEstimate;
        set => _horizontalAccuracyEstimate = value;
    }

    /// <summary>
    /// Height above mean sea level, m
    /// </summary>
    public double AltMsl
    {
        get => _altMsl;
        set => _altMsl = value;
    }

    /// <summary>
    /// Height above ellipsoid, m
    /// </summary>
    public double AltElipsoid
    {
        get => _altElipsoid;
        set => _altElipsoid = value;
    }

    /// <summary>
    /// WGS84 geodetic latitude in degrees; converted from the wire scale of 0.0000001 degree.
    /// </summary>
    public double Latitude
    {
        get => _latitude;
        set => _latitude = value;
    }

    /// <summary>
    /// WGS84 geodetic longitude in degrees; converted from the wire scale of 0.0000001 degree.
    /// </summary>
    public double Longitude
    {
        get => _longitude;
        set => _longitude = value;
    }

    /// <summary>
    /// Number of satellites used in Nav Solution
    /// </summary>
    public byte NumberOfSatellites
    {
        get => _numberOfSatellites;
        set => _numberOfSatellites = value;
    }


    /// <summary>
    /// UTC Time of Day could be confirmed (see Time Validity section for details)
    /// </summary>
    public bool UTCConfirmedTime
    {
        get => _utcConfirmedTime;
        set => _utcConfirmedTime = value;
    }

    /// <summary>
    /// UTC Date validity could be confirmed (see Time Validity section for details)
    /// </summary>
    public bool UTCConfirmedDate
    {
        get => _utcConfirmedDate;
        set => _utcConfirmedDate = value;
    }

    /// <summary>
    /// 1 = information about UTC Date and Time of Day validity confirmation is available (see Time Validity section for details)
    /// This flag is only supported in Protocol Versions 19.00, 19.10, 20.10, 20.20, 20.30, 22.00, 23.00, 23.01, 27 and 28
    /// </summary>
    public bool UTCConfirmedAvailable
    {
        get => _utcConfirmedAvailable;
        set => _utcConfirmedAvailable = value;
    }

    /// <summary>
    /// Power-save state from flags bits 2 through 4; the R23 NAV-PVT field table does not define these bits, so interpret only with the applicable firmware interface description.
    /// </summary>
    public byte PsmState
    {
        get => _psmState;
        set => _psmState = value;
    }


    /// <summary>
    /// Carrier-phase ambiguity status: unavailable, float, or fixed. This status alone does not establish a valid navigation fix.
    /// </summary>
    public UbxCarrierSolutionStatus CarrierSolution
    {
        get => _carrierSolution;
        set => _carrierSolution = value;
    }

    /// <summary>
    /// Whether the solution satisfies the receiver DOP and accuracy masks. Check this flag as well as FixType before using the position.
    /// </summary>
    public bool GnssFixOK
    {
        get => _gnssFixOK;
        set => _gnssFixOK = value;
    }

    /// <summary>
    /// Differential corrections were applied
    /// </summary>
    public bool IsAppliedDifferentialCorrections
    {
        get => _isAppliedDifferentialCorrections;
        set => _isAppliedDifferentialCorrections = value;
    }

    /// <summary>
    /// Whether the vehicle heading is valid (headVehValid); this is separate from differential-correction status.
    /// </summary>
    public bool IsValidVehicleHeading
    {
        get => _isValidVehicleHeading;
        set => _isValidVehicleHeading = value;
    }



    /// <summary>
    /// GNSSfix Type
    /// </summary>
    public UbxGnssFixType FixType
    {
        get => _fixType;
        set => _fixType = value;
    }

    /// <summary>
    /// Signed UTC fractional-second correction in nanoseconds, from -1000000000 to 1000000000; this property retains the unscaled wire value.
    /// </summary>
    public double UTCFractionOfSecond
    {
        get => _utcFractionOfSecond;
        set => _utcFractionOfSecond = value;
    }

    /// <summary>
    /// Estimated UTC time accuracy in seconds; converted from the wire nanosecond value.
    /// </summary>
    public double UTCTimeAccuracyEstimate
    {
        get => _utcTimeAccuracyEstimate;
        set => _utcTimeAccuracyEstimate = value;
    }

    /// <summary>
    /// valid magnetic declination
    /// </summary>
    public bool IsValidMagneticDeclination
    {
        get => _isValidMagneticDeclination;
        set => _isValidMagneticDeclination = value;
    }

    /// <summary>
    /// Whether UTC time has no whole-second ambiguity; also check the date and time validity flags.
    /// </summary>
    public bool UTCTimeOfDayIsFullyResolved
    {
        get => _utcTimeOfDayIsFullyResolved;
        set => _utcTimeOfDayIsFullyResolved = value;
    }

    /// <summary>
    /// valid UTC time of day (see Time Validity section for details)
    /// </summary>
    public bool UTCTimeIsConfirmation
    {
        get => _utcTimeIsConfirmation;
        set => _utcTimeIsConfirmation = value;
    }

    /// <summary>
    /// valid UTC Date (see Time Validity section for details)
    /// </summary>
    public bool UTCDateIsConfirmation
    {
        get => _utcDateIsConfirmation;
        set => _utcDateIsConfirmation = value;
    }

    /// <summary>
    /// Seconds of minute, range 0..60 (UTC)
    /// </summary>
    public byte Sec
    {
        get => _sec;
        set => _sec = value;
    }

    /// <summary>
    /// Minute of hour, range 0..59 (UTC)
    /// </summary>
    public byte Min
    {
        get => _min;
        set => _min = value;
    }

    /// <summary>
    /// Hour of day, range 0..23 (UTC)
    /// </summary>
    public byte Hour
    {
        get => _hour;
        set => _hour = value;
    }

    /// <summary>
    /// Day of month, range 1..31 (UTC)
    /// </summary>
    public byte Day
    {
        get => _day;
        set => _day = value;
    }

    /// <summary>
    /// Month, range 1..12 (UTC)
    /// </summary>
    public byte Month
    {
        get => _month;
        set => _month = value;
    }

    /// <summary>
    /// Year (UTC)
    /// </summary>
    public ushort Year
    {
        get => _year;
        set => _year = value;
    }

    /// <summary>
    /// GPS time of week of the navigation epoch in milliseconds.
    /// </summary>
    public ulong iTOW
    {
        get => _iTOW;
        set => _iTOW = value;
    }

    /// <summary>
    /// Derived geodetic position in degrees and meters above the ellipsoid. Updated when GnssFixOK is set and FixType is at least Fix3D; otherwise the previous value is retained.
    /// </summary>
    public GlobalPosition MovingBaseLocation
    {
        get => _movingBaseLocation;
        set => _movingBaseLocation = value;
    }

    protected override void SerializeContent(ref Span<byte> buffer)
    {
        throw new NotImplementedException();
    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        iTOW = BinSerialize.ReadUInt(ref buffer);
        Year = BinSerialize.ReadUShort(ref buffer);
        Month = BinSerialize.ReadByte(ref buffer);
        Day = BinSerialize.ReadByte(ref buffer);
        Hour = BinSerialize.ReadByte(ref buffer);
        Min = BinSerialize.ReadByte(ref buffer);
        Sec = BinSerialize.ReadByte(ref buffer);
        var valid = BinSerialize.ReadByte(ref buffer);
        //UTC Date and Time Confirmation Status   Date: CONFIRMED, Time: CONFIRMED

        UTCDateIsConfirmation = (valid & 0b0000_0001) != 0;
        UTCTimeIsConfirmation = (valid & 0b0000_0010) != 0;
        UTCTimeOfDayIsFullyResolved = (valid & 0b0000_0100) != 0;
        IsValidMagneticDeclination = (valid & 0b0000_1000) != 0;


        UTCTimeAccuracyEstimate = BinSerialize.ReadUInt(ref buffer) * 1e-9;
        UTCFractionOfSecond = BinSerialize.ReadInt(ref buffer);
        FixType = (UbxGnssFixType)BinSerialize.ReadByte(ref buffer);

        var flags = BinSerialize.ReadByte(ref buffer);

        GnssFixOK = (flags & 0b0000_0001) != 0;
        IsAppliedDifferentialCorrections = (flags & 0b0000_0010) != 0;
        IsValidVehicleHeading = (flags & 0b0010_0000) != 0;
        PsmState = (byte)((flags & 0b0001_1100) >> 2);
        CarrierSolution = (UbxCarrierSolutionStatus)((flags & 0b1100_0000) >> 6);

        flags = BinSerialize.ReadByte(ref buffer);
        UTCConfirmedAvailable = (flags & 0b0010_0000) != 0;
        UTCConfirmedDate = (flags & 0b0100_0000) != 0;
        UTCConfirmedTime = (flags & 0b1000_0000) != 0;
        //
        NumberOfSatellites = BinSerialize.ReadByte(ref buffer);
        Longitude = BinSerialize.ReadInt(ref buffer) * 1e-7;
        Latitude = BinSerialize.ReadInt(ref buffer) * 1e-7;
        AltElipsoid = BinSerialize.ReadInt(ref buffer) * 0.001;
        AltMsl = BinSerialize.ReadInt(ref buffer) * 0.001;
        HorizontalAccuracyEstimate = BinSerialize.ReadUInt(ref buffer) * 0.001;
        VerticalAccuracyEstimate = BinSerialize.ReadUInt(ref buffer) * 0.001;
        VelocityNorth = BinSerialize.ReadInt(ref buffer) * 0.001;
        VelocityEast = BinSerialize.ReadInt(ref buffer) * 0.001;
        VelocityDown = BinSerialize.ReadInt(ref buffer) * 0.001;
        GroundSpeed2D = BinSerialize.ReadInt(ref buffer) * 0.001;
        HeadingOfMotion2D = BinSerialize.ReadInt(ref buffer) * 1e-5;
        SpeedAccuracyEstimate = BinSerialize.ReadUInt(ref buffer) * 0.001;
        HeadingAccuracyEstimate = BinSerialize.ReadUInt(ref buffer) * 1e-5;
        PositionDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
        Flags3 = BinSerialize.ReadUShort(ref buffer);
        IsValidLLH = (Flags3 & 0b0000_0001) == 0;
        buffer = buffer[4..];
        HeadingOfVehicle2D = BinSerialize.ReadInt(ref buffer) * 1e-5;
        if (!IsValidVehicleHeading) HeadingOfVehicle2D = double.NaN;
        MagneticDeclination = BinSerialize.ReadShort(ref buffer) * 1e-2;
        MagneticDeclinationAccuracy = BinSerialize.ReadUShort(ref buffer) * 1e-2;
        //
        if (FixType >= UbxGnssFixType.Fix3D && GnssFixOK)
        {
            MovingBaseLocation = new GlobalPosition(new GlobalCoordinates(Latitude, Longitude), AltElipsoid);
        }

        UtcTime = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);
        GpsTime = UtcTime;
        if (!UTCDateIsConfirmation || !UTCTimeIsConfirmation ||
            Year is < 1 or > 9999 || Month is < 1 or > 12 ||
            Day < 1 || Day > DateTime.DaysInMonth(Year, Month) ||
            Hour > 23 || Min > 59 || Sec > 60)
        {
            return;
        }

        try
        {
            // DateTime cannot represent the leap-second label 60, so normalize it to the next minute.
            var utcTime = new DateTime(Year, Month, Day, Hour, Min, Math.Min(Sec, (byte)59), DateTimeKind.Utc)
                .AddSeconds(Sec == 60 ? 1 : 0)
                .AddTicks((long)Math.Round(UTCFractionOfSecond / 100.0));

            var week = 0;
            var towP = 0.0;
            var tow = iTOW * 1e-3;
            GpsRawHelper.Time2Gps(GpsRawHelper.Utc2Gps(utcTime), ref week, ref towP);
            if (tow < towP - 302400.0) week += 1;
            else if (tow > towP + 302400.0) week -= 1;
            var gpsTime = GpsRawHelper.Gps2Time(week, tow);
            UtcTime = utcTime;
            GpsTime = gpsTime;
        }
        catch (ArgumentOutOfRangeException)
        {
            // An unrepresentable receiver timestamp must not discard the decoded navigation fields.
        }
    }

    /// <summary>
    /// Derived GPS time reconstructed from iTOW and the week inferred from UtcTime. DateTime.MinValue with UTC kind indicates an invalid or unrepresentable receiver date or time.
    /// </summary>
    public DateTime GpsTime
    {
        get => _gpsTime;
        set => _gpsTime = value;
    }

    /// <summary>
    /// UTC timestamp assembled from valid calendar fields and the signed nanosecond correction, rounded to 100 ns. Leap-second label 60 is normalized to the following minute. DateTime.MinValue with UTC kind indicates an invalid or unrepresentable receiver date or time; check the fully-resolved flag before precise time use.
    /// </summary>
    public DateTime UtcTime
    {
        get => _utcTime;
        set => _utcTime = value;
    }

    protected override int GetContentByteSize() => 92;


    public override void Randomize(Random random)
    {

    }
}

public enum UbxCarrierSolutionStatus
{
    /// <summary>
    /// no carrier phase range solution
    /// </summary>
    NoCarrierSolution = 0,

    /// <summary>
    /// carrier phase range solution with floating ambiguities
    /// </summary>
    FloatingAmbiguities = 1,

    /// <summary>
    /// carrier phase range solution with fixed ambiguities
    /// </summary>
    FixedAmbiguities = 2,
}

public enum UbxGnssFixType : byte
{
    NoFix = 0,
    DeadReckoningOnly = 1,
    Fix2D = 2,
    Fix3D = 3,
    GNSSDeadReckoning = 4,
    TimeOnlyFix = 5,
}
