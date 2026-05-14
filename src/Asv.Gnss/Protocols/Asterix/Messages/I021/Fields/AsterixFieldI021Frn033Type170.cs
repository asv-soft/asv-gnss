using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn033Type170 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 29;
    public override string Name => "Target Identification";
    public override byte FieldReferenceNumber => StaticFrn;
    public string? TargetIdentification { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        TargetIdentification = AsterixProtocol.GetAircraftId(buffer[..6]);
        buffer = buffer[6..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var data = buffer[..6];
        AsterixProtocol.SetAircraftId(TargetIdentification, ref data);
        buffer = buffer[6..];
    }

    public override int GetByteSize() => 6;
}