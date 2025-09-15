using System;
using Asv.IO;

namespace Asv.Gnss;

public abstract class RtcmV3MessageBase : IProtocolMessage<ushort>
{
    private ProtocolTags _tags = [];
    
    /// <summary>
    /// Deserializes the specified buffer into the current object.
    /// </summary>
    /// <param name="buffer">The buffer containing the serialized data.</param>
    /// <exception cref="Exception">
    /// Thrown when the deserialization of the RTCMv3 message fails due to an incorrect preamble,
    /// length too small, or incorrect message number.
    /// </exception>
    
    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var bitIndex = 0;
        var preamble = (byte)SpanBitHelper.GetBitU(buffer,ref bitIndex, 8);
        if (preamble != RtcmV3Protocol.SyncByte)
        {
            throw new Exception($"Deserialization RTCMv3 message failed: want {RtcmV3Protocol.SyncByte:X}. Read {preamble:X}");
        }

        bitIndex += 6; // reserved
        var messageLength = (byte)SpanBitHelper.GetBitU(buffer,ref bitIndex, 10);
        if (messageLength > (buffer.Length - 3 /* preamble-8bit + reserved-6bit + length-10bit  */ - 3 /* crc 24 bit */))
        {
            throw new Exception($"Deserialization RTCMv3 message failed: length too small. Want '{messageLength}'. Read = '{buffer.Length - 6}'");
        }
        var msgId = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);
        if (msgId != Id)
        {
            throw new Exception($"Deserialization RTCMv3 message failed: want message number '{Id}'. Read = '{msgId}'");
        }

        var crcBitPos = (3 /* preamble-8bit + reserved-6bit + length-10bit */ + messageLength) * 8;
        var originalCrc = RtcmV3Crc24.Calc(buffer, messageLength + 3 /* preamble-8bit + reserved-6bit + length-10bit  */, 0);
        var sourceCrc = SpanBitHelper.GetBitU(buffer, ref crcBitPos, 24);
        if (originalCrc != sourceCrc)
        {
            throw new ProtocolDeserializeMessageException(Protocol, this, $"Invalid crc: want {originalCrc}, got {sourceCrc}");
        }
         /* preamble-8bit + reserved-6bit + length-10bit  */
        InternalDeserialize(buffer, ref bitIndex);
        bitIndex += 3 * 8; // skip crc
        buffer = bitIndex % 8.0 == 0 ? buffer[(bitIndex / 8)..] : buffer[(bitIndex / 8 + 1)..];
    }
    protected abstract void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitIndex);
    
    public void Serialize(ref Span<byte> buffer)
    {
        var bitIndex = 0;
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 8, RtcmV3Protocol.SyncByte);
        bitIndex += 6; // Reserved
        var msgBitSize = 12 /* message Id 12 bits */ + InternalGetBitSize();
        var msgByteSize = msgBitSize % 8.0 == 0 ? msgBitSize / 8 : msgBitSize / 8 + 1;
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 10, msgByteSize);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 12, Id);
        InternalSerialize(buffer, ref bitIndex);
        bitIndex = msgByteSize * 8 + 24;
        var calcCrc = RtcmV3Crc24.Calc(buffer, msgByteSize + 3 /* preamble-8bit + reserved-6bit + length-10bit  */, 0);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 24, calcCrc);
        // buffer = bitIndex / 8.0 == 0 ? buffer[(bitIndex / 8)..] : buffer[(bitIndex / 8 + 1)..];
        buffer = buffer[(bitIndex / 8)..];
    }

    protected abstract void InternalSerialize(Span<byte> buffer, ref int bitIndex);

    public int GetByteSize()
    {
        var msgBitSize = 12 /* message Id 12 bits */ + InternalGetBitSize();
        var msgByteSize = msgBitSize % 8.0 == 0 ? msgBitSize / 8 : msgBitSize / 8 + 1;
        return 3 + msgByteSize + 3;
    }

    protected abstract int InternalGetBitSize();

    public ref ProtocolTags Tags => ref _tags;

    public string GetIdAsString()
    {
        return Id.ToString();
    }

    public ProtocolInfo Protocol => RtcmV3Protocol.Info;

    public abstract string Name { get; }
    public abstract ushort Id { get; }
}