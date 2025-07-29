using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020.Fields;

[TestSubject(typeof(AsterixFieldI020Frn006Type161))]
public class AsterixFieldI020Frn006Type161Test
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(1234)]
    [InlineData(4095)]
    public void SerializeDeserialize_ShouldMatchOriginal(ushort expected)
    {
        var field = new AsterixFieldI020Frn006Type161
        {
            TrackNumber = expected
        };

        Span<byte> buffer = stackalloc byte[field.GetByteSize()];
        field.Serialize(ref buffer);

        var bytes = buffer.ToArray();

        var readBuffer = new ReadOnlySpan<byte>(bytes);
        var deserializedField = new AsterixFieldI020Frn006Type161();
        deserializedField.Deserialize(ref readBuffer);

        Assert.Equal(expected, deserializedField.TrackNumber);
    }

    [Fact]
    public void GetByteSize_ShouldBeTwo()
    {
        var field = new AsterixFieldI020Frn006Type161();
        Assert.Equal(2, field.GetByteSize());
    }

    [Fact]
    public void Serialize_ShouldUseBigEndian()
    {
        var field = new AsterixFieldI020Frn006Type161
        {
            TrackNumber = 0x1234
        };

        Span<byte> buffer = stackalloc byte[2];
        var copy = buffer;
        field.Serialize(ref copy);
        Assert.Equal(0, copy.Length);
        Assert.Equal(0x12, buffer[0]);
        Assert.Equal(0x34, buffer[1]);
    }
}