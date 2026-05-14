using System;
using System.IO;
using System.Linq;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test;

[TestSubject(typeof(AsterixMessageI021))]
public class AsterixMessageI021Test
{
    private const string JasterixCat021Ed21FileName = "cat021ed2.1.bin";
    private static readonly byte[] JasterixCat021ExpectedFields =
    [
        1, 2, 3, 4, 6, 11, 12, 14, 16, 17, 18, 19, 21, 23, 28, 29, 35
    ];

    [Fact]
    public void Deserialize_JasterixCat021Ed21Resource_ShouldMatchReferenceValues()
    {
        // Source: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/cat021ed2.1.bin
        // Reference assertions: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/test_cat021_2.1.cpp
        var data = ReadJasterixCat021Ed21Data();

        Assert.Equal(49, data.Length);

        var message = new AsterixMessageI021();
        var buffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);

        var record = Assert.Single(message);
        Assert.Equal(JasterixCat021ExpectedFields, record.Select(x => x.FieldReferenceNumber).ToArray());

        Assert.NotNull(record.DataSourceIdentifier);
        Assert.Equal(SystemAreaCode.LocalAirport, record.DataSourceIdentifier.Sac);
        Assert.Equal(3, record.DataSourceIdentifier.Sic);

        Assert.NotNull(record.TargetReportDescriptor);
        Assert.Equal(0, record.TargetReportDescriptor.Atp);
        Assert.Equal(0, record.TargetReportDescriptor.Arc);
        Assert.False(record.TargetReportDescriptor.Rc);
        Assert.False(record.TargetReportDescriptor.Rab);
        Assert.True(record.TargetReportDescriptor.HasFirstExtension);
        Assert.False(record.TargetReportDescriptor.Dcr);
        Assert.False(record.TargetReportDescriptor.Gbs);
        Assert.False(record.TargetReportDescriptor.Sim);
        Assert.False(record.TargetReportDescriptor.Tst);
        Assert.True(record.TargetReportDescriptor.Saa);
        Assert.Equal(0, record.TargetReportDescriptor.Cl);
        Assert.False(record.TargetReportDescriptor.HasSecondExtension);

        Assert.NotNull(record.TrackNumber);
        Assert.Equal(1375, record.TrackNumber.TrackNumber);

        Assert.NotNull(record.ServiceIdentification);
        Assert.Equal(0, record.ServiceIdentification.ServiceIdentification);

        Assert.NotNull(record.PositionWgs84);
        Assert.Equal(46.84420108795166015625, record.PositionWgs84.Latitude, 10);
        Assert.Equal(12.29852914810180664063, record.PositionWgs84.Longitude, 10);

        Assert.NotNull(record.TargetAddress);
        Assert.Equal(1723237u, record.TargetAddress.TargetAddress);

        Assert.NotNull(record.TimeOfMessageReceptionForPosition);
        Assert.Equal(33502.8828125, record.TimeOfMessageReceptionForPosition.Seconds, 10);

        Assert.NotNull(record.TimeOfMessageReceptionForVelocity);
        Assert.Equal(33502.46875, record.TimeOfMessageReceptionForVelocity.Seconds, 10);

        Assert.NotNull(record.GeometricHeight);
        Assert.Equal(34750.0, record.GeometricHeight.GeometricHeightFt);

        Assert.NotNull(record.QualityIndicators);
        Assert.Equal(0, record.QualityIndicators.NucrOrNacv);
        Assert.Equal(7, record.QualityIndicators.NucpOrNic);
        Assert.False(record.QualityIndicators.HasFirstExtension);

        Assert.NotNull(record.MopsVersion);
        Assert.False(record.MopsVersion.Vns);
        Assert.Equal(0, record.MopsVersion.Vn);
        Assert.Equal(2, record.MopsVersion.Ltt);

        Assert.NotNull(record.Mode3ACode);
        Assert.Equal(7106, record.Mode3ACode.Mode3ACode);

        Assert.NotNull(record.FlightLevel);
        Assert.Equal(350.0, record.FlightLevel.FlightLevel);

        Assert.NotNull(record.TargetStatus);
        Assert.False(record.TargetStatus.Icf);
        Assert.False(record.TargetStatus.Lnav);
        Assert.Equal(0, record.TargetStatus.Ps);
        Assert.Equal(0, record.TargetStatus.Ss);

        Assert.NotNull(record.TimeOfAsterixReportTransmission);
        Assert.Equal(33503.1328125, record.TimeOfAsterixReportTransmission.Seconds, 10);

        Assert.NotNull(record.TargetIdentification);
        Assert.Equal("EZS14ZH ", record.TargetIdentification.TargetIdentification);

        Assert.NotNull(record.ServiceManagement);
        Assert.Equal(2.0, record.ServiceManagement.ReportPeriod);
    }

    [Fact]
    public void Serialize_JasterixCat021Ed21Resource_ShouldRoundtripBytes()
    {
        var data = ReadJasterixCat021Ed21Data();
        var message = new AsterixMessageI021();
        var buffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref buffer);

        var serialized = new byte[message.GetByteSize()];
        var writeBuffer = new Span<byte>(serialized);
        message.Serialize(ref writeBuffer);

        Assert.Equal(0, writeBuffer.Length);
        Assert.Equal(data, serialized);
    }

    private static byte[] ReadJasterixCat021Ed21Data()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", JasterixCat021Ed21FileName);
        return File.ReadAllBytes(path);
    }
}
