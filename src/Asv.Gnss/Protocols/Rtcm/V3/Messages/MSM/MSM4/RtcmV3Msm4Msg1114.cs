namespace Asv.Gnss;

public class RtcmV3Msm4Msg1114 : RtcmV3Msm4Base
{
    public static readonly ushort MessageId = 1114;
    
    public override string Name => "QZSS MSM4";
    public override ushort Id => MessageId;
}