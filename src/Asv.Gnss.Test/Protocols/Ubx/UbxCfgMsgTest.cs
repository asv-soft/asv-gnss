using System;
using Xunit;

namespace Asv.Gnss;

public class UbxCfgMsgTest
{
    [Fact]
    public void Deserialize_PerPortRates_ShouldReadSixRatesAndPreserveWireFrame()
    {
        byte[] payload = [0x01, 0x07, 1, 2, 0, 4, 5, 0];
        var frame = UbxTestHelper.CreateFrame(0x06, 0x01, payload);

        var message = UbxTestHelper.Deserialize<UbxCfgMsg>(frame);

        Assert.Equal(0x01, message.MsgClass);
        Assert.Equal(0x07, message.MsgId);
        Assert.Null(message.CurrentPortRate);
        Assert.Equal(new byte[] { 1, 2, 0, 4, 5, 0 }, message.Ports);
        Assert.Equal(frame, UbxTestHelper.Serialize(message));
    }

    [Fact]
    public void Deserialize_CurrentPortRate_ShouldReadThreeBytePayload()
    {
        var frame = UbxTestHelper.CreateFrame(0x06, 0x01, [0x01, 0x35, 2]);

        var message = UbxTestHelper.Deserialize<UbxCfgMsg>(frame);

        Assert.Equal(0x01, message.MsgClass);
        Assert.Equal(0x35, message.MsgId);
        Assert.Equal((byte)2, message.CurrentPortRate);
        Assert.Null(message.Ports);
        Assert.Equal(frame, UbxTestHelper.Serialize(message));
    }

    [Fact]
    public void Deserialize_ReusedMessage_ShouldClearPreviousRateForm()
    {
        var message = new UbxCfgMsg();
        var allPorts = UbxTestHelper.CreateFrame(0x06, 0x01, [0x01, 0x07, 1, 2, 3, 4, 5, 0]);
        var currentPort = UbxTestHelper.CreateFrame(0x06, 0x01, [0x01, 0x35, 0]);

        UbxTestHelper.Deserialize(message, allPorts);
        UbxTestHelper.Deserialize(message, currentPort);
        Assert.Null(message.Ports);
        Assert.Equal((byte)0, message.CurrentPortRate);
        Assert.Equal(currentPort, UbxTestHelper.Serialize(message));

        UbxTestHelper.Deserialize(message, allPorts);
        Assert.Null(message.CurrentPortRate);
        Assert.NotNull(message.Ports);
        Assert.Equal(6, message.Ports.Length);
        Assert.Equal(allPorts, UbxTestHelper.Serialize(message));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(7)]
    [InlineData(10)]
    public void Deserialize_InvalidPayloadLength_ShouldRejectInsteadOfReadingAnotherForm(int length)
    {
        var frame = UbxTestHelper.CreateFrame(0x06, 0x01, new byte[length]);

        Assert.Throws<ArgumentException>(() => UbxTestHelper.Deserialize<UbxCfgMsg>(frame));
    }
}
