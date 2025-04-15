using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Asv.IO;
using DotNext;

namespace Asv.Gnss;

public abstract class NmeaMessageBase : IProtocolMessage<NmeaMessageId>
{
    private ProtocolTags _tags = [];
    private NmeaTalkerId _talkerId;

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        buffer = buffer
            .TrimEnd(NmeaProtocol.EndMessageByte1)
            .TrimEnd(NmeaProtocol.EndMessageByte2)
            .TrimEnd(NmeaProtocol.SpaceByte)
            .TrimStart(NmeaProtocol.SpaceByte)
            .TrimStart(NmeaProtocol.StartMessageByte1)
            .TrimStart(NmeaProtocol.StartMessageByte2);
        
        if (buffer.Length < 5) throw new Exception("Too small string for NMEA");
       
        var crcIndex = buffer.IndexOf(NmeaProtocol.StartCrcByte);
        var trimFromEnd = 0;
        if (crcIndex > 0)
        {
            HasCrc = true;
            var crcBuffer = buffer[..crcIndex];
            var calcCrc = NmeaProtocol.CalcCrc(crcBuffer);
            var readCrc = NmeaProtocol.Encoding.GetString(buffer.Slice(crcIndex + 1, 2));
            if (calcCrc != readCrc)
            {
                throw new ProtocolDeserializeMessageException(Protocol, this, $"Invalid crc: want {calcCrc}, got {readCrc}");
            }
            trimFromEnd = buffer.Length - crcIndex;
        }
        else
        {
            HasCrc = false;
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
        //Debug.Assert(false,"Buffer is not empty. Maybe unexpected data after message. We skip it for Release build.");
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
        // go back for 1 symbol (last coma must be replaced by * with crc or end bytes)
        buffer = beforeInternalSerialize[(beforeInternalSerialize.Length - buffer.Length - 1)..];
        if (HasCrc)
        {
            var crc = origin[..^buffer.Length];
            BinSerialize.WriteByte(ref buffer, NmeaProtocol.StartCrcByte);
            var crcStr = NmeaProtocol.CalcCrc(crc);
            NmeaProtocol.Encoding.GetBytes(crcStr, buffer);
            buffer = buffer[2..];    
        }
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
        + (HasCrc ? 2 : -1) // there will be an extra last comma here after InternalGetByteSize, we'll replace it with star (*) if CRC required
        + 2 /*End1 + End2*/;
    protected abstract int InternalGetByteSize();
    
    public override string ToString()
    {
        var size = GetByteSize();
        var arr = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            var wSpan = new Span<byte>(arr, 0, size);
            Serialize(ref wSpan);
            var rSpan = new ReadOnlySpan<byte>(arr, 0, size);
            rSpan = rSpan.TrimEnd(NmeaProtocol.EndMessageByte1);
            rSpan = rSpan.TrimEnd(NmeaProtocol.EndMessageByte2);
            return NmeaProtocol.Encoding.GetString(rSpan);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(arr);
        }
    }

    public ref ProtocolTags Tags => ref _tags;

    public string GetIdAsString() => Id.ToString();

    public ProtocolInfo Protocol => NmeaProtocol.Info;
    public abstract string Name { get; }
    public abstract NmeaMessageId Id { get; }
    public bool HasCrc { get; set; } = true;
    public NmeaTalkerId TalkerId
    {
        get => _talkerId;
        set => _talkerId = value;
    }

    #region Time

