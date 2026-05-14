using System;
using System.IO;
using System.Linq;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test;

[TestSubject(typeof(AsterixMessageI048))]
public class AsterixMessageI048Test
{
    private const string JasterixCat048Ed115FileName = "cat048ed1.15.bin";
    private const string JasterixCat048Ed123FileName = "cat048ed1.23.bin";

    private static readonly byte[] Cat048Ed115ExpectedFields =
    [
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 21
    ];

    private static readonly byte[] Cat048Ed123ExpectedFields =
    [
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 14, 21, 28
    ];

    [Fact]
    public void Deserialize_JasterixCat048Ed115Resource_ShouldMatchReferenceValues()
    {
        // Source: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/cat048ed1.15.bin
        // Reference assertions: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/test_cat048_1.15.cpp
        var data = ReadJasterixCat048Ed115Data();

        Assert.Equal(65, data.Length);

        var message = new AsterixMessageI048();
        var buffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);

        var record = Assert.Single(message);
        Assert.Equal(Cat048Ed115ExpectedFields, record.Select(x => x.FieldReferenceNumber).ToArray());
        AssertCommonReferenceValues(record);

        var targetReport = Assert.IsType<AsterixFieldI048Frn003Type020>(record.TargetReportDescriptor);
        Assert.False(targetReport.HasFirstExtension);
        Assert.False(targetReport.Err);
        Assert.False(targetReport.Mi);

        var measuredPosition = Assert.IsType<AsterixFieldI048Frn004Type040>(record.MeasuredPositionPolarCoordinates);
        Assert.Equal(73.921875, measuredPosition.Rho, 10);

