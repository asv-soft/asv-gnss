using System;
using Asv.IO;

namespace Asv.Gnss;

public abstract class UbxMessageBase : IProtocolMessage<ushort>
{
    private ProtocolTags _tags = [];
    
    /// <summary>
    /// Represents a property that exposes the class value in a byte format.
    /// This property is read-only. </summary>
    /// /
    public abstract byte Class { get; }

    /// <summary>
    /// Gets the value of the SubClass property.
    /// </summary>
    /// <value>
    /// A byte representing the value of the SubClass property.
    /// </value>
    public abstract byte SubClass { get; }
    

    protected abstract void DeserializeContent(ref ReadOnlySpan<byte> buffer);

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        if (BinSerialize.ReadByte(ref buffer) != UbxProtocol.SyncByte1 ||
            BinSerialize.ReadByte(ref buffer) != UbxProtocol.SyncByte2)
        {
            throw new Exception(
                $"Deserialization UBX message failed: want {UbxProtocol.SyncByte1:X} {UbxProtocol.SyncByte2:X}. Read {buffer[0]:X} {buffer[1]:X}");
        }
        var msgSpan = buffer;
        
        var msgId = (ushort)((BinSerialize.ReadByte(ref buffer) << 8) | BinSerialize.ReadByte(ref buffer));
        if (msgId != Id)
        {
            throw new Exception(
                $"Deserialization UBX message failed: want message number '{UbxProtocol.GetMessageName(Id)}'. Read = '{UbxProtocol.GetMessageName(msgId)}'");
        }

        var payloadLength = BinSerialize.ReadUShort(ref buffer);
        msgSpan = msgSpan[..(payloadLength + 4)];
        var crcSpan = buffer[payloadLength..];
        var originalCrc = UbxCrc16.Calc(msgSpan[..(payloadLength + 4)]);
        if (originalCrc.Crc1 != crcSpan[0] || originalCrc.Crc2 != crcSpan[1])
        {
            throw new ProtocolDeserializeMessageException(Protocol, this,
                $"Invalid crc: want {(originalCrc.Crc1 << 8) | originalCrc.Crc2}, got {(crcSpan[0] << 8) | crcSpan[1]})");
        }
        
        var readSpan = buffer[..payloadLength];
        DeserializeContent(ref readSpan);
        buffer = buffer[(payloadLength + 2)..];
    }

    protected abstract void SerializeContent(ref Span<byte> buffer);
    

    public void Serialize(ref Span<byte> buffer)
    {
        BinSerialize.WriteByte(ref buffer, UbxProtocol.SyncByte1);
        BinSerialize.WriteByte(ref buffer, UbxProtocol.SyncByte2);

        var crcSpan = buffer;

        BinSerialize.WriteByte(ref buffer, Class);
        BinSerialize.WriteByte(ref buffer, SubClass);
            
        var size = (ushort)GetContentByteSize();
        BinSerialize.WriteUShort(ref buffer, size);

        var writeSpan = buffer[..size];
        SerializeContent(ref writeSpan);
            
        buffer = buffer[size..];
        crcSpan = crcSpan[..(size + 4)];
        var crc = UbxCrc16.Calc(crcSpan);
        BinSerialize.WriteByte(ref buffer, crc.Crc1);
        BinSerialize.WriteByte(ref buffer, crc.Crc2);
    }

    protected abstract int GetContentByteSize();
    
    public int GetByteSize()
    {
        return UbxProtocol.HeaderOffset + 2/*CRC*/ + GetContentByteSize();
    }

    public ref ProtocolTags Tags => ref _tags;

    public string GetIdAsString()
    {
        return Id.ToString();
    }

    public ProtocolInfo Protocol => UbxProtocol.Info;

    public abstract string Name { get; }
    public ushort Id => (ushort)((Class << 8) | SubClass);
    
    /// <summary>
    /// Randomizes the object based on the given random number generator.
    /// </summary>
    /// <param name="random">The random number generator to use.</param>
    public abstract void Randomize(Random random);
}