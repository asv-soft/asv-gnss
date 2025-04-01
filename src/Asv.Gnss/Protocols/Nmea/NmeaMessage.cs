using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
            var wSpan = charBuffer.AsSpan(0, buffer.Length);
            NmeaProtocol.Encoding.GetChars(buffer, wSpan);
            var rSpan = new ReadOnlySpan<char>(charBuffer,0,buffer.Length);
            InternalDeserialize(ref rSpan);
            buffer = buffer[^rSpan.Length..];
        }
        finally
        {
            ArrayPool<char>.Shared.Return(charBuffer);
        }

        if (crcIndex > 0)
        {
            // skip CRC
            buffer = buffer[2..];
        }

        if (buffer.IsEmpty)
        {
            return;
        }
        // here /r/n
        if (buffer[0] == NmeaProtocol.EndMessage1)
        {
            buffer = buffer[1..];
        }
        else
        {
            throw new ProtocolParserException(Protocol, $"Invalid end message byte: want '{NmeaProtocol.EndMessage1}', got '{buffer[0]}'");
        }
        if (buffer.IsEmpty)
        {
            return;
        }
        if (buffer[0] == NmeaProtocol.EndMessage2)
        {
            buffer = buffer[1..];
        }
        else
        {
            throw new ProtocolParserException(Protocol, $"Invalid end message byte: want '{NmeaProtocol.EndMessage2}', got '{buffer[0]}'");
        }
        if (buffer.IsEmpty == false)
        {
            Debug.Assert(false,"Buffer is not empty");
        }
    }
    protected abstract void InternalDeserialize(ref ReadOnlySpan<char> charBufferSpan);
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
        + 1 /*start CRC (*) */ 
        + 2 /*CRC*/
        + 2 /*End1 + End2*/;
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

    protected void ReadTime(ref ReadOnlySpan<char> buffer, out TimeSpan? field, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token) != false)
        {
            NmeaProtocol.ReadTime(token, out field);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.ProtocolInfo, this, "Time is required");
        }

        field = null;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteTime(ref Span<byte> charBufferSpan, TimeSpan? value)
    {
        NmeaProtocol.WriteTime(ref charBufferSpan, value);   
        NmeaProtocol.WriteSeparator(ref charBufferSpan);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfTime(in TimeSpan? value)
    {
        return NmeaProtocol.SizeOfTime(in value) + NmeaProtocol.SizeOfSeparator();    
    }
    
    protected void ReadDouble(ref ReadOnlySpan<char> buffer, out double field, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token) != false)
        {
            NmeaProtocol.ReadDouble(token, out field);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.ProtocolInfo, this, "Time is required");
        }

        field = double.NaN;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteDouble(ref Span<byte> buffer,in double value,in NmeaDoubleFormat format)
    {
        NmeaProtocol.WriteDouble(ref buffer,in value, format);   
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfDouble(in double value,in  NmeaDoubleFormat format)
    {
        return NmeaProtocol.SizeOfDouble(in value, format) + NmeaProtocol.SizeOfSeparator();    
    }
    
    protected void ReadInt(ref ReadOnlySpan<char> buffer, out int? field, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token) != false)
        {
            NmeaProtocol.ReadInt(token, out field);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.ProtocolInfo, this, "Time is required");
        }

        field = null;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteInt(ref Span<byte> buffer,in int? value,in NmeaIntFormat format)
    {
        NmeaProtocol.WriteInt(ref buffer,in value,in format);
        NmeaProtocol.WriteSeparator(ref buffer);
        
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfInt(in int? value,in NmeaIntFormat format)
    {
        return NmeaProtocol.SizeOfInt(value, format) + NmeaProtocol.SizeOfSeparator();    
    }
    
    
    
    
}