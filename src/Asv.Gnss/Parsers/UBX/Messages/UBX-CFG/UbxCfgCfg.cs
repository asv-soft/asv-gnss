using System;
using Asv.IO;

namespace Asv.Gnss
{
    public enum UbxCfgDeviceMask : byte
    {
        DevBbr = 0b0000_0001,
        DevFlash = 0b0000_0010,
        DevEeprom = 0b0000_0100,
        DevSpiFlash = 0b0001_0000,
    }

    [Flags]
    public enum UbxCfgSection:uint
    {
        None = 0b0000_0000_0000_0000,
        All = 0b0001_1111_0001_1111,

        /// <summary>
        ///  Communications port settings. Modifying this sub-section results in an IO system reset. Because of
        ///  this undefined data may be output for a short period of time after receiving the message.
        /// </summary>
        IoPort = 0b0000_0000_0000_0001,
        /// <summary>
        ///  Message configuration
        /// </summary>
        MsgConf = 0b0000_0000_0000_0010,
        /// <summary>
        ///  INF message configuration
        /// </summary>
        InfMsg = 0b0000_0000_0000_0100,
        /// <summary>
        ///  Navigation configuration
        /// </summary>
        NavConf = 0b0000_0000_0000_1000,
        /// <summary>
        ///  Receiver Manager configuration
        /// </summary>
        RxmConf = 0b0000_0000_0001_0000,
        /// <summary>
        ///  Sensor interface configuration (not supported in protocol versions less than 19)
        /// </summary>
        SenConf = 0b0000_0001_0000_0000,
        /// <summary>
        ///  Remote inventory configuration
        /// </summary>
        RinvConf = 0b0000_0010_0000_0000,
        /// <summary>
        ///  Antenna configuration
        /// </summary>
        AntConf = 0b0000_0100_0000_0000,
        /// <summary>
        ///  Logging configuration
        /// </summary>
        LogConf = 0b0000_1000_0000_0000,
        /// <summary>
        ///  FTS configuration. Only applicable to the FTS product variant
        /// </summary>
        FtsConf = 0b0001_0000_0000_0000,
    }

    public class UbxCfgCfg:UbxMessageBase
    {
        public override string Name => "UBX-CFG-CFG";
        public override byte Class => 0x06;
        public override byte SubClass => 0x09;

        public UbxCfgDeviceMask? DeviceMask { get; set; }
        public UbxCfgSection ClearMask { get; set; }
        public UbxCfgSection SaveMask { get; set; }
        public UbxCfgSection LoadMask { get; set; }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            BinSerialize.WriteUInt(ref buffer,(uint)ClearMask);
            BinSerialize.WriteUInt(ref buffer, (uint)SaveMask);
            BinSerialize.WriteUInt(ref buffer, (uint)LoadMask);
            if (DeviceMask.HasValue)
            {
                BinSerialize.WriteByte(ref buffer,(byte)DeviceMask.Value);
            }
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            ClearMask = (UbxCfgSection)BinSerialize.ReadUInt(ref buffer);
            SaveMask = (UbxCfgSection)BinSerialize.ReadUInt(ref buffer);
            LoadMask = (UbxCfgSection)BinSerialize.ReadUInt(ref buffer);
            if (buffer.IsEmpty == false)
            {
                DeviceMask = (UbxCfgDeviceMask?)BinSerialize.ReadByte(ref buffer);
            }
        }

        protected override int GetContentByteSize() => DeviceMask.HasValue ? 13 : 12;

        public override void Randomize(Random random)
        {
            ClearMask = (UbxCfgSection)random.Next(0, int.MaxValue);
            SaveMask = (UbxCfgSection)random.Next(0, int.MaxValue);
            LoadMask = (UbxCfgSection)random.Next(0, int.MaxValue);
            if (random.NextDouble() > 0.5)
            {
                DeviceMask = (UbxCfgDeviceMask?)random.Next(0, byte.MaxValue);
            }
        }
    }
}