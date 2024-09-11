using System;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Xunit;

namespace Asv.Gnss.Test
{
    public class NmeaTests
    {

        private string GetNmeaMessages(Nmea0183MessageBase msg)
        {
            var byteBuff = new byte[1024];
            var byteSpan = new Span<byte>(byteBuff);
            var origSpan = byteSpan;
            msg.Serialize(ref byteSpan);
            var length = origSpan.Length - byteSpan.Length;
            return Encoding.ASCII.GetString(origSpan[..length]);
        }
        
        [Fact]
        public void Parsing_GGA_message_from_string()
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
        public void Serialize_GGA_message_from_byteArray()
        {
            var array = new byte[]
            {
                0x24, 0x47, 0x50, 0x47, 0x47, 0x41, 0x2C, 0x31, 0x32, 0x35, 0x33, 0x31, 0x39, 0x2E, 0x30, 0x30, 0x2C,
                0x35, 0x35, 0x30, 0x38, 0x2E, 0x37, 0x30, 0x32, 0x30, 0x30, 0x39, 0x38, 0x2C, 0x4E, 0x2C, 0x30, 0x36,
                0x31, 0x32, 0x34, 0x2E, 0x33, 0x33, 0x37, 0x38, 0x36, 0x39, 0x38, 0x2C, 0x45, 0x2C, 0x37, 0x2C, 0x30,
                0x38, 0x2C, 0x32, 0x2E, 0x34, 0x2C, 0x32, 0x35, 0x39, 0x2E, 0x30, 0x30, 0x30, 0x30, 0x2C, 0x4D, 0x2C,
                0x2D, 0x31, 0x32, 0x2E, 0x37, 0x39, 0x34, 0x2C, 0x4D, 0x2C, 0x2C, 0x2A, 0x37, 0x36, 0x0D, 0x0A
            };
            Nmea0183MessageGGA msg = null;
            var parser = new Nmea0183Parser().RegisterDefaultMessages();
            parser.OnMessage.Cast<Nmea0183MessageGGA>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }

