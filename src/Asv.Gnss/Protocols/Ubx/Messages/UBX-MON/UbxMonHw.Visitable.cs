using Asv.IO;

namespace Asv.Gnss;

public partial class UbxMonHw
{
    private static readonly Field PinSelField = UbxVisitor.Field(nameof(PinSel), Int32Type.Default, "Pin-selection bit mask: each bit distinguishes peripheral selection from GPIO selection.");
    private static readonly Field PinBankField = UbxVisitor.Field(nameof(PinBank), Int32Type.Default, "Pin-bank bit mask reported by the receiver hardware.");
    private static readonly Field PinDirField = UbxVisitor.Field(nameof(PinDir), Int32Type.Default, "GPIO direction bit mask; a set bit denotes an output.");
    private static readonly Field PinValField = UbxVisitor.Field(nameof(PinVal), Int32Type.Default, "GPIO logic-level bit mask; a set bit denotes a high level.");
    private static readonly Field NoiseField = UbxVisitor.Field(nameof(Noise), UInt16Type.Default, "Receiver noise measurement per millisecond (noisePerMS); this is a hardware-dependent diagnostic count.", "counts/ms");
    private static readonly Field AgcCntField = UbxVisitor.Field(nameof(AgcCnt), UInt16Type.Default, "Automatic gain control count, with a documented range of 0 to 8191.", "count");
    private static readonly Field AStatusField = UbxVisitor.Enum(nameof(AStatus), Int32Type.Default, "Antenna supervisor state; interpret only when antenna supervision is supported and enabled.",
            new EnumValue<int>(0, "Initializing"),
            new EnumValue<int>(1, "Unknown"),
            new EnumValue<int>(2, "OK"),
            new EnumValue<int>(3, "Short circuit"),
            new EnumValue<int>(4, "Open circuit"));
    private static readonly Field APowerField = UbxVisitor.Enum(nameof(APower), Int32Type.Default, "Reported antenna power state: off, on, or unknown.",
            new EnumValue<int>(0, "Off"),
            new EnumValue<int>(1, "On"),
            new EnumValue<int>(2, "Unknown"));
    private static readonly Field RtcCalibField = UbxVisitor.Field(nameof(RtcCalib), BoolType.Default, "Whether the real-time clock has been calibrated.");
    private static readonly Field SafeBootField = UbxVisitor.Field(nameof(SafeBoot), BoolType.Default, "Whether the receiver is running in safe-boot mode.");
    private static readonly Field XTalAbsentField = UbxVisitor.Field(nameof(XTalAbsent), BoolType.Default, "Whether the RTC crystal is absent.");
    private static readonly Field UsedMaskField = UbxVisitor.Field(nameof(UsedMask), Int32Type.Default, "Bit mask identifying pins assigned to virtual-pin functions.");
    private static readonly Field VPField = UbxVisitor.Field(nameof(VP), new ArrayType(UInt8Type.Default, 17), "Seventeen virtual-pin mapping bytes; entries are hardware-dependent pin assignments.");
    private static readonly Field JamIndField = UbxVisitor.Field(nameof(JamInd), UInt8Type.Default, "Continuous-wave interference indicator, from 0 to 255; larger values indicate stronger interference.");
    private static readonly Field PinIrqField = UbxVisitor.Field(nameof(PinIrq), Int32Type.Default, "Bit mask identifying pins with an enabled interrupt.");
    private static readonly Field PullHField = UbxVisitor.Field(nameof(PullH), Int32Type.Default, "Bit mask identifying pins with a high pull resistor enabled.");
    private static readonly Field PullLField = UbxVisitor.Field(nameof(PullL), Int32Type.Default, "Bit mask identifying pins with a low pull resistor enabled.");
    private static readonly Field JammingStateField = UbxVisitor.Enum(nameof(JammingState), Int32Type.Default, "Interference monitor state: unknown or disabled, normal, warning, or critical.",
            new EnumValue<int>(0, "Unknown or disabled"),
            new EnumValue<int>(1, "OK"),
            new EnumValue<int>(2, "Warning"),
            new EnumValue<int>(3, "Critical"));
    private static readonly Field CwJammingIndicatorField = UbxVisitor.Field(nameof(CwJammingIndicator), DoubleType.Default, "Library-normalized continuous-wave interference indicator, equal to JamInd divided by 255.");
    private static readonly Field AgcMonitorField = UbxVisitor.Field(nameof(AgcMonitor), DoubleType.Default, "Library-normalized automatic gain control count, equal to AgcCnt divided by 8191.");

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        Int32Type.Accept(visitor, PinSelField, PinSelField.DataType, ref _pinSel);
        Int32Type.Accept(visitor, PinBankField, PinBankField.DataType, ref _pinBank);
        Int32Type.Accept(visitor, PinDirField, PinDirField.DataType, ref _pinDir);
        Int32Type.Accept(visitor, PinValField, PinValField.DataType, ref _pinVal);
        UInt16Type.Accept(visitor, NoiseField, NoiseField.DataType, ref _noise);
        UInt16Type.Accept(visitor, AgcCntField, AgcCntField.DataType, ref _agcCnt);
        var aStatus = (int)_aStatus;
        Int32Type.Accept(visitor, AStatusField, AStatusField.DataType, ref aStatus);
        _aStatus = (AntennaSupervisorStateMachineStatus)aStatus;
        var aPower = (int)_aPower;
        Int32Type.Accept(visitor, APowerField, APowerField.DataType, ref aPower);
        _aPower = (AntennaPowerStatus)aPower;
        BoolType.Accept(visitor, RtcCalibField, RtcCalibField.DataType, ref _rtcCalib);
        BoolType.Accept(visitor, SafeBootField, SafeBootField.DataType, ref _safeBoot);
        BoolType.Accept(visitor, XTalAbsentField, XTalAbsentField.DataType, ref _xTalAbsent);
        Int32Type.Accept(visitor, UsedMaskField, UsedMaskField.DataType, ref _usedMask);
        UbxVisitor.VisitArray(visitor, VPField, ref _vp, static () => default(byte),
            static (IVisitor v, Field f, IFieldType t, ref byte value) => UInt8Type.Accept(v, f, t, ref value));
        UInt8Type.Accept(visitor, JamIndField, JamIndField.DataType, ref _jamInd);
        Int32Type.Accept(visitor, PinIrqField, PinIrqField.DataType, ref _pinIrq);
        Int32Type.Accept(visitor, PullHField, PullHField.DataType, ref _pullH);
        Int32Type.Accept(visitor, PullLField, PullLField.DataType, ref _pullL);
        var jammingState = (int)_jammingState;
        Int32Type.Accept(visitor, JammingStateField, JammingStateField.DataType, ref jammingState);
        _jammingState = (JammingStateEnum)jammingState;
        DoubleType.Accept(visitor, CwJammingIndicatorField, CwJammingIndicatorField.DataType, ref _cwJammingIndicator);
        DoubleType.Accept(visitor, AgcMonitorField, AgcMonitorField.DataType, ref _agcMonitor);
    }
}
