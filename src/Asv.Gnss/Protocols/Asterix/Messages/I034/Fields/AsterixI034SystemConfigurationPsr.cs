namespace Asv.Gnss;

/// <summary>
/// PSR system configuration and status subfield.
/// </summary>
public sealed class AsterixI034SystemConfigurationPsr
{
    /// <summary>
    /// Antenna selection.
    /// </summary>
    public bool Antenna { get; set; }

    /// <summary>
    /// Channel A/B status.
    /// </summary>
    public byte ChannelAB { get; set; }

    /// <summary>
    /// Overload status.
    /// </summary>
    public bool Overload { get; set; }

    /// <summary>
    /// Monitoring system connected bit.
    /// </summary>
    public bool MonitoringSystemConnected { get; set; }

    internal void Deserialize(byte raw)
    {
        Antenna = (raw & 0x80) != 0;
        ChannelAB = (byte)((raw >> 5) & 0x03);
        Overload = (raw & 0x10) != 0;
        MonitoringSystemConnected = (raw & 0x08) != 0;
    }

    internal byte Serialize()
    {
        return (byte)((Antenna ? 0x80 : 0) |
                      ((ChannelAB & 0x03) << 5) |
                      (Overload ? 0x10 : 0) |
                      (MonitoringSystemConnected ? 0x08 : 0));
    }
}