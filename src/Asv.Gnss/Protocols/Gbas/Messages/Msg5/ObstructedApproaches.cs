using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public partial class ObstructedApproaches : SpanBitCompactSerializableBase
{
    public ObstructedApproaches()
    {
        ImpactedSources = new List<ImpactedSource>();
    }

    public byte RPDS { get; set; }

    public List<ImpactedSource> ImpactedSources { get; } = new();

    public override int GetBitSize()
    {
        return 2 * 8 + ImpactedSources.Sum(_ => _.GetBitSize());
    }

    public override void Deserialize(ReadOnlySpan<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, 16);
        RPDS = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);

        var impactedSourcesNumber = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        ImpactedSources.Clear();
        for (var i = 0; i < impactedSourcesNumber; i++)
        {
            var source = new ImpactedSource();
            source.Deserialize(buffer, ref bitPosition);
            ImpactedSources.Add(source);
        }
    }

    public override void Serialize(Span<byte> buffer, ref int bitPosition)
    {
        GbasMessageBlockBase.RequireBits(buffer.Length, bitPosition, GetBitSize());
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, RPDS);
        if (ImpactedSources.Count > byte.MaxValue) throw new InvalidOperationException("Too many impacted sources.");
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, ImpactedSources.Count);

        foreach (var source in ImpactedSources)
        {
            source.Serialize(buffer, ref bitPosition);
        }
    }
}
