using System;

namespace Asv.Gnss;

/// <summary>
/// I048/120 Radial Doppler Speed.
/// </summary>
public sealed class AsterixFieldI048Frn020Type120 : AsterixFieldI048
{
    public const byte StaticFrn = 20;
    public override string Name => "Radial Doppler Speed";
    public override byte FieldReferenceNumber => StaticFrn;
    public byte[] Data { get; private set; } = [];
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var start = buffer;
        var primary = buffer[0];
        buffer = buffer[1..];
        if ((primary & 0x80) != 0) buffer = buffer[2..];
        if ((primary & 0x40) != 0)
        {
            var count = buffer[0];
            buffer = buffer[(1 + count * 2)..];
        }
        Data = start[..(start.Length - buffer.Length)].ToArray();
    }
    public override void Serialize(ref Span<byte> buffer)
    {
        Data.CopyTo(buffer);
        buffer = buffer[Data.Length..];
    }
    public override int GetByteSize() => Data.Length;
}