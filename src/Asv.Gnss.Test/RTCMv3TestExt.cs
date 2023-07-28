using System;
using System.Reactive.Linq;
using Xunit;

namespace Asv.Gnss.Test
{
    public class RTCMv3TestExt
    {
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
            var parser = new RtcmV3Parser().RegisterExtendedMessages();
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
            var parser = new RtcmV3Parser().RegisterExtendedMessages();
            RtcmV3MessageBase msg = null;
            parser.OnMessage.Cast<RtcmV3MessageBase>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }
            Assert.NotNull(msg);
            Assert.Equal("1033", msg.MessageStringId);
        }
    }
}