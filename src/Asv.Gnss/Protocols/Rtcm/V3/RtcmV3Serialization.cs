using System;
using Asv.IO;

namespace Asv.Gnss;

internal static class RtcmV3Serialization
{
    public static void WriteS(Span<byte> buffer, ref int bitIndex, int bits, double value, double scale)
    {
        var raw = Math.Round(value / scale, MidpointRounding.AwayFromZero);
        if (!double.IsFinite(raw) || raw < -Math.Pow(2, bits - 1) || raw >= Math.Pow(2, bits - 1))
            throw new ArgumentOutOfRangeException(nameof(value));
        SpanBitHelper.SetBitS(buffer, ref bitIndex, bits, (int)raw);
    }

    public static void WriteU(Span<byte> buffer, ref int bitIndex, int bits, double value, double scale)
    {
        var raw = Math.Round(value / scale, MidpointRounding.AwayFromZero);
        if (!double.IsFinite(raw) || raw < 0 || raw >= Math.Pow(2, bits))
            throw new ArgumentOutOfRangeException(nameof(value));
        SpanBitHelper.SetBitU(buffer, ref bitIndex, bits, (uint)raw);
    }

    public static void WriteG(Span<byte> buffer, ref int bitIndex, int bits, double value, double scale)
    {
        var raw = Math.Round(value / scale, MidpointRounding.AwayFromZero);
        if (!double.IsFinite(raw) || Math.Abs(raw) >= Math.Pow(2, bits - 1))
            throw new ArgumentOutOfRangeException(nameof(value));
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 1, double.IsNegative(raw) ? 1 : 0);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, bits - 1, (uint)Math.Abs(raw));
    }

    public static int StringBitSize(string? value)
    {
        if (value is { Length: > 255 }) throw new ArgumentOutOfRangeException(nameof(value));
        return 8 + (value?.Length ?? 0) * 8;
    }

    public static void WriteString(Span<byte> buffer, ref int bitIndex, string? value)
    {
        StringBitSize(value);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 8, value?.Length ?? 0);
        if (value == null) return;
        foreach (var character in value)
        {
            if (character > byte.MaxValue) throw new ArgumentOutOfRangeException(nameof(value));
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 8, character);
        }
    }

    public static void WriteColumn(Span<byte> buffer, ref int bitIndex, int bits, int[] values, int count, bool signed)
    {
        if (values.Length != count) throw new ArgumentException("MSM column length does not match its mask.", nameof(values));
        foreach (var value in values)
        {
            if (signed) WriteS(buffer, ref bitIndex, bits, value, 1);
            else WriteU(buffer, ref bitIndex, bits, value, 1);
        }
    }

    public static double SecondsOfWeek(DateTime time)
    {
        var week = 0;
        var seconds = 0.0;
        RtcmV3Helper.GetFromTime(time, ref week, ref seconds);
        return seconds;
    }
}
