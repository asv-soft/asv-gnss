using System;

namespace Asv.Gnss;

/// <summary>
/// I048/SP Special Purpose Field.
/// </summary>
public sealed class AsterixFieldI048Frn027TypeSp : AsterixFieldI048
{
    public const byte StaticFrn = 27;
    public override string Name => "Special Purpose Field";
    public override byte FieldReferenceNumber => StaticFrn;
    public byte[] Data { get; set; } = [];
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var length = buffer[0];
        Data = buffer.Slice(1, length - 1).ToArray();
        buffer = buffer[length..];
    }
    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)(Data.Length + 1);
        Data.CopyTo(buffer[1..]);
        buffer = buffer[(Data.Length + 1)..];
    }
    public override int GetByteSize() => 1 + Data.Length;
}