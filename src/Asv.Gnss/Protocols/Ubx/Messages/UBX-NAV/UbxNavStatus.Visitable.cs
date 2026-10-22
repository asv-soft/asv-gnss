using Asv.IO;

namespace Asv.Gnss;

public partial class UbxNavStatus
{
    private static readonly Field iTOWField = UbxVisitor.Field(nameof(iTOW), UInt32Type.Default, "GPS time of week of the navigation epoch.", "ms");
    private static readonly Field GpsFixField = UbxVisitor.Field(nameof(GpsFix), UInt8Type.Default, "GPSfix Type, this value does not qualify a fix as valid and within the limits. See note on flag gpsFixOk below. 0x00 = no fix 0x01 = dead reckoning only 0x02 = 2D-fix 0x03 = 3D-fix 0x04 = GPS + dead reckoning combined 0x05 = Time only fix 0x06..0xff = reserved");
    private static readonly Field FlagsField = UbxVisitor.Field(nameof(Flags), UInt8Type.Default, "Navigation Status Flags (see graphic below)");
    private static readonly Field IsGpsFixOkField = UbxVisitor.Field(nameof(IsGpsFixOk), BoolType.Default, "Position and velocity valid and within DOP and ACC Masks.");
    private static readonly Field IsDiffSolnField = UbxVisitor.Field(nameof(IsDiffSoln), BoolType.Default, "Differential corrections were applied.");
    private static readonly Field IsWknSetField = UbxVisitor.Field(nameof(IsWknSet), BoolType.Default, "Week Number valid.");
    private static readonly Field IsTowSetField = UbxVisitor.Field(nameof(IsTowSet), BoolType.Default, "Time of Week valid.");
    private static readonly Field FixStatField = UbxVisitor.Field(nameof(FixStat), UInt8Type.Default, "Fix Status Information (see graphic below)");
    private static readonly Field IsIndoorDetectedField = UbxVisitor.Field(nameof(IsIndoorDetected), BoolType.Default, "Indoor-detection flag; defined for protocol versions 52.00 and later, not X20 protocol 50.11.");
    private static readonly Field SelectiveL5StateField = UbxVisitor.Field(nameof(SelectiveL5State), UInt8Type.Default, "Selective L5 state: 0 disabled, 1 L5 off, 2 L5 on, 3 reserved; defined for protocol versions 52.00 and later.");
    private static readonly Field IsDiffCorrField = UbxVisitor.Field(nameof(IsDiffCorr), BoolType.Default, "Differential corrections available.");
    private static readonly Field IsCarrSolnValidField = UbxVisitor.Field(nameof(IsCarrSolnValid), BoolType.Default, "Valid carrSoln.");
    private static readonly Field MapMatchingField = UbxVisitor.Enum<int>(nameof(MapMatching), Int32Type.Default, "Map matching status.",
        new EnumValue<int>((int)MapMatchingStatus.None, nameof(MapMatchingStatus.None)),
        new EnumValue<int>((int)MapMatchingStatus.ValidButNotUsed, nameof(MapMatchingStatus.ValidButNotUsed)),
        new EnumValue<int>((int)MapMatchingStatus.ValidAndUsed, nameof(MapMatchingStatus.ValidAndUsed)),
        new EnumValue<int>((int)MapMatchingStatus.ValidAndUsedSensor, nameof(MapMatchingStatus.ValidAndUsedSensor)));
    private static readonly Field Flags2Field = UbxVisitor.Field(nameof(Flags2), UInt8Type.Default, "further information about navigation output (see graphic below)");
    private static readonly Field PsmStateField = UbxVisitor.Enum<int>(nameof(PsmState), Int32Type.Default, "Power-save operating state decoded from flags2 bits 0 and 1.",
        new EnumValue<int>((int)PowerSaveModeState.Acquisition, nameof(PowerSaveModeState.Acquisition)),
        new EnumValue<int>((int)PowerSaveModeState.Tracking, nameof(PowerSaveModeState.Tracking)),
        new EnumValue<int>((int)PowerSaveModeState.PowerOptimizedTracking, nameof(PowerSaveModeState.PowerOptimizedTracking)),
        new EnumValue<int>((int)PowerSaveModeState.Inactive, nameof(PowerSaveModeState.Inactive)));
    private static readonly Field SpoofDetStateField = UbxVisitor.Enum<int>(nameof(SpoofDetState), Int32Type.Default, "Spoofing detector status decoded from flags2 bits 3 and 4; an unknown state does not establish absence of spoofing.",
        new EnumValue<int>((int)SpoofingDetectionState.UnknownOrDeactivated, nameof(SpoofingDetectionState.UnknownOrDeactivated)),
        new EnumValue<int>((int)SpoofingDetectionState.NoSpoofingIndicated, nameof(SpoofingDetectionState.NoSpoofingIndicated)),
        new EnumValue<int>((int)SpoofingDetectionState.SpoofingIndicated, nameof(SpoofingDetectionState.SpoofingIndicated)),
        new EnumValue<int>((int)SpoofingDetectionState.MultipleSpoofingIndications, nameof(SpoofingDetectionState.MultipleSpoofingIndications)));
    private static readonly Field CarrSolnField = UbxVisitor.Enum<int>(nameof(CarrSoln), Int32Type.Default, "Carrier-phase solution state decoded from flags2 bits 6 and 7; interpret together with the carrier-solution validity flag.",
        new EnumValue<int>((int)CPRSolutionStatus.NoCPRSolution, nameof(CPRSolutionStatus.NoCPRSolution)),
        new EnumValue<int>((int)CPRSolutionStatus.CPRWithFloatingAmbiguities, nameof(CPRSolutionStatus.CPRWithFloatingAmbiguities)),
        new EnumValue<int>((int)CPRSolutionStatus.CPRWithFixedAmbiguities, nameof(CPRSolutionStatus.CPRWithFixedAmbiguities)));
    private static readonly Field TTFFField = UbxVisitor.Field(nameof(TTFF), UInt32Type.Default, "Time to first fix (millisecond time tag)", "ms");
    private static readonly Field MSSSField = UbxVisitor.Field(nameof(MSSS), UInt32Type.Default, "Milliseconds since Startup / Reset", "ms");

