using System;
using Xunit;

namespace Asv.Gnss.Test;

public class AsvMessageTest
{
    [Fact]
    public void AllLegacyV163AsvMessages_ShouldRoundTrip()
    {
        var random = new Random(163);
        AsvMessageBase[] messages =
        [
            new AsvMessageHeartBeat(),
            new AsvMessageGbasVdbSend(),
            new AsvMessageGbasVdbSendV2(),
            new AsvMessageGbasMonDevSendV2(),
            new AsvMessageGpsObservations(),
            new AsvMessageGloObservations(),
            new AsvMessageGpsRawCa(),
            new AsvMessageGloRawCa(),
            new AsvMessagePvtGeo(),
        ];

        foreach (var message in messages)
        {
            message.Randomize(random);
            var buffer = new byte[message.GetByteSize()];
            var write = buffer.AsSpan();
            message.Serialize(ref write);

            ReadOnlySpan<byte> read = buffer;
            var copy = (AsvMessageBase)Activator.CreateInstance(message.GetType())!;
            copy.Deserialize(ref read);

            Assert.Equal(0, read.Length);
            Assert.Equal(message.GetByteSize(), copy.GetByteSize());
            Assert.Equal(message.MessageId, copy.MessageId);
        }
    }

    [Fact]
    public void HeartBeat_SerializeAndDeserialize_ShouldRoundTrip()
    {
        var origin = new AsvMessageHeartBeat
        {
            Sequence = 42,
            SenderId = 2,
            TargetId = 7,
            DeviceType = AsvDeviceType.GbasServer,
            DeviceState = AsvDeviceState.Active,
            Reserved1 = 1,
            Reserved2 = 2,
            Reserved3 = 3,
            Reserved4 = 4,
        };

        var copy = RoundTrip<AsvMessageHeartBeat>(origin);

        Assert.Equal(origin.Sequence, copy.Sequence);
        Assert.Equal(origin.SenderId, copy.SenderId);
        Assert.Equal(origin.TargetId, copy.TargetId);
        Assert.Equal(origin.DeviceType, copy.DeviceType);
        Assert.Equal(origin.DeviceState, copy.DeviceState);
        Assert.Equal(origin.Reserved1, copy.Reserved1);
        Assert.Equal(origin.Reserved2, copy.Reserved2);
        Assert.Equal(origin.Reserved3, copy.Reserved3);
        Assert.Equal(origin.Reserved4, copy.Reserved4);
    }

    [Fact]
    public void HeartBeat_WithLegacyV1Buffer_ShouldParseAndSerializeByteForByte()
    {
        byte[] legacyBuffer =
        [
            0xAA, 0x44, 0x07, 0x00, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00,
            0x02, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0xF7, 0x1B
        ];

        ReadOnlySpan<byte> read = legacyBuffer;
        var message = new AsvMessageHeartBeat();
        message.Deserialize(ref read);

        Assert.Equal(0, read.Length);
        Assert.Equal(0, message.Sequence);
        Assert.Equal(2, message.SenderId);
        Assert.Equal(1, message.TargetId);
        Assert.Equal(AsvDeviceType.GbasModulator, message.DeviceType);
        Assert.Equal(AsvDeviceState.Active, message.DeviceState);

        var serialized = new byte[message.GetByteSize()];
        var write = serialized.AsSpan();
        message.Serialize(ref write);

        Assert.Equal(legacyBuffer, serialized);
    }

    [Fact]
    public void GbasVdbSend_SerializeAndDeserialize_ShouldRoundTrip()
    {
        var origin = new AsvMessageGbasVdbSend
        {
            Sequence = 11,
            SenderId = 1,
            TargetId = 9,
            Slot = AsvGbasSlot.SlotA | AsvGbasSlot.SlotC,
            Msgs = AsvGbasMessage.Msg1 | AsvGbasMessage.Msg4,
            LastByteLength = 6,
            Data = [1, 2, 3, 4, 5],
        };

        var copy = RoundTrip<AsvMessageGbasVdbSend>(origin);

        Assert.Equal(origin.Sequence, copy.Sequence);
        Assert.Equal(origin.Slot, copy.Slot);
        Assert.Equal(origin.Msgs, copy.Msgs);
        Assert.Equal(origin.LastByteLength, copy.LastByteLength);
        Assert.Equal(origin.Data, copy.Data);
    }

    [Fact]
    public void GbasVdbSendV2_SerializeAndDeserialize_ShouldRoundTrip()
    {
        var origin = new AsvMessageGbasVdbSendV2
        {
            Sequence = 12,
            SenderId = 3,
            TargetId = 10,
            Slot = AsvGbasSlotMsg.SlotD,
            GbasMessageId = 17,
            ActiveSlots = AsvGbasSlot.SlotB | AsvGbasSlot.SlotD,
            LifeTime = 5,
            LastByteOffset = 3,
            ReservedFlags = 4,
            IsLastSlotInFrame = true,
            Data = [9, 8, 7],
        };

        var copy = RoundTrip<AsvMessageGbasVdbSendV2>(origin);

        Assert.Equal(origin.Sequence, copy.Sequence);
        Assert.Equal(origin.Slot, copy.Slot);
        Assert.Equal(origin.GbasMessageId, copy.GbasMessageId);
        Assert.Equal(origin.ActiveSlots, copy.ActiveSlots);
        Assert.Equal(origin.LifeTime, copy.LifeTime);
        Assert.Equal(origin.LastByteOffset, copy.LastByteOffset);
        Assert.Equal(origin.ReservedFlags, copy.ReservedFlags);
        Assert.Equal(origin.IsLastSlotInFrame, copy.IsLastSlotInFrame);
        Assert.Equal(origin.Data, copy.Data);
    }

    private static T RoundTrip<T>(T origin)
        where T : AsvMessageBase, new()
    {
        var buffer = new byte[origin.GetByteSize()];
        var write = buffer.AsSpan();
        origin.Serialize(ref write);

        ReadOnlySpan<byte> read = buffer;
        var copy = new T();
        copy.Deserialize(ref read);
        Assert.Equal(0, read.Length);
        return copy;
    }
}
