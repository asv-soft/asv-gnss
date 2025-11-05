using System;
using Asv.IO;

namespace Asv.Gnss;

public static class AdsbProtocol
{
    public static ProtocolInfo Info { get; } = new("ADS-B", "ADS-B Protocol");
    
    public static void RegisterAdsbProtocol(this IProtocolParserBuilder builder, Action<IProtocolMessageFactoryBuilder<AdsbDfMessage, ushort>>? configure = null)
    {
        var factory = new ProtocolMessageFactoryBuilder<AdsbDfMessage, ushort>(Info);
        // register default messages
        //factory.Add<AsterixMessageI021>();
        configure?.Invoke(factory);
        var messageFactory = factory.Build();
        builder.Register(Info, (core,stat) => new AdsbParser(messageFactory, core, stat));
    }
}