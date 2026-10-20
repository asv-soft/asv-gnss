using Asv.IO;

namespace Asv.Gnss;

public partial class UbxCfgCfg
{
    private static readonly Field DeviceMaskField = UbxVisitor.Enum(nameof(DeviceMask), UInt8OptionalType.Default, "Optional storage-device mask. Null selects the 12-byte form without a device selector; otherwise the 13-byte form is used.",
        new EnumValue<byte>((byte)UbxCfgDeviceMask.DevBbr, nameof(UbxCfgDeviceMask.DevBbr)),
        new EnumValue<byte>((byte)UbxCfgDeviceMask.DevFlash, nameof(UbxCfgDeviceMask.DevFlash)),
        new EnumValue<byte>((byte)UbxCfgDeviceMask.DevEeprom, nameof(UbxCfgDeviceMask.DevEeprom)),
        new EnumValue<byte>((byte)UbxCfgDeviceMask.DevSpiFlash, nameof(UbxCfgDeviceMask.DevSpiFlash)));

    private static readonly Field ClearMaskField = UbxVisitor.Enum(nameof(ClearMask), UInt32Type.Default, "Configuration groups whose stored values are to be replaced with factory defaults. Combined operations execute in clear, save, load order.",
        new EnumValue<uint>((uint)UbxCfgSection.None, nameof(UbxCfgSection.None)),
        new EnumValue<uint>((uint)UbxCfgSection.All, nameof(UbxCfgSection.All)),
        new EnumValue<uint>((uint)UbxCfgSection.IoPort, nameof(UbxCfgSection.IoPort)),
        new EnumValue<uint>((uint)UbxCfgSection.MsgConf, nameof(UbxCfgSection.MsgConf)),
        new EnumValue<uint>((uint)UbxCfgSection.InfMsg, nameof(UbxCfgSection.InfMsg)),
        new EnumValue<uint>((uint)UbxCfgSection.NavConf, nameof(UbxCfgSection.NavConf)),
        new EnumValue<uint>((uint)UbxCfgSection.RxmConf, nameof(UbxCfgSection.RxmConf)),
        new EnumValue<uint>((uint)UbxCfgSection.SenConf, nameof(UbxCfgSection.SenConf)),
        new EnumValue<uint>((uint)UbxCfgSection.RinvConf, nameof(UbxCfgSection.RinvConf)),
        new EnumValue<uint>((uint)UbxCfgSection.AntConf, nameof(UbxCfgSection.AntConf)),
        new EnumValue<uint>((uint)UbxCfgSection.LogConf, nameof(UbxCfgSection.LogConf)),
        new EnumValue<uint>((uint)UbxCfgSection.FtsConf, nameof(UbxCfgSection.FtsConf)));

    private static readonly Field SaveMaskField = UbxVisitor.Enum(nameof(SaveMask), UInt32Type.Default, "Configuration groups to save to the selected nonvolatile storage.",
        new EnumValue<uint>((uint)UbxCfgSection.None, nameof(UbxCfgSection.None)),
        new EnumValue<uint>((uint)UbxCfgSection.All, nameof(UbxCfgSection.All)),
        new EnumValue<uint>((uint)UbxCfgSection.IoPort, nameof(UbxCfgSection.IoPort)),
        new EnumValue<uint>((uint)UbxCfgSection.MsgConf, nameof(UbxCfgSection.MsgConf)),
        new EnumValue<uint>((uint)UbxCfgSection.InfMsg, nameof(UbxCfgSection.InfMsg)),
        new EnumValue<uint>((uint)UbxCfgSection.NavConf, nameof(UbxCfgSection.NavConf)),
        new EnumValue<uint>((uint)UbxCfgSection.RxmConf, nameof(UbxCfgSection.RxmConf)),
        new EnumValue<uint>((uint)UbxCfgSection.SenConf, nameof(UbxCfgSection.SenConf)),
        new EnumValue<uint>((uint)UbxCfgSection.RinvConf, nameof(UbxCfgSection.RinvConf)),
        new EnumValue<uint>((uint)UbxCfgSection.AntConf, nameof(UbxCfgSection.AntConf)),
        new EnumValue<uint>((uint)UbxCfgSection.LogConf, nameof(UbxCfgSection.LogConf)),
        new EnumValue<uint>((uint)UbxCfgSection.FtsConf, nameof(UbxCfgSection.FtsConf)));

    private static readonly Field LoadMaskField = UbxVisitor.Enum(nameof(LoadMask), UInt32Type.Default, "Configuration groups to load from the selected storage into the active configuration.",
        new EnumValue<uint>((uint)UbxCfgSection.None, nameof(UbxCfgSection.None)),
        new EnumValue<uint>((uint)UbxCfgSection.All, nameof(UbxCfgSection.All)),
        new EnumValue<uint>((uint)UbxCfgSection.IoPort, nameof(UbxCfgSection.IoPort)),
        new EnumValue<uint>((uint)UbxCfgSection.MsgConf, nameof(UbxCfgSection.MsgConf)),
        new EnumValue<uint>((uint)UbxCfgSection.InfMsg, nameof(UbxCfgSection.InfMsg)),
        new EnumValue<uint>((uint)UbxCfgSection.NavConf, nameof(UbxCfgSection.NavConf)),
        new EnumValue<uint>((uint)UbxCfgSection.RxmConf, nameof(UbxCfgSection.RxmConf)),
        new EnumValue<uint>((uint)UbxCfgSection.SenConf, nameof(UbxCfgSection.SenConf)),
        new EnumValue<uint>((uint)UbxCfgSection.RinvConf, nameof(UbxCfgSection.RinvConf)),
        new EnumValue<uint>((uint)UbxCfgSection.AntConf, nameof(UbxCfgSection.AntConf)),
        new EnumValue<uint>((uint)UbxCfgSection.LogConf, nameof(UbxCfgSection.LogConf)),
        new EnumValue<uint>((uint)UbxCfgSection.FtsConf, nameof(UbxCfgSection.FtsConf)));

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var deviceMask = (byte?)_deviceMask;
        UInt8OptionalType.Accept(visitor, DeviceMaskField, ref deviceMask);
        _deviceMask = (UbxCfgDeviceMask?)deviceMask;
        var clearMask = (uint)_clearMask;
        UInt32Type.Accept(visitor, ClearMaskField, ref clearMask);
        _clearMask = (UbxCfgSection)clearMask;
        var saveMask = (uint)_saveMask;
        UInt32Type.Accept(visitor, SaveMaskField, ref saveMask);
        _saveMask = (UbxCfgSection)saveMask;
        var loadMask = (uint)_loadMask;
        UInt32Type.Accept(visitor, LoadMaskField, ref loadMask);
        _loadMask = (UbxCfgSection)loadMask;
    }
}
