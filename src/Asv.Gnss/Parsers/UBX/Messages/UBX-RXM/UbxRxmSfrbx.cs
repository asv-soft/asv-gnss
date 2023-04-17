using System;
using Asv.Common;
using Asv.IO;

namespace Asv.Gnss
{
    public enum UbxGnssTypeEnum
    {
        GPS = 0,
        SBAS = 1,
        Galileo = 2,
        BeiDou = 3,
        IMES = 4,
        QZSS = 5,
        GLONASS = 6,
    }
    public class UbxRxmSfrbxPool : UbxMessageBase
    {
        public override string Name => "UBX-RXM-SFRBX-POOL";
        public override byte Class => 0x02;
        public override byte SubClass => 0x13;
        protected override void SerializeContent(ref Span<byte> buffer)
        {
             
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
             
        }

        protected override int GetContentByteSize() => 0;

        public override void Randomize(Random random)
        {
            
        }
    }
    public class UbxRxmSfrbx : UbxMessageBase
    {
        public override string Name => "UBX-RXM-SFRBX";
        public override byte Class => 0x02;
        public override byte SubClass => 0x13;
        
        /// <summary>
        /// GNSS identifier
        /// </summary>
        public UbxGnssTypeEnum UbxGnssId { get; set; }
        
        /// <summary>
        /// Satellite identifier
        /// </summary>
        public byte SvId { get; set; }
        
        /// <summary>
        /// Reserved
        /// </summary>
        public byte Reserved1 { get; set; }
        
        /// Only used for GLONASS: This is the
        /// frequency slot (range from -7 to 6)
        /// </summary>
        public sbyte FreqId { get; set; }
        
        /// <summary>
        /// The number of data words contained in
        /// this message (0..16)
        /// </summary>
        public byte NumWords { get; set; }
        
        /// <summary>
        /// Reserved
        /// </summary>
        public byte Chn { get; set; }
        
        /// <summary>
        /// Message version (0x01 for this version)
        /// </summary>
        public byte Version { get; set; }
        
        /// <summary>
        /// Reserved
        /// </summary>
        public byte Reserved2 { get; set; }
        
        /// <summary>
        /// The data words
        /// </summary>
        public uint[] RawData { get; set; }
        
        protected override void SerializeContent(ref Span<byte> buffer)
        {
            
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            UbxGnssId = (UbxGnssTypeEnum)BinSerialize.ReadByte(ref buffer);
            SvId = BinSerialize.ReadByte(ref buffer);
            Reserved1 = BinSerialize.ReadByte(ref buffer);
            FreqId = (sbyte)(BinSerialize.ReadSByte(ref buffer) - 7);
            NumWords = BinSerialize.ReadByte(ref buffer);
            Chn = BinSerialize.ReadByte(ref buffer);
            Version = BinSerialize.ReadByte(ref buffer);
            Reserved2 = BinSerialize.ReadByte(ref buffer);

            if (Version == 0x01)
            {
                Chn = 0;
            }
            
            RawData = new uint[NumWords];
            
            for (int i = 0; i < NumWords; i++)
            {
                RawData[i] = BinSerialize.ReadUInt(ref buffer);
            }
            
            if (UbxGnssId == UbxGnssTypeEnum.GPS)
            {
                GpsSubFrame = GpsSubFrameFactory.Create(RawData);
            }
            else if (UbxGnssId == UbxGnssTypeEnum.GLONASS)
            {
                GlonassWord = GlonassWordFactory.Create(RawData);
            }
        }

        protected override int GetContentByteSize() => 8 + (4 * RawData.Length);

        public override void Randomize(Random random)
        {
            
        }
        
        public GlonassWordBase GlonassWord { get; set; }
        
        public GpsSubframeBase GpsSubFrame { get; set; }
    }
}