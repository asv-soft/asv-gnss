using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class DObservationItem 
    {
        private readonly NavigationSystemEnum _system;

        private DateTime GetDateTime(uint tb)
        {
            var utc = DateTime.UtcNow;
            var week = 0;
            var tow = 0.0;
            RtcmV3Helper.GetFromTime(utc, ref week, ref tow);
            var toe = (double)tb; /* lt->utc */
            var toh = tow % 3600.0;
            tow -= toh;

            if (toe < toh - 1800.0) toe += 3600.0;
            else if (toe > toh + 1800.0) toe -= 3600.0;
            return RtcmV3Helper.GetFromGps(week, tow + toe).AddHours(3.0);
        }

        public DObservationItem(NavigationSystemEnum system)
        {
            _system = system;
        }

        public void Deserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
        {
            var fact = (byte)SpanBitHelper.GetBitU(buffer,ref bitIndex, 1);
            var udre = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
            var prn = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
            var prc = SpanBitHelper.GetBitS(buffer,ref bitIndex, 16);
            var rrc = SpanBitHelper.GetBitS(buffer,ref bitIndex, 8);
            var iod = (byte)SpanBitHelper.GetBitU(buffer,ref bitIndex, 8);

            if (prn == 0) prn = 32;

            Prn = prn;

            if (prc == -32_768 || rrc == -128)
            {
                Prc = double.NaN;
                Rrc = double.NaN;
            }
            else
            {
                Prc = prc * (fact == 1 ? 0.32 : 0.02);
                Rrc = rrc * (fact == 1 ? 0.032 : 0.002);
            }
            SatelliteId = RtcmV3Helper.satno(_system, prn);
            Iod = _system == NavigationSystemEnum.SYS_GLO ? (byte)(iod & 0x7F) : iod;
            if (_system == NavigationSystemEnum.SYS_GLO)
                Tk = GetDateTime((uint)(Iod * 30));
            Udre = GetUdre(udre);
        }

        
        private SatUdreEnum GetUdre(byte udre)
        {
            return udre switch
            {
                0 => SatUdreEnum.LessOne,
                1 => SatUdreEnum.BetweenOneAndFour,
                2 => SatUdreEnum.BetweenFourAndEight,
                3 => SatUdreEnum.MoreEight,
                _ => throw new ArgumentOutOfRangeException(nameof(udre))
            };
        }

        public int SatelliteId { get; set; }

        public byte Prn { get; set; }

        public double Prc { get; set; }

        public double Rrc { get; set; }

        public byte Iod { get; set; }

        public SatUdreEnum Udre { get; set; }

        public DateTime Tk { get; set; }
    }
}