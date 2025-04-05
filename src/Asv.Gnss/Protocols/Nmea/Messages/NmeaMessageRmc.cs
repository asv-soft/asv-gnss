using System;

namespace Asv.Gnss;

/// <summary>
/// [RMC] GPS specific information
/// https://receiverhelp.trimble.com/alloy-gnss/en-us/NMEA-0183messages_RMC.html
/// https://docs.novatel.com/OEM7/Content/Logs/GPRMC.htm?tocpath=Commands%20%2526%20Logs%7CLogs%7CGNSS%20Logs%7C_____69#NMEAPositioningSystemModeIndicator
/// </summary>
public class NmeaMessageRmc : NmeaMessageBase
{
    public const string MessageName = "RMC";
    public static readonly NmeaMessageId MessageId = new(MessageName);
    private TimeSpan? _time;
    private NmeaPositionFixStatus? _status;
    private double _latitude;
    private double _longitude;
    private double _speedOverGroundKnots;
    private double _trackMadeGoodDegreesTrue;
    private DateTime? _date;
    private double _magneticVariationDegrees;
    private NmeaMagneticVariationDirection? _magneticVariationDirection;
    private NmeaPositioningSystemMode? _positioningSystemMode;
    private NmeaNavigationStatus? _navigationStatus;
    public override string Name => MessageName;
    public override NmeaMessageId Id => MessageId;
    
    protected override void InternalDeserialize(ref ReadOnlySpan<char> buffer)
    {
        ReadTime(ref buffer, out _time);
        ReadPositionFixStatus(ref buffer, out _status);
        ReadLatitude(ref buffer, out _latitude);
        ReadLongitude(ref buffer, out _longitude);
        ReadDouble(ref buffer, out _speedOverGroundKnots);
        ReadDouble(ref buffer, out _trackMadeGoodDegreesTrue);
        ReadDate(ref buffer, out _date);
        ReadDouble(ref buffer, out _magneticVariationDegrees);
        ReadMagneticVariationDirection(ref buffer, out _magneticVariationDirection, false);
        ReadPositioningSystemMode(ref buffer, out _positioningSystemMode, false);
        ReadNavigationStatus(ref buffer, out _navigationStatus, false);
    }

    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        WriteTime(ref buffer, _time);
        WritePositionFixStatus(ref buffer, _status);
        WriteLatitude(ref buffer, _latitude);
        WriteLongitude(ref buffer, _longitude);
        WriteDouble(ref buffer, _speedOverGroundKnots, NmeaDoubleFormat.Double1X3);
        WriteDouble(ref buffer, _trackMadeGoodDegreesTrue, NmeaDoubleFormat.Double1X1);
        WriteDate(ref buffer, _date);
        WriteDouble(ref buffer, _magneticVariationDegrees, NmeaDoubleFormat.Double1X1);
        WriteMagneticVariationDirection(ref buffer, _magneticVariationDirection);
        WritePositioningSystemMode(ref buffer, _positioningSystemMode);
        WriteNavigationStatus(ref buffer, _navigationStatus);
    }

    protected override int InternalGetByteSize()
    {
        return SizeOfTime(in _time)
            + SizeOfPositionFixStatus(_status)
            + SizeOfLatitude(_latitude)
            + SizeOfLongitude(_longitude)
            + SizeOfDouble(_speedOverGroundKnots, NmeaDoubleFormat.Double1X3)
            + SizeOfDouble(_trackMadeGoodDegreesTrue, NmeaDoubleFormat.Double1X1)
            + SizeOfDate(_date)
            + SizeOfDouble(_magneticVariationDegrees, NmeaDoubleFormat.Double1X1)
            + SizeOfMagneticVariationDirection(_magneticVariationDirection)
            + SizeOfPositioningSystemMode(_positioningSystemMode)
            + SizeOfNavigationStatus(_navigationStatus);
    }

    

    public TimeSpan? Time
    {
        get => _time;
        set => _time = value;
    }

    public NmeaPositionFixStatus? Status
    {
        get => _status;
        set => _status = value;
    }

    public double Latitude
    {
        get => _latitude;
        set => _latitude = value;
    }

    public double Longitude
    {
        get => _longitude;
        set => _longitude = value;
    }

    public double SpeedOverGroundKnots
    {
        get => _speedOverGroundKnots;
        set => _speedOverGroundKnots = value;
    }

    public double TrackMadeGoodDegreesTrue
    {
        get => _trackMadeGoodDegreesTrue;
        set => _trackMadeGoodDegreesTrue = value;
    }

    public DateTime? Date
    {
        get => _date;
        set => _date = value;
    }

    public double MagneticVariationDegrees
    {
        get => _magneticVariationDegrees;
        set => _magneticVariationDegrees = value;
    }
    public NmeaMagneticVariationDirection? MagneticVariationDirection
    {
        get => _magneticVariationDirection;
        set => _magneticVariationDirection = value;
    }

    public NmeaPositioningSystemMode? PositioningSystemMode
    {
        get => _positioningSystemMode;
        set => _positioningSystemMode = value;
    }
    
    public NmeaNavigationStatus? NavigationStatus
    {
        get => _navigationStatus;
        set => _navigationStatus = value;
    }
}