using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn005Type042))]
public class AsterixFieldI020Frn005Type042Test
{

    [Fact]
    public void SerializeAndDeserialize_ShouldPreserveData()
    {
        // Arrange
        var original = new AsterixFieldI020Frn005Type042
        {
            X = 173529.5,
            Y = 45109.0
        };

        // Act
        var buffer = new byte[original.GetByteSize()];
        var writeSpan = new Span<byte>(buffer);
        original.Serialize(ref writeSpan);

        var deserialized = new AsterixFieldI020Frn005Type042();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        deserialized.Deserialize(ref readSpan);

        // Assert
        Assert.Equal(6, original.GetByteSize());
        Assert.Equal(original.X, deserialized.X);
        Assert.Equal(original.Y, deserialized.Y);
        Assert.Equal(original.Category, AsterixMessageI020.Category);
        Assert.Equal(AsterixFieldI020Frn005Type042.StaticFrn, original.FieldReferenceNumber);
        Assert.Equal("Position in Cartesian Coordinates", original.Name);
    }
}