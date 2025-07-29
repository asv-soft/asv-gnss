using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn017Type300 : AsterixField
{
    public const byte StaticFrn = 17;
    public override string Name => "Vehicle Fleet Identification";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        buffer = buffer[GetByteSize()..]; // TODO: Implement deserialization logic
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        
    }

    public override int GetByteSize() => 1;

    public override void Accept(IVisitor visitor)
    {
        
    }
}