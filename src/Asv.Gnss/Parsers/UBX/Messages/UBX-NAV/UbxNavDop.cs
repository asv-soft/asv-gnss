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
        /// Gets or sets gPS time of week of the navigation epoch.
        /// </summary>
        public ulong ITOW { get; set; }

        /// <summary>
        /// Gets or sets geometric DOP.
        /// </summary>
        public double GDOP { get; set; }

        /// <summary>
        /// Gets or sets position DOP.
        /// </summary>
        public double PDOP { get; set; }

        /// <summary>
        /// Gets or sets time DOP.
        /// </summary>
        public double TDOP { get; set; }

        /// <summary>
        /// Gets or sets vertical DOP.
        /// </summary>
        public double VDOP { get; set; }

        /// <summary>
        /// Gets or sets horizontal DOP.
        /// </summary>
        public double HDOP { get; set; }

        /// <summary>
        /// Gets or sets northing DOP.
        /// </summary>
        public double NDOP { get; set; }

        /// <summary>
        /// Gets or sets easting DOP.
        /// </summary>
        public double EDOP { get; set; }

        protected override void SerializeContent(ref Span<byte> buffer) { }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            ITOW = BinSerialize.ReadUInt(ref buffer);
            GDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            PDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            TDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            VDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            HDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            NDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            EDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
        }

        protected override int GetContentByteSize() => 18;

        public override void Randomize(Random random) { }
    }
}
