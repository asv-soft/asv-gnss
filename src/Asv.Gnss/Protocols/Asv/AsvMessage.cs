using System;
using Asv.IO;

namespace Asv.Gnss;

public abstract class AsvMessageBase : IProtocolMessage<ushort>
{
    private ProtocolTags _tags = [];

    public ushort Sequence { get; set; }
    public byte TargetId { get; set; }
    public byte SenderId { get; set; }

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var origin = buffer;

        if (BinSerialize.ReadByte(ref buffer) != AsvProtocol.SyncByte1 ||
            BinSerialize.ReadByte(ref buffer) != AsvProtocol.SyncByte2)
        {
            throw new Exception($"Deserialization ASV message failed: invalid sync bytes for {Name}");
        }

        var payloadLength = BinSerialize.ReadUShort(ref buffer);
        var packetSize = payloadLength + AsvProtocol.HeaderSize + AsvProtocol.CrcSize;
        if (packetSize > origin.Length)
        {
            throw new Exception($"Deserialization ASV message failed: length too small. Want '{packetSize}'. Read = '{origin.Length}'");
        }

        var calcCrc = AsvCrc16.Calc(origin, payloadLength + AsvProtocol.HeaderSize);
        var crcSpan = origin[(payloadLength + AsvProtocol.HeaderSize)..];
        var sourceCrc = BinSerialize.ReadUShort(ref crcSpan);
        if (calcCrc != sourceCrc)
        {
            throw new ProtocolDeserializeMessageException(Protocol, this, $"Invalid crc: want {calcCrc}, got {sourceCrc}");
        }

        Sequence = BinSerialize.ReadUShort(ref buffer);
        SenderId = BinSerialize.ReadByte(ref buffer);
        TargetId = BinSerialize.ReadByte(ref buffer);
        var msgId = BinSerialize.ReadUShort(ref buffer);
        if (msgId != Id)
        {
            throw new Exception($"Deserialization ASV message failed: want message number '{Id}'. Read = '{msgId}'");
        }

        var payload = buffer[..payloadLength];
        InternalDeserialize(ref payload);
        buffer = origin[packetSize..];
    }

    public void Serialize(ref Span<byte> buffer)
    {
        var origin = buffer;
        BinSerialize.WriteByte(ref buffer, AsvProtocol.SyncByte1);
        BinSerialize.WriteByte(ref buffer, AsvProtocol.SyncByte2);
        var payloadLength = (ushort)GetPayloadByteSize();
        BinSerialize.WriteUShort(ref buffer, payloadLength);
        BinSerialize.WriteUShort(ref buffer, Sequence);
        BinSerialize.WriteByte(ref buffer, SenderId);
        BinSerialize.WriteByte(ref buffer, TargetId);
        BinSerialize.WriteUShort(ref buffer, Id);
        InternalSerialize(ref buffer);

        var crc = AsvCrc16.Calc(origin, payloadLength + AsvProtocol.HeaderSize);
        BinSerialize.WriteUShort(ref buffer, crc);
    }

    protected abstract void InternalDeserialize(ref ReadOnlySpan<byte> buffer);
    protected abstract void InternalSerialize(ref Span<byte> buffer);
    protected abstract int GetPayloadByteSize();

    public int GetByteSize() => AsvProtocol.HeaderSize + GetPayloadByteSize() + AsvProtocol.CrcSize;

    public ref ProtocolTags Tags => ref _tags;

    public string GetIdAsString() => Id.ToString();

    public ProtocolInfo Protocol => AsvProtocol.Info;
    public string ProtocolId => AsvMessageParser.GnssProtocolId;

    public abstract string Name { get; }
    public ushort Id => MessageId;
    public abstract ushort MessageId { get; }

    public abstract void Randomize(Random random);
}
