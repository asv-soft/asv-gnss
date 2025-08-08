namespace Asv.Gnss;

public enum NmeaNavigationStatus : byte
{
    Safe = (byte)'S',
    Caution = (byte)'C',
    Unsafe = (byte)'U',
    NotValid = (byte)'V',
}