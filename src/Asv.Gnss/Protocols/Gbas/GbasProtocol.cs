using System;
using Asv.IO;

namespace Asv.Gnss;

public static class GbasProtocol
{
    public static ProtocolInfo Info { get; } = new("Gbas", "GBAS");
    public const int MaxPacketSize = (48 + 25 + GbasMessage.MaxAppDataSize * 8 + 48 + 2 + 7) / 8;

    public static void RegisterGbasProtocol(
        this IProtocolParserBuilder builder,
        Action<IProtocolMessageFactoryBuilder<GbasMessageBlockBase, byte>>? configure = null)
    {
        var factory = new ProtocolMessageFactoryBuilder<GbasMessageBlockBase, byte>(Info);
        factory
            .Add<MessageBlockType1>()
            .Add<MessageBlockType2>()
            .Add<MessageBlockType3>()
            .Add<MessageBlockType4>()
            .Add<MessageBlockType5>()
            .Add<MessageBlockType11>()
            .Add<MessageBlockType101>();
        configure?.Invoke(factory);
        var messageFactory = factory.Build();
        builder.Register(Info, (context, statistic) => new GbasMessageParser(messageFactory, context, statistic));
    }
}