            var targetMsg = GetNmeaMessages(msg);
            Assert.Equal("$GPGGA,125319,5508.7020098,N,06124.3378697,E,7,08,2.4,259.000,M,-12.794,M,,0000*67\r\n", targetMsg);
        }
        
        
        [Fact]
        public void Parsing_GLL_message_from_string()
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
        public void Parsing_GSV_message_from_string()
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
        public void Parsing_multi_constellation_GSV_message_from_string()
        {
            var source = new[]
            {
                "$GLGSV,3,1,10,81,63,034,51,82,53,272,51,80,52,292,,79,38,200,49*6A\r\n",
                "$GLGSV,3,2,10,65,24,046,42,66,16,105,47,73,15,334,46,88,14,062,46*61\r\n",
                "$GLGSV,3,3,10,83,11,253,,72,07,001,44*68\r\n",
                "$GPGSV,4,1,16,02,80,085,53,11,76,091,,12,61,180,51,25,58,284,52*79\r\n",
                "$GPGSV,4,2,16,20,42,142,50,06,33,056,47,29,28,280,47,05,19,166,47*7F\r\n",
                "$GPGSV,4,3,16,31,15,321,46,19,08,093,46,04,04,025,,09,03,055,*78\r\n",
                "$GPGSV,4,4,16,44,32,184,48,51,31,171,48,48,31,194,47,46,30,199,48*7E\r\n",
                "$GAGSV,3,1,09,34,72,231,53,30,65,251,53,36,51,059,51,02,36,170,49*62\r\n",
                "$GAGSV,3,2,09,27,25,314,47,15,19,236,47,04,08,037,46,09,04,085,*65\r\n",
                "$GAGSV,3,3,09,11,03,057,*50\r\n",
                "$GQGSV,1,1,01,02,08,309,37*4D\r\n",
                "$BDGSV,5,1,18,34,85,015,53,11,67,274,51,12,55,069,49,43,39,265,50*61\r\n",
                "$BDGSV,5,2,18,23,37,289,50,25,36,225,48,44,31,078,49,22,25,064,46*69\r\n",
                "$BDGSV,5,3,18,21,20,119,45,16,11,320,42,06,10,325,40,09,10,340,38*6D\r\n",
                "$BDGSV,5,4,18,39,08,310,40,37,07,331,45,59,06,288,,19,05,017,*64\r\n",
                "$BDGSV,5,5,18,31,03,333,,24,02,179,*68\r\n",
                "$GIGSV,1,1,00,,,,*60\r\n"
            };
            var array = source.Select(_ => Encoding.ASCII.GetBytes(_)).SelectMany(__ => __).ToArray();
            var msgs = new Nmea0183MessageGSV[17];
            var index = 0;
            var parser = new Nmea0183Parser().RegisterDefaultMessages();
            parser.OnMessage.Cast<Nmea0183MessageGSV>().Subscribe(_ =>
            {
                if (index >= 17) return;
                msgs[index++] = _;
            });
            foreach (var p in array)
            {
                parser.Read(p);
            }

            var targetConstellation = new (NmeaNavigationSystemEnum Sys, int?[] SatPrn)[17];
            targetConstellation[0] = (Sys: NmeaNavigationSystemEnum.SYS_GLO, SatPrn: new int?[] { 17, 18, 16, 15 });
            targetConstellation[1] = (Sys: NmeaNavigationSystemEnum.SYS_GLO, SatPrn: new int?[] { 1, 2, 9, 24 });
            targetConstellation[2] = (Sys: NmeaNavigationSystemEnum.SYS_GLO, SatPrn: new int?[] { 19, 8 });
            targetConstellation[3] = (Sys: NmeaNavigationSystemEnum.SYS_GPS, SatPrn: new int?[] { 02, 11, 12, 25 });
            targetConstellation[4] = (Sys: NmeaNavigationSystemEnum.SYS_GPS, SatPrn: new int?[] { 20, 06, 29, 05 });
            targetConstellation[5] = (Sys: NmeaNavigationSystemEnum.SYS_GPS, SatPrn: new int?[] { 31, 19, 04, 09 });
            targetConstellation[6] = (Sys: NmeaNavigationSystemEnum.SYS_SBS, SatPrn: new int?[] { 131, 138, 135, 133 });
            targetConstellation[7] = (Sys: NmeaNavigationSystemEnum.SYS_GAL, SatPrn: new int?[] { 34, 30, 36, 02 });
            targetConstellation[8] = (Sys: NmeaNavigationSystemEnum.SYS_GAL, SatPrn: new int?[] { 27, 15, 04, 09 });
            targetConstellation[9] = (Sys: NmeaNavigationSystemEnum.SYS_GAL, SatPrn: new int?[] { 11 });
            targetConstellation[10] = (Sys: NmeaNavigationSystemEnum.SYS_QZS, SatPrn: new int?[] { 2 });
            targetConstellation[11] = (Sys: NmeaNavigationSystemEnum.SYS_CMP, SatPrn: new int?[] { 34, 11, 12, null });
            targetConstellation[12] = (Sys: NmeaNavigationSystemEnum.SYS_CMP, SatPrn: new int?[] { 23, 25, null, 22 });
            targetConstellation[13] = (Sys: NmeaNavigationSystemEnum.SYS_CMP, SatPrn: new int?[] { 21, 16, 06, 09 });
            targetConstellation[14] = (Sys: NmeaNavigationSystemEnum.SYS_CMP, SatPrn: new int?[] { null, null, null, 19 });
            targetConstellation[15] = (Sys: NmeaNavigationSystemEnum.SYS_CMP, SatPrn: new int?[] { 31, 24 });
            targetConstellation[16] = (Sys: NmeaNavigationSystemEnum.SYS_IRN, SatPrn: Array.Empty<int?>());

            for (var i = 0; i < 17; i++)
            {
                Assert.NotNull(msgs[i]);
                Assert.Equal(targetConstellation[i].SatPrn.Length, msgs[i].Satellites.Length);
                for (var j = 0; j < msgs[i].Satellites.Length; j++)
                {
                    Assert.Equal(targetConstellation[i].SatPrn[j], msgs[i].Satellites[j].ExtPRN);
                    if (msgs[i].Satellites[j].ExtPRN != null)
                        Assert.Equal(targetConstellation[i].Sys, msgs[i].Satellites[j].ExtNavSys);
                }
            }
        }

