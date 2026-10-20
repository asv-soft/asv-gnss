using System;
using Asv.IO;

namespace Asv.Gnss;

public abstract class AdditionalDataBlock : SpanBitCompactSerializableBase
{
    public override int GetBitSize()
    {
        return 2 * 8; /*Length + ID*/
    }

    public override void Serialize(Span<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, GetByteSize());
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, TypeNumber);
        InternalSerialize(buffer, ref bitPosition);
    }

    protected abstract void InternalSerialize(Span<byte> buffer, ref int bitPosition);


    public override void Deserialize(ReadOnlySpan<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        var size = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        var type = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        if (type != TypeNumber)
        {
            throw new FormatException($"AdditionalDataBlock type error: want {TypeNumber}, got {type}");
        }
        if (size != GetByteSize()) throw new FormatException($"Invalid additional data block length: {size}.");
        InternalDeserialize(buffer, ref bitPosition, size);
    }

    protected abstract void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitPosition, byte byteSize);

    public abstract byte TypeNumber { get; }
}
