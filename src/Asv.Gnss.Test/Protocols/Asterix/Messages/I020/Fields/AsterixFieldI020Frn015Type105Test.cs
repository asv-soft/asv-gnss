using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn015Type105))]
public class AsterixFieldI020Frn015Type105Test
{

    
    [Fact]
    public void SerializeDeserialize_ValidAltitudeValues_ShouldRoundTripCorrectly()
    {
        // Arrange
        var field = new AsterixFieldI020Frn015Type105();
        var testCases = new[]
        {
            0.0,        // Sea level
            1000.0,     // 1000 feet
            -500.0,     // Below sea level
            35000.0,    // High altitude
            -1000.0,    // Negative altitude
            6.25,       // Minimum resolution
            12.5        // Double minimum resolution
        };

        foreach (var expectedAltitudeFt in testCases)
        {
            // Set altitude in feet
            field.EllipsoidAltitudeFt = expectedAltitudeFt;
        
            // Act - Serialize
            var buffer = new byte[field.GetByteSize()];
            var span = buffer.AsSpan();
            field.Serialize(ref span);
        
            // Act - Deserialize
            var deserializedField = new AsterixFieldI020Frn015Type105();
            var readSpan = new ReadOnlySpan<byte>(buffer);
            deserializedField.Deserialize(ref readSpan);
        
            // Assert
            Assert.Equal(expectedAltitudeFt, deserializedField.EllipsoidAltitudeFt, 1); // 6.25 ft resolution tolerance
        
            // Test meters conversion
            var expectedAltitudeM = expectedAltitudeFt * 0.3048;
            Assert.Equal(expectedAltitudeM, deserializedField.EllipsoidAltitudeM, 3); // Meter precision
        }
    }
}