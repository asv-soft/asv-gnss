using System;
using System.Collections.Generic;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn044Type250 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 39;
    public override string Name => "Mode S MB Data";
    public override byte FieldReferenceNumber => StaticFrn;
    public List<AsterixI021ModeSData> Data { get; } = [];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Data.Clear();
        for (var i = 0; i < count; i++)
        {
            var item = new AsterixI021ModeSData();
            item.Deserialize(ref buffer);
            Data.Add(item);
        }
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = checked((byte)Data.Count);
        buffer = buffer[1..];
        foreach (var item in Data)
        {
            item.Serialize(ref buffer);
        }
    }

    public override int GetByteSize() => 1 + Data.Count * AsterixI021ModeSData.ByteSize;
}