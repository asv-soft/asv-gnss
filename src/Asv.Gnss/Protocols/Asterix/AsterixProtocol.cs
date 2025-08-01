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
    
    private static void SetSquawkBits(out byte x1, out byte x2, out byte x4, byte value)
    {
        if ((value & 0xF8) != 0) throw new ArgumentOutOfRangeException(nameof(value));
        x1 = (byte)(value & 0x1);
        x2 = (byte)((value & 0x2) >> 1);
        x4 = (byte)((value & 0x4) >> 2);
    }

    public static ushort GetSquawk(ushort id)
    {
        var a = (byte)(((id & (1 << 11)) >> 11) | ((id & (1 << 9)) >> 8) | ((id & (1 << 7)) >> 5));
        var b = (byte)(((id & (1 << 5)) >> 5) | ((id & (1 << 3)) >> 2) | ((id & (1 << 1)) << 1));
        var c = (byte)(((id & (1 << 12)) >> 12) | ((id & (1 << 10)) >> 9) | ((id & (1 << 8)) >> 6));
        var d = (byte)(((id & (1 << 4)) >> 4) | ((id & (1 << 2)) >> 1) | ((id & 1) << 2));

        return (ushort)(a * 1000 + b * 100 + c * 10 + d);
    }

    public static ushort SetSquawk(ushort squawk)
    {
        var a = (byte)((squawk / 1000) % 10);
        var b = (byte)((squawk / 100) % 10);
        var c = (byte)((squawk / 10) % 10);
        var d = (byte)(squawk % 10);

        SetSquawkBits(out var a1, out var a2, out var a4, a);
        SetSquawkBits(out var b1, out var b2, out var b4, b);
        SetSquawkBits(out var c1, out var c2, out var c4, c);
        SetSquawkBits(out var d1, out var d2, out var d4, d);

        return (ushort)((c1 << 12) | (a1 << 11) | (c2 << 10) | (a2 << 9) | (c4 << 8) | (a4 << 7) | (b1 << 5) | (d1 << 4) |
                        (b2 << 3) | (d2 << 2) | (b4 << 1) | d4);
    }
    
    
    

}