using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Dilution of precision
    /// </summary>
    public class SbfPacketDOP:SbfMessageBase
    {
        public override ushort MessageRevision => 0;
        public override ushort MessageType => 4001;
        public override string Name => "DOP";

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            NrSV = BinSerialize.ReadByte(ref buffer);
            Reserved = BinSerialize.ReadByte(ref buffer);
            PDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            if (Math.Abs(PDOP) < double.Epsilon) PDOP = Double.NaN;
            TDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            HDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            VDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            HPL = CheckNan(BinSerialize.ReadDouble(ref buffer));
            VPL = CheckNan(BinSerialize.ReadDouble(ref buffer));
        }

        
        /// <summary>
        /// Vertical Protection Level (see the DO 229 standard).
        /// </summary>
        public double VPL { get; set; }
        /// <summary>
        /// Horizontal Protection Level (see the DO 229 standard)
        /// </summary>
        public double HPL { get; set; }

        public double VDOP { get; set; }

        public double HDOP { get; set; }

        public double TDOP { get; set; }

        /// <summary>
        /// If 0, PDOP not available
        /// </summary>
        public double PDOP { get; set; }

        public byte Reserved { get; set; }

        /// <summary>
        /// Total number of satellites used in the DOP computation, or 0 if the DOP
        /// information is not available(in that case, the xDOP fields are all set to 0)
        /// </summary>
        public byte NrSV { get; set; }

        private static double CheckNan(double toSingle)
        {
            return toSingle == -2E10 ? Single.NaN : toSingle;
        }

        
    }
}