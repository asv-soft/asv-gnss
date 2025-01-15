using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss
{
    public class RtcmV3Message1029 : RtcmV3MessageBase
    {
        public const int RtcmMessageId = 1029;
        public override ushort MessageId => RtcmMessageId;
        public override string Name => "Unicode Text String";

        protected override void DeserializeContent(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            int messageLength
        )
        {
            ReferenceStationID = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);
            ModifiedJulianDay = (ushort)SpanBitHelper.GetBitU(buffer, ref bitIndex, 16);
            var secondsOfDDay = SpanBitHelper.GetBitU(buffer, ref bitIndex, 17);
            NumberOfCharactersFollow = SpanBitHelper.GetBitU(buffer, ref bitIndex, 7);
            var dateTime = RtcmV3Helper.GetUtc(DateTime.UtcNow, secondsOfDDay);
            EpochTime = RtcmV3Helper.Utc2Gps(dateTime);

            var codeUnitsCount = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            var buff = new byte[codeUnitsCount];
            for (var i = 0; i < codeUnitsCount; i++)
            {
                buff[i] = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            }

            Message = new string(Encoding.UTF8.GetChars(buff));
        }

        /// <summary>
        /// This represents the number of fully formed Unicode
        /// characters in the message text. It is not necessarily
        /// the number of bytes that are needed to represent the
        /// characters as UTF-8. Note that for some messages it
        /// may not be possible to utilize the full range of this
        /// field, e.g. where many characters require 3 or 4 byte
        /// representations and together will exceed 255 code
        /// units
        /// </summary>
        public uint NumberOfCharactersFollow { get; set; }

        public ushort ModifiedJulianDay { get; set; }

        public uint ReferenceStationID { get; set; }

        public string Message { get; set; }

        public DateTime EpochTime { get; set; }
    }
}
