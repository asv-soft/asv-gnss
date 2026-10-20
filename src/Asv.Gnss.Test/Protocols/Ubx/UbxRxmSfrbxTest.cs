using System;
using System.Buffers.Binary;
using Xunit;

namespace Asv.Gnss;

public class UbxRxmSfrbxTest
{
    private static readonly uint[] GpsL1Words =
    [
        0x22C0593C, 0x226AEA68, 0x393F01C1, 0x8CE67A0E, 0x8A40FA3A,
        0xBF22802D, 0xA0EE0F0F, 0x03C82852, 0x83326C66, 0x99ED9F33
    ];

    private static readonly uint[] GpsL2Words =
    [
        0x8B12189A, 0xC283E97B, 0x05D42E71, 0x7FE3D001, 0xFFDBFFE0,
        0x02520008, 0x593C4B89, 0x00000000, 0x00000E89, 0xBAB8B10A
    ];

    private static readonly uint[] GlonassWords = [0x0801C4B5, 0x01103220, 0x35F50000, 0x090C0002];

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    public void Deserialize_GlonassThreeOrFourWords_ShouldDecodeStringAndPreserveAllRawWords(int count)
    {
        var words = GlonassWords.AsSpan(0, count);
        var message = new UbxRxmSfrbx { GpsSubFrame = new GpsSubframe1() };

        UbxTestHelper.Deserialize(message, CreateFrame(UbxGnssTypeEnum.GLONASS, 0, 2, words));

        Assert.Equal(words.ToArray(), message.RawData);
        Assert.Equal(count, message.NumWords);
        Assert.IsType<GlonassWord1>(message.GlonassWord);
        Assert.Equal(1, message.GlonassWord.WordId);
        Assert.Null(message.GpsSubFrame);
        if (count == 4)
        {
            Assert.Equal(0x090C0002u, message.RawData[3]);
        }
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    public void Deserialize_GpsL2Signals_ShouldRetainRawWordsWithoutInvokingLegacyDecoder(byte signalId)
    {
        var message = new UbxRxmSfrbx
        {
            GpsSubFrame = new GpsSubframe1(),
            GlonassWord = new GlonassWord1()
        };

        UbxTestHelper.Deserialize(message, CreateFrame(UbxGnssTypeEnum.GPS, signalId, 2, GpsL2Words));

        Assert.Equal(GpsL2Words, message.RawData);
        Assert.Equal(10, message.NumWords);
        Assert.Equal(signalId, message.Reserved1);
        Assert.Null(message.GpsSubFrame);
        Assert.Null(message.GlonassWord);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void Deserialize_GpsL1_ShouldKeepDecodedLegacySubframes(byte version)
    {
        var message = UbxTestHelper.Deserialize<UbxRxmSfrbx>(CreateFrame(UbxGnssTypeEnum.GPS, 0, version, GpsL1Words));

        Assert.Equal(GpsL1Words, message.RawData);
        Assert.IsType<GpsSubframe2>(message.GpsSubFrame);
        Assert.Equal(2, message.GpsSubFrame.SubframeId);
        Assert.Null(message.GlonassWord);
    }

    [Fact]
    public void Deserialize_ReusedMessage_ShouldClearDecodedViewsWhenConstellationChanges()
    {
        var message = new UbxRxmSfrbx();
        UbxTestHelper.Deserialize(message, CreateFrame(UbxGnssTypeEnum.GPS, 0, 2, GpsL1Words));
        Assert.IsType<GpsSubframe2>(message.GpsSubFrame);

        UbxTestHelper.Deserialize(message, CreateFrame(UbxGnssTypeEnum.GLONASS, 0, 2, GlonassWords));
        Assert.IsType<GlonassWord1>(message.GlonassWord);
        Assert.Null(message.GpsSubFrame);

        UbxTestHelper.Deserialize(message, CreateFrame(UbxGnssTypeEnum.Galileo, 1, 2, [0x955555, 0x55555555]));
        Assert.Equal(UbxGnssTypeEnum.Galileo, message.UbxGnssId);
        Assert.Equal(new uint[] { 0x955555, 0x55555555 }, message.RawData);
        Assert.Null(message.GpsSubFrame);
        Assert.Null(message.GlonassWord);
    }

    private static byte[] CreateFrame(UbxGnssTypeEnum gnss, byte signalId, byte version, ReadOnlySpan<uint> words)
    {
        var payload = new byte[8 + words.Length * 4];
        payload[0] = (byte)gnss;
        payload[1] = 4;
        payload[2] = signalId;
        payload[4] = checked((byte)words.Length);
        payload[5] = 12;
        payload[6] = version;
        for (var index = 0; index < words.Length; index++)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(8 + index * 4), words[index]);
        }
        return UbxTestHelper.CreateFrame(0x02, 0x13, payload);
    }
}
