namespace Asv.Gnss;

public class RtcmV2Message9 : RtcmV2Message1
{
    public new const int RtcmMessageId = 9;

    public override ushort MessageId => RtcmMessageId;
    public override string Name => "GPS Partial Correction Set";
}
