using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn025Type055 : AsterixField
{
    public const byte StaticFrn = 25;
    public override string Name => "Mode-1 Code in Octal Representation";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        buffer = buffer[GetByteSize()..]; // TODO: Implement deserialization logic
    }

    public override void Serialize(ref Span<byte> buffer)
    {
    }

    public override int GetByteSize()
    {
        return 1;
    }


    public override void Accept(IVisitor visitor)
    {
        
    }
}