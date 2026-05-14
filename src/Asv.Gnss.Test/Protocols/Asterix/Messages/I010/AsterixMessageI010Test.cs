using System;
using System.IO;
using System.Linq;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test;

[TestSubject(typeof(AsterixMessageI010))]
public class AsterixMessageI010Test
{
    private const string JasterixCat010Ed031FileName = "cat010ed0.31.bin";
    private const string JasterixCat010Ed024SensisFileName = "cat010ed0.24_sensis.bin";

    private static readonly byte[] JasterixCat010Ed031ExpectedFields =
    [
        1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 19, 25
    ];

    private static readonly byte[] JasterixCat010Ed024SensisExpectedFields =
    [
        1, 2, 3, 4, 5, 7, 9, 10, 11, 13, 18, 22
    ];

    [Fact]
    public void Deserialize_JasterixCat010Ed031Resource_ShouldMatchReferenceValues()
    {
        // Source: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/cat010ed0.31.bin
        // Reference assertions: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/test_cat010_0.31.cpp
        var data = ReadJasterixCat010Ed031Data();

        Assert.Equal(41, data.Length);

        var message = new AsterixMessageI010();
        var buffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);

        var record = Assert.Single(message);
        Assert.Equal(JasterixCat010Ed031ExpectedFields, record.Select(x => x.FieldReferenceNumber).ToArray());

        Assert.NotNull(record.DataSourceIdentifier);
        Assert.Equal(SystemAreaCode.LocalAirport, record.DataSourceIdentifier.Sac);
        Assert.Equal(1, record.DataSourceIdentifier.Sic);

        Assert.NotNull(record.MessageType);
        Assert.Equal(1, record.MessageType.MessageType);

        Assert.NotNull(record.TargetReportDescriptor);
        Assert.Equal(3, record.TargetReportDescriptor.Typ);
        Assert.False(record.TargetReportDescriptor.Dcr);
        Assert.False(record.TargetReportDescriptor.Chn);
        Assert.False(record.TargetReportDescriptor.Gbs);
        Assert.False(record.TargetReportDescriptor.Crt);
        Assert.True(record.TargetReportDescriptor.HasFirstExtension);
        Assert.False(record.TargetReportDescriptor.Sim);
        Assert.False(record.TargetReportDescriptor.Tst);
        Assert.False(record.TargetReportDescriptor.Rab);
        Assert.Equal(0, record.TargetReportDescriptor.Lop);
        Assert.Equal(0, record.TargetReportDescriptor.Tot);
        Assert.False(record.TargetReportDescriptor.HasSecondExtension);

        Assert.NotNull(record.TimeOfDay);
        Assert.Equal(24693.140625, record.TimeOfDay.Seconds, 10);

        Assert.NotNull(record.MeasuredPositionPolarCoordinates);
        Assert.Equal(1588.0, record.MeasuredPositionPolarCoordinates.Rho);
        Assert.Equal(189.5086669921875, record.MeasuredPositionPolarCoordinates.Theta, 10);

        Assert.NotNull(record.PositionCartesian);
        Assert.Equal(-267.0, record.PositionCartesian.X);
        Assert.Equal(-1566.0, record.PositionCartesian.Y);

        Assert.NotNull(record.TrackVelocityPolar);
        Assert.Equal(0.000244140625, record.TrackVelocityPolar.GroundSpeed, 12);
        Assert.Equal(267.275390625, record.TrackVelocityPolar.TrackAngle, 10);

        Assert.NotNull(record.TrackVelocityCartesian);
        Assert.False(record.TrackVelocityCartesian.IsSensisEncoding);
        Assert.Equal(-0.5, record.TrackVelocityCartesian.Vx);
        Assert.Equal(0.0, record.TrackVelocityCartesian.Vy);

        Assert.NotNull(record.TrackNumber);
        Assert.Equal(4, record.TrackNumber.TrackNumber);

        Assert.NotNull(record.TrackStatus);
        Assert.False(record.TrackStatus.Cnf);
        Assert.False(record.TrackStatus.Tre);
        Assert.False(record.TrackStatus.Cst);
        Assert.False(record.TrackStatus.Mah);
        Assert.False(record.TrackStatus.Tcc);
        Assert.True(record.TrackStatus.Sth);
        Assert.True(record.TrackStatus.HasFirstExtension);
        Assert.Equal(3, record.TrackStatus.Tom);
        Assert.False(record.TrackStatus.Dou);
        Assert.Equal(0, record.TrackStatus.Mrs);
        Assert.True(record.TrackStatus.HasSecondExtension);
        Assert.False(record.TrackStatus.Gho);

        Assert.NotNull(record.TargetSizeAndOrientation);
        Assert.Equal(27.0, record.TargetSizeAndOrientation.Length);
        Assert.True(record.TargetSizeAndOrientation.HasOrientation);
        Assert.Equal(267.1875, record.TargetSizeAndOrientation.Orientation, 10);
        Assert.True(record.TargetSizeAndOrientation.HasWidth);
        Assert.Equal(40.0, record.TargetSizeAndOrientation.Width);

