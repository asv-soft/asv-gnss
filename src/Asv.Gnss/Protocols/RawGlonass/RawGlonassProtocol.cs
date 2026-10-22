using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>Legacy GLONASS L1/L2 navigation strings, including their Hamming check bits.</summary>
public static class RawGlonassProtocol
{
    public const int StringBitSize = 85;
    public const int PacketSize = 11;
    public static ProtocolInfo Info { get; } = new("RawGlonass", "GLONASS navigation strings");

    /// <summary>
    /// Registers a dedicated navigation-bit parser. Input bytes carry MSB-first decoded data bits,
    /// without relative coding, meander, or time marks. Both contiguous strings and the existing
    /// eleven-byte representation with three leading padding bits are accepted.
    /// </summary>
    public static void RegisterRawGlonassProtocol(
        this IProtocolParserBuilder builder,
        Action<IProtocolMessageFactoryBuilder<GlonassWordBase, byte>>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        var factory = new ProtocolMessageFactoryBuilder<GlonassWordBase, byte>(Info);
        factory
            .Add<GlonassWord1>()
            .Add<GlonassWord2>()
            .Add<GlonassWord3>()
            .Add<GlonassWord4>()
            .Add<GlonassWord5>()
            .Add<GlonassWord6>()
            .Add<GlonassWord7>()
            .Add<GlonassWord8>()
            .Add<GlonassWord9>()
            .Add<GlonassWord10>()
            .Add<GlonassWord11>()
            .Add<GlonassWord12>()
            .Add<GlonassWord13>()
            .Add<GlonassWord14>()
            .Add<GlonassWord15>();
        configure?.Invoke(factory);
        var messages = factory.Build();
        builder.Register(
            Info,
            (context, statistic) => new GlonassWordParser(messages, context, statistic)
        );
    }
}
