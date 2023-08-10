namespace Asv.Gnss
{
    public class RtcmV3Message1031 : RtcmV3Message1030and1031
    {
        public const int RtcmMessageRecAntId = 1031;
        public override ushort MessageId => RtcmMessageRecAntId;
        public override string Name => "GLONASS Network RTK Residual";

        protected override int ResidualEpochBitLen => 17;
    }
}
