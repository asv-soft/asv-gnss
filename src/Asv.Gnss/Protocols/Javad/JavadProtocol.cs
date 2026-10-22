using System;
using Asv.IO;

namespace Asv.Gnss;

public static class JavadProtocol
{
    public const int HeaderSize = 5;
    public const int MaxBodySize = 0xFFF;
    public const int MaxPacketSize = HeaderSize + MaxBodySize;
    public static ProtocolInfo Info { get; } = new("Javad", "Javad GREIS");

    public static void RegisterJavadProtocol(
        this IProtocolParserBuilder builder,
        Action<IProtocolMessageFactoryBuilder<JavadMessageBase, string>>? configure = null
    )
    {
        ArgumentNullException.ThrowIfNull(builder);
        var factory = new ProtocolMessageFactoryBuilder<JavadMessageBase, string>(Info);
        JavadBinaryMessageFactory.Register(factory);
        JavadTextMessageFactory.Register(factory);
        configure?.Invoke(factory);
        var messages = factory.Build();
        builder.Register(
            Info,
            (context, statistic) => new JavadMessageParser(messages, context, statistic)
        );
    }

    internal static bool IsIdentifierByte(byte value) => value is >= 0x30 and <= 0x7E;

    internal static int HexValue(byte value) =>
        value switch
        {
            >= (byte)'0' and <= (byte)'9' => value - '0',
            >= (byte)'A' and <= (byte)'F' => value - 'A' + 10,
            _ => -1,
        };

    internal static byte HexDigit(int value) => (byte)(value < 10 ? '0' + value : 'A' + value - 10);

    internal static bool TryReadBodyLength(ReadOnlySpan<byte> frame, out int length)
    {
        length = 0;
        if (frame.Length < HeaderSize || !IsIdentifierByte(frame[0]) || !IsIdentifierByte(frame[1]))
        {
            return false;
        }
        for (var i = 2; i < HeaderSize; i++)
        {
            var digit = HexValue(frame[i]);
            if (digit < 0)
            {
                return false;
            }
            length = (length << 4) | digit;
        }
        return true;
    }

    internal static void WriteHeader(Span<byte> frame, string id, int bodySize)
    {
        ValidateIdentifier(id);
        if (bodySize is < 0 or > MaxBodySize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bodySize),
                "A GREIS body cannot exceed 4095 bytes."
            );
        }
        frame[0] = (byte)id[0];
        frame[1] = (byte)id[1];
        frame[2] = HexDigit((bodySize >> 8) & 15);
        frame[3] = HexDigit((bodySize >> 4) & 15);
        frame[4] = HexDigit(bodySize & 15);
    }

    internal static void ValidateIdentifier(string id)
    {
        ArgumentNullException.ThrowIfNull(id);
        if (id.Length != 2 || id[0] is < '0' or > '~' || id[1] is < '0' or > '~')
        {
            throw new ArgumentException(
                "A GREIS identifier must contain two ASCII characters from '0' through '~'.",
                nameof(id)
            );
        }
    }
}
