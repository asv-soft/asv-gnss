namespace Asv.Gnss;

/// <summary>
/// Version information for a single ASTERIX category.
/// </summary>
public class AsterixCategoryVersion
{
    /// <summary>
    /// ASTERIX category number.
    /// </summary>
    public byte Category { get; set; }

    /// <summary>
    /// Main version number.
    /// </summary>
    public byte Major { get; set; }

    /// <summary>
    /// Sub-version number.
    /// </summary>
    public byte Minor { get; set; }
}