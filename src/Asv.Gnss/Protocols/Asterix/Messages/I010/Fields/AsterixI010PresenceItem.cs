namespace Asv.Gnss;

/// <summary>
/// Presence item used by I010/280.
/// </summary>
public readonly record struct AsterixI010PresenceItem(sbyte Drho, sbyte Dtheta);