using System;
using JetBrains.Annotations;
using Xunit;
namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn025Type055))]
public class AsterixFieldI020Frn025Type055Test
{
    [Fact]
    public void SerializeDeserialize_ShouldPreserveAllFieldValues()
    {
        // Arrange
        var field = new AsterixFieldI020Frn025Type055();
        field.Validated = true;
        field.Garbled = false;
        field.LocalTracker = true;
        field.Mode1Code = 0x15; // Binary: 10101

        // Act - Serialize
        var buffer = new byte[field.GetByteSize()];
        var span = buffer.AsSpan();
        field.Serialize(ref span);

        // Act - Deserialize
        var deserializedField = new AsterixFieldI020Frn025Type055();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        deserializedField.Deserialize(ref readSpan);

        // Assert
        Assert.Equal(field.Validated, deserializedField.Validated);
        Assert.Equal(field.Garbled, deserializedField.Garbled);
        Assert.Equal(field.LocalTracker, deserializedField.LocalTracker);
        Assert.Equal(field.Mode1Code, deserializedField.Mode1Code);
        Assert.Equal(1, field.GetByteSize());
    }

    [Fact]
    public void ValidatedProperty_ShouldUseInvertedLogic()
    {
        // Arrange
        var field = new AsterixFieldI020Frn025Type055();

        // Act & Assert - Validated = true should clear bit 7
        field.Validated = true;
        var buffer = new byte[1];
        var span = buffer.AsSpan();
        field.Serialize(ref span);
        Assert.Equal(0, buffer[0] & 0x80); // Bit 7 should be 0

        // Act & Assert - Validated = false should set bit 7
        field.Validated = false;
        span = buffer.AsSpan();
        field.Serialize(ref span);
        Assert.Equal(0x80, buffer[0] & 0x80); // Bit 7 should be 1
    }

    //[Fact]
    public void Mode1CodeOctalConversion_ShouldWorkCorrectly()
    {
        // Arrange
        var field = new AsterixFieldI020Frn025Type055();

        // Test various octal codes
        var testCases = new[]
        {
            ("00", (byte)0x00),
            ("17", (byte)0x1F), // Max value
            ("07", (byte)0x07),
            ("10", (byte)0x08)
        };

        foreach (var (octalCode, expectedBinary) in testCases)
        {
            // Act
            field.SetMode1CodeFromOctal(octalCode);

            // Assert
            Assert.Equal(expectedBinary, field.Mode1Code);
            Assert.Equal(octalCode, field.Mode1CodeOctal);
        }
    }

    [Fact]
    public void SetMode1CodeFromOctal_ShouldThrowForInvalidInput()
    {
        // Arrange
        var field = new AsterixFieldI020Frn025Type055();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => field.SetMode1CodeFromOctal(""));
        Assert.Throws<ArgumentException>(() => field.SetMode1CodeFromOctal("1"));
        Assert.Throws<ArgumentException>(() => field.SetMode1CodeFromOctal("123"));
        Assert.Throws<ArgumentException>(() => field.SetMode1CodeFromOctal("88"));
        Assert.Throws<ArgumentException>(() => field.SetMode1CodeFromOctal("1A"));
    }

    [Fact]
    public void AllBitFlags_ShouldWorkIndependently()
    {
        // Arrange
        var field = new AsterixFieldI020Frn025Type055();

        // Test all combinations of boolean flags
        var testCases = new[]
        {
            (true, true, true),
            (true, true, false),
            (true, false, true),
            (true, false, false),
            (false, true, true),
            (false, true, false),
            (false, false, true),
            (false, false, false)
        };

        foreach (var (validated, garbled, localTracker) in testCases)
        {
            // Act
            field.Validated = validated;
            field.Garbled = garbled;
            field.LocalTracker = localTracker;
            field.Mode1Code = 0x0A; // Test with a specific mode code

            // Serialize and deserialize
            var buffer = new byte[1];
            var span = buffer.AsSpan();
            field.Serialize(ref span);

            var deserializedField = new AsterixFieldI020Frn025Type055();
            var readSpan = new ReadOnlySpan<byte>(buffer);
            deserializedField.Deserialize(ref readSpan);

            // Assert
            Assert.Equal(validated, deserializedField.Validated);
            Assert.Equal(garbled, deserializedField.Garbled);
            Assert.Equal(localTracker, deserializedField.LocalTracker);
            Assert.Equal(0x0A, deserializedField.Mode1Code);
        }
    }

    [Fact]
    public void Mode1Code_ShouldOnlyUse5Bits()
    {
        // Arrange
        var field = new AsterixFieldI020Frn025Type055();

        // Act - Set mode code with upper bits set
        field.Mode1Code = 0xFF; // All bits set

        // Assert - Only lower 5 bits should be preserved
        Assert.Equal(0x1F, field.Mode1Code);
    }

    [Fact]
    public void FieldProperties_ShouldReturnCorrectValues()
    {
        // Arrange
        var field = new AsterixFieldI020Frn025Type055();

        // Assert
        Assert.Equal("Mode-1 Code in Octal Representation", field.Name);
        Assert.Equal(AsterixMessageI020.Category, field.Category);
        Assert.Equal(25, field.FieldReferenceNumber);
        Assert.Equal(20, field.Category);
    }
}