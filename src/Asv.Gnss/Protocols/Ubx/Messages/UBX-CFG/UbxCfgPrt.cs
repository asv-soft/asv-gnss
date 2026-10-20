using System;
using System.IO.Ports;
using Asv.IO;

namespace Asv.Gnss;

public partial class UbxCfgPrtPool : UbxMessageBase
{
    public override string Name => "UBX-CFG-PRT-POOL";
    public override byte Class => 0x06;
    public override byte SubClass => 0x00;

    private byte _portId;

    /// <summary>
    /// Receiver port identifier: 0 for DDC, 1 for UART1, 2 for UART2, 3 for USB, or 4 for SPI, subject to receiver support.
    /// </summary>
    public byte PortId
    {
        get => _portId;
        set => _portId = value;
    }

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

public partial class UbxCfgPrt : UbxMessageBase
{
    public override string Name => "UBX-CFG-PRT";
    public override byte Class => 0x06;
    public override byte SubClass => 0x00;

    private UbxCfgPrtConfig? _config;

    /// <summary>
    /// Port-specific configuration, including protocol masks and transport parameters.
    /// </summary>
    public UbxCfgPrtConfig Config
    {
        get => _config!;
        set => _config = value;
    }

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
            _ => new UbxCfgPrtConfigUart()
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
    Spi
}

public abstract partial class UbxCfgPrtConfig : ISizedSpanSerializable, IVisitable
{
    /// <summary>
    /// Transport type selecting the port-specific CFG-PRT payload layout.
    /// </summary>
    public abstract UbxCfgPrtType PortType { get; }

    private bool _isInUbxProtocol = true;

    /// <summary>
    /// Whether UBX is enabled in the input protocol mask; availability depends on receiver firmware.
    /// </summary>
    public bool IsInUbxProtocol
    {
        get => _isInUbxProtocol;
        set => _isInUbxProtocol = value;
    }
    private bool _isInNmeaProtocol = true;

    /// <summary>
    /// Whether NMEA is enabled in the input protocol mask; availability depends on receiver firmware.
    /// </summary>
    public bool IsInNmeaProtocol
    {
        get => _isInNmeaProtocol;
        set => _isInNmeaProtocol = value;
    }
    private bool _isInRtcm2Protocol;

    /// <summary>
    /// Whether RTCM2 is enabled in the input protocol mask; availability depends on receiver firmware.
    /// </summary>
    public bool IsInRtcm2Protocol
    {
        get => _isInRtcm2Protocol;
        set => _isInRtcm2Protocol = value;
    }
    private bool _isInRtcm3Protocol = true;

    /// <summary>
    /// Whether RTCM3 is enabled in the input protocol mask; availability depends on receiver firmware.
    /// </summary>
    public bool IsInRtcm3Protocol
    {
        get => _isInRtcm3Protocol;
        set => _isInRtcm3Protocol = value;
    }

    private bool _isOutUbxProtocol = true;

    /// <summary>
    /// Whether UBX is enabled in the output protocol mask; availability depends on receiver firmware.
    /// </summary>
    public bool IsOutUbxProtocol
    {
        get => _isOutUbxProtocol;
        set => _isOutUbxProtocol = value;
    }
    private bool _isOutNmeaProtocol = true;

    /// <summary>
    /// Whether NMEA is enabled in the output protocol mask; availability depends on receiver firmware.
    /// </summary>
    public bool IsOutNmeaProtocol
    {
        get => _isOutNmeaProtocol;
        set => _isOutNmeaProtocol = value;
    }
    private bool _isOutRtcm3Protocol = true;

    /// <summary>
    /// Whether RTCM3 is enabled in the output protocol mask; availability depends on receiver firmware.
    /// </summary>
    public bool IsOutRtcm3Protocol
    {
        get => _isOutRtcm3Protocol;
        set => _isOutRtcm3Protocol = value;
    }

    private bool _isExtendedTxTimeout;

    /// <summary>
    /// Whether the extended transmitter timeout is enabled (flags bit 1).
    /// </summary>
    public bool IsExtendedTxTimeout
    {
        get => _isExtendedTxTimeout;
        set => _isExtendedTxTimeout = value;
    }

