using System;

namespace Asv.Gnss;

public partial class RtcmV2Message31 : RtcmV2MessageBase
{
    public const int RtcmMessageId = 31;

    public override ushort MessageId => RtcmMessageId;
    public override string Name => "Differential GLONASS Corrections (Tentative)";

    public DObservationItem[] ObservationItems { get; set; } = [];

    /// <inheritdoc />
    protected override int GetContentBitSize() => checked(ObservationItems.Length * 40);

    /// <inheritdoc />
    protected override void SerializeContent(Span<byte> buffer, ref int bitIndex)
    {
        foreach (var item in ObservationItems)
        {
            item.Serialize(buffer, ref bitIndex);
        }
    }

    protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
    {
        var itemCount = payloadLength / 5;
        ObservationItems = new DObservationItem[itemCount];

        for (var i = 0; i < itemCount; i++)
        {
            var item = new DObservationItem(NavigationSystemEnum.SYS_GLO);
            item.Deserialize(buffer, ref bitIndex);
            ObservationItems[i] = item;
        }
    }
}
