using System;
using Asv.IO;

namespace Asv.Gnss;

public class RtcmV3Message1006 : RtcmV3Message1005and1006
{
    public static readonly ushort MessageId = 1006;

    public override string Name => "Stationary RTK reference station ARP with height";
    public override ushort Id => MessageId;

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
    {
        base.InternalDeserialize(buffer, ref bitIndex);
        Height = SpanBitHelper.GetBitU(buffer, ref bitIndex, 16) * 0.0001;
    }

    protected override void InternalSerialize(Span<byte> buffer, ref int bitIndex)
    {
        base.InternalSerialize(buffer, ref bitIndex);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 16, (uint)Math.Round(Height * 10000));
    }

    protected override int InternalGetBitSize()
    {
        return base.InternalGetBitSize() + 16;
    }
}