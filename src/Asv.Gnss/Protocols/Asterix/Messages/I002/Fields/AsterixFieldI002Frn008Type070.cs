using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Asv.Gnss;

public sealed class AsterixFieldI002Frn008Type070 : AsterixFieldI002Fixed
{
    public const byte StaticFrn = 8;
    public override string Name => "Plot Count Values";
    public override byte FieldReferenceNumber => StaticFrn;
    public List<AsterixI002PlotCountValue> Items { get; } = [];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Items.Clear();
        for (var i = 0; i < count; i++)
        {
            var raw = BinaryPrimitives.ReadUInt16BigEndian(buffer);
            buffer = buffer[2..];
            Items.Add(new AsterixI002PlotCountValue
            {
                Aerial = (raw & 0x8000) != 0,
                Ident = (byte)((raw >> 10) & 0x1F),
                Counter = (ushort)(raw & 0x03FF)
            });
        }
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = checked((byte)Items.Count);
        buffer = buffer[1..];
        foreach (var item in Items)
        {
            var raw = (ushort)((item.Aerial ? 0x8000 : 0) | ((item.Ident & 0x1F) << 10) | (item.Counter & 0x03FF));
            BinaryPrimitives.WriteUInt16BigEndian(buffer, raw);
            buffer = buffer[2..];
        }
    }

    public override int GetByteSize() => 1 + Items.Count * 2;
}