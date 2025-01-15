using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class ComNavBinaryPsrDopPacket : ComNavBinaryMessageBase
    {
        public const ushort ComNavMessageId = 174;
        public override ushort MessageId => ComNavMessageId;
        public override string Name => "PSRDOP";

        /// <summary>
        /// Geometric dilution of precision
        /// </summary>
        public float Gdop { get; set; }

        /// <summary>
        /// Position dilution of precision
        /// </summary>
        public float Pdop { get; set; }

        /// <summary>
        /// Horizontal dilution of precision
        /// </summary>
        public float Hdop { get; set; }

        /// <summary>
        /// Horizontal position and time dilution of precision
        /// </summary>
        public float Htdop { get; set; }

        /// <summary>
        /// Time dilution of precision
        /// </summary>
        public float Tdop { get; set; }

        /// <summary>
        /// Elevation cut-off angle
        /// </summary>
        public float Cutoff { get; set; }

        /// <summary>
        /// PRNs of SV PRN tracking
        /// </summary>
        public uint[] Satellites { get; set; }

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            Gdop = BinSerialize.ReadFloat(ref buffer);
            Pdop = BinSerialize.ReadFloat(ref buffer);
            Hdop = BinSerialize.ReadFloat(ref buffer);
            Htdop = BinSerialize.ReadFloat(ref buffer);
            Tdop = BinSerialize.ReadFloat(ref buffer);
            Cutoff = BinSerialize.ReadFloat(ref buffer);

            var prnNum = BinSerialize.ReadInt(ref buffer);
            Satellites = new uint[prnNum];

            for (var i = 0; i < prnNum; i++)
            {
                Satellites[i] = BinSerialize.ReadUInt(ref buffer);
            }
        }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        protected override int InternalGetContentByteSize()
        {
            return 28 + Satellites.Length * 4;
        }
    }
}
