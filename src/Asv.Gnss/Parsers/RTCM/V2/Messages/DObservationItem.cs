using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a single observation item.
    /// </summary>
    public class DObservationItem
    {
        /// <summary>
        /// Determines the navigation system used by the software.
        /// </summary>
        private readonly NavigationSystemEnum _system;

        /// <summary>
        /// Calculates the DateTime based on the given time value.
        /// </summary>
        /// <param name="tb">The time value.</param>
        /// <returns>The calculated DateTime.</returns>
        private DateTime GetDateTime(uint tb)
        {
            var utc = DateTime.UtcNow;
            var week = 0;
            var tow = 0.0;
            RtcmV3Helper.GetFromTime(utc, ref week, ref tow);
            var toe = (double)tb; /* lt->utc */
            var toh = tow % 3600.0;
            tow -= toh;

            if (toe < toh - 1800.0)
                toe += 3600.0;
            else if (toe > toh + 1800.0)
                toe -= 3600.0;
            return RtcmV3Helper.GetFromGps(week, tow + toe).AddHours(3.0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DObservationItem"/> class with the specified navigation system.
        /// </summary>
        /// <param name="system">The navigation system type.</param>
        public DObservationItem(NavigationSystemEnum system)
        {
            _system = system;
        }

        /// Deserializes data from a buffer and updates the state of the object.
        /// @param buffer The buffer containing the serialized data.
        /// @param bitIndex The index of the current bit in the buffer. This is an input/output parameter that is used to keep track of the current bit position in the buffer.
        /// @remarks This method reads and interprets data from the specified buffer to update the state of the object. The buffer contains serialized data in a specific format, which is des
        /// erialized and assigned to the appropriate fields and properties of the object.
        /// /
        public void Deserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
        {
            var fact = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            var udre = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
            var prn = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
            var prc = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16);
            var rrc = SpanBitHelper.GetBitS(buffer, ref bitIndex, 8);
            var iod = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);

            if (prn == 0)
                prn = 32;

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

        /// <summary>
        /// Converts a byte value representing UDRE (User Differential Range Error) to the corresponding SatUdreEnum value.
        /// </summary>
        /// <param name="udre">The byte value representing UDRE.</param>
        /// <returns>The SatUdreEnum value corresponding to the given UDRE value.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the given UDRE value is not within the valid range of 0 to 3.</exception>
        private SatUdreEnum GetUdre(byte udre)
        {
            return udre switch
            {
                0 => SatUdreEnum.LessOne,
                1 => SatUdreEnum.BetweenOneAndFour,
                2 => SatUdreEnum.BetweenFourAndEight,
                3 => SatUdreEnum.MoreEight,
                _ => throw new ArgumentOutOfRangeException(nameof(udre)),
            };
        }

        /// <summary>
        /// Gets or sets the satellite ID.
        /// </summary>
        /// <value>
        /// The satellite ID.
        /// </value>
        public int SatelliteId { get; set; }

        /// <summary>
        /// Gets or sets the Prn property.
        /// </summary>
        /// <value>
        /// The Prn property.
        /// </value>
        public byte Prn { get; set; }

        /// <summary>
        /// Gets or sets the value of the property Prc.
        /// </summary>
        /// <remarks>
        /// This property represents a double value.
        /// </remarks>
        public double Prc { get; set; }

        /// <summary>
        /// Gets or sets the value of Rrc.
        /// </summary>
        /// <remarks>
        /// The Rrc property represents a double value.
        /// </remarks>
        public double Rrc { get; set; }

        /// <summary>
        /// Gets or sets the Iod property.
        /// </summary>
        /// <value>
        /// The Iod property.
        /// </value>
        public byte Iod { get; set; }

        /// <summary>
        /// Gets or sets the Udre property.
        /// </summary>
        /// <value>The Udre property.</value>
        public SatUdreEnum Udre { get; set; }

        /// <summary>
        /// Represents the Tk property. </summary> <value>
        /// Gets or sets the value of Tk property. </value> <remarks>
        /// This property is of type DateTime and it represents the Tk value. </remarks>
        /// /
        public DateTime Tk { get; set; }
    }
}
