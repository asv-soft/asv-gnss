using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn016Type210 : AsterixField
{
    public const byte StaticFrn = 16;
    public override string Name => "Calculated Acceleration";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public double Ax { get; set; }
    public double Ay { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        buffer = buffer[GetByteSize()..]; // TODO: Implement deserialization logic
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        
    }

    public override int GetByteSize() => 2;

    public override void Accept(IVisitor visitor)
    {
        
    }
}