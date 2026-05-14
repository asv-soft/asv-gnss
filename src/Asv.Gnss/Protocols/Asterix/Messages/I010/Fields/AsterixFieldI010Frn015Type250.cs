using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// I010/250 Mode S MB Data.
/// </summary>
public class AsterixFieldI010Frn015Type250 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 15;

    /// <inheritdoc />
    public override string Name => "Mode S MB Data";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Repetitive Mode S MB blocks.
    /// </summary>
    public List<AsterixI010ModeSMbData> Items { get; } = [];

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Items.Clear();
        for (var i = 0; i < count; i++)
        {
            Items.Add(new AsterixI010ModeSMbData(buffer[..8].ToArray()));
            buffer = buffer[8..];
        }
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Items.Count;
        buffer = buffer[1..];
        foreach (var item in Items)
        {
            item.Data.CopyTo(buffer);
            buffer = buffer[8..];
        }
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + Items.Count * 8;
}