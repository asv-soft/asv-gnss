namespace Asv.Gnss;

/// <summary>
/// Mode S MB data block used by I010/250.
/// </summary>
public readonly partial record struct AsterixI010ModeSMbData(byte[] Data);