        Assert.NotNull(record.CalculatedAcceleration);
        Assert.Equal(-1.0, record.CalculatedAcceleration.Ax);
        Assert.Equal(-0.25, record.CalculatedAcceleration.Ay);
    }

    [Fact]
    public void Deserialize_JasterixCat010Ed024SensisResource_ShouldMatchReferenceValues()
    {
        // Source: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/cat010ed0.24_sensis.bin
        // Reference assertions: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/test_cat010_0.24_sensis.cpp
        var data = ReadJasterixCat010Ed024SensisData();

        Assert.Equal(43, data.Length);

        var message = new AsterixMessageI010();
        var buffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);

        var record = Assert.Single(message);
        Assert.Equal(JasterixCat010Ed024SensisExpectedFields, record.Select(x => x.FieldReferenceNumber).ToArray());

        Assert.NotNull(record.DataSourceIdentifier);
        Assert.Equal(SystemAreaCode.LocalAirport, record.DataSourceIdentifier.Sac);
        Assert.Equal(0, record.DataSourceIdentifier.Sic);

        Assert.NotNull(record.MessageType);
        Assert.Equal(1, record.MessageType.MessageType);

        Assert.NotNull(record.TargetReportDescriptor);
        Assert.Equal(1, record.TargetReportDescriptor.Typ);
        Assert.False(record.TargetReportDescriptor.Dcr);
        Assert.False(record.TargetReportDescriptor.Chn);
        Assert.False(record.TargetReportDescriptor.Gbs);
        Assert.True(record.TargetReportDescriptor.Crt);
        Assert.True(record.TargetReportDescriptor.HasFirstExtension);
        Assert.False(record.TargetReportDescriptor.Sim);
        Assert.False(record.TargetReportDescriptor.Tst);
        Assert.False(record.TargetReportDescriptor.Rab);
        Assert.Equal(0, record.TargetReportDescriptor.Lop);
        Assert.Equal(0, record.TargetReportDescriptor.Tot);
        Assert.True(record.TargetReportDescriptor.HasSecondExtension);
        Assert.False(record.TargetReportDescriptor.Spi);

        Assert.NotNull(record.TimeOfDay);
        Assert.Equal(84080.484375, record.TimeOfDay.Seconds, 10);

        Assert.NotNull(record.PositionWgs84);
        Assert.Equal(25.62264827080070972443, record.PositionWgs84.Latitude, 10);
        Assert.Equal(22.1754983067512512207, record.PositionWgs84.Longitude, 10);

        Assert.NotNull(record.PositionCartesian);
        Assert.Equal(-1423.0, record.PositionCartesian.X);
        Assert.Equal(1368.0, record.PositionCartesian.Y);

        Assert.NotNull(record.TrackVelocityCartesian);
        Assert.True(record.TrackVelocityCartesian.IsSensisEncoding);
        Assert.Equal(-8.0, record.TrackVelocityCartesian.Vx);
        Assert.Equal(1.0, record.TrackVelocityCartesian.Vy);

        Assert.NotNull(record.TrackNumber);
        Assert.Equal(922, record.TrackNumber.TrackNumber);

        Assert.NotNull(record.TrackStatus);
        Assert.False(record.TrackStatus.Cnf);
        Assert.False(record.TrackStatus.Tre);
        Assert.False(record.TrackStatus.Cst);
        Assert.False(record.TrackStatus.Mah);
        Assert.False(record.TrackStatus.Tcc);
        Assert.True(record.TrackStatus.Sth);
        Assert.False(record.TrackStatus.HasFirstExtension);

        Assert.NotNull(record.TargetAddress);
        Assert.Equal(4672673, record.TargetAddress.TargetAddress);

        Assert.NotNull(record.MeasuredHeight);
        Assert.Equal(0.0, record.MeasuredHeight.MeasuredHeight);

        Assert.NotNull(record.PositionAccuracy);
        Assert.Equal(2.5, record.PositionAccuracy.SdpX);
        Assert.Equal(3.25, record.PositionAccuracy.SdpY);
        Assert.Equal(-1.0, record.PositionAccuracy.SdpXy);
    }

    [Fact]
    public void Serialize_JasterixCat010Ed031Resource_ShouldRoundTripBytes()
    {
        var data = ReadJasterixCat010Ed031Data();
        var message = new AsterixMessageI010();
        var readBuffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref readBuffer);

        var serialized = new byte[message.GetByteSize()];
        var writeBuffer = new Span<byte>(serialized);
        message.Serialize(ref writeBuffer);

        Assert.Equal(0, writeBuffer.Length);
        Assert.Equal(data, serialized);
    }

    [Fact]
    public void Serialize_JasterixCat010Ed024SensisResource_ShouldRoundTripBytes()
    {
        var data = ReadJasterixCat010Ed024SensisData();
        var message = new AsterixMessageI010();
        var readBuffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref readBuffer);

        var serialized = new byte[message.GetByteSize()];
        var writeBuffer = new Span<byte>(serialized);
        message.Serialize(ref writeBuffer);

        Assert.Equal(0, writeBuffer.Length);
        Assert.Equal(data, serialized);
    }

    private static byte[] ReadJasterixCat010Ed031Data()
    {
        return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", JasterixCat010Ed031FileName));
    }

    private static byte[] ReadJasterixCat010Ed024SensisData()
    {
        return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", JasterixCat010Ed024SensisFileName));
    }
}
