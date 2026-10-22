using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss
{
    public partial class RtcmV3Message1029 : RtcmV3LegacyMessageBase
    {
        public const int RtcmMessageId = 1029;
        public override ushort MessageId => RtcmMessageId;
        public override string Name => "Unicode Text String";
        protected override void InternalSerialize(Span<byte> buffer, ref int bitIndex)
        {
            var bytes = Encoding.UTF8.GetBytes(Message ?? string.Empty);
            var characterCount = 0;
            foreach (var rune in (Message ?? string.Empty).EnumerateRunes()) characterCount++;
            if (characterCount > 127 || bytes.Length > 255)
                throw new ArgumentOutOfRangeException(nameof(Message));
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 12, ReferenceStationID);
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 16, ModifiedJulianDay);
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 17, SecondsOfDay);
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 7, characterCount);
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 8, bytes.Length);
            foreach (var value in bytes) SpanBitHelper.SetBitU(buffer, ref bitIndex, 8, value);
        }

        protected override int InternalGetBitSize() => 60 + Encoding.UTF8.GetByteCount(Message ?? string.Empty) * 8;

        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            ReferenceStationID = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 12); 
            ModifiedJulianDay = (ushort)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 16);
            var secondsOfDDay = SecondsOfDay = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 17);
            NumberOfCharactersFollow = SpanBitHelper.GetBitU(buffer, ref bitIndex, 7);
            var dateTime = RtcmV3Helper.GetUtc(DateTime.UtcNow, secondsOfDDay);
            EpochTime = RtcmV3Helper.Utc2Gps(dateTime);

            var codeUnitsCount = SpanBitHelper.GetBitU(buffer, ref  bitIndex, 8);
            var buff = new byte[codeUnitsCount];
            for (var i = 0; i < codeUnitsCount; i++)
            {
                buff[i] = (byte)SpanBitHelper.GetBitU(buffer, ref  bitIndex, 8);
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

        public uint SecondsOfDay { get; set; }

        public ushort ModifiedJulianDay { get; set; }


        public uint ReferenceStationID { get; set; }

        public string Message { get; set; }

        public DateTime EpochTime { get; set; }

        
    }
}
