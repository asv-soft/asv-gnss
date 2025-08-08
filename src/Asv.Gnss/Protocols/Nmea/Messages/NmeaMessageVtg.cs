using System;

namespace Asv.Gnss;

/// <summary>
/// [VTG] Track made good and ground speed
/// https://receiverhelp.trimble.com/alloy-gnss/en-us/NMEA-0183messages_VTG.html
/// https://docs.novatel.com/OEM7/Content/Logs/GPVTG.htm?tocpath=Commands%20%2526%20Logs%7CLogs%7CGNSS%20Logs%7C_____72
/// </summary>
public class NmeaMessageVtg : NmeaMessageBase
{
    public const string MessageName = "VTG";
    public static readonly NmeaMessageId MessageId = new(MessageName);
    
    private double _trueTrack;
    private TrueTrackUnit? _tureTrackUnits;
    private double _magneticTrack;
    private MagneticTrackUnit? _magneticTrackUnit;
    private double _groundSpeedKnots;
    private GroundSpeedKnotsUnit? _groundSpeedKnotsUnit;
    private double _groundSpeedKmh;
    private GroundSpeedKmhUnit? _groundSpeedKmhUnits;
    private NmeaPositioningSystemMode? _mode;

    public override string Name => MessageName;
    public override NmeaMessageId Id => MessageId;
    
    protected override void InternalDeserialize(ref ReadOnlySpan<char> buffer)
    {
        ReadTrueTrack(ref buffer, out _trueTrack, out _tureTrackUnits);
        ReadMagneticTrack(ref buffer, out _magneticTrack, out _magneticTrackUnit);
        ReadGroundSpeedKnots(ref buffer, out _groundSpeedKnots, out _groundSpeedKnotsUnit);
        ReadGroundSpeedKmh(ref buffer, out _groundSpeedKmh, out _groundSpeedKmhUnits);
        ReadPositioningSystemMode(ref buffer, out _mode, false);
    }

    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        WriteTrueTrack(ref buffer, in _trueTrack, in _tureTrackUnits);
        WriteMagneticTrack(ref buffer, in _magneticTrack, in _magneticTrackUnit);
        WriteGroundSpeedKnots(ref buffer, in _groundSpeedKnots, in _groundSpeedKnotsUnit);
        WriteGroundSpeedKmh(ref buffer, in _groundSpeedKmh, in _groundSpeedKmhUnits);
        WritePositioningSystemMode(ref buffer, in _mode);
    }

    protected override int InternalGetByteSize()
    {
        return SizeOfTrueTrack(in _trueTrack, _tureTrackUnits)
            + SizeOfMagneticTrack(in _magneticTrack, _magneticTrackUnit)
            + SizeOfGroundSpeedKnots(in _groundSpeedKnots, _groundSpeedKnotsUnit)
            + SizeOfGroundSpeedKmh(in _groundSpeedKmh, _groundSpeedKmhUnits)
            + SizeOfPositioningSystemMode(_mode);
        
    }

    public double TrueTrack
    {
        get => _trueTrack;
        set => _trueTrack = value;
    }
    
    public TrueTrackUnit? TrueTrackUnits
    {
        get => _tureTrackUnits;
        set => _tureTrackUnits = value;
    }

    public double MagneticTrack
    {
        get => _magneticTrack;
        set => _magneticTrack = value;
    }

    public MagneticTrackUnit? MagneticTrackUnit
    {
        get => _magneticTrackUnit;
        set => _magneticTrackUnit = value;
    }
    
    public double GroundSpeedKnots
    {
        get => _groundSpeedKnots;
        set => _groundSpeedKnots = value;
    }
    
    public GroundSpeedKnotsUnit? GroundSpeedKnotsUnit
    {
        get => _groundSpeedKnotsUnit;
        set => _groundSpeedKnotsUnit = value;
    }

    public double GroundSpeedKmh
    {
        get => _groundSpeedKmh;
        set => _groundSpeedKmh = value;
    }
    
    public GroundSpeedKmhUnit? GroundSpeedKmhUnits
    {
        get => _groundSpeedKmhUnits;
        set => _groundSpeedKmhUnits = value;
    }

    public NmeaPositioningSystemMode? Mode
    {
        get => _mode;
        set => _mode = value;
    }
}