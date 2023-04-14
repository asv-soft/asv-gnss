using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class UbxMonRxbuf : UbxMessageBase
    {
        public override string Name => "UBX-MON-RXBUF";
        public override byte Class => 0x0A;
        public override byte SubClass => 0x07;
        
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
        }

        protected override int GetContentByteSize() => 24;

        public override void Randomize(Random random)
        {
             
        }
    }
}