using System;
using System.Numerics;
using Asv.IO;

namespace Asv.Gnss;

internal static class RawGpsCodec
{
    // IS-GPS-200N, Table 20-XIV. Each mask selects source bits d1 through d24.
    private static ReadOnlySpan<uint> ParityMasks =>
        [0xEC7CD2, 0x763E69, 0xBB1F34, 0x5D8F9A, 0xAEC7CD, 0x2DEA27];

    internal static byte Parity(uint data, byte previous)
    {
        uint result = 0;
        for (var index = 0; index < 6; index++)
        {
            var previousBit = index is 0 or 2 or 5 ? previous >> 1 : previous;
            result =
                (result << 1)
                | ((uint)(BitOperations.PopCount(data & ParityMasks[index]) ^ previousBit) & 1);
        }
        return (byte)result;
    }

    internal static uint Read(ReadOnlySpan<byte> data, int position, int length)
    {
        uint result = 0;
        for (var bit = position; bit < position + length; bit++)
            result = (result << 1) | (uint)((data[bit / 8] >> (7 - bit % 8)) & 1);
        return result;
    }

    internal static void Write(Span<byte> data, int position, int length, uint value)
    {
        for (var index = 0; index < length; index++)
        {
            var bit = position + index;
            var mask = (byte)(1 << (7 - bit % 8));
            data[bit / 8] = (byte)(
                (data[bit / 8] & ~mask) | (((value >> (length - index - 1)) & 1) != 0 ? mask : 0)
            );
        }
    }

    internal static bool TryDecode(ReadOnlySpan<byte> frame, Span<byte> source, out byte previous)
    {
        previous = 0;
        if (frame.Length != RawGpsProtocol.PacketSize || (frame[0] & 0xF0) != 0)
            return false;
        for (byte candidate = 0; candidate < 4; candidate++)
        {
            var state = candidate;
            var valid = true;
            for (var index = 0; index < 10; index++)
            {
                var word = Read(frame, 4 + index * 30, 30);
                var data = word >> 6;
                if ((state & 1) != 0)
                    data ^= 0xFFFFFF;
                if (Parity(data, state) != (word & 63))
                {
                    valid = false;
                    break;
                }
                source[index * 3] = (byte)(data >> 16);
                source[index * 3 + 1] = (byte)(data >> 8);
                source[index * 3 + 2] = (byte)data;
                state = (byte)(word & 3);
            }
            if (
                valid
                && source[0] == GpsRawHelper.GpsSubframePreamble
                && (source[5] >> 2 & 7) is >= 1 and <= 5
            )
            {
                previous = candidate;
                return true;
            }
        }
        return false;
    }

    internal static void Encode(
        ReadOnlySpan<byte> source,
        Span<byte> frame,
        byte previous,
        bool solveNonInformationBits
    )
    {
        if (previous > 3)
            throw new ArgumentOutOfRangeException(nameof(previous));
        frame.Clear();
        for (var index = 0; index < 10; index++)
        {
            var data = Read(source, index * 24, 24);
            if (solveNonInformationBits && index is 1 or 9)
            {
                // HOW and the final word select d23/d24 so the next word starts with D29*=D30*=0.
                for (uint candidate = 0; candidate < 4; candidate++)
                {
                    var adjusted = (data & 0xFFFFFC) | candidate;
                    if ((Parity(adjusted, previous) & 3) == 0)
                    {
                        data = adjusted;
                        break;
                    }
                }
            }
            var parity = Parity(data, previous);
            var transmitted = (previous & 1) != 0 ? data ^ 0xFFFFFF : data;
            Write(frame, 4 + index * 30, 30, (transmitted << 6) | parity);
            previous = (byte)(parity & 3);
        }
    }

    internal static void Unsigned(
        Span<byte> data,
        int position,
        int bits,
        double value,
        double scale = 1
    ) => Scaled(data, position, bits, value, scale, false);

    internal static void Signed(
        Span<byte> data,
        int position,
        int bits,
        double value,
        double scale = 1
    ) => Scaled(data, position, bits, value, scale, true);

    private static void Scaled(
        Span<byte> data,
        int position,
        int bits,
        double value,
        double scale,
        bool signed
    )
    {
        var number = Math.Round(value / scale);
        var minimum = signed ? -Math.Pow(2, bits - 1) : 0;
        var maximum = Math.Pow(2, signed ? bits - 1 : bits) - 1;
        if (!double.IsFinite(value) || number < minimum || number > maximum)
            throw new ArgumentOutOfRangeException(
                nameof(value),
                $"Value does not fit the {bits}-bit GPS field."
            );
        Write(data, position, bits, unchecked((uint)(long)number));
    }

    internal static void AlmanacClock(Span<byte> data, double af0, double af1)
    {
        // Preserve the historical decoded Af0 convention, including its non-unique split encoding.
        var highBits = (int)Read(data, 235, 3);
        var high = highBits >= 4 ? highBits - 8 : highBits;
        var low = Read(data, 216, 8);
        var original = high * GpsRawHelper.P2_17 + low * GpsRawHelper.P2_20;
        if (af0 != original)
        {
            var target = Math.Round(af0 / GpsRawHelper.P2_20);
            if (!double.IsFinite(af0) || target is < -32 or > 279)
                throw new ArgumentOutOfRangeException(
                    nameof(af0),
                    "Af0 exceeds the legacy decoded almanac range."
                );
            high = Math.Clamp(
                high,
                (int)Math.Ceiling((target - 255) / 8),
                (int)Math.Floor(target / 8)
            );
            high = Math.Clamp(high, -4, 3);
            Unsigned(data, 216, 8, target - high * 8);
            Signed(data, 235, 3, high);
        }
        Signed(data, 224, 11, af1, GpsRawHelper.P2_38);
    }
}
