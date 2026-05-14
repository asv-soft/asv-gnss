using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// ASTERIX CAT001 field I001/010: Data Source Identifier.
/// Identifies the system from which the data was received by SAC and SIC.
/// </summary>
public class AsterixFieldI001Frn001Type010 : AsterixField
{
    /// <summary>
    /// Field reference number for I001/010.
    /// </summary>
    public const byte StaticFrn = 1;

    /// <summary>
    /// Human-readable field name.
    /// </summary>
    public const string StaticName = "Data Source Identifier";

    /// <inheritdoc />
    public override string Name => StaticName;

    /// <inheritdoc />
    public override int Category => AsterixMessageI001.Category;

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Gets or sets the System Area Code.
    /// </summary>
    public SystemAreaCode Sac { get; set; }

    /// <summary>
    /// Gets or sets the System Identification Code.
    /// </summary>
    public byte Sic { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Sac = (SystemAreaCode)buffer[0];
        Sic = buffer[1];
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Sac;
        buffer[1] = Sic;
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
    }
}
