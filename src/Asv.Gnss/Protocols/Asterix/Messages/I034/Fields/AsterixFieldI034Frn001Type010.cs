using System;

namespace Asv.Gnss;

/// <summary>
/// I034/010 Data Source Identifier.
/// </summary>
public sealed class AsterixFieldI034Frn001Type010 : AsterixFieldI034
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 1;

    /// <inheritdoc />
    public override string Name => "Data Source Identifier";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// System Area Code.
    /// </summary>
    public SystemAreaCode Sac { get; set; }

    /// <summary>
    /// System Identification Code.
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
}