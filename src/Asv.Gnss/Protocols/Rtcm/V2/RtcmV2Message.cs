using System;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class RtcmV2MessageBase : IProtocolMessage<ushort>
{
    private ProtocolTags _tags = [];
    private byte[]? _serializedTemplate;

    public double Udre { get; set; }
    public byte SequenceNumber { get; set; }
    public DateTime GpsTime { get; set; }
    public double ZCount { get; set; }
    public ushort ReferenceStationId { get; set; }

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 6)
        {
            throw new ArgumentException("An RTCM v2 header requires six normalized bytes.", nameof(buffer));
        }

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
        var packetLength = 6 + payloadLength;
        var packet = buffer[..packetLength];
        DeserializeContent(packet, ref bitIndex, payloadLength);
        _serializedTemplate = packet.ToArray();
        buffer = buffer[packetLength..];
    }

    protected abstract void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength);

    /// <summary>Gets the number of modeled payload bits, excluding final word fill bits.</summary>
    protected virtual int GetContentBitSize() =>
        throw new NotSupportedException($"Serialization is not implemented for {Name}.");

    /// <summary>Writes modeled payload fields into the normalized packet, advancing the bit position.</summary>
    protected virtual void SerializeContent(Span<byte> buffer, ref int bitIndex) =>
        throw new NotSupportedException($"Serialization is not implemented for {Name}.");

    /// <summary>
    /// Writes normalized 24-bit data words, without parity or transport framing.
    /// Use <see cref="RtcmV2Encoder"/> to encode an outgoing RTCM v2 stream.
    /// </summary>
    public void Serialize(ref Span<byte> buffer)
    {
        var size = GetByteSize();
        if (buffer.Length < size)
        {
            throw new ArgumentException("The destination is too small for the normalized RTCM v2 message.", nameof(buffer));
        }

        ArgumentOutOfRangeException.ThrowIfGreaterThan(ReferenceStationId, (ushort)1023);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(SequenceNumber, (byte)7);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(MessageId, (ushort)63);

        var zCount = Math.Round(ZCount / 0.6, MidpointRounding.AwayFromZero);
        if (!double.IsFinite(zCount) || ZCount < 0 || zCount >= 6000)
        {
            throw new ArgumentOutOfRangeException(nameof(ZCount));
        }

        var health = Udre switch
        {
            1.0 => 0,
            0.75 => 1,
            0.5 => 2,
            0.3 => 3,
            0.2 => 4,
            0.1 => 5,
            double.NaN => 6,
            0.0 => 7,
            _ => throw new ArgumentOutOfRangeException(nameof(Udre))
        };
        var packet = buffer[..size];
        packet.Fill(0xAA);
        if (_serializedTemplate?.Length == size)
        {
            _serializedTemplate.AsSpan().CopyTo(packet);
        }

        var bitIndex = 0;
        SpanBitHelper.SetBitU(packet, ref bitIndex, 8, RtcmV2Protocol.SyncByte);
        SpanBitHelper.SetBitU(packet, ref bitIndex, 6, MessageId);
        SpanBitHelper.SetBitU(packet, ref bitIndex, 10, ReferenceStationId);
        SpanBitHelper.SetBitU(packet, ref bitIndex, 13, (uint)zCount);
        SpanBitHelper.SetBitU(packet, ref bitIndex, 3, SequenceNumber);
        SpanBitHelper.SetBitU(packet, ref bitIndex, 5, (uint)((size - 6) / 3));
        SpanBitHelper.SetBitU(packet, ref bitIndex, 3, health);
        SerializeContent(packet, ref bitIndex);
        if (bitIndex != 48 + GetContentBitSize())
        {
            throw new InvalidOperationException("The RTCM v2 payload serializer wrote an unexpected number of bits.");
        }

        buffer = buffer[size..];
    }

    /// <summary>Gets the size of the normalized data words, excluding parity and transport framing.</summary>
    public int GetByteSize()
    {
        var contentBits = GetContentBitSize();
        if (contentBits is < 0 or > 31 * 24)
        {
            throw new InvalidOperationException("An RTCM v2 payload must fit in 31 data words.");
        }

        return 6 + (contentBits + 23) / 24 * 3;
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
