using System;
using Asv.IO;

namespace Asv.Gnss;

public class RtcmV2Message15 : RtcmV2MessageBase
{
    public const int RtcmMessageId = 15;

    public override ushort MessageId => RtcmMessageId;
    public override string Name => "Ionospheric Delay Message (Fixed)";

    public IonosphericDelayItem[] Delays { get; set; } = [];

    protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
    {
        var itemCount = payloadLength * 8 / 36;
        Delays = new IonosphericDelayItem[itemCount];

        for (var i = 0; i < itemCount; i++)
        {
            Delays[i] = new IonosphericDelayItem();
            Delays[i].Deserialize(buffer, ref bitIndex);
        }
    }
}

public class IonosphericDelayItem
{
    public NavigationSystemEnum NavigationSystem { get; set; }
    public byte Prn { get; set; }
    public double IonosphericDelay { get; set; }
    public double IonoRateOfChange { get; set; }

    public void Deserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
    {
        var sys = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
        NavigationSystem = sys == 0 ? NavigationSystemEnum.SYS_GPS : NavigationSystemEnum.SYS_GLO;
        Prn = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
        if (Prn == 0)
        {
            Prn = 32;
        }

        IonosphericDelay = SpanBitHelper.GetBitU(buffer, ref bitIndex, 14) * 0.001;
        var rateOfChange = SpanBitHelper.GetBitS(buffer, ref bitIndex, 14);
        if (rateOfChange == -8192)
        {
            IonosphericDelay = double.NaN;
            IonoRateOfChange = double.NaN;
        }
        else
        {
            IonoRateOfChange = rateOfChange * 0.05;
        }
    }
}
