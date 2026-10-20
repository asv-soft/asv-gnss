using Asv.IO;

namespace Asv.Gnss;

public partial class UbxCfgNav5
{
    private static readonly Field ApplyDynamicModelField = UbxVisitor.Field(nameof(ApplyDynamicModel), BoolType.Default, "Whether the NAV5 update mask applies the platform model.");

    private static readonly Field ApplyMinimumElevationField = UbxVisitor.Field(nameof(ApplyMinimumElevation), BoolType.Default, "Whether the NAV5 update mask applies the minimum elevation.");

    private static readonly Field ApplyFixModeField = UbxVisitor.Field(nameof(ApplyFixMode), BoolType.Default, "Whether the NAV5 update mask applies the 2D/3D fix policy.");

    private static readonly Field ApplyDrLimitField = UbxVisitor.Field(nameof(ApplyDrLimit), BoolType.Default, "Legacy update-mask bit for the dead-reckoning limit; reserved in the R23 NAV5 mask definition.");

    private static readonly Field ApplyPositionMaskField = UbxVisitor.Field(nameof(ApplyPositionMask), BoolType.Default, "Whether the NAV5 update mask applies the position DOP and accuracy masks.");

    private static readonly Field ApplyTimeMaskField = UbxVisitor.Field(nameof(ApplyTimeMask), BoolType.Default, "Whether the NAV5 update mask applies the time DOP and accuracy masks.");

    private static readonly Field ApplyStaticHoldField = UbxVisitor.Field(nameof(ApplyStaticHold), BoolType.Default, "Whether the NAV5 update mask applies the static-hold settings.");

    private static readonly Field ApplyDGPSField = UbxVisitor.Field(nameof(ApplyDGPS), BoolType.Default, "Whether the NAV5 update mask applies the differential correction timeout.");

    private static readonly Field ApplyCnoThresholdField = UbxVisitor.Field(nameof(ApplyCnoThreshold), BoolType.Default, "Whether the NAV5 update mask applies the carrier-to-noise thresholds.");

    private static readonly Field ApplyUTCField = UbxVisitor.Field(nameof(ApplyUTC), BoolType.Default, "Whether the NAV5 update mask applies the UTC standard selector.");

    private static readonly Field PlatformModelField = UbxVisitor.Enum(nameof(PlatformModel), Int32Type.Default, "Dynamic platform model used by the navigation engine; supported models depend on firmware.",
        new EnumValue<int>((int)ModelEnum.Portable, nameof(ModelEnum.Portable)),
        new EnumValue<int>((int)ModelEnum.Stationary, nameof(ModelEnum.Stationary)),
        new EnumValue<int>((int)ModelEnum.Pedestrian, nameof(ModelEnum.Pedestrian)),
        new EnumValue<int>((int)ModelEnum.Automotive, nameof(ModelEnum.Automotive)),
        new EnumValue<int>((int)ModelEnum.Sea, nameof(ModelEnum.Sea)),
        new EnumValue<int>((int)ModelEnum.AirborneWithLess1gAcceleration, nameof(ModelEnum.AirborneWithLess1gAcceleration)),
        new EnumValue<int>((int)ModelEnum.AirborneWithLess2gAcceleration, nameof(ModelEnum.AirborneWithLess2gAcceleration)),
        new EnumValue<int>((int)ModelEnum.AirborneWithLess4gAcceleration, nameof(ModelEnum.AirborneWithLess4gAcceleration)),
        new EnumValue<int>((int)ModelEnum.WristWornWatch, nameof(ModelEnum.WristWornWatch)),
        new EnumValue<int>((int)ModelEnum.Bike, nameof(ModelEnum.Bike)));

    private static readonly Field PositionModeField = UbxVisitor.Enum(nameof(PositionMode), Int32Type.Default, "Position solution policy: 2D only, 3D only, or automatic 2D/3D.",
        new EnumValue<int>((int)PositionModeEnum.Only2D, nameof(PositionModeEnum.Only2D)),
        new EnumValue<int>((int)PositionModeEnum.Only3D, nameof(PositionModeEnum.Only3D)),
        new EnumValue<int>((int)PositionModeEnum.Auto, nameof(PositionModeEnum.Auto)));

    private static readonly Field FixedAltitudeField = UbxVisitor.Field(nameof(FixedAltitude), DoubleType.Default, "Altitude constraint for 2D fixes in meters above mean sea level; wire resolution is 0.01 m.", "m");

    private static readonly Field FixedAltitudeVarianceField = UbxVisitor.Field(nameof(FixedAltitudeVariance), DoubleType.Default, "Variance of the fixed-altitude constraint, in square meters; wire resolution is 0.0001 square meters.", "m^2");

    private static readonly Field DrLimitField = UbxVisitor.Field(nameof(DrLimit), UInt8Type.Default, "Legacy dead-reckoning limit byte; reserved in the R23 NAV5 field table.");

