using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn018Type310))]
public class AsterixFieldI020Frn018Type310Test
{
    [Fact]
    public void SerializeDeserialize_ValidValues_ShouldPreserveData()
    {
        // Arrange
        var field = new AsterixFieldI020Frn018Type310();
        var buffer = new byte[field.GetByteSize()];
        
        // Test case 1: Default values (TRB = false, MSG = None)
        field.Trb = false;
        field.Msg = PreProgrammedMessageEnum.None;
        
        // Act
        var span = buffer.AsSpan();
        field.Serialize(ref span);
        
        var readOnlySpan = new ReadOnlySpan<byte>(buffer);
        var deserializedField = new AsterixFieldI020Frn018Type310();
        deserializedField.Deserialize(ref readOnlySpan);
        
        // Assert
        Assert.False(deserializedField.Trb);
        Assert.Equal(PreProgrammedMessageEnum.None, deserializedField.Msg);
        
        // Test case 2: TRB = true, MSG = TowingAircraft
        field.Trb = true;
        field.Msg = PreProgrammedMessageEnum.TowingAircraft;
        
        span = buffer.AsSpan();
        field.Serialize(ref span);
        
        readOnlySpan = buffer.AsSpan();
        deserializedField = new AsterixFieldI020Frn018Type310();
        deserializedField.Deserialize(ref readOnlySpan);
        
        Assert.True(deserializedField.Trb);
        Assert.Equal(PreProgrammedMessageEnum.TowingAircraft, deserializedField.Msg);
        
        // Test case 3: TRB = false, MSG = EmergencyOperation
        field.Trb = false;
        field.Msg = PreProgrammedMessageEnum.EmergencyOperation;
        
        span = buffer.AsSpan();
        field.Serialize(ref span);
        
        readOnlySpan = new ReadOnlySpan<byte>(buffer);
        deserializedField = new AsterixFieldI020Frn018Type310();
        deserializedField.Deserialize(ref readOnlySpan);
        
        Assert.False(deserializedField.Trb);
        Assert.Equal(PreProgrammedMessageEnum.EmergencyOperation, deserializedField.Msg);
        
        // Test case 4: All message types
        var messageTypes = new[]
        {
            PreProgrammedMessageEnum.None,
            PreProgrammedMessageEnum.TowingAircraft,
            PreProgrammedMessageEnum.FollowMeOperation,
            PreProgrammedMessageEnum.RunwayCheck,
            PreProgrammedMessageEnum.EmergencyOperation,
            PreProgrammedMessageEnum.WorkInProgress
        };
        
        foreach (var msgType in messageTypes)
        {
            foreach (var trbValue in new[] { false, true })
            {
                field.Trb = trbValue;
                field.Msg = msgType;
                
                span = buffer.AsSpan();
                field.Serialize(ref span);
                
                readOnlySpan = new ReadOnlySpan<byte>(buffer);
                deserializedField = new AsterixFieldI020Frn018Type310();
                deserializedField.Deserialize(ref readOnlySpan);
                
                Assert.Equal(trbValue, deserializedField.Trb);
                Assert.Equal(msgType, deserializedField.Msg);
            }
        }
        
        // Verify field properties
        Assert.Equal(AsterixFieldI020Frn018Type310.StaticFrn, field.FieldReferenceNumber);
        Assert.Equal(AsterixFieldI020Frn018Type310.StaticName, field.Name);
        Assert.Equal(AsterixMessageI020.Category, field.Category);
        Assert.Equal(1, field.GetByteSize());
    }
}