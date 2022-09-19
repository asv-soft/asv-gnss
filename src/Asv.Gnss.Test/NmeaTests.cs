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
        public void TestGLL()
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

        [Fact]
        public void TestGSV()
        {
            var source = "$GPGSV,4,1,15,10,61,242,,08,28,312,,32,13,190,,24,12,105,*72\r\n";
            var array = Encoding.ASCII.GetBytes(source);
            Nmea0183MessageGSV msg = null;
            var parser = new Nmea0183Parser().RegisterDefaultMessages();
            parser.OnMessage.Cast<Nmea0183MessageGSV>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }
            Assert.NotNull(msg);
            Assert.Equal("GP", msg.SourceId);
            Assert.Equal(15, msg.SatellitesInView);
            Assert.Equal(4, msg.TotalNumberOfMsg);
            Assert.Equal(1, msg.MessageNumber);
            Assert.Equal(4, msg.Satellites.Length);
            Assert.Equal(242, msg.Satellites[0].AzimuthDeg);
            Assert.Equal(61, msg.Satellites[0].ElevationDeg);
            Assert.Equal(10, msg.Satellites[0].Number);
            Assert.Equal(0, msg.Satellites[0].SnrdB);
            Assert.Equal(NmeaNavigationSystemEnum.SYS_GPS, msg.Satellites[0].ExtNavSys);
            Assert.Equal(10, msg.Satellites[0].ExtPRN);

        }

        [Fact]
        public void TestGST()
        {
            var source = "$GPGST,060417.00,6.167,11.396,3.866,295.633,6.038,10.409,12.671*68\r\n";
            var array = Encoding.ASCII.GetBytes(source);
            Nmea0183MessageGST msg = null;
            var parser = new Nmea0183Parser().RegisterDefaultMessages();
            parser.OnMessage.Cast<Nmea0183MessageGST>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }
            Assert.NotNull(msg);
            Assert.Equal("GP", msg.SourceId);
            Assert.Equal(295.633, msg.OrientationSemiMajorAxis);
            Assert.Equal(6.167, msg.RmsSd);
            Assert.Equal(12.671, msg.SdAltitude);
            Assert.Equal(6.038, msg.SdLatitude);
            Assert.Equal(10.409, msg.SdLongitude);
            Assert.Equal(11.396, msg.SdSemiMajorAxis);
            Assert.Equal(3.866, msg.SdSemiMinorAxis);
            Assert.Equal(DateTime.Parse("15.09.2022 6:04:17"), msg.Time);
        }
    }
}