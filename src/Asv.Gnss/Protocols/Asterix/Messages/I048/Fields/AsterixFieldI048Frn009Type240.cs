using System;

namespace Asv.Gnss;

/// <summary>
/// I048/240 Aircraft Identification.
/// </summary>
public sealed class AsterixFieldI048Frn009Type240 : AsterixFieldI048
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 9;

    /// <inheritdoc />
    public override string Name => "Aircraft Identification";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Aircraft identification or callsign.
    /// </summary>
    public string? AircraftIdentification { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        AircraftIdentification = AsterixProtocol.GetAircraftId(buffer[..6]);
        buffer = buffer[6..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        var data = buffer[..6];
        AsterixProtocol.SetAircraftId(AircraftIdentification, ref data);
        buffer = buffer[6..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 6;
}