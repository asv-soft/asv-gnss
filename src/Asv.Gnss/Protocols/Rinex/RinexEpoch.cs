using System;

namespace Asv.Gnss;

/// <summary>A RINEX calendar epoch in the file's time system, including leap seconds and picoseconds.</summary>
public readonly partial record struct RinexEpoch(int Year, int Month, int Day, int Hour, int Minute, decimal Seconds)
{
    /// <summary>Converts to an unspecified-kind calendar value, rounding down below .NET tick precision.</summary>
    /// <remarks>No time-system or leap-second conversion is performed.</remarks>
    public DateTime ToDateTime()
    {
        Validate();
        if (Seconds >= 60)
            throw new InvalidOperationException("A leap-second epoch cannot be represented by DateTime without conversion.");
        return new DateTime(Year, Month, Day, Hour, Minute, 0, DateTimeKind.Unspecified)
            .AddTicks((long)(Seconds * TimeSpan.TicksPerSecond));
    }

    /// <summary>Validates the calendar fields and permits seconds in the leap-second interval.</summary>
    public void Validate()
    {
        _ = new DateTime(Year, Month, Day, Hour, Minute, 0, DateTimeKind.Unspecified);
        if (Seconds is < 0 or >= 61)
            throw new FormatException("RINEX seconds must be in the interval [0, 61).");
    }
}
