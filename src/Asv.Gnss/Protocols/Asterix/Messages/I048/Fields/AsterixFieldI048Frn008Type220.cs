using System;

namespace Asv.Gnss;

/// <summary>
/// I048/220 Aircraft Address.
/// </summary>
public sealed class AsterixFieldI048Frn008Type220 : AsterixFieldI048
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 8;

    /// <inheritdoc />
    public override string Name => "Aircraft Address";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// 24-bit aircraft address.
    /// </summary>
    public uint AircraftAddress { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        AircraftAddress = AsterixI048Binary.ReadUInt24(ref buffer);
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI048Binary.WriteUInt24(ref buffer, AircraftAddress);
    }

    /// <inheritdoc />
    public override int GetByteSize() => 3;
}