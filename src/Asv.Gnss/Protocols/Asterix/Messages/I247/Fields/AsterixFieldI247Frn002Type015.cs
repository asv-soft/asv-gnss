using System;

namespace Asv.Gnss;

/// <summary>
/// I247/015 Service Identification.
/// </summary>
public class AsterixFieldI247Frn002Type015 : AsterixFieldI247
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 2;

    /// <inheritdoc />
    public override string Name => "Service Identification";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Identification of the service provided to one or more users.
    /// </summary>
    public byte ServiceIdentification { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        ServiceIdentification = buffer[0];
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = ServiceIdentification;
        buffer = buffer[1..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 1;
}