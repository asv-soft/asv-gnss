using System;
using System.IO;
using System.Linq;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Asterix.Messages.I002;

[TestSubject(typeof(AsterixMessageI002))]
public class AsterixMessageI002Test
{
    private const string JasterixCat002Ed10FileName = "cat002ed1.0.bin";
    private static readonly byte[] JasterixCat002ExpectedFields = [1, 2, 4, 6];

    [Fact]
    public void Deserialize_JasterixCat002Ed10Resource_ShouldMatchReferenceValues()
    {
        // Source: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/cat002ed1.0.bin
        // Reference assertions: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/test_cat002_1.0.cpp
        var data = ReadJasterixCat002Ed10Data();

        Assert.Equal(12, data.Length);

        var message = new AsterixMessageI002();
        var buffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);

        var record = Assert.Single(message);
        Assert.Equal(JasterixCat002ExpectedFields, record.Select(x => x.FieldReferenceNumber).ToArray());

        Assert.NotNull(record.DataSourceIdentifier);
        Assert.Equal(SystemAreaCode.LocalAirport, record.DataSourceIdentifier.Sac);
        Assert.Equal(1, record.DataSourceIdentifier.Sic);

        Assert.NotNull(record.MessageType);
        Assert.Equal(1, record.MessageType.MessageType);

        Assert.NotNull(record.TimeOfDay);
        Assert.Equal(33501.4140625, record.TimeOfDay.Seconds, 10);

        Assert.NotNull(record.StationConfigurationStatus);
        Assert.Equal([73, 1], record.StationConfigurationStatus.Values);
    }

    [Fact]
    public void Serialize_JasterixCat002Ed10Resource_ShouldRoundtripBytes()
    {
        var data = ReadJasterixCat002Ed10Data();
        var message = new AsterixMessageI002();
        var buffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref buffer);

        var serialized = new byte[message.GetByteSize()];
        var writeBuffer = new Span<byte>(serialized);
        message.Serialize(ref writeBuffer);

        Assert.Equal(0, writeBuffer.Length);
        Assert.Equal(data, serialized);
    }

    private static byte[] ReadJasterixCat002Ed10Data()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", JasterixCat002Ed10FileName);
        return File.ReadAllBytes(path);
    }
}
