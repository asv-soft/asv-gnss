using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

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
        
        // Act - Deserialize
        var deserializedField = new AsterixFieldI020Frn013Type245();
        var readOnlySpan = new ReadOnlySpan<byte>(buffer);
        deserializedField.Deserialize(ref readOnlySpan);
        
        // Assert
        Assert.Equal(originalSti, deserializedField.Sti);
        Assert.Equal(originalTargetId, deserializedField.TargetIdentification);
        Assert.Equal(7, field.GetByteSize());
        Assert.Equal("Target Identification", field.Name);
        Assert.Equal(13, field.FieldReferenceNumber);
        Assert.Equal(20, field.Category);
    }
}