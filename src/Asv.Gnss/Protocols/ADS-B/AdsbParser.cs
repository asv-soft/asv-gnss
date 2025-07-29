using System;
using Asv.IO;

namespace Asv.Gnss;

public class AdsbParser : ProtocolParser<AdsbDfMessage, ushort>
{
    public AdsbParser(IProtocolMessageFactory<AdsbDfMessage, ushort> messageFactory, IProtocolContext context, IStatisticHandler? statisticHandler) 
        : base(messageFactory, context, statisticHandler)
    {
        
    }

    public override bool Push(byte data)
    {
        throw new NotImplementedException();
    }

    public override void Reset()
    {
        throw new NotImplementedException();
    }

    public override ProtocolInfo Info => AdsbProtocol.Info;
}

public class AdsbDfMessage : IProtocolMessage<ushort>
{
    public ProtocolInfo Protocol => AdsbProtocol.Info;
    public string Name { get; }
    public ushort Id { get; }
    
    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public void Serialize(ref Span<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public int GetByteSize()
    {
        throw new NotImplementedException();
    }

    public ref ProtocolTags Tags => throw new NotImplementedException();

    public string GetIdAsString()
    {
        throw new NotImplementedException();
    }

    
}