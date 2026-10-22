using System;

namespace Asv.Gnss;

internal static class JavadBinaryPayload
{
    public static int GetRemainingCount(int length, int trailingBytes, int elementSize)
    {
        var available = length - trailingBytes;
        if (available < 0 || available % elementSize != 0)
        {
            throw new FormatException("GREIS payload does not contain a whole number of records.");
        }
        return available / elementSize;
    }

    public static void RequireAvailable(int available, int count, int elementSize)
    {
        if (count < 0 || count > available / elementSize)
        {
            throw new FormatException("GREIS array count exceeds the available payload.");
        }
    }

    public static void RequireLength<T>(T[] values, int length, string name)
    {
        ArgumentNullException.ThrowIfNull(values, name);
        if (values.Length != length)
        {
            throw new ArgumentException($"{name} must contain {length} elements.", name);
        }
    }

    public static void RequireAscii(string value, int length, string name)
    {
        ArgumentNullException.ThrowIfNull(value, name);
        if (value.Length > length)
        {
            throw new ArgumentException($"{name} exceeds its {length}-character field.", name);
        }
        foreach (var character in value)
        {
            if (character > 127)
            {
                throw new ArgumentException("GREIS text fields require ASCII characters.", name);
            }
        }
    }
}

public partial class JavadMessagesFormatMessage
{
    private void ValidateFormat()
    {
        if (FormatIdentifier != "JP" || MajorVer.Length != 2 || MinorVer.Length != 2
            || !IsDecimal(MajorVer) || !IsDecimal(MinorVer) || Order is not ('0' or '1'))
        {
            throw new FormatException("Invalid GREIS message format identifier, version, or byte order.");
        }
    }

    private static bool IsDecimal(string value) => value[0] is >= '0' and <= '9'
        && value[1] is >= '0' and <= '9';
}

public partial class JavadAntennaInformationMessage
{
    private void ValidateCoordinates()
    {
        if (Apc.Length is not (0 or 3) || Arp.Length is not (0 or 3))
        {
            throw new FormatException("GREIS antenna coordinates require zero or three components.");
        }
    }
}
