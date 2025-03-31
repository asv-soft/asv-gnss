using System;
using System.Buffers;
using Asv.IO;

namespace Asv.Gnss;

public abstract class NmeaMessage : IProtocolMessage<NmeaMessageId>
{
    private ProtocolTags _tags = [];
    private NmeaTalkerId _talkerId;

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < 5) throw new Exception("Too small string for NMEA");
        
        var start = buffer.IndexOfAny(NmeaProtocol.StartMessageByte2, NmeaProtocol.StartMessageByte1);
        if (start >= 0)
        {
            // skip start symbols
            buffer = buffer.Slice(start + 1, buffer.Length - start - 1);
        }
        var crcIndex = buffer.IndexOf(NmeaProtocol.StartCrcByte);
        if (crcIndex > 0)
        {
            var crcBuffer = buffer[..crcIndex];
            var calcCrc = NmeaProtocol.CalcCrc(crcBuffer);
            var readCrc = NmeaProtocol.Encoding.GetString(buffer.Slice(crcIndex + 1, 2));
            if (calcCrc != readCrc)
            {
                throw new ProtocolDeserializeMessageException(Protocol, this, $"Invalid crc: want {calcCrc}, got {readCrc}");
            }
        }
        
        if (NmeaProtocol.TryGetMessageId(ref buffer, out var msgId, out _talkerId))
        {
            if (msgId != Id)
            {
                throw new Exception($"Invalid message id {msgId} for {Name}");
            }
        }
        var charBuffer = ArrayPool<char>.Shared.Rent(buffer.Length);
        try
        {
            var charBufferSpan = charBuffer.AsSpan(0, buffer.Length);
            NmeaProtocol.Encoding.GetChars(buffer, charBufferSpan);
            InternalDeserialize(charBufferSpan);
        }
        finally
        {
            ArrayPool<char>.Shared.Return(charBuffer);
        }
        
        var stop = buffer.IndexOf(NmeaProtocol.EndMessageByte1);
        if (stop >= 0)
        {
            buffer = buffer[..stop];
        }
        var stop2 = buffer.IndexOf(NmeaProtocol.EndMessageByte1);
        if (stop2 >= 0)
        {
            buffer = buffer[..stop2];
        }
    }
    protected abstract void InternalDeserialize(ReadOnlySpan<char> charBufferSpan);
    public void Serialize(ref Span<byte> buffer)
    {
        BinSerialize.WriteByte(ref buffer, NmeaProtocol.StartMessageByte2);
        var origin = buffer;
        _talkerId.Serialize(ref buffer);
        Id.Serialize(ref buffer);
        BinSerialize.WriteByte(ref buffer, NmeaProtocol.ComaByte);
        InternalSerialize(ref buffer);
        var crc = origin[..(buffer.Length - 1)];
        BinSerialize.WriteByte(ref buffer, NmeaProtocol.StartCrcByte);
        var crcStr = NmeaProtocol.CalcCrc(crc);
        NmeaProtocol.Encoding.GetBytes(crcStr, buffer);
        BinSerialize.WriteByte(ref buffer, NmeaProtocol.EndMessageByte1);
        BinSerialize.WriteByte(ref buffer, NmeaProtocol.EndMessageByte2);
    }

    protected abstract void InternalSerialize(ref Span<byte> buffer);

    public int GetByteSize() => 
        1 /*Start*/ 
        + _talkerId.GetByteSize() 
        + Id.GetByteSize() 
        + 1 /*coma after message id*/ 
        + InternalGetByteSize()
        + 2 /*CRC*/
        + 1 /*End*/;
    protected abstract int InternalGetByteSize();

    public ref ProtocolTags Tags => ref _tags;

    public string GetIdAsString() => Id.ToString();

    public ProtocolInfo Protocol => NmeaProtocol.ProtocolInfo;
    public abstract string Name { get; }
    public abstract NmeaMessageId Id { get; }

    public NmeaTalkerId TalkerId
    {
        get => _talkerId;
        set => _talkerId = value;
    }

    protected static TimeSpan? ReadTime(ref ReadOnlySpan<char> charBufferSpan)
    {
        return NmeaProtocol.ReadTime(NmeaProtocol.ReadNextRequiredToken(ref charBufferSpan));
    }
    
    protected static void WriteTime(ref Span<byte> charBufferSpan, TimeSpan? value)
    {
        NmeaProtocol.WriteTime(ref charBufferSpan, value);   
        NmeaProtocol.WriteSeparator(ref charBufferSpan);
    }
    
    protected static int SizeOfTime(TimeSpan? value)
    {
        return NmeaProtocol.SizeOfTime(value) + NmeaProtocol.SizeOfSeparator();    
    }
    
    protected static double ReadDouble(ref ReadOnlySpan<char> charBufferSpan)
    {
        return NmeaProtocol.ReadDouble(NmeaProtocol.ReadNextRequiredToken(ref charBufferSpan));
    }

    protected static void WriteDouble(ref Span<byte> buffer, double value, NmeaDoubleFormat format)
    {
        NmeaProtocol.WriteDouble(ref buffer, value, format);   
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    
    protected static int SizeOfDouble(double value, NmeaDoubleFormat format)
    {
        return NmeaProtocol.SizeOfDouble(value, format) + NmeaProtocol.SizeOfSeparator();    
    }
    
    protected static int? ReadInt(ref ReadOnlySpan<char> buffer)
    {
        return NmeaProtocol.ReadInt(NmeaProtocol.ReadNextRequiredToken(ref buffer));
    }
    protected static void WriteInt(ref Span<byte> buffer, int? value, NmeaIntFormat format)
    {
        NmeaProtocol.WriteInt(ref buffer, value, format);
        NmeaProtocol.WriteSeparator(ref buffer);
        
    }
    
    protected static int SizeOfInt(int? value, NmeaIntFormat format)
    {
        return NmeaProtocol.SizeOfInt(value, format) + NmeaProtocol.SizeOfSeparator();    
    }
    
    
    
    
}