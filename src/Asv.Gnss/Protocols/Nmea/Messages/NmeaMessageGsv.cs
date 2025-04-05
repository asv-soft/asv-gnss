using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// [GSV] Satellites in view 
/// https://docs.novatel.com/OEM7/Content/Logs/GPGSV.htm?tocpath=Commands%20%2526%20Logs%7CLogs%7CGNSS%20Logs%7C_____65
/// https://receiverhelp.trimble.com/alloy-gnss/en-us/NMEA-0183messages_GSV.html
/// </summary>
public class NmeaMessageGsv : NmeaMessageBase
{
    public const string MessageName = "GSV";
    public static readonly NmeaMessageId MessageId = new(MessageName);
    
    private int? _totalMessages;
    private int? _messageNumber;
    private int? _satellitesInView;
    private int? _systemId;
    public override string Name => MessageName;
    public override NmeaMessageId Id => MessageId;

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
        
        ReadHex(ref buffer, out _systemId, false);
        
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
        WriteHex(ref buffer, _systemId, NmeaHexFormat.HexX1);;
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
        size += SizeOfHex(_systemId, NmeaHexFormat.HexX1);
        return size;

    }

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
    
}