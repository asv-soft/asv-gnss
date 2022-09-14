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

    /// <summary>
    /// 32.10.18 UBX-CFG-MSG (0x06 0x01)
    /// Poll a message configuration
    /// Supported on:
    /// u-blox 8 / u-blox M8 protocol versions 15, 15.01, 16, 17, 18, 19, 19.1, 19.2, 20, 20.01,
    /// 20.1, 20.2, 20.3, 22, 22.01, 23 and 23.01
    /// </summary>
    public abstract class UbxCfgMsg : UbxMessageBase
    {
        public override byte Class => 0x06;
        public override byte SubClass => 0x01;
        public override string Name => "UBX-CFG-MSG";

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer,MsgClass);
            BinSerialize.WriteByte(ref buffer, MsgId);
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer, int payloadByteSize)
        {
            MsgClass = BinSerialize.ReadByte(ref buffer);
            MsgId = BinSerialize.ReadByte(ref buffer);
        }

        protected override int GetContentByteSize() => 2;

        public byte MsgClass { get; set; }
        public byte MsgId { get; set; }

        
    }

    public class UbxCfgMsgRate : UbxCfgMsg
    {
        public byte[] Rate { get; set; }

        public UbxCfgMsgRate(byte msgClass, byte msgId, byte msgRate)
        {
            MsgClass = msgClass;
            MsgId = msgId;
            Rate = new byte[] { 0, msgRate, 0, msgRate, 0, 0 };
        }

        protected override int GetContentByteSize() => base.GetContentByteSize() + (Rate?.Length ?? 6);

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer, int payloadByteSize)
        {
            base.DeserializeContent(ref buffer,payloadByteSize);
            Rate = new byte[payloadByteSize - 2];
            for (int i = 0; i < Rate.Length; i++)
            {
                Rate[i] = BinSerialize.ReadByte(ref buffer);
            }
        }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            base.SerializeContent(ref buffer);
            for (int i = 0; i < Rate.Length; i++)
            {
                BinSerialize.WriteByte(ref buffer,Rate[i]);
            }
        }
    }

}