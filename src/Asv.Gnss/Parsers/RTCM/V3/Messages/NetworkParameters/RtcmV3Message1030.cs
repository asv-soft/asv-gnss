﻿namespace Asv.Gnss
{
    public class RtcmV3Message1030 : RtcmV3Message1030and1031
    {
        public const int RtcmMessageRecAntId = 1030;
        public override ushort MessageId => RtcmMessageRecAntId;
        public override string Name => "GPS Network RTK Residual";

        protected override int ResidualEpochBitLen => 20;
    }
}
