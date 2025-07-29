using System;
using System.Buffers.Binary;
using System.Collections;
using System.Runtime.CompilerServices;
using Asv.IO;

namespace Asv.Gnss;

public static class AsterixProtocol 
{
    
    public static ProtocolInfo Info { get; } = new("ASTERIX", "ASTERIX Protocol");
    
    public static void RegisterAsterixProtocol(this IProtocolParserBuilder builder, Action<IProtocolMessageFactoryBuilder<AsterixMessage, byte>>? configure = null)
    {
        var factory = new ProtocolMessageFactoryBuilder<AsterixMessage, byte>(Info);
        // register default messages
        factory
            .Add<AsterixMessageI020>();
            //.Add<AsterixMessageI021>();
        configure?.Invoke(factory);
        var messageFactory = factory.Build();
        builder.Register(Info, (core,stat) => new AsterixMessageParser(messageFactory, core, stat));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadLength(ref ReadOnlySpan<byte> span)
    {
        var result = BinaryPrimitives.ReadUInt16BigEndian(span);
        span = span[2..];
        return result;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLength(ref Span<byte> buffer, int length)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)length);
        buffer = buffer[2..];
    }
    

}