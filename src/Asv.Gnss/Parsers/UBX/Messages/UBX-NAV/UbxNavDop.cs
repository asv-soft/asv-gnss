using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class UbxNavDopPool : UbxMessageBase
    {
        public override string Name => "UBX-NAV-DOP";
        public override byte Class => 0x01;
        public override byte SubClass => 0x04;

        protected override void SerializeContent(ref Span<byte> buffer) { }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer) { }

        protected override int GetContentByteSize() => 0;

        public override void Randomize(Random random) { }
    }

    public class UbxNavDop : UbxMessageBase
    {
        public override string Name => "UBX-NAV-DOP";
        public override byte Class => 0x01;
        public override byte SubClass => 0x04;

        /// <summary>
        /// GPS time of week of the navigation epoch.
        /// </summary>
        public ulong iTOW { get; set; }

        /// <summary>
        /// Geometric DOP
        /// </summary>
        public double gDOP { get; set; }

        /// <summary>
        /// Position DOP
        /// </summary>
        public double pDOP { get; set; }

        /// <summary>
        /// Time DOP
        /// </summary>
        public double tDOP { get; set; }

        /// <summary>
        /// Vertical DOP
        /// </summary>
        public double vDOP { get; set; }

        /// <summary>
        /// Horizontal DOP
        /// </summary>
        public double hDOP { get; set; }

        /// <summary>
        /// Northing DOP
        /// </summary>
        public double nDOP { get; set; }

        /// <summary>
        /// Easting DOP
        /// </summary>
        public double eDOP { get; set; }

        protected override void SerializeContent(ref Span<byte> buffer) { }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            iTOW = BinSerialize.ReadUInt(ref buffer);
            gDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            pDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            tDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            vDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            hDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            nDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            eDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
        }

        protected override int GetContentByteSize() => 18;

        public override void Randomize(Random random) { }
    }
}
