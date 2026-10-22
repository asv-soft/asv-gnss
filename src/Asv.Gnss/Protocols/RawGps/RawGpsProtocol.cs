using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>GPS L1 C/A LNAV subframes, including transmitted parity and data inversion.</summary>
public static class RawGpsProtocol
{
    public const int SubframeBitSize = 300;
    public const int PacketSize = 38;
    public const int MaxPacketSize = PacketSize;
    public static ProtocolInfo Info { get; } = new("RawGps", "GPS LNAV words");

    public static void RegisterRawGpsProtocol(
        this IProtocolParserBuilder builder,
        Action<IProtocolMessageFactoryBuilder<GpsSubframeBase, byte>>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        var factory = new ProtocolMessageFactoryBuilder<GpsSubframeBase, byte>(Info);
        factory
            .Add<GpsSubframe1>()
            .Add<GpsSubframe2>()
            .Add<GpsSubframe3>()
            .Add<GpsSubframe4>()
            .Add<GpsSubFrame5>();
        configure?.Invoke(factory);
        var messages = factory.Build();
        builder.Register(
            Info,
            (context, statistics) => new GpsSubframeParser(messages, context, statistics)
        );
    }
}
