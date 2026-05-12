using System;
using Asv.IO;

namespace Asv.Gnss;

public class RtcmV2Message14 : RtcmV2MessageBase
{
    public const int RtcmMessageId = 14;

    public override ushort MessageId => RtcmMessageId;
    public override string Name => "GPS Time of Week (Fixed)";

    protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
    {
        var week = SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
        var hour = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
        var leap = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);

        week = AdjustGpsWeek(week, leap);
        GpsTime = RtcmV3Protocol.GetFromGps((int)week, hour * 3600.0 + ZCount);
    }

    private static uint AdjustGpsWeek(uint week, uint leap)
    {
        var nowGps = DateTime.UtcNow.AddSeconds(leap);
        var currentWeek = 0;
        var seconds = 0.0;

        RtcmV3Protocol.GetFromTime(nowGps, ref currentWeek, ref seconds);
        if (currentWeek < 1560)
        {
            currentWeek = 1560;
        }

        return (uint)(week + (currentWeek - week + 1) / 1024 * 1024);
    }
}
