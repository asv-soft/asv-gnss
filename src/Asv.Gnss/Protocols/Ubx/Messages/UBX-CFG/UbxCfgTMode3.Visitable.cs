using Asv.IO;

namespace Asv.Gnss;

public partial class UbxCfgTMode3
{
    private static readonly Field VersionField = UbxVisitor.Field(nameof(Version), UInt8Type.Default, "Payload format version; the documented CFG-TMODE3 format uses version 0.");

    private static readonly Field IsGivenInLLAField = UbxVisitor.Field(nameof(IsGivenInLLA), BoolType.Default, "Position is given in LAT/LON/ALT (default is ECEF)");

    private static readonly Field ModeField = UbxVisitor.Enum(nameof(Mode), UInt16Type.Default, "Receiver Mode",
        new EnumValue<ushort>((ushort)TMode3Enum.Disabled, nameof(TMode3Enum.Disabled)),
        new EnumValue<ushort>((ushort)TMode3Enum.SurveyIn, nameof(TMode3Enum.SurveyIn)),
        new EnumValue<ushort>((ushort)TMode3Enum.FixedMode, nameof(TMode3Enum.FixedMode)));

    private static readonly Field SurveyInPositionAccuracyLimitField = UbxVisitor.Field(nameof(SurveyInPositionAccuracyLimit), DoubleType.Default, "Survey-in mean-position accuracy limit in meters; wire resolution is 0.1 mm. Survey-in completes only after this limit and the minimum duration are both satisfied.", "m");

    private static readonly Field SurveyInMinDurationField = UbxVisitor.Field(nameof(SurveyInMinDuration), UInt32Type.Default, "Minimum survey-in observation duration in seconds; completion also requires the configured position accuracy.", "s");

    private static readonly Field FixedPosition3DAccuracyField = UbxVisitor.Field(nameof(FixedPosition3DAccuracy), DoubleType.Default, "Accuracy assigned to the fixed reference position, in meters; wire resolution is 0.1 mm.", "m");

    private static readonly Field LocationField = UbxVisitor.Field(nameof(Location), UbxVisitor.LocationType, "Fixed reference position: latitude and longitude in degrees and ellipsoidal height in meters. Required in FixedMode. The wire coordinates are geodetic or WGS84 ECEF according to IsGivenInLLA.");

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt8Type.Accept(visitor, VersionField, ref _version);
        BoolType.Accept(visitor, IsGivenInLLAField, ref _isGivenInLLA);
        var mode = (ushort)_mode;
        UInt16Type.Accept(visitor, ModeField, ref mode);
        _mode = (TMode3Enum)mode;
        DoubleType.Accept(visitor, SurveyInPositionAccuracyLimitField, ref _surveyInPositionAccuracyLimit);
        UInt32Type.Accept(visitor, SurveyInMinDurationField, ref _surveyInMinDuration);
        DoubleType.Accept(visitor, FixedPosition3DAccuracyField, ref _fixedPosition3DAccuracy);
        UbxVisitor.VisitLocation(visitor, LocationField, ref _location);
    }
}
