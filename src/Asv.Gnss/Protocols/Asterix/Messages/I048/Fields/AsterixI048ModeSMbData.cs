namespace Asv.Gnss;

/// <summary>
/// Mode S MB data block used by CAT048.
/// </summary>
public sealed class AsterixI048ModeSMbData
{
    /// <summary>
    /// Raw seven-octet MB data.
    /// </summary>
    public byte[] MbData { get; } = new byte[7];

    /// <summary>
    /// BDS register first digit.
    /// </summary>
    public byte Bds1 { get; set; }

    /// <summary>
    /// BDS register second digit.
    /// </summary>
    public byte Bds2 { get; set; }
}