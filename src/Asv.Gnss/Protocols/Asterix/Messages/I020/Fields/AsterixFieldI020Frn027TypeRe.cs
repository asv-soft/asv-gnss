using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn027TypeRe : AsterixField
{
    public const byte StaticFrn = 27;
    public override string Name => "Reserved Expansion Field";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var length = buffer[0];
        buffer = buffer[1..];
        if (length == 0)
        {
            return;
        }
        Data = new byte[length - 1];
        buffer[..Data.Length].CopyTo(Data);
        buffer = buffer[Data.Length..];
    }

    public byte[] Data { get; set; } = [];

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Data.Length;
        Data.CopyTo(buffer[1..]);
        buffer = buffer[(1 + Data.Length)..];
    }

    public override int GetByteSize() => 1 + Data.Length;


    public override void Accept(IVisitor visitor)
    {
        
    }
}