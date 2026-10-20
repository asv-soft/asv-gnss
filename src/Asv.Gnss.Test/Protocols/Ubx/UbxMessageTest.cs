using System;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class UbxMessageTest
{
    [Theory]
    [InlineData(true, "B5620501020006010F38")]
    [InlineData(false, "B5620500020006010E33")]
    public void Serialize_Acknowledgement_WritesReferenceFrame(bool accepted, string expectedHex)
    {
        UbxAckBase message = accepted ? new UbxAckAck() : new UbxAckNak();
        message.AckClassId = 0x06;
        message.AckSubclassId = 0x01;
        var bytes = new byte[message.GetByteSize() + 1];
        bytes[^1] = 0xCC;
        var buffer = bytes.AsSpan();

        message.Serialize(ref buffer);

        Assert.Equal(Convert.FromHexString(expectedHex), bytes[..^1]);
        Assert.Equal(10, message.GetByteSize());
        Assert.Equal(1, buffer.Length);
        Assert.Equal(0xCC, buffer[0]);
    }

    [Theory]
    [InlineData(true, "B5620501020006010F38")]
    [InlineData(false, "B5620500020006010E33")]
    public void Deserialize_Acknowledgement_ConsumesOnlyItsFrame(bool accepted, string frameHex)
    {
        UbxAckBase message = accepted ? new UbxAckAck() : new UbxAckNak();
        ReadOnlySpan<byte> buffer = Convert.FromHexString(frameHex + "CC");

        message.Deserialize(ref buffer);

        Assert.Equal(0x06, message.AckClassId);
        Assert.Equal(0x01, message.AckSubclassId);
        Assert.Equal(1, buffer.Length);
        Assert.Equal(0xCC, buffer[0]);
    }

    [Theory]
    [InlineData("B5620501020006010038")]
    [InlineData("B5620501020006010F00")]
    [InlineData("B5620501020006020F38")]
    public void Deserialize_CorruptedAcknowledgement_RejectsChecksum(string frameHex)
    {
        var bytes = Convert.FromHexString(frameHex);

        Assert.Throws<ProtocolDeserializeMessageException>(() =>
        {
            ReadOnlySpan<byte> buffer = bytes;
            new UbxAckAck().Deserialize(ref buffer);
        });
    }
}
