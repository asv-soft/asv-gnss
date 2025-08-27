namespace Asv.Gnss;

public class RtcmV3Msm4Msg1104 : RtcmV3Msm4Base
{
    public static readonly ushort MessageId = 1104;
    
    public override string Name => "SBAS MSM4";
    public override ushort Id => MessageId;
}