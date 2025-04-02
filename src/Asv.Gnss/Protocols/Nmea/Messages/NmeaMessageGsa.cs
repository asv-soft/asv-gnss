using System;
using System.Collections.Generic;
using DotNext.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// GSA GPS DOP and active satellites. 
/// 1) Selection mode 
/// 2) Mode 
/// 3) ID of 1st satellite used for fix 
/// 4) ID of 2nd satellite used for fix 
/// ... 
/// 14) ID of 12th satellite used for fix 
/// 15) PDOP in meters 
/// 16) HDOP in meters 
/// 17) VDOP in meters 
/// 18) Checksum
/// </summary>
public class NmeaMessageGsa : NmeaMessageBase
{
    public const string MessageName = "GSA";
    public static readonly NmeaMessageId MessageId = new(MessageName);
    
    private NmeaDopMode? _dopMode;
    private NmeaFixQuality? _fixMode;
    private double _pdop;
    private double _hdop;
    private double _vdop;
    public override string Name => MessageName;
    public override NmeaMessageId Id => MessageId;
    protected override void InternalDeserialize(ref ReadOnlySpan<char> buffer)
    {
        ReadDopMode(ref buffer, out _dopMode);
        ReadFixMode(ref buffer, out _fixMode);
        Satellites.Clear();
        for (var i = 0; i < 12; i++)
        {
            ReadInt(ref buffer, out var prn);
            if (prn == null) continue;
            Satellites.Add((byte)prn);
        }
        ReadDouble(ref buffer, out _pdop);
        ReadDouble(ref buffer, out _hdop);
        ReadDouble(ref buffer, out _vdop);
    }

    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        WriteDopMode(ref buffer, in _dopMode);
        WriteFixMode(ref buffer, in _fixMode);
        for (var i = 0; i < 12; i++)
        {
            if (i < Satellites.Count)
            {
                WriteInt(ref buffer, Satellites[i], NmeaIntFormat.IntD2);
            }
            else
            {
                WriteInt(ref buffer, null, NmeaIntFormat.IntD2);
            }
        }
        WriteDouble(ref buffer, in _pdop, NmeaDoubleFormat.Double1X1);
        WriteDouble(ref buffer, in _hdop, NmeaDoubleFormat.Double1X1);
        WriteDouble(ref buffer, in _vdop, NmeaDoubleFormat.Double1X1);
    }
    
    protected override int InternalGetByteSize()
    {
        var summ = 0;
        for (var i = 0; i < 12; i++)
        {
            if (i < Satellites.Count)
            {
                summ += SizeOfInt(Satellites[i], NmeaIntFormat.IntD2);
            }
            else
            {
                summ += SizeOfInt(null, NmeaIntFormat.IntD2);
            }
        }
        return SizeOfDopMode(in _dopMode) 
               + SizeOfFixMode(in _fixMode) 
               + summ 
               + SizeOfDouble(in _pdop, NmeaDoubleFormat.Double1X1) 
               + SizeOfDouble(in _hdop, NmeaDoubleFormat.Double1X1) 
               + SizeOfDouble(in _vdop, NmeaDoubleFormat.Double1X1);
    }

    public NmeaDopMode? DopMode
    {
        get => _dopMode;
        set => _dopMode = value;
    }
    
    public NmeaFixQuality? FixMode
    {
        get => _fixMode;
        set => _fixMode = value;
    }
    
    public List<byte> Satellites { get; } = new();
    
    public double Pdop
    {
        get => _pdop;
        set => _pdop = value;
    }
    
    public double Hdop
    {
        get => _hdop;
        set => _hdop = value;
    }
    
    public double Vdop
    {
        get => _vdop;
        set => _vdop = value;
    }
}