using System;
using Xunit;

namespace Asv.Gnss;

public class RtcmV2MessageSerializationTest
{
    [Fact]
    public void DecodeOnlySubclass_ShouldKeepLegacyDeserializationWithoutImplementingSerializationHooks()
    {
        byte[] original = [0x66, 0x04, 0, 0, 0, 0x08, 0x12, 0x34, 0x56];
        var message = new DecodeOnlyMessage();
        var buffer = new ReadOnlySpan<byte>(original);

        message.Deserialize(ref buffer);

        Assert.True(buffer.IsEmpty);
        Assert.Equal(new byte[] { 0x12, 0x34, 0x56 }, message.Payload);
        Assert.Equal(original.Length, message.PacketLength);
        Assert.Throws<NotSupportedException>(() => message.GetByteSize());
        Assert.Throws<NotSupportedException>(() =>
        {
            var destination = new byte[99].AsSpan();
            message.Serialize(ref destination);
        });
    }

    [Fact]
    public void Deserialize_PaddedPacketFollowedByAnotherPacket_ShouldConsumeDeclaredWordsAndPreserveFill()
    {
        byte[] first = [0x66, 0x04, 0, 0, 0, 0x10, 1, 0, 1, 2, 3, 0xA5];
        byte[] second = [0x66, 0x04, 0, 0, 0, 7];
        var buffer = new ReadOnlySpan<byte>([.. first, .. second]);
        var message = new RtcmV2Message1();

        message.Deserialize(ref buffer);

        Assert.Equal(second, buffer.ToArray());
        var item = Assert.Single(message.ObservationItems);
        Assert.Equal((byte)1, item.Prn);
        Assert.Equal(0.02, item.Prc);
        Assert.Equal(0.004, item.Rrc);
        Assert.Equal((byte)3, item.Iod);
        Assert.Equal(first, Serialize(message));

        message.Deserialize(ref buffer);

        Assert.True(buffer.IsEmpty);
        Assert.Empty(message.ObservationItems);
        Assert.Equal(0, message.Udre);
        Assert.Equal(second, Serialize(message));
    }

    [Theory]
    [InlineData(0, 1.0)]
    [InlineData(1, 0.75)]
    [InlineData(2, 0.5)]
    [InlineData(3, 0.3)]
    [InlineData(4, 0.2)]
    [InlineData(5, 0.1)]
    [InlineData(6, double.NaN)]
    [InlineData(7, 0.0)]
    public void StationHealth_ShouldDecodeAndEncodeEveryWireCode(byte health, double expectedUdre)
    {
        byte[] original = [0x66, 0x04, 0, 0, 0, health];
        var message = new RtcmV2Message1();
        var buffer = new ReadOnlySpan<byte>(original);

        message.Deserialize(ref buffer);

        Assert.True(buffer.IsEmpty);
        if (double.IsNaN(expectedUdre))
        {
            Assert.True(double.IsNaN(message.Udre));
        }
        else
        {
            Assert.Equal(expectedUdre, message.Udre);
        }
        Assert.Equal(original, Serialize(message));
        Assert.Equal(original, Serialize(new RtcmV2Message1 { Udre = expectedUdre }));
    }

    [Fact]
    public void Serialize_EmptyPayload_ShouldWriteExactMinimumAndMaximumHeaderFields()
    {
        Assert.Equal(new byte[] { 0x66, 0x04, 0, 0, 0, 7 }, Serialize(new RtcmV2Message1()));
        var maximum = new RtcmV2Message1
        {
            ReferenceStationId = 1023,
            SequenceNumber = 7,
            ZCount = 3599.4,
            Udre = double.NaN
        };

        var encoded = Serialize(maximum);

        Assert.Equal(new byte[] { 0x66, 0x07, 0xFF, 0xBB, 0x7F, 6 }, encoded);
        var decoded = new RtcmV2Message1();
        var buffer = new ReadOnlySpan<byte>(encoded);
        decoded.Deserialize(ref buffer);
        Assert.True(buffer.IsEmpty);
        Assert.Equal((ushort)1023, decoded.ReferenceStationId);
        Assert.Equal((byte)7, decoded.SequenceNumber);
        Assert.Equal(3599.4, decoded.ZCount, 9);
        Assert.True(double.IsNaN(decoded.Udre));
    }

    [Fact]
    public void Serialize_NewPartialWord_ShouldFillThreeByteBoundaryAndPreserveFollowingStorage()
    {
        var message = new RtcmV2Message1
        {
            ObservationItems =
            [
                new DObservationItem(NavigationSystemEnum.SYS_GPS) { Prn = 1, Prc = 0.02, Rrc = 0.004, Iod = 3 }
            ]
        };
        byte[] destination = new byte[15];
        destination.AsSpan().Fill(0xCC);
        var buffer = destination.AsSpan();

        Assert.Equal(12, message.GetByteSize());
        message.Serialize(ref buffer);

        Assert.Equal(new byte[] { 0x66, 0x04, 0, 0, 0, 0x17, 1, 0, 1, 2, 3, 0xAA }, destination[..12]);
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0xCC }, buffer.ToArray());
        Assert.Equal(new byte[] { 0xCC, 0xCC, 0xCC }, destination[12..]);
    }

    [Fact]
    public void Serialize_HeaderValuesExceedingWireWidth_ShouldRejectInsteadOfWrapping()
    {
        var station = new RtcmV2Message1 { ReferenceStationId = 1024 };
        var sequence = new RtcmV2Message1 { SequenceNumber = 8 };

        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(station));
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(sequence));
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(new InvalidMessageType()));
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(3599.8)]
    [InlineData(3600)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Serialize_InvalidModifiedZCount_ShouldReject(double zCount)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(new RtcmV2Message1 { ZCount = zCount }));
    }

    [Theory]
    [InlineData(0.4)]
    [InlineData(-1)]
    [InlineData(double.PositiveInfinity)]
    public void Serialize_UnsupportedStationHealth_ShouldReject(double udre)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(new RtcmV2Message1 { Udre = udre }));
    }

    [Fact]
    public void GetByteSize_NineteenCorrectionsExceedThirtyOneWords_ShouldReject()
    {
        var message = new RtcmV2Message1 { ObservationItems = new DObservationItem[19] };

        Assert.Throws<InvalidOperationException>(() => message.GetByteSize());
    }

    private static byte[] Serialize(RtcmV2MessageBase message)
    {
        var result = new byte[message.GetByteSize()];
        var buffer = result.AsSpan();
        message.Serialize(ref buffer);
        Assert.True(buffer.IsEmpty);
        return result;
    }

    private sealed class DecodeOnlyMessage : RtcmV2MessageBase
    {
        public override string Name => "Legacy decode-only test message";
        public override ushort MessageId => 1;
        public byte[] Payload { get; private set; } = [];
        public int PacketLength { get; private set; }

        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
        {
            PacketLength = buffer.Length;
            Payload = buffer.Slice(bitIndex / 8, payloadLength).ToArray();
            bitIndex += payloadLength * 8;
        }
    }

    private sealed class InvalidMessageType : RtcmV2Message1
    {
        public override ushort MessageId => 64;
    }
}
