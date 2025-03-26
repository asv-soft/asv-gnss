using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Asv.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Asv.Gnss
{
    public enum ComNavMessageTypeEnum : byte
    {
        Original = 0,
        Response = 1,
    }

    public enum ComNavPortEnum : uint
    {
        NO_PORTS,
        COM1,
        COM2,
        COM3,
        SPECIAL,
        USB,
        GPRS,
        COM4,
    }

    public enum ComNavTimeStatusEnum : byte
    {
        /// <summary>
        ///  Time validity is unknown.
        /// </summary>
        UNKNOWN,

        /// <summary>
        ///  Time is set approximately.
        /// </summary>
        APPROXIMATE,

        /// <summary>
        ///  Time is approaching coarse precision.
        /// </summary>
        COARSEADJUSTING,

        /// <summary>
        ///  This time is valid to coarse precision.
        /// </summary>
        COARSE,

        /// <summary>
        ///  Time is coarse set and is being steered.
        /// </summary>
        COARSESTEERING,

        /// <summary>
        ///  Position is lost and the range bias cannot be calculated.
        /// </summary>
        FREEWHEELING,

        /// <summary>
        ///  Time is adjusting to fine precision.
        /// </summary>
        FINEADJUSTING,

        /// <summary>
        ///  Time has fine precision.
        /// </summary>
        FINE,

        /// <summary>
        ///  Time is fine set and is being steered by the backup system.
        /// </summary>
        FINEBACKUPSTEERING,

        /// <summary>
        ///  Time is fine set and is being steered.
        /// </summary>
        FINESTEERING,

        /// <summary>
        ///  Time from satellite. Only used in logs containing satellite data such as ephemeris and almanac.
        /// </summary>
        SATTIME,
    }

    public abstract class ComNavAsciiMessageBase : GnssMessageBase<string>
    {
        public override string ProtocolId => ComNavAsciiParser.GnssProtocolId;

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            var sync = BinSerialize.ReadByte(ref buffer);
            if (sync != ComNavAsciiParser.FirstSyncByte)
            {
                throw new GnssParserException(
                    ProtocolId,
                    $"First sync byte error: want {ComNavAsciiParser.FirstSyncByte}, got {sync}"
                );
            }

            var originBuffer = buffer;

            var msg = Encoding
                .ASCII.GetString(buffer.ToArray(), 0, buffer.Length - 5)
                .Split(Encoding.ASCII.GetChars(new[] { ComNavAsciiParser.HeaderSeparator }));

            if (msg.Length != 2)
            {
                throw new GnssParserException(
                    ProtocolId,
                    $"Error to deserialize {ProtocolId}:{MessageId} packet"
                );
            }

            var header = msg[0]
                .Split(Encoding.ASCII.GetChars(new[] { ComNavAsciiParser.Separator }));
            var payload = msg[1]
                .Split(Encoding.ASCII.GetChars(new[] { ComNavAsciiParser.Separator }));

            if (header.Length != 10)
            {
                throw new GnssParserException(
                    ProtocolId,
                    $"Error to deserialize {ProtocolId}:{MessageId} packet"
                );
            }

            if (header[0] != MessageId)
            {
                throw new GnssParserException(
                    ProtocolId,
                    $"Error to deserialize {ProtocolId} packet: message id not equal (want [{MessageId}] got [{header[0]}])"
                );
            }
#if NETFRAMEWORK
            Source = Enum.TryParse(header[1], true, out ComNavPortEnum source)
                ? source
                : ComNavPortEnum.NO_PORTS;
#else
            if (Enum.TryParse(typeof(ComNavPortEnum), header[1], true, out var source))
            {
                Source = (ComNavPortEnum)source;
            }
            else
            {
                Source = ComNavPortEnum.NO_PORTS;
            }
#endif
            var headerLength = header.Sum(_ => _.Length) + header.Length - 1;
            MessageLength =
                payload.Length != 0 ? (payload.Sum(_ => _.Length) + payload.Length - 1) : 0;

#if NETFRAMEWORK
            TimeStatus = Enum.TryParse(header[4], true, out ComNavTimeStatusEnum timeStatus)
                ? timeStatus
                : ComNavTimeStatusEnum.UNKNOWN;
#else
            if (Enum.TryParse(typeof(ComNavTimeStatusEnum), header[4], true, out var timeStatus))
            {
                TimeStatus = (ComNavTimeStatusEnum)timeStatus;
            }
            else
            {
                TimeStatus = ComNavTimeStatusEnum.UNKNOWN;
            }
#endif

            if (!uint.TryParse(header[5], out var gpsWeek))
            {
                gpsWeek = 0;
            }

            if (
                !double.TryParse(
                    header[6],
                    NumberStyles.Any,
                    NumberFormatInfo.InvariantInfo,
                    out var gpsSecs
                )
            )
            {
                gpsSecs = 0.0;
            }

            GpsTime = GetFromGps((int)gpsWeek, gpsSecs);
            UtcTime = Gps2Utc(GpsTime);

            InternalContentDeserialize(payload);

            var calculatedHash = ComNavCrc32.Calc(originBuffer, headerLength + 1 + MessageLength); // 32-bit CRC performed on all data including the header
            var crcStart = buffer.Slice(headerLength + 1 + MessageLength + 1);
            var readedHash = BinSerialize.ReadUInt(ref crcStart);
            if (calculatedHash != readedHash)
            {
                throw new Exception(
                    $"Error to deserialize {ProtocolId}.{Name}: CRC error. Want {calculatedHash}. Got {readedHash}"
                );
            }

            buffer = crcStart;
        }

        protected abstract void InternalContentDeserialize(string[] msg);

        [JsonConverter(typeof(StringEnumConverter))]
        public ComNavPortEnum Source { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ComNavTimeStatusEnum TimeStatus { get; set; }

        public DateTime GpsTime { get; set; }

        public DateTime UtcTime { get; set; }

        public int MessageLength { get; set; }

        public override void Serialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public override int GetByteSize()
        {
            throw new NotImplementedException();
        }

        private static DateTime GetFromGps(int weeknumber, double seconds)
        {
            var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            var week = datum.AddDays(weeknumber * 7);
            var time = week.AddSeconds(seconds);
            return time;
        }

        private static DateTime Gps2Utc(DateTime t)
        {
            return t.AddSeconds(-LeapSecondsGPS(t.Year, t.Month));
        }

        private static int LeapSecondsGPS(int year, int month)
        {
            return LeapSecondsTAI(year, month) - 19;
        }

        private static int LeapSecondsTAI(int year, int month)
        {
            // http://maia.usno.navy.mil/ser7/tai-utc.dat
            var yyyymm = (year * 100) + month;
            if (yyyymm >= 201701)
            {
                return 37;
            }

            if (yyyymm >= 201507)
            {
                return 36;
            }

            if (yyyymm >= 201207)
            {
                return 35;
            }

            if (yyyymm >= 200901)
            {
                return 34;
            }

            if (yyyymm >= 200601)
            {
                return 33;
            }

            if (yyyymm >= 199901)
            {
                return 32;
            }

            if (yyyymm >= 199707)
            {
                return 31;
            }

            if (yyyymm >= 199601)
            {
                return 30;
            }

            if (yyyymm >= 199407)
            {
                return 29;
            }

            if (yyyymm >= 199307)
            {
                return 28;
            }

            if (yyyymm >= 199207)
            {
                return 27;
            }

            if (yyyymm >= 199101)
            {
                return 26;
            }

            if (yyyymm >= 199001)
            {
                return 25;
            }

            if (yyyymm >= 198801)
            {
                return 24;
            }

            if (yyyymm >= 198507)
            {
                return 23;
            }

            if (yyyymm >= 198307)
            {
                return 22;
            }

            if (yyyymm >= 198207)
            {
                return 21;
            }

            if (yyyymm >= 198107)
            {
                return 20;
            }

            if (yyyymm >= 0)
            {
                return 19;
            }

            return 0;
        }
    }
}
