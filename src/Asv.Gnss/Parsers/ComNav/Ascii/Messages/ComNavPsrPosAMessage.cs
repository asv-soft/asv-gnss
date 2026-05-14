using System;
using System.Globalization;

namespace Asv.Gnss
{
    /// <summary>
    /// PSRPOSA pseudorange position ASCII log.
    /// </summary>
    public class ComNavPsrPosAMessage : ComNavAsciiMessageBase
    {
        public const string ComNavMessageId = "PSRPOSA";

        /// <inheritdoc />
        public override string MessageId => ComNavMessageId;

        /// <inheritdoc />
        public override string Name => ComNavMessageId;

        /// <inheritdoc />
        protected override void InternalContentDeserialize(string[] msg)
        {
            if (msg.Length < 21)
            {
                throw new GnssParserException(ProtocolId, $"Error to deserialize {ProtocolId}:{MessageId} packet: expected 21 fields, got {msg.Length}");
            }

            SolutionStatusName = msg[0];
            SolutionStatus = ParseSolutionStatus(msg[0]);
            PositionTypeName = msg[1];
            PositionType = ParsePositionType(msg[1]);
            Latitude = ReadDouble(msg[2]);
            Longitude = ReadDouble(msg[3]);
            HeightMsl = ReadDouble(msg[4]);
            Undulation = ReadFloat(msg[5]);
            DatumName = msg[6];
            Datum = ParseDatum(msg[6]);
            LatitudeSd = ReadFloat(msg[7]);
            LongitudeSd = ReadFloat(msg[8]);
            HeightMslSd = ReadFloat(msg[9]);
            BaseStationId = msg[10].Trim('"');
            DifferentialAgeSec = ReadFloat(msg[11]);
            SolutionAgeSec = ReadFloat(msg[12]);
            TrackedSats = ReadByte(msg[13]);
            SolutionSats = ReadByte(msg[14]);
            Reserved1 = ReadByte(msg[15]);
            Reserved2 = ReadByte(msg[16]);
            Reserved3 = ReadByte(msg[17], NumberStyles.HexNumber);
            ExtSolutionStatus = ReadByte(msg[18], NumberStyles.HexNumber);
            GalileoBeiDouSignalMask = ReadByte(msg[19], NumberStyles.HexNumber);
            GpsGlonassSignalMask = ReadByte(msg[20], NumberStyles.HexNumber);
        }

        /// <summary>
        /// Raw solution status name from the ASCII log.
        /// </summary>
        public string SolutionStatusName { get; set; } = string.Empty;

        /// <summary>
        /// Solution status.
        /// </summary>
        public ComNavSolutionStatus SolutionStatus { get; set; }

        /// <summary>
        /// Raw position type name from the ASCII log.
        /// </summary>
        public string PositionTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Position type.
        /// </summary>
        public ComNavPositionType PositionType { get; set; }

        /// <summary>
        /// Latitude, degrees.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude, degrees.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Height above mean sea level, metres.
        /// </summary>
        public double HeightMsl { get; set; }

        /// <summary>
        /// Geoid undulation, metres.
        /// </summary>
        public float Undulation { get; set; }

        /// <summary>
        /// Raw datum name from the ASCII log.
        /// </summary>
        public string DatumName { get; set; } = string.Empty;

        /// <summary>
        /// Datum identifier.
        /// </summary>
        public ComNavDatum Datum { get; set; }

        /// <summary>
        /// Latitude standard deviation, metres.
        /// </summary>
        public float LatitudeSd { get; set; }

        /// <summary>
        /// Longitude standard deviation, metres.
        /// </summary>
        public float LongitudeSd { get; set; }

        /// <summary>
        /// Height standard deviation, metres.
        /// </summary>
        public float HeightMslSd { get; set; }

        /// <summary>
        /// Differential base station identifier.
        /// </summary>
        public string BaseStationId { get; set; } = string.Empty;

        /// <summary>
        /// Differential age, seconds.
        /// </summary>
        public float DifferentialAgeSec { get; set; }

        /// <summary>
        /// Solution age, seconds.
        /// </summary>
        public float SolutionAgeSec { get; set; }

        /// <summary>
        /// Number of tracked satellites.
        /// </summary>
        public byte TrackedSats { get; set; }

        /// <summary>
        /// Number of satellites used in solution.
        /// </summary>
        public byte SolutionSats { get; set; }

        public byte Reserved1 { get; set; }
        public byte Reserved2 { get; set; }
        public byte Reserved3 { get; set; }

        /// <summary>
        /// Extended solution status.
        /// </summary>
        public byte ExtSolutionStatus { get; set; }

        /// <summary>
        /// Galileo and BeiDou signal mask.
        /// </summary>
        public byte GalileoBeiDouSignalMask { get; set; }

        /// <summary>
        /// GPS and GLONASS signal mask.
        /// </summary>
        public byte GpsGlonassSignalMask { get; set; }

        private static ComNavSolutionStatus ParseSolutionStatus(string value)
        {
            return value.Equals("SOL_COMPUTED", StringComparison.OrdinalIgnoreCase)
                ? ComNavSolutionStatus.SolComputed
                : Enum.TryParse(value, true, out ComNavSolutionStatus status) ? status : default;
        }

        private static ComNavPositionType ParsePositionType(string value)
        {
            return value.Equals("WAAS", StringComparison.OrdinalIgnoreCase)
                ? ComNavPositionType.Sbas
                : Enum.TryParse(value, true, out ComNavPositionType type) ? type : default;
        }

        private static ComNavDatum ParseDatum(string value)
        {
            return value.Equals("WGS84", StringComparison.OrdinalIgnoreCase)
                ? ComNavDatum.Wgs84
                : Enum.TryParse(value, true, out ComNavDatum datum) ? datum : default;
        }

        private static double ReadDouble(string value) => double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);

        private static float ReadFloat(string value) => float.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);

        private static byte ReadByte(string value, NumberStyles style = NumberStyles.Integer) => byte.Parse(value, style, CultureInfo.InvariantCulture);
    }
}
