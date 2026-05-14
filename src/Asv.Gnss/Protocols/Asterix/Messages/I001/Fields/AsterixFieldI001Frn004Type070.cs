using System;
using System.Buffers.Binary;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX CAT001 field I001/070: Mode-3/A Reply in Octal Representation.
/// Contains validation flags and the transponder Mode-3/A reply code.
/// </summary>
public class AsterixFieldI001Frn004Type070 : AsterixField
{
    /// <summary>
    /// Field reference number for I001/070.
    /// </summary>
    public const byte StaticFrn = 4;

    /// <summary>
    /// Human-readable field name.
    /// </summary>
    public const string StaticName = "Mode-3/A Reply in Octal Representation";

    /// <inheritdoc />
    public override string Name => StaticName;

    /// <inheritdoc />
    public override int Category => AsterixMessageI001.Category;

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    private ushort _rawValue;

    /// <summary>
    /// Gets or sets a value indicating whether the Mode-3/A code is not validated.
    /// </summary>
    public bool V
    {
        get => (_rawValue & 0x8000) != 0;
        set => SetBit(0x8000, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Mode-3/A code is garbled.
    /// </summary>
    public bool G
    {
        get => (_rawValue & 0x4000) != 0;
        set => SetBit(0x4000, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Mode-3/A code was not extracted during the last update period.
    /// </summary>
    public bool L
    {
        get => (_rawValue & 0x2000) != 0;
        set => SetBit(0x2000, value);
    }

    /// <summary>
    /// Gets or sets the Mode-3/A reply as a four-digit octal code stored in a decimal value.
    /// </summary>
    public ushort Mode3AReply
    {
        get
        {
            var d = _rawValue & 0x7;
            var c = (_rawValue >> 3) & 0x7;
            var b = (_rawValue >> 6) & 0x7;
            var a = (_rawValue >> 9) & 0x7;

            return (ushort)(a * 1000 + b * 100 + c * 10 + d);
        }
        set
        {
            var a = (value / 1000) % 10;
            var b = (value / 100) % 10;
            var c = (value / 10) % 10;
            var d = value % 10;

            _rawValue = (ushort)((_rawValue & 0xF000) | ((a & 0x7) << 9) | ((b & 0x7) << 6) | ((c & 0x7) << 3) | (d & 0x7));
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

    private void SetBit(ushort mask, bool value)
    {
        if (value)
        {
            _rawValue |= mask;
        }
        else
        {
            _rawValue &= (ushort)~mask;
        }
    }
}
