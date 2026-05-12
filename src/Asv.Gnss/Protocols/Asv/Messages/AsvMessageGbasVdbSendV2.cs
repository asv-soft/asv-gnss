using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsvMessageGbasVdbSendV2 : AsvMessageBase
{
    public override ushort MessageId => 0x0102;
    public override string Name => "GbasVdbSendV2";

    public AsvGbasSlotMsg Slot { get; set; }
    public byte GbasMessageId { get; set; }
    public AsvGbasSlot ActiveSlots { get; set; }
    public byte LifeTime { get; set; }
    public byte LastByteOffset { get; set; }
    public byte ReservedFlags { get; set; }
    public byte ReservedFlgas
    {
        get => ReservedFlags;
        set => ReservedFlags = value;
    }
    public bool IsLastSlotInFrame { get; set; }
    public byte[] Data { get; set; } = [];

    protected override void InternalDeserialize(ref ReadOnlySpan<byte> buffer)
    {
        var slotAndMsg = BinSerialize.ReadByte(ref buffer);
        Slot = (AsvGbasSlotMsg)(slotAndMsg & 0b0000_0111);
        GbasMessageId = (byte)(slotAndMsg >> 3);
        ActiveSlots = (AsvGbasSlot)BinSerialize.ReadByte(ref buffer);
        LifeTime = BinSerialize.ReadByte(ref buffer);

        var flags = BinSerialize.ReadByte(ref buffer);
        LastByteOffset = (byte)(flags & 0b0000_0111);
        IsLastSlotInFrame = ((flags >> 3) & 0b0000_0001) != 0;
        ReservedFlags = (byte)(flags >> 4);

        Data = buffer.ToArray();
        buffer = buffer[Data.Length..];
    }

    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        BinSerialize.WriteByte(ref buffer, (byte)((byte)Slot | (GbasMessageId << 3)));
        BinSerialize.WriteByte(ref buffer, (byte)ActiveSlots);
        BinSerialize.WriteByte(ref buffer, LifeTime);
        BinSerialize.WriteByte(ref buffer, (byte)((LastByteOffset & 0b0000_0111) |
                                                 ((IsLastSlotInFrame ? 1 : 0) << 3) |
                                                 (ReservedFlags << 4)));
        Data.CopyTo(buffer);
        buffer = buffer[Data.Length..];
    }

    protected override int GetPayloadByteSize() => Data.Length + 4;

    public override void Randomize(Random random)
    {
        Sequence = (ushort)random.Next(0, ushort.MaxValue);
        TargetId = (byte)random.Next(0, byte.MaxValue);
        SenderId = (byte)random.Next(0, byte.MaxValue);
        Data = new byte[random.Next(0, AsvProtocol.MaxPayloadSize - 4)];
        random.NextBytes(Data);
        LastByteOffset = (byte)random.Next(0, 8);
        ReservedFlags = (byte)random.Next(0, 16);
        ActiveSlots = (AsvGbasSlot)random.Next(0, byte.MaxValue);
        Slot = (AsvGbasSlotMsg)random.Next(0, Enum.GetValues<AsvGbasSlotMsg>().Length);
        GbasMessageId = (byte)random.Next(0, 32);
        LifeTime = (byte)random.Next(0, byte.MaxValue);
        IsLastSlotInFrame = random.Next() % 2 == 0;
    }
}

public enum AsvGbasSlotMsg : byte
{
    SlotA = 0,
    SlotB = 1,
    SlotC = 2,
    SlotD = 3,
    SlotE = 4,
    SlotF = 5,
    SlotG = 6,
    SlotH = 7,
}
