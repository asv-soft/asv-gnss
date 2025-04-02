using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;


public class NmeaMessageFactory : IProtocolMessageFactory<NmeaMessageBase, NmeaMessageId>
{
    private readonly ImmutableDictionary<NmeaMessageId,Func<NmeaMessageBase>> _factory;
    public ProtocolInfo Info => NmeaProtocol.Info;

    public NmeaMessageFactory()
    {
        var builder = ImmutableDictionary.CreateBuilder<NmeaMessageId, Func<NmeaMessageBase>>(); 
        builder.Add(NmeaMessageGbs.MessageId, () => new NmeaMessageGbs());
        _factory = builder.ToImmutable();
    }
    public NmeaMessageBase? Create(NmeaMessageId id) => _factory.TryGetValue(id, out var factory) ? factory() : null;
    
    public IEnumerable<NmeaMessageId> GetSupportedIds() => _factory.Keys.Select(x=>x);
}