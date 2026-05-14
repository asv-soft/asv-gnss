namespace Asv.Gnss;

/// <summary>
/// Mode-S system processing mode subfield.
/// </summary>
public sealed class AsterixI034SystemProcessingModeS
{
    /// <summary>
    /// Reduced radar mode.
    /// </summary>
    public bool ReducedRadar { get; set; }

    /// <summary>
    /// Cluster state.
    /// </summary>
    public bool ClusterState { get; set; }

    internal void Deserialize(byte raw)
    {
        ReducedRadar = (raw & 0x80) != 0;
        ClusterState = (raw & 0x10) != 0;
    }

    internal byte Serialize()
    {
        return (byte)((ReducedRadar ? 0x80 : 0) | (ClusterState ? 0x10 : 0));
    }
}