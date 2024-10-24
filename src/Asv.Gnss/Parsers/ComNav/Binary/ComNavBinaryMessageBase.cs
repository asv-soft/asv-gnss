﻿using System;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class ComNavBinaryMessageBase : GnssMessageBase<ushort>
    {
	    /// <summary>
	    /// In current version, the length of header is always 28 bytes.
	    /// </summary>
	    private const int HeaderLength = 28;
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
			
			var messageType = (BinSerialize.ReadByte(ref buffer) >> 7) == 1
				? ComNavMessageTypeEnum.Response
				: ComNavMessageTypeEnum.Original;

			Source = (ComNavPortEnum)BinSerialize.ReadByte(ref buffer);
            MessageLength = BinSerialize.ReadUShort(ref buffer);

            Reserved3 = BinSerialize.ReadUShort(ref buffer); // Sequence 
			Reserved4 = BinSerialize.ReadByte(ref buffer); // Idle time
			TimeStatus = (ComNavTimeStatusEnum)BinSerialize.ReadByte(ref buffer);

			GpsWeek = BinSerialize.ReadUShort(ref buffer);
            GpsMSecs = BinSerialize.ReadUInt(ref buffer);
            GpsTime = GetFromGps(GpsWeek, GpsMSecs / 1000.0);
            UtcTime = Gps2Utc(GpsTime);

            Reserved6 = BinSerialize.ReadUInt(ref buffer); // Receiver Status
            Reserved7 = BinSerialize.ReadUShort(ref buffer);

            ReceiverSwVersion = BinSerialize.ReadUShort(ref buffer);

			// In current version, the length of header is always 28 bytes.
			// But fields may be appended in the future. =>
			buffer = originBuffer.Slice(headerLength);
			
			InternalContentDeserialize(ref buffer);

            var calculatedHash = ComNavCrc32.Calc(originBuffer, headerLength + MessageLength); // 32-bit CRC performed on all data including the header
            var crcStart = originBuffer.Slice(headerLength + MessageLength);
            var readedHash = BinSerialize.ReadUInt(ref crcStart);
            if (calculatedHash != readedHash)
            {
                throw new Exception($"Error to deserialize {ProtocolId}.{Name}: CRC error. Want {calculatedHash}. Got {readedHash}");
            }

            buffer = crcStart;
        }

        public ushort MessageLength { get; set; }

        public ushort Reserved7 { get; set; }
        public uint Reserved6 { get; set; }
        public ComNavTimeStatusEnum TimeStatus { get; set; }
        public byte Reserved4 { get; set; }
        public ushort Reserved3 { get; set; }
        public ComNavPortEnum Source { get; set; }
        public DateTime UtcTime { get; set; }
        /// <summary>
        /// GPS Time
        /// </summary>
        public DateTime GpsTime { get; set; }
        /// <summary>
        /// This is a value (0 - 65535) that represents the receiver software build number
        /// </summary>
        public ushort ReceiverSwVersion { get; set; }
        /// <summary>
        /// Milliseconds from the beginning of the GPS week.
        /// </summary>
        public uint GpsMSecs { get; set; }
        /// <summary>
        /// GPS week number.
        /// </summary>
        public ushort GpsWeek { get; set; }

        protected abstract void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer);
        protected abstract void InternalContentSerialize(ref Span<byte> buffer);
        protected abstract int InternalGetContentByteSize();


        public override void Serialize(ref Span<byte> buffer)
        {
			var originBuffer = buffer;

			BinSerialize.WriteByte(ref buffer, ComNavBinaryParser.FirstSyncByte);
			BinSerialize.WriteByte(ref buffer, ComNavBinaryParser.SecondSyncByte);
			BinSerialize.WriteByte(ref buffer, ComNavBinaryParser.ThirdSyncByte);

			// In current version, the length of header is always 28 bytes.
			BinSerialize.WriteByte(ref buffer, HeaderLength);
			BinSerialize.WriteUShort(ref buffer, MessageId);
			BinSerialize.WriteByte(ref buffer, 2); // Indicate the measurement source
			BinSerialize.WriteByte(ref buffer, (byte)Source);
			MessageLength = (ushort)InternalGetContentByteSize();
			BinSerialize.WriteUShort(ref buffer, MessageLength);
			BinSerialize.WriteUShort(ref buffer, Reserved3);
			BinSerialize.WriteByte(ref buffer, Reserved4);
			BinSerialize.WriteByte(ref buffer, (byte)TimeStatus);

			GpsTime = Utc2Gps(UtcTime);
			var gpsWeek = 0;
			var gpsSecs = 0.0;
			GetFromTime(GpsTime, ref gpsWeek, ref gpsSecs);

			GpsWeek = (ushort)gpsWeek;
			GpsMSecs = (uint)Math.Round(gpsSecs * 1000);
			BinSerialize.WriteUShort(ref buffer, GpsWeek);
			BinSerialize.WriteUInt(ref buffer, GpsMSecs);

			BinSerialize.WriteUInt(ref buffer, Reserved6);
			BinSerialize.WriteUShort(ref buffer, Reserved7);

			BinSerialize.WriteUShort(ref buffer, ReceiverSwVersion);

			// In current version, the length of header is always 28 bytes.
			// But fields may be appended in the future. =>
			// buffer = originBuffer.Slice(HeaderLength);

			InternalContentSerialize(ref buffer);

			var calculatedHash = ComNavCrc32.Calc(originBuffer, HeaderLength + MessageLength); // 32-bit CRC performed on all data including the header
			BinSerialize.WriteUInt(ref buffer, calculatedHash);
        }

		public override int GetByteSize()
        {
            return /*in current version, the length of header is always 28 bytes.*/ HeaderLength + InternalGetContentByteSize() + 4 /*CRC32*/;
        }

        private static DateTime GetFromGps(int weeknumber, double seconds)
        {
            var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);
            var week = datum.AddDays(weeknumber * 7);
            var time = week.AddSeconds(seconds);
            return time;
        }

        private static void GetFromTime(DateTime time, ref int week, ref double seconds)
        {
	        var datum = new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);

	        var dif = time - datum;

	        var weeks = (int)(dif.TotalDays / 7);

	        week = weeks;

	        dif = time - datum.AddDays(weeks * 7);

	        seconds = dif.TotalSeconds;
        }

        private static DateTime Gps2Utc(DateTime t)
        {
            return t.AddSeconds(-LeapSecondsGPS(t.Year, t.Month));
        }

        private static DateTime Utc2Gps(DateTime t)
        {
	        return t.AddSeconds(LeapSecondsGPS(t.Year, t.Month));
        }

        private static int LeapSecondsGPS(int year, int month)
        {
            return LeapSecondsTAI(year, month) - 19;
        }

        private static int LeapSecondsTAI(int year, int month)
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