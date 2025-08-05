using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn013Type245 : AsterixField
{
    private string? _targetIdentification;
    private StiEnum _sti;
    public const byte StaticFrn = 13;
    public override string Name => "Target Identification";
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;
    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Sti = (StiEnum)(buffer[0] >> 6);
        TargetIdentification = AsterixProtocol.GetAircraftId(buffer.Slice(1, 6));
        buffer = buffer[GetByteSize()..];
    }


    

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((byte)Sti << 6);
        buffer = buffer[1..];
        AsterixProtocol.SetAircraftId(TargetIdentification, ref buffer);
        buffer = buffer[GetByteSize()..];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetByteSize() => 7;

    public override void Accept(IVisitor visitor)
    {
        StringOptionalType.Accept(visitor, TargetIdentificationField, TargetIdentificationField.DataType,
            ref _targetIdentification);

        var temp = (byte)_sti;
        UInt8Type.Accept(visitor, StiField, StiField.DataType, ref temp);
        _sti = (StiEnum)temp;
    }

    private static readonly Field TargetIdentificationField = new Field.Builder()
        .Name(nameof(TargetIdentification))
        .Title("Target Identification")
        .Description("Aircraft identification (callsign or registration)")
        .DataType(new StringOptionalType(EncodingId.Ascii, 8, 8, AsterixProtocol.ValidIcaoCharacters))
        .Build();
            
    public string? TargetIdentification
    {
        get => _targetIdentification;
        set => _targetIdentification = value;
    }
    
    private static readonly Field StiField = new Field.Builder()
        .Name(nameof(Sti))
        .Title("STI")
        .Description("Source and Type Identifier")
        .DataType(Int8Type.Default)
        .Enum(StiEnumMixin.GetStiEnum())
        .Build();   
    public StiEnum Sti
    {
        get => _sti;
        set => _sti = value;
    }
}

public enum StiEnum : byte
{
    CallsignOrRegistrationNotDownlinked = 0b00,
    RegistrationDownlinked = 0b01,
    CallsignDownlinked = 0b10,
    NotDefined = 0b11
}

public static class StiEnumMixin
{
    public static IEnumerable<EnumValue<StiEnum>> GetStiEnum()
    {
        yield return new EnumValue<StiEnum>(StiEnum.CallsignOrRegistrationNotDownlinked, "Callsign or registration not downlinked from transponder");
        yield return new EnumValue<StiEnum>(StiEnum.RegistrationDownlinked, "Registration downlinked from transponder");
        yield return new EnumValue<StiEnum>(StiEnum.CallsignDownlinked, "Callsign downlinked from transponder");
        yield return new EnumValue<StiEnum>(StiEnum.NotDefined, "Not defined");
    }
}