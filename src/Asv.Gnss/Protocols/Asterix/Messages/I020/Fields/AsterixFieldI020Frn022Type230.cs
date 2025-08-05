using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixFieldI020Frn022Type230 : AsterixField
{
    public const byte StaticFrn = 22;
    private const string StaticName = "Comms/ACAS Capability and Flight Status";
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;

    private ushort _rawValue;

    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        ComField,
        StatField,
        MsscField,
        ArcField,
        AicField,
        B1AField,
        B1BField
    ]);

    private static readonly Field ComField = new Field.Builder()
        .Name(nameof(Com))
        .Title("COM")
        .Description("Communications capability of the transponder")
        .DataType(UInt8Type.Default)
        .Enum(CommunicationsCapabilityMixin.GetCommunicationsCapabilityEnum())
        .Build();

    /// <summary>
    /// Communications capability of the transponder (bits 16-14)
    /// </summary>
    public CommunicationsCapability Com
    {
        get => (CommunicationsCapability)((_rawValue >> 13) & 0x07);
        set => _rawValue = (ushort)((_rawValue & 0x1FFF) | ((byte)value << 13));
    }

    private static readonly Field StatField = new Field.Builder()
        .Name(nameof(Stat))
        .Title("STAT")
        .Description("Flight Status")
        .DataType(UInt8Type.Default)
        .Enum(FlightStatusMixin.GetFlightStatusEnum())
        .Build();

    /// <summary>
    /// Flight Status (bits 13-11)
    /// </summary>
    public FlightStatus Stat
    {
        get => (FlightStatus)((_rawValue >> 10) & 0x07);
        set => _rawValue = (ushort)((_rawValue & 0xE3FF) | ((byte)value << 10));
    }

    private static readonly Field MsscField = new Field.Builder()
        .Name(nameof(Mssc))
        .Title("MSSC")
        .Description("Mode-S Specific Service Capability")
        .DataType(BoolType.Default)
        .Build();

    /// <summary>
    /// Mode-S Specific Service Capability (bit 8)
    /// false = No, true = Yes
    /// </summary>
    public bool Mssc
    {
        get => (_rawValue & 0x0100) != 0;
        set
        {
            if (value)
                _rawValue |= 0x0100;
            else
                _rawValue &= 0xFEFF;
        }
    }

    private static readonly Field ArcField = new Field.Builder()
        .Name(nameof(Arc))
        .Title("ARC")
        .Description("Altitude reporting capability")
        .DataType(BoolType.Default)
        .Build();

    /// <summary>
    /// Altitude reporting capability (bit 7)
    /// false = 100 ft resolution, true = 25 ft resolution
    /// </summary>
    public bool Arc
    {
        get => (_rawValue & 0x0040) != 0;
        set
        {
            if (value)
                _rawValue |= 0x0040;
            else
                _rawValue &= 0xFFBF;
        }
    }

    private static readonly Field AicField = new Field.Builder()
        .Name(nameof(Aic))
        .Title("AIC")
        .Description("Aircraft identification capability")
        .DataType(BoolType.Default)
        .Build();

    /// <summary>
    /// Aircraft identification capability (bit 6)
    /// false = No, true = Yes
    /// </summary>
    public bool Aic
    {
        get => (_rawValue & 0x0020) != 0;
        set
        {
            if (value)
                _rawValue |= 0x0020;
            else
                _rawValue &= 0xFFDF;
        }
    }

    private static readonly Field B1AField = new Field.Builder()
        .Name(nameof(B1A))
        .Title("B1A")
        .Description("BDS 1,0 bit 16")
        .DataType(BoolType.Default)
        .Build();

    /// <summary>
    /// BDS 1,0 bit 16 (bit 5)
    /// </summary>
    public bool B1A
    {
        get => (_rawValue & 0x0020) != 0;
        set
        {
            if (value)
                _rawValue |= 0x0020;
            else
                _rawValue &= 0xFFDF;
        }
    }

    private static readonly Field B1BField = new Field.Builder()
        .Name(nameof(B1B))
        .Title("B1B")
        .Description("BDS 1,0 bits 37/40")
        .DataType(UInt8Type.Default)
        .Build();

    /// <summary>
    /// BDS 1,0 bits 37/40 (bits 4-1)
    /// </summary>
    public byte B1B
    {
        get => (byte)(_rawValue & 0x0F);
        set => _rawValue = (ushort)((_rawValue & 0xFFF0) | (value & 0x0F));
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
        var comValue = (byte)Com;
        UInt8Type.Accept(visitor, ComField, ComField.DataType, ref comValue);
        Com = (CommunicationsCapability)comValue;

        var statValue = (byte)Stat;
        UInt8Type.Accept(visitor, StatField, StatField.DataType, ref statValue);
        Stat = (FlightStatus)statValue;

        var msscValue = Mssc;
        BoolType.Accept(visitor, MsscField, MsscField.DataType, ref msscValue);
        Mssc = msscValue;

        var arcValue = Arc;
        BoolType.Accept(visitor, ArcField, ArcField.DataType, ref arcValue);
        Arc = arcValue;

        var aicValue = Aic;
        BoolType.Accept(visitor, AicField, AicField.DataType, ref aicValue);
        Aic = aicValue;

        var b1AValue = B1A;
        BoolType.Accept(visitor, B1AField, B1AField.DataType, ref b1AValue);
        B1A = b1AValue;

        var b1BValue = B1B;
        UInt8Type.Accept(visitor, B1BField, B1BField.DataType, ref b1BValue);
        B1B = b1BValue;
    }
}

