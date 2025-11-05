using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn004Type041))]
public class AsterixFieldI020Frn004Type041Test
{

    [Fact]
    public void SerializeAndDeserialize_ShouldWorkCorrectly()
    {
        // Arrange
        var field = new AsterixFieldI020Frn004Type041
        {
            Latitude = 47.88232132925,
            Longitude = 16.32056296698
        };
        
        // Act
        var size = field.GetByteSize();
        var buffer = new byte[size];
        var writeSpan = new Span<byte>(buffer);
        field.Serialize(ref writeSpan);
        
        var field2 = new AsterixFieldI020Frn004Type041();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        field2.Deserialize(ref readSpan);

        // Assert
        Assert.Equal(8, size);  // 4 bytes for latitude + 4 bytes for longitude
        Assert.Equal(field.Latitude, field2.Latitude, 5);  
        Assert.Equal(field.Longitude, field2.Longitude, 5); 
    }
}