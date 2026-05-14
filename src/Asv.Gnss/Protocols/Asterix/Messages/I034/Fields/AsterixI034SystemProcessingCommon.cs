namespace Asv.Gnss;

/// <summary>
/// Common system processing mode subfield.
/// </summary>
public sealed class AsterixI034SystemProcessingCommon
{
    /// <summary>
    /// Reduced RDP chain mode.
    /// </summary>
    public bool ReducedRdp { get; set; }

    /// <summary>
    /// Reduced transmission mode.
    /// </summary>
    public bool ReducedTransmission { get; set; }

    internal void Deserialize(byte raw)
    {
        ReducedRdp = (raw & 0x80) != 0;
        ReducedTransmission = (raw & 0x40) != 0;
    }

    internal byte Serialize()
    {
        return (byte)((ReducedRdp ? 0x80 : 0) | (ReducedTransmission ? 0x40 : 0));
    }
}