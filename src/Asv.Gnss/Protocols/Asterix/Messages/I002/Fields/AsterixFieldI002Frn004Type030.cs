using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI002Frn004Type030 : AsterixFieldI002Fixed
{
    public const byte StaticFrn = 4;
    public override string Name => "Time of Day";
    public override byte FieldReferenceNumber => StaticFrn;
    public double Seconds { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Seconds = AsterixI002Binary.ReadUInt24(ref buffer) / 128.0;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI002Binary.WriteUInt24(ref buffer, (uint)Math.Round(Seconds * 128.0));
    }

    public override int GetByteSize() => 3;
}