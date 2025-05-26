namespace Asv.Gnss;

public enum NmeaGpsQuality
{
    Unknown = -1,
    FixNotAvailable = 0,
    Fix2D3D = 1,
    Dgnss = 2,
    RtkFixed = 4,
    RtkFloat = 5,
    DeadReckoning = 6,

}