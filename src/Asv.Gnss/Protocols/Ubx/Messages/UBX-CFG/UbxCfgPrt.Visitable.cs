using System;
using System.IO.Ports;
using Asv.IO;

namespace Asv.Gnss;

public partial class UbxCfgPrtPool
{
    private static readonly Field PortIdField = UbxVisitor.Field(nameof(PortId), UInt8Type.Default, "Receiver port identifier: 0 for DDC, 1 for UART1, 2 for UART2, 3 for USB, or 4 for SPI, subject to receiver support.");

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt8Type.Accept(visitor, PortIdField, ref _portId);
    }
}

public partial class UbxCfgPrt
{
    private static readonly Field UartConfigField = UbxVisitor.Field(nameof(Config), new OptionalStructType(UbxCfgPrtConfigUart.VisitorType), "Port-specific configuration, including protocol masks and transport parameters.");
    private static readonly Field UsbConfigField = UbxVisitor.Field(nameof(Config), new OptionalStructType(UbxCfgPrtConfig.VisitorType), "Port-specific configuration, including protocol masks and transport parameters.");
    private static readonly Field SpiConfigField = UbxVisitor.Field(nameof(Config), new OptionalStructType(UbxCfgPrtConfig.VisitorType), "Port-specific configuration, including protocol masks and transport parameters.");
    private static readonly Field OtherConfigField = UbxVisitor.Field(nameof(Config), new OptionalStructType(UbxCfgPrtConfig.VisitorType), "Port-specific configuration, including protocol masks and transport parameters.");

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var field = _config switch
        {
            UbxCfgPrtConfigUsb => UsbConfigField,
            UbxCfgPrtConfigSpi => SpiConfigField,
            UbxCfgPrtConfigUart or null => UartConfigField,
            _ => OtherConfigField
        };
        if (visitor is not OptionalStructType.IVisitor optionalVisitor)
        {
            visitor.VisitUnknown(field, field.DataType);
            return;
        }

        optionalVisitor.BeginOptionalStruct(field, (OptionalStructType)field.DataType, _config != null, out var createNew);
        if (createNew)
        {
            _config = _config switch
            {
                UbxCfgPrtConfigUsb => new UbxCfgPrtConfigUsb(),
                UbxCfgPrtConfigSpi => new UbxCfgPrtConfigSpi(),
                UbxCfgPrtConfigUart or null => new UbxCfgPrtConfigUart(),
                _ => throw new NotSupportedException("Recreating a custom CFG-PRT configuration is not supported.")
            };
        }
        _config?.Accept(visitor);
        optionalVisitor.EndOptionalStruct(_config != null);
    }
}

public abstract partial class UbxCfgPrtConfig
{
    private static readonly Field PortTypeField = UbxVisitor.Enum(nameof(PortType), Int32Type.Default, "Transport type selecting the port-specific CFG-PRT payload layout. This read-only value identifies the concrete configuration type.",
        new EnumValue<int>((int)UbxCfgPrtType.Uart, nameof(UbxCfgPrtType.Uart)),
        new EnumValue<int>((int)UbxCfgPrtType.Usb, nameof(UbxCfgPrtType.Usb)),
        new EnumValue<int>((int)UbxCfgPrtType.Spi, nameof(UbxCfgPrtType.Spi)));

    private static readonly Field IsInUbxProtocolField = UbxVisitor.Field(nameof(IsInUbxProtocol), BoolType.Default, "Whether UBX is enabled in the input protocol mask; availability depends on receiver firmware.");

    private static readonly Field IsInNmeaProtocolField = UbxVisitor.Field(nameof(IsInNmeaProtocol), BoolType.Default, "Whether NMEA is enabled in the input protocol mask; availability depends on receiver firmware.");

    private static readonly Field IsInRtcm2ProtocolField = UbxVisitor.Field(nameof(IsInRtcm2Protocol), BoolType.Default, "Whether RTCM2 is enabled in the input protocol mask; availability depends on receiver firmware.");

    private static readonly Field IsInRtcm3ProtocolField = UbxVisitor.Field(nameof(IsInRtcm3Protocol), BoolType.Default, "Whether RTCM3 is enabled in the input protocol mask; availability depends on receiver firmware.");

    private static readonly Field IsOutUbxProtocolField = UbxVisitor.Field(nameof(IsOutUbxProtocol), BoolType.Default, "Whether UBX is enabled in the output protocol mask; availability depends on receiver firmware.");

    private static readonly Field IsOutNmeaProtocolField = UbxVisitor.Field(nameof(IsOutNmeaProtocol), BoolType.Default, "Whether NMEA is enabled in the output protocol mask; availability depends on receiver firmware.");

    private static readonly Field IsOutRtcm3ProtocolField = UbxVisitor.Field(nameof(IsOutRtcm3Protocol), BoolType.Default, "Whether RTCM3 is enabled in the output protocol mask; availability depends on receiver firmware.");

    private static readonly Field IsExtendedTxTimeoutField = UbxVisitor.Field(nameof(IsExtendedTxTimeout), BoolType.Default, "Whether the extended transmitter timeout is enabled (flags bit 1).");

