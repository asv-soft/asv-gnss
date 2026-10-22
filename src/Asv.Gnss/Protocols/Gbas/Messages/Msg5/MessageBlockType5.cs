using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public partial class MessageBlockType5 : GbasMessageBlockBase
{
    public const byte GbasMessageTypeId = 5;

    public override byte MessageTypeId => GbasMessageTypeId;

    public double ModifiedZCount { get; set; }

    public List<ImpactedSource> ImpactedSources { get; } = new();

    public List<ObstructedApproaches> ObstructedApproaches { get; } = new();

    public override int GetBitSize()
    {
        return base.GetBitSize() + 16 /*ModifiedZCount*/ + 8 /*ImpactedSources.Count*/ +
               8 /*ObstructedApproaches.Count*/ + ImpactedSources.Sum(_ => _.GetBitSize()) +
               ObstructedApproaches.Sum(_ => _.GetBitSize());
    }

    protected override void InternalSerialize(Span<byte> buffer, ref int bitPosition)
    {
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 14, Math.Round(ModifiedZCount / 0.1));
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 2, 0);

        if (ImpactedSources.Count > byte.MaxValue) throw new InvalidOperationException("Too many impacted sources.");
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, ImpactedSources.Count);
        foreach (var source in ImpactedSources)
        {
            source.Serialize(buffer, ref bitPosition);
        }

        if (ObstructedApproaches.Count > byte.MaxValue) throw new InvalidOperationException("Too many obstructed approaches.");
        SpanBitHelper.SetBitUReverse(buffer, ref bitPosition, 8, ObstructedApproaches.Count);
        foreach (var source in ObstructedApproaches)
        {
            source.Serialize(buffer, ref bitPosition);
        }
    }

    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitPosition, int dataBitSize)
    {
        if (dataBitSize < 32) throw new FormatException("GBAS type 5 payload is truncated.");
        ModifiedZCount = SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 14) * 0.1;
        bitPosition += 2; // reserved
        var impactedSourcesNumber = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        ImpactedSources.Clear();
        for (var i = 0; i < impactedSourcesNumber; i++)
        {
            var source = new ImpactedSource();
            source.Deserialize(buffer, ref bitPosition);
            ImpactedSources.Add(source);
        }

        ObstructedApproaches.Clear();
        var obstructedApproachesNumber = (byte)SpanBitHelper.GetBitUReverse(buffer, ref bitPosition, 8);
        for (var i = 0; i < obstructedApproachesNumber; i++)
        {
            var approaches = new ObstructedApproaches();
            approaches.Deserialize(buffer, ref bitPosition);
            ObstructedApproaches.Add(approaches);
        }
    }
}
