namespace Asv.Gnss
{
    internal class RtcmV3Message1031 : RtcmV3Message1030and1031Base
    {
        public const int RtcmMessageRecAntId = 1031;
        public override ushort MessageId => RtcmMessageRecAntId;
        public override string Name => "GLONASS Network RTK Residual";
    }
}
