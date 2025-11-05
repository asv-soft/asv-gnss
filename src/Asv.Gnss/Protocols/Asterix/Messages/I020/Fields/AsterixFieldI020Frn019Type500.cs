using System;
using System.Buffers.Binary;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn019Type500 : AsterixField
{
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        DopField,
        SdField,
        SdHeightField
    ]);
    
    public const byte StaticFrn = 19;
    public override string Name => "Position Accuracy";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var flags = buffer[0];
        buffer = buffer[1..];
        if ((flags & 0b1000_000) != 0)
        {
            Dop = new PostitionDop();
            Dop.Deserialize(ref buffer);
        }
        if ((flags & 0b0100_000) != 0)
        {
            Sd = new PositionSd();
            Sd.Deserialize(ref buffer);
        }
        if ((flags & 0b0010_000) != 0)
        {
            SdHeight = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 0.25;
            buffer = buffer[2..];
        }
        else
        {
            SdHeight = null;
        }
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var flags = (byte)0;
        var origin = buffer;
        buffer = buffer[1..];
        if (Dop != null)
        {
            flags |= 0b1000_000;
            Dop.Serialize(ref buffer);
        }
        if (Sd != null)
        {
            flags |= 0b0100_000;
            Sd.Serialize(ref buffer);
        }
        if (SdHeight.HasValue)
        {
            flags |= 0b0010_000;
            BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)(SdHeight.Value * 4));
            buffer = buffer[2..];
        }
        origin[0] = flags;
    }

    public override int GetByteSize() => 1 
        + Dop?.GetByteSize() ?? 0
        + Sd?.GetByteSize() ?? 0
        + (SdHeight.HasValue ? 2 : 0);

    public override void Accept(IVisitor visitor)
    {
        if (Dop != null)
        {
            StructType.Accept(visitor, DopField,DopField.DataType, Dop);
        }
        if (Sd != null)
        {
            StructType.Accept(visitor, SdField, SdField.DataType, Sd);
        }

        if (SdHeight.HasValue)
        {
            var value = SdHeight.Value;
            DoubleType.Accept(visitor, SdHeightField, SdHeightField.DataType, ref value);
            SdHeight = value;
        }
    }

    private static readonly Field DopField = new Field.Builder()
        .Name(nameof(Dop))
        .Title("DOP")
        .Description("Dilution of Precision")
        .DataType(SubObject.StructType)
        .Build();

    public PostitionDop? Dop { get; set; }

    private static readonly Field SdField = new Field.Builder()
        .Name(nameof(Sd))
        .Title("SD")
        .Description("Standard Deviation of Position")
        .DataType(SubObject.StructType)
        .Build();

    public PositionSd? Sd { get; set; }

    private static readonly Field SdHeightField = new Field.Builder()
        .Name(nameof(SdHeight))
        .Title(nameof(SdHeight))
        .Description("Standard Deviation of Geometric Height (WGS 84)")
        .DataType(DoubleType.Default)
        .Build();
    public double? SdHeight { get; set; }
}

public class PostitionDop : ISizedSpanSerializable, IVisitable
{
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        XField,
        YField,
        XyField
    ]);
    
    private static readonly Field XField = new Field.Builder()
        .Name(nameof(X))
        .Title(nameof(X))
        .Description("DOP along x axis")
        .DataType(DoubleType.Default)
        .Build();
    private double _x;
    public double X
    {
        get => _x;
        set => _x = value;
    }

    
    private static readonly Field YField = new Field.Builder()
        .Name(nameof(Y))
        .Title(nameof(Y))
        .Description("DOP along y axis")
        .DataType(DoubleType.Default)
        .Build();
    private double _y;
    public double Y
    {
        get => _y;
        set => _y = value;
    }
    
    private static readonly Field XyField = new Field.Builder()
        .Name(nameof(Xy))
        .Title(nameof(Xy))
        .Description("Combined DOP along x and y axis")
        .DataType(DoubleType.Default)
        .Build();
    private double _xy;
    public double Xy
    {
        get => _xy;
        set => _xy = value;
    }

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        X = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
        Y = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
        Xy = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
        
    }

    public void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)(X * 4));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)(Y * 4));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)(Xy * 4));
        buffer = buffer[2..];
    }

    public int GetByteSize()
    {
        return 6; // 3 * 2 bytes for X, Y, XY
    }

    public void Accept(IVisitor visitor)
    {
        DoubleType.Accept(visitor, XField, XField.DataType, ref _x);
        DoubleType.Accept(visitor, YField, YField.DataType, ref _y);
        DoubleType.Accept(visitor, XyField, XyField.DataType, ref _xy);
    }
}


public class PositionSd : ISizedSpanSerializable, IVisitable
{
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        SdXField,
        SdYField,
        CorXyField
    ]);
    
    private static readonly Field SdXField = new Field.Builder()
        .Name(nameof(SdX))
        .Title(nameof(SdX))
        .Description("Standard Deviation of X component")
        .DataType(DoubleType.Default)
        .Build();
    private double _sdX;
    public double SdX
    {
        get => _sdX;
        set => _sdX = value;
    }

    
    private static readonly Field SdYField = new Field.Builder()
        .Name(nameof(SdY))
        .Title(nameof(SdY))
        .Description("Standard Deviation of Y component")
        .DataType(DoubleType.Default)
        .Build();
    private double _sdY;
    public double SdY
    {
        get => _sdY;
        set => _sdY = value;
    }
    
    private static readonly Field CorXyField = new Field.Builder()
        .Name(nameof(CorXy))
        .Title(nameof(CorXy))
        .Description("Correlation coefficient in two’s complement")
        .DataType(DoubleType.Default)
        .Build();
    private double _corXy;
    public double CorXy
    {
        get => _corXy;
        set => _corXy = value;
    }

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        SdX = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
        SdY = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
        CorXy = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 0.25;
        buffer = buffer[2..];
        
    }

    public void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)(SdX * 4));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)(SdY * 4));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)(CorXy * 4));
        buffer = buffer[2..];
    }

    public int GetByteSize()
    {
        return 6; // 3 * 2 
    }

    public void Accept(IVisitor visitor)
    {
        DoubleType.Accept(visitor, SdXField, SdXField.DataType, ref _sdX);
        DoubleType.Accept(visitor, SdYField, SdYField.DataType, ref _sdY);
        DoubleType.Accept(visitor, CorXyField, CorXyField.DataType, ref _corXy);
    }
}