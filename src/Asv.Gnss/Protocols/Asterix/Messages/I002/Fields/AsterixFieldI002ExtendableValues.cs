using System;
using System.Collections.Generic;

namespace Asv.Gnss;

public abstract class AsterixFieldI002ExtendableValues : AsterixFieldI002Fixed
{
    public List<byte> Values { get; } = [];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Values.Clear();
        byte raw;
        do
        {
            raw = buffer[0];
            Values.Add((byte)(raw >> 1));
            buffer = buffer[1..];
        }
        while ((raw & 0x01) != 0);
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        for (var i = 0; i < Values.Count; i++)
        {
            buffer[0] = (byte)((Values[i] << 1) | (i < Values.Count - 1 ? 0x01 : 0x00));
            buffer = buffer[1..];
        }
    }

    public override int GetByteSize() => Values.Count;
}