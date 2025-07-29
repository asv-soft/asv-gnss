using System;
using JetBrains.Annotations;
using Xunit;
using Field = Asv.IO.Field;
using IFieldType = Asv.IO.IFieldType;
using IVisitor = Asv.IO.IVisitor;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn010Type090))]
public class AsterixFieldI020Frn010Type090Test
{
    [Fact]
    public void FlightLevelField_SerializationDeserialization_ShouldWorkCorrectly()
    {
        // Arrange
        var field = new AsterixFieldI020Frn010Type090();
        var testData = new[]
        {
            new { V = false, G = false, FlightLevel = 0.0 },
            new { V = true, G = false, FlightLevel = 25000.0 },
            new { V = false, G = true, FlightLevel = -5000.0 },
            new { V = true, G = true, FlightLevel = 37500.0 }
        };

        foreach (var test in testData)
        {
            // Set properties
            field.V = test.V;
            field.G = test.G;
            field.FlightLevelFt = test.FlightLevel;

            // Act - Serialize
            var buffer = new byte[field.GetByteSize()];
            var span = buffer.AsSpan();
            field.Serialize(ref span);

            // Act - Deserialize
            var deserializedField = new AsterixFieldI020Frn010Type090();
            var readSpan = new ReadOnlySpan<byte>(buffer);
            deserializedField.Deserialize(ref readSpan);

            // Assert
            Assert.Equal(test.V, deserializedField.V);
            Assert.Equal(test.G, deserializedField.G);
            Assert.Equal(test.FlightLevel, deserializedField.FlightLevelFt, 1); // Allow small precision difference
            
            // Test MSL conversion
            var expectedMsl = test.FlightLevel * 0.3048;
            Assert.Equal(expectedMsl, deserializedField.FlightLevelMsl, 3);
        }
    }

    [Fact]
    public void Properties_ShouldHaveCorrectStaticValues()
    {
        // Arrange
        var field = new AsterixFieldI020Frn010Type090();

        // Assert
        Assert.Equal(10, AsterixFieldI020Frn010Type090.StaticFrn);
        Assert.Equal("Flight Level and Vertical Rate", AsterixFieldI020Frn010Type090.StaticName);
        Assert.Equal(AsterixFieldI020Frn010Type090.StaticName, field.Name);
        Assert.Equal(AsterixMessageI020.Category, field.Category);
        Assert.Equal(AsterixFieldI020Frn010Type090.StaticFrn, field.FieldReferenceNumber);
        Assert.Equal(2, field.GetByteSize());
    }

    [Fact]
    public void FlightLevelConversion_ShouldWorkCorrectly()
    {
        // Arrange
        var field = new AsterixFieldI020Frn010Type090();

        // Test feet to MSL conversion
        field.FlightLevelFt = 10000.0;
        Assert.Equal(10000.0 * 0.3048, field.FlightLevelMsl, 3);

        // Test MSL to feet conversion
        field.FlightLevelMsl = 3048.0; // Should be 10000 feet
        Assert.Equal(10000.0, field.FlightLevelFt, 1);
    }

    [Fact]
    public void AcceptVisitor_ShouldNotThrowException()
    {
        // Arrange
        var field = new AsterixFieldI020Frn010Type090
        {
            V = true,
            G = false,
            FlightLevelFt = 25000.0
        };

        // Act & Assert - Should not throw
        field.Accept(new TestVisitor());
    }

    private class TestVisitor : IVisitor
    {
       

        public void VisitUnknown(Field field, IFieldType type)
        {
            
        }
    }
}