using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test;

[TestSubject(typeof(AsterixMessageI247))]
public class AsterixMessageI247Test
{
    private const string JasterixCat247Ed12FileName = "cat247ed1.2.bin";
    private const string AsterixStreamFileName = "2025-07-09-06.stream";

    [Fact]
    public void Deserialize_JasterixCat247Ed12Resource_ShouldMatchReferenceValues()
    {
        // Source: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/cat247ed1.2.bin
        // Reference: ASTERIX CAT247 edition 1.2 UAP.
        var data = ReadJasterixCat247Ed12Data();

        Assert.Equal(20, data.Length);

        var message = new AsterixMessageI247();
        var buffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);

        var record = Assert.Single(message);
        Assert.Equal([1, 2, 3, 4], record.Select(x => x.FieldReferenceNumber).ToArray());

        var source = Assert.IsType<AsterixFieldI247Frn001Type010>(record.DataSourceIdentifier);
        Assert.Equal(SystemAreaCode.LocalAirport, source.Sac);
        Assert.Equal(1, source.Sic);

        var service = Assert.IsType<AsterixFieldI247Frn002Type015>(record.ServiceIdentification);
        Assert.Equal(1, service.ServiceIdentification);

        var time = Assert.IsType<AsterixFieldI247Frn003Type140>(record.TimeOfDay);
        Assert.Equal(14431.4609375, time.Seconds, 10);

        var report = Assert.IsType<AsterixFieldI247Frn004Type550>(record.CategoryVersionNumberReport);
        AssertVersions(report,
            (21, 2, 1),
            (23, 1, 2),
            (247, 1, 2));
    }

    [Fact]
    public void Deserialize_FirstCat247FromStream_ShouldMatchVersionReport()
    {
        var data = ReadFirstCat247PayloadFromStream();

        Assert.Equal(26, data.Length);

        var message = new AsterixMessageI247();
        var buffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);

        var record = Assert.Single(message);
        Assert.Equal([1, 2, 3, 4], record.Select(x => x.FieldReferenceNumber).ToArray());

        var source = Assert.IsType<AsterixFieldI247Frn001Type010>(record.DataSourceIdentifier);
        Assert.Equal(SystemAreaCode.RussiaUrals, source.Sac);
        Assert.Equal(0x89, source.Sic);

        var service = Assert.IsType<AsterixFieldI247Frn002Type015>(record.ServiceIdentification);
        Assert.Equal(0, service.ServiceIdentification);

        var time = Assert.IsType<AsterixFieldI247Frn003Type140>(record.TimeOfDay);
        Assert.Equal(21600.84375, time.Seconds, 10);

        var report = Assert.IsType<AsterixFieldI247Frn004Type550>(record.CategoryVersionNumberReport);
        AssertVersions(report,
            (21, 2, 4),
            (23, 1, 2),
            (25, 1, 1),
            (20, 1, 8),
            (19, 1, 3));
    }

    [Fact]
    public void Serialize_JasterixCat247Ed12Resource_ShouldRoundtripBytes()
    {
        AssertRoundtrip(ReadJasterixCat247Ed12Data());
    }

    [Fact]
    public void Serialize_FirstCat247FromStream_ShouldRoundtripBytes()
    {
        AssertRoundtrip(ReadFirstCat247PayloadFromStream());
    }

    private static void AssertRoundtrip(byte[] data)
    {
        var message = new AsterixMessageI247();
        var readBuffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref readBuffer);

        var serialized = new byte[message.GetByteSize()];
        var writeBuffer = new Span<byte>(serialized);
        message.Serialize(ref writeBuffer);

        Assert.Equal(0, writeBuffer.Length);
        Assert.Equal(data, serialized);
    }

    private static void AssertVersions(
        AsterixFieldI247Frn004Type550 report,
        params (int Category, int Major, int Minor)[] expected)
    {
        Assert.Equal(expected.Length, report.Versions.Count);
        for (var i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].Category, report.Versions[i].Category);
            Assert.Equal(expected[i].Major, report.Versions[i].Major);
            Assert.Equal(expected[i].Minor, report.Versions[i].Minor);
        }
    }

    private static byte[] ReadJasterixCat247Ed12Data()
    {
        return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", JasterixCat247Ed12FileName));
    }

    private static byte[] ReadFirstCat247PayloadFromStream()
    {
        var data = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", AsterixStreamFileName));
        var position = 64;

        while (position + 11 <= data.Length)
        {
            var payloadLength = BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(position, 4));
            var payload = data.AsSpan(position + 8, payloadLength);
            if (payload[0] == AsterixMessageI247.Category)
            {
                return payload.ToArray();
            }

            position += 8 + payloadLength;
        }

        throw new InvalidDataException("CAT247 data block was not found in stream resource.");
    }
}
