using System;

namespace Asv.Gnss;

public abstract class AsterixI004CompoundField : AsterixFieldI004Fixed
{
    public byte[] Data { get; set; } = [];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var start = buffer;
        var fspec = new VariableLengthValue();
        fspec.Deserialize(ref buffer);
        for (var i = 0; i < fspec.DataBitsCount; i++)
        {
            if (fspec[i] != true)
            {
                continue;
            }

            var size = GetItemByteSize(i, buffer);
            buffer = buffer[size..];
        }

        Data = start[..(start.Length - buffer.Length)].ToArray();
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        Data.CopyTo(buffer);
        buffer = buffer[Data.Length..];
    }

    public override int GetByteSize() => Data.Length;
    protected abstract int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer);
}