    /// <summary>Field metadata for this receiver-port configuration.</summary>
    public static StructType VisitorType { get; } = new([PortTypeField, IsInUbxProtocolField, IsInNmeaProtocolField, IsInRtcm2ProtocolField, IsInRtcm3ProtocolField, IsOutUbxProtocolField, IsOutNmeaProtocolField, IsOutRtcm3ProtocolField, IsExtendedTxTimeoutField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var portType = (int)PortType;
        Int32Type.Accept(visitor, PortTypeField, ref portType);
        BoolType.Accept(visitor, IsInUbxProtocolField, ref _isInUbxProtocol);
        BoolType.Accept(visitor, IsInNmeaProtocolField, ref _isInNmeaProtocol);
        BoolType.Accept(visitor, IsInRtcm2ProtocolField, ref _isInRtcm2Protocol);
        BoolType.Accept(visitor, IsInRtcm3ProtocolField, ref _isInRtcm3Protocol);
        BoolType.Accept(visitor, IsOutUbxProtocolField, ref _isOutUbxProtocol);
        BoolType.Accept(visitor, IsOutNmeaProtocolField, ref _isOutNmeaProtocol);
        BoolType.Accept(visitor, IsOutRtcm3ProtocolField, ref _isOutRtcm3Protocol);
        BoolType.Accept(visitor, IsExtendedTxTimeoutField, ref _isExtendedTxTimeout);
    }
}

public partial class UbxCfgPrtConfigUart
{
    private static readonly Field PortIdField = UbxVisitor.Field(nameof(PortId), UInt8Type.Default, "Receiver port identifier: 0 for DDC, 1 for UART1, 2 for UART2, 3 for USB, or 4 for SPI, subject to receiver support.");

    private static readonly Field IsEnableField = UbxVisitor.Field(nameof(IsEnable), BoolType.Default, "Whether the transmit-ready pin function is enabled (txReady bit 0).");

    private static readonly Field PolarityField = UbxVisitor.Enum(nameof(Polarity), Int32Type.Default, "Active polarity of the transmit-ready signal (txReady bit 1).",
        new EnumValue<int>((int)PortPolarity.HighActive, nameof(PortPolarity.HighActive)),
        new EnumValue<int>((int)PortPolarity.LowActive, nameof(PortPolarity.LowActive)));

    private static readonly Field PinField = UbxVisitor.Field(nameof(Pin), UInt8Type.Default, "PIO number for the transmit-ready signal, encoded in txReady bits 2 through 6.");

    private static readonly Field ThresholdField = UbxVisitor.Field(nameof(Threshold), UInt16Type.Default, "Transmit-ready threshold in units of eight bytes, encoded in txReady bits 7 through 15.", "8 bytes");

    private static readonly Field DataBitsField = UbxVisitor.Field(nameof(DataBits), Int32Type.Default, "UART character length in bits; the documented receiver modes support 7 or 8 data bits.");

    private static readonly Field ParityField = UbxVisitor.Enum(nameof(Parity), Int32Type.Default, "UART parity selection encoded in the port mode.",
        new EnumValue<int>((int)Parity.None, nameof(Parity.None)),
        new EnumValue<int>((int)Parity.Odd, nameof(Parity.Odd)),
        new EnumValue<int>((int)Parity.Even, nameof(Parity.Even)),
        new EnumValue<int>((int)Parity.Mark, nameof(Parity.Mark)),
        new EnumValue<int>((int)Parity.Space, nameof(Parity.Space)));

    private static readonly Field StopBitsField = UbxVisitor.Enum(nameof(StopBits), Int32Type.Default, "UART stop-bit selection encoded in the port mode.",
        new EnumValue<int>((int)StopBits.None, nameof(StopBits.None)),
        new EnumValue<int>((int)StopBits.One, nameof(StopBits.One)),
        new EnumValue<int>((int)StopBits.Two, nameof(StopBits.Two)),
        new EnumValue<int>((int)StopBits.OnePointFive, nameof(StopBits.OnePointFive)));

    private static readonly Field BoundRateField = UbxVisitor.Field(nameof(BoundRate), Int32Type.Default, "UART baud rate in bits per second; supported rates depend on the receiver.", "bit/s");

    /// <summary>Field metadata for this receiver-port configuration.</summary>
    public new static StructType VisitorType { get; } = new([..UbxCfgPrtConfig.VisitorType.Fields, PortIdField, IsEnableField, PolarityField, PinField, ThresholdField, DataBitsField, ParityField, StopBitsField, BoundRateField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt8Type.Accept(visitor, PortIdField, ref _portId);
        BoolType.Accept(visitor, IsEnableField, ref _isEnable);
        var polarity = (int)_polarity;
        Int32Type.Accept(visitor, PolarityField, ref polarity);
        _polarity = (PortPolarity)polarity;
        UInt8Type.Accept(visitor, PinField, ref _pin);
        UInt16Type.Accept(visitor, ThresholdField, ref _threshold);
        Int32Type.Accept(visitor, DataBitsField, ref _dataBits);
        var parity = (int)_parity;
        Int32Type.Accept(visitor, ParityField, ref parity);
        _parity = (Parity)parity;
        var stopBits = (int)_stopBits;
        Int32Type.Accept(visitor, StopBitsField, ref stopBits);
        _stopBits = (StopBits)stopBits;
        Int32Type.Accept(visitor, BoundRateField, ref _boundRate);
    }
}
