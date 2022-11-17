using System;
using System.Reactive.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Gnss.Test
{
    public class RTCMv3Test
    {
        private readonly ITestOutputHelper _output;

        public RTCMv3Test(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestMsg1()
        {
            var array = new byte[]
            {
                0xD3, 0x00, 0x66, 0x43, 0x50, 0x00, 0x58, 0xBE, 0xDF, 0x42, 
                0x00, 0x00, 0x00, 0x20, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 
                0x28, 0x20, 0x81, 0x00, 0x0F, 0xE8, 0x89, 0x60, 0x0D, 0x3C, 
                0xAA, 0x02, 0x27, 0xF9, 0xFF, 0x3B, 0xDD, 0xF0, 0x29, 0xC3, 
                0x35, 0x24, 0x33, 0x24, 0x63, 0x58, 0x14, 0x35, 0xB6, 0xA3, 
                0x47, 0xED, 0xFD, 0x1E, 0x6D, 0xFC, 0x51, 0x82, 0x0C, 0x9F, 
                0xB0, 0x0C, 0x9F, 0xB0, 0x0D, 0xA2, 0x58, 0x0D, 0x79, 0xAC, 
                0x0D, 0x68, 0x8C, 0xEF, 0x3C, 0x0F, 0x13, 0xC2, 0xF0, 0xBC, 
                0x4F, 0x10, 0x08, 0x86, 0x12, 0x94, 0x9E, 0x77, 0x9E, 0x52, 
                0xA1, 0x3E, 0xC5, 0x80, 0xB1, 0xE8, 0x77, 0xD0, 0xEF, 0xA1, 
                0xB7, 0x5E, 0x6E, 0xAD, 0xE0, 0x10, 0x3D, 0x08,
                
                
            };
            var parser = new RtcmV3Parser().RegisterDefaultMessages();
            RtcmV3MessageBase msg = null;
            parser.OnMessage.Cast<RtcmV3MessageBase>().Subscribe(_ => msg = _);
            for (var index = 0; index < array.Length; index++)
            {
                var p = array[index];
                parser.Read(p);
            }

            Assert.NotNull(msg);
        }

        [Fact]
        public void TestFromData()
        {
            
            var parser = new RtcmV3Parser().RegisterDefaultMessages();
            parser.OnError.Subscribe(_ =>
            {
                //_output.WriteLine("ERR:"+_.Message);
            });
            parser.OnMessage.Subscribe(_ =>
            {
                _output.WriteLine($"[{_.MessageStringId}]=> {_.Name}");
            });
            foreach (var b in TestData.test_rtcm3)
            {
                parser.Read(b);
            }
        }

        [Fact]
        public void TestFromData2()
        {
            var parser = new RtcmV3Parser().RegisterDefaultMessages();
            parser.OnError.Subscribe(_ =>
            {
               //_output.WriteLine("ERR:"+_.Message);
            });
            parser.OnMessage.Subscribe(_ =>
            {
                _output.WriteLine($"[{_.MessageStringId}]=> {_.Name}");
            });
            foreach (var b in TestData.testglo_rtcm3)
            {
                parser.Read(b);
            }
        }
    }
}