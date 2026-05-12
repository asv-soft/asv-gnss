using System;

namespace Asv.Gnss;

public class RtcmV2Message1 : RtcmV2MessageBase
{
    public const int RtcmMessageId = 1;

    public override ushort MessageId => RtcmMessageId;
    public override string Name => "Differential GPS Corrections (Fixed)";

    public DObservationItem[] ObservationItems { get; set; } = [];

    protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
    {
        var itemCount = payloadLength / 5;
        ObservationItems = new DObservationItem[itemCount];

        for (var i = 0; i < itemCount; i++)
        {
            var item = new DObservationItem(NavigationSystemEnum.SYS_GPS);
            item.Deserialize(buffer, ref bitIndex);
            ObservationItems[i] = item;
        }
    }
}
