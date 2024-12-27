using System;

namespace Asv.Gnss
{
    public static class ComNavBinaryHelper
    {
        public static int GetPnrAndRinexCode(
            ComNavSatelliteSystemEnum sys,
            int svId,
            out string rCode
        )
        {
            switch (sys)
            {
                case ComNavSatelliteSystemEnum.GPS:
                    rCode = $"G{svId}";
                    return svId;
                case ComNavSatelliteSystemEnum.GLONASS:
                    rCode = $"R{svId - 37}";
                    return svId - 37;
                case ComNavSatelliteSystemEnum.GALILEO:
                    rCode = $"E{svId}";
                    return svId;
                case ComNavSatelliteSystemEnum.BD2:
                case ComNavSatelliteSystemEnum.BD3:
                    rCode = $"C{svId - 140}";
                    return svId - 140;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sys), sys, null);
            }
        }

        public static ComNavSolutionStatus ParseSolutionStatus(byte[] buffer, int byteOffset)
        {
            var solStat = BitConverter.ToUInt32(buffer, byteOffset);
            return (ComNavSolutionStatus)solStat;
        }

        public static ComNavPositionType ParsePositionType(byte[] buffer, int byteOffset)
        {
            var id = BitConverter.ToUInt32(buffer, byteOffset);
            return (ComNavPositionType)id;
        }

        public static ComNavDatum ParseDatum(byte[] buffer, int offsetInBytes)
        {
            return (ComNavDatum)BitConverter.ToUInt32(buffer, offsetInBytes);
        }
    }
}
