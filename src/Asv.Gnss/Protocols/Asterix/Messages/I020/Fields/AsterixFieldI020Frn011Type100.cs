using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn011Type100 : AsterixField
{
    public const byte StaticFrn = 11;
    public override string Name => "Mode-C Code";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        buffer = buffer[GetByteSize()..]; // TODO: Implement deserialization logic
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        
    }

    public override int GetByteSize() => 4;

    public override void Accept(IVisitor visitor)
    {
        
    }
}