using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn045Type260 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 40;
    public override string Name => "ACAS Resolution Advisory Report";
    public override byte FieldReferenceNumber => StaticFrn;
    public byte[] MbData { get; } = new byte[7];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        buffer[..7].CopyTo(MbData);
        buffer = buffer[7..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        MbData.CopyTo(buffer);
        buffer = buffer[7..];
    }

    public override int GetByteSize() => 7;
}