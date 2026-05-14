using System;
using System.IO;
using System.Linq;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test;

[TestSubject(typeof(AsterixMessageI001))]
public class AsterixMessageI001Test
{
    private const string JasterixCat001Ed11FileName = "cat001ed1.1.bin";
    private static readonly byte[] JasterixCat001ExpectedFields = [1, 2, 3, 4, 5, 6, 7];

    [Fact]
    public void Deserialize_JasterixCat001Ed11Resource_ShouldMatchReferenceValues()
    {
        // Source: https://github.com/hpuhr/jASTERIX/blob/master/src/test/cat001ed1.1.bin
        // Reference assertions: https://github.com/hpuhr/jASTERIX/blob/master/src/test/test_cat001_1.1.cpp
        var data = ReadJasterixCat001Ed11Data();

        Assert.Equal(20, data.Length);

        var message = new AsterixMessageI001();
        var buffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);

        var record = Assert.Single(message);
        Assert.Equal(JasterixCat001ExpectedFields, record.Select(x => x.FieldReferenceNumber).ToArray());

        Assert.NotNull(record.DataSourceIdentifier);
        Assert.Equal(SystemAreaCode.LocalAirport, record.DataSourceIdentifier.Sac);
        Assert.Equal(1, record.DataSourceIdentifier.Sic);

        Assert.NotNull(record.TargetReportDescriptor);
        Assert.Equal(0, record.TargetReportDescriptor.Typ);
        Assert.False(record.TargetReportDescriptor.Sim);
        Assert.Equal(2, record.TargetReportDescriptor.SsrPsr);
        Assert.False(record.TargetReportDescriptor.Ant);
        Assert.False(record.TargetReportDescriptor.Spi);
        Assert.False(record.TargetReportDescriptor.Rab);
        Assert.False(record.TargetReportDescriptor.Fx);

        Assert.NotNull(record.MeasuredPositionPolarCoordinates);
        Assert.Equal(127.4375, record.MeasuredPositionPolarCoordinates.Rho, 4);
        Assert.Equal(256.61865234375, record.MeasuredPositionPolarCoordinates.Theta, 10);

        Assert.NotNull(record.Mode3AReply);
        Assert.False(record.Mode3AReply.V);
        Assert.False(record.Mode3AReply.G);
        Assert.False(record.Mode3AReply.L);
        Assert.Equal(5543, record.Mode3AReply.Mode3AReply);

        Assert.NotNull(record.ModeCCode);
        Assert.False(record.ModeCCode.V);
        Assert.False(record.ModeCCode.G);
        Assert.Equal(38000.0, record.ModeCCode.HeightFt);

        Assert.NotNull(record.RadarPlotCharacteristics);
        Assert.Equal(3, record.RadarPlotCharacteristics.Items.Count);
        Assert.Equal(96, record.RadarPlotCharacteristics.Items[0].Value);
        Assert.True(record.RadarPlotCharacteristics.Items[0].Extend);
        Assert.Equal(60, record.RadarPlotCharacteristics.Items[1].Value);
        Assert.True(record.RadarPlotCharacteristics.Items[1].Extend);
        Assert.Equal(96, record.RadarPlotCharacteristics.Items[2].Value);
        Assert.False(record.RadarPlotCharacteristics.Items[2].Extend);

        Assert.NotNull(record.TruncatedTimeOfDay);
        Assert.Equal(221.4296875, record.TruncatedTimeOfDay.Seconds, 6);
    }

    [Fact]
    public void Serialize_JasterixCat001Ed11Resource_ShouldRoundTripBytes()
    {
        var data = ReadJasterixCat001Ed11Data();
        var message = new AsterixMessageI001();
        var readBuffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref readBuffer);

        var serialized = new byte[message.GetByteSize()];
        var writeBuffer = new Span<byte>(serialized);
        message.Serialize(ref writeBuffer);

        Assert.Equal(0, writeBuffer.Length);
        Assert.Equal(data, serialized);
    }

    private static byte[] ReadJasterixCat001Ed11Data()
    {
        return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", JasterixCat001Ed11FileName));
    }
}
