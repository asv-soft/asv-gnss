using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// GSV Satellites in view 
///  
/// 1) total number of messages 
/// 2) message number 
/// 3) satellites in view 
/// 4) satellite number 
/// 5) elevation in degrees 
/// 6) azimuth in degrees to true 
/// 7) SNR in dB 
/// more satellite infos like 4)-7) 
/// n) Checksum
/// </summary>
public class NmeaMessageGsv : NmeaMessageBase
{
    private int? _totalMessages;
    private int? _messageNumber;
    private int? _satellitesInView;
    private int? _systemId;
    public const string MessageName = "GSV";
    public static readonly NmeaMessageId MessageId = new(MessageName);

    public override string Name => MessageName;
    public override NmeaMessageId Id => MessageId;

    

    public List<SatelliteInfo> Satellites { get; } = new();
    
    public int? TotalMessages
    {
        get => _totalMessages;
        set => _totalMessages = value;
    }
    
    public int? MessageNumber
    {
        get => _messageNumber;
        set => _messageNumber = value;
    }
    
    public int? SatellitesInView
    {
        get => _satellitesInView;
        set => _satellitesInView = value;
    }
    
    /// <summary>
    /// This field is only output if the NMEAVERSION is 4.11
    /// </summary>
    public int? SystemId
    {
        get => _systemId;
        set => _systemId = value;
    }
    
    protected override void InternalDeserialize(ref ReadOnlySpan<char> buffer)
    {
        ReadInt(ref buffer, out _totalMessages);
        ReadInt(ref buffer, out _messageNumber);
        ReadInt(ref buffer, out _satellitesInView);
        Satellites.Clear();
        var satCount = NmeaProtocol.TokenCount(ref buffer) / 4;
        for (int i = 0; i < satCount; i++)
        {
            ReadInt(ref buffer, out var number, false);
            ReadInt(ref buffer, out var elevation, false);
            ReadInt(ref buffer, out var azimuth, false);
            ReadInt(ref buffer, out var snr, false);
            if (number == null)
            {
                continue;
            }
            var sat = new SatelliteInfo(TalkerId, number.Value, elevation, azimuth, snr);
            Satellites.Add(sat);
        }
        
        ReadInt(ref buffer, out _systemId, false);
        
    }
    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        WriteInt(ref buffer, _totalMessages, NmeaIntFormat.IntD1);
        WriteInt(ref buffer, _messageNumber, NmeaIntFormat.IntD1);
        WriteInt(ref buffer, _satellitesInView, NmeaIntFormat.IntD2);
        foreach (var sat in Satellites)
        {
            WriteInt(ref buffer, sat.NmeaPrn, NmeaIntFormat.IntD2);
            WriteInt(ref buffer, sat.Elevation, NmeaIntFormat.IntD2);
            WriteInt(ref buffer, sat.Azimuth, NmeaIntFormat.IntD3);
            WriteInt(ref buffer, sat.Snr, NmeaIntFormat.IntD2);
        }
    }

    protected override int InternalGetByteSize()
    {
        var size = 0;
        size += SizeOfInt(_totalMessages, NmeaIntFormat.IntD1);
        size += SizeOfInt(_messageNumber, NmeaIntFormat.IntD1);
        size += SizeOfInt(_satellitesInView, NmeaIntFormat.IntD2);
        foreach (var sat in Satellites)
        {
            size += SizeOfInt(sat.NmeaPrn, NmeaIntFormat.IntD2);
            size += SizeOfInt(sat.Elevation, NmeaIntFormat.IntD2);
            size += SizeOfInt(sat.Azimuth, NmeaIntFormat.IntD3);
            size += SizeOfInt(sat.Snr, NmeaIntFormat.IntD2);
        }

        return size;

    }

    
}