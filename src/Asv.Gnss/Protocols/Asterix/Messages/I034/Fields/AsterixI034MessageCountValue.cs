namespace Asv.Gnss;

/// <summary>
/// Message count value used by I034/070.
/// </summary>
public sealed class AsterixI034MessageCountValue
{
    /// <summary>
    /// Aerial indicator.
    /// </summary>
    public bool Aerial { get; set; }

    /// <summary>
    /// Message count type identifier.
    /// </summary>
    public byte Type { get; set; }

    /// <summary>
    /// Message counter value.
    /// </summary>
    public ushort Counter { get; set; }
}