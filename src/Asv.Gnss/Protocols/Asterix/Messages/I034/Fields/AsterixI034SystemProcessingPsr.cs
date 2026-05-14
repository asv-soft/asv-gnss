namespace Asv.Gnss;

/// <summary>
/// PSR system processing mode subfield.
/// </summary>
public sealed class AsterixI034SystemProcessingPsr
{
    /// <summary>
    /// Polarization mode.
    /// </summary>
    public bool Polarization { get; set; }

    /// <summary>
    /// Reduced radar mode.
    /// </summary>
    public bool ReducedRadar { get; set; }

    /// <summary>
    /// Sensitivity time control mode.
    /// </summary>
    public bool SensitivityTimeControl { get; set; }

    internal void Deserialize(byte raw)
    {
        Polarization = (raw & 0x80) != 0;
        ReducedRadar = (raw & 0x40) != 0;
        SensitivityTimeControl = (raw & 0x20) != 0;
    }

    internal byte Serialize()
    {
        return (byte)((Polarization ? 0x80 : 0) |
                      (ReducedRadar ? 0x40 : 0) |
                      (SensitivityTimeControl ? 0x20 : 0));
    }
}