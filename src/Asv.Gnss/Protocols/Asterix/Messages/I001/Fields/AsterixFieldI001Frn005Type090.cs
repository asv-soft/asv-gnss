using System;
using System.Buffers.Binary;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX CAT001 field I001/090: Mode-C Code in Binary Representation.
/// Contains validation flags and altitude derived from the Mode-C reply.
/// </summary>
public class AsterixFieldI001Frn005Type090 : AsterixField
{
    /// <summary>
    /// Field reference number for I001/090.
    /// </summary>
    public const byte StaticFrn = 5;

    /// <summary>
    /// Human-readable field name.
    /// </summary>
    public const string StaticName = "Mode-C Code in Binary Representation";

    /// <inheritdoc />
    public override string Name => StaticName;

    /// <inheritdoc />
    public override int Category => AsterixMessageI001.Category;

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    private ushort _rawValue;

    /// <summary>
    /// Gets or sets a value indicating whether the Mode-C code is not validated.
    /// </summary>
    public bool V
    {
        get => (_rawValue & 0x8000) != 0;
        set => SetBit(0x8000, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Mode-C code is garbled.
    /// </summary>
    public bool G
    {
        get => (_rawValue & 0x4000) != 0;
        set => SetBit(0x4000, value);
    }

    /// <summary>
    /// Gets or sets the decoded Mode-C height in feet.
    /// </summary>
    public double HeightFt
    {
        get
        {
            var raw = _rawValue & 0x3FFF;
            if ((raw & 0x2000) != 0)
            {
                raw |= unchecked((int)0xFFFFC000);
            }

            return raw * 25.0;
        }
        set
        {
            var scaled = (int)Math.Round(value / 25.0);
            _rawValue = (ushort)((_rawValue & 0xC000) | (scaled & 0x3FFF));
        }
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

    private void SetBit(int mask, bool value)
    {
        if (value)
        {
            _rawValue = (ushort)(_rawValue | mask);
        }
        else
        {
            _rawValue = (ushort)(_rawValue & ~mask);
        }
    }
}
