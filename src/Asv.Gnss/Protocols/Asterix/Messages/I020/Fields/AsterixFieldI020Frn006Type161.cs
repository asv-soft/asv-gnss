using System;
using Asv.IO;

namespace Asv.Gnss;

using System;
using System.Buffers.Binary;
using Asv.IO;

public class AsterixFieldI020Frn006Type161 : AsterixField
{
    public const byte StaticFrn = 6;
    
    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        TrackNumberField,
    ]);
    
    public const string StaticName = "Track Number";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;

    private ushort _trackNumber;
    public ushort TrackNumber
    {
        get => _trackNumber;
        set => _trackNumber = value;
    }

    private static readonly Field TrackNumberField = new Field.Builder()
        .Name(nameof(TrackNumber))
        .Title("Track Number")
        .DataType(UInt16Type.Default)
        .Build();

   

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        TrackNumber = BinaryPrimitives.ReadUInt16BigEndian(buffer);
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, TrackNumber);
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 2;

    public override void Accept(IVisitor visitor)
    {
        UInt16Type.Accept(visitor, TrackNumberField, TrackNumberField.DataType, ref _trackNumber);
    }
}
