using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn022Type230 : AsterixField
{
    public const byte StaticFrn = 22;
    public override string Name => "Comms/ACAS Capability and Flight Status";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public bool AIC { get; set; }
    public bool ARC { get; set; }
    public bool B1A { get; set; }
    public bool B1B { get; set; }
    public bool COM { get; set; }
    public bool MSSC { get; set; }
    public bool STAT { get; set; }

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