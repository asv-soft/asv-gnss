using System;
using Asv.Common;

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
        public RxValue<bool> IsGps { get; set; }
        public uint[] UIntBuffer { get; set; }
        protected override void SerializeContent(ref Span<byte> buffer)
        {
            
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            UIntBuffer = new uint[buffer.Length];
                
            for (int i = 0; i < UIntBuffer.Length; i++)
            {
                UIntBuffer[i] = buffer[i];
            }
            
            if (UIntBuffer[0] == 0)
            {
                IsGps.Value = true;
                GpsSubFrame = GpsSubFrameFactory.Create(UIntBuffer);
            }
            else if (UIntBuffer[0] == 6)
            {
                IsGps.Value = false;
                GlonassWord = GlonassWordFactory.Create(UIntBuffer);
            }
        }

        protected override int GetContentByteSize() => UIntBuffer.Length;

        public override void Randomize(Random random)
        {
            
        }
        
        public GlonassWordBase GlonassWord { get; set; }
        
        public GpsSubframeBase GpsSubFrame { get; set; }
    }
}