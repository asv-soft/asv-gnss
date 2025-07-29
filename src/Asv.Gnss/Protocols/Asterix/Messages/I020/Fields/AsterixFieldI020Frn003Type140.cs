using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn003Type140 : AsterixField
{
    public const byte StaticFrn = 3;
    
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        TimeField
    ]);
    
    public const string StaticName = "Time of Day";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var pos = 0;
        var sec128 = SpanBitHelper.GetBitU(buffer, ref pos, 24);
        var sec =  sec128 / 128.0;
        buffer = buffer[3..];
        Time = TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(sec));
    }

    private static readonly Field TimeField = new Field.Builder()
        .Name(nameof(Time))
        .DataType(DoubleType.Default)
        .Title("Time of Day")
        .Description("Time of Day in seconds")
        .Build();
    public TimeOnly Time { get; set; }

    public override void Serialize(ref Span<byte> buffer)
    {
        var pos = 0;
        SpanBitHelper.SetBitU(buffer, ref pos, 24, (uint)(Time.ToTimeSpan().TotalSeconds * 128.0));
        buffer = buffer[3..];
    }

    public override int GetByteSize() => 3;

    public override void Accept(IVisitor visitor)
    {
        var temp = Time.ToTimeSpan().TotalSeconds;
        DoubleType.Accept(visitor, TimeField, TimeField.DataType, ref temp);
        Time = TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(temp));
    }
}