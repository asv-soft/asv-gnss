using System;
using Asv.IO;

namespace Asv.Gnss;

public static class AdsbProtocol
{
    public static ProtocolInfo Info { get; } = new("ADS-B", "ADS-B Protocol");
    
    public static void RegisterAdsbProtocol(this IProtocolParserBuilder builder, Action<IProtocolMessageFactoryBuilder<AdsbDfMessage, ushort>>? configure = null)
    {
        var factory = new ProtocolMessageFactoryBuilder<AdsbDfMessage, ushort>(Info);
        // Every five-bit DF is retained, including formats whose payload is not decoded.
        for (ushort df = 0; df < 32; df++) factory.Add<AdsbDfMessage>(df);
        configure?.Invoke(factory);
        var messageFactory = factory.Build();
        builder.Register(Info, (core,stat) => new AdsbParser(messageFactory, core, stat));
    }
}