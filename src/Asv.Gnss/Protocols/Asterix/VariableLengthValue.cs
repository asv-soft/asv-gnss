using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Asv.IO;

namespace Asv.Gnss;

public class VariableLengthValue : ISizedSpanSerializable
{
    private byte[] _rawData = [0x00];
    public int DataBitsCount => _rawData.Length * 7;

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var length = 0;

        while ((buffer[length] & 0x01) != 0)
        {
            length++;
        }
        length++;
        _rawData = new byte[length];
        buffer[..length].CopyTo(_rawData);
        buffer = buffer[length..];
        
    }

    public void Serialize(ref Span<byte> buffer)
    {
        Debug.Assert(_rawData.Length > 0, "Raw data must not be empty");
        for (var i = 0; i < _rawData.Length - 1; i++)
        {
            _rawData[i] |= 0b0000_0001; 
        }
        _rawData[^1] &= 0xFE;
        _rawData.CopyTo(buffer);
        buffer = buffer[_rawData.Length..];
    }

    public int GetByteSize() => _rawData.Length;

    public bool? this[int dataBitIndex]
    {
        get
        {
            var octet = dataBitIndex / 7;
            if (_rawData.Length <= octet)
                return null;

            var bitIndex = 6 - (dataBitIndex % 7); // 0..6 → позиция битов 7..1
            return (_rawData[octet] & (1 << (bitIndex + 1))) != 0;
        }
        set
        {
            var octet = dataBitIndex / 7;
            if (_rawData.Length <= octet)
            {
                Array.Resize(ref _rawData, octet + 1);
            }

            var bitIndex = 6 - (dataBitIndex % 7);
            var mask = (byte)(1 << (bitIndex + 1));

            if (value is null)
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");

            if (value.Value)
                _rawData[octet] |= mask;
            else
                _rawData[octet] &= (byte)~mask;
        }
    }

    public override string ToString()
    {
        // byte to string conversion
        var sb = new StringBuilder();
        sb.Append('[');
        for (var i = 0; i < DataBitsCount; i++)
        {
            sb.Append(this[i]);
            if (i < DataBitsCount - 1)
            {
                sb.Append(", ");
            }
        }
        sb.Append(']');
        return sb.ToString();
    }

    public void Clear()
    {
        _rawData = [0x00];
    }
}