using System;
using Asv.IO;

namespace Asv.Gnss;

public static class AsvProtocol
{
    public static ProtocolInfo Info { get; } = new("Asv", "ASV");

    public const ushort MaxPayloadSize = 1012;
    public const ushort MaxPacketSize = MaxPayloadSize + HeaderSize + CrcSize;
    public const byte SyncByte1 = 0xAA;
    public const byte SyncByte2 = 0x44;
    public const int HeaderSize = 10;
    public const int CrcSize = 2;

    public static void RegisterAsvProtocol(
        this IProtocolParserBuilder builder,
        Action<IProtocolMessageFactoryBuilder<AsvMessageBase, ushort>>? configure = null)
    {
        var factory = new ProtocolMessageFactoryBuilder<AsvMessageBase, ushort>(Info);
        factory
            .Add<AsvMessageHeartBeat>()
            .Add<AsvMessageGbasVdbSend>()
            .Add<AsvMessageGbasVdbSendV2>()
            .Add<AsvMessageGbasMonDevSendV2>()
            .Add<AsvMessageGpsObservations>()
            .Add<AsvMessageGloObservations>()
            .Add<AsvMessageGpsRawCa>()
            .Add<AsvMessageGloRawCa>()
            .Add<AsvMessagePvtGeo>();

        configure?.Invoke(factory);
        var messageFactory = factory.Build();
        builder.Register(Info, (core, stat) => new AsvMessageParser(messageFactory, core, stat));
    }

    public static ushort ReadPayloadLength(byte[] buffer)
    {
        return (ushort)(buffer[2] | (buffer[3] << 8));
    }

    public static ushort ReadMessageId(byte[] buffer)
    {
        return (ushort)(buffer[8] | (buffer[9] << 8));
    }
}
