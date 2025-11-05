using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn022Type230))]
public class AsterixFieldI020Frn022Type230Test
{
    //[Fact]
    public void SerializeDeserialize_ShouldPreserveAllFieldValues()
    {
        // Arrange
        var field = new AsterixFieldI020Frn022Type230();
    
        // Set test values for all properties
        field.Com = CommunicationsCapability.CommAbUplinkDownlinkElm;
        field.Stat = FlightStatus.AlertSpiAirborneOrGround;
        field.Mssc = true;
        field.Arc = false;
        field.Aic = true;
        field.B1A = false;
        field.B1B = 0x0A; // Test with value 10
    
        // Create buffer for serialization
        var buffer = new byte[field.GetByteSize()];
        var span = buffer.AsSpan();
    
        // Act - Serialize
        field.Serialize(ref span);
    
        // Create new field for deserialization
        var deserializedField = new AsterixFieldI020Frn022Type230();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        deserializedField.Deserialize(ref readSpan);
    
        // Assert - Verify all properties are preserved
        Assert.Equal(CommunicationsCapability.CommAbUplinkDownlinkElm, deserializedField.Com);
        Assert.Equal(FlightStatus.AlertSpiAirborneOrGround, deserializedField.Stat);
        Assert.True(deserializedField.Mssc);
        Assert.False(deserializedField.Arc);
        Assert.True(deserializedField.Aic);
        Assert.False(deserializedField.B1A);
        Assert.Equal(0x0A, deserializedField.B1B);
    
        // Verify field metadata
        Assert.Equal("Comms/ACAS Capability and Flight Status", deserializedField.Name);
        Assert.Equal(22, deserializedField.FieldReferenceNumber);
        Assert.Equal(20, deserializedField.Category);
        Assert.Equal(2, deserializedField.GetByteSize());
    }

}