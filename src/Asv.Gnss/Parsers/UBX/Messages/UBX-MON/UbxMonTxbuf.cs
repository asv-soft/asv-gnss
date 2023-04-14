using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class UbxMonTxbuf : UbxMessageBase
    {
        public override string Name => "UBX-MON-TXBUF";
        public override byte Class => 0x0A;
        public override byte SubClass => 0x08;
        
        /// <summary>
        /// Number of bytes pending in transmitter
        /// buffer for each target
        /// </summary>
        public ushort[] Pending { get; } = new ushort[6];
        
        /// <summary>
        /// Maximum usage transmitter buffer during
        /// the last sysmon period for each target
        /// </summary>
        public byte[] Usage { get; } = new byte[6];
        
        /// <summary>
        /// Maximum usage transmitter buffer for each target
        /// </summary>
        public byte[] PeakUsage { get; } = new byte[6];
        
        /// <summary>
        /// Maximum usage of transmitter buffer
        /// during the last sysmon period for all targets
        /// </summary>
        public byte TUsage { get; set; }
        
        /// <summary>
        /// Maximum usage of transmitter buffer for
        /// all targets
        /// </summary>
        public byte TPeakUsage { get; set; }
        
        /// <summary>
        /// Error bitmask (error bits in region below)
        /// </summary>
        public byte Errors { get; set; }

        #region Errors bits
        
        /// <summary>
        /// Allocation error (TX buffer full)
        /// </summary>
        public bool IsAlloc { get; set; }
        
        /// <summary>
        /// Memory Allocation error
        /// </summary>
        public bool IsMem { get; set; }
        
        /// <summary>
        /// Buffer limit of corresponding target reached
        /// </summary>
        public bool IsLimit { get; set; }
        
        #endregion
        
        /// <summary>
        /// Reserved
        /// </summary>
        public byte Reserved1 { get; set; }

        
        protected override void SerializeContent(ref Span<byte> buffer)
        {
            
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            for (int i = 0; i < Pending.Length; i++)
            {
                Pending[i] = BinSerialize.ReadUShort(ref buffer);
            }
            
            for (int i = 0; i < Usage.Length; i++)
            {
                Usage[i] = BinSerialize.ReadByte(ref buffer);
            }
            
            for (int i = 0; i < PeakUsage.Length; i++)
            {
                PeakUsage[i] = BinSerialize.ReadByte(ref buffer);
            }
            
            TUsage = BinSerialize.ReadByte(ref buffer);
            
            TPeakUsage = BinSerialize.ReadByte(ref buffer);
            
            Errors = BinSerialize.ReadByte(ref buffer);

            IsAlloc = (Errors & 0b1000_0000) != 0;
            
            IsMem = (Errors & 0b0100_0000) != 0;
            
            IsLimit = (Errors & 0b0010_0000) != 0;
            
            Reserved1 = BinSerialize.ReadByte(ref buffer);
        }

        protected override int GetContentByteSize() => 28;

        public override void Randomize(Random random)
        {
            
        }
    }
}