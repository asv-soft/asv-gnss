using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// BESTPOS binary position log.
    /// </summary>
    public class ComNavBinaryBestPosPacket : ComNavBinaryMessageBase
    {
        public const ushort ComNavMessageId = 42;

        /// <inheritdoc />
        public override ushort MessageId => ComNavMessageId;

        /// <inheritdoc />
        public override string Name => "BESTPOS";

        /// <inheritdoc />
        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            SolutionStatus = (ComNavSolutionStatus)BinSerialize.ReadUInt(ref buffer);
            PositionType = (ComNavPositionType)BinSerialize.ReadUInt(ref buffer);
            Latitude = BinSerialize.ReadDouble(ref buffer);
            Longitude = BinSerialize.ReadDouble(ref buffer);
            HeightMsl = BinSerialize.ReadDouble(ref buffer);
            Undulation = BinSerialize.ReadFloat(ref buffer);
            Datum = (ComNavDatum)BinSerialize.ReadUInt(ref buffer);
            LatitudeSd = BinSerialize.ReadFloat(ref buffer);
            LongitudeSd = BinSerialize.ReadFloat(ref buffer);
            HeightMslSd = BinSerialize.ReadFloat(ref buffer);
            var station = new byte[4];
            BinSerialize.ReadBlock(ref buffer, new Span<byte>(station));
            BaseStationId = Encoding.ASCII.GetString(station).TrimEnd('\0');
            DifferentialAgeSec = BinSerialize.ReadFloat(ref buffer);
            SolutionAgeSec = BinSerialize.ReadFloat(ref buffer);
            TrackedSats = BinSerialize.ReadByte(ref buffer);
            SolutionSats = BinSerialize.ReadByte(ref buffer);
            L1CarrierPhaseSats = BinSerialize.ReadByte(ref buffer);
            L2CarrierPhaseSats = BinSerialize.ReadByte(ref buffer);
            Reserved1 = BinSerialize.ReadByte(ref buffer);
            ExtSolutionStatus = BinSerialize.ReadByte(ref buffer);
            Reserved2 = BinSerialize.ReadByte(ref buffer);
            SignalMask = BinSerialize.ReadByte(ref buffer);
        }

        /// <inheritdoc />
        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override int InternalGetContentByteSize()
        {
            return 72;
        }

        /// <summary>
        /// Solution status.
        /// </summary>
        public ComNavSolutionStatus SolutionStatus { get; set; }

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
        /// Base station identifier.
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

        /// <summary>
        /// Number of L1 carrier phases used.
        /// </summary>
        public byte L1CarrierPhaseSats { get; set; }

        /// <summary>
        /// Number of L2 carrier phases used.
        /// </summary>
        public byte L2CarrierPhaseSats { get; set; }

        public byte Reserved1 { get; set; }

        /// <summary>
        /// Extended solution status.
        /// </summary>
        public byte ExtSolutionStatus { get; set; }

        public byte Reserved2 { get; set; }

        /// <summary>
        /// Signals used mask.
        /// </summary>
        public byte SignalMask { get; set; }
    }
}
