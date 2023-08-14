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
                0xd3,0x00,0x19,0x3e,0xf3,0xfe,0x14,0x41,0x44,0x56,0x4e,0x55,0x4c,0x4c,0x41,0x4e,
                0x54,0x45,0x4e,0x4e,0x41,0x20,0x20,0x4e,0x4f,0x4e,0x45,0x00,0x8a,0x4f,0x77
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
            Assert.Equal((uint)1022, msg1007.ReferenceStationID);
            Assert.Equal("ADVNULLANTENNA  NONE", msg1007.AntennaDescriptor);
        }

        [Fact]
        public void TestMsg1008()
        {
            var array = new byte[]
            {
                0xd3, 0x00, 0x1f, 0x3f, 0x00, 0x0d, 0x14, 0x4a, 0x41, 0x56,
                0x47, 0x52, 0x41, 0x4e, 0x54, 0x5f, 0x47, 0x35, 0x54, 0x2b,
                0x47, 0x50, 0x20, 0x4a, 0x56, 0x47, 0x52, 0x00, 0x05, 0x30,
                0x33, 0x32, 0x30, 0x31, 0xec, 0xbd, 0x40
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
                0xd3, 0x00, 0x22, 0x40, 0x90, 0x00, 0x00, 0x00, 0x00, 0x09,
                0x48, 0x45, 0x4d, 0x49, 0x5f, 0x44, 0x46, 0x35, 0x72, 0x08,
                0x36, 0x2e, 0x30, 0x41, 0x61, 0x30, 0x35, 0x62, 0x08, 0x32,
                0x31, 0x38, 0x35, 0x30, 0x38, 0x34, 0x38, 0x02, 0x2e, 0x4e
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
                0xd3, 0x00, 0x0c, 0x4c, 0xe3, 0xfe, 0x0f, 0x03, 0xba, 0x03, 0xba, 0x03, 0xba, 0x03, 0xba, 0x7c, 0x66, 0xda
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
                0xd3, 0x00, 0x14, 0x40, 0x83, 0xfe, 0x40, 0x10, 0x07, 0xc4,
                0xf0, 0xd9, 0x30, 0x0e, 0xb7, 0x12, 0xbe, 0x74, 0xc1, 0x1b,
                0xad, 0x89, 0x80, 0xc9, 0x29, 0xe6
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
            Assert.Equal((uint)1025, msg1032.PhysicalReferenceStationID);
            Assert.Equal((uint)1022, msg1032.NonPhysicalReferenceStationID);
        }

        [Fact]
        public void TestMsg1030()
        {
            var array = new byte[]
            {
                0xd3,0x00,0x3f,0x40,0x62,0x08,0x4f,0x3f,0xe0,0xc9,0x08,0x4c,0x00,0x00,0x1e,0x00,0x06,0x00,0x00,0x00,
                0x00,0x00,0x04,0x05,0x00,0x00,0x08,0x80,0x03,0x06,0x80,0x00,0x04,0xc0,0x02,0x43,0x80,0x00,0x01,0x20,
                0x02,0x22,0xa0,0x00,0x00,0xd0,0x01,0x31,0x00,0x00,0x00,0x28,0x00,0xa9,0x08,0x00,0x00,0x28,0x00,0x7c,
                0x24,0x00,0x00,0x12,0x00,0x00,0x52,0x20,0x7d
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
            Assert.Equal((uint)1022, msg1030.ReferenceStationID);
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
                0xd3,0x00,0x32,0x40,0x77,0x07,0x69,0xff,0x06,0x38,0x60,0xc0,0x00,0x00,0xe0,0x00,0x40,0x20,0x00,0x00,
                0x10,0x00,0x28,0x80,0x00,0x00,0x18,0x00,0x30,0x30,0x00,0x00,0x52,0x00,0x24,0x54,0x00,0x00,0x17,0x00,
                0x13,0x12,0x00,0x00,0x09,0x80,0x0a,0x03,0x80,0x00,0x02,0x80,0x00,0x38,0x01,0x10
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
            Assert.Equal((uint)1022, msg1031.ReferenceStationID);
            Assert.InRange(msg1031.SOc, 0.0, 127 * 0.5);
            Assert.InRange(msg1031.SOd, 0.0, 5.11 * 0.01);
            Assert.InRange(msg1031.SOh, 0.0, 5.11 * 0.1);
            Assert.InRange(msg1031.SIc, 0.0, 511 * 0.5);
            Assert.InRange(msg1031.SId, 0.0, 10.23 * 0.01);
        }

 
    }
}