using System;

namespace Asv.Gnss
{
    public class GlonassWord1 : GlonassWordBase
    {
        public override byte WordId => 1;

        public override void Deserialize(byte[] data)
        {
            base.Deserialize(data);
            var bitIndex = 8U;
            P1 = (byte)GlonassRawHelper.GetBitU(data, bitIndex + 2, 2); bitIndex += 4;
            
            var hh = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 5); bitIndex += 5;
            var mm = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 6); bitIndex += 6;
            var ss = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 1) * 30; bitIndex += 1;
            Tk = GetDateTime(hh, mm, ss);

            // VelocityX = GlonassRawHelper.GetBitG(data, bitIndex, 24) * GlonassRawHelper.P2_20 * 1E3; bitIndex += 24;
            // AccelerationX = GlonassRawHelper.GetBitG(data, bitIndex, 5) * GlonassRawHelper.P2_30 * 1E3; bitIndex += 5;
            // PositionX = GlonassRawHelper.GetBitG(data, bitIndex, 27) * GlonassRawHelper.P2_11 * 1E3; bitIndex += 27;


        }

        private DateTime GetDateTime(byte hh, byte mm, int ss)
        {
            var utc = DateTime.UtcNow;
            var week = 0;
            var tow = 0.0;
            GlonassRawHelper.GetFromTime(utc, ref week, ref tow);

            var tod = tow % 86400.0;
            tow -= tod;
            var tof = hh * 3600.0 + mm * 60.0 + ss - 10800.0; /* lt->utc */

            if (tof < tod - 43200.0) tof += 86400.0;
            else if (tof > tod + 43200.0) tof -= 86400.0;

            return GlonassRawHelper.GetFromUtc(week, tow + tof).AddHours(3.0);
        }

        public byte P1 { get; set; }

        public DateTime Tk { get; set; }

        /// <summary>
        /// satellite position (ecef) (m). Координаты n-го спутника в системе координат ПЗ-90 на момент времени tb
        /// </summary>
        // public double PositionX { get; set; }

        /// <summary>
        /// satellite velocity (ecef) (m/s). составляющие вектора скорости n-го спутника в системе координат ПЗ-90 на момент
        /// времени tb
        /// </summary>
        // public double VelocityX { get; set; }

        /// <summary>
        /// satellite acceleration (ecef) (m/s^2). Составляющие ускорения n-го спутника в системе координат ПЗ-90 на момент времени tb,
        /// обусловленные действием луны и солнца
        /// </summary>
        // public double AccelerationX { get; set; }
    }
}