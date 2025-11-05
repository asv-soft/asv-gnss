using JetBrains.Annotations;
using Xunit;
using System;
using System.Linq;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn021Type250))]
public class AsterixFieldI020Frn021Type250Test
{
    //[Fact]
    public void SerializeDeserialize_ShouldPreserveData()
    {
        // Arrange
        var field = new AsterixFieldI020Frn021Type250();
        
        // Add test data - 3 ModeSData entries with different patterns
        var modeSData1 = new ModeSData();
        modeSData1.RawData[0] = 0x12;
        modeSData1.RawData[1] = 0x34;
        modeSData1.RawData[2] = 0x56;
        modeSData1.RawData[3] = 0x78;
        modeSData1.RawData[4] = 0x9A;
        modeSData1.RawData[5] = 0xBC;
        modeSData1.RawData[6] = 0xDE;
        modeSData1.RawData[7] = 0xF0;
        field.Data.Add(modeSData1);
        
        var modeSData2 = new ModeSData();
        for (int i = 0; i < 8; i++)
        {
            modeSData2.RawData[i] = (byte)(0xFF - i);
        }
        field.Data.Add(modeSData2);
        
        var modeSData3 = new ModeSData();
        for (int i = 0; i < 8; i++)
        {
            modeSData3.RawData[i] = (byte)(i * 16);
        }
        field.Data.Add(modeSData3);
        
        // Calculate expected size: 1 byte for count + 3 * 8 bytes for data
        var expectedSize = 1 + 3 * ModeSData.ByteSize;
        var buffer = new byte[expectedSize];
        var span = buffer.AsSpan();
        
        // Act - Serialize
        field.Serialize(ref span);
        
        // Create new field for deserialization
        var deserializedField = new AsterixFieldI020Frn021Type250();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        deserializedField.Deserialize(ref readSpan);
        
        // Assert
        Assert.Equal(3, deserializedField.Data.Count);
        Assert.Equal(expectedSize, field.GetByteSize());
        
        // Verify first ModeSData
        Assert.Equal(0x12, deserializedField.Data[0].RawData[0]);
        Assert.Equal(0x34, deserializedField.Data[0].RawData[1]);
        Assert.Equal(0x56, deserializedField.Data[0].RawData[2]);
        Assert.Equal(0x78, deserializedField.Data[0].RawData[3]);
        Assert.Equal(0x9A, deserializedField.Data[0].RawData[4]);
        Assert.Equal(0xBC, deserializedField.Data[0].RawData[5]);
        Assert.Equal(0xDE, deserializedField.Data[0].RawData[6]);
        Assert.Equal(0xF0, deserializedField.Data[0].RawData[7]);
        
        // Verify second ModeSData
        for (int i = 0; i < 8; i++)
        {
            Assert.Equal((byte)(0xFF - i), deserializedField.Data[1].RawData[i]);
        }
        
        // Verify third ModeSData
        for (int i = 0; i < 8; i++)
        {
            Assert.Equal((byte)(i * 16), deserializedField.Data[2].RawData[i]);
        }
        
        // Verify field properties
        Assert.Equal("Mode S MB Data", deserializedField.Name);
        Assert.Equal(21, deserializedField.FieldReferenceNumber);
        Assert.Equal(20, deserializedField.Category);
        
        // Verify that all original data is preserved
        for (int dataIndex = 0; dataIndex < 3; dataIndex++)
        {
            for (int byteIndex = 0; byteIndex < 8; byteIndex++)
            {
                Assert.Equal(field.Data[dataIndex].RawData[byteIndex], 
                           deserializedField.Data[dataIndex].RawData[byteIndex]);
            }
        }
    }
    
    [Fact]
    public void EmptyData_ShouldSerializeDeserializeCorrectly()
    {
        // Arrange
        var field = new AsterixFieldI020Frn021Type250();
        var buffer = new byte[1]; // Only size byte needed
        var span = buffer.AsSpan();
        
        // Act - Serialize
        field.Serialize(ref span);
        
        // Deserialize
        var deserializedField = new AsterixFieldI020Frn021Type250();
        var readSpan = new ReadOnlySpan<byte>(buffer);
        deserializedField.Deserialize(ref readSpan);
        
        // Assert
        Assert.Equal(0, deserializedField.Data.Count);
        Assert.Equal(0, buffer[0]); // Size should be 0
    }
    
}