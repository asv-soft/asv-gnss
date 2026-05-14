using System;
using System.IO;
using System.Linq;
using Asv.Gnss;
using DeepEqual.Syntax;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test;

[TestSubject(typeof(AsterixMessageI020))]
public class AsterixMessageI020Test
{
    private const string JasterixCat020Ed15FileName = "cat020ed1.5.bin";
    private static readonly byte[] JasterixCat020ExpectedFields =
    [
        1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 16, 20, 21, 22, 27
    ];

    [Fact]
    public void Deserialize_ShouldPreserveAllProperties()
    {
        // https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/cat020ed1.5.bin
        // https://github.com/OpenATSGmbH/jASTERIX/blob/master/src/test/test_cat020_1.5.cpp
        byte[] data =
        [
            0x14, 0x00, 0x65, 0xFF, 0xE9, 0x47, 0x84, 0x00,
            0x02, 0x41, 0x00, 0x41, 0x6F, 0x5B, 0x00, 0x88,
            0x32, 0xE5, 0x00, 0x2E, 0x6C, 0x4A, 0x05, 0x4B,
            0xB3, 0x01, 0x60, 0x6A, 0x0D, 0xC8, 0x18, 0x2E,
            0x00, 0xFF, 0xC9, 0xFF, 0xDB, 0x00, 0x2D, 0x02,
            0x44, 0x2F, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10,
            0x00, 0x00, 0x20, 0x00, 0x22, 0x02, 0x10, 0x00,
            0x00, 0x00, 0xA0, 0x00, 0x00, 0x10, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x17, 0x20, 0x40,
            0x15, 0x80, 0xD0, 0x00, 0x12, 0x00, 0x0F, 0xFF,
            0xF1, 0x00, 0x89, 0x00, 0x7C, 0xFF, 0x86, 0x00,
            0x35, 0x00, 0x53, 0xFF, 0xC1
        ];

        var deserialized = new AsterixMessageI020();

        var origin = new AsterixMessageI020
        {
            new AsterixRecordI020
            {
                DataSourceIdentifier = new()
                {
                    Sac = SystemAreaCode.LocalAirport,
                    Sic = 2
                },
                TargetReportDescriptor = new()
                {
                    Ot = false,
                    Dme = false,
                    Uat = false,
                    Vdl4 = false,
                    Hf = false,
                    Ms = true,
                    Ssr = false,
                    Tst = false,
                    Sim = false,
                    Crt = false,
                    Gbs = false,
                    Chn = false,
                    Spi = false,
                    Rab = false,
                },
                TimeOfDay = new AsterixFieldI020Frn003Type140
                {
                    Time = TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(33502.7109375))
                },
                PositionWgs84 = new AsterixFieldI020Frn004Type041
                {
                    Latitude = 47.88239300251007,
                    Longitude = 16.320587396621704,
                },
                PositionCartesian = new AsterixFieldI020Frn005Type042
                {
                    X = 173529.5,
                    Y = 45109.0,
                },
                TrackNumber = new AsterixFieldI020Frn006Type161
                {
                    TrackNumber = 3528,
                },
                TrackStatus = new AsterixFieldI020Frn007Type170
                {
                    Cdm = CdmEnum.Maintaining,
                    Cnf = false,
                    Cst = false,
                    Mah = false,
                    Sth = false,
                    Tre = false
                },
                Mode3ACode = new AsterixFieldI020Frn008Type070
                {
                    G = false,
                    L = true,
                    Mode3ACode = 7000,
                    V = false,
                },
                TrackVelocityCartesian = new AsterixFieldI020Frn009Type202
                {
                    Vx = -13.75,
                    Vy = -9.25,
                },
                FlightLevel = new AsterixFieldI020Frn010Type090
                {
                    FlightLevelFt = 11.25 * 100.0, // 1125 feet
                    G = false,
                    V = false,
                },
                ModeCCode = null,
                TargetAddress = new AsterixFieldI020Frn012Type220()
                {
                    TargetAddress = 148527,
                },
                TargetIdentification = null,
                MeasuredHeightCartesian = null,
                GeometricHeightWgs84 = null,
                CalculatedAcceleration = new AsterixFieldI020Frn016Type210()
                {
                    Ax = 0.0,
                    Ay = 0.0,
                },
                VehicleFleetId = null,
                PreProgrammedMessage = null,
                PositionAccuracy = null,
                ContributingDevices = new AsterixFieldI020Frn020Type400
                {
                    84, 107, 123, 127
                },
                ModeSMbData = new AsterixFieldI020Frn021Type250
                {
                    Data =
                    {
                        CreateModeSData(0x10, 0x00, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x10),
                        CreateModeSData(0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17),
                    }

                },
                CommsAcasCapabilityStatus = new AsterixFieldI020Frn022Type230
                {
                    Aic = false,
                    Arc = true,
                    B1A = false,
                    B1B = 0,
                    Com = CommunicationsCapability.CommAAndB,
                    Mssc = false,
                    Stat = FlightStatus.NoAlertNoSpiAirborne
                },
                AcasResolutionAdvisoryReport = null,
                WarningErrorConditions = null,
                Mode1Code = null,
                Mode2Code = null,
                ReservedExpansionField = new AsterixFieldI020Frn027TypeRe()
                {
                    Data =
                    [
                        0x80, 0xD0, 0x00, 0x12, 0x00, 0x0F, 0xFF, 0xF1, 0x00, 0x89,
                        0x00, 0x7C, 0xFF, 0x86, 0x00, 0x35, 0x00, 0x53, 0xFF, 0xC1
                    ]
                },

            }
        };
        
