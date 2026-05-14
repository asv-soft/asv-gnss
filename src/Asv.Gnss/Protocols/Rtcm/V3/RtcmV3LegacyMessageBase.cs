using System;
using Asv.IO;

namespace Asv.Gnss;

public abstract class RtcmV3LegacyMessageBase : RtcmV3MessageBase
{
    public abstract ushort MessageId { get; }
    public override ushort Id => MessageId;

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
    {
        var lengthBitIndex = 14;
        var messageLength = (int)SpanBitHelper.GetBitU(buffer, ref lengthBitIndex, 10);
        DeserializeContent(buffer, ref bitIndex, messageLength);
    }

    protected abstract void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength);

    protected override void InternalSerialize(Span<byte> buffer, ref int bitIndex)
    {
        throw new NotImplementedException();
    }

    protected override int InternalGetBitSize()
    {
        throw new NotImplementedException();
    }
}
