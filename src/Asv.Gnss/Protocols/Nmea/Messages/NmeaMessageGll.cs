using System;

namespace Asv.Gnss;



/// <summary>
/// Geographic Position, Latitude / Longitude and time.
/// 1) Latitude 
/// 2) N or S (North or South) 
/// 3) Longitude 
/// 4) E or W (East or West) 
/// 5) Time (UTC) 
/// 6) Status A - Data Valid, V - Data Invalid 
/// 7) Checksum
/// eg1. $GPGLL,3751.65,S,14507.36,E*77
/// eg2. $GPGLL,4916.45,N,12311.12,W,225444,A
/// eg3. $GPGLL,5133.81,N,00042.25,W*75
/// </summary>
public class NmeaMessageGll : NmeaMessageBase
{
    public const string MessageName = "GLL";
    public static readonly NmeaMessageId MessageId = new(MessageName);
    private double _longitude;
    private double _latitude;
    private TimeSpan? _time;
    private NmeaDataStatus? _status;
    private NmeaPositioningSystemMode? _positioningMode;
    public override string Name => MessageName;
    public override NmeaMessageId Id => MessageId;

    protected override void InternalDeserialize(ref ReadOnlySpan<char> buffer)
    {
        ReadLatitude(ref buffer, out _latitude);
        ReadLongitude(ref buffer, out _longitude);
        ReadTime(ref buffer, out _time, false);
        ReadDataStatus(ref buffer, out _status, false);
        ReadPositioningSystemMode(ref buffer, out _positioningMode, false);
    }

    
    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        WriteLatitude(ref buffer, in _latitude);
        WriteLongitude(ref buffer, in _longitude);
        WriteTime(ref buffer, in _time);
        WriteDataStatus(ref buffer, in _status);
        WritePositioningSystemMode(ref buffer, in _positioningMode);
    }

    protected override int InternalGetByteSize() =>
        SizeOfLatitude(in _latitude) 
        + SizeOfLongitude(in _longitude) 
        + SizeOfTime(in _time) 
        + SizeOfStatus(_status)
        + SizeOfPositioningSystemMode(_positioningMode);

    /// <summary>
    /// Gets or sets the latitude value for a location.
    /// </summary>
    /// <value>
    /// The latitude value for a location.
    /// </value>
    public double Latitude
    {
        get => _latitude;
        set => _latitude = value;
    }

    /// <summary>
    /// Gets or sets the longitude of a location.
    /// </summary>
    /// <value>The longitude value.</value>
    public double Longitude
    {
        get => _longitude;
        set => _longitude = value;
    }

    /// <summary>
    /// Time (UTC)
    /// </summary>
    public TimeSpan? Time
    {
        get => _time;
        set => _time = value;
    }

    public NmeaDataStatus? Status
    {
        get => _status;
        set => _status = value;
    }
    
    public NmeaPositioningSystemMode? PositioningMode
    {
        get => _positioningMode;
        set => _positioningMode = value;
    }
}