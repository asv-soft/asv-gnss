using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;


public class NmeaMessageFactory : IProtocolMessageFactory<NmeaMessage, NmeaMessageId>
{
    private readonly ImmutableDictionary<NmeaMessageId,Func<NmeaMessage>> _factory;
    public ProtocolInfo Info => NmeaProtocol.Info;

    public NmeaMessageFactory()
    {
        var builder = ImmutableDictionary.CreateBuilder<NmeaMessageId, Func<NmeaMessage>>(); 
        builder.Add(NmeaMessageGbs.MessageId, () => new NmeaMessageGbs());
        _factory = builder.ToImmutable();
    }
    public NmeaMessage? Create(NmeaMessageId id) => _factory.TryGetValue(id, out var factory) ? factory() : null;
    
    public IEnumerable<NmeaMessageId> GetSupportedIds() => _factory.Keys.Select(x=>x);
}