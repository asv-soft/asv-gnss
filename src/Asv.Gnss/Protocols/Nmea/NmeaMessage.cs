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
    }
    protected abstract void InternalDeserialize(ReadOnlySpan<char> charBufferSpan);
    public void Serialize(ref Span<byte> buffer)
    {
        BinSerialize.WriteByte(ref buffer, NmeaProtocol.StartMessageByte2);
        _talkerId.Serialize(ref buffer);
        Id.Serialize(ref buffer);
        BinSerialize.WriteByte(ref buffer, NmeaProtocol.ComaByte);
        InternalSerialize(ref buffer);
    }

    protected abstract void InternalSerialize(ref Span<byte> buffer);

    public int GetByteSize() => 1 /*Start*/ + _talkerId.GetByteSize() + Id.GetByteSize() + 1 /*coma after message id*/ + InternalGetByteSize();
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

    protected TimeSpan? ReadTime(ref ReadOnlySpan<char> charBufferSpan)
    {
        return NmeaProtocol.ReadTime(NmeaProtocol.ReadNextRequiredToken(ref charBufferSpan));
    }
    
    protected void WriteTime(ref Span<byte> charBufferSpan, TimeSpan? value)
    {
        NmeaProtocol.WriteTime(ref charBufferSpan, value);   
        NmeaProtocol.WriteSeparator(ref charBufferSpan);
    }
    
    protected int SizeOfTime(TimeSpan? value)
    {
        return NmeaProtocol.SizeOfTime(value) + NmeaProtocol.SizeOfSeparator();    
    }
    
    protected double ReadDouble(ref ReadOnlySpan<char> charBufferSpan)
    {
        return NmeaProtocol.ReadDouble(NmeaProtocol.ReadNextRequiredToken(ref charBufferSpan));
    }

    protected void WriteDouble(ref Span<byte> buffer, double value, ReadOnlySpan<char> format)
    {
        NmeaProtocol.WriteDouble(ref buffer, value, format);   
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    
    protected int SizeOfDouble(double value, ReadOnlySpan<char> format)
    {
        return NmeaProtocol.SizeOfDouble(value, format) + NmeaProtocol.SizeOfSeparator();    
    }
    
    protected int? ReadInt(ref ReadOnlySpan<char> buffer)
    {
        return NmeaProtocol.ReadInt(NmeaProtocol.ReadNextRequiredToken(ref buffer));
    }
    protected void WriteInt(ref Span<byte> buffer, int? value, ReadOnlySpan<char> format)
    {
        NmeaProtocol.WriteInt(ref buffer, value, format);
        NmeaProtocol.WriteSeparator(ref buffer);
        
    }
    
    protected int SizeOfInt(int? value, ReadOnlySpan<char> format)
    {
        return NmeaProtocol.SizeOfInt(value, format) + NmeaProtocol.SizeOfSeparator();    
    }
    
    
    
    
}