        [Fact]
        public void Parsing_GST_message_from_string()
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
            Assert.Equal(6,msg.Time?.Hour);
            Assert.Equal(04,msg.Time?.Minute);
            Assert.Equal(17,msg.Time?.Second);
        }
        
        [Fact]
        public void Parsing_NoChecksumm_GSV_message_from_string()
        {
            var source = new[]
            {
                "$GLGSV,3,1,10,81,63,034,51,82,53,272,51,80,52,292,,79,38,200,49\r\n",
                "$GLGSV,3,2,10,65,24,046,42,66,16,105,47,73,15,334,46,88,14,062,46\r\n",
                "$GLGSV,3,3,10,83,11,253,,72,07,001,44\r\n",
                "$GPGSV,4,1,16,02,80,085,53,11,76,091,,12,61,180,51,25,58,284,52\r\n",
                "$GPGSV,4,2,16,20,42,142,50,06,33,056,47,29,28,280,47,05,19,166,47\r\n",
                "$GPGSV,4,3,16,31,15,321,46,19,08,093,46,04,04,025,,09,03,055,\r\n",
                "$GPGSV,4,4,16,44,32,184,48,51,31,171,48,48,31,194,47,46,30,199,48\r\n",
                "$GAGSV,3,1,09,34,72,231,53,30,65,251,53,36,51,059,51,02,36,170,49\r\n",
                "$GAGSV,3,2,09,27,25,314,47,15,19,236,47,04,08,037,46,09,04,085,\r\n",
                "$GAGSV,3,3,09,11,03,057,\r\n",
                "$GQGSV,1,1,01,02,08,309,37\r\n",
                "$BDGSV,5,1,18,34,85,015,53,11,67,274,51,12,55,069,49,43,39,265,50\r\n",
                "$BDGSV,5,2,18,23,37,289,50,25,36,225,48,44,31,078,49,22,25,064,46\r\n",
                "$BDGSV,5,3,18,21,20,119,45,16,11,320,42,06,10,325,40,09,10,340,38\r\n",
                "$BDGSV,5,4,18,39,08,310,40,37,07,331,45,59,06,288,,19,05,017,\r\n",
                "$BDGSV,5,5,18,31,03,333,,24,02,179,\r\n",
                "$GIGSV,1,1,00,,,,\r\n"
            };
            var array = source.Select(_ => Encoding.ASCII.GetBytes(_)).SelectMany(__ => __).ToArray();
            var msgs = new Nmea0183MessageGSV[17];
            var index = 0;
            var parser = new Nmea0183Parser().RegisterDefaultMessages();
            parser.OnMessage.Cast<Nmea0183MessageGSV>().Subscribe(_ =>
            {
                if (index >= 17) return;
                msgs[index++] = _;
            });
            foreach (var p in array)
            {
                parser.Read(p);
            }

            var targetConstellation = new (NmeaNavigationSystemEnum Sys, int?[] SatPrn)[17];
            targetConstellation[0] = (Sys: NmeaNavigationSystemEnum.SYS_GLO, SatPrn: new int?[] { 17, 18, 16, 15 });
            targetConstellation[1] = (Sys: NmeaNavigationSystemEnum.SYS_GLO, SatPrn: new int?[] { 1, 2, 9, 24 });
            targetConstellation[2] = (Sys: NmeaNavigationSystemEnum.SYS_GLO, SatPrn: new int?[] { 19, 8 });
            targetConstellation[3] = (Sys: NmeaNavigationSystemEnum.SYS_GPS, SatPrn: new int?[] { 02, 11, 12, 25 });
            targetConstellation[4] = (Sys: NmeaNavigationSystemEnum.SYS_GPS, SatPrn: new int?[] { 20, 06, 29, 05 });
            targetConstellation[5] = (Sys: NmeaNavigationSystemEnum.SYS_GPS, SatPrn: new int?[] { 31, 19, 04, 09 });
            targetConstellation[6] = (Sys: NmeaNavigationSystemEnum.SYS_SBS, SatPrn: new int?[] { 131, 138, 135, 133 });
            targetConstellation[7] = (Sys: NmeaNavigationSystemEnum.SYS_GAL, SatPrn: new int?[] { 34, 30, 36, 02 });
            targetConstellation[8] = (Sys: NmeaNavigationSystemEnum.SYS_GAL, SatPrn: new int?[] { 27, 15, 04, 09 });
            targetConstellation[9] = (Sys: NmeaNavigationSystemEnum.SYS_GAL, SatPrn: new int?[] { 11 });
            targetConstellation[10] = (Sys: NmeaNavigationSystemEnum.SYS_QZS, SatPrn: new int?[] { 2 });
            targetConstellation[11] = (Sys: NmeaNavigationSystemEnum.SYS_CMP, SatPrn: new int?[] { 34, 11, 12, null });
            targetConstellation[12] = (Sys: NmeaNavigationSystemEnum.SYS_CMP, SatPrn: new int?[] { 23, 25, null, 22 });
            targetConstellation[13] = (Sys: NmeaNavigationSystemEnum.SYS_CMP, SatPrn: new int?[] { 21, 16, 06, 09 });
            targetConstellation[14] = (Sys: NmeaNavigationSystemEnum.SYS_CMP, SatPrn: new int?[] { null, null, null, 19 });
            targetConstellation[15] = (Sys: NmeaNavigationSystemEnum.SYS_CMP, SatPrn: new int?[] { 31, 24 });
            targetConstellation[16] = (Sys: NmeaNavigationSystemEnum.SYS_IRN, SatPrn: Array.Empty<int?>());

            for (var i = 0; i < 17; i++)
            {
                Assert.NotNull(msgs[i]);
                Assert.Equal(targetConstellation[i].SatPrn.Length, msgs[i].Satellites.Length);
                for (var j = 0; j < msgs[i].Satellites.Length; j++)
                {
                    Assert.Equal(targetConstellation[i].SatPrn[j], msgs[i].Satellites[j].ExtPRN);
                    if (msgs[i].Satellites[j].ExtPRN != null)
                        Assert.Equal(targetConstellation[i].Sys, msgs[i].Satellites[j].ExtNavSys);
                }
            }
        }
        
        [Fact]
        public void Serialize_RMC_message_from_byteArray()
        {
            var array = new byte[]
            {
                0x24, 0x47, 0x50, 0x52, 0x4D, 0x43, 0x2C, 0x31, 0x32, 0x33, 0x35, 0x31, 0x39, 0x2C, 0x41, 0x2C, 0x34,
                0x38, 0x30, 0x37, 0x2E, 0x30, 0x33, 0x38, 0x2C, 0x4E, 0x2C, 0x30, 0x31, 0x31, 0x33, 0x31, 0x2E, 0x30,
                0x30, 0x30, 0x2C, 0x45, 0x2C, 0x30, 0x32, 0x32, 0x2E, 0x34, 0x2C, 0x30, 0x38, 0x34, 0x2E, 0x34, 0x2C,
                0x32, 0x33, 0x30, 0x33, 0x39, 0x34, 0x2C, 0x30, 0x30, 0x33, 0x2E, 0x31, 0x2C, 0x57, 0x2A, 0x36, 0x41,
                0x0D, 0x0A
            };
            Nmea0183MessageRMC msg = null;
            var parser = new Nmea0183Parser().RegisterDefaultMessages();
            parser.OnMessage.Cast<Nmea0183MessageRMC>().Subscribe(_ => msg = _);
            foreach (var p in array)
            {
                parser.Read(p);
            }

            var targetMsg = GetNmeaMessages(msg);
            Assert.Equal("$GPRMC,123519,A,4807.0379999,N,01131.00,E,022.4,084.4,230394,003.1,W*55\r\n", targetMsg);
        }
    }
}