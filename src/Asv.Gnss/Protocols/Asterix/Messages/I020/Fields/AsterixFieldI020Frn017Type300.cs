using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Data Item I020/300, Vehicle Fleet Identification
/// Definition: Vehicle fleet identification number.
/// Format: One octet fixed length Data Item.
/// Structure:
/// Octet no. 1
/// 8 7 6 5 4 3 2 1
/// VFI
/// Bits 8-1 (VFI) = 0 Unknown
/// = 1 ATC equipment maintenance
/// = 2 Airport maintenance
/// = 3 Fire
/// = 4 Bird scarer
/// = 5 Snow plough
/// = 6 Runway sweeper
/// = 7 Emergency
/// = 8 Police
/// = 9 Bus
/// = 10 Tug (push/tow)
/// = 11 Grass cutter
/// = 12 Fuel
/// = 13 Baggage
/// = 14 Catering
/// = 15 Aircraft maintenance
/// = 16 Flyco (follow me)
/// Encoding Rule: This item is optional
/// </summary>
public class AsterixFieldI020Frn017Type300 : AsterixField
{
    public const byte StaticFrn = 17;
    public const string StaticName = "Vehicle Fleet Identification";
    
    public override string Name => StaticName;
    public override int Category => AsterixMessageI020.Category;
    public override byte FieldReferenceNumber => StaticFrn;

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        VehicleFleetId = (VehicleFleetIdEnum)buffer[0];
        buffer = buffer[GetByteSize()..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)VehicleFleetId;
        buffer = buffer[GetByteSize()..];
    }

    public override int GetByteSize() => 1;

    private static StructType? type;
    public static StructType StructType => type ??= new StructType([
        VehicleFleetIdField
    ]);

    public override void Accept(IVisitor visitor)
    {
        var temp = (byte)VehicleFleetId;
        UInt8Type.Accept(visitor, VehicleFleetIdField, VehicleFleetIdField.DataType, ref temp);
        VehicleFleetId = (VehicleFleetIdEnum)temp;
    }

    private static readonly Field VehicleFleetIdField = new Field.Builder()
        .Name(nameof(VehicleFleetId))
        .Title("VFI")
        .Description("Vehicle Fleet Identification")
        .DataType(UInt8Type.Default)
        .Enum(VehicleFleetIdEnumMixin.GetVehicleFleetIdEnum())
        .Build();

    public VehicleFleetIdEnum VehicleFleetId { get; set; }
}

public enum VehicleFleetIdEnum : byte
{
    Unknown = 0,
    AtcEquipmentMaintenance = 1,
    AirportMaintenance = 2,
    Fire = 3,
    BirdScarer = 4,
    SnowPlough = 5,
    RunwaySweeper = 6,
    Emergency = 7,
    Police = 8,
    Bus = 9,
    Tug = 10,
    GrassCutter = 11,
    Fuel = 12,
    Baggage = 13,
    Catering = 14,
    AircraftMaintenance = 15,
    Flyco = 16
}

public static class VehicleFleetIdEnumMixin
{
    public static IEnumerable<EnumValue<VehicleFleetIdEnum>> GetVehicleFleetIdEnum()
    {
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.Unknown, "Unknown");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.AtcEquipmentMaintenance, "ATC equipment maintenance");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.AirportMaintenance, "Airport maintenance");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.Fire, "Fire");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.BirdScarer, "Bird scarer");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.SnowPlough, "Snow plough");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.RunwaySweeper, "Runway sweeper");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.Emergency, "Emergency");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.Police, "Police");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.Bus, "Bus");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.Tug, "Tug (push/tow)");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.GrassCutter, "Grass cutter");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.Fuel, "Fuel");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.Baggage, "Baggage");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.Catering, "Catering");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.AircraftMaintenance, "Aircraft maintenance");
        yield return new EnumValue<VehicleFleetIdEnum>(VehicleFleetIdEnum.Flyco, "Flyco (follow me)");
    }
}