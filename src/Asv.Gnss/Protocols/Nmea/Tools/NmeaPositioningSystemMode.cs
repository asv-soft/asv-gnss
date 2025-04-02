namespace Asv.Gnss;

/// <summary>
/// NMEA Positioning System Mode Indicator
/// </summary>
public enum NmeaPositioningSystemMode
{
    Autonomous = 'A',
    Differential = 'D',
    Estimated = 'E',
    ManualInput = 'M',
    Simulated = 'S',
    NotValid = 'N',
}