using System;

namespace Asv.Gnss
{
    public static class UbxHelper
    {
        public const int HeaderOffset = 6;
        public const byte SyncByte1 = 0xB5;
        public const byte SyncByte2 = 0x62;

        private const double RE_WGS84 = 6378137.0; /* earth semimajor axis (WGS84) (m) */
        private const double FE_WGS84 = (1.0 / 298.257223563); /* earth flattening (WGS84) */

        public enum ClassIDs : byte
        {
            /// <summary>
            /// Navigation Results Messages: Position, Speed, Time, Acceleration, Heading, DOP, SVs used 
            /// </summary>
            NAV = 0x01,

            /// <summary>
            /// Receiver Manager Messages: Satellite Status, RTC Status 
            /// </summary>
            RXM = 0x02,

            /// <summary>
            /// Information Messages: Printf-Style Messages, with IDs such as Error, Warning, Notice 
            /// </summary>
            INF = 0x04,

            /// <summary>
            /// Ack/Nak Messages: Acknowledge or Reject messages to UBX-CFG input messages 
            /// </summary>
            ACK = 0x05,

            /// <summary>
            /// Configuration Input Messages: Configure the receiver 
            /// </summary>
            CFG = 0x06,

            /// <summary>
            /// Firmware Update Messages: Memory/Flash erase/write, Reboot, Flash identification, etc 
            /// </summary>
            UPD = 0x09,

            /// <summary>
            /// Monitoring Messages: Communication Status, CPU Load, Stack Usage, Task Status 
            /// </summary>
            MON = 0x0A,

            /// <summary>
            /// AssistNow Aiding Messages: Ephemeris, Almanac, other A-GPS data input 
            /// </summary>
            AID = 0x0B,

            /// <summary>
            /// Timing Messages: Time Pulse Output, Time Mark Results 
            /// </summary>
            TIM = 0x0D,

            /// <summary>
            /// External Sensor Fusion Messages: External Sensor Measurements and Status Information 
            /// </summary>
            ESF = 0x10,

            /// <summary>
            /// Multiple GNSS Assistance Messages: Assistance data for various GNSS 
            /// </summary>
            MGA = 0x13,

            /// <summary>
            /// Logging Messages: Log creation, deletion, info and retrieval 
            /// </summary>
            LOG = 0x21,

            /// <summary>
            /// Security Feature Messages 
            /// </summary>
            SEC = 0x27,

            /// <summary>
            /// High Rate Navigation Results Messages: High rate time, position, speed, heading
            /// </summary>
            HNR = 0x28,

            NMEA = 0xF0,

            RTCM3 = 0xF5
        }

        public static ushort ReadMessageId(byte[] buffer)
        {
            return (ushort)((buffer[2] << 8) | buffer[3]);
        }

        public static string GetMessageName(ushort msgNum)
        {
            return $"{(ClassIDs)(msgNum >> 8):G}-0x{msgNum & 0xFF:X}";
        }

        public static ushort ReadMessageLength(byte[] buffer)
        {
            return (ushort)(buffer[4] + (buffer[5] << 8));
        }


        /// <summary>
        /// Transform ecef position to geodetic position.
        /// Notes  : WGS84, ellipsoidal height
        /// </summary>
        /// <param name="r">I   ecef position {x,y,z} (m)</param>
        /// <param name="pos">O   geodetic position {lat,lon,h} (rad,m)</param>
        public static (double X, double Y, double Z) Ecef2Pos((double X, double Y, double Z) r)
        {
            var e2 = FE_WGS84 * (2.0 - FE_WGS84);
            double r2 = Dot(r, r, 2), z, zk;
            var v = RE_WGS84;

            for (z = r.Z, zk = 0.0; Math.Abs(z - zk) >= 1E-4;)
            {
                zk = z;
                var sinp = z / Math.Sqrt(r2 + z * z);
                v = RE_WGS84 / Math.Sqrt(1.0 - e2 * sinp * sinp);
                z = r.Z + v * e2 * sinp;
            }

            (double X, double Y, double Z) position;
            position.X = r2 > 1E-12 ? Math.Atan(z / Math.Sqrt(r2)) : (r.Z > 0.0 ? Math.PI / 2.0 : -Math.PI / 2.0);
            position.Y = r2 > 1E-12 ? Math.Atan2(r.Y, r.X) : 0.0;
            position.Z = Math.Sqrt(r2 + z * z) - v;

            return position;
        }

        private static double Dot((double X, double Y, double Z) a, (double X, double Y, double Z) b, int n)
        {
            var result = 0.0;
            if (n <= 0) return result;
            result += a.X * b.X;
            if (n == 1) return result;
            result += a.Y * b.Y;
            if (n == 2) return result;
            result += a.Z * b.Z;
            return result;
        }
    }
}