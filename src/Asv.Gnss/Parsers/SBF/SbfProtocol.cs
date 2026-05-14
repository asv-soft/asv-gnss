using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Septentrio Binary Format protocol registration helpers.
/// </summary>
public static class SbfProtocol
{
    /// <summary>
    /// SBF protocol information.
    /// </summary>
    public static ProtocolInfo Info { get; } = new(SbfBinaryParser.GnssProtocolId, "Septentrio Binary Format");

    /// <summary>
    /// Registers the SBF binary parser with default messages.
    /// </summary>
    public static void RegisterSbfProtocol(this IProtocolParserBuilder builder, Action<IProtocolMessageFactoryBuilder<SbfMessageBase, ushort>>? configure = null)
    {
        var factory = new ProtocolMessageFactoryBuilder<SbfMessageBase, ushort>(Info);
        SbfMessageFactory.RegisterDefaultMessages(factory);
        configure?.Invoke(factory);
        var messageFactory = factory.Build();
        builder.Register(Info, (core, stat) => new SbfBinaryParser(messageFactory, core, stat));
    }
}