        Assert.Null(record.ReservedExpansionField);
    }

    [Fact]
    public void Deserialize_JasterixCat048Ed123Resource_ShouldMatchReferenceValues()
    {
        // Source: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/cat048ed1.23.bin
        // Reference assertions: https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/test_cat048_1.23.cpp
        var data = ReadJasterixCat048Ed123Data();

        Assert.Equal(72, data.Length);

        var message = new AsterixMessageI048();
        var buffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);

        var record = Assert.Single(message);
        Assert.Equal(Cat048Ed123ExpectedFields, record.Select(x => x.FieldReferenceNumber).ToArray());
        AssertCommonReferenceValues(record);

        var targetReport = Assert.IsType<AsterixFieldI048Frn003Type020>(record.TargetReportDescriptor);
        Assert.True(targetReport.HasFirstExtension);
        Assert.False(targetReport.HasSecondExtension);
        Assert.True(targetReport.Err);
        Assert.True(targetReport.Mi);

        var measuredPosition = Assert.IsType<AsterixFieldI048Frn004Type040>(record.MeasuredPositionPolarCoordinates);
        Assert.Equal(255.99609375, measuredPosition.Rho, 10);

        var reservedExpansion = Assert.IsType<AsterixFieldI048Frn028TypeRe>(record.ReservedExpansionField);
        Assert.Equal([0x08, 0x01, 0x01, 0x00], reservedExpansion.Data);
    }

    [Fact]
    public void Serialize_JasterixCat048Ed115Resource_ShouldRoundtripBytes()
    {
        AssertRoundtrip(ReadJasterixCat048Ed115Data());
    }

    [Fact]
    public void Serialize_JasterixCat048Ed123Resource_ShouldRoundtripBytes()
    {
        AssertRoundtrip(ReadJasterixCat048Ed123Data());
    }

    private static void AssertCommonReferenceValues(AsterixRecordI048 record)
    {
        var source = Assert.IsType<AsterixFieldI048Frn001Type010>(record.DataSourceIdentifier);
        Assert.Equal(SystemAreaCode.LocalAirport, source.Sac);
        Assert.Equal(1, source.Sic);

        var time = Assert.IsType<AsterixFieldI048Frn002Type140>(record.TimeOfDay);
        Assert.Equal(33499.8359375, time.Seconds, 10);

        var targetReport = Assert.IsType<AsterixFieldI048Frn003Type020>(record.TargetReportDescriptor);
        Assert.Equal(5, targetReport.Typ);
        Assert.False(targetReport.Sim);
        Assert.Equal(1, targetReport.Rdp);
        Assert.False(targetReport.Spi);
        Assert.False(targetReport.Rab);

        var measuredPosition = Assert.IsType<AsterixFieldI048Frn004Type040>(record.MeasuredPositionPolarCoordinates);
        Assert.Equal(89.67041015625, measuredPosition.Theta, 10);

        var mode3A = Assert.IsType<AsterixFieldI048Frn005Type070>(record.Mode3ACode);
        Assert.False(mode3A.V);
        Assert.False(mode3A.G);
        Assert.True(mode3A.L);
        Assert.Equal(470, mode3A.Mode3AReply);

        var flightLevel = Assert.IsType<AsterixFieldI048Frn006Type090>(record.FlightLevel);
        Assert.False(flightLevel.V);
        Assert.False(flightLevel.G);
        Assert.Equal(370.0, flightLevel.FlightLevel, 10);

        var radarPlot = Assert.IsType<AsterixFieldI048Frn007Type130>(record.RadarPlotCharacteristics);
        Assert.Equal(0x20, radarPlot.Flags);
        Assert.Null(radarPlot.Srl);
        Assert.Null(radarPlot.Srr);
        Assert.Equal((sbyte)-63, radarPlot.Sam);
        Assert.Null(radarPlot.Prl);
        Assert.Null(radarPlot.Pam);
        Assert.Null(radarPlot.Rpd);
        Assert.Null(radarPlot.Apd);

        var aircraftAddress = Assert.IsType<AsterixFieldI048Frn008Type220>(record.AircraftAddress);
        Assert.Equal(11226301u, aircraftAddress.AircraftAddress);

        var aircraftIdentification = Assert.IsType<AsterixFieldI048Frn009Type240>(record.AircraftIdentification);
        Assert.Equal("RYR5XW  ", aircraftIdentification.AircraftIdentification);

        var modeSMbData = Assert.IsType<AsterixFieldI048Frn010Type250>(record.ModeSMbData);
        Assert.Equal(3, modeSMbData.Data.Count);
        AssertModeSMbData(modeSMbData.Data[0], 6, 0, [0x8B, 0xD9, 0xEB, 0x2F, 0xBF, 0xE4, 0x00]);
        AssertModeSMbData(modeSMbData.Data[1], 5, 0, [0x80, 0x91, 0x9F, 0x39, 0xA0, 0x04, 0xDD]);
        AssertModeSMbData(modeSMbData.Data[2], 4, 0, [0xC8, 0x48, 0x00, 0x30, 0xA8, 0x00, 0x00]);

        var trackNumber = Assert.IsType<AsterixFieldI048Frn011Type161>(record.TrackNumber);
        Assert.Equal(919, trackNumber.TrackNumber);

        var velocity = Assert.IsType<AsterixFieldI048Frn013Type200>(record.CalculatedTrackVelocityPolar);
        Assert.Equal(463.18359375, velocity.GroundSpeedKt, 10);
        Assert.Equal(32.607421875, velocity.HeadingDeg, 10);

        var trackStatus = Assert.IsType<AsterixFieldI048Frn014Type170>(record.TrackStatus);
        Assert.False(trackStatus.Cnf);
        Assert.Equal(2, trackStatus.Rad);
        Assert.False(trackStatus.Dou);
        Assert.False(trackStatus.Mah);
        Assert.Equal(0, trackStatus.Cdm);
        Assert.False(trackStatus.HasExtension);

        var communications = Assert.IsType<AsterixFieldI048Frn021Type230>(record.CommunicationsCapability);
        Assert.Equal(1, communications.Com);
        Assert.Equal(0, communications.Stat);
        Assert.False(communications.Si);
        Assert.True(communications.Mssc);
        Assert.True(communications.Arc);
        Assert.True(communications.Aic);
        Assert.True(communications.B1A);
        Assert.Equal(13, communications.B1B);
    }

    private static void AssertModeSMbData(AsterixI048ModeSMbData item, byte bds1, byte bds2, byte[] mbData)
    {
        Assert.Equal(bds1, item.Bds1);
        Assert.Equal(bds2, item.Bds2);
        Assert.Equal(mbData, item.MbData);
    }

    private static void AssertRoundtrip(byte[] data)
    {
        var message = new AsterixMessageI048();
        var readBuffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref readBuffer);

        var serialized = new byte[message.GetByteSize()];
        var writeBuffer = new Span<byte>(serialized);
        message.Serialize(ref writeBuffer);

        Assert.Equal(0, writeBuffer.Length);
        Assert.Equal(data, serialized);
    }

    private static byte[] ReadJasterixCat048Ed115Data()
    {
        return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", JasterixCat048Ed115FileName));
    }

    private static byte[] ReadJasterixCat048Ed123Data()
    {
        return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", JasterixCat048Ed123FileName));
    }
}
