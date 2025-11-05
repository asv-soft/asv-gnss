using System;
using System.Buffers.Binary;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn008Type070 : AsterixField
{
    private ushort _rawValue;
    public const byte StaticFrn = 8;
    
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        VField,
        GField,
        LField,
        Mode3ACodeField
    ]);
    
    public const string StaticName = "Mode-3/A Code in Octal Representation";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;

    private static readonly Field VField = new Field.Builder()
        .Name(nameof(V))
        .Title("V")
        .Description("0 Code validated, 1 Code not validated")
        .DataType(BoolType.Default)
        .Build();
    public bool V
    {
        get => (_rawValue & (1 << 15)) != 0;
        set
        {
            if (value)
            {
                _rawValue |= (1 << 15);
            }
            else
            {
                _rawValue &= unchecked((ushort)~(1 << 15));
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
        get => (_rawValue & (1 << 14)) != 0;
        set
        {
            if (value)
            {
                _rawValue |= (1 << 14);
            }
            else
            {
                _rawValue &= unchecked((ushort)~(1 << 14));
            }
        }
    }
    
    private static readonly Field LField = new Field.Builder()
        .Name(nameof(L))
        .Title("L")
        .Description("0 Mode-3/A code derived from the reply of the transponder, 1 Mode-3/A code not extracted during the last update period")
        .DataType(BoolType.Default)
        .Build();
    public bool L 
    {
        get => (_rawValue & (1 << 13)) != 0;
        set
        {
            if (value)
            {
                _rawValue |= (1 << 13);
            }
            else
            {
                _rawValue &= unchecked((ushort)~(1 << 13));
            }
        }
    }

    private static readonly Field Mode3ACodeField = new Field.Builder()
        .Name(nameof(Mode3ACode))
        .Title(nameof(Mode3ACode))
        .DataType(UInt16Type.Default)
        .Build();
    public ushort Mode3ACode
    {
        get
        {
            var d = (_rawValue & 0x7);
            var c = ((_rawValue >> 3) & 0x7);
            var b = ((_rawValue >> 6) & 0x7);
            var a = ((_rawValue >> 9) & 0x7);

            return (ushort)(a * 1000 + b * 100 + c * 10 + d);
        }
        set
        {
            // Convert decimal Mode 3/A code to octal digits
            var a = (value / 1000) % 10;  // Thousands digit
            var b = (value / 100) % 10;   // Hundreds digit  
            var c = (value / 10) % 10;    // Tens digit
            var d = value % 10;           // Units digit
            
            // Clear the lower 12 bits (Mode 3/A code area) and preserve upper bits
            _rawValue = (ushort)((_rawValue & 0xF000) | ((a & 0x7) << 9) | ((b & 0x7) << 6) | ((c & 0x7) << 3) | (d & 0x7));
        }
    }


    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        _rawValue = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, _rawValue);
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;

    public override void Accept(IVisitor visitor)
    {
        var temp = V;
        BoolType.Accept(visitor, VField, VField.DataType, ref temp);
        V = temp;
        
        temp = G;
        BoolType.Accept(visitor, GField, GField.DataType, ref temp);
        G = temp;
        
        temp = L;
        BoolType.Accept(visitor, LField, LField.DataType, ref temp);
        L = temp;
        
        var value = Mode3ACode;
        UInt16Type.Accept(visitor, Mode3ACodeField, Mode3ACodeField.DataType, ref value);
        Mode3ACode = value;
    }
}