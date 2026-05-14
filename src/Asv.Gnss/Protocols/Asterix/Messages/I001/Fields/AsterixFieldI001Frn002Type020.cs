using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX CAT001 field I001/020: Target Report Descriptor.
/// Describes the report type and radar source flags for a CAT001 plot.
/// </summary>
public class AsterixFieldI001Frn002Type020 : AsterixField
{
    /// <summary>
    /// Field reference number for I001/020.
    /// </summary>
    public const byte StaticFrn = 2;

    /// <summary>
    /// Human-readable field name.
    /// </summary>
    public const string StaticName = "Target Report Descriptor";

    /// <inheritdoc />
    public override string Name => StaticName;

    /// <inheritdoc />
    public override int Category => AsterixMessageI001.Category;

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    private byte _rawValue;

    /// <summary>
    /// Gets or sets the target report type code.
    /// </summary>
    public byte Typ
    {
        get => (byte)((_rawValue >> 6) & 0x03);
        set => _rawValue = (byte)((_rawValue & 0x3F) | ((value & 0x03) << 6));
    }

    /// <summary>
    /// Gets or sets a value indicating whether the report is simulated.
    /// </summary>
    public bool Sim
    {
        get => (_rawValue & 0x08) != 0;
        set => SetBit(0x08, value);
    }

    /// <summary>
    /// Gets or sets the SSR/PSR report source code.
    /// </summary>
    public byte SsrPsr
    {
        get => (byte)((_rawValue >> 4) & 0x03);
        set => _rawValue = (byte)((_rawValue & 0xCF) | ((value & 0x03) << 4));
    }

    /// <summary>
    /// Gets or sets the antenna flag.
    /// </summary>
    public bool Ant
    {
        get => (_rawValue & 0x04) != 0;
        set => SetBit(0x04, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether SPI is present.
    /// </summary>
    public bool Spi
    {
        get => (_rawValue & 0x02) != 0;
        set => SetBit(0x02, value);
    }

    /// <summary>
    /// Gets or sets the report from fixed field monitor or real target flag.
    /// </summary>
    public bool Rab
    {
        get => (_rawValue & 0x01) != 0;
        set => SetBit(0x01, value);
    }

    /// <summary>
    /// Gets the extension bit. I001/020 is a single-octet field in this implementation.
    /// </summary>
    public bool Fx => false;

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        _rawValue = buffer[0];
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = _rawValue;
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
    }

    private void SetBit(byte mask, bool value)
    {
        if (value)
        {
            _rawValue |= mask;
        }
        else
        {
            _rawValue &= (byte)~mask;
        }
    }
}
