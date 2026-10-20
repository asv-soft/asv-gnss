using Asv.IO;

namespace Asv.Gnss;

public partial class UbxCfgAnt
{
    private static readonly Field ReconfigField = UbxVisitor.Field(nameof(Reconfig), BoolType.Default, "Whether to apply the antenna pin assignment in this message.");

    private static readonly Field PinOCDField = UbxVisitor.Field(nameof(PinOCD), UInt8Type.Default, "PIO number used for antenna open-circuit detection.");

    private static readonly Field PinSCDField = UbxVisitor.Field(nameof(PinSCD), UInt8Type.Default, "PIO number used for antenna short-circuit detection.");

    private static readonly Field PinSwitchField = UbxVisitor.Field(nameof(PinSwitch), UInt8Type.Default, "PIO number used to switch antenna power.");

    private static readonly Field FlagsField = UbxVisitor.Enum(nameof(Flags), UInt16Type.Default, "Antenna supervision options for power control, fault detection and recovery.",
        new EnumValue<ushort>((ushort)UbxCfgAntFlags.Svcs, nameof(UbxCfgAntFlags.Svcs)),
        new EnumValue<ushort>((ushort)UbxCfgAntFlags.Scd, nameof(UbxCfgAntFlags.Scd)),
        new EnumValue<ushort>((ushort)UbxCfgAntFlags.Ocd, nameof(UbxCfgAntFlags.Ocd)),
        new EnumValue<ushort>((ushort)UbxCfgAntFlags.PdwnOnScd, nameof(UbxCfgAntFlags.PdwnOnScd)),
        new EnumValue<ushort>((ushort)UbxCfgAntFlags.Recovery, nameof(UbxCfgAntFlags.Recovery)));

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        BoolType.Accept(visitor, ReconfigField, ref _reconfig);
        UInt8Type.Accept(visitor, PinOCDField, ref _pinOCD);
        UInt8Type.Accept(visitor, PinSCDField, ref _pinSCD);
        UInt8Type.Accept(visitor, PinSwitchField, ref _pinSwitch);
        var flags = (ushort)_flags;
        UInt16Type.Accept(visitor, FlagsField, ref flags);
        _flags = (UbxCfgAntFlags)flags;
    }
}
