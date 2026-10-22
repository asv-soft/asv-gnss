using System;
using Asv.IO;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss;

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
        Assert.True(span.IsEmpty);
        Assert.Equal(new byte[] { 0x03, 0x07, 0x15, 0x1E }, buffer);
        
        // Create new field for deserialization
        var deserializedField = new AsterixFieldI020Frn024Type030();
        var readOnlySpan = new ReadOnlySpan<byte>(buffer);
        deserializedField.Deserialize(ref readOnlySpan);
        Assert.True(readOnlySpan.IsEmpty);
        
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

    [Fact]
    public void Deserialize_ShouldReplacePreviousWarningConditionsAndPreserveFollowingBytes()
    {
        var field = new AsterixFieldI020Frn024Type030();
        field.AddWarningErrorCondition(WarningErrorCode.TransponderAnomaly);
        var buffer = new ReadOnlySpan<byte>(new byte[] { 0x03, 0x06, 0xA5 });

        field.Deserialize(ref buffer);

        Assert.Equal(new[] { WarningErrorCode.MultipathReply, WarningErrorCode.SplitPlot }, field.WarningErrorConditions);
        Assert.Equal(new byte[] { 0xA5 }, buffer.ToArray());
    }

    [Fact]
    public void Accept_ShouldKeepChangedWarningsAndSerializedDataConsistent()
    {
        var field = new AsterixFieldI020Frn024Type030();
        field.AddWarningErrorCondition(WarningErrorCode.MultipathReply);

        field.Accept(new ReplaceWarningErrorVisitor((byte)WarningErrorCode.PhantomSSRPlot, (byte)WarningErrorCode.TransponderAnomaly));

        Assert.Equal(new[] { WarningErrorCode.PhantomSSRPlot, WarningErrorCode.TransponderAnomaly }, field.WarningErrorConditions);
        var buffer = new byte[field.GetByteSize()];
        var span = buffer.AsSpan();
        field.Serialize(ref span);
        Assert.True(span.IsEmpty);
        Assert.Equal(new byte[] { 0x15, 0x1E }, buffer);
        var decoded = new AsterixFieldI020Frn024Type030();
        var remaining = new ReadOnlySpan<byte>(buffer);
        decoded.Deserialize(ref remaining);
        Assert.Equal(field.WarningErrorConditions, decoded.WarningErrorConditions);
    }

    private sealed class ReplaceWarningErrorVisitor(params byte[] values) : ListType.IVisitor, UInt8Type.IVisitor
    {
        private int _index;

        public void BeginList(Field field, ListType type, ref uint size)
        {
            size = (uint)values.Length;
        }

        public void EndList()
        {
        }

        public void Visit(Field field, UInt8Type type, ref byte value)
        {
            value = values[_index++];
        }

        public void VisitUnknown(Field field, IFieldType type)
        {
            throw new InvalidOperationException($"Unexpected field type: {type.Name}");
        }
    }
}
