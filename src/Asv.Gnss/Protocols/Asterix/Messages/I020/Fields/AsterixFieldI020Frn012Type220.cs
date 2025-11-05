using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn012Type220 : AsterixField
{
    
    public const byte StaticFrn = 12;
    public override string Name => "Target Address";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;

    
    private static readonly Field TargetAddressField = new Field.Builder()
        .Name(nameof(TargetAddress))
        .Title(nameof(TargetAddress))
        .DataType(UInt32Type.Default)
        .Build();
    private uint _targetAddress;
    public uint TargetAddress
    {
        get => _targetAddress;
        set => _targetAddress = value;
    }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var pos = 0;
        _targetAddress = SpanBitHelper.GetBitU(buffer, ref pos, 24);
        buffer = buffer[GetByteSize()..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var pos = 0;
        SpanBitHelper.SetBitU(buffer, ref pos, 24, _targetAddress);
        buffer = buffer[GetByteSize()..];
    }   

    public override int GetByteSize() => 3;

    public override void Accept(IVisitor visitor)
    {
        UInt32Type.Accept(visitor, TargetAddressField, TargetAddressField.DataType, ref _targetAddress);
    }
}