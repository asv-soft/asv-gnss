using System;
using System.Text;

namespace Asv.Gnss;



public readonly struct NmeaMessageId : IEquatable<NmeaMessageId>
{
    public static NmeaMessageId Unknown = new(NmeaProtocol.UnknownMessageId);
    
    public NmeaMessageId(string messageId) 
        : this(messageId.AsSpan())
    {
        
    }

    public NmeaMessageId(ReadOnlySpan<char> msgId)
    {
        if (msgId.IsEmpty)
        {
            throw new ArgumentException("MessageId is empty", nameof(msgId));
        }

        if (msgId[0] == NmeaProtocol.ProprietaryPrefix)
        {
            MessageId = new string(msgId);
        }
        
        if (msgId.Length < 5)
        {
            throw new ArgumentException("MessageId is too short", nameof(msgId));
        }
        
        if (msgId[0] == NmeaProtocol.TalkerIgnoreSymbol && msgId[1] == NmeaProtocol.ProprietaryPrefix)
        {
            MessageId = new string(msgId);
        }

        MessageId = new StringBuilder(msgId.Length)
            .Append(NmeaProtocol.TalkerIgnoreSymbol)
            .Append(NmeaProtocol.TalkerIgnoreSymbol)
            .Append(msgId[2..]).ToString();
    }
    public string MessageId { get; }
    public bool IsProprietary => MessageId[0] == NmeaProtocol.ProprietaryPrefix;
    
    public override string ToString() => MessageId;

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