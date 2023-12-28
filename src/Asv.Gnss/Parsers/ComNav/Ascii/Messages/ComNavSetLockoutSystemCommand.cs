using System;

namespace Asv.Gnss
{

    public enum ComNavSatelliteSystemEnum
    {
        GPS = 0,
        GLONASS = 1,
        SBAS = 2,
        Galileo = 5,
        BeiDou = 6,
        QZSS = 7,
        NavIC = 9,
    }
    /// <summary>
    /// Prevents the receiver from using a system
    /// This command is used to prevent the receiver from using all satellites in a system in the solution computations.
    /// </summary>
    public class ComNavSetLockoutSystemCommand:ComNavAsciiCommandBase
    {
        /// Gets or sets the satellite system of the property.
        public ComNavSatelliteSystemEnum SatelliteSystem { get; set; }

        /// <summary>
        /// Serializes the satellite system information to an ASCII string representation.
        /// </summary>
        /// <returns>
        /// The ASCII string representation of the satellite system information.
        /// </returns>
        protected override string SerializeToAsciiString()
        {
            return SatelliteSystem switch
            {
                ComNavSatelliteSystemEnum.GPS => "LOCKOUTSYSTEM GPS",
                ComNavSatelliteSystemEnum.GLONASS => "LOCKOUTSYSTEM GLONASS",
                ComNavSatelliteSystemEnum.SBAS => "LOCKOUTSYSTEM SBAS",
                ComNavSatelliteSystemEnum.Galileo => "LOCKOUTSYSTEM Galileo",
                ComNavSatelliteSystemEnum.BeiDou => "LOCKOUTSYSTEM BeiDou",
                ComNavSatelliteSystemEnum.QZSS => "LOCKOUTSYSTEM QZSS",
                ComNavSatelliteSystemEnum.NavIC => "LOCKOUTSYSTEM NavIC",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Gets the unique identifier for the message.
        /// </summary>
        public override string MessageId => "LOCKOUTSYSTEM";
    }

}