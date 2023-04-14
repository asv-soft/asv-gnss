using System;
using Asv.Common;
using Asv.IO;

namespace Asv.Gnss
{
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
        public bool IsGps { get; set; }
        
        /// <summary>
        /// GNSS identifier
        /// </summary>
        public byte GnssId { get; set; }
        
        /// <summary>
        /// Satellite identifier
        /// </summary>
        public byte SvId { get; set; }
        
        /// <summary>
        /// Reserved
        /// </summary>
        public byte Reserved1 { get; set; }
        
        /// <summary>
        /// Only used for GLONASS: This is the
        /// frequency slot + 7 (range from 0 to 13)
        /// </summary>
        public byte FreqId { get; set; }
        
        /// <summary>
        /// The number of data words contained in
        /// this message (0..16)
        /// </summary>
        public byte NumWords { get; set; }
        
        /// <summary>
        /// Reserved
        /// </summary>
        public byte Reserved2 { get; set; }
        
        /// <summary>
        /// Message version (0x01 for this version)
        /// </summary>
        public byte Version { get; set; }
        
        /// <summary>
        /// Reserved
        /// </summary>
        public byte Reserved3 { get; set; }
        
        /// <summary>
        /// The data words
        /// </summary>
        public uint[] RawData { get; set; }
        
        protected override void SerializeContent(ref Span<byte> buffer)
        {
            
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            GnssId = BinSerialize.ReadByte(ref buffer);
            SvId = BinSerialize.ReadByte(ref buffer);
            Reserved1 = BinSerialize.ReadByte(ref buffer);
            FreqId = BinSerialize.ReadByte(ref buffer);
            NumWords = BinSerialize.ReadByte(ref buffer);
            Reserved2 = BinSerialize.ReadByte(ref buffer);
            Version = BinSerialize.ReadByte(ref buffer);
            Reserved3 = BinSerialize.ReadByte(ref buffer);
            
            RawData = new uint[NumWords];
            
            for (int i = 0; i < NumWords; i++)
            {
                RawData[i] = BinSerialize.ReadUInt(ref buffer);
            }
            
            if (GnssId == 0)
            {
                IsGps = true;
                GpsSubFrame = GpsSubFrameFactory.Create(RawData);
            }
            else if (GnssId == 6)
            {
                IsGps = false;
                GlonassWord = GlonassWordFactory.Create(RawData);
            }
        }

        protected override int GetContentByteSize() => RawData.Length;

        public override void Randomize(Random random)
        {
            
        }
        
        public GlonassWordBase GlonassWord { get; set; }
        
        public GpsSubframeBase GpsSubFrame { get; set; }
    }
}