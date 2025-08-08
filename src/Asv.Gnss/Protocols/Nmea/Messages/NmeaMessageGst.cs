using System;

namespace Asv.Gnss;


/// <summary>
/// [GST] Position error statistics
/// https://receiverhelp.trimble.com/alloy-gnss/en-us/NMEA-0183messages_GST.html
/// https://docs.novatel.com/OEM7/Content/Logs/GPGST.htm?tocpath=Commands%20%2526%20Logs%7CLogs%7CGNSS%20Logs%7C_____64
/// </summary>
public class NmeaMessageGst : NmeaMessageBase
{
    
    public const string MessageName = "GST";
    public static readonly NmeaMessageId MessageId = new(MessageName);

    private TimeOnly? _time;
    private double _rms;
    private double _majorAxis;
    private double _minorAxis;
    private double _orientation;
    private double _latStdDev;
    private double _lonStdDev;
    private double _altStdDev;
    public override string Name => MessageName;
    public override NmeaMessageId Id => MessageId;
        
    protected override void InternalDeserialize(ref ReadOnlySpan<char> buffer)
    {
        ReadTime(ref buffer, out _time);
        ReadDouble(ref buffer, out _rms);
        ReadDouble(ref buffer, out _majorAxis);
        ReadDouble(ref buffer, out _minorAxis);
        ReadDouble(ref buffer, out _orientation);
        ReadDouble(ref buffer, out _latStdDev);
        ReadDouble(ref buffer, out _lonStdDev);
        ReadDouble(ref buffer, out _altStdDev);
    }

    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        WriteTime(ref buffer, in _time);
        WriteDouble(ref buffer, _rms, NmeaDoubleFormat.Double1X3);
        WriteDouble(ref buffer, _majorAxis, NmeaDoubleFormat.Double1X3);
        WriteDouble(ref buffer, _minorAxis, NmeaDoubleFormat.Double1X3);
        WriteDouble(ref buffer, _orientation, NmeaDoubleFormat.Double1X4);
        WriteDouble(ref buffer, _latStdDev, NmeaDoubleFormat.Double1X3);
        WriteDouble(ref buffer, _lonStdDev, NmeaDoubleFormat.Double1X3);
        WriteDouble(ref buffer, _altStdDev, NmeaDoubleFormat.Double1X3);
    }

    protected override int InternalGetByteSize()
    {
        return SizeOfTime(in _time) +
               SizeOfDouble(_rms, NmeaDoubleFormat.Double1X3) +
               SizeOfDouble(_majorAxis, NmeaDoubleFormat.Double1X3) +
               SizeOfDouble(_minorAxis, NmeaDoubleFormat.Double1X3) +
               SizeOfDouble(_orientation, NmeaDoubleFormat.Double1X4) +
               SizeOfDouble(_latStdDev, NmeaDoubleFormat.Double1X3) +
               SizeOfDouble(_lonStdDev, NmeaDoubleFormat.Double1X3) +
               SizeOfDouble(_altStdDev, NmeaDoubleFormat.Double1X3);
    }
    
    /// <summary>
    /// UTC time status of position
    /// (hours/minutes/seconds/ decimal seconds)
    /// </summary>
    public TimeOnly? Time
    {
        get => _time;
        set => _time = value;
    }

    /// <summary>
    /// RMS value of the standard deviation of the range inputs to the navigation process. Range inputs include pseudoranges and DGPS corrections
    /// </summary>
    public double Rms
    {
        get => _rms;
        set => _rms = value;
    }

    /// <summary>
    /// Standard deviation of semi-major axis of error ellipse (m)
    /// </summary>
    public double MajorAxis
    {
        get => _majorAxis;
        set => _majorAxis = value;
    }
    
    /// <summary>
    /// Standard deviation of semi-minor axis of error ellipse (m)
    /// </summary>
    public double MinorAxis
    {
        get => _minorAxis;
        set => _minorAxis = value;
    }
    
    /// <summary>
    /// Orientation of semi-major axis of error ellipse (degrees from true north)
    /// </summary>
    public double Orientation
    {
        get => _orientation;
        set => _orientation = value;
    }
    
    /// <summary>
    /// Standard deviation of latitude error (m)
    /// </summary>
    public double LatStdDev
    {
        get => _latStdDev;
        set => _latStdDev = value;
    }
    
    /// <summary>
    /// Standard deviation of longitude error (m)
    /// </summary>
    public double LonStdDev
    {
        get => _lonStdDev;
        set => _lonStdDev = value;
    }
    
    /// <summary>
    /// Standard deviation of altitude error (m)
    /// </summary>
    public double AltStdDev
    {
        get => _altStdDev;
        set => _altStdDev = value;
    }
    
}