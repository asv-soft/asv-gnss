using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn004Type015 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 4;
    public override string Name => "Service Identification";
    public override byte FieldReferenceNumber => StaticFrn;
    public byte ServiceIdentification { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        ServiceIdentification = buffer[0];
        buffer = buffer[1..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = ServiceIdentification;
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}