using System;

namespace Asv.Gnss
{
    /// GlonassWord1 class extends GlonassWordBase class and represents a Glonass word with ID 1.
    /// /
    public class GlonassWord1 : GlonassWordBase
    {
        /// <summary>
        /// The ID of the word represented by this object.
        /// </summary>
        /// <remarks>
        /// This property specifies the unique identifier for the word. It is an override of the base class property.
        /// </remarks>
        /// <value>
        /// The ID of the word.
        /// </value>
        public override byte WordId => 1;

        /// <summary>
        /// Deserializes the byte array data into object properties.
        /// </summary>
        /// <param name="data">The byte array data to be deserialized.</param>
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

        /// <summary>
        /// Retrieves the DateTime representation of a given time in hours, minutes, and seconds.
        /// </summary>
        /// <param name="hh">The hour component of the time.</param>
        /// <param name="mm">The minute component of the time.</param>
        /// <param name="ss">The second component of the time.</param>
        /// <returns>The DateTime representation of the given time.</returns>
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

        /// <summary>
        /// Gets or sets the value of property P1.
        /// </summary>
        /// <value>
        /// The value of property P1.
        /// </value>
        /// <remarks>
        /// This property represents a byte value.
        /// </remarks>
        public byte P1 { get; set; }

        /// <summary>
        /// Gets or sets the value of Tk property.
        /// </summary>
        /// <value>
        /// The value of the Tk property.
        /// </value>
        /// <remarks>
        /// The Tk property represents a DateTime value.
        /// </remarks>
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