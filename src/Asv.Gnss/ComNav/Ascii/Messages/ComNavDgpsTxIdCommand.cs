using System;

namespace Asv.Gnss
{

    public enum DgpsTxIdEnum
    {
        RTCM = 0,
        RTCA = 1,
        CMR = 2,
        AUTO = 10,
        RTCMV3 = 13,
        NOVATELX = 14
    }
    public class ComNavDgpsTxIdCommand : ComNavAsciiCommandBase
    {
        public DgpsTxIdEnum Type { get; set; }

        protected override string SerializeToAsciiString()
        {
            return Type switch
            {
                DgpsTxIdEnum.RTCM => $"DGPSTXID RTCM {Id:0000}",
                DgpsTxIdEnum.RTCA => $"DGPSTXID RTCA {Id:0000}",
                DgpsTxIdEnum.CMR => $"DGPSTXID CMR {Id:0000}",
                DgpsTxIdEnum.AUTO => $"DGPSTXID AUTO {Id:0000}",
                DgpsTxIdEnum.RTCMV3 => $"DGPSTXID RTCMV3 {Id:0000}",
                DgpsTxIdEnum.NOVATELX => $"DGPSTXID NOVATELX {Id:0000}",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public uint Id { get; set; }

        public override string MessageId => "DGPSTXID";
    }
}