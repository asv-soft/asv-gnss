using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsvMessageGbasMonDevSendV2 : AsvMessageBase
{
    public override ushort MessageId => 0x0103;
    public override string Name => "GbasCuSendV2";

    public AsvGbasSlotMsg Slot { get; set; }
    public bool IsLastSlotInFrame { get; set; }
    public byte LifeTime { get; set; }
    public ushort MsgLength { get; set; }
    public ushort MsgCrc { get; set; }

    protected override void InternalDeserialize(ref ReadOnlySpan<byte> buffer)
    {
        var slotAndMsg = BinSerialize.ReadByte(ref buffer);
        Slot = (AsvGbasSlotMsg)(slotAndMsg & 0b0000_0111);

        var flags = BinSerialize.ReadByte(ref buffer);
        IsLastSlotInFrame = (flags & 0b0000_1000) != 0;

        LifeTime = BinSerialize.ReadByte(ref buffer);
        MsgLength = BinSerialize.ReadUShort(ref buffer);
        MsgCrc = BinSerialize.ReadUShort(ref buffer);
    }

    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        BinSerialize.WriteByte(ref buffer, (byte)Slot);
        BinSerialize.WriteByte(ref buffer, (byte)((IsLastSlotInFrame ? 1 : 0) << 3));
        BinSerialize.WriteByte(ref buffer, LifeTime);
        BinSerialize.WriteUShort(ref buffer, MsgLength);
        BinSerialize.WriteUShort(ref buffer, MsgCrc);
    }

    protected override int GetPayloadByteSize() => 7;

    public override void Randomize(Random random)
    {
        Sequence = (ushort)random.Next(0, ushort.MaxValue);
        TargetId = (byte)random.Next(0, byte.MaxValue);
        SenderId = (byte)random.Next(0, byte.MaxValue);
        Slot = (AsvGbasSlotMsg)random.Next(0, Enum.GetValues<AsvGbasSlotMsg>().Length);
        IsLastSlotInFrame = random.Next() % 2 == 0;
        LifeTime = (byte)((random.Next() % 2 == 0) ? 1 : byte.MaxValue);
        MsgLength = (ushort)random.Next(0, ushort.MaxValue);
        MsgCrc = (ushort)random.Next(0, ushort.MaxValue);
    }
}
