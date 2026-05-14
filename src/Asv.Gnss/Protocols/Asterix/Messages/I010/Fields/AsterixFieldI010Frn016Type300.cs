using System;

namespace Asv.Gnss;

/// <summary>
/// I010/300 Vehicle Fleet Identification.
/// </summary>
public class AsterixFieldI010Frn016Type300 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 16;

    /// <inheritdoc />
    public override string Name => "Vehicle Fleet Identification";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Vehicle fleet identification code.
    /// </summary>
    public byte VehicleFleetIdentification { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        VehicleFleetIdentification = buffer[0];
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = VehicleFleetIdentification;
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}