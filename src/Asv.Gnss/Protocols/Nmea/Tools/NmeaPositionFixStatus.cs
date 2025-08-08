namespace Asv.Gnss;

public enum NmeaPositionFixStatus : byte
{
    Valid = (byte)'A', 
    Warning = (byte)'V',
}