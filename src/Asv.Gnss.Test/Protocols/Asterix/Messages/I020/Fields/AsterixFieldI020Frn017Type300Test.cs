using System;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn017Type300))]
public class AsterixFieldI020Frn017Type300Test
{
    [Fact]
    public void DeserializeSerialize_ShouldPreserveVehicleFleetId()
    {
        // Arrange
        var field = new AsterixFieldI020Frn017Type300();
        var buffer = new byte[] { (byte)VehicleFleetIdEnum.Fire };
        var readBuffer = new ReadOnlySpan<byte>(buffer);
        
        // Act - Deserialize
        field.Deserialize(ref readBuffer);
        
        // Assert - Check deserialized value
        Assert.Equal(VehicleFleetIdEnum.Fire, field.VehicleFleetId);
        
        // Act - Serialize back
        var writeBuffer = new byte[1];
        var writeSpan = new Span<byte>(writeBuffer);
        field.Serialize(ref writeSpan);
        
        // Assert - Check serialized value matches original
        Assert.Equal((byte)VehicleFleetIdEnum.Fire, writeBuffer[0]);
    }

    [Fact]
    public void GetByteSize_ShouldReturnOne()
    {
        // Arrange
        var field = new AsterixFieldI020Frn017Type300();
        
        // Act & Assert
        Assert.Equal(1, field.GetByteSize());
    }

    [Fact]
    public void Properties_ShouldReturnCorrectValues()
    {
        // Arrange
        var field = new AsterixFieldI020Frn017Type300();
        
        // Act & Assert
        Assert.Equal("Vehicle Fleet Identification", field.Name);
        Assert.Equal(20, field.Category);
        Assert.Equal(17, field.FieldReferenceNumber);
    }

    [Theory]
    [InlineData(VehicleFleetIdEnum.Unknown, 0)]
    [InlineData(VehicleFleetIdEnum.AtcEquipmentMaintenance, 1)]
    [InlineData(VehicleFleetIdEnum.AirportMaintenance, 2)]
    [InlineData(VehicleFleetIdEnum.Fire, 3)]
    [InlineData(VehicleFleetIdEnum.BirdScarer, 4)]
    [InlineData(VehicleFleetIdEnum.SnowPlough, 5)]
    [InlineData(VehicleFleetIdEnum.RunwaySweeper, 6)]
    [InlineData(VehicleFleetIdEnum.Emergency, 7)]
    [InlineData(VehicleFleetIdEnum.Police, 8)]
    [InlineData(VehicleFleetIdEnum.Bus, 9)]
    [InlineData(VehicleFleetIdEnum.Tug, 10)]
    [InlineData(VehicleFleetIdEnum.GrassCutter, 11)]
    [InlineData(VehicleFleetIdEnum.Fuel, 12)]
    [InlineData(VehicleFleetIdEnum.Baggage, 13)]
    [InlineData(VehicleFleetIdEnum.Catering, 14)]
    [InlineData(VehicleFleetIdEnum.AircraftMaintenance, 15)]
    [InlineData(VehicleFleetIdEnum.Flyco, 16)]
    public void DeserializeSerialize_AllVehicleFleetTypes_ShouldWork(VehicleFleetIdEnum expectedEnum, byte expectedByte)
    {
        // Arrange
        var field = new AsterixFieldI020Frn017Type300();
        var buffer = new byte[] { expectedByte };
        var readBuffer = new ReadOnlySpan<byte>(buffer);
        
        // Act - Deserialize
        field.Deserialize(ref readBuffer);
        
        // Assert - Check deserialized enum value
        Assert.Equal(expectedEnum, field.VehicleFleetId);
        
        // Act - Serialize back
        var writeBuffer = new byte[1];
        var writeSpan = new Span<byte>(writeBuffer);
        field.Serialize(ref writeSpan);
        
        // Assert - Check serialized byte value
        Assert.Equal(expectedByte, writeBuffer[0]);
    }

    [Fact]
    public void VehicleFleetId_SetProperty_ShouldSerializeCorrectly()
    {
        // Arrange
        var field = new AsterixFieldI020Frn017Type300
        {
            VehicleFleetId = VehicleFleetIdEnum.Police
        };
            
        // Act
        var writeBuffer = new byte[1];
        var writeSpan = new Span<byte>(writeBuffer);
        field.Serialize(ref writeSpan);
        
        // Assert
        Assert.Equal((byte)VehicleFleetIdEnum.Police, writeBuffer[0]);
    }
}