    protected void ReadTime(ref ReadOnlySpan<char> buffer, out TimeOnly? field, bool required = true)
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
    protected static void WriteTime(ref Span<byte> charBufferSpan,in TimeOnly? value)
    {
        NmeaProtocol.WriteTime(ref charBufferSpan, value);   
        NmeaProtocol.WriteSeparator(ref charBufferSpan);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfTime(in TimeOnly? value)
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
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"Int is required: '{buffer}'");
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
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(NmeaGpsQuality)} is required: '...{buffer}'");
        }

        value = NmeaGpsQuality.Unknown;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteGpsQuality(ref Span<byte> buffer, in NmeaGpsQuality? gpsQuality)
    {
        NmeaProtocol.WriteGpsQuality(ref buffer, in gpsQuality);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfGpsQuality(in NmeaGpsQuality? gpsQuality)
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
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(String)} is required: '...{buffer}'");
        }

        value = string.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteString(ref Span<byte> buffer, string? value)
    {
        NmeaProtocol.WriteString(ref buffer, value);
        NmeaProtocol.WriteSeparator(ref buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfString(string? value)
    {
        return NmeaProtocol.SizeOfString(value) + NmeaProtocol.SizeOfSeparator();
    }

    #endregion

    #region Status

    protected void ReadDataStatus(ref ReadOnlySpan<char> buffer, out NmeaDataStatus? status, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadDataStatus(token, out status);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(NmeaDataStatus)} is required: '...{buffer}'");
        }

        status = null;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteDataStatus(ref Span<byte> buffer, in NmeaDataStatus? status)
    {
        NmeaProtocol.WriteDataStatus(ref buffer, in status);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfStatus(NmeaDataStatus? status) => NmeaProtocol.SizeOfStatus(status) + NmeaProtocol.SizeOfSeparator();

    #endregion
    
    #region Status

    protected void ReadPositioningSystemMode(ref ReadOnlySpan<char> buffer, out NmeaPositioningSystemMode? status, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadPositioningSystemMode(token, out status);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(NmeaPositioningSystemMode)} is required: '...{buffer}'");
        }

        status = null;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WritePositioningSystemMode(ref Span<byte> buffer, in NmeaPositioningSystemMode? status)
    {
        NmeaProtocol.WritePositioningSystemMode(ref buffer, in status);
        NmeaProtocol.WriteSeparator(ref buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfPositioningSystemMode(NmeaPositioningSystemMode? status) 
        => NmeaProtocol.SizeOfPositioningSystemMode(status) + NmeaProtocol.SizeOfSeparator();

    #endregion


    #region FixMode

    protected void ReadFixMode(ref ReadOnlySpan<char> buffer, out NmeaFixQuality? value, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadFixMode(token, out value);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(NmeaFixQuality)} is required: '...{buffer}'");
        }

        value = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteFixMode(ref Span<byte> buffer, in NmeaFixQuality? value)
    {
        NmeaProtocol.WriteFixMode(ref buffer, in value);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfFixMode(in NmeaFixQuality? fixMode)
    {
        return NmeaProtocol.SizeOfFixMode(fixMode) + NmeaProtocol.SizeOfSeparator();
    }

    #endregion

    #region DopMod

    protected void ReadDopMode(ref ReadOnlySpan<char> buffer, out NmeaDopMode? value, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadDopMode(token, out value);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(NmeaDopMode)} is required: '...{buffer}'");
        }

        value = null;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteDopMode(ref Span<byte> buffer, in NmeaDopMode? value)
    {
        NmeaProtocol.WriteDopMode(ref buffer, in value);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfDopMode(in NmeaDopMode? value)
    {
        return NmeaProtocol.SizeOfDopMode(in value) + NmeaProtocol.SizeOfSeparator();
    }

    #endregion
    
    #region Hex

    protected void ReadHex(ref ReadOnlySpan<char> buffer, out int? value, bool required)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadHex(token, out value);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(NmeaDopMode)} is required: '...{buffer}'");
        }

        value = null;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteHex(ref Span<byte> buffer, int? value,in NmeaHexFormat format)
    {
        NmeaProtocol.WriteHex(ref buffer, value,in format);
        NmeaProtocol.WriteSeparator(ref buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfHex(in int? systemId, in NmeaHexFormat format)
    {
        return NmeaProtocol.SizeOfHex(systemId, format) + NmeaProtocol.SizeOfSeparator();  
    } 

    #endregion
    
    #region PositionFixStatus

    protected void ReadPositionFixStatus(ref ReadOnlySpan<char> buffer, out NmeaPositionFixStatus? value, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadPositionFixStatus(token, out value);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(NmeaPositionFixStatus)} is required: '...{buffer}'");
        }

        value = null;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WritePositionFixStatus(ref Span<byte> buffer, NmeaPositionFixStatus? value)
    {
        NmeaProtocol.WritePositionFixStatus(ref buffer, value);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfPositionFixStatus(NmeaPositionFixStatus? status)
    {
        return NmeaProtocol.SizeOfPositionFixStatus(status) + NmeaProtocol.SizeOfSeparator();
    }

    #endregion
    
    #region Date

    protected void ReadDate(ref ReadOnlySpan<char> buffer, out DateTime? value, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token) != false)
        {
            NmeaProtocol.ReadDate(token, out value);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "Date is required");
        }

        value = null;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteDate(ref Span<byte> buffer, DateTime? date)
    {
        NmeaProtocol.WriteDate(ref buffer, date);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfDate(DateTime? date)
    {
        return NmeaProtocol.SizeOfDate(date) + NmeaProtocol.SizeOfSeparator();
    }

    #endregion

    #region MagneticVariationDirection
    
    protected void ReadMagneticVariationDirection(ref ReadOnlySpan<char> buffer, out NmeaMagneticVariationDirection? value, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadMagneticVariationDirection(token, out value);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(NmeaMagneticVariationDirection)} is required: '...{buffer}'");
        }

        value = null;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteMagneticVariationDirection(ref Span<byte> buffer, NmeaMagneticVariationDirection? value)
    {
        NmeaProtocol.WriteMagneticVariationDirection(ref buffer, value);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfMagneticVariationDirection(NmeaMagneticVariationDirection? value)
    {
        return NmeaProtocol.SizeOfMagneticVariationDirection(value) + NmeaProtocol.SizeOfSeparator();
    }
    #endregion
    
    #region TrueTrack

    protected void ReadTrueTrack(ref ReadOnlySpan<char> buffer, out double value, out TrueTrackUnit? unit, bool required = true)
    {       
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var valueToken) != false)
        {
            if (NmeaProtocol.TryReadNextToken(ref buffer, out var unitToken) == false)
            {
                throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "True track unit field is required");
            }
            NmeaProtocol.ReadDouble(valueToken, out value);
            NmeaProtocol.ReadTrueTrackUnit(unitToken, out unit);
            return;
        }
        
        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"True track field is required: '...{buffer}'");
        }

        value = double.NaN;
        unit = null;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteTrueTrack(ref Span<byte> buffer, in double value, in TrueTrackUnit? unit)
    {
        NmeaProtocol.WriteDouble(ref buffer, in value, NmeaDoubleFormat.Double1X3 );
        NmeaProtocol.WriteSeparator(ref buffer);
        NmeaProtocol.WriteTrueTrackUnit(ref buffer, unit);
        NmeaProtocol.WriteSeparator(ref buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfTrueTrack(in double value, TrueTrackUnit? unit)
    {
        return NmeaProtocol.SizeOfDouble(in value, NmeaDoubleFormat.Double1X3) + NmeaProtocol.SizeOfTrueTrackUnit(unit) + NmeaProtocol.SizeOfSeparator() * 2;
    }

    #endregion

    #region MagneticTrack

    protected void ReadMagneticTrack(ref ReadOnlySpan<char> buffer, out double value, out MagneticTrackUnit? unit, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var valueToken) != false)
        {
            if (NmeaProtocol.TryReadNextToken(ref buffer, out var unitToken) == false)
            {
                throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(MagneticTrackUnit)} field is required");
            }
            NmeaProtocol.ReadDouble(valueToken, out value);
            NmeaProtocol.ReadMagneticTrackUnit(unitToken, out unit);
            return;
        }
        
        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(MagneticTrackUnit)} is required: '...{buffer}'");
        }

        value = double.NaN;
        unit = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteMagneticTrack(ref Span<byte> buffer, in double value, in MagneticTrackUnit? unit)
    {
        NmeaProtocol.WriteDouble(ref buffer, in value, NmeaDoubleFormat.Double1X3 );
        NmeaProtocol.WriteSeparator(ref buffer);
        NmeaProtocol.WriteMagneticTrackUnit(ref buffer, unit);
        NmeaProtocol.WriteSeparator(ref buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfMagneticTrack(in double value, MagneticTrackUnit? unit)
    {
        return NmeaProtocol.SizeOfDouble(in value, NmeaDoubleFormat.Double1X3) + NmeaProtocol.SizeOfMagneticTrackUnit(unit) + NmeaProtocol.SizeOfSeparator() * 2;
    }

    #endregion

    #region GroundSpeedKnots

    protected void ReadGroundSpeedKnots(ref ReadOnlySpan<char> buffer, out double value, out GroundSpeedKnotsUnit? unit, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var valueToken))
        {
            if (NmeaProtocol.TryReadNextToken(ref buffer, out var unitToken) == false)
            {
                throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, "Ground speed unit field is required");
            }
            NmeaProtocol.ReadDouble(valueToken, out value);
            NmeaProtocol.ReadGroundSpeedKnotsUnit(unitToken, out unit);
            return;
        }
        
        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"Magnetic track field is required: '...{buffer}'");
        }

        value = double.NaN;
        unit = null;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void WriteGroundSpeedKnots(ref Span<byte> buffer, in double value, in GroundSpeedKnotsUnit? unit)
    {
        NmeaProtocol.WriteDouble(ref buffer, in value, NmeaDoubleFormat.Double1X7 );
        NmeaProtocol.WriteSeparator(ref buffer);
        NmeaProtocol.WriteGroundSpeedKnotsUnit(ref buffer, unit);
        NmeaProtocol.WriteSeparator(ref buffer);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static int SizeOfGroundSpeedKnots(in double value, GroundSpeedKnotsUnit? unit)
    {
        return NmeaProtocol.SizeOfDouble(in value, NmeaDoubleFormat.Double1X7) + NmeaProtocol.SizeOfGroundSpeedKnotsUnit(unit) + NmeaProtocol.SizeOfSeparator() * 2;
    }

    #endregion
    
    #region GroundSpeedKmh

    protected void ReadGroundSpeedKmh(ref ReadOnlySpan<char> buffer, out double value, out GroundSpeedKmhUnit? unit, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var valueToken) != false)
        {
            if (NmeaProtocol.TryReadNextToken(ref buffer, out var unitToken) == false)
            {
                throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(GroundSpeedKmhUnit)} field is required");
            }
            NmeaProtocol.ReadDouble(valueToken, out value);
            NmeaProtocol.ReadGroundSpeedKmhUnit(unitToken, out unit);
            return;
        }
        
        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(GroundSpeedKmhUnit)} field is required: '...{buffer}'");
        }

        value = double.NaN;
        unit = null;
    }

    protected static void WriteGroundSpeedKmh(ref Span<byte> buffer, in double value, in GroundSpeedKmhUnit? unit)
    {
        NmeaProtocol.WriteDouble(ref buffer, in value, NmeaDoubleFormat.Double1X7 );
        NmeaProtocol.WriteSeparator(ref buffer);
        NmeaProtocol.WriteGroundSpeedKmhUnit(ref buffer, unit);
        NmeaProtocol.WriteSeparator(ref buffer);
    }

    protected static int SizeOfGroundSpeedKmh(in double value, GroundSpeedKmhUnit? unit)
    {
        return NmeaProtocol.SizeOfDouble(in value, NmeaDoubleFormat.Double1X7) + NmeaProtocol.SizeOfGroundSpeedKmhUnit(unit) + NmeaProtocol.SizeOfSeparator() * 2;
    }

    #endregion

    #region NavigationStatus

    protected void ReadNavigationStatus(ref ReadOnlySpan<char> buffer, out NmeaNavigationStatus? value, bool required = true)
    {
        if (NmeaProtocol.TryReadNextToken(ref buffer, out var token))
        {
            NmeaProtocol.ReadNavigationStatus(token, out value);
            return;
        }

        if (required)
        {
            throw new ProtocolDeserializeMessageException(NmeaProtocol.Info, this, $"{nameof(NmeaPositionFixStatus)} is required: '...{buffer}'");
        }

        value = null;
    }

    protected static void WriteNavigationStatus(ref Span<byte> buffer, NmeaNavigationStatus? value)
    {
        NmeaProtocol.WriteNavigationStatus(ref buffer, value);
        NmeaProtocol.WriteSeparator(ref buffer);
    }

    protected static int SizeOfNavigationStatus(NmeaNavigationStatus? value)
    {
        return NmeaProtocol.SizeOfNavigationStatus(value) + NmeaProtocol.SizeOfSeparator();
    }

    #endregion
    

}