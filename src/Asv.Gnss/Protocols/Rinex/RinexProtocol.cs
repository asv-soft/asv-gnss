using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>Registration and implementation buffering limits for the native RINEX file protocol.</summary>
public static class RinexProtocol
{
    /// <summary>Protocol identity used by the Asv.IO registration and parser APIs.</summary>
    public static ProtocolInfo Info { get; } = new("Rinex", "RINEX");
    /// <summary>Maximum buffered ASCII line length in bytes, independent of format-level record limits.</summary>
    public const int MaxLineLength = 1024 * 1024;
    /// <summary>Maximum buffered record or header line count.</summary>
    public const int MaxRecordLines = 32768;
    /// <summary>Maximum buffered record or header size in bytes.</summary>
    public const int MaxRecordBytes = 16 * 1024 * 1024;

    /// <summary>Registers native header, observation, navigation and meteorological record factories.</summary>
    public static void RegisterRinexProtocol(this IProtocolParserBuilder builder,
        Action<IProtocolMessageFactoryBuilder<RinexMessageBase, RinexMessageId>>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var factory = new ProtocolMessageFactoryBuilder<RinexMessageBase, RinexMessageId>(Info);
        factory.Add<RinexHeaderMessage>().Add<RinexObservationEpochMessage>()
            .Add<RinexNavigationMessage>().Add<RinexMeteorologicalMessage>();
        configure?.Invoke(factory);
        var messages = factory.Build();
        builder.Register(Info, (context, statistic) => new RinexMessageParser(messages, context, statistic));
    }
}
