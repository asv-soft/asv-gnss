namespace Asv.Gnss;

public class RtcmV3Message1005 : RtcmV3Message1005and1006
{
    public static readonly ushort MessageId = 1005;

    public override string Name => "Stationary RTK Reference Station ARP";
    public override ushort Id => MessageId;
}