    private static readonly Field MinimumElevationField = UbxVisitor.Field(nameof(MinimumElevation), Int8Type.Default, "Minimum satellite elevation for navigation use, in degrees.", "deg");

    private static readonly Field PositionDOPField = UbxVisitor.Field(nameof(PositionDOP), DoubleType.Default, "Position dilution-of-precision mask, dimensionless; wire resolution is 0.1.", "1");

    private static readonly Field TimeDOPField = UbxVisitor.Field(nameof(TimeDOP), DoubleType.Default, "Time dilution-of-precision mask, dimensionless; wire resolution is 0.1.", "1");

    private static readonly Field PositionAccuracyField = UbxVisitor.Field(nameof(PositionAccuracy), UInt16Type.Default, "Position accuracy mask in meters.", "m");

    private static readonly Field TimeAccuracyField = UbxVisitor.Field(nameof(TimeAccuracy), UInt16Type.Default, "Time accuracy mask expressed as a distance in meters, as specified for the NAV5 tAcc field.", "m");

    private static readonly Field StaticHoldThresholdField = UbxVisitor.Field(nameof(StaticHoldThreshold), DoubleType.Default, "Speed threshold for static hold, in meters per second; wire resolution is 0.01 m/s.", "m/s");

    private static readonly Field DGNSSTimeoutField = UbxVisitor.Field(nameof(DGNSSTimeout), UInt8Type.Default, "Maximum age of differential GNSS corrections, in seconds.", "s");

    private static readonly Field CnoThreshNumSVsField = UbxVisitor.Field(nameof(CnoThreshNumSVs), UInt8Type.Default, "Minimum number of satellites that must meet the configured carrier-to-noise threshold.");

    private static readonly Field CnoThresholdField = UbxVisitor.Field(nameof(CnoThreshold), UInt8Type.Default, "Carrier-to-noise density threshold in dB-Hz.", "dB-Hz");

    private static readonly Field StaticHoldMaxDistanceField = UbxVisitor.Field(nameof(StaticHoldMaxDistance), UInt16Type.Default, "Maximum displacement from the static-hold position, in meters.", "m");

    private static readonly Field UtcStandardField = UbxVisitor.Field(nameof(UtcStandard), UInt8Type.Default, "UTC realization selector: 0 automatic, 3 USNO, 5 European, 6 SU, or 7 NTSC; support depends on firmware.");

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        BoolType.Accept(visitor, ApplyDynamicModelField, ref _applyDynamicModel);
        BoolType.Accept(visitor, ApplyMinimumElevationField, ref _applyMinimumElevation);
        BoolType.Accept(visitor, ApplyFixModeField, ref _applyFixMode);
        BoolType.Accept(visitor, ApplyDrLimitField, ref _applyDrLimit);
        BoolType.Accept(visitor, ApplyPositionMaskField, ref _applyPositionMask);
        BoolType.Accept(visitor, ApplyTimeMaskField, ref _applyTimeMask);
        BoolType.Accept(visitor, ApplyStaticHoldField, ref _applyStaticHold);
        BoolType.Accept(visitor, ApplyDGPSField, ref _applyDGPS);
        BoolType.Accept(visitor, ApplyCnoThresholdField, ref _applyCnoThreshold);
        BoolType.Accept(visitor, ApplyUTCField, ref _applyUTC);
        var platformModel = (int)_platformModel;
        Int32Type.Accept(visitor, PlatformModelField, ref platformModel);
        _platformModel = (ModelEnum)platformModel;
        var positionMode = (int)_positionMode;
        Int32Type.Accept(visitor, PositionModeField, ref positionMode);
        _positionMode = (PositionModeEnum)positionMode;
        DoubleType.Accept(visitor, FixedAltitudeField, ref _fixedAltitude);
        DoubleType.Accept(visitor, FixedAltitudeVarianceField, ref _fixedAltitudeVariance);
        UInt8Type.Accept(visitor, DrLimitField, ref _drLimit);
        Int8Type.Accept(visitor, MinimumElevationField, ref _minimumElevation);
        DoubleType.Accept(visitor, PositionDOPField, ref _positionDOP);
        DoubleType.Accept(visitor, TimeDOPField, ref _timeDOP);
        UInt16Type.Accept(visitor, PositionAccuracyField, ref _positionAccuracy);
        UInt16Type.Accept(visitor, TimeAccuracyField, ref _timeAccuracy);
        DoubleType.Accept(visitor, StaticHoldThresholdField, ref _staticHoldThreshold);
        UInt8Type.Accept(visitor, DGNSSTimeoutField, ref _dgnssTimeout);
        UInt8Type.Accept(visitor, CnoThreshNumSVsField, ref _cnoThreshNumSVs);
        UInt8Type.Accept(visitor, CnoThresholdField, ref _cnoThreshold);
        UInt16Type.Accept(visitor, StaticHoldMaxDistanceField, ref _staticHoldMaxDistance);
        UInt8Type.Accept(visitor, UtcStandardField, ref _utcStandard);
    }
}