        var buffer = new ReadOnlySpan<byte>(data);
        deserialized.Deserialize(ref buffer);

        var a = origin.First().Select(x => x.FieldReferenceNumber).ToArray();
        var b = deserialized.First().Select(x => x.FieldReferenceNumber).ToArray();
        
        origin.ShouldDeepEqual(deserialized);
        
        

    }

    [Fact]
    public void Deserialize_JasterixCat020Ed15Resource_ShouldMatchReferenceValues()
    {
        // Source: https://github.com/hpuhr/jASTERIX/blob/master/src/test/cat020ed1.5.bin
        // Reference assertions: https://github.com/hpuhr/jASTERIX/blob/master/src/test/test_cat020_1.5.cpp
        var data = ReadJasterixCat020Ed15Data();

        Assert.Equal(101, data.Length);

        var message = new AsterixMessageI020();
        var buffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);

        var record = Assert.Single(message);
        Assert.Equal(JasterixCat020ExpectedFields, record.Select(x => x.FieldReferenceNumber).ToArray());

        Assert.NotNull(record.DataSourceIdentifier);
        Assert.Equal(SystemAreaCode.LocalAirport, record.DataSourceIdentifier.Sac);
        Assert.Equal(2, record.DataSourceIdentifier.Sic);

        Assert.NotNull(record.TargetReportDescriptor);
        Assert.False(record.TargetReportDescriptor.Ssr);
        Assert.True(record.TargetReportDescriptor.Ms);
        Assert.False(record.TargetReportDescriptor.Hf);
        Assert.False(record.TargetReportDescriptor.Vdl4);
        Assert.False(record.TargetReportDescriptor.Uat);
        Assert.False(record.TargetReportDescriptor.Dme);
        Assert.False(record.TargetReportDescriptor.Ot);
        Assert.False(record.TargetReportDescriptor.Rab);
        Assert.False(record.TargetReportDescriptor.Spi);
        Assert.False(record.TargetReportDescriptor.Chn);
        Assert.False(record.TargetReportDescriptor.Gbs);
        Assert.False(record.TargetReportDescriptor.Crt);
        Assert.False(record.TargetReportDescriptor.Sim);
        Assert.False(record.TargetReportDescriptor.Tst);

        Assert.NotNull(record.TimeOfDay);
        Assert.Equal(TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(33502.7109375)), record.TimeOfDay.Time);

        Assert.NotNull(record.PositionWgs84);
        Assert.Equal(47.88239300251007080078, record.PositionWgs84.Latitude, 10);
        Assert.Equal(16.32058739662170410156, record.PositionWgs84.Longitude, 10);

        Assert.NotNull(record.PositionCartesian);
        Assert.Equal(173529.5, record.PositionCartesian.X);
        Assert.Equal(45109.0, record.PositionCartesian.Y);

        Assert.NotNull(record.TrackNumber);
        Assert.Equal(3528, record.TrackNumber.TrackNumber);

        Assert.NotNull(record.TrackStatus);
        Assert.False(record.TrackStatus.Cnf);
        Assert.False(record.TrackStatus.Tre);
        Assert.False(record.TrackStatus.Cst);
        Assert.Equal(CdmEnum.Maintaining, record.TrackStatus.Cdm);
        Assert.False(record.TrackStatus.Mah);
        Assert.False(record.TrackStatus.Sth);

        Assert.NotNull(record.Mode3ACode);
        Assert.False(record.Mode3ACode.V);
        Assert.False(record.Mode3ACode.G);
        Assert.True(record.Mode3ACode.L);
        Assert.Equal(7000, record.Mode3ACode.Mode3ACode);

        Assert.NotNull(record.TrackVelocityCartesian);
        Assert.Equal(-13.75, record.TrackVelocityCartesian.Vx);
        Assert.Equal(-9.25, record.TrackVelocityCartesian.Vy);

        Assert.NotNull(record.FlightLevel);
        Assert.False(record.FlightLevel.V);
        Assert.False(record.FlightLevel.G);
        Assert.Equal(1125.0, record.FlightLevel.FlightLevelFt);

        Assert.NotNull(record.TargetAddress);
        Assert.Equal(148527u, record.TargetAddress.TargetAddress);

        Assert.NotNull(record.CalculatedAcceleration);
        Assert.Equal(0.0, record.CalculatedAcceleration.Ax);
        Assert.Equal(0.0, record.CalculatedAcceleration.Ay);

        Assert.NotNull(record.ContributingDevices);
        Assert.Equal([84, 107, 123, 127], record.ContributingDevices.ContributingUnits);

        Assert.NotNull(record.ModeSMbData);
        Assert.Equal(2, record.ModeSMbData.Data.Count);
        Assert.Equal([0x10, 0x00, 0x00, 0x00, 0xA0, 0x00, 0x00, 0x10], record.ModeSMbData.Data[0].RawData);
        Assert.Equal([0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x17], record.ModeSMbData.Data[1].RawData);

        Assert.NotNull(record.CommsAcasCapabilityStatus);
        Assert.Equal(CommunicationsCapability.CommAAndB, record.CommsAcasCapabilityStatus.Com);
        Assert.Equal(FlightStatus.NoAlertNoSpiAirborne, record.CommsAcasCapabilityStatus.Stat);
        Assert.False(record.CommsAcasCapabilityStatus.Mssc);
        Assert.True(record.CommsAcasCapabilityStatus.Arc);
        Assert.False(record.CommsAcasCapabilityStatus.Aic);
        Assert.False(record.CommsAcasCapabilityStatus.B1A);
        Assert.Equal(0, record.CommsAcasCapabilityStatus.B1B);

        Assert.NotNull(record.ReservedExpansionField);
        Assert.Equal(
            [
                0x80, 0xD0, 0x00, 0x12, 0x00, 0x0F, 0xFF, 0xF1, 0x00, 0x89,
                0x00, 0x7C, 0xFF, 0x86, 0x00, 0x35, 0x00, 0x53, 0xFF, 0xC1
            ],
            record.ReservedExpansionField.Data);
    }

    [Fact]
    public void Serialize_JasterixCat020Ed15Resource_ShouldRoundTripBytes()
    {
        var data = ReadJasterixCat020Ed15Data();
        var message = new AsterixMessageI020();
        var readBuffer = new ReadOnlySpan<byte>(data);
        message.Deserialize(ref readBuffer);

        var serialized = new byte[message.GetByteSize()];
        var writeBuffer = new Span<byte>(serialized);
        message.Serialize(ref writeBuffer);

        Assert.Equal(0, writeBuffer.Length);
        Assert.Equal(data, serialized);
    }

    private static ModeSData CreateModeSData(params byte[] data)
    {
        var result = new ModeSData();
        data.CopyTo(result.RawData, 0);
        return result;
    }

    private static byte[] ReadJasterixCat020Ed15Data()
    {
        return File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", JasterixCat020Ed15FileName));
    }
}
