using System;

namespace Asv.Gnss
{

    public enum ComNavSatelliteSystemEnum
    {
        GPS = 0,
        BD2 = 4,
        GLONASS = 1,
        GALILEO = 3,
        BD3 = 7
    }
    /// <summary>
    /// Prevents the receiver from using a system
    /// This command is used to prevent the receiver from using all satellites in a system in the solution computations.
    /// </summary>
    public class ComNavSetLockoutSystemCommand:ComNavAsciiCommandBase
    {
        public ComNavSatelliteSystemEnum SatelliteSystem { get; set; }

        protected override string SerializeToAsciiString()
        {
            return SatelliteSystem switch
            {
                ComNavSatelliteSystemEnum.GPS => "LOCKOUTSYSTEM GPS",
                ComNavSatelliteSystemEnum.BD2 => "LOCKOUTSYSTEM BD2",
                ComNavSatelliteSystemEnum.GLONASS => "LOCKOUTSYSTEM GLONASS",
                ComNavSatelliteSystemEnum.GALILEO => "LOCKOUTSYSTEM GALILEO",
                ComNavSatelliteSystemEnum.BD3 => "LOCKOUTSYSTEM BD3",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override string MessageId => "LOCKOUTSYSTEM";
    }

}