using System;
using Asv.IO;

namespace Asv.Gnss
{

    /// <summary>
    /// Reset receiver / Clear backup data structures
    /// Supported on:
    /// • u-blox 8 / u-blox M8 protocol versions 15, 15.01, 16, 17, 18, 19, 19.1, 19.2, 20, 20.01,
    /// 20.1, 20.2, 20.3, 22, 22.01, 23 and 23.01
    /// Do not expect this message to be acknowledged by the receiver.
    /// • Newer FW version will not acknowledge this message at all.
    /// • Older FW version will acknowledge this message but the acknowledge may not
    /// be sent completely before the receiver is reset.
    /// Notes:
    /// • If Galileo is enabled, UBX-CFG-RST Controlled GNSS start must be followed by
    /// UBX-CFG-RST with resetMode set to Hardware reset.
    /// • If Galileo is enabled, use resetMode Hardware reset instead of Controlled
    /// software reset or Controlled software reset (GNSS only)
    /// </summary>
    public class UbxCfgRst : UbxMessageBase
    {
        public override string Name => "UBX-CFG-RST";
        public override byte Class => 0x06;
        public override byte SubClass => 0x04;

        public BbrMask Bbr { get; set; } = BbrMask.HotStart;
        public ResetMode Mode { get; set; } = ResetMode.HardwareResetImmediately;

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            BinSerialize.WriteUShort(ref buffer,(ushort)Bbr);
            BinSerialize.WriteByte(ref buffer,(byte)Mode);
            BinSerialize.WriteByte(ref buffer, 0);
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            Bbr = (BbrMask)BinSerialize.ReadUShort(ref buffer);
            Mode = (ResetMode)BinSerialize.ReadByte(ref buffer);
            var reserved = BinSerialize.ReadByte(ref buffer);
        }

        protected override int GetContentByteSize() => 4;

        public override void Randomize(Random random)
        {
            Bbr = (BbrMask)random.Next(0, ushort.MaxValue);
            Mode = (ResetMode)random.Next(0, byte.MaxValue);
        }
    }

    public enum BbrMask : ushort
    {
        HotStart = 0x00,
        WarmStart = 0x01,
        ColdStart = 0xFF14
    }

    public enum ResetMode : byte
    {
        HardwareResetImmediately = 0x00,
        ControlledSoftwareReset = 0x01,
        ControlledSoftwareResetGnssOnly = 0x02,
        HardwareResetAfterShutdown = 0x04,
        ControlledGnssStop = 0x08,
        ControlledGnssStart = 0x09
    }
}