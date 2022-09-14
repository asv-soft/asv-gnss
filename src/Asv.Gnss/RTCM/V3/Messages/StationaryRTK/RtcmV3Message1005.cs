namespace Asv.Gnss
{
    public class RtcmV3Message1005 : RtcmV3Message1005and1006
    {
        public const int RtcmMessageId = 1005;

        public override ushort MessageId => RtcmMessageId;
        public override string Name => "Stationary RTK Reference Station ARP";
    }
}