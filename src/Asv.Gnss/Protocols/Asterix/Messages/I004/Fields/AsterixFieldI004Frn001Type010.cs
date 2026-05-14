using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn001Type010 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 1;
    public override string Name => "Data Source Identifier";
    public override byte FieldReferenceNumber => StaticFrn;
    public SystemAreaCode Sac { get; set; }
    public byte Sic { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Sac = (SystemAreaCode)buffer[0];
        Sic = buffer[1];
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Sac;
        buffer[1] = Sic;
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;
}