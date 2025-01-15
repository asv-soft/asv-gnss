using System;
using System.Reactive.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Gnss.Test
{
    public class RTCMv2Test
    {
        private readonly ITestOutputHelper _output;

        public RTCMv2Test(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestMsg1()
        {
            var array = new byte[]
            {
                0x66,
                0x41,
                0x42,
                0x40,
                0x4E,
                0x4C,
                0x5A,
                0x4C,
                0x42,
                0x68,
                0x67,
                0x43,
                0x68,
                0x4B,
                0x52,
                0x40,
                0x50,
                0x55,
                0x55,
                0x5B,
            };
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

        [Fact]
        public void TestFromData2()
        {
            var parser = new RtcmV2Parser().RegisterDefaultMessages();
            parser.OnError.Subscribe(_ =>
            {
                //_output.WriteLine("ERR:"+_.Message);
            });
            parser.OnMessage.Subscribe(_ =>
            {
                _output.WriteLine($"[{_.MessageStringId}]=> {_.Name}");
            });
            foreach (var b in TestData.testglo_rtcm2)
            {
                parser.Read(b);
            }
        }
    }
}
