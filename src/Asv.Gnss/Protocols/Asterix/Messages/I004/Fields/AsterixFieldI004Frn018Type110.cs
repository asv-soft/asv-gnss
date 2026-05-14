using System;
using System.Collections.Generic;

namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn018Type110 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 18;
    public override string Name => "FDPS Sector Control Identification";
    public override byte FieldReferenceNumber => StaticFrn;
    public List<AsterixI004FdpsSectorControl> Items { get; } = [];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Items.Clear();
        for (var i = 0; i < count; i++)
        {
            Items.Add(new AsterixI004FdpsSectorControl
            {
                Centre = buffer[0],
                Position = buffer[1]
            });
            buffer = buffer[2..];
        }
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = checked((byte)Items.Count);
        buffer = buffer[1..];
        foreach (var item in Items)
        {
            buffer[0] = item.Centre;
            buffer[1] = item.Position;
            buffer = buffer[2..];
        }
    }

    public override int GetByteSize() => 1 + Items.Count * 2;
}