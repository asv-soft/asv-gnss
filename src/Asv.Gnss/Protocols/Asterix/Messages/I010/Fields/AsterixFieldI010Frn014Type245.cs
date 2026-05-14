using System;

namespace Asv.Gnss;

/// <summary>
/// I010/245 Target Identification.
/// </summary>
public class AsterixFieldI010Frn014Type245 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 14;

    /// <inheritdoc />
    public override string Name => "Target Identification";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Source and type identifier.
    /// </summary>
    public StiEnum Sti { get; set; }

    /// <summary>
    /// Aircraft identification or registration.
    /// </summary>
    public string? TargetIdentification { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Sti = (StiEnum)(buffer[0] >> 6);
        TargetIdentification = AsterixProtocol.GetAircraftId(buffer.Slice(1, 6));
        buffer = buffer[7..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((byte)Sti << 6);
        buffer = buffer[1..];
        AsterixProtocol.SetAircraftId(TargetIdentification, ref buffer);
        buffer = buffer[6..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 7;
}