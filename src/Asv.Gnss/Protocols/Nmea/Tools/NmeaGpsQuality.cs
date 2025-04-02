namespace Asv.Gnss;

public enum NmeaGpsQuality
{
    Unknown = -1,
    FixNotAvailable = 0,
    GpsFix = 1,
    DifferentialGpsFix = 2,
    /// <summary>
    /// Real-Time Kinematic, fixed integers
    /// </summary>
    RtkFixed = 4,
    /// <summary>
    /// Real-Time Kinematic, float integers, OmniSTAR XP/HP or Location RTK
    /// </summary>
    RtkFloat = 5,

}