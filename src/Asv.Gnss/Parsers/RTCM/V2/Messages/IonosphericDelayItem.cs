using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents an ionospheric delay item for a specific navigation system and satellite.
    /// </summary>
    public class IonosphericDelayItem
    {
        /// <summary>
        /// Deserializes the given buffer and populates the necessary properties.
        /// </summary>
        /// <param name="buffer">The byte buffer from which to deserialize data.</param>
        /// <param name="bitIndex">The bit index used to track the current bit being read.</param>
        public void Deserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
        {
            var sys = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            NavigationSystem =
                sys == 0 ? NavigationSystemEnum.SYS_GPS : NavigationSystemEnum.SYS_GLO;
            Prn = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
            if (Prn == 0)
            {
                Prn = 32;
            }

            IonosphericDelay = SpanBitHelper.GetBitU(buffer, ref bitIndex, 14) * 0.001;
            var rateOfChange = SpanBitHelper.GetBitS(buffer, ref bitIndex, 14);

            if (rateOfChange == -8192)
            {
                IonosphericDelay = double.NaN;
                IonoRateOfChange = double.NaN;
            }
            else
            {
                IonoRateOfChange = rateOfChange * 0.05;
            }
        }

        /// <summary>
        /// Gets or sets the navigation system for the application.
        /// </summary>
        /// <value>
        /// The navigation system for the application.
        /// </value>
        public NavigationSystemEnum NavigationSystem { get; set; }

        /// <summary>
        /// Gets or sets the Prn property.
        /// </summary>
        public byte Prn { get; set; }

        /// <summary>
        /// Gets or sets the ionospheric delay in centimeters.
        /// </summary>
        public double IonosphericDelay { get; set; }

        /// <summary>
        /// Gets or sets the rate of change of the ionosphere measuring unit in cm/min.
        /// </summary>
        public double IonoRateOfChange { get; set; }
    }
}
