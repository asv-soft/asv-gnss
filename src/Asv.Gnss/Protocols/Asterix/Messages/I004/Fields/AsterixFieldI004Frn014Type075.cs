using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn014Type075 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 14;
    public override string Name => "Transversal Distance Deviation";
    public override byte FieldReferenceNumber => StaticFrn;
    public double TransversalDistanceDeviationM { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        TransversalDistanceDeviationM = AsterixI004Binary.ReadInt24(ref buffer) * 0.5;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI004Binary.WriteInt24(ref buffer, (int)Math.Round(TransversalDistanceDeviationM / 0.5));
    }

    public override int GetByteSize() => 3;
}