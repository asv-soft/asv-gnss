using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Asv.Gnss;

/// <summary>Factories for the compound primary object types in GREIS section 4.3.</summary>
public static class JavadParameterValues
{
    public static JavadValue Scheduling(
        double? period = null,
        double? phase = null,
        int? count = null,
        ushort? flags = null
    )
    {
        CheckRange(period, 0, 86400, false, nameof(period));
        CheckRange(phase, 0, 86400, false, nameof(phase));
        if (count is < -256 or >= 32767)
            throw new ArgumentOutOfRangeException(nameof(count));
        return JavadValue.List(
            period.HasValue ? JavadValue.Float(period.Value) : JavadValue.Omitted,
            phase.HasValue ? JavadValue.Float(phase.Value) : JavadValue.Omitted,
            count.HasValue ? JavadValue.Integer(count.Value) : JavadValue.Omitted,
            flags.HasValue ? JavadValue.Hexadecimal(flags.Value) : JavadValue.Omitted
        );
    }

    public static JavadValue IpAddress(IPAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);
        if (address.AddressFamily != AddressFamily.InterNetwork)
            throw new ArgumentException("GREIS ip_address is an IPv4 address.", nameof(address));
        return JavadValue.Atom(address.ToString());
    }

    public static JavadValue Datum(string datum)
    {
        ArgumentException.ThrowIfNullOrEmpty(datum);
        if (
            datum.Length > 5
            || !Regex.IsMatch(datum, "^[A-Z0-9_]+$", RegexOptions.CultureInvariant)
        )
            throw new ArgumentException(
                "A datum identifier contains at most five uppercase characters.",
                nameof(datum)
            );
        return JavadValue.Atom(datum);
    }

    public static JavadValue Cartesian(string datum, double x, double y, double z)
    {
        CheckRange(x, -10000000, 10000000, true, nameof(x));
        CheckRange(y, -10000000, 10000000, true, nameof(y));
        CheckRange(z, -10000000, 10000000, true, nameof(z));
        return JavadValue.List(
            Datum(datum),
            JavadValue.Float(x),
            JavadValue.Float(y),
            JavadValue.Float(z)
        );
    }

    /// <summary>Creates pos_geo with decimal-degree angles and altitude in meters.</summary>
    public static JavadValue Geodetic(
        string datum,
        double latitude,
        double longitude,
        double altitude
    )
    {
        CheckRange(latitude, -90, 90, true, nameof(latitude));
        CheckRange(longitude, -180, 180, true, nameof(longitude));
        CheckRange(altitude, -20000, 20000, true, nameof(altitude));
        return JavadValue.List(
            Datum(datum),
            JavadValue.Float(latitude),
            JavadValue.Float(longitude),
            JavadValue.Float(altitude)
        );
    }

    private static void CheckRange(
        double? value,
        double minimum,
        double maximum,
        bool inclusive,
        string name
    )
    {
        if (
            value.HasValue
            && (
                !double.IsFinite(value.Value)
                || value < minimum
                || (inclusive ? value > maximum : value >= maximum)
            )
        )
            throw new ArgumentOutOfRangeException(name);
    }
}

/// <summary>A recurring time specification. A null field denotes the GREIS wildcard.</summary>
public readonly record struct JavadTimeSpecification
{
    public JavadTimeSpecification(
        int? day = null,
        int? hour = null,
        int? minute = null,
        int? second = null
    )
    {
        if (day is < 1 or > 7)
            throw new ArgumentOutOfRangeException(nameof(day));
        if (hour is < 0 or > 23)
            throw new ArgumentOutOfRangeException(nameof(hour));
        if (minute is < 0 or > 59)
            throw new ArgumentOutOfRangeException(nameof(minute));
        if (second is < 0 or > 59)
            throw new ArgumentOutOfRangeException(nameof(second));
        Day = day;
        Hour = hour;
        Minute = minute;
        Second = second;
    }

    public int? Day { get; }
    public int? Hour { get; }
    public int? Minute { get; }
    public int? Second { get; }

    public JavadValue ToValue() => JavadValue.Atom(ToString());

    public override string ToString() =>
        $"{Format(Day, "0")}d{Format(Hour, "00")}h{Format(Minute, "00")}m{Format(Second, "00")}s";

    private static string Format(int? value, string format) =>
        value?.ToString(format, CultureInfo.InvariantCulture) ?? "__";

    public static JavadTimeSpecification Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var match = Regex.Match(
            text,
            "^(?:(?<d>[0-9]+|_{1,2})d)?(?:(?<h>[0-9]+|_{1,2})h)?(?:(?<m>[0-9]+|_{1,2})m)?(?:(?<s>[0-9]+|_{1,2})s)?$",
            RegexOptions.CultureInvariant
        );
        if (!match.Success)
            throw new FormatException("Invalid GREIS time specification.");
        return new(Read("d"), Read("h"), Read("m"), Read("s"));
        int? Read(string group) =>
            !match.Groups[group].Success || match.Groups[group].Value[0] == '_'
                ? null
                : int.Parse(match.Groups[group].Value, CultureInfo.InvariantCulture);
    }
}
