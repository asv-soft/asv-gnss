namespace Asv.Gnss;

public class RtcmV3Msm4Msg1094 : RtcmV3Msm4Base
{
    public static readonly ushort MessageId = 1094;
    
    public override string Name => "Galileo MSM4";
    public override ushort Id => MessageId;
}