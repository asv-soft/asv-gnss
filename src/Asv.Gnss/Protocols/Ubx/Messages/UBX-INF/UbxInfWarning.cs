namespace Asv.Gnss;

/// <summary>Receiver warning text from the UBX-INF-WARNING message.</summary>
/// <remarks>Defined by the u-blox X20 HPG 2.11 interface description, UBXDOC-304424225-21617 R01.</remarks>
public class UbxInfWarning : UbxInfBase
{
    public override string Name => "UBX-INF-WARNING";
    public override byte Class => 0x04;
    public override byte SubClass => 0x01;
}
