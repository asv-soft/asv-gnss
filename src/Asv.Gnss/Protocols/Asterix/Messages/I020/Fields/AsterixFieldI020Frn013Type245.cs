using System;
using System.Runtime.CompilerServices;
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
        Sti = buffer[0]; // TODO: create enum
        TargetIdentification = AsterixProtocol.GetAircraftId(buffer.Slice(1, 6));
        buffer = buffer[GetByteSize()..];
    }

    public string? TargetIdentification { get; set; }
    public byte Sti { get; set; }

    public override void Serialize(ref Span<byte> buffer)
    {
        
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetByteSize() => 7;

    public override void Accept(IVisitor visitor)
    {
        
    }
}