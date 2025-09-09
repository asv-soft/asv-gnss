using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public class UbxMessageFactory : IProtocolMessageFactory<UbxMessageBase, ushort>
{
    private readonly ImmutableDictionary<ushort, Func<UbxMessageBase>> _factory;
    public ProtocolInfo Info => UbxProtocol.Info;

    public UbxMessageFactory()
    {
        var builder = ImmutableDictionary.CreateBuilder<ushort, Func<UbxMessageBase>>(); 
        // builder.Add(UbxMessage.MessageId, () => new UbxMessage());
        
        _factory = builder.ToImmutable();
    }
    public UbxMessageBase? Create(ushort id) => _factory.TryGetValue(id, out var factory) ? factory() : null;
    
    public IEnumerable<ushort> GetSupportedIds() => _factory.Keys.Select(x=>x);
}