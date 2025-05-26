namespace Asv.Gnss;

public enum NmeaDopMode
{
    /// <summary>
    /// A = Automatic 2D/3D
    /// </summary>
    Automatic2D3D = 'A',
    /// <summary>
    /// M = Manual, forced to operate in 2D or 3D
    /// </summary>
    Manual2D = 'M',
}