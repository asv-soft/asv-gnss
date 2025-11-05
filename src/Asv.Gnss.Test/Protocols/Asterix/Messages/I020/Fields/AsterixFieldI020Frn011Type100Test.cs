using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn011Type100))]
public class AsterixFieldI020Frn011Type100Test
{
    [Fact]
    public void SerializeDeserialize_ShouldPreserveAllProperties()
    {
        // Arrange
        var original = new AsterixFieldI020Frn011Type100
        {
            V = true,
            G = false,
            Squawk = 0x1234,
            QSquawk = 0x5678
        };

        // Act - Serialize
        var buffer = new byte[original.GetByteSize()];
        var span = buffer.AsSpan();
        original.Serialize(ref span);

        // Act - Deserialize
        var deserialized = new AsterixFieldI020Frn011Type100();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        deserialized.Deserialize(ref readSpan);

        // Assert
        Assert.Equal(original.V, deserialized.V);
        Assert.Equal(original.G, deserialized.G);
        Assert.Equal(original.Squawk, deserialized.Squawk);
        Assert.Equal(original.QSquawk, deserialized.QSquawk);
        Assert.Equal(4, original.GetByteSize());
        Assert.Equal("Mode-C Code", original.Name);
        Assert.Equal(11, original.FieldReferenceNumber);
        Assert.Equal(20, original.Category);
    }
}