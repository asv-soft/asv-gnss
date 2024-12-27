using System;
using System.Reactive.Linq;
using Xunit;

namespace Asv.Gnss.Test;

public class SBFTest
{
    [Fact]
    public void TestMsg1()
    {
        var array = new byte[]
        {
            0x24,
            0x40,
            0x01,
            0x51,
            0xF2,
            0x0F,
            0x1C,
            0x00,
            0xB8,
            0xDD,
            0x53,
            0x16,
            0xB3,
            0x08,
            0x05,
            0x00,
            0x0B,
            0x0A,
            0x01,
            0x02,
            0x15,
            0x0A,
            0x1F,
            0x07,
            0x00,
            0x02,
            0x00,
            0x00,
        };
        var parser = new SbfBinaryParser().RegisterDefaultMessages();
        SbfPacketQualityInd msg = null;
        parser.OnMessage.Cast<SbfPacketQualityInd>().Subscribe(_ => msg = _);
        for (var index = 0; index < array.Length; index++)
        {
            var p = array[index];
            parser.Read(p);
        }

        Assert.NotNull(msg);
    }
}
