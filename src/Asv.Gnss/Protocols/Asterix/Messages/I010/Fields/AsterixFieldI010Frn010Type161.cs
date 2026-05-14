using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

/// <summary>
/// I010/161 Track Number.
/// </summary>
public class AsterixFieldI010Frn010Type161 : AsterixFieldI010
{
    /// <summary>
    /// Field reference number.
    /// </summary>
    public const byte StaticFrn = 10;

    /// <inheritdoc />
    public override string Name => "Track Number";

    /// <inheritdoc />
    public override byte FieldReferenceNumber => StaticFrn;

    /// <summary>
    /// Track number.
    /// </summary>
    public ushort TrackNumber { get; set; }

    /// <inheritdoc />
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        TrackNumber = (ushort)(BinaryPrimitives.ReadUInt16BigEndian(buffer) & 0x0FFF);
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)(TrackNumber & 0x0FFF));
        buffer = buffer[2..];
    }

    /// <inheritdoc />
    public override int GetByteSize() => 2;
}