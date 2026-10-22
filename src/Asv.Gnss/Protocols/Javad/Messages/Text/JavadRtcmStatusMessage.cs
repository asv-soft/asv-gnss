namespace Asv.Gnss;

/// <summary>[MS] Legacy RTCM 2.x status, GREIS appendix A.4.4. Prefer DL for new receiver configurations.</summary>
public sealed partial class JavadRtcmStatusMessage() : JavadTextMessage("RTCMST", "0,0,0,0")
{
    public override string Id => "MS";
    public override string Name => "RTCM 2.x Status";
    public int? AgeSeconds
    {
        get => Integer(0);
        set => Set(0, value);
    }
    public int? ReceivedMessages
    {
        get => Integer(1);
        set => Set(1, value);
    }
    public int? CorruptMessages
    {
        get => Integer(2);
        set => Set(2, value);
    }
    public double? QualityPercent
    {
        get => Number(3);
        set => Set(3, value);
    }

    protected override void ValidateFields() => Validate("iiif");
}
