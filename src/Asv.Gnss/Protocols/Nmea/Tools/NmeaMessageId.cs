using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss;



public readonly struct NmeaMessageId : IEquatable<NmeaMessageId>, ISizedSpanSerializable
{
    public NmeaMessageId(string messageId) 
    {
        MessageId = messageId;        
    }
    
    public NmeaMessageId(ReadOnlySpan<char> messageId) 
    {
        MessageId = new string(messageId);        
    }

    public string MessageId { get; }

    public override string ToString()
    {
        return MessageId;   
    }

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        throw new NotImplementedException($"We don't need this method for {nameof(NmeaMessageId)} cause it's readonly struct");
    }

    public void Serialize(ref Span<byte> buffer)
    {
        var slice = NmeaProtocol.Encoding.GetBytes(MessageId, buffer);
        buffer = buffer[slice..];
    }

    public int GetByteSize()
    {
        return NmeaProtocol.Encoding.GetByteCount(MessageId);
    }

    public bool Equals(NmeaMessageId other)
    {
        return string.Equals(MessageId, other.MessageId, StringComparison.InvariantCultureIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return obj is NmeaMessageId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return StringComparer.InvariantCultureIgnoreCase.GetHashCode(MessageId);
    }

    public static bool operator ==(NmeaMessageId left, NmeaMessageId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(NmeaMessageId left, NmeaMessageId right)
    {
        return !left.Equals(right);
    }

    
}