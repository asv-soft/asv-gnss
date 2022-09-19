using System;
using System.Reactive.Linq;
using Xunit;

namespace Asv.Gnss.Test
{
    public class RTCMv2Test
    {
        [Fact]
        public void TestMsg1()
        {
            var array = new byte[]{ 0x66, 0x41, 0x42, 0x40, 0x4E, 0x4C, 0x5A, 0x4C, 0x42, 0x68, 0x67, 0x43, 0x68, 0x4B, 0x52, 0x40, 0x50, 0x55, 0x55, 0x5B};
            var parser = new RtcmV2Parser().RegisterDefaultMessages();
            RtcmV2MessageBase msg = null;
            parser.OnMessage.Cast<RtcmV2MessageBase>().Subscribe(_ => msg = _);
            for (var index = 0; index < array.Length; index++)
            {
                var p = array[index];
                parser.Read(p);
            }

            Assert.NotNull(msg);
        }
    }
}