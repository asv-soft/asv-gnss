using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public class RtcmV3MessageFactory : IProtocolMessageFactory<RtcmV3MessageBase, ushort>
{
    private readonly ImmutableDictionary<ushort, Func<RtcmV3MessageBase>> _factory;
    public ProtocolInfo Info => RtcmV3Protocol.Info;

    public RtcmV3MessageFactory()
    {
        var builder = ImmutableDictionary.CreateBuilder<ushort, Func<RtcmV3MessageBase>>(); 
        builder.Add(RtcmV3Message1005.MessageId, () => new RtcmV3Message1005());
        builder.Add(RtcmV3Message1006.MessageId, () => new RtcmV3Message1006());
        builder.Add(RtcmV3Message1230.MessageId, () => new RtcmV3Message1230());
        builder.Add(RtcmV3Msm4Msg1074.MessageId, () => new RtcmV3Msm4Msg1074());
        builder.Add(RtcmV3Msm4Msg1084.MessageId, () => new RtcmV3Msm4Msg1084());
        builder.Add(RtcmV3Msm4Msg1094.MessageId, () => new RtcmV3Msm4Msg1094());
        builder.Add(RtcmV3Msm4Msg1104.MessageId, () => new RtcmV3Msm4Msg1104());
        builder.Add(RtcmV3Msm4Msg1114.MessageId, () => new RtcmV3Msm4Msg1114());
        builder.Add(RtcmV3Msm4Msg1124.MessageId, () => new RtcmV3Msm4Msg1124());
        _factory = builder.ToImmutable();
    }
    public RtcmV3MessageBase? Create(ushort id) => _factory.TryGetValue(id, out var factory) ? factory() : null;
    
    public IEnumerable<ushort> GetSupportedIds() => _factory.Keys.Select(x=>x);
}