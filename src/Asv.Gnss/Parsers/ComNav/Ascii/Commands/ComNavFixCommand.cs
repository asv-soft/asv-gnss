using System;
using System.Globalization;

namespace Asv.Gnss
{

    public enum ComNavFixType
    {
        /// <summary>
        /// Unfix. Clears any previous FIX commands
        /// </summary>
        None,
        /// <summary>
        /// Configures the receiver to fix the height at the last calculated value if the number of
        /// satellites available is insufficient for a 3-D solution. This provides a 2-D solution.
        /// Height calculation resumes when the number of satellites available allows a 3-D solution
        /// </summary>
        Auto,
        /// <summary>
        /// Configures the receiver in 2-D mode with its height constrained to a given value. This
        /// command is used mainly in marine applications where height in relation to mean sea
        /// level may be considered to be approximately constant. The height entered using this
        /// command is referenced to the mean sea level, see the BESTPOS log on page 497
        /// (is in metres). The receiver is capable of receiving and applying differential
        /// corrections from a base station while fix height is in effect. The fix height command
        /// overrides any previous FIX HEIGHT or FIX POSITION command.
        /// Note: This command only affects pseudorange corrections and solutions.
        /// </summary>
        Height,
        /// <summary>
        /// Configures the receiver with its position fixed. This command is used when it is
        /// necessary to generate differential corrections.
        /// For both pseudorange and differential corrections, this command must be properly
        /// initialized before the receiver can operate as a GNSS base station. Once initialized,
        /// the receiver computes differential corrections for each satellite being tracked. The
        /// computed differential corrections can then be output to rover stations using the
        /// RTCMV3 differential corrections data log format.S
        /// The values entered into the fix position command should reflect the precise position
        /// of the base station antenna phase center. Any errors in the fix position coordinates
        /// directly bias the corrections calculated by the base receiver
        /// 
        /// The receiver performs all internal computations based on WGS84 and the DATUM
        /// command (see page 131) is defaulted as such. The datum in which you choose to
        /// operate (by changing the DATUM command (see page 131)) is internally converted
        /// to and from WGS84. Therefore, all differential corrections are based on WGS84,
        /// regardless of your operating datum.
        ///
        /// The FIX POSITION command overrides any previous FIX HEIGHT or FIX
        /// POSITION command settings.
        /// </summary>
        Position,

    }

    public class ComNavFixCommand: ComNavAsciiCommandBase
    {
        public ComNavFixType FixType { get; set; }

        protected override string SerializeToAsciiString()
        {
            switch (FixType)
            {
                case ComNavFixType.Auto:
                    return "FIX AUTO";
                case ComNavFixType.Position:
                    return $"FIX POSITION {Lat.ToString(CultureInfo.InvariantCulture)} {Lon.ToString(CultureInfo.InvariantCulture)} {Alt.ToString(CultureInfo.InvariantCulture)} ";
                case ComNavFixType.Height:
                    return $"FIX HEIGHT {Alt.ToString(CultureInfo.InvariantCulture)} ";
                case ComNavFixType.None:
                    return "FIX NONE";
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        public double Lat { get; set; } = Double.NaN;
        public double Lon { get; set; } = Double.NaN;
        public double Alt { get; set; } = Double.NaN;

        public override string MessageId => "FIX";
    }
}