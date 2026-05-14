using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX CAT001 field I001/130: Radar Plot Characteristics.
/// Contains a variable-length chain of characteristic values, each followed by an extension bit.
/// </summary>
public class AsterixFieldI001Frn006Type130 : AsterixField
{
    /// <summary>
    /// Field reference number for I001/130.
    /// </summary>
    public const byte StaticFrn = 6;

    /// <summary>
    /// Human-readable field name.
    /// </summary>
    public const string StaticName = "Radar Plot Characteristics";

    /// <inheritdoc />
    public override string Name => StaticName;

    /// <inheritdoc />
    public override int Category => AsterixMessageI001.Category;

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Gets the decoded radar plot characteristic values in wire order.
    /// </summary>
    public List<RadarPlotCharacteristic> Items { get; } = [];

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Items.Clear();
        bool extend;
        do
        {
            var raw = buffer[0];
            buffer = buffer[1..];
            extend = (raw & 0x01) != 0;
            Items.Add(new RadarPlotCharacteristic((byte)(raw >> 1), extend));
        }
        while (extend);
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var extend = i < Items.Count - 1;
            buffer[0] = (byte)((item.Value << 1) | (extend ? 1 : 0));
            buffer = buffer[1..];
        }
    }

    /// <inheritdoc />
    public override int GetByteSize() => Items.Count;

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
    }
}

/// <summary>
/// One I001/130 radar plot characteristic subfield value.
/// </summary>
/// <param name="Value">The seven-bit characteristic value.</param>
/// <param name="Extend">True when another radar plot characteristic octet follows.</param>
public readonly record struct RadarPlotCharacteristic(byte Value, bool Extend);
