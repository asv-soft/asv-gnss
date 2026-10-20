using Xunit;

namespace Asv.Gnss;

public class UbxCfgRstTest
{
    [Theory]
    [InlineData(BbrMask.HotStart, 0x00, 0x00)]
    [InlineData(BbrMask.WarmStart, 0x01, 0x00)]
    [InlineData(BbrMask.ColdStart, 0xFF, 0xFF)]
    public void Serialize_StartMode_ShouldUseDocumentedBackupMemoryMask(BbrMask start, byte low, byte high)
    {
        var message = new UbxCfgRst { Bbr = start, Mode = ResetMode.ControlledSoftwareResetGnssOnly };

        var frame = UbxTestHelper.Serialize(message);

        Assert.Equal(UbxTestHelper.CreateFrame(0x06, 0x04, [low, high, 0x02, 0x00]), frame);
        var decoded = UbxTestHelper.Deserialize<UbxCfgRst>(frame);
        Assert.Equal(start, decoded.Bbr);
        Assert.Equal(ResetMode.ControlledSoftwareResetGnssOnly, decoded.Mode);
    }
}
