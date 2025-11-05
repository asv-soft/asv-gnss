using System;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn026Type050))]
public class AsterixFieldI020Frn026Type050Test
{
    [Fact]
    public void SerializeDeserialize_ShouldPreserveAllFieldValues()
    {
        // Arrange
        var field = new AsterixFieldI020Frn026Type050();
        
        // Set test values
        field.Validated = true;
        field.Garbled = false;
        field.LocalTracker = true;
        field.Mode2Code = 0x567; // 12-bit value
        
        var buffer = new byte[field.GetByteSize()];
        var writeSpan = buffer.AsSpan();
        var readSpan = buffer.AsSpan();
        
        // Act - Serialize
        field.Serialize(ref writeSpan);
        
        // Create new field for deserialization
        var deserializedField = new AsterixFieldI020Frn026Type050();
        var readOnlySpan = new ReadOnlySpan<byte>(buffer);
        
        // Act - Deserialize
        deserializedField.Deserialize(ref readOnlySpan);
        
        // Assert
        Assert.Equal(field.Validated, deserializedField.Validated);
        Assert.Equal(field.Garbled, deserializedField.Garbled);
        Assert.Equal(field.LocalTracker, deserializedField.LocalTracker);
        Assert.Equal(field.Mode2Code, deserializedField.Mode2Code);
        Assert.Equal(field.Mode2CodeOctal, deserializedField.Mode2CodeOctal);
    }
    
    [Fact]
    public void Mode2CodeOctal_ShouldConvertCorrectly()
    {
        // Arrange
        var field = new AsterixFieldI020Frn026Type050();
        
        // Test various octal codes
        var testCases = new[]
        {
            new { Binary = (ushort)0x000, Octal = "0000" },
            new { Binary = (ushort)0x249, Octal = "1111" }, // 001 001 001 001
            new { Binary = (ushort)0x492, Octal = "2222" }, // 010 010 010 010
            new { Binary = (ushort)0xFFF, Octal = "7777" }  // 111 111 111 111
        };
        
        foreach (var testCase in testCases)
        {
            // Act
            field.Mode2Code = testCase.Binary;
            
            // Assert
            Assert.Equal(testCase.Octal, field.Mode2CodeOctal);
        }
    }
    
    [Fact]
    public void SetMode2CodeFromOctal_ShouldSetCorrectBinaryValue()
    {
        // Arrange
        var field = new AsterixFieldI020Frn026Type050();
        
        // Act & Assert
        field.SetMode2CodeFromOctal("1234");
        Assert.Equal("1234", field.Mode2CodeOctal);
        
        field.SetMode2CodeFromOctal("7777");
        Assert.Equal("7777", field.Mode2CodeOctal);
        
        field.SetMode2CodeFromOctal("0000");
        Assert.Equal("0000", field.Mode2CodeOctal);
    }
    
    [Fact]
    public void SetMode2CodeFromOctal_WithInvalidInput_ShouldThrowException()
    {
        // Arrange
        var field = new AsterixFieldI020Frn026Type050();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => field.SetMode2CodeFromOctal(""));
        Assert.Throws<ArgumentException>(() => field.SetMode2CodeFromOctal("123"));
        Assert.Throws<ArgumentException>(() => field.SetMode2CodeFromOctal("12345"));
        Assert.Throws<ArgumentException>(() => field.SetMode2CodeFromOctal(null));
    }
    
    [Fact]
    public void ValidatedProperty_ShouldUseInvertedLogic()
    {
        // Arrange
        var field = new AsterixFieldI020Frn026Type050();
        
        // Test inverted logic: 0 = validated, 1 = not validated
        field.Validated = true; // Should clear bit 15
        var buffer = new byte[2];
        var span = buffer.AsSpan();
        field.Serialize(ref span);
        
        // Bit 15 should be 0 for validated
        Assert.Equal(0, buffer[0] & 0x80);
        
        field.Validated = false; // Should set bit 15
        span = buffer.AsSpan();
        field.Serialize(ref span);
        
        // Bit 15 should be 1 for not validated
        Assert.Equal(0x80, buffer[0] & 0x80);
    }
    
    [Fact]
    public void GetByteSize_ShouldReturnTwo()
    {
        // Arrange
        var field = new AsterixFieldI020Frn026Type050();
        
        // Act & Assert
        Assert.Equal(2, field.GetByteSize());
    }
    
    [Fact]
    public void StaticProperties_ShouldHaveCorrectValues()
    {
        // Arrange
        var field = new AsterixFieldI020Frn026Type050();
        
        // Act & Assert
        Assert.Equal(26, field.FieldReferenceNumber);
        Assert.Equal(26, AsterixFieldI020Frn026Type050.StaticFrn);
        Assert.Equal("Mode-2 Code in Octal Representation", field.Name);
        Assert.Equal(20, field.Category);
    }
    
    [Fact]
    public void BitManipulation_ShouldWorkCorrectly()
    {
        // Arrange
        var field = new AsterixFieldI020Frn026Type050();
        
        // Test individual bit flags
        field.Validated = false;
        field.Garbled = true;
        field.LocalTracker = false;
        field.Mode2Code = 0x123;
        
        // Act - serialize and deserialize
        var buffer = new byte[2];
        var writeSpan = buffer.AsSpan();
        field.Serialize(ref writeSpan);
        
        var newField = new AsterixFieldI020Frn026Type050();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        newField.Deserialize(ref readSpan);
        
        // Assert
        Assert.False(newField.Validated);
        Assert.True(newField.Garbled);
        Assert.False(newField.LocalTracker);
        Assert.Equal((ushort)0x123, newField.Mode2Code);
    }
}