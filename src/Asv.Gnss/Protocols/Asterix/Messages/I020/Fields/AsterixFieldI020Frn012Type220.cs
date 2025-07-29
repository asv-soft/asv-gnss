using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn012Type220 : AsterixField
{
    public const byte StaticFrn = 12;
    public override string Name => "Target Address";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public int TargetAddress { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        buffer = buffer[GetByteSize()..]; // TODO: Implement deserialization logic
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        
    }

    public override int GetByteSize() => 3;

    public override void Accept(IVisitor visitor)
    {
        
    }
}