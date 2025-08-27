namespace Asv.Gnss;

public class RtcmV3Msm4Msg1074 : RtcmV3Msm4Base
{
    public static readonly ushort MessageId = 1074;
    
    public override string Name => "GPS MSM4";
    public override ushort Id => MessageId;
}