/// <summary>
/// Communications capability of the transponder
/// </summary>
public enum CommunicationsCapability : byte
{
    /// <summary>
    /// No communications capability (surveillance only)
    /// </summary>
    NoCapability = 0,
    /// <summary>
    /// Comm. A and Comm. B capability
    /// </summary>
    CommAAndB = 1,
    /// <summary>
    /// Comm. A, Comm. B and Uplink ELM
    /// </summary>
    CommAbAndUplinkElm = 2,
    /// <summary>
    /// Comm. A, Comm. B, Uplink ELM and Downlink ELM
    /// </summary>
    CommAbUplinkDownlinkElm = 3,
    /// <summary>
    /// Level 5 Transponder capability
    /// </summary>
    Level5Transponder = 4,
    /// <summary>
    /// Not assigned (5-7)
    /// </summary>
    NotAssigned5 = 5,
    NotAssigned6 = 6,
    NotAssigned7 = 7
}

/// <summary>
/// Flight Status
/// </summary>
public enum FlightStatus : byte
{
    /// <summary>
    /// No alert, no SPI, aircraft airborne
    /// </summary>
    NoAlertNoSpiAirborne = 0,
    /// <summary>
    /// No alert, no SPI, aircraft on ground
    /// </summary>
    NoAlertNoSpiOnGround = 1,
    /// <summary>
    /// Alert, no SPI, aircraft airborne
    /// </summary>
    AlertNoSpiAirborne = 2,
    /// <summary>
    /// Alert, no SPI, aircraft on ground
    /// </summary>
    AlertNoSpiOnGround = 3,
    /// <summary>
    /// Alert, SPI, aircraft airborne or on ground
    /// </summary>
    AlertSpiAirborneOrGround = 4,
    /// <summary>
    /// No alert, SPI, aircraft airborne or on ground
    /// </summary>
    NoAlertSpiAirborneOrGround = 5,
    /// <summary>
    /// Not assigned
    /// </summary>
    NotAssigned = 6,
    /// <summary>
    /// Information not yet extracted
    /// </summary>
    InfoNotExtracted = 7
}

public static class CommunicationsCapabilityMixin
{
    public static IEnumerable<EnumValue<CommunicationsCapability>> GetCommunicationsCapabilityEnum()
    {
        yield return new EnumValue<CommunicationsCapability>(CommunicationsCapability.NoCapability, "No communications capability (surveillance only)");
        yield return new EnumValue<CommunicationsCapability>(CommunicationsCapability.CommAAndB, "Comm. A and Comm. B capability");
        yield return new EnumValue<CommunicationsCapability>(CommunicationsCapability.CommAbAndUplinkElm, "Comm. A, Comm. B and Uplink ELM");
        yield return new EnumValue<CommunicationsCapability>(CommunicationsCapability.CommAbUplinkDownlinkElm, "Comm. A, Comm. B, Uplink ELM and Downlink ELM");
        yield return new EnumValue<CommunicationsCapability>(CommunicationsCapability.Level5Transponder, "Level 5 Transponder capability");
        yield return new EnumValue<CommunicationsCapability>(CommunicationsCapability.NotAssigned5, "Not assigned");
        yield return new EnumValue<CommunicationsCapability>(CommunicationsCapability.NotAssigned6, "Not assigned");
        yield return new EnumValue<CommunicationsCapability>(CommunicationsCapability.NotAssigned7, "Not assigned");
    }
}

public static class FlightStatusMixin
{
    public static IEnumerable<EnumValue<FlightStatus>> GetFlightStatusEnum()
    {
        yield return new EnumValue<FlightStatus>(FlightStatus.NoAlertNoSpiAirborne, "No alert, no SPI, aircraft airborne");
        yield return new EnumValue<FlightStatus>(FlightStatus.NoAlertNoSpiOnGround, "No alert, no SPI, aircraft on ground");
        yield return new EnumValue<FlightStatus>(FlightStatus.AlertNoSpiAirborne, "Alert, no SPI, aircraft airborne");
        yield return new EnumValue<FlightStatus>(FlightStatus.AlertNoSpiOnGround, "Alert, no SPI, aircraft on ground");
        yield return new EnumValue<FlightStatus>(FlightStatus.AlertSpiAirborneOrGround, "Alert, SPI, aircraft airborne or on ground");
        yield return new EnumValue<FlightStatus>(FlightStatus.NoAlertSpiAirborneOrGround, "No alert, SPI, aircraft airborne or on ground");
        yield return new EnumValue<FlightStatus>(FlightStatus.NotAssigned, "Not assigned");
        yield return new EnumValue<FlightStatus>(FlightStatus.InfoNotExtracted, "Information not yet extracted");
    }
}