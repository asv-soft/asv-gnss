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
        
        if (NmeaProtocol.TryGetMessageIdAndTalkerId(buffer, out var msgId, out _talkerId))
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
            
        }
        finally
        {
            ArrayPool<char>.Shared.Return(charBuffer);
        }
    }

    public virtual void Serialize(ref Span<byte> buffer)
    {
        
    }
   
    public abstract int GetByteSize();

    public ref ProtocolTags Tags => ref _tags;

    public string GetIdAsString() => Id.ToString();

    public ProtocolInfo Protocol => NmeaProtocol.ProtocolInfo;
    public abstract string Name { get; }
    public abstract NmeaMessageId Id { get; }
    
    public NmeaTalkerId TalkerId { get; set; }
}