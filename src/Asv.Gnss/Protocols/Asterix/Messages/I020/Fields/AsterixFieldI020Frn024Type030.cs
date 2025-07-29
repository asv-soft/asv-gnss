using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn024Type030 : AsterixField
{
    public const byte StaticFrn = 24;
    public override string Name => "Warning/Error Conditions";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer) => RawValue.Deserialize(ref buffer);

    public override void Serialize(ref Span<byte> buffer) => RawValue.Serialize(ref buffer);

    public override int GetByteSize() => RawValue.GetByteSize();
    
    public VariableLengthValue RawValue { get; } = new();

    public override void Accept(IVisitor visitor)
    {
        
    }
}