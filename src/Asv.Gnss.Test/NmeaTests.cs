using System;
using System.Reactive.Linq;
using System.Text;
using Xunit;

namespace Asv.Gnss.Test
{
    public class NmeaTests
    {
        [Fact]
        public void TestGGA()
        {
            var source = "$GPGGA,125319.00,5508.7020098,N,06124.3378698,E,7,08,2.4,259.0000,M,-12.794,M,,*76\r\n";
            var array = Encoding.ASCII.GetBytes(source);
            Nmea0183MessageGGA msg = null;
            var parser = new Nmea0183Parser().RegisterDefaultMessages();
            parser.OnMessage.Cast<Nmea0183MessageGGA>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }
            Assert.NotNull(msg);
            Assert.Equal("GP",msg.SourceId);
            Assert.Equal(55.14503349666667, msg.Latitude, 7);
            Assert.Equal(61.405631163333325, msg.Longitude, 7);
        }

        [Fact]
        public void TestGGLL()
        {
            var source = "$GPGLL,5508.7020098,N,06124.3378698,E,130521.00,A,M*64\r\n";
            var array = Encoding.ASCII.GetBytes(source);
            Nmea0183MessageGLL msg = null;
            var parser = new Nmea0183Parser().RegisterDefaultMessages();
            parser.OnMessage.Cast<Nmea0183MessageGLL>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }
            Assert.NotNull(msg);
            Assert.Equal("GP", msg.SourceId);
            Assert.Equal(55.14503349666667, msg.Latitude, 7);
            Assert.Equal(61.405631163333325, msg.Longitude, 7);
        }
    }
}