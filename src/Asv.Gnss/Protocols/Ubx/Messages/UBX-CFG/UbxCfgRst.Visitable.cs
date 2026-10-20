using Asv.IO;

namespace Asv.Gnss;

public partial class UbxCfgRst
{
    private static readonly Field BbrField = UbxVisitor.Enum(nameof(Bbr), UInt16Type.Default, "Battery-backed data to clear before reset; HotStart preserves these data, WarmStart clears ephemerides, and ColdStart clears all selected data.",
        new EnumValue<ushort>((ushort)BbrMask.HotStart, nameof(BbrMask.HotStart)),
        new EnumValue<ushort>((ushort)BbrMask.WarmStart, nameof(BbrMask.WarmStart)),
        new EnumValue<ushort>((ushort)BbrMask.ColdStart, nameof(BbrMask.ColdStart)));

    private static readonly Field ModeField = UbxVisitor.Enum(nameof(Mode), UInt8Type.Default, "Requested reset mechanism, including hardware reset, software reset, and GNSS-only stop or restart.",
        new EnumValue<byte>((byte)ResetMode.HardwareResetImmediately, nameof(ResetMode.HardwareResetImmediately)),
        new EnumValue<byte>((byte)ResetMode.ControlledSoftwareReset, nameof(ResetMode.ControlledSoftwareReset)),
        new EnumValue<byte>((byte)ResetMode.ControlledSoftwareResetGnssOnly, nameof(ResetMode.ControlledSoftwareResetGnssOnly)),
        new EnumValue<byte>((byte)ResetMode.HardwareResetAfterShutdown, nameof(ResetMode.HardwareResetAfterShutdown)),
        new EnumValue<byte>((byte)ResetMode.ControlledGnssStop, nameof(ResetMode.ControlledGnssStop)),
        new EnumValue<byte>((byte)ResetMode.ControlledGnssStart, nameof(ResetMode.ControlledGnssStart)));

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var bbr = (ushort)_bbr;
        UInt16Type.Accept(visitor, BbrField, ref bbr);
        _bbr = (BbrMask)bbr;
        var mode = (byte)_mode;
        UInt8Type.Accept(visitor, ModeField, ref mode);
        _mode = (ResetMode)mode;
    }
}
