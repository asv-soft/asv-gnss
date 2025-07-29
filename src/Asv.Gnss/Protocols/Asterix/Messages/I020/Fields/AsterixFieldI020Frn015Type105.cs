using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn015Type105 : AsterixField
{
    public const byte StaticFrn = 15;
    public override string Name => "Geometric Height (WGS-84)";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
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