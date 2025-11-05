using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Data Item I020/310, Pre-programmed Message
/// Definition: Number related to a pre-programmed message that can be
/// transmitted by a vehicle.
/// Format: One octet fixed length Data Item.
/// Structure:
/// Octet no. 1
/// 8 7 6 5 4 3 2 1
/// TRB MSG
/// Bit-8 (TRB) = 0 Default
/// = 1 In Trouble
/// Bits 7-1 (MSG) = 1 Towing aircraft
/// = 2 "Follow me" operation
/// = 3 Runway check
/// = 4 Emergency operation (fire, medical…)
/// = 5 Work in progress (maintenance, birds
/// scarer, sweepers…)
/// Encoding Rule: This item is optional
/// </summary>
public class AsterixFieldI020Frn018Type310 : AsterixField
{
    public const byte StaticFrn = 18;
    public const string StaticName = "Pre-programmed Message";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;

    private byte _rawValue;

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        _rawValue = buffer[0];
        buffer = buffer[GetByteSize()..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = _rawValue;
        buffer = buffer[GetByteSize()..];
    }

    public override int GetByteSize() => 1;

    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        TrbField,
        MsgField
    ]);

    public override void Accept(IVisitor visitor)
    {
        var trbValue = Trb;
        BoolType.Accept(visitor, TrbField, TrbField.DataType, ref trbValue);
        Trb = trbValue;

        var msgValue = (byte)Msg;
        UInt8Type.Accept(visitor, MsgField, MsgField.DataType, ref msgValue);
        Msg = (PreProgrammedMessageEnum)msgValue;
    }

    private static readonly Field TrbField = new Field.Builder()
        .Name(nameof(Trb))
        .Title("TRB")
        .Description("In Trouble: 0 = Default, 1 = In Trouble")
        .DataType(BoolType.Default)
        .Build();

    /// <summary>
    /// Bit-8 (TRB) - In Trouble flag
    /// 0 = Default
    /// 1 = In Trouble
    /// </summary>
    public bool Trb
    {
        get => (_rawValue & 0x80) != 0;
        set
        {
            if (value)
            {
                _rawValue |= 0x80;
            }
            else
            {
                _rawValue &= 0x7F;
            }
        }
    }

    private static readonly Field MsgField = new Field.Builder()
        .Name(nameof(Msg))
        .Title("MSG")
        .Description("Pre-programmed Message")
        .DataType(UInt8Type.Default)
        .Enum(PreProgrammedMessageEnumMixin.GetPreProgrammedMessageEnum())
        .Build();

    /// <summary>
    /// Bits 7-1 (MSG) - Message type
    /// 1 = Towing aircraft
    /// 2 = "Follow me" operation
    /// 3 = Runway check
    /// 4 = Emergency operation (fire, medical…)
    /// 5 = Work in progress (maintenance, birds scarer, sweepers…)
    /// </summary>
    public PreProgrammedMessageEnum Msg
    {
        get => (PreProgrammedMessageEnum)(_rawValue & 0x7F);
        set => _rawValue = (byte)((_rawValue & 0x80) | ((byte)value & 0x7F));
    }
}

public enum PreProgrammedMessageEnum : byte
{
    None = 0,
    TowingAircraft = 1,
    FollowMeOperation = 2,
    RunwayCheck = 3,
    EmergencyOperation = 4,
    WorkInProgress = 5
}

public static class PreProgrammedMessageEnumMixin
{
    public static IEnumerable<EnumValue<PreProgrammedMessageEnum>> GetPreProgrammedMessageEnum()
    {
        yield return new EnumValue<PreProgrammedMessageEnum>(PreProgrammedMessageEnum.None, "None");
        yield return new EnumValue<PreProgrammedMessageEnum>(PreProgrammedMessageEnum.TowingAircraft, "Towing aircraft");
        yield return new EnumValue<PreProgrammedMessageEnum>(PreProgrammedMessageEnum.FollowMeOperation, "Follow me operation");
        yield return new EnumValue<PreProgrammedMessageEnum>(PreProgrammedMessageEnum.RunwayCheck, "Runway check");
        yield return new EnumValue<PreProgrammedMessageEnum>(PreProgrammedMessageEnum.EmergencyOperation, "Emergency operation (fire, medical…)");
        yield return new EnumValue<PreProgrammedMessageEnum>(PreProgrammedMessageEnum.WorkInProgress, "Work in progress (maintenance, birds scarer, sweepers…)");
    }
}