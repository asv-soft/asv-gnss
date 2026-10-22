using System.Collections.Immutable;
using Asv.IO;

namespace Asv.Gnss;

public sealed partial class JavadReferenceStationStatusMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(0, JavadVisitor.Field("UtcValidity", JavadTextVisitor.CharacterType, "Utc Validity")),
        new(1, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
        new(
            2,
            JavadVisitor.Field(
                "ReferenceLocation",
                JavadReferenceLocationStatus.StructType,
                "Reference Location"
            )
        ),
    ];
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadCorrectionTextMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields = [];
    private static readonly Field CountField = JavadVisitor.Field(
        "MessageCount",
        Int32Type.Default,
        "Message Count"
    );
    private static readonly Field GroupsField = JavadVisitor.Field(
        "Messages",
        new ListType(JavadCorrectionText.StructType, 0, 4095),
        "Messages"
    );
    public static StructType StructType { get; } =
        new([ExtensionDataField, CountField, GroupsField]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        JavadTextVisitor.VisitRepeated(visitor, CountField, GroupsField, 0, 1, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadBaselineTextMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(0, JavadVisitor.Field("UtcValidity", JavadTextVisitor.CharacterType, "Utc Validity")),
        new(1, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
        new(
            2,
            JavadVisitor.Field(
                "BaselineValidity",
                JavadTextVisitor.CharacterType,
                "Baseline Validity"
            )
        ),
        new(
            3,
            JavadVisitor.Field(
                "ComputationMode",
                JavadTextVisitor.CharacterType,
                "Computation Mode"
            )
        ),
        new(
            4,
            JavadVisitor.Field(
                "ReferenceMinusRoverX",
                DoubleOptionalType.Default,
                "Reference Minus RoverX",
                "m"
            )
        ),
        new(
            5,
            JavadVisitor.Field(
                "ReferenceMinusRoverY",
                DoubleOptionalType.Default,
                "Reference Minus RoverY",
                "m"
            )
        ),
        new(
            6,
            JavadVisitor.Field(
                "ReferenceMinusRoverZ",
                DoubleOptionalType.Default,
                "Reference Minus RoverZ",
                "m"
            )
        ),
        new(
            7,
            JavadVisitor.Field(
                "Position3DRmsMeters",
                DoubleOptionalType.Default,
                "Position3D Rms Meters",
                "m"
            )
        ),
    ];
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
            VisitorFields[3].Field,
            VisitorFields[4].Field,
            VisitorFields[5].Field,
            VisitorFields[6].Field,
            VisitorFields[7].Field,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadRtkParametersMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(0, JavadVisitor.Field("UtcValidity", JavadTextVisitor.CharacterType, "Utc Validity")),
        new(1, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
        new(
            2,
            JavadVisitor.Field(
                "RtkDataValidity",
                JavadTextVisitor.CharacterType,
                "Rtk Data Validity"
            )
        ),
        new(
            3,
            JavadVisitor.Field(
                "AmbiguityFixProgressPercent",
                Int32OptionalType.Default,
                "Ambiguity Fix Progress Percent",
                "%"
            )
        ),
        new(
            4,
            JavadVisitor.Field(
                "AmbiguityFixProbability",
                DoubleOptionalType.Default,
                "Ambiguity Fix Probability"
            )
        ),
        new(
            5,
            JavadVisitor.Field(
                "PositionChiSquared",
                DoubleOptionalType.Default,
                "Position Chi Squared"
            )
        ),
        new(
            6,
            JavadVisitor.Field(
                "ReceiverTimeShiftMeters",
                DoubleOptionalType.Default,
                "Receiver Time Shift Meters",
                "m"
            )
        ),
        new(
            7,
            JavadVisitor.Field(
                "TimeShiftDerivativeMetersPerSecond",
                DoubleOptionalType.Default,
                "Time Shift Derivative Meters Per Second",
                "m/s"
            )
        ),
        new(
            8,
            JavadVisitor.Field(
                "IonosphereRmsErrorMeters",
                DoubleOptionalType.Default,
                "Ionosphere Rms Error Meters",
                "m"
            )
        ),
        new(
            9,
            JavadVisitor.Field(
                "CorrectionsAgeSeconds",
                DoubleOptionalType.Default,
                "Corrections Age Seconds",
                "s"
            )
        ),
    ];
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
            VisitorFields[3].Field,
            VisitorFields[4].Field,
            VisitorFields[5].Field,
            VisitorFields[6].Field,
            VisitorFields[7].Field,
            VisitorFields[8].Field,
            VisitorFields[9].Field,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadImuMeasurementsMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(0, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
        new(
            1,
            JavadVisitor.Field(
                "AccelerationXMetersPerSecondSquared",
                DoubleOptionalType.Default,
                "AccelerationX Meters Per Second Squared",
                "m/s²"
            )
        ),
        new(
            2,
            JavadVisitor.Field(
                "AccelerationYMetersPerSecondSquared",
                DoubleOptionalType.Default,
                "AccelerationY Meters Per Second Squared",
                "m/s²"
            )
        ),
        new(
            3,
            JavadVisitor.Field(
                "AccelerationZMetersPerSecondSquared",
                DoubleOptionalType.Default,
                "AccelerationZ Meters Per Second Squared",
                "m/s²"
            )
        ),
        new(
            4,
            JavadVisitor.Field(
                "AngularVelocityXDegreesPerSecond",
                DoubleOptionalType.Default,
                "Angular VelocityX Degrees Per Second",
                "deg/s"
            )
        ),
        new(
            5,
            JavadVisitor.Field(
                "AngularVelocityYDegreesPerSecond",
                DoubleOptionalType.Default,
                "Angular VelocityY Degrees Per Second",
                "deg/s"
            )
        ),
        new(
            6,
            JavadVisitor.Field(
                "AngularVelocityZDegreesPerSecond",
                DoubleOptionalType.Default,
                "Angular VelocityZ Degrees Per Second",
                "deg/s"
            )
        ),
    ];
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
            VisitorFields[3].Field,
            VisitorFields[4].Field,
            VisitorFields[5].Field,
            VisitorFields[6].Field,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadImuAntennaVelocityMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(
            0,
            JavadVisitor.Field(
                "SolutionValidity",
                JavadTextVisitor.CharacterType,
                "Solution Validity"
            )
        ),
        new(1, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
        new(
            2,
            JavadVisitor.Field(
                "NorthVelocityMetersPerSecond",
                DoubleOptionalType.Default,
                "North Velocity Meters Per Second",
                "m/s"
            )
        ),
        new(
            3,
            JavadVisitor.Field(
                "EastVelocityMetersPerSecond",
                DoubleOptionalType.Default,
                "East Velocity Meters Per Second",
                "m/s"
            )
        ),
        new(
            4,
            JavadVisitor.Field(
                "DownVelocityMetersPerSecond",
                DoubleOptionalType.Default,
                "Down Velocity Meters Per Second",
                "m/s"
            )
        ),
    ];
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
            VisitorFields[3].Field,
            VisitorFields[4].Field,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadLeverArmPositionTextMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(0, JavadVisitor.Field("UtcValidity", JavadTextVisitor.CharacterType, "Utc Validity")),
        new(1, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
        new(2, JavadVisitor.Field("PositionStatus", Int32OptionalType.Default, "Position Status")),
        new(
            3,
            JavadVisitor.Field("ComputationModes", JavadTextVisitor.TextType, "Computation Modes")
        ),
        new(
            4,
            JavadVisitor.Field(
                "SatellitesPerSystem",
                new ListType(Int32OptionalType.Default, 0, 4095),
                "Satellites Per System"
            )
        ),
        new(5, JavadVisitor.Field("GeodeticDatum", JavadTextVisitor.TextType, "Geodetic Datum")),
        new(6, JavadVisitor.Field("Latitude", JavadTextVisitor.TextType, "Latitude")),
        new(7, JavadVisitor.Field("Longitude", JavadTextVisitor.TextType, "Longitude")),
        new(
            8,
            JavadVisitor.Field("AltitudeMeters", DoubleOptionalType.Default, "Altitude Meters", "m")
        ),
        new(
            9,
            JavadVisitor.Field(
                "HorizontalPositionRmsMeters",
                DoubleOptionalType.Default,
                "Horizontal Position Rms Meters",
                "m"
            )
        ),
        new(
            10,
            JavadVisitor.Field(
                "VerticalPositionRmsMeters",
                DoubleOptionalType.Default,
                "Vertical Position Rms Meters",
                "m"
            )
        ),
    ];
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
            VisitorFields[3].Field,
            VisitorFields[4].Field,
            VisitorFields[5].Field,
            VisitorFields[6].Field,
            VisitorFields[7].Field,
            VisitorFields[8].Field,
            VisitorFields[9].Field,
            VisitorFields[10].Field,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadTimeResidualsMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(0, JavadVisitor.Field("UtcValidity", JavadTextVisitor.CharacterType, "Utc Validity")),
        new(1, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
    ];
    private static readonly Field CountField = JavadVisitor.Field(
        "SatelliteCount",
        Int32Type.Default,
        "Satellite Count"
    );
    private static readonly Field GroupsField = JavadVisitor.Field(
        "Residuals",
        new ListType(JavadTimeResidual.StructType, 0, 4095),
        "Residuals"
    );
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            CountField,
            GroupsField,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        JavadTextVisitor.VisitRepeated(visitor, CountField, GroupsField, 2, 3, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadRtcmStatusMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(0, JavadVisitor.Field("AgeSeconds", Int32OptionalType.Default, "Age Seconds", "s")),
        new(
            1,
            JavadVisitor.Field("ReceivedMessages", Int32OptionalType.Default, "Received Messages")
        ),
        new(
            2,
            JavadVisitor.Field("CorruptMessages", Int32OptionalType.Default, "Corrupt Messages")
        ),
        new(
            3,
            JavadVisitor.Field("QualityPercent", DoubleOptionalType.Default, "Quality Percent", "%")
        ),
    ];
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
            VisitorFields[3].Field,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadMapProjectionMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(0, JavadVisitor.Field("UtcValidity", JavadTextVisitor.CharacterType, "Utc Validity")),
        new(1, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
        new(2, JavadVisitor.Field("PositionStatus", Int32OptionalType.Default, "Position Status")),
        new(3, JavadVisitor.Field("GridValidity", JavadTextVisitor.CharacterType, "Grid Validity")),
        new(4, JavadVisitor.Field("GridSystem", JavadTextVisitor.TextType, "Grid System")),
        new(5, JavadVisitor.Field("Zone", Int32OptionalType.Default, "Zone")),
        new(
            6,
            JavadVisitor.Field("NorthingMeters", DoubleOptionalType.Default, "Northing Meters", "m")
        ),
        new(
            7,
            JavadVisitor.Field("EastingMeters", DoubleOptionalType.Default, "Easting Meters", "m")
        ),
        new(
            8,
            JavadVisitor.Field("AltitudeMeters", DoubleOptionalType.Default, "Altitude Meters", "m")
        ),
    ];
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
            VisitorFields[3].Field,
            VisitorFields[4].Field,
            VisitorFields[5].Field,
            VisitorFields[6].Field,
            VisitorFields[7].Field,
            VisitorFields[8].Field,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadPositionCovarianceTextMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(0, JavadVisitor.Field("UtcValidity", JavadTextVisitor.CharacterType, "Utc Validity")),
        new(1, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
        new(
            2,
            JavadVisitor.Field(
                "CovarianceValidity",
                JavadTextVisitor.CharacterType,
                "Covariance Validity"
            )
        ),
        new(
            3,
            JavadVisitor.Field(
                "ComputationMode",
                JavadTextVisitor.CharacterType,
                "Computation Mode"
            )
        ),
        new(4, JavadVisitor.Field("Covariance11", DoubleOptionalType.Default, "Covariance11")),
        new(5, JavadVisitor.Field("Covariance22", DoubleOptionalType.Default, "Covariance22")),
        new(6, JavadVisitor.Field("Covariance33", DoubleOptionalType.Default, "Covariance33")),
        new(7, JavadVisitor.Field("Covariance44", DoubleOptionalType.Default, "Covariance44")),
        new(8, JavadVisitor.Field("Covariance12", DoubleOptionalType.Default, "Covariance12")),
        new(9, JavadVisitor.Field("Covariance13", DoubleOptionalType.Default, "Covariance13")),
        new(10, JavadVisitor.Field("Covariance14", DoubleOptionalType.Default, "Covariance14")),
        new(11, JavadVisitor.Field("Covariance23", DoubleOptionalType.Default, "Covariance23")),
        new(12, JavadVisitor.Field("Covariance24", DoubleOptionalType.Default, "Covariance24")),
        new(13, JavadVisitor.Field("Covariance34", DoubleOptionalType.Default, "Covariance34")),
    ];
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
            VisitorFields[3].Field,
            VisitorFields[4].Field,
            VisitorFields[5].Field,
            VisitorFields[6].Field,
            VisitorFields[7].Field,
            VisitorFields[8].Field,
            VisitorFields[9].Field,
            VisitorFields[10].Field,
            VisitorFields[11].Field,
            VisitorFields[12].Field,
            VisitorFields[13].Field,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadClockOffsetsMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(0, JavadVisitor.Field("UtcValidity", JavadTextVisitor.CharacterType, "Utc Validity")),
        new(1, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
        new(2, JavadVisitor.Field("PositionStatus", Int32OptionalType.Default, "Position Status")),
        new(
            3,
            JavadVisitor.Field(
                "ComputationMode",
                JavadTextVisitor.CharacterType,
                "Computation Mode"
            )
        ),
        new(
            4,
            JavadVisitor.Field(
                "SatellitesPerSystem",
                new ListType(Int32OptionalType.Default, 0, 4095),
                "Satellites Per System"
            )
        ),
        new(
            5,
            JavadVisitor.Field(
                "Clocks",
                new OptionalStructType(
                    new StructType([
                        JavadVisitor.Field(
                            "GpsClock",
                            JavadClockOffset.StructType,
                            "GPS clock offset"
                        ),
                        JavadVisitor.Field(
                            "GlonassClock",
                            JavadClockOffset.StructType,
                            "GLONASS clock offset"
                        ),
                    ])
                ),
                "Clocks"
            )
        ),
        new(6, JavadVisitor.Field("Tdop", DoubleOptionalType.Default, "Tdop")),
        new(
            7,
            JavadVisitor.Field(
                "ClockOffsetRmsMeters",
                DoubleOptionalType.Default,
                "Clock Offset Rms Meters",
                "m"
            )
        ),
        new(
            8,
            JavadVisitor.Field(
                "ClockDerivativeRmsMetersPerSecond",
                DoubleOptionalType.Default,
                "Clock Derivative Rms Meters Per Second",
                "m/s"
            )
        ),
        new(
            9,
            JavadVisitor.Field(
                "ImprovedTimingMode",
                Int32OptionalType.Default,
                "Improved Timing Mode"
            )
        ),
    ];
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
            VisitorFields[3].Field,
            VisitorFields[4].Field,
            VisitorFields[5].Field,
            VisitorFields[6].Field,
            VisitorFields[7].Field,
            VisitorFields[8].Field,
            VisitorFields[9].Field,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadNavigationPositionMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(0, JavadVisitor.Field("UtcValidity", JavadTextVisitor.CharacterType, "Utc Validity")),
        new(1, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
        new(2, JavadVisitor.Field("PositionStatus", Int32OptionalType.Default, "Position Status")),
        new(
            3,
            JavadVisitor.Field("ComputationModes", JavadTextVisitor.TextType, "Computation Modes")
        ),
        new(
            4,
            JavadVisitor.Field(
                "SatellitesPerSystem",
                new ListType(Int32OptionalType.Default, 0, 4095),
                "Satellites Per System"
            )
        ),
        new(5, JavadVisitor.Field("GeodeticDatum", JavadTextVisitor.TextType, "Geodetic Datum")),
        new(6, JavadVisitor.Field("Latitude", JavadTextVisitor.TextType, "Latitude")),
        new(7, JavadVisitor.Field("Longitude", JavadTextVisitor.TextType, "Longitude")),
        new(
            8,
            JavadVisitor.Field("AltitudeMeters", DoubleOptionalType.Default, "Altitude Meters", "m")
        ),
        new(
            9,
            JavadVisitor.Field(
                "GeoidalSeparationValidity",
                JavadTextVisitor.CharacterType,
                "Geoidal Separation Validity"
            )
        ),
        new(
            10,
            JavadVisitor.Field(
                "GeoidalSeparationMeters",
                DoubleOptionalType.Default,
                "Geoidal Separation Meters",
                "m"
            )
        ),
        new(11, JavadVisitor.Field("Hdop", DoubleOptionalType.Default, "Hdop")),
        new(12, JavadVisitor.Field("Vdop", DoubleOptionalType.Default, "Vdop")),
        new(
            13,
            JavadVisitor.Field(
                "HorizontalPositionRmsMeters",
                DoubleOptionalType.Default,
                "Horizontal Position Rms Meters",
                "m"
            )
        ),
        new(
            14,
            JavadVisitor.Field(
                "VerticalPositionRmsMeters",
                DoubleOptionalType.Default,
                "Vertical Position Rms Meters",
                "m"
            )
        ),
        new(
            15,
            JavadVisitor.Field(
                "HorizontalVelocityKilometersPerHour",
                DoubleOptionalType.Default,
                "Horizontal Velocity Kilometers Per Hour",
                "km/h"
            )
        ),
        new(
            16,
            JavadVisitor.Field(
                "VerticalVelocityKilometersPerHour",
                DoubleOptionalType.Default,
                "Vertical Velocity Kilometers Per Hour",
                "km/h"
            )
        ),
        new(
            17,
            JavadVisitor.Field(
                "TrueHeadingDegrees",
                DoubleOptionalType.Default,
                "True Heading Degrees",
                "deg"
            )
        ),
        new(
            18,
            JavadVisitor.Field(
                "MagneticHeadingValidity",
                JavadTextVisitor.CharacterType,
                "Magnetic Heading Validity"
            )
        ),
        new(
            19,
            JavadVisitor.Field(
                "MagneticHeadingDegrees",
                DoubleOptionalType.Default,
                "Magnetic Heading Degrees",
                "deg"
            )
        ),
        new(
            20,
            JavadVisitor.Field(
                "HorizontalVelocityRmsMetersPerSecond",
                DoubleOptionalType.Default,
                "Horizontal Velocity Rms Meters Per Second",
                "m/s"
            )
        ),
        new(
            21,
            JavadVisitor.Field(
                "VerticalVelocityRmsMetersPerSecond",
                DoubleOptionalType.Default,
                "Vertical Velocity Rms Meters Per Second",
                "m/s"
            )
        ),
        new(
            22,
            JavadVisitor.Field(
                "DataLinkQualityPercent",
                DoubleOptionalType.Default,
                "Data Link Quality Percent",
                "%"
            )
        ),
        new(
            23,
            JavadVisitor.Field(
                "CorrectionsAgeSeconds",
                Int32OptionalType.Default,
                "Corrections Age Seconds",
                "s"
            )
        ),
    ];
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
            VisitorFields[3].Field,
            VisitorFields[4].Field,
            VisitorFields[5].Field,
            VisitorFields[6].Field,
            VisitorFields[7].Field,
            VisitorFields[8].Field,
            VisitorFields[9].Field,
            VisitorFields[10].Field,
            VisitorFields[11].Field,
            VisitorFields[12].Field,
            VisitorFields[13].Field,
            VisitorFields[14].Field,
            VisitorFields[15].Field,
            VisitorFields[16].Field,
            VisitorFields[17].Field,
            VisitorFields[18].Field,
            VisitorFields[19].Field,
            VisitorFields[20].Field,
            VisitorFields[21].Field,
            VisitorFields[22].Field,
            VisitorFields[23].Field,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadImuAttitudeMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(
            0,
            JavadVisitor.Field(
                "SolutionValidity",
                JavadTextVisitor.CharacterType,
                "Solution Validity"
            )
        ),
        new(1, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
        new(
            2,
            JavadVisitor.Field("RollDegrees", DoubleOptionalType.Default, "Roll Degrees", "deg")
        ),
        new(
            3,
            JavadVisitor.Field("PitchDegrees", DoubleOptionalType.Default, "Pitch Degrees", "deg")
        ),
        new(
            4,
            JavadVisitor.Field(
                "HeadingDegrees",
                DoubleOptionalType.Default,
                "Heading Degrees",
                "deg"
            )
        ),
    ];
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
            VisitorFields[3].Field,
            VisitorFields[4].Field,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadDataLinkStatusMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields = [];
    private static readonly Field CountField = JavadVisitor.Field(
        "LinkCount",
        Int32Type.Default,
        "Link Count"
    );
    private static readonly Field GroupsField = JavadVisitor.Field(
        "Links",
        new ListType(JavadDataLinkStatus.StructType, 0, 4095),
        "Links"
    );
    public static StructType StructType { get; } =
        new([ExtensionDataField, CountField, GroupsField]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        JavadTextVisitor.VisitRepeated(visitor, CountField, GroupsField, 0, 1, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadQuasiGeographicPositionMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(0, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
        new(1, JavadVisitor.Field("Indicator", JavadTextVisitor.CharacterType, "Indicator")),
        new(
            2,
            JavadVisitor.Field(
                "LatitudeDegreesMinutes",
                DoubleOptionalType.Default,
                "Latitude in degrees and minutes",
                "ddmm.mmm"
            )
        ),
        new(
            3,
            JavadVisitor.Field(
                "LatitudeHemisphere",
                JavadTextVisitor.CharacterType,
                "Latitude Hemisphere"
            )
        ),
        new(
            4,
            JavadVisitor.Field(
                "LongitudeDegreesMinutes",
                DoubleOptionalType.Default,
                "Longitude in degrees and minutes",
                "dddmm.mmm"
            )
        ),
        new(
            5,
            JavadVisitor.Field(
                "LongitudeHemisphere",
                JavadTextVisitor.CharacterType,
                "Longitude Hemisphere"
            )
        ),
        new(
            6,
            JavadVisitor.Field("GroundSpeedKnots", DoubleOptionalType.Default, "Ground speed", "kn")
        ),
        new(
            7,
            JavadVisitor.Field(
                "TrueCourseDegrees",
                DoubleOptionalType.Default,
                "True Course Degrees",
                "deg"
            )
        ),
        new(8, JavadVisitor.Field("Day", Int32OptionalType.Default, "Day")),
        new(9, JavadVisitor.Field("Month", Int32OptionalType.Default, "Month")),
        new(10, JavadVisitor.Field("YearModulo100", Int32OptionalType.Default, "Year Modulo100")),
        new(
            11,
            JavadVisitor.Field(
                "HeadingDegrees",
                DoubleOptionalType.Default,
                "Heading Degrees",
                "deg"
            )
        ),
        new(
            12,
            JavadVisitor.Field(
                "QuasiCourseDegrees",
                DoubleOptionalType.Default,
                "Quasi Course Degrees",
                "deg"
            )
        ),
        new(
            13,
            JavadVisitor.Field(
                "QuasiHeadingDegrees",
                DoubleOptionalType.Default,
                "Quasi Heading Degrees",
                "deg"
            )
        ),
        new(
            14,
            JavadVisitor.Field(
                "AttitudeSolutionType",
                JavadTextVisitor.CharacterType,
                "Attitude Solution Type"
            )
        ),
    ];
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
            VisitorFields[3].Field,
            VisitorFields[4].Field,
            VisitorFields[5].Field,
            VisitorFields[6].Field,
            VisitorFields[7].Field,
            VisitorFields[8].Field,
            VisitorFields[9].Field,
            VisitorFields[10].Field,
            VisitorFields[11].Field,
            VisitorFields[12].Field,
            VisitorFields[13].Field,
            VisitorFields[14].Field,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadReferenceStationParametersMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields = [];
    private static readonly Field CountField = JavadVisitor.Field(
        "StationCount",
        Int32Type.Default,
        "Station Count"
    );
    private static readonly Field GroupsField = JavadVisitor.Field(
        "Stations",
        new ListType(JavadReferenceStationParameters.StructType, 0, 4095),
        "Stations"
    );
    public static StructType StructType { get; } =
        new([ExtensionDataField, CountField, GroupsField]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        JavadTextVisitor.VisitRepeated(visitor, CountField, GroupsField, 0, 1, ref fields);
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadRaimMessage
{
    private static readonly ImmutableArray<JavadTextVisitField> VisitorFields =
    [
        new(0, JavadVisitor.Field("UtcValidity", JavadTextVisitor.CharacterType, "Utc Validity")),
        new(1, JavadVisitor.Field("UtcTime", TimeSpanOptionalType.Default, "Utc Time")),
        new(2, JavadVisitor.Field("RaimStatus", Int32OptionalType.Default, "Raim Status")),
    ];
    private static readonly Field CountField = JavadVisitor.Field(
        "ExcludedMeasurementCount",
        Int32Type.Default,
        "Excluded Measurement Count"
    );
    private static readonly Field GroupsField = JavadVisitor.Field(
        "ExcludedSatellites",
        new ListType(JavadTextVisitor.TextType, 0, 4095),
        "Excluded Satellites"
    );
    public static StructType StructType { get; } =
        new([
            ExtensionDataField,
            VisitorFields[0].Field,
            VisitorFields[1].Field,
            VisitorFields[2].Field,
            CountField,
            GroupsField,
        ]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitFields(visitor, VisitorFields, ref fields);
        if (JavadTextFields.Int(fields, 2) == 2)
        {
            if (fields.Length < 4)
                JavadTextVisitor.Set(ref fields, 3, JavadValue.Integer(0));
            JavadTextVisitor.VisitRepeated(visitor, CountField, GroupsField, 3, 4, ref fields);
        }
        if (fields != Fields)
            Fields = fields;
    }
}