    /// <summary>Describes the public navigation payload fields exposed to visitors.</summary>
    public static StructType StructType { get; } = new([
        iTOWField,
        GpsFixField,
        FlagsField,
        IsGpsFixOkField,
        IsDiffSolnField,
        IsWknSetField,
        IsTowSetField,
        FixStatField,
        IsIndoorDetectedField,
        SelectiveL5StateField,
        IsDiffCorrField,
        IsCarrSolnValidField,
        MapMatchingField,
        Flags2Field,
        PsmStateField,
        SpoofDetStateField,
        CarrSolnField,
        TTFFField,
        MSSSField
    ]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt32Type.Accept(visitor, iTOWField, iTOWField.DataType, ref _iTOW);
        UInt8Type.Accept(visitor, GpsFixField, GpsFixField.DataType, ref _gpsFix);
        UInt8Type.Accept(visitor, FlagsField, FlagsField.DataType, ref _flags);
        BoolType.Accept(visitor, IsGpsFixOkField, IsGpsFixOkField.DataType, ref _isGpsFixOk);
        BoolType.Accept(visitor, IsDiffSolnField, IsDiffSolnField.DataType, ref _isDiffSoln);
        BoolType.Accept(visitor, IsWknSetField, IsWknSetField.DataType, ref _isWknSet);
        BoolType.Accept(visitor, IsTowSetField, IsTowSetField.DataType, ref _isTowSet);
        UInt8Type.Accept(visitor, FixStatField, FixStatField.DataType, ref _fixStat);
        var isIndoorDetected = IsIndoorDetected;
        BoolType.Accept(visitor, IsIndoorDetectedField, IsIndoorDetectedField.DataType, ref isIndoorDetected);
        IsIndoorDetected = isIndoorDetected;
        var selectiveL5State = SelectiveL5State;
        UInt8Type.Accept(visitor, SelectiveL5StateField, SelectiveL5StateField.DataType, ref selectiveL5State);
        SelectiveL5State = selectiveL5State;
        BoolType.Accept(visitor, IsDiffCorrField, IsDiffCorrField.DataType, ref _isDiffCorr);
        BoolType.Accept(visitor, IsCarrSolnValidField, IsCarrSolnValidField.DataType, ref _isCarrSolnValid);
        var mapMatchingValue = (int)MapMatching;
        Int32Type.Accept(visitor, MapMatchingField, MapMatchingField.DataType, ref mapMatchingValue);
        MapMatching = (MapMatchingStatus)mapMatchingValue;
        UInt8Type.Accept(visitor, Flags2Field, Flags2Field.DataType, ref _flags2);
        var psmStateValue = (int)PsmState;
        Int32Type.Accept(visitor, PsmStateField, PsmStateField.DataType, ref psmStateValue);
        PsmState = (PowerSaveModeState)psmStateValue;
        var spoofDetStateValue = (int)SpoofDetState;
        Int32Type.Accept(visitor, SpoofDetStateField, SpoofDetStateField.DataType, ref spoofDetStateValue);
        SpoofDetState = (SpoofingDetectionState)spoofDetStateValue;
        var carrSolnValue = (int)CarrSoln;
        Int32Type.Accept(visitor, CarrSolnField, CarrSolnField.DataType, ref carrSolnValue);
        CarrSoln = (CPRSolutionStatus)carrSolnValue;
        UInt32Type.Accept(visitor, TTFFField, TTFFField.DataType, ref _ttff);
        UInt32Type.Accept(visitor, MSSSField, MSSSField.DataType, ref _msss);
    }
}
