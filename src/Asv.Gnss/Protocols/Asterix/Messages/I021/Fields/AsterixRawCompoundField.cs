using System;
using System.Collections.Generic;

namespace Asv.Gnss;

public abstract class AsterixRawCompoundField : AsterixFieldI021Fixed
{
    private readonly List<byte> _fspecBytes = [];
    public byte[] Data { get; set; } = [];

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var start = buffer;
        var fspec = new VariableLengthValue();
        fspec.Deserialize(ref buffer);
        var dataStartLength = buffer.Length;
        for (var i = 0; i < fspec.DataBitsCount; i++)
        {
            if (fspec[i] != true)
            {
                continue;
            }

            var size = GetItemByteSize(i, buffer);
            buffer = buffer[size..];
        }

        var totalSize = start.Length - buffer.Length;
        Data = start[..totalSize].ToArray();
        _fspecBytes.Clear();
        _fspecBytes.AddRange(start[..(totalSize - (dataStartLength - buffer.Length))].ToArray());
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        Data.CopyTo(buffer);
        buffer = buffer[Data.Length..];
    }

    public override int GetByteSize() => Data.Length;
    protected abstract int GetItemByteSize(int dataBitIndex, ReadOnlySpan<byte> buffer);
}