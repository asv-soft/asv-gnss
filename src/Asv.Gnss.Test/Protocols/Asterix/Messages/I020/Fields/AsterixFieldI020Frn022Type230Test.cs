using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss;

[TestSubject(typeof(AsterixFieldI020Frn022Type230))]
public class AsterixFieldI020Frn022Type230Test
{
    [Fact]
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

    [Theory]
    [InlineData(false, false, false, 0x00)]
    [InlineData(false, false, true, 0x10)]
    [InlineData(false, true, false, 0x20)]
    [InlineData(false, true, true, 0x30)]
    [InlineData(true, false, false, 0x80)]
    [InlineData(true, false, true, 0x90)]
    [InlineData(true, true, false, 0xA0)]
    [InlineData(true, true, true, 0xB0)]
    public void CapabilityFlags_ShouldUseIndependentSpecifiedBits(bool mssc, bool aic, bool b1A, int expectedBits)
    {
        var field = new AsterixFieldI020Frn022Type230
        {
            Com = CommunicationsCapability.CommAbUplinkDownlinkElm,
            Stat = FlightStatus.AlertSpiAirborneOrGround,
            Mssc = mssc,
            Arc = true,
            Aic = aic,
            B1A = b1A,
            B1B = 0x0A
        };
        var buffer = new byte[field.GetByteSize()];
        var span = buffer.AsSpan();

        field.Serialize(ref span);

        Assert.True(span.IsEmpty);
        Assert.Equal(new byte[] { 0x70, (byte)(expectedBits | 0x4A) }, buffer);
        var decoded = new AsterixFieldI020Frn022Type230();
        var remaining = new ReadOnlySpan<byte>(buffer);
        decoded.Deserialize(ref remaining);
        Assert.True(remaining.IsEmpty);
        Assert.Equal(mssc, decoded.Mssc);
        Assert.Equal(aic, decoded.Aic);
        Assert.Equal(b1A, decoded.B1A);
        Assert.True(decoded.Arc);
        Assert.Equal(0x0A, decoded.B1B);
        Assert.Equal(field.Com, decoded.Com);
        Assert.Equal(field.Stat, decoded.Stat);

        decoded.Mssc = !mssc;
        decoded.Aic = !aic;
        decoded.B1A = !b1A;
        span = buffer.AsSpan();
        decoded.Serialize(ref span);
        Assert.Equal(new byte[] { 0x70, (byte)((expectedBits ^ 0xB0) | 0x4A) }, buffer);
    }

}
