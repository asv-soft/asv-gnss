using System;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn016Type210))]
public class AsterixFieldI020Frn016Type210Test
{
    [Fact]
    public void SerializeDeserialize_ShouldPreserveAccelerationValues()
    {
        // Arrange
        var field = new AsterixFieldI020Frn016Type210();
        var testAxValue = 15.75; // Test value within range
        var testAyValue = -20.5;  // Test negative value
        
        field.Ax = testAxValue;
        field.Ay = testAyValue;
        
        // Act - Serialize
        var buffer = new byte[field.GetByteSize()];
        var span = buffer.AsSpan();
        field.Serialize(ref span);
        
        // Act - Deserialize
        var newField = new AsterixFieldI020Frn016Type210();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        newField.Deserialize(ref readSpan);
        
        // Assert
        Assert.Equal(testAxValue, newField.Ax, 2); // Allow small precision difference
        Assert.Equal(testAyValue, newField.Ay, 2);
        Assert.Equal(2, field.GetByteSize());
        Assert.Equal(16, field.FieldReferenceNumber);
        Assert.Equal("Calculated Acceleration", field.Name);
        Assert.Equal(20, field.Category);
    }
    
    [Fact]
    public void Deserialize_ShouldHandleMaximumValues()
    {
        // Arrange - Maximum positive and negative signed byte values
        var buffer = new byte[] { 0x7F, 0x80 }; // 127, -128 in signed bytes
        var field = new AsterixFieldI020Frn016Type210();
        
        // Act
        var span = new ReadOnlySpan<byte>(buffer);
        field.Deserialize(ref span);
        
        // Assert
        Assert.Equal(127 * 0.25, field.Ax); // 31.75 m/s²
        Assert.Equal(-128 * 0.25, field.Ay); // -32 m/s²
    }
    
    [Fact]
    public void Serialize_ShouldHandleZeroValues()
    {
        // Arrange
        var field = new AsterixFieldI020Frn016Type210();
        field.Ax = 0.0;
        field.Ay = 0.0;
        
        // Act
        var buffer = new byte[field.GetByteSize()];
        var span = buffer.AsSpan();
        field.Serialize(ref span);
        
        // Assert
        Assert.Equal(0, buffer[0]);
        Assert.Equal(0, buffer[1]);
    }
}