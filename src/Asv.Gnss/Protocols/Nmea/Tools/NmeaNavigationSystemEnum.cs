namespace Asv.Gnss;

public enum NmeaNavigationSystemEnum
{
    /// <summary>
    /// None
    /// </summary>
    SysNone = 0x00,
    /// <summary>
    /// GPS
    /// </summary>
    SysGps = 0x01,
    /// <summary>
    /// SBAS
    /// </summary>
    SysSbs = 0x02,
    /// <summary>
    /// GLONASS
    /// </summary>
    SysGlo = 0x04,
    /// <summary>
    /// Galileo
    /// </summary>
    SysGal = 0x08,
    /// <summary>
    /// QZSS
    /// </summary>
    SysQzs = 0x10,
    /// <summary>
    /// BeiDou
    /// </summary>
    SysCmp = 0x20,
    /// <summary>
    /// IRNS
    /// </summary>
    SysIrn = 0x40,
    /// <summary>
    /// LEO
    /// </summary>
    SysLeo = 0x80,
    /// <summary>
    /// ALL
    /// </summary>
    SysAll = 0xFF
}