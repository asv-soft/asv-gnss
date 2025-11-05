using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn021Type250 : AsterixField
{
    public const byte StaticFrn = 21;
    public const string StaticName = "Mode S MB Data";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var size = buffer[0];
        buffer = buffer[1..];
        Data.Clear();
        for (var i = 0; i < size; i++)
        {
            var modeSData = new ModeSData();
            modeSData.Deserialize(ref buffer);
            Data.Add(modeSData);
        }
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)Data.Count;
        buffer = buffer[1..];
        foreach (var modeSData in Data)
        {
            modeSData.Serialize(ref buffer);
        }
        
    }

    public override int GetByteSize() => 1 + ModeSData.ByteSize;

    public override void Accept(IVisitor visitor)
    {
        ListType.Accept(visitor, DataField, DataField.DataType, _data, (index, v, f, t) =>
        {
            StructType.Accept(v,f,t,_data[index]);
        });
    }
    
    private static readonly Field DataField = new Field.Builder()
        .Name(nameof(Data))
        .DataType(new ListType(ModeSData.StructType, 0 , 10))
        .Title(nameof(ModeSData))
        .Description("Mode S Comm B data as extracted from the aircraft transponder.").Build();
    
    private readonly List<ModeSData> _data = new();
    public List<ModeSData> Data => _data;
}

public class ModeSData : ISizedSpanSerializable, IVisitable
{
    public const int ByteSize = 8;
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        RawDataField
    ]);
    
    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        buffer[..ByteSize].CopyTo(RawData);
        buffer = buffer[ByteSize..];
    }

    public void Serialize(ref Span<byte> buffer)
    {
        if (buffer.Length < RawData.Length)
            throw new ArgumentException("Buffer is too small to serialize ModeSData");
        RawData.CopyTo(buffer);
        buffer = buffer[ByteSize..];
    }

    public int GetByteSize() => ByteSize;
    public void Accept(IVisitor visitor)
    {
        ArrayType.Accept(visitor, RawDataField, (index, v, f, t) =>
        {
            var temp = RawData[index];
            UInt8Type.Accept(v,f,t, ref temp);
            RawData[index] = temp;
        });
    }
    
    private static readonly Field RawDataField = new Field.Builder()
        .Name(nameof(RawData))
        .DataType(new ArrayType(UInt8Type.Default, ByteSize))
        .Title("Raw Data")
        .Build();
    public byte[] RawData { get; } = new byte[ByteSize];
}