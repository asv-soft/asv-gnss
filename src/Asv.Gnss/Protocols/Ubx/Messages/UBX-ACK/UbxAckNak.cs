namespace Asv.Gnss;

/// <summary>
/// UBX-ACK-NAK (0x05 0x00)
/// Message not acknowledged
/// Output upon processing of an input message. A UBX-ACK-NAK is sent as soon
/// as possible but at least within one second.
/// Supported on:
/// • u-blox 8 / u-blox M8 protocol versions 15, 15.01, 16, 17, 18, 19, 19.1, 19.2, 20, 20.01,
/// 20.1, 20.2, 20.3, 22, 22.01, 23 and 23.01
/// </summary>
public class UbxAckNak : UbxAckBase
{
    public override byte Class => 0x05;
    public override byte SubClass => 0x00;
    public override string Name => "UBX-ACK-NAK";
}