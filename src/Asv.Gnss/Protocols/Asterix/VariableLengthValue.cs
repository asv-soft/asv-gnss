using System;
using System.Collections;
using System.Diagnostics;
using Asv.IO;

namespace Asv.Gnss;

public class VariableLengthValue : ISizedSpanSerializable
{
    private byte[] _rawData = [0x00];

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

    public bool? this[int octet, int bitNumber2To8]
    {
        get
        {
            if (_rawData.Length <= octet)
            {
                return null;
            }
            if (bitNumber2To8 is < 2 or > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bitNumber2To8), "Bit number must be between 2 and 8.");
            }
            return (_rawData[octet] & (1 << (bitNumber2To8 - 1))) != 0;
        }
        set
        {
            if (_rawData.Length <= octet)
            {
                Array.Resize(ref _rawData, octet + 1);
            }
        
            if (bitNumber2To8 is < 2 or > 8)
            {
                throw new ArgumentOutOfRangeException(nameof(bitNumber2To8), "Bit number must be between 2 and 8.");
            }

            if (value.HasValue)
            {
                if (value.Value)
                {
                    _rawData[octet] |= (byte)(1 << (bitNumber2To8 - 1));
                }
                else
                {
                    _rawData[octet] &= (byte)~(1 << (bitNumber2To8 - 1));
                }
            }
            else
            {
                
            }
        }
    }

    public bool GetBoolFromFirstOctet(int bitNumberFrom2To8)
    {
        if (bitNumberFrom2To8 is < 2 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(bitNumberFrom2To8), "Bit number must be between 2 and 8.");
        }
        
        return (_rawData[0] & (1 << (bitNumberFrom2To8 - 1))) != 0;
    }
    
    public void SetBoolToFirstOctet(int bitNumberFrom2To8, bool value)
    {
        if (bitNumberFrom2To8 is < 2 or > 8)
            throw new ArgumentOutOfRangeException(nameof(bitNumberFrom2To8), "Bit number must be between 2 and 8.");

        if (value)
        {
            _rawData[0] |= (byte)(1 << (bitNumberFrom2To8 - 1));
        }
        else
        {
            _rawData[0] &= (byte)~(1 << (bitNumberFrom2To8 - 1));
        }
    }

    public bool? GetBoolFromExtend(int octetFrom1, int bitNumberFrom2To8)
    {
        if (octetFrom1 < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(octetFrom1), "Octet number for extended field must be greater than or equal to 1.");
        }

        if (_rawData.Length <= octetFrom1)
        {
            return null;
        }
        
        if (bitNumberFrom2To8 is < 2 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(bitNumberFrom2To8), "Bit number must be between 2 and 8.");
        }
        
        return (_rawData[octetFrom1] & (1 << (bitNumberFrom2To8 - 1))) != 0;
        
    }

    public void SetBoolToExtend(int octedFrom1, int bitNumber2To8, bool? value)
    {
        if (octedFrom1 < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(octedFrom1), "Octet number for extended field must be greater than or equal to 1.");
        }

        if (_rawData.Length <= octedFrom1)
        {
            Array.Resize(ref _rawData, octedFrom1 + 1);
            for (var i = 0; i < _rawData.Length - 1; i++)
            {
                _rawData[i] |= 0b0000_0001; 
            }
        }
        
        if (bitNumber2To8 is < 2 or > 8)
        {
            throw new ArgumentOutOfRangeException(nameof(bitNumber2To8), "Bit number must be between 2 and 8.");
        }

        if (value.HasValue)
        {
            if (value.Value)
            {
                _rawData[octedFrom1] |= (byte)(1 << (bitNumber2To8 - 1));
            }
            else
            {
                _rawData[octedFrom1] &= (byte)~(1 << (bitNumber2To8 - 1));
            }
        }
    }
}