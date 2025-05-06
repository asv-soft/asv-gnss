using System;
using System.IO.Ports;
using Asv.IO;

namespace Asv.Gnss
{
    public class UbxCfgPrtPool : UbxMessageBase
    {
        public override string Name => "UBX-CFG-PRT-POOL";
        public override byte Class => 0x06;
        public override byte SubClass => 0x00;

        public byte PortId { get; set; }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, PortId);
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            PortId = BinSerialize.ReadByte(ref buffer);
        }

        protected override int GetContentByteSize() => 1;

        public override void Randomize(Random random)
        {
            PortId = (byte)random.Next(0, 10);
        }
    }

    public class UbxCfgPrt : UbxMessageBase
    {
        public override string Name => "UBX-CFG-PRT";
        public override byte Class => 0x06;
        public override byte SubClass => 0x00;

        public UbxCfgPrtConfig Config { get; set; }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            Config.Serialize(ref buffer);
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            var portId = buffer[0];
            Config = portId switch
            {
                4 => new UbxCfgPrtConfigSpi(),
                3 => new UbxCfgPrtConfigUsb(),
                _ => new UbxCfgPrtConfigUart(),
            };
            Config.Deserialize(ref buffer);
        }

        protected override int GetContentByteSize() => 20;

        public override void Randomize(Random random)
        {
            Config = new UbxCfgPrtConfigUart();
            Config.Randomize(random);
        }
    }

    public enum UbxCfgPrtType
    {
        Uart,
        Usb,
        Spi,
    }

    public abstract class UbxCfgPrtConfig : ISizedSpanSerializable
    {
        public abstract UbxCfgPrtType PortType { get; }

        public bool IsInUbxProtocol { get; set; } = true;
        public bool IsInNmeaProtocol { get; set; } = true;
        public bool IsInRtcm2Protocol { get; set; }
        public bool IsInRtcm3Protocol { get; set; } = true;

        public bool IsOutUbxProtocol { get; set; } = true;
        public bool IsOutNmeaProtocol { get; set; } = true;
        public bool IsOutRtcm3Protocol { get; set; } = true;

        public bool IsExtendedTxTimeout { get; set; }

        public abstract void Deserialize(ref ReadOnlySpan<byte> buffer);
        public abstract void Serialize(ref Span<byte> buffer);
        public abstract int GetByteSize();
        public abstract void Randomize(Random random);
    }

    public class UbxCfgPrtConfigUart : UbxCfgPrtConfig
    {
        public byte PortId { get; set; }
        public bool IsEnable { get; set; }
        public PortPolarity Polarity { get; set; }
        public byte Pin { get; set; }
        public ushort Threshold { get; set; }

        public int DataBits { get; set; } = 8;
        public Parity Parity { get; set; } = Parity.None;
        public StopBits StopBits { get; set; } = StopBits.One;
        public int BoundRate { get; set; } = 115200;

        #region Classes

        public enum PortPolarity
        {
            HighActive = 0,
            LowActive = 1,
        }

        private static class SerialPortHelper
        {
            public static int GetCharLength(uint value)
            {
                return value switch
                {
                    0x00 => 5,
                    0x01 => 6,
                    0x02 => 7,
                    0x03 => 8,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
                };
            }

            public static byte GetByteFromCharLength(int value)
            {
                return value switch
                {
                    5 => 0x00,
                    6 => 0x01,
                    7 => 0x02,
                    8 => 0x03,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
                };
            }

            public static Parity GetParity(byte value)
            {
                return value switch
                {
                    0 => Parity.Even,
                    1 => Parity.Odd,
                    _ => (value & 0x6) == 4 ? Parity.None : Parity.Space,
                };
            }

            public static byte GetByteFromParity(Parity value)
            {
                return value switch
                {
                    Parity.None => 0x04,
                    Parity.Odd => 0x01,
                    Parity.Even => 0x00,
                    Parity.Mark => 0x02,
                    Parity.Space => 0x02,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
                };
            }

            public static StopBits GetStopBit(byte value)
            {
                return value switch
                {
                    0x00 => StopBits.One,
                    0x01 => StopBits.OnePointFive,
                    0x02 => StopBits.Two,
                    0x03 => StopBits.None,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
                };
            }

            public static byte GetByteFromStopBit(StopBits value)
            {
                return value switch
                {
                    StopBits.None => 0x03,
                    StopBits.One => 0x00,
                    StopBits.Two => 0x02,
                    StopBits.OnePointFive => 0x01,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
                };
            }
        }
        #endregion

        public override UbxCfgPrtType PortType => UbxCfgPrtType.Uart;

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            PortId = BinSerialize.ReadByte(ref buffer);
            var reserved = BinSerialize.ReadByte(ref buffer);
            var txReady = BinSerialize.ReadUShort(ref buffer);
            IsEnable = (txReady & 0x01) != 0;
            Polarity = (PortPolarity)((txReady & 0x02) >> 1);
            Pin = (byte)((txReady & 0x7C) >> 2);
            Threshold = (ushort)((txReady & 0xFF80) >> 7);
            var uartMode = BinSerialize.ReadUInt(ref buffer);

            DataBits = SerialPortHelper.GetCharLength((uartMode & 0xC0) >> 6);
            Parity = SerialPortHelper.GetParity((byte)((uartMode & 0xE00) >> 9));
            StopBits = SerialPortHelper.GetStopBit((byte)((uartMode & 0x3000) >> 12));
            BoundRate = (int)BinSerialize.ReadUInt(ref buffer);

            var inProtocol = BinSerialize.ReadUShort(ref buffer);
            IsInUbxProtocol = (inProtocol & 0x01) != 0;
            IsInNmeaProtocol = (inProtocol & 0x02) != 0;
            IsInRtcm2Protocol = (inProtocol & 0x04) != 0;
            IsInRtcm3Protocol = (inProtocol & 0x20) != 0;

            var outProtocol = BinSerialize.ReadUShort(ref buffer);
            IsOutUbxProtocol = (outProtocol & 0x01) != 0;
            IsOutNmeaProtocol = (outProtocol & 0x02) != 0;
            IsOutRtcm3Protocol = (outProtocol & 0x20) != 0;

            IsExtendedTxTimeout = (BinSerialize.ReadUShort(ref buffer) & 0x02) != 0;

            var reserved1 = BinSerialize.ReadByte(ref buffer);
            var reserved2 = BinSerialize.ReadByte(ref buffer);
        }

        public override void Serialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, PortId);
            BinSerialize.WriteByte(ref buffer, 0);
            var txReady = (ushort)(
                (IsEnable ? 1 : 0) | ((byte)Polarity << 1) | (Pin << 2) | (Threshold << 7)
            );
            BinSerialize.WriteUShort(ref buffer, txReady);

            var dataBits = (uint)(SerialPortHelper.GetByteFromCharLength(DataBits) << 6);
            var parity = (uint)(SerialPortHelper.GetByteFromParity(Parity) << 9);
            var stopBits = (uint)(SerialPortHelper.GetByteFromStopBit(StopBits) << 12);
            var uartMode = dataBits | parity | stopBits;
            BinSerialize.WriteUInt(ref buffer, uartMode);
            BinSerialize.WriteUInt(ref buffer, (uint)BoundRate);

            var inProtocol = (ushort)(
                (IsInUbxProtocol ? 1 : 0)
                | ((IsInNmeaProtocol ? 1 : 0) << 1)
                | ((IsInRtcm2Protocol ? 1 : 0) << 2)
                | ((IsInRtcm3Protocol ? 1 : 0) << 5)
            );
            BinSerialize.WriteUShort(ref buffer, inProtocol);

            var outProtocol = (ushort)(
                (IsOutUbxProtocol ? 1 : 0)
                | ((IsOutNmeaProtocol ? 1 : 0) << 1)
                | ((IsOutRtcm3Protocol ? 1 : 0) << 5)
            );
            BinSerialize.WriteUShort(ref buffer, outProtocol);

            var isExtendedTxTimeout = (ushort)((IsExtendedTxTimeout ? 1 : 0) << 1);
            BinSerialize.WriteUShort(ref buffer, isExtendedTxTimeout);
            BinSerialize.WriteByte(ref buffer, 0);
            BinSerialize.WriteByte(ref buffer, 0);
        }

        public override int GetByteSize() => 20;

        public override void Randomize(Random random)
        {
            PortId = (byte)random.Next(0, 2);
            DataBits = 8;
            Parity = Parity.None;
            StopBits = StopBits.One;
        }
    }

    public class UbxCfgPrtConfigUsb : UbxCfgPrtConfig
    {
        public override UbxCfgPrtType PortType => UbxCfgPrtType.Usb;

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            buffer = buffer.Slice(GetByteSize()); // TODO: implement
        }

        public override void Serialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, 3); /*PortID USB = 3*/
            buffer = buffer.Slice(GetByteSize() - 3); // TODO: implement
        }

        public override int GetByteSize() => 20;

        public override void Randomize(Random random) { }
    }

    public class UbxCfgPrtConfigSpi : UbxCfgPrtConfig
    {
        public override UbxCfgPrtType PortType => UbxCfgPrtType.Spi;

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            buffer = buffer.Slice(GetByteSize()); // TODO: implement
        }

        public override void Serialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, 3); /*PortID USB = 4*/
            buffer = buffer.Slice(GetByteSize() - 3); // TODO: implement
        }

        public override int GetByteSize() => 20;

        public override void Randomize(Random random) { }
    }
}
