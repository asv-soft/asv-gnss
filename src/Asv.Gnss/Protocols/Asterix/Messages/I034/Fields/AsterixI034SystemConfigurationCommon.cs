namespace Asv.Gnss;

/// <summary>
/// Common system configuration and status subfield.
/// </summary>
public sealed class AsterixI034SystemConfigurationCommon
{
    /// <summary>
    /// No-go bit.
    /// </summary>
    public bool Nogo { get; set; }

    /// <summary>
    /// Radar Data Processor Chain status.
    /// </summary>
    public bool Rdpc { get; set; }

    /// <summary>
    /// Radar Data Processor Reset status.
    /// </summary>
    public bool Rdpr { get; set; }

    /// <summary>
    /// Overload of the RDP chain.
    /// </summary>
    public bool OverloadRdp { get; set; }

    /// <summary>
    /// Overload of the transmission subsystem.
    /// </summary>
    public bool OverloadTransmission { get; set; }

    /// <summary>
    /// Monitoring system connected bit.
    /// </summary>
    public bool MonitoringSystemConnected { get; set; }

    /// <summary>
    /// Time source validity bit.
    /// </summary>
    public bool TimeSourceValid { get; set; }

    internal void Deserialize(byte raw)
    {
        Nogo = (raw & 0x80) != 0;
        Rdpc = (raw & 0x40) != 0;
        Rdpr = (raw & 0x20) != 0;
        OverloadRdp = (raw & 0x10) != 0;
        OverloadTransmission = (raw & 0x08) != 0;
        MonitoringSystemConnected = (raw & 0x04) != 0;
        TimeSourceValid = (raw & 0x02) != 0;
    }

    internal byte Serialize()
    {
        return (byte)((Nogo ? 0x80 : 0) |
                      (Rdpc ? 0x40 : 0) |
                      (Rdpr ? 0x20 : 0) |
                      (OverloadRdp ? 0x10 : 0) |
                      (OverloadTransmission ? 0x08 : 0) |
                      (MonitoringSystemConnected ? 0x04 : 0) |
                      (TimeSourceValid ? 0x02 : 0));
    }
}