    public abstract void Deserialize(ref ReadOnlySpan<byte> buffer);
    public abstract void Serialize(ref Span<byte> buffer);
    public abstract int GetByteSize();
    public abstract void Randomize(Random random);
}

public partial class UbxCfgPrtConfigUart : UbxCfgPrtConfig
{
    private byte _portId;

    /// <summary>
    /// Receiver port identifier: 0 for DDC, 1 for UART1, 2 for UART2, 3 for USB, or 4 for SPI, subject to receiver support.
    /// </summary>
    public byte PortId
    {
        get => _portId;
        set => _portId = value;
    }
    private bool _isEnable;

    /// <summary>
    /// Whether the transmit-ready pin function is enabled (txReady bit 0).
    /// </summary>
    public bool IsEnable
    {
        get => _isEnable;
        set => _isEnable = value;
    }
    private PortPolarity _polarity;

    /// <summary>
    /// Active polarity of the transmit-ready signal (txReady bit 1).
    /// </summary>
    public PortPolarity Polarity
    {
        get => _polarity;
        set => _polarity = value;
    }
    private byte _pin;

    /// <summary>
    /// PIO number for the transmit-ready signal, encoded in txReady bits 2 through 6.
    /// </summary>
    public byte Pin
    {
        get => _pin;
        set => _pin = value;
    }
    private ushort _threshold;

    /// <summary>
    /// Transmit-ready threshold in units of eight bytes, encoded in txReady bits 7 through 15.
    /// </summary>
    public ushort Threshold
    {
        get => _threshold;
        set => _threshold = value;
    }

    private int _dataBits = 8;

    /// <summary>
    /// UART character length in bits; the documented receiver modes support 7 or 8 data bits.
    /// </summary>
    public int DataBits
    {
        get => _dataBits;
        set => _dataBits = value;
    }
    private Parity _parity = Parity.None;

    /// <summary>
    /// UART parity selection encoded in the port mode.
    /// </summary>
    public Parity Parity
    {
        get => _parity;
        set => _parity = value;
    }
    private StopBits _stopBits = StopBits.One;

    /// <summary>
    /// UART stop-bit selection encoded in the port mode.
    /// </summary>
    public StopBits StopBits
    {
        get => _stopBits;
        set => _stopBits = value;
    }
    private int _boundRate = 115200;

    /// <summary>
    /// UART baud rate in bits per second; supported rates depend on the receiver.
    /// </summary>
    public int BoundRate
    {
        get => _boundRate;
        set => _boundRate = value;
    }



    #region Classes

    public enum PortPolarity
    {
        HighActive = 0,
        LowActive = 1
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
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
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
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }

        public static Parity GetParity(byte value)
        {
            return value switch
            {
                0 => Parity.Even,
                1 => Parity.Odd,
                _ => (value & 0x6) == 4 ? Parity.None : Parity.Space
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
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
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
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
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
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
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
        var txReady = (ushort)((IsEnable ? 1 : 0) | ((byte)Polarity << 1) | (Pin << 2) | (Threshold << 7));
        BinSerialize.WriteUShort(ref buffer, txReady);

        var dataBits = (uint)(SerialPortHelper.GetByteFromCharLength(DataBits) << 6);
        var parity = (uint)(SerialPortHelper.GetByteFromParity(Parity) << 9);
        var stopBits = (uint)(SerialPortHelper.GetByteFromStopBit(StopBits) << 12);
        var uartMode = dataBits | parity | stopBits;
        BinSerialize.WriteUInt(ref buffer, uartMode);
        BinSerialize.WriteUInt(ref buffer, (uint)BoundRate);

        var inProtocol = (ushort)((IsInUbxProtocol ? 1 : 0) | ((IsInNmeaProtocol ? 1 : 0) << 1) |
                                  ((IsInRtcm2Protocol ? 1 : 0) << 2) | ((IsInRtcm3Protocol ? 1 : 0) << 5));
        BinSerialize.WriteUShort(ref buffer, inProtocol);

        var outProtocol = (ushort)((IsOutUbxProtocol ? 1 : 0) | ((IsOutNmeaProtocol ? 1 : 0) << 1) |
                                   ((IsOutRtcm3Protocol ? 1 : 0) << 5));
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

    public override void Randomize(Random random)
    {

    }
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

    public override void Randomize(Random random)
    {

    }
}