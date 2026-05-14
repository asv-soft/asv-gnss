using System;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// I010/280 Presence.
/// </summary>
public class AsterixFieldI010Frn023Type280 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 23;

    /// <inheritdoc />
    public override string Name => "Presence";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Presence cells.
    /// </summary>
    public List<AsterixI010PresenceItem> Items { get; } = [];

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Items.Clear();
        for (var i = 0; i < count; i++)
        {
            Items.Add(new AsterixI010PresenceItem((sbyte)buffer[0], (sbyte)buffer[1]));
            buffer = buffer[2..];
        }
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Items.Count;
        buffer = buffer[1..];
        foreach (var item in Items)
        {
            buffer[0] = (byte)item.Drho;
            buffer[1] = (byte)item.Dtheta;
            buffer = buffer[2..];
        }
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + Items.Count * 2;
}