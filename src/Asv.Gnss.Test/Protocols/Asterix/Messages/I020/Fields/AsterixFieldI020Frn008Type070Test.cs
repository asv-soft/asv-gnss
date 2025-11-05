using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn008Type070))]
public class AsterixFieldI020Frn008Type070Test
{

    
    //[Fact]
    public void DeserializeAndSerialize_ShouldWorkCorrectly()
    {
        // Arrange
        var field = new AsterixFieldI020Frn008Type070();
        var testValues = new byte[] { 0x40, 0x58 }; // Binary: 0100 0000 0101 1000
        var buffer = new ReadOnlySpan<byte>(testValues);
        
        // Act
        field.Deserialize(ref buffer);
        
        // Assert
        Assert.False(field.V); // V bit (15) should be 0
        Assert.True(field.G);  // G bit (14) should be 1
        Assert.False(field.L); // L bit (13) should be 0
        Assert.Equal(0x058, field.Mode3ACode); // Mode-3/A code should be 0x058 (88 decimal)
        
        // Test serialization
        var outputBuffer = new byte[2];
        var span = new Span<byte>(outputBuffer);
        field.Serialize(ref span);
        
        // Verify serialized output matches input
        Assert.Equal(testValues, outputBuffer);
        
        // Verify byte size
        Assert.Equal(2, field.GetByteSize());
    }
}