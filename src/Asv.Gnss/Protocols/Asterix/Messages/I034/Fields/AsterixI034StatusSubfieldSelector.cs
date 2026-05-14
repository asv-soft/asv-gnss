namespace Asv.Gnss;

/// <summary>
/// Subfield selector used by CAT034 compound status items.
/// </summary>
public sealed class AsterixI034StatusSubfieldSelector
{
    /// <summary>
    /// Raw subfield selector octet.
    /// </summary>
    public byte Raw { get; set; }

    /// <summary>
    /// Common part is present.
    /// </summary>
    public bool HasCommonPart
    {
        get => (Raw & 0x80) != 0;
        set => Raw = value ? (byte)(Raw | 0x80) : (byte)(Raw & ~0x80);
    }

    /// <summary>
    /// PSR sensor part is present.
    /// </summary>
    public bool HasPsrPart
    {
        get => (Raw & 0x10) != 0;
        set => Raw = value ? (byte)(Raw | 0x10) : (byte)(Raw & ~0x10);
    }

    /// <summary>
    /// Mode-S sensor part is present.
    /// </summary>
    public bool HasModeSPart
    {
        get => (Raw & 0x04) != 0;
        set => Raw = value ? (byte)(Raw | 0x04) : (byte)(Raw & ~0x04);
    }

    /// <summary>
    /// Extension bit.
    /// </summary>
    public bool HasExtension
    {
        get => (Raw & 0x01) != 0;
        set => Raw = value ? (byte)(Raw | 0x01) : (byte)(Raw & ~0x01);
    }
}