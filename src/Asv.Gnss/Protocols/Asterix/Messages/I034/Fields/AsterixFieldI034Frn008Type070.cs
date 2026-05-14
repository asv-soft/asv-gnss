using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace Asv.Gnss;

/// <summary>
/// I034/070 Message Count Values.
/// </summary>
public sealed class AsterixFieldI034Frn008Type070 : AsterixFieldI034
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 8;

    /// <inheritdoc />
    public override string Name => "Message Count Values";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Repetitive message count values.
    /// </summary>
    public List<AsterixI034MessageCountValue> Items { get; } = [];

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var count = buffer[0];
        buffer = buffer[1..];
        Items.Clear();
        for (var i = 0; i < count; i++)
        {
            var raw = BinaryPrimitives.ReadUInt16BigEndian(buffer);
            buffer = buffer[2..];
            Items.Add(new AsterixI034MessageCountValue
            {
                Aerial = (raw & 0x8000) != 0,
                Type = (byte)((raw >> 10) & 0x1F),
                Counter = (ushort)(raw & 0x03FF)
            });
        }
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = checked((byte)Items.Count);
        buffer = buffer[1..];
        foreach (var item in Items)
        {
            var raw = (ushort)((item.Aerial ? 0x8000 : 0) | ((item.Type & 0x1F) << 10) | (item.Counter & 0x03FF));
            BinaryPrimitives.WriteUInt16BigEndian(buffer, raw);
            buffer = buffer[2..];
        }
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1 + Items.Count * 2;
}