using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn023Type260))]
public class AsterixFieldI020Frn023Type260Test
{

    [Fact]
    public void SerializeDeserialize_ShouldPreserveData()
    {
        // Arrange
        var field = new AsterixFieldI020Frn023Type260();
        
        // Set test data pattern in MbData - using a recognizable pattern
        field.MbData[0] = 0x12;
        field.MbData[1] = 0x34;
        field.MbData[2] = 0x56;
        field.MbData[3] = 0x78;
        field.MbData[4] = 0x9A;
        field.MbData[5] = 0xBC;
        field.MbData[6] = 0xDE;
        
        var buffer = new byte[7];
        var span = buffer.AsSpan();
        
        // Act - Serialize
        field.Serialize(ref span);
        
        // Create new field for deserialization
        var deserializedField = new AsterixFieldI020Frn023Type260();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        deserializedField.Deserialize(ref readSpan);
        
        // Assert
        Assert.Equal(7, field.GetByteSize());
        Assert.Equal("ACAS Resolution Advisory Report", deserializedField.Name);
        Assert.Equal(23, deserializedField.FieldReferenceNumber);
        Assert.Equal(20, deserializedField.Category);
        
        // Verify all bytes are preserved
        for (int i = 0; i < 7; i++)
        {
            Assert.Equal(field.MbData[i], deserializedField.MbData[i]);
        }
        
        // Verify specific test pattern
        Assert.Equal(0x12, deserializedField.MbData[0]);
        Assert.Equal(0x34, deserializedField.MbData[1]);
        Assert.Equal(0x56, deserializedField.MbData[2]);
        Assert.Equal(0x78, deserializedField.MbData[3]);
        Assert.Equal(0x9A, deserializedField.MbData[4]);
        Assert.Equal(0xBC, deserializedField.MbData[5]);
        Assert.Equal(0xDE, deserializedField.MbData[6]);
        
        // Test utility methods
        Assert.True(deserializedField.HasData());
        
        // Test UInt64 conversion
        var expectedUInt64 = 0x123456789ABCDE00UL >> 8; // Shift right 8 bits since we only have 7 bytes
        Assert.Equal(0x123456789ABCDEUL, deserializedField.GetMbDataAsUInt64());
        
        // Test setting from UInt64
        var testField = new AsterixFieldI020Frn023Type260();
        testField.SetMbDataFromUInt64(0x123456789ABCDEUL);
        Assert.Equal(0x12, testField.MbData[0]);
        Assert.Equal(0x34, testField.MbData[1]);
        Assert.Equal(0x56, testField.MbData[2]);
        Assert.Equal(0x78, testField.MbData[3]);
        Assert.Equal(0x9A, testField.MbData[4]);
        Assert.Equal(0xBC, testField.MbData[5]);
        Assert.Equal(0xDE, testField.MbData[6]);
        
        // Test clear functionality
        testField.ClearMbData();
        Assert.False(testField.HasData());
        for (int i = 0; i < 7; i++)
        {
            Assert.Equal(0, testField.MbData[i]);
        }
    }
}