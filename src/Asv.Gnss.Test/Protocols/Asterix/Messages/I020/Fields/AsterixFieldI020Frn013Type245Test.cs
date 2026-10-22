using System;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss;

[TestSubject(typeof(AsterixFieldI020Frn013Type245))]
public class AsterixFieldI020Frn013Type245Test
{

    [Fact]
    public void Serialize_Deserialize_ShouldPreserveData()
    {
        // Arrange
        var field = new AsterixFieldI020Frn013Type245();
        var originalSti = StiEnum.CallsignDownlinked;
        var originalTargetId = "ABC123  ";
        
        field.Sti = originalSti;
        field.TargetIdentification = originalTargetId;
        
        // Act - Serialize
        var buffer = new byte[field.GetByteSize()];
        var span = buffer.AsSpan();
        field.Serialize(ref span);
        Assert.True(span.IsEmpty);
        
        // Act - Deserialize
        var deserializedField = new AsterixFieldI020Frn013Type245();
        var readOnlySpan = new ReadOnlySpan<byte>(buffer);
        deserializedField.Deserialize(ref readOnlySpan);
        Assert.True(readOnlySpan.IsEmpty);
        
        // Assert
        Assert.Equal(originalSti, deserializedField.Sti);
        Assert.Equal(originalTargetId, deserializedField.TargetIdentification);
        Assert.Equal(7, field.GetByteSize());
        Assert.Equal("Target Identification", field.Name);
        Assert.Equal(13, field.FieldReferenceNumber);
        Assert.Equal(20, field.Category);
    }

    [Fact]
    public void Serialize_ShouldPreserveFollowingFieldBytes()
    {
        var field = new AsterixFieldI020Frn013Type245
        {
            Sti = StiEnum.CallsignDownlinked,
            TargetIdentification = "ABC123  "
        };
        var buffer = new byte[field.GetByteSize() + 2];
        buffer[^2] = 0xA5;
        buffer[^1] = 0x5A;
        var span = buffer.AsSpan();

        field.Serialize(ref span);

        Assert.Equal(new byte[] { 0xA5, 0x5A }, span.ToArray());
        var remaining = new ReadOnlySpan<byte>(buffer);
        var decoded = new AsterixFieldI020Frn013Type245();
        decoded.Deserialize(ref remaining);
        Assert.Equal(field.TargetIdentification, decoded.TargetIdentification);
        Assert.Equal(field.Sti, decoded.Sti);
        Assert.Equal(new byte[] { 0xA5, 0x5A }, remaining.ToArray());
    }
}
