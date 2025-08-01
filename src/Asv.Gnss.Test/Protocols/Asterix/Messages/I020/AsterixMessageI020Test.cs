using System;
using System.Linq;
using Asv.Gnss;
using DeepEqual.Syntax;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test.Protocols.Messages.I020;

[TestSubject(typeof(AsterixMessageI020))]
public class AsterixMessageI020Test
{

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

                },
                ModeSMbData = new AsterixFieldI020Frn021Type250
                {

                },
                CommsAcasCapabilityStatus = new AsterixFieldI020Frn022Type230
                {
                    AIC = false,
                    ARC = true,
                    B1A = false,
                    B1B = false,
                    COM = true,
                    MSSC = false,
                    STAT = false
                },
                AcasResolutionAdvisoryReport = null,
                WarningErrorConditions = null,
                Mode1Code = null,
                Mode2Code = null,
                ReservedExpansionField = new AsterixFieldI020Frn027TypeRe()
                {

                },

            }
        };
        
        var buffer = new ReadOnlySpan<byte>(data);
        deserialized.Deserialize(ref buffer);

        var a = origin.First().Select(x => x.FieldReferenceNumber).ToArray();
        var b = deserialized.First().Select(x => x.FieldReferenceNumber).ToArray();
        
        origin.ShouldDeepEqual(deserialized);
        
        

    }
}