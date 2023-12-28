using System;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class ComNavBinaryMessageBase : GnssMessageBase<ushort>
    {
        public override string ProtocolId => ComNavBinaryParser.GnssProtocolId;

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            var originBuffer = buffer;

            var sync = BinSerialize.ReadByte(ref buffer);
            if (sync != ComNavBinaryParser.FirstSyncByte) throw new GnssParserException(ProtocolId,$"First sync byte error: want {ComNavBinaryParser.FirstSyncByte}, got {sync}");
            sync = BinSerialize.ReadByte(ref buffer);
            if (sync != ComNavBinaryParser.SecondSyncByte) throw new GnssParserException(ProtocolId, $"Second sync byte error: want {ComNavBinaryParser.SecondSyncByte}, got {sync}");
            sync = BinSerialize.ReadByte(ref buffer);
            if (sync != ComNavBinaryParser.ThirdSyncByte) throw new GnssParserException(ProtocolId, $"Third sync byte error: want {ComNavBinaryParser.ThirdSyncByte}, got {sync}");
            var headerLength = BinSerialize.ReadByte(ref buffer);
            var msgId = BinSerialize.ReadUShort(ref buffer);
            if (msgId != MessageId) throw new GnssParserException(ProtocolId, $"Error to deserialize {ProtocolId} packet: message id not equal (want [{MessageId}] got [{msgId}])");
            Reserved1 = BinSerialize.ReadByte(ref buffer);
            Reserved2 = BinSerialize.ReadByte(ref buffer);
            var messageLength = BinSerialize.ReadUShort(ref buffer);

            Reserved3 = BinSerialize.ReadByte(ref buffer);
            Reserved4 = BinSerialize.ReadByte(ref buffer);
            Reserved5 = BinSerialize.ReadByte(ref buffer);

            GpsWeek = BinSerialize.ReadUShort(ref buffer);
            GpsTime = BinSerialize.ReadUShort(ref buffer);
            TGpsTime = GetFromGps(GpsWeek, GpsTime / 1000.0);
            UtcTime = Gps2Utc(TGpsTime);

            Reserved6 = BinSerialize.ReadByte(ref buffer);
            Reserved7 = BinSerialize.ReadByte(ref buffer);

            ReceiverSwVersion = BinSerialize.ReadUShort(ref buffer);
            
            InternalContentDeserialize(ref buffer);

            var calculatedHash = ComNavCrc32.Calc(originBuffer, headerLength + messageLength); // 32-bit CRC performed on all data including the header
            var crcStart = originBuffer.Slice(headerLength + messageLength);
            var readedHash = BinSerialize.ReadUInt(ref crcStart);
            if (calculatedHash != readedHash)
            {
                throw new Exception($"Error to deserialize {ProtocolId}.{Name}: CRC error. Want {calculatedHash}. Got {readedHash}");
            }

            buffer = crcStart;
        }

        public byte Reserved7 { get; set; }
        public byte Reserved6 { get; set; }
        public byte Reserved5 { get; set; }
        public byte Reserved4 { get; set; }
        public byte Reserved3 { get; set; }
        public byte Reserved2 { get; set; }
        public byte Reserved1 { get; set; }

        public DateTime UtcTime { get; set; }
        /// <summary>
        /// GPS Time (TGPS)
        /// </summary>
        public DateTime TGpsTime { get; set; }
        /// <summary>
        /// This is a value (0 - 65535) that represents the receiver software build number
        /// </summary>
        public ushort ReceiverSwVersion { get; set; }
        /// <summary>
        /// Milliseconds from the beginning of the GPS week.
        /// </summary>
        public uint GpsTime { get; set; }
        /// <summary>
        /// GPS week number.
        /// </summary>
        public ushort GpsWeek { get; set; }

        /// <summary>
         /// Internal method for content deserialization. 
         /// </summary>
         /// <param name="buffer"></param>
         protected abstract void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer);
         
         /// <summary>
         /// Internal method for content serialization.
         /// </summary>
         /// <param name="buffer"></param>
         protected abstract void InternalContentSerialize(ref Span<byte> buffer);
         
         /// <summary>
         /// Internal method for getting byte size of content.
         /// </summary>
         /// <returns>Byte size of the content.</returns>
         protected abstract int InternalGetContentByteSize();
         
         /// <summary>
         /// Overrides the base method for serialization.
         /// </summary>
         /// <param name="buffer"></param>
         public override void Serialize(ref Span<byte> buffer)
         {
             throw new NotImplementedException();
         }
         
         /// <summary>
         /// Overrides the base method for getting byte size.
         /// </summary>
         /// <returns>Byte size of the object.</returns>
         public override int GetByteSize()
         {
             return /*in current version, the length of header is always 28 bytes.*/ 28 + InternalGetContentByteSize() + 4 /*CRC32*/;
         }
         
         /// <summary>
         /// Converts GPS timestamp to DateTime.
         /// </summary>
         /// <param name="weeknumber">Week number.</param>
         /// <param name="seconds">Seconds.</param>
         /// <returns>DateTime.</returns>
         public static DateTime GetFromGps(int weeknumber, double seconds)
         {
             var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
             var week = datum.AddDays(weeknumber * 7);
             var time = week.AddSeconds(seconds);
             return time;
         }
         
         /// <summary>
         /// Converts GPS timestamp to UTC Time.
         /// </summary>
         /// <param name="t">GPS DateTime.</param>
         /// <returns>UTC DateTime.</returns>
         public static DateTime Gps2Utc(DateTime t)
         {
             return t.AddSeconds(-LeapSecondsGPS(t.Year, t.Month));
         }
         
         /// <summary>
         /// Computes Leap seconds for GPS.
         /// </summary>
         /// <param name="year">Year.</param>
         /// <param name="month">Month.</param>
         /// <returns>Number of Leap seconds.</returns>
         public static int LeapSecondsGPS(int year, int month)
         {
             return LeapSecondsTAI(year, month) - 19;
         }
         
         /// <summary>
         /// Computes Leap seconds for TAI system.
         /// </summary>
         /// <param name="year">Year.</param>
         /// <param name="month">Month.</param>
         /// <returns>Number of Leap seconds.</returns>
         public static int LeapSecondsTAI(int year, int month)
         {
             //http://maia.usno.navy.mil/ser7/tai-utc.dat
             var yyyymm = year * 100 + month;
             if (yyyymm >= 201701) return 37;
             if (yyyymm >= 201507) return 36;
             if (yyyymm >= 201207) return 35;
             if (yyyymm >= 200901) return 34;
             if (yyyymm >= 200601) return 33;
             if (yyyymm >= 199901) return 32;
             if (yyyymm >= 199707) return 31;
             if (yyyymm >= 199601) return 30;
             if (yyyymm >= 199407) return 29;
             if (yyyymm >= 199307) return 28;
             if (yyyymm >= 199207) return 27;
             if (yyyymm >= 199101) return 26;
             if (yyyymm >= 199001) return 25;
             if (yyyymm >= 198801) return 24;
             if (yyyymm >= 198507) return 23;
             if (yyyymm >= 198307) return 22;
             if (yyyymm >= 198207) return 21;
             if (yyyymm >= 198107) return 20;
             if (yyyymm >= 0) return 19;
             return 0;
         }
    }
}