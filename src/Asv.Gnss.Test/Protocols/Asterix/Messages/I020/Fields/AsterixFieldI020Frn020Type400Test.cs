using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn020Type400))]
public class AsterixFieldI020Frn020Type400Test
{
    [Fact]
    public void DeserializeAndSerialize_ShouldWorkCorrectly()
    {
        // Arrange
        var field = new AsterixFieldI020Frn020Type400();
    
        // Test case 1: Empty contributing devices (REP = 0)
        var emptyData = new byte[] { 0x00 };
        var buffer = new ReadOnlySpan<byte>(emptyData);
    
        // Act
        field.Deserialize(ref buffer);
    
        // Assert
        Assert.Empty(field.ContributingUnits);
        Assert.Equal(1, field.GetByteSize());
    
        // Test serialization of empty data
        var outputBuffer = new byte[1];
        var span = new Span<byte>(outputBuffer);
        field.Serialize(ref span);
        Assert.Equal(emptyData, outputBuffer);
    
        // Test case 2: Contributing devices with pattern 0x81 (devices 1 and 8)
        var testData = new byte[] { 0x01, 0x81 }; // REP=1, first octet has bits 1 and 8 set
        buffer = new ReadOnlySpan<byte>(testData);
    
        // Act
        field.Deserialize(ref buffer);
    
        // Assert
        Assert.Equal(2, field.ContributingUnits.Count);
        Assert.Contains((byte)1, field.ContributingUnits);
        Assert.Contains((byte)8, field.ContributingUnits);
        Assert.True(field.HasContributed(1));
        Assert.True(field.HasContributed(8));
        Assert.False(field.HasContributed(2));
    
        // Test serialization
        outputBuffer = new byte[2];
        span = new Span<byte>(outputBuffer);
        field.Serialize(ref span);
        Assert.Equal(testData, outputBuffer);
    
        // Test case 3: Multiple octets with various devices
        var multiOctetData = new byte[] { 0x02, 0xFF, 0x01 }; // REP=2, all bits in first octet, only bit 8 in second
        buffer = new ReadOnlySpan<byte>(multiOctetData);
    
        field.Deserialize(ref buffer);
    
        // Should have devices 1-8 from first octet and device 16 from second octet
        Assert.Equal(9, field.ContributingUnits.Count);
        for (byte i = 1; i <= 8; i++)
        {
            Assert.Contains(i, field.ContributingUnits);
        }
        Assert.Contains((byte)16, field.ContributingUnits);
    
        // Test add/remove functionality
        field.ClearContributingDevices();
        Assert.Empty(field.ContributingUnits);
    
        field.Add(5);
        field.Add(10);
        Assert.Equal(2, field.ContributingUnits.Count);
        Assert.True(field.HasContributed(5));
        Assert.True(field.HasContributed(10));
    
        field.RemoveContributingDevice(5);
        Assert.Single(field.ContributingUnits);
        Assert.False(field.HasContributed(5));
        Assert.True(field.HasContributed(10));
    
        // Test adding duplicate device (should not add twice)
        field.Add(10);
        Assert.Single(field.ContributingUnits);
    
        // Test invalid device number
        Assert.Throws<ArgumentException>(() => field.Add(0));
    }
}