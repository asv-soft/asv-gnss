using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn024Type030))]
public class AsterixFieldI020Frn024Type030Test
{

    [Fact]
    public void Should_SerializeDeserialize_WarningErrorConditions_Correctly()
    {
        // Arrange
        var field = new AsterixFieldI020Frn024Type030();
        
        // Add some warning/error conditions
        field.AddWarningErrorCondition(WarningErrorCode.MultipathReply);
        field.AddWarningErrorCondition(WarningErrorCode.SplitPlot);
        field.AddWarningErrorCondition(WarningErrorCode.PhantomSSRPlot);
        field.AddWarningErrorCondition(WarningErrorCode.TransponderAnomaly);
        
        // Act - Serialize
        var buffer = new byte[field.GetByteSize()];
        var span = buffer.AsSpan();
        field.Serialize(ref span);
        
        // Create new field for deserialization
        var deserializedField = new AsterixFieldI020Frn024Type030();
        var readOnlySpan = new ReadOnlySpan<byte>(buffer);
        deserializedField.Deserialize(ref readOnlySpan);
        
        // Assert
        Assert.Equal(4, deserializedField.WarningErrorConditions.Count);
        Assert.True(deserializedField.HasWarningErrorCondition(WarningErrorCode.MultipathReply));
        Assert.True(deserializedField.HasWarningErrorCondition(WarningErrorCode.SplitPlot));
        Assert.True(deserializedField.HasWarningErrorCondition(WarningErrorCode.PhantomSSRPlot));
        Assert.True(deserializedField.HasWarningErrorCondition(WarningErrorCode.TransponderAnomaly));
        Assert.False(deserializedField.HasWarningErrorCondition(WarningErrorCode.NotDefined));
        
        // Test field properties
        Assert.Equal(AsterixFieldI020Frn024Type030.StaticName, deserializedField.Name);
        Assert.Equal(AsterixFieldI020Frn024Type030.StaticFrn, deserializedField.FieldReferenceNumber);
        Assert.Equal(AsterixMessageI020.Category, deserializedField.Category);
        
        // Test manipulation methods
        deserializedField.RemoveWarningErrorCondition(WarningErrorCode.SplitPlot);
        Assert.False(deserializedField.HasWarningErrorCondition(WarningErrorCode.SplitPlot));
        Assert.Equal(3, deserializedField.WarningErrorConditions.Count);
        
        deserializedField.ClearWarningErrorConditions();
        Assert.Empty(deserializedField.WarningErrorConditions);
        
        // Test adding duplicate condition
        deserializedField.AddWarningErrorCondition(WarningErrorCode.MultipathReply);
        deserializedField.AddWarningErrorCondition(WarningErrorCode.MultipathReply); // Should not add duplicate
        Assert.Single(deserializedField.WarningErrorConditions);
    }
}