using System;

namespace Asv.Gnss
{
    public class GlonassWord2 : GlonassWordBase
    {
        public override byte WordId => 2;

        public override void Deserialize(byte[] data)
        {
            base.Deserialize(data);
            var bitIndex = 8U;

            Bn = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 3); bitIndex += 3;
            P2 = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 1); bitIndex += 1;
            TbRaw = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 7); bitIndex += 7 + 5;
            Tb = GetDateTime(TbRaw);
            
            // VelocityY = GlonassRawHelper.GetBitU(data, bitIndex, 24) * GlonassRawHelper.P2_20 * 1E3; bitIndex += 24;
            // AccelerationY = GlonassRawHelper.GetBitU(data, bitIndex, 5) * GlonassRawHelper.P2_30 * 1E3; bitIndex += 5;
            // PositionY = GlonassRawHelper.GetBitU(data, bitIndex, 27) * GlonassRawHelper.P2_11 * 1E3; bitIndex += 27;
        }

        private DateTime GetDateTime(uint tb)
        {
            var utc = DateTime.UtcNow;
            var week = 0;
            var tow = 0.0;
            RtcmV3Helper.GetFromTime(utc, ref week, ref tow);
            var toe = tb * 900.0 - 10800.0; /* lt->utc */
            var tod = tow % 86400.0;
            tow -= tod;

            if (toe < tod - 43200.0) toe += 86400.0;
            else if (toe > tod + 43200.0) toe -= 86400.0;
            return RtcmV3Helper.GetFromGps(week, tow + toe).AddHours(3.0);
        }

        public byte TbRaw { get; set; }

        public DateTime Tb { get; set; }

        public byte Bn { get; set; }

        public byte P2 { get; set; }
        /// <summary>
        /// satellite position (ecef) (m). Координаты n-го спутника в системе координат ПЗ-90 на момент времени tb
        /// </summary>
        // public double PositionY { get; set; }

        /// <summary>
        /// satellite velocity (ecef) (m/s). составляющие вектора скорости n-го спутника в системе координат ПЗ-90 на момент
        /// времени tb
        /// </summary>
        // public double VelocityY { get; set; }

        /// <summary>
        /// satellite acceleration (ecef) (m/s^2). Составляющие ускорения n-го спутника в системе координат ПЗ-90 на момент времени tb,
        /// обусловленные действием луны и солнца
        /// </summary>
        // public double AccelerationY { get; set; }

    }
}