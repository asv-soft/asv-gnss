using System;
using Asv.Gnss;
using Xunit;

namespace Asv.Gnss.Test;

public class SbfProtocolTest
{
    private static readonly byte[] QualityIndPacket =
    [
        0x24, 0x40, 0x01, 0x51, 0xF2, 0x0F, 0x1C, 0x00,
        0xB8, 0xDD, 0x53, 0x16, 0xB3, 0x08, 0x05, 0x00,
        0x0B, 0x0A, 0x01, 0x02, 0x15, 0x0A, 0x1F, 0x07,
        0x00, 0x02, 0x00, 0x00
    ];

    [Fact]
    public void Deserialize_QualityIndPacketFromV163_ShouldReadIndicators()
    {
        Assert.Equal(0x5101, SbfCrc16.checksum(QualityIndPacket, 4, QualityIndPacket.Length - 4));

        var buffer = new ReadOnlySpan<byte>(QualityIndPacket);
        var message = new SbfPacketQualityInd();

        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);
        Assert.Equal(4082, message.MessageType);
        Assert.Equal(0, message.MessageRevision);
        Assert.Equal(4082, message.MessageId);
        Assert.Equal(28, QualityIndPacket.Length);
        Assert.Equal(374595000u, message.TOW);
        Assert.Equal(2227, message.WNc);
        Assert.Equal(5, message.Indicators.Length);
        Assert.Equal(SbfQualityIndicatorTypeEnum.RfPowerLevelMainAntenna, message.Indicators[0].IndicatorType);
        Assert.Equal(10, message.Indicators[0].Value);
        Assert.Equal(SbfQualityIndicatorTypeEnum.SignalMainAntenna, message.Indicators[1].IndicatorType);
        Assert.Equal(2, message.Indicators[1].Value);
        Assert.Equal(SbfQualityIndicatorTypeEnum.CpuHeadroom, message.Indicators[2].IndicatorType);
        Assert.Equal(10, message.Indicators[2].Value);
        Assert.Equal(SbfQualityIndicatorTypeEnum.RtkPostProcessing, message.Indicators[3].IndicatorType);
        Assert.Equal(7, message.Indicators[3].Value);
        Assert.Equal(SbfQualityIndicatorTypeEnum.All, message.Indicators[4].IndicatorType);
        Assert.Equal(2, message.Indicators[4].Value);
    }
}
