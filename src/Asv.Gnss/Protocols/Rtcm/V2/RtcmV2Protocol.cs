using System;
using Asv.IO;

namespace Asv.Gnss;

public static class RtcmV2Protocol
{
    public static ProtocolInfo Info { get; } = new("RtcmV2", "RTCM v2");

    public const string GnssProtocolId = "RtcmV2";
    public const byte SyncByte = 0x66;
    public const int MaxMessageSize = 33 * 3;

    public static void RegisterRtcmV2Protocol(
        this IProtocolParserBuilder builder,
        Action<IProtocolMessageFactoryBuilder<RtcmV2MessageBase, ushort>>? configure = null)
    {
        var factory = new ProtocolMessageFactoryBuilder<RtcmV2MessageBase, ushort>(Info);
        factory
            .Add<RtcmV2Message1>()
            .Add<RtcmV2Message14>()
            .Add<RtcmV2Message15>()
            .Add<RtcmV2Message17>()
            .Add<RtcmV2Message21>()
            .Add<RtcmV2Message31>();

        configure?.Invoke(factory);
        var messageFactory = factory.Build();
        builder.Register(Info, (core, stat) => new RtcmV2Parser(messageFactory, core, stat));
    }
}
