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
    
    public static BitArray ReadVariableLengthFieldAsBitArray(ref ReadOnlySpan<byte> buffer)
    {
        var length = 0;

        // Определяем длину (пока установлен 1 в младшем бите)
        while ((buffer[length] & 0x01) != 0)
        {
            length++;
        }
        length++; // последний байт, где FX == 0

        // Количество битов данных: 7 битов на байт
        var totalBits = length * 7;
        var arr = new BitArray(totalBits);

        var bitIndex = 0;

        for (var i = 0; i < length; i++)
        {
            var b = buffer[i];
            // Биты с 7 по 1 (бит 8 самый старший, бит 1 — FX)
            for (var bit = 7; bit >= 1; bit--)
            {
                var bitValue = ((b >> bit) & 0x01) != 0;
                arr[bitIndex++] = bitValue;
            }
        }

        // Сдвигаем буфер на остаток
        buffer = buffer[length..];

        return arr;
    }

}