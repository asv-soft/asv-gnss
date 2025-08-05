using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn014Type110))]
public class AsterixFieldI020Frn014Type110Test
{

    [Fact]
    public void SerializeDeserialize_ShouldPreserveValues()
    {
        // Arrange
        var field = new AsterixFieldI020Frn014Type110();
        const double expectedAltitudeFt = 12500.0; // Test value in feet
        const double expectedAltitudeM = expectedAltitudeFt * 0.3048; // Expected value in meters
        
        field.LocalCartesianAltitudeFt = expectedAltitudeFt;
        
        // Act - Serialize
        var buffer = new byte[field.GetByteSize()];
        var span = buffer.AsSpan();
        field.Serialize(ref span);
        
        // Act - Deserialize
        var deserializedField = new AsterixFieldI020Frn014Type110();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        deserializedField.Deserialize(ref readSpan);
        
        // Assert
        Assert.Equal(expectedAltitudeFt, deserializedField.LocalCartesianAltitudeFt, 2); // Allow small precision difference due to scaling
        Assert.Equal(expectedAltitudeM, deserializedField.LocalCartesianAltitudeM, 2); // Test meter conversion
        Assert.Equal(2, field.GetByteSize()); // Verify expected byte size
        Assert.Equal(14, field.FieldReferenceNumber); // Verify FRN
        Assert.Equal(20, field.Category); // Verify category
        Assert.Equal("Measured Height (Cartesian Coordinates)", field.Name); // Verify name
    }
}