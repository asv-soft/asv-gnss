using System;
using System.Buffers.Binary;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX CAT001 field I001/141: Truncated Time of Day.
/// Contains the truncated time in seconds with 1/128 second resolution.
/// </summary>
public class AsterixFieldI001Frn007Type141 : AsterixField
{
    /// <summary>
    /// Field reference number for I001/141.
    /// </summary>
    public const byte StaticFrn = 7;

    /// <summary>
    /// Human-readable field name.
    /// </summary>
    public const string StaticName = "Truncated Time of Day";

    /// <inheritdoc />
    public override string Name => StaticName;

    /// <inheritdoc />
    public override int Category => AsterixMessageI001.Category;

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    private ushort _rawValue;

    /// <summary>
    /// Gets or sets the truncated time of day in seconds.
    /// </summary>
    public double Seconds
    {
        get => _rawValue / 128.0;
        set => _rawValue = checked((ushort)Math.Round(value * 128.0));
    }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        _rawValue = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, _rawValue);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
    }
}
