using System;

namespace Asv.Gnss;

/// <summary>
/// I010/220 Target Address.
/// </summary>
public class AsterixFieldI010Frn013Type220 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 13;

    /// <inheritdoc />
    public override string Name => "Target Address";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// 24-bit ICAO target address.
    /// </summary>
    public int TargetAddress { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        TargetAddress = AsterixI010Binary.ReadUInt24(ref buffer);
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI010Binary.WriteUInt24(ref buffer, TargetAddress);
    }

    /// <inheritdoc />
    public override int GetByteSize() => 3;
}