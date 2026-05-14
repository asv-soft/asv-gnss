using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// I048/030 Warning/Error Conditions.
/// </summary>
public sealed class AsterixFieldI048Frn016Type030 : AsterixFieldI048
{
    public const byte StaticFrn = 16;
    public override string Name => "Warning/Error Conditions";
    public override byte FieldReferenceNumber => StaticFrn;
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
            buffer[0] = (byte)((Values[i] << 1) | (i < Values.Count - 1 ? 1 : 0));
            buffer = buffer[1..];
        }
    }
    public override int GetByteSize() => Values.Count;
}