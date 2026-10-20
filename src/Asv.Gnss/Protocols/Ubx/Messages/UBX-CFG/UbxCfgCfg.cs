using System;
using Asv.IO;

namespace Asv.Gnss;

public enum UbxCfgDeviceMask : byte
{
    DevBbr = 0b0000_0001,
    DevFlash = 0b0000_0010,
    DevEeprom = 0b0000_0100,
    DevSpiFlash = 0b0001_0000,
}

[Flags]
public enum UbxCfgSection : uint
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

/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.10.3. Receiver and firmware support must be checked against the applicable interface description.</remarks>
/// <remarks>ZED-F9P integration manual UBX-18010802 R08, section 3.1.6, describes limited legacy UBX-CFG compatibility and recommends the configuration-item interface.</remarks>
public partial class UbxCfgCfg : UbxMessageBase
{
    public override string Name => "UBX-CFG-CFG";
    public override byte Class => 0x06;
    public override byte SubClass => 0x09;

    private UbxCfgDeviceMask? _deviceMask;

    /// <summary>
    /// Optional storage-device mask. Null selects the 12-byte form without a device selector; otherwise the 13-byte form is used.
    /// </summary>
    public UbxCfgDeviceMask? DeviceMask
    {
        get => _deviceMask;
        set => _deviceMask = value;
    }
    private UbxCfgSection _clearMask;

    /// <summary>
    /// Configuration groups whose stored values are to be replaced with factory defaults. Combined operations execute in clear, save, load order.
    /// </summary>
    public UbxCfgSection ClearMask
    {
        get => _clearMask;
        set => _clearMask = value;
    }
    private UbxCfgSection _saveMask;

    /// <summary>
    /// Configuration groups to save to the selected nonvolatile storage.
    /// </summary>
    public UbxCfgSection SaveMask
    {
        get => _saveMask;
        set => _saveMask = value;
    }
    private UbxCfgSection _loadMask;

    /// <summary>
    /// Configuration groups to load from the selected storage into the active configuration.
    /// </summary>
    public UbxCfgSection LoadMask
    {
        get => _loadMask;
        set => _loadMask = value;
    }

    protected override void SerializeContent(ref Span<byte> buffer)
    {
        BinSerialize.WriteUInt(ref buffer, (uint)ClearMask);
        BinSerialize.WriteUInt(ref buffer, (uint)SaveMask);
        BinSerialize.WriteUInt(ref buffer, (uint)LoadMask);
        if (DeviceMask.HasValue)
        {
            BinSerialize.WriteByte(ref buffer, (byte)DeviceMask.Value);
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