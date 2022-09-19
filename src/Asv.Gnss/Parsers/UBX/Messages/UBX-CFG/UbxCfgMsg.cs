using System;
using Asv.IO;

namespace Asv.Gnss
{
    public enum UbxCfgMsgType
    {
        Pool,
        Rate,
        ManyRate,
    }

    
    public class UbxCfgMsgPool : UbxMessageBase
    {
        public override byte Class => 0x06;
        public override byte SubClass => 0x01;
        public override string Name => "UBX-CFG-MSG-POOL";

        public byte MsgClass { get; set; }
        public byte MsgId { get; set; }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, MsgClass);
            BinSerialize.WriteByte(ref buffer, MsgId);
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            MsgClass = BinSerialize.ReadByte(ref buffer);
            MsgId = BinSerialize.ReadByte(ref buffer);
        }

        protected override int GetContentByteSize() => 2;

        public override void Randomize(Random random)
        {
            MsgClass = (byte)random.Next(0, byte.MaxValue);
            MsgId = (byte)random.Next(0, byte.MaxValue);
        }

        
    }

    /// <summary>
    /// 32.10.18 UBX-CFG-MSG (0x06 0x01)
    /// Poll a message configuration
    /// Supported on:
    /// u-blox 8 / u-blox M8 protocol versions 15, 15.01, 16, 17, 18, 19, 19.1, 19.2, 20, 20.01,
    /// 20.1, 20.2, 20.3, 22, 22.01, 23 and 23.01
    /// Set message rate configuration for the current port.
    /// </summary>
    public class UbxCfgMsg : UbxMessageBase
    {
        public override byte Class => 0x06;
        public override byte SubClass => 0x01;
        public override string Name => "UBX-CFG-MSG";

        public byte MsgClass { get; set; }
        public byte MsgId { get; set; }
        public byte? CurrentPortRate { get; set; }
        public byte[] Ports { get; set; }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, MsgClass);
            BinSerialize.WriteByte(ref buffer, MsgId);
            if (CurrentPortRate != null && Ports != null)
            {
                throw new Exception($"Must set only one field: {nameof(CurrentPortRate)} or {nameof(Ports)}");
            }
            if (CurrentPortRate == null && Ports == null)
            {
                throw new Exception($"Must set one field: {nameof(CurrentPortRate)}==null or {nameof(Ports)}==null");
            }

            if (Ports != null && Ports.Length != 6)
            {
                throw new Exception($"{nameof(Ports)} length must be 6 element");
            }

            if (Ports != null)
            {
                foreach (var port in Ports)
                {
                    BinSerialize.WriteByte(ref buffer, port);
                }
            }

            if (CurrentPortRate != null)
            {
                BinSerialize.WriteByte(ref buffer, CurrentPortRate.Value);
            }
            
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            MsgClass = BinSerialize.ReadByte(ref buffer);
            MsgId = BinSerialize.ReadByte(ref buffer);
            if (buffer.Length == 1)
            {
                CurrentPortRate = BinSerialize.ReadByte(ref buffer);
            }
            else
            {
                Ports = new byte[8];
                BinSerialize.ReadBlock(ref buffer, Ports);
            }
        }

        protected override int GetContentByteSize() => CurrentPortRate != null ? 3 : 8;

        public override void Randomize(Random random)
        {
            MsgClass = (byte)random.Next(0, byte.MaxValue);
            MsgId = (byte)random.Next(0, byte.MaxValue);
            CurrentPortRate = (byte)random.Next(0, byte.MaxValue);
        }

       
    }

}