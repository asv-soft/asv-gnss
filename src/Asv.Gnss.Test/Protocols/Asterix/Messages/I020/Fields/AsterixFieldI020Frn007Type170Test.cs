using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn007Type170))]
public class AsterixFieldI020Frn007Type170Test
{

    [Fact]
    public void SerializeAndDeserialize_ShouldPreserveAllProperties()
    {
        
        // Arrange
        var field = new AsterixFieldI020Frn007Type170
        {
            Cnf = false,    // Confirmed track
            Tre = false,    // Not last report
            Cst = false,    // Not extrapolated
            Cdm = CdmEnum.Descending,  // Invalid climb/descent
            Mah = false,    // No horizontal maneuver
            Sth = false,    // Measured position
        };

        // Create buffer for serialization test
        var buffer = new byte[field.GetByteSize()];
        var writeSpan = new Span<byte>(buffer);
        var readSpan = new ReadOnlySpan<byte>(buffer);

        // Act
        field.Serialize(ref writeSpan);
        
        var deserializedField = new AsterixFieldI020Frn007Type170();
        deserializedField.Deserialize(ref readSpan);

        // Assert
        Assert.False(deserializedField.Cnf);
        Assert.False(deserializedField.Tre);
        Assert.False(deserializedField.Cst);
        Assert.Equal(CdmEnum.Descending, deserializedField.Cdm);
        Assert.False(deserializedField.Mah);
        Assert.False(deserializedField.Sth);
        Assert.Null(deserializedField.Gho);

        // Verify that all properties match between original and deserialized
        Assert.Equal(field.Cnf, deserializedField.Cnf);
        Assert.Equal(field.Tre, deserializedField.Tre);
        Assert.Equal(field.Cst, deserializedField.Cst);
        Assert.Equal(field.Cdm, deserializedField.Cdm);
        Assert.Equal(field.Mah, deserializedField.Mah);
        Assert.Equal(field.Sth, deserializedField.Sth);
        Assert.Equal(field.Gho, deserializedField.Gho);
    }
}