using System;
using System.Reactive.Linq;
using Xunit;

namespace Asv.Gnss.Test
{
    public class RTCMv3TestExt
    {
        [Fact]
        public void TestMsg1007()
        {
            var array = new byte[]
            {
                0xd3,
                0x00,
                0x19,
                0x3e,
                0xf3,
                0xfe,
                0x14,
                0x41,
                0x44,
                0x56,
                0x4e,
                0x55,
                0x4c,
                0x4c,
                0x41,
                0x4e,
                0x54,
                0x45,
                0x4e,
                0x4e,
                0x41,
                0x20,
                0x20,
                0x4e,
                0x4f,
                0x4e,
                0x45,
                0x00,
                0x8a,
                0x4f,
                0x77,
            };
            var parser = new RtcmV3Parser().RegisterDefaultMessages();
            RtcmV3MessageBase msg = null;
            parser.OnMessage.Cast<RtcmV3MessageBase>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }

            Assert.NotNull(msg);
            Assert.Equal("1007", msg.MessageStringId);
            var msg1007 = msg as RtcmV3Message1007;
            Assert.Equal(1022U, msg1007.ReferenceStationID);
            Assert.Equal("ADVNULLANTENNA  NONE", msg1007.AntennaDescriptor);
        }

        [Fact]
        public void TestMsg1008()
        {
            var array = new byte[]
            {
                0xd3,
                0x00,
                0x1f,
                0x3f,
                0x00,
                0x0d,
                0x14,
                0x4a,
                0x41,
                0x56,
                0x47,
                0x52,
                0x41,
                0x4e,
                0x54,
                0x5f,
                0x47,
                0x35,
                0x54,
                0x2b,
                0x47,
                0x50,
                0x20,
                0x4a,
                0x56,
                0x47,
                0x52,
                0x00,
                0x05,
                0x30,
                0x33,
                0x32,
                0x30,
                0x31,
                0xec,
                0xbd,
                0x40,
            };
            var parser = new RtcmV3Parser().RegisterDefaultMessages();
            RtcmV3MessageBase msg = null;
            parser.OnMessage.Cast<RtcmV3MessageBase>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }

            Assert.NotNull(msg);
            Assert.Equal("1008", msg.MessageStringId);
        }

        [Fact]
        public void TestMsg1033()
        {
            var array = new byte[]
            {
                0xd3,
                0x00,
                0x22,
                0x40,
                0x90,
                0x00,
                0x00,
                0x00,
                0x00,
                0x09,
                0x48,
                0x45,
                0x4d,
                0x49,
                0x5f,
                0x44,
                0x46,
                0x35,
                0x72,
                0x08,
                0x36,
                0x2e,
                0x30,
                0x41,
                0x61,
                0x30,
                0x35,
                0x62,
                0x08,
                0x32,
                0x31,
                0x38,
                0x35,
                0x30,
                0x38,
                0x34,
                0x38,
                0x02,
                0x2e,
                0x4e,
            };
            var parser = new RtcmV3Parser().RegisterDefaultMessages();
            RtcmV3MessageBase msg = null;
            parser.OnMessage.Cast<RtcmV3MessageBase>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }

            Assert.NotNull(msg);
            Assert.Equal("1033", msg.MessageStringId);
        }

        [Fact]
        public void TestMsg1230()
        {
            var array = new byte[]
            {
                0xd3,
                0x00,
                0x0c,
                0x4c,
                0xe3,
                0xfe,
                0x0f,
                0x03,
                0xba,
                0x03,
                0xba,
                0x03,
                0xba,
                0x03,
                0xba,
                0x7c,
                0x66,
                0xda,
            };
            var parser = new RtcmV3Parser().RegisterExtendedMessages();
            RtcmV3MessageBase msg = null;
            parser.OnMessage.Cast<RtcmV3MessageBase>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }

            Assert.NotNull(msg);
            Assert.Equal("1230", msg.MessageStringId);

            var msg1230 = msg as RtcmV3Message1230;
            Assert.Equal(19.08, msg1230.L1CACodePhaseBias, 2);
            Assert.Equal(19.08, msg1230.L1PCodePhaseBias, 2);
            Assert.Equal(19.08, msg1230.L2CACodePhaseBias, 2);
            Assert.Equal(19.08, msg1230.L2PCodePhaseBias, 2);
        }

        [Fact]
        public void TestMsg1032()
        {
            var array = new byte[]
            {
                0xd3,
                0x00,
                0x14,
                0x40,
                0x83,
                0xfe,
                0x40,
                0x10,
                0x07,
                0xc4,
                0xf0,
                0xd9,
                0x30,
                0x0e,
                0xb7,
                0x12,
                0xbe,
                0x74,
                0xc1,
                0x1b,
                0xad,
                0x89,
                0x80,
                0xc9,
                0x29,
                0xe6,
            };
            var parser = new RtcmV3Parser().RegisterExtendedMessages();
            RtcmV3MessageBase msg = null;
            parser.OnMessage.Cast<RtcmV3MessageBase>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }

            Assert.NotNull(msg);
            Assert.Equal("1032", msg.MessageStringId);
            var msg1032 = msg as RtcmV3Message1032;
            Assert.Equal(1025U, msg1032.PhysicalReferenceStationID);
            Assert.Equal(1022U, msg1032.NonPhysicalReferenceStationID);
        }

        [Fact]
        public void TestMsg1030()
        {
            var array = new byte[]
            {
                0xd3,
                0x00,
                0x3f,
                0x40,
                0x62,
                0x08,
                0x4f,
                0x3f,
                0xe0,
                0xc9,
                0x08,
                0x4c,
                0x00,
                0x00,
                0x1e,
                0x00,
                0x06,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x04,
                0x05,
                0x00,
                0x00,
                0x08,
                0x80,
                0x03,
                0x06,
                0x80,
                0x00,
                0x04,
                0xc0,
                0x02,
                0x43,
                0x80,
                0x00,
                0x01,
                0x20,
                0x02,
                0x22,
                0xa0,
                0x00,
                0x00,
                0xd0,
                0x01,
                0x31,
                0x00,
                0x00,
                0x00,
                0x28,
                0x00,
                0xa9,
                0x08,
                0x00,
                0x00,
                0x28,
                0x00,
                0x7c,
                0x24,
                0x00,
                0x00,
                0x12,
                0x00,
                0x00,
                0x52,
                0x20,
                0x7d,
            };
            var parser = new RtcmV3Parser().RegisterExtendedMessages();
            RtcmV3MessageBase msg = null;
            parser.OnMessage.Cast<RtcmV3MessageBase>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }

            Assert.NotNull(msg);
            Assert.Equal("1030", msg.MessageStringId);
            var msg1030 = msg as RtcmV3Message1030;
            Assert.Equal(1022U, msg1030.ReferenceStationID);
            Assert.InRange(msg1030.SOc, 0.0, 127 * 0.5);
            Assert.InRange(msg1030.SOd, 0.0, 5.11 * 0.01);
            Assert.InRange(msg1030.SOh, 0.0, 5.11 * 0.1);
            Assert.InRange(msg1030.SIc, 0.0, 511 * 0.5);
            Assert.InRange(msg1030.SId, 0.0, 10.23 * 0.01);
        }

        [Fact]
        public void TestMsg1031()
        {
            var array = new byte[]
            {
                0xd3,
                0x00,
                0x32,
                0x40,
                0x77,
                0x07,
                0x69,
                0xff,
                0x06,
                0x38,
                0x60,
                0xc0,
                0x00,
                0x00,
                0xe0,
                0x00,
                0x40,
                0x20,
                0x00,
                0x00,
                0x10,
                0x00,
                0x28,
                0x80,
                0x00,
                0x00,
                0x18,
                0x00,
                0x30,
                0x30,
                0x00,
                0x00,
                0x52,
                0x00,
                0x24,
                0x54,
                0x00,
                0x00,
                0x17,
                0x00,
                0x13,
                0x12,
                0x00,
                0x00,
                0x09,
                0x80,
                0x0a,
                0x03,
                0x80,
                0x00,
                0x02,
                0x80,
                0x00,
                0x38,
                0x01,
                0x10,
            };
            var parser = new RtcmV3Parser().RegisterExtendedMessages();
            RtcmV3MessageBase msg = null;
            parser.OnMessage.Cast<RtcmV3MessageBase>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }

            Assert.NotNull(msg);
            Assert.Equal("1031", msg.MessageStringId);
            var msg1031 = msg as RtcmV3Message1031;
            Assert.Equal(1022U, msg1031.ReferenceStationID);
            Assert.InRange(msg1031.SOc, 0.0, 127 * 0.5);
            Assert.InRange(msg1031.SOd, 0.0, 5.11 * 0.01);
            Assert.InRange(msg1031.SOh, 0.0, 5.11 * 0.1);
            Assert.InRange(msg1031.SIc, 0.0, 511 * 0.5);
            Assert.InRange(msg1031.SId, 0.0, 10.23 * 0.01);
        }

        [Fact]
        public void TestMsg1042()
        {
            var array = new byte[]
            {
                0xd3,
                0x00,
                0x40,
                0x41,
                0x28,
                0xc7,
                0x1c,
                0x1e,
                0xc5,
                0x05,
                0x2e,
                0x58,
                0x00,
                0x3f,
                0xde,
                0x66,
                0x4f,
                0xb3,
                0x6b,
                0x0f,
                0xdb,
                0xe6,
                0x5d,
                0xc6,
                0x6e,
                0x64,
                0x03,
                0x7b,
                0xf1,
                0x9e,
                0x80,
                0x36,
                0xcf,
                0x68,
                0x81,
                0xbb,
                0xf4,
                0xa2,
                0xaa,
                0xeb,
                0xc9,
                0x72,
                0xcf,
                0xff,
                0x18,
                0xe0,
                0xf0,
                0xec,
                0x04,
                0x01,
                0x16,
                0x27,
                0x11,
                0xb3,
                0x1e,
                0x14,
                0x75,
                0x45,
                0xe8,
                0xf3,
                0x5c,
                0x3f,
                0xeb,
                0xf4,
                0xbe,
                0xdf,
                0xb4,
                0x8c,
                0x66,
                0xfd,
            };

            // File.WriteAllBytes("rtcm1125.log", array);
            var parser = new RtcmV3Parser().RegisterDefaultMessages().RegisterExtendedMessages();
            RtcmV3MessageBase msg = null;
            parser.OnError.Subscribe(_ => Console.WriteLine("ERR:" + _.Message));
            parser.OnMessage.Cast<RtcmV3MessageBase>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }

            Assert.NotNull(msg);
            Assert.Equal("1042", msg.MessageStringId);
            var msg1042 = msg as RtcmV3Message1042;
            Assert.Equal(35U, msg1042.SatellitePrn);
            Assert.Equal(910U, msg1042.WeekRaw);
            var week = msg1042.GetWeek(new DateTime(2023, 6, 14));
            Assert.Equal(910, week);
            Assert.Equal(0, msg1042.Urai);
            Assert.True((-7.16227077646181e-11 - msg1042.IdotRaw) < double.Epsilon);
            Assert.Equal(1U, msg1042.Aode);
            Assert.Equal(309600D, msg1042.TocRaw);
            Assert.Equal(0.0, msg1042.A2);
            Assert.True((-7.640110766260477e-12 - msg1042.A1) < double.Epsilon);
            Assert.True(msg1042.A0 - 0.0006080692401155829 < double.Epsilon);
            Assert.Equal(1U, msg1042.Adoc);
            Assert.Equal(-72.203125, msg1042.Crs);
            Assert.True(msg1042.DeltaN - 1.3645831131725572e-09 < double.Epsilon);
            Assert.True(0.43121358612552285 - msg1042.M0 < double.Epsilon);
            Assert.True(-3.428664058446884e-06 - msg1042.Cuc < double.Epsilon);
            Assert.True(msg1042.E - 0.0008363371016457677 < double.Epsilon);
            Assert.True(1.653563231229782e-06 - msg1042.Cus < double.Epsilon);
            Assert.True(5282.6676597595215 - msg1042.Apow1_2 < double.Epsilon);
            Assert.Equal(309600D, msg1042.ToeRaw);
            Assert.True(-2.7008354663848877e-08 - msg1042.Cic < double.Epsilon);
            Assert.True(0.4393380885012448 - msg1042.OmegaBig0 < double.Epsilon);
            Assert.True(1.2945383787155151e-07 - msg1042.Cis < double.Epsilon);
            Assert.True(0.3052276512607932 - msg1042.I0 < double.Epsilon);
            Assert.Equal(327.328125, msg1042.Crc);
            Assert.True(0.1846863552927971 - msg1042.Omega < double.Epsilon);
            Assert.True(-2.3335360310738906e-09 - msg1042.OmegaBig < double.Epsilon);
            Assert.Equal(-1.9000000000000001, msg1042.TGd1);
            Assert.Equal(-1.9000000000000001, msg1042.TGd2);
            Assert.Equal(0, msg1042.SvHealth);
        }

        [Fact]
        public void TestMsg1046()
        {
            var array = new byte[]
            {
                0xd3,
                0x00,
                0x3f,
                0x41,
                0x61,
                0x53,
                0x68,
                0x07,
                0x6b,
                0x05,
                0x5d,
                0x44,
                0x60,
                0x00,
                0x01,
                0xf1,
                0xff,
                0xc3,
                0x20,
                0x18,
                0x02,
                0x14,
                0x99,
                0x79,
                0xd3,
                0x13,
                0x72,
                0xdc,
                0x01,
                0x48,
                0x00,
                0x89,
                0xdb,
                0x80,
                0x27,
                0xe2,
                0xa8,
                0x13,
                0xb6,
                0x05,
                0x44,
                0x60,
                0x00,
                0xda,
                0x55,
                0x76,
                0xa5,
                0xcf,
                0xfe,
                0x72,
                0x70,
                0xd8,
                0x10,
                0x71,
                0xe3,
                0x9d,
                0x17,
                0xe7,
                0x50,
                0x6f,
                0xfc,
                0x0f,
                0xf0,
                0x34,
                0x0f,
                0x00,
                0x95,
                0x78,
                0x82,
            };

            // File.WriteAllBytes("rtcm1125.log", array);
            var parser = new RtcmV3Parser().RegisterExtendedMessages();
            RtcmV3MessageBase msg = null;
            parser.OnError.Subscribe(_ => Console.WriteLine("ERR:" + _.Message));
            parser.OnMessage.Cast<RtcmV3MessageBase>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }

            Assert.NotNull(msg);
            Assert.Equal("1046", msg.MessageStringId);
            var msg1046 = msg as RtcmV3Message1046;
            Assert.Equal(5U, msg1046.SatellitePrn);
            Assert.Equal(1242U, msg1046.WeekRaw);
            var week = msg1046.GetWeek(new DateTime(2023, 6, 14));
            Assert.Equal(1242, week);
            Assert.Equal(7U, msg1046.IoDnav);
            Assert.Equal((byte)107, msg1046.SvSisa);
            Assert.True((msg1046.IdotRaw - 3.89945853385143e-11) < double.Epsilon);
            Assert.Equal(311400U, msg1046.TocRaw);
            Assert.Equal(0.0, msg1046.Af2);
            Assert.True((msg1046.Af1 - 3.524291969370097e-12) < double.Epsilon);
            Assert.Equal(-5.805457476526499e-05, msg1046.Af0);
            Assert.Equal(4.15625, msg1046.Crs);
            Assert.True((msg1046.DeltaN - 1.1166321201017126e-09) < double.Epsilon);
            Assert.True((0.9122577565722167 - msg1046.M0) < double.Epsilon);
            Assert.True((1.5273690223693848e-07 - msg1046.Cuc) < double.Epsilon);
            Assert.True((msg1046.E - 0.0002629421651363373) < double.Epsilon);
            Assert.True((4.753470420837402e-06 - msg1046.Cus) < double.Epsilon);
            Assert.True((5440.615968704224 - msg1046.Apow1_2) < double.Epsilon);
            Assert.Equal(311400U, msg1046.ToeRaw);
            Assert.True((2.421438694000244e-08 - msg1046.Cic) < double.Epsilon);
            Assert.Equal(-0.7082697916775942, msg1046.OmegaBig0);
            Assert.True((-4.6566128730773926e-08 - msg1046.Cis) < double.Epsilon);
            Assert.True((0.30509960977360606 - msg1046.I0) < double.Epsilon);
            Assert.Equal(241.78125, msg1046.Crc);
            Assert.True((-0.36332833487540483 - msg1046.Omega) < double.Epsilon);
            Assert.True((-1.833655005611945e-09 - msg1046.OmegaDot) < double.Epsilon);
            Assert.True((msg1046.BGdE1E5a - 3.026798367500305e-09) < double.Epsilon);
            Assert.True((msg1046.BGdE5bE1 - 3.4924596548080444e-09) < double.Epsilon);
            Assert.Equal(0, msg1046.E5bSignalHealthFlag);
            Assert.Equal(0, msg1046.E5bDataValidity);
            Assert.Equal(0, msg1046.E1BSignalHealthFlag);
            Assert.Equal(0, msg1046.E1BDataValidity);
            Assert.Equal(0, msg1046.Reserved);
            Assert.True(msg1046.IsE5bSignalOk);
            Assert.False(msg1046.IsE5bSignalOuOfService);
            Assert.False(msg1046.IsE5bSignalWillOuOfService);
            Assert.False(msg1046.IsE5bSignalInTest);
            Assert.True(msg1046.IsE1BSignalOk);
            Assert.False(msg1046.E1BSignalOuOfService);
            Assert.False(msg1046.E1BSignalWillOuOfService);
            Assert.False(msg1046.E1BSignalInTest);
        }
    }
}
