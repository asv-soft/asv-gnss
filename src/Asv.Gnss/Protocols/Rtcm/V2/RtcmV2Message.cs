using System;
using Asv.IO;

namespace Asv.Gnss;

public abstract class RtcmV2MessageBase : IProtocolMessage<ushort>
{
    private ProtocolTags _tags = [];

    public double Udre { get; set; }
    public byte SequenceNumber { get; set; }
    public DateTime GpsTime { get; set; }
    public double ZCount { get; set; }
    public ushort ReferenceStationId { get; set; }

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var bitIndex = 0;
        var preamble = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
        if (preamble != RtcmV2Protocol.SyncByte)
        {
            throw new Exception($"Deserialization RTCMv2 message failed: want {RtcmV2Protocol.SyncByte:X}. Read {preamble:X}");
        }

        var msgType = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
        if (msgType != MessageId)
        {
            throw new Exception($"Deserialization RTCMv2 message failed: want message number '{MessageId}'. Read = '{msgType}'");
        }

        ReferenceStationId = (ushort)SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
        var zCountRaw = SpanBitHelper.GetBitU(buffer, ref bitIndex, 13);
        ZCount = zCountRaw * 0.6;
        if (ZCount >= 3600.0)
        {
            throw new Exception($"RTCMv2 Modified Z-count error: zcnt={ZCount}");
        }

        GpsTime = Adjhour(ZCount);
        SequenceNumber = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 3);
        var payloadLength = (byte)(SpanBitHelper.GetBitU(buffer, ref bitIndex, 5) * 3);
        if (payloadLength > buffer.Length - 6)
        {
            throw new Exception($"Deserialization RTCMv2 message failed: length too small. Want '{payloadLength}'. Read = '{buffer.Length - 6}'");
        }

        Udre = GetUdre((byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 3));
        DeserializeContent(buffer, ref bitIndex, payloadLength);
        buffer = bitIndex % 8 == 0 ? buffer[(bitIndex / 8)..] : buffer[(bitIndex / 8 + 1)..];
    }

    protected abstract void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength);

    public void Serialize(ref Span<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public int GetByteSize()
    {
        throw new NotImplementedException();
    }

    protected virtual DateTime Adjhour(double zcnt)
    {
        var utc = DateTime.UtcNow;
        var tow = 0.0;
        var week = 0;

        var time = RtcmV3Protocol.Utc2Gps(utc);
        RtcmV3Protocol.GetFromTime(time, ref week, ref tow);

        var hour = Math.Floor(tow / 3600.0);
        var sec = tow - hour * 3600.0;
        if (zcnt < sec - 1800.0) zcnt += 3600.0;
        else if (zcnt > sec + 1800.0) zcnt -= 3600.0;

        return RtcmV3Protocol.GetFromGps(week, hour * 3600 + zcnt);
    }

    private static double GetUdre(byte rsHealth)
    {
        return rsHealth switch
        {
            0 => 1.0,
            1 => 0.75,
            2 => 0.5,
            3 => 0.3,
            4 => 0.2,
            5 => 0.1,
            6 => double.NaN,
            7 => 0.0,
            _ => double.NaN
        };
    }

    public ref ProtocolTags Tags => ref _tags;
    public string GetIdAsString() => Id.ToString();
    public ProtocolInfo Protocol => RtcmV2Protocol.Info;
    public string ProtocolId => RtcmV2Protocol.GnssProtocolId;
    public abstract string Name { get; }
    public ushort Id => MessageId;
    public abstract ushort MessageId { get; }
}
