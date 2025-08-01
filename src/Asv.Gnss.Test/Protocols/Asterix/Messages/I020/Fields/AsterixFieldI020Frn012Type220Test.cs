using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;



namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn012Type220))]
public class AsterixFieldI020Frn012Type220Test
{
    [Fact]
    public void SerializeDeserialize_ShouldPreserveTargetAddress()
    {
        // Arrange
        var field = new AsterixFieldI020Frn012Type220();
        var testValue = 0x24324Fu; // Test value from the integration test (148527)
        field.TargetAddress = testValue;

        // Act - Serialize
        var buffer = new byte[field.GetByteSize()];
        var span = buffer.AsSpan();
        field.Serialize(ref span);

        // Act - Deserialize
        var deserializedField = new AsterixFieldI020Frn012Type220();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        deserializedField.Deserialize(ref readSpan);

        // Assert
        Assert.Equal(testValue, deserializedField.TargetAddress);
        Assert.Equal("Target Address", deserializedField.Name);
        Assert.Equal(12, deserializedField.FieldReferenceNumber);
        Assert.Equal(20, deserializedField.Category);
        Assert.Equal(3, deserializedField.GetByteSize());
    }

    [Fact]
    public void TargetAddress_ShouldHandle24BitValues()
    {
        // Arrange
        var field = new AsterixFieldI020Frn012Type220();

        // Test minimum value (0)
        field.TargetAddress = 0;
        Assert.Equal(0u, field.TargetAddress);

        // Test maximum 24-bit value (0xFFFFFF)
        field.TargetAddress = 0xFFFFFFu;
        Assert.Equal(0xFFFFFFu, field.TargetAddress);

        // Test middle value
        field.TargetAddress = 0x800000u;
        Assert.Equal(0x800000u, field.TargetAddress);
    }

    [Fact]
    public void GetByteSize_ShouldReturn3()
    {
        // Arrange
        var field = new AsterixFieldI020Frn012Type220();

        // Act & Assert
        Assert.Equal(3, field.GetByteSize());
    }
}
