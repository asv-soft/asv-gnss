using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Asv.IO;

namespace Asv.Gnss;

public abstract class NmeaMessageBase : IProtocolMessage<NmeaMessageId>
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
        var trimFromEnd = 0;
        if (crcIndex > 0)
        {
            var crcBuffer = buffer[..crcIndex];
            var calcCrc = NmeaProtocol.CalcCrc(crcBuffer);
            var readCrc = NmeaProtocol.Encoding.GetString(buffer.Slice(crcIndex + 1, 2));
            if (calcCrc != readCrc)
            {
                throw new ProtocolDeserializeMessageException(Protocol, this, $"Invalid crc: want {calcCrc}, got {readCrc}");
            }
            trimFromEnd = buffer.Length - crcIndex - 1 /*we stay end star (*) here */;
        }
        
        if (NmeaProtocol.TryGetMessageId(ref buffer, out var msgId, out _talkerId))
        {
            if (msgId != Id)
            {
                throw new Exception($"Invalid message id {msgId} for {Name}");
            }
        }

        // trim crc with star
        var contentBuffer = buffer[.. ^trimFromEnd];
        var charBuffer = ArrayPool<char>.Shared.Rent(contentBuffer.Length);
        try
        {
            var wSpan = charBuffer.AsSpan(0, contentBuffer.Length);
            NmeaProtocol.Encoding.GetChars(contentBuffer, wSpan);
            var rSpan = new ReadOnlySpan<char>(charBuffer,0,contentBuffer.Length);
            InternalDeserialize(ref rSpan);
            buffer = buffer[^rSpan.Length..];
        }
        finally
        {
            ArrayPool<char>.Shared.Return(charBuffer);
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
    protected abstract void InternalDeserialize(ref ReadOnlySpan<char> buffer);
    public void Serialize(ref Span<byte> buffer)
    {
        BinSerialize.WriteByte(ref buffer, NmeaProtocol.StartMessageByte1);
        var origin = buffer;
        _talkerId.Serialize(ref buffer);
        Id.Serialize(ref buffer);
        BinSerialize.WriteByte(ref buffer, NmeaProtocol.ComaByte);
        var beforeInternalSerialize = buffer;
        InternalSerialize(ref buffer);
        // go back for 1 symbol (last coma must be replaced by *)
        buffer = beforeInternalSerialize[(beforeInternalSerialize.Length - buffer.Length - 1)..];
        var crc = origin[..(buffer.Length - 1)];
        BinSerialize.WriteByte(ref buffer, NmeaProtocol.StartCrcByte);
        var crcStr = NmeaProtocol.CalcCrc(crc);
        NmeaProtocol.Encoding.GetBytes(crcStr, buffer);
        buffer = buffer[2..];
        BinSerialize.WriteByte(ref buffer, NmeaProtocol.EndMessageByte1);
        BinSerialize.WriteByte(ref buffer, NmeaProtocol.EndMessageByte2);
    }

    protected abstract void InternalSerialize(ref Span<byte> buffer);

    public int GetByteSize() => 
        1 /*Start*/ 
        + _talkerId.GetByteSize() 
        + Id.GetByteSize() 
        + 1 /*coma after message id*/ 
        + InternalGetByteSize() // there will be an extra last comma here, we'll replace it with star (*)
        + 2 /*CRC*/
        + 2 /*End1 + End2*/;
    protected abstract int InternalGetByteSize();

    public ref ProtocolTags Tags => ref _tags;

    public string GetIdAsString() => Id.ToString();

    public ProtocolInfo Protocol => NmeaProtocol.Info;
    public abstract string Name { get; }
    public abstract NmeaMessageId Id { get; }

    public NmeaTalkerId TalkerId
    {
        get => _talkerId;
        set => _talkerId = value;
    }

    #region Time

    protected void ReadTime(ref ReadOnlySpan<char> buffer, out TimeSpan? field, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token) != false)
        {
            NmeaProtocol.ReadTime(token, out field);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "Time is required");
        }

        field = null;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteTime(ref Span<byte> charBufferSpan,in TimeSpan? value)
    {
        NmeaProtocol.WriteTime(ref charBufferSpan, value);   
        NmeaProtocol.WriteSeparator(ref charBufferSpan);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfTime(in TimeSpan? value)
    {
        return NmeaProtocol.SizeOfTime(in value) + NmeaProtocol.SizeOfSeparator();    
    }

    #endregion

    #region Latitude

    protected void ReadLatitude(ref ReadOnlySpan<char> buffer, out double field, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var digit))
        {
            if (NmeaProtocol.TryReadNextToken(ref buffer, out var northSouthSpan) == false)
            {
                throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "Latitude direction is required");
            }
            
            NmeaProtocol.ReadLatitude(digit, northSouthSpan,  out field);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "Latitude is required");
        }

        field = double.NaN;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void WriteLatitude(ref Span<byte> buffer, in double latitude)
    {
        NmeaProtocol.WriteLatitude(ref buffer, in latitude);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int SizeOfLatitude(in double latitude) => NmeaProtocol.SizeOfLatitude(in latitude) + NmeaProtocol.SizeOfSeparator();

    #endregion

    #region Longitude

    protected void ReadLongitude(ref ReadOnlySpan<char> buffer, out double field, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var digit))
        {
            if (NmeaProtocol.TryReadNextToken(ref buffer, out var northSouthSpan) == false)
            {
                throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "Longitude direction is required");
            }
            
            NmeaProtocol.ReadLongitude(digit, northSouthSpan,  out field);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "Longitude is required");
        }

        field = double.NaN;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void WriteLongitude(ref Span<byte> buffer, in double latitude)
    {
        NmeaProtocol.WriteLongitude(ref buffer, in latitude);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int SizeOfLongitude(in double longitude) => NmeaProtocol.SizeOfLongitude(in longitude) + NmeaProtocol.SizeOfSeparator();

    #endregion

    #region Double

    protected void ReadDouble(ref ReadOnlySpan<char> buffer, out double field, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadDouble(token, out field);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "Double is required");
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

    #endregion

    #region Int

    protected void ReadInt(ref ReadOnlySpan<char> buffer, out int? field, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token) != false)
        {
            NmeaProtocol.ReadInt(token, out field);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "Time is required");
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

    #endregion

    #region GpsQuality

    protected void ReadGpsQuality(ref ReadOnlySpan<char> buffer, out NmeaGpsQuality? value, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadGpsQuality(token, out value);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "Time is required");
        }

        value = NmeaGpsQuality.Unknown;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void WriteGpsQuality(ref Span<byte> buffer, in NmeaGpsQuality? gpsQuality)
    {
        NmeaProtocol.WriteGpsQuality(ref buffer, in gpsQuality);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int SizeOfGpsQuality(in NmeaGpsQuality? gpsQuality)
    {
        return NmeaProtocol.SizeOfGpsQuality(in gpsQuality) + NmeaProtocol.SizeOfSeparator();    
    }

    #endregion

    #region String

    protected void ReadString(ref ReadOnlySpan<char> buffer, out string value, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            value = NmeaProtocol.ReadString(token);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "String is required");
        }

        value = string.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void WriteString(ref Span<byte> buffer, string? value)
    {
        NmeaProtocol.WriteString(ref buffer, value);
        NmeaProtocol.WriteSeparator(ref buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int SizeOfString(string? value)
    {
        return NmeaProtocol.SizeOfString(value) + NmeaProtocol.SizeOfSeparator();
    }

    #endregion

    #region Status

    protected void ReadDataStatus(ref ReadOnlySpan<char> buffer, out NmeaDataStatus? status, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadDataStatus(ref token, out status);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "String is required");
        }

        status = null;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void WriteDataStatus(ref Span<byte> buffer, in NmeaDataStatus? status)
    {
        NmeaProtocol.WriteDataStatus(ref buffer, in status);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int SizeOfStatus(NmeaDataStatus? status) => NmeaProtocol.SizeOfStatus(status) + NmeaProtocol.SizeOfSeparator();

    #endregion
    
    #region Status

    protected void ReadPositioningSystemMode(ref ReadOnlySpan<char> buffer, out NmeaPositioningSystemMode? status, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadPositioningSystemMode(ref token, out status);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "String is required");
        }

        status = null;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void WritePositioningSystemMode(ref Span<byte> buffer, in NmeaPositioningSystemMode? status)
    {
        NmeaProtocol.WritePositioningSystemMode(ref buffer, in status);
        NmeaProtocol.WriteSeparator(ref buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int SizeOfPositioningSystemMode(NmeaPositioningSystemMode? status) 
        => NmeaProtocol.SizeOfPositioningSystemMode(status) + NmeaProtocol.SizeOfSeparator();

    #endregion


    #region FixMode

    protected void ReadFixMode(ref ReadOnlySpan<char> buffer, out NmeaFixQuality? value, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadFixMode(ref token, out value);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "String is required");
        }

        value = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void WriteFixMode(ref Span<byte> buffer, in NmeaFixQuality? value)
    {
        NmeaProtocol.WriteFixMode(ref buffer, in value);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int SizeOfFixMode(in NmeaFixQuality? fixMode)
    {
        return NmeaProtocol.SizeOfFixMode(fixMode) + NmeaProtocol.SizeOfSeparator();
    }

    #endregion

    #region DopMod

    protected void ReadDopMode(ref ReadOnlySpan<char> buffer, out NmeaDopMode? value, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadDopMode(ref token, out value);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "String is required");
        }

        value = null;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void WriteDopMode(ref Span<byte> buffer, in NmeaDopMode? value)
    {
        NmeaProtocol.WriteDopMode(ref buffer, in value);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected int SizeOfDopMode(in NmeaDopMode? value)
    {
        return NmeaProtocol.SizeOfDopMode(in value) + NmeaProtocol.SizeOfSeparator();
    }

    #endregion
    
}