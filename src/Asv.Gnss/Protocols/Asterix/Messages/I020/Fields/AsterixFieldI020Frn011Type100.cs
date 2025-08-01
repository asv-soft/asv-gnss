using System;
using System.Buffers.Binary;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Mode-C height in Gray notation as received from the
/// transponder together with the confidence level for each reply
/// bit as provided by a MSSR/Mode-S station.
/// </summary>
public class AsterixFieldI020Frn011Type100 : AsterixField
{
    private uint _rawValue;
    public const byte StaticFrn = 11;
    public override string Name => "Mode-C Code";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        _rawValue = BinaryPrimitives.ReadUInt32BigEndian(buffer);
        buffer = buffer[GetByteSize()..]; 
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt32BigEndian(buffer, _rawValue);
        buffer = buffer[4..];
    }

    public override int GetByteSize() => 4;

    public override void Accept(IVisitor visitor)
    {
        var vValue = V;
        BoolType.Accept(visitor, VField, VField.DataType, ref vValue);
        V = vValue;

        var gValue = G;
        BoolType.Accept(visitor, GField, GField.DataType, ref gValue);
        G = gValue;
        
        var squawkValue = Squawk;
        UInt16Type.Accept(visitor, SquawkField, SquawkField.DataType, ref squawkValue);
        Squawk = squawkValue;
    }
    
    private static readonly Field VField = new Field.Builder()
        .Name(nameof(V))
        .Title("V")
        .Description("0 Code validated, 1 Code not validated")
        .DataType(BoolType.Default)
        .Build();

    public bool V
    {
        get => (_rawValue & (1U << 31)) != 0;
        set
        {
            if (value)
            {
                _rawValue |= 1U << 31;
            }
            else
            {
                _rawValue &= ~(1U << 31);
            }
        }
    }

    private static readonly Field GField = new Field.Builder()
        .Name(nameof(G))
        .Title("G")
        .Description("0 Default, 1 Garbled code")
        .DataType(BoolType.Default)
        .Build();

    
    public bool G
    {
        get => (_rawValue & (1U << 30)) != 0;
        set
        {
            if (value)
            {
                _rawValue |= 1U << 30;
            }
            else
            {
                _rawValue &= ~(1U << 30);
            }
        }
    }

    private static readonly Field SquawkField = new Field.Builder()
        .Name(nameof(Squawk))
        .Title("Squawk Code")
        .Description("Mode-C code in Gray notation")
        .DataType(UInt16Type.Default)
        .Build();
    public ushort Squawk
    {
        get => AsterixProtocol.GetSquawk((ushort)((_rawValue >> 16) & 0x0FFF));
        set
        {
            var encodedValue = AsterixProtocol.SetSquawk(value);
            // Clear bits 16-27 and set new value
            _rawValue = (_rawValue & 0xF000FFFF) | ((uint)(encodedValue & 0x0FFF) << 16);
        }
    }

    private static readonly Field QSquawkField = new Field.Builder()
        .Name(nameof(QSquawk))
        .Title("QSquawk Code")
        .Description("Mode-C code in Gray notation")
        .DataType(UInt16Type.Default)
        .Build();
    public ushort QSquawk
    {
        get => AsterixProtocol.GetSquawk((ushort)(_rawValue & 0x0FFF));
        set
        {
            var grayCode = AsterixProtocol.SetSquawk(value);
            _rawValue = (_rawValue & 0xFFFFF000U) | (uint)(grayCode & 0x0FFF);
        }
    }
}