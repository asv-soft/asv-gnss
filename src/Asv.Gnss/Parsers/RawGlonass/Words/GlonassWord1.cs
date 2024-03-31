using System;

namespace Asv.Gnss
{
    public class GlonassWord1 : GlonassWordBase
    {
        public override byte WordId => 1;

        public override void Deserialize(byte[] data)
        {
            base.Deserialize(data);
            var bitIndex = 8U + 2U;
            P1 = (byte)GlonassRawHelper.GetBitU(data, bitIndex + 2, 2); bitIndex += 2;
            var hh = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 5); bitIndex += 5;
            var mm = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 6); bitIndex += 6;
            var ss = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 1) * 30; bitIndex += 1;
            
            VelocityX = GlonassRawHelper.GetBitG(data, bitIndex, 24) * GlonassRawHelper.P2_20 * 1E3; bitIndex += 24;
            AccelerationX = GlonassRawHelper.GetBitG(data, bitIndex, 5) * GlonassRawHelper.P2_30 * 1E3; bitIndex += 5;
            PositionX = GlonassRawHelper.GetBitG(data, bitIndex, 27) * GlonassRawHelper.P2_11 * 1E3; bitIndex += 27;
            TofLocalSec = hh * 3600.0 + mm * 60.0 + ss;
        }

        /// <summary>
        /// The number of seconds elapsed since the beginning of the current day
        /// </summary>
        public double TofLocalSec { get; set; }

        
        public byte P1 { get; set; }
        

        /// <summary>
        /// satellite position (ecef) (m). Координаты n-го спутника в системе координат ПЗ-90 на момент времени tb
        /// </summary>
        public double PositionX { get; set; }

        /// <summary>
        /// satellite velocity (ecef) (m/s). составляющие вектора скорости n-го спутника в системе координат ПЗ-90 на момент
        /// времени tb
        /// </summary>
        public double VelocityX { get; set; }

        /// <summary>
        /// satellite acceleration (ecef) (m/s^2). Составляющие ускорения n-го спутника в системе координат ПЗ-90 на момент времени tb,
        /// обусловленные действием луны и солнца
        /// </summary>
        public double AccelerationX { get; set; }
    }
}