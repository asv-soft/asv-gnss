using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn013Type245 : AsterixField
{
    public const byte StaticFrn = 13;
    public override string Name => "Target Identification";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        buffer = buffer[GetByteSize()..]; // TODO: Implement deserialization logic
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        
    }

    public override int GetByteSize() => 7;

    public override void Accept(IVisitor visitor)
    {
        
    }
}