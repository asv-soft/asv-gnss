using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI002Frn013TypeSp : AsterixFieldI002Fixed
{
    public const byte StaticFrn = 13;
    public override string Name => "Special Purpose Field";
    public override byte FieldReferenceNumber => StaticFrn;
    public byte[] Data { get; set; } = [];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var length = buffer[0];
        buffer = buffer[1..];
        Data = length == 0 ? [] : buffer[..(length - 1)].ToArray();
        buffer = buffer[Data.Length..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = checked((byte)(Data.Length + 1));
        Data.CopyTo(buffer[1..]);
        buffer = buffer[(Data.Length + 1)..];
    }

    public override int GetByteSize() => Data.Length + 1;
}