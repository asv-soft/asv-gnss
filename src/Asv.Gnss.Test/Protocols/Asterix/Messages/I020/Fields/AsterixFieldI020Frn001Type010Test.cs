using System;
using Asv.Gnss;
using Asv.IO;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn001Type010))]
public class AsterixFieldI020Frn001Type010Test
{

    [Fact]
    public void SerializeAndDeserialize_ShouldPreserveData()
    {
        // Arrange
        var field = new AsterixFieldI020Frn001Type010
        {
            Sac = SystemAreaCode.Greece,
            Sic = 42
        };
    
        // Act
        var buffer = new byte[field.GetByteSize()];
        var writeSpan = new Span<byte>(buffer);
        field.Serialize(ref writeSpan);
    
        var readField = new AsterixFieldI020Frn001Type010();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        readField.Deserialize(ref readSpan);
    
        // Assert
        Assert.Equal(SystemAreaCode.Greece, readField.Sac);
        Assert.Equal(42, readField.Sic);
        Assert.Equal(2, field.GetByteSize());
        Assert.Equal(1, field.FieldReferenceNumber);
        Assert.Equal(20, field.Category);
        Assert.Equal("Data Source Identifier", field.Name);
    }
    
    [Theory]
    [InlineData(SystemAreaCode.Greece, 42)]
    [InlineData(SystemAreaCode.LocalAirport, 0)]
    [InlineData(SystemAreaCode.Israel, 255)]
    [InlineData(SystemAreaCode.NATO, 127)]
    public void SerializeAndDeserialize_VariousValues_ShouldWork(SystemAreaCode sac, byte sic)
    {
        // Arrange
        var field = new AsterixFieldI020Frn001Type010
        {
            Sac = sac,
            Sic = sic
        };

        // Act
        var buffer = new byte[field.GetByteSize()];
        var writeSpan = new Span<byte>(buffer);
        field.Serialize(ref writeSpan);

        var readField = new AsterixFieldI020Frn001Type010();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        readField.Deserialize(ref readSpan);

        // Assert
        Assert.Equal(sac, readField.Sac);
        Assert.Equal(sic, readField.Sic);
    }

}