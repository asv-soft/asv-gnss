using System;
using Asv.IO;

namespace Asv.Gnss;

public class RtcmV2Message14 : RtcmV2MessageBase
{
    public const int RtcmMessageId = 14;

    public override ushort MessageId => RtcmMessageId;
    public override string Name => "GPS Time of Week (Fixed)";

    /// <summary>GPS-minus-UTC leap-second offset, encoded as an unsigned six-bit value.</summary>
    public byte LeapSeconds { get; set; }

    /// <inheritdoc />
    protected override int GetContentBitSize() => 24;

    /// <inheritdoc />
    protected override void SerializeContent(Span<byte> buffer, ref int bitIndex)
    {
        var epoch = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
        var elapsed = GpsTime - epoch;
        if (elapsed.Ticks < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(GpsTime), "GPS time must not precede the GPS epoch.");
        }
        var week = elapsed.Ticks / (TimeSpan.TicksPerDay * 7);
        var hour = elapsed.Ticks % (TimeSpan.TicksPerDay * 7) / TimeSpan.TicksPerHour;
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 10, week % 1024);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 8, hour);
        RtcmV2Serialization.WriteUnsigned(buffer, ref bitIndex, 6, LeapSeconds);
    }

    protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
    {
        var week = SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
        var hour = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
        var leap = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
        LeapSeconds = (byte)leap;

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
