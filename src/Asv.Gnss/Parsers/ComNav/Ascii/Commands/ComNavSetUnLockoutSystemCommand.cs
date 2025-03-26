using System;

namespace Asv.Gnss
{
    public class ComNavSetUnLockoutSystemCommand : ComNavAsciiCommandBase
    {
        public ComNavSatelliteSystemEnum SatelliteSystem { get; set; }

        protected override string SerializeToAsciiString()
        {
            return SatelliteSystem switch
            {
                ComNavSatelliteSystemEnum.GPS => "UNLOCKOUTSYSTEM GPS",
                ComNavSatelliteSystemEnum.BD2 => "UNLOCKOUTSYSTEM BD2",
                ComNavSatelliteSystemEnum.GLONASS => "UNLOCKOUTSYSTEM GLONASS",
                ComNavSatelliteSystemEnum.GALILEO => "UNLOCKOUTSYSTEM GALILEO",
                ComNavSatelliteSystemEnum.BD3 => "UNLOCKOUTSYSTEM BD3",
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public override string MessageId => "UNLOCKOUTSYSTEM";
    }
}
