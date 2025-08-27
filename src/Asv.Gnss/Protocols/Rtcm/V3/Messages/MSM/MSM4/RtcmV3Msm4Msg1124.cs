namespace Asv.Gnss;

public class RtcmV3Msm4Msg1124 : RtcmV3Msm4Base
{
    public static readonly ushort MessageId = 1124;
    
    public override string Name => "BeiDou MSM4";
    public override ushort Id => MessageId;
}