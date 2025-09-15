namespace Asv.Gnss;

public class RtcmV3Msm4Msg1084 : RtcmV3Msm4Base
{
    public static readonly ushort MessageId = 1084;
    
    public override string Name => "GLONASS MSM4";
    public override ushort Id => MessageId;
}