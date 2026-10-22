using Asv.IO;

namespace Asv.Gnss;

public partial class UbxNavSvin
{
    private static readonly Field Reserved32Field = UbxVisitor.Field(nameof(Reserved32), UInt8Type.Default, "Reserved payload byte; no application meaning is defined by this protocol version.");
    private static readonly Field Reserved31Field = UbxVisitor.Field(nameof(Reserved31), UInt8Type.Default, "Reserved payload byte; no application meaning is defined by this protocol version.");
    private static readonly Field Reserved2Field = UbxVisitor.Field(nameof(Reserved2), UInt8Type.Default, "Reserved payload byte; no application meaning is defined by this protocol version.");
    private static readonly Field Reserved13Field = UbxVisitor.Field(nameof(Reserved13), UInt8Type.Default, "Reserved payload byte; no application meaning is defined by this protocol version.");
    private static readonly Field Reserved12Field = UbxVisitor.Field(nameof(Reserved12), UInt8Type.Default, "Reserved payload byte; no application meaning is defined by this protocol version.");
    private static readonly Field Reserved11Field = UbxVisitor.Field(nameof(Reserved11), UInt8Type.Default, "Reserved payload byte; no application meaning is defined by this protocol version.");
    private static readonly Field VersionField = UbxVisitor.Field(nameof(Version), UInt8Type.Default, "Payload format version; the documented NAV-SVIN format uses version 0.");
    private static readonly Field ITowField = UbxVisitor.Field(nameof(ITow), UInt32Type.Default, "Navigation epoch as GPS time of week, in milliseconds.", "ms");
    private static readonly Field DurationField = UbxVisitor.Field(nameof(Duration), UInt32Type.Default, "Elapsed survey-in observation time, in seconds.", "s");
    private static readonly Field MeanXField = UbxVisitor.Field(nameof(MeanX), Int32Type.Default, "Coarse ECEF X coordinate of the mean position, in centimeters; add the corresponding high-precision component.", "cm");
    private static readonly Field MeanYField = UbxVisitor.Field(nameof(MeanY), Int32Type.Default, "Coarse ECEF Y coordinate of the mean position, in centimeters; add the corresponding high-precision component.", "cm");
    private static readonly Field MeanZField = UbxVisitor.Field(nameof(MeanZ), Int32Type.Default, "Coarse ECEF Z coordinate of the mean position, in centimeters; add the corresponding high-precision component.", "cm");
    private static readonly Field EcefField = UbxVisitor.Field(nameof(Ecef), UbxVisitor.EcefType, "Mean Earth-centered, Earth-fixed position in meters, combining the centimeter coordinates and their high-precision components.");
    private static readonly Field MeanXhpField = UbxVisitor.Field(nameof(MeanXhp), Int8Type.Default, "High-precision correction to the ECEF X coordinate, in units of 0.1 mm; documented range is -99 to 99.", "0.1 mm");
    private static readonly Field MeanYhpField = UbxVisitor.Field(nameof(MeanYhp), Int8Type.Default, "High-precision correction to the ECEF Y coordinate, in units of 0.1 mm; documented range is -99 to 99.", "0.1 mm");
    private static readonly Field MeanZhpField = UbxVisitor.Field(nameof(MeanZhp), Int8Type.Default, "High-precision correction to the ECEF Z coordinate, in units of 0.1 mm; documented range is -99 to 99.", "0.1 mm");
    private static readonly Field AccuracyField = UbxVisitor.Field(nameof(Accuracy), DoubleType.Default, "Estimated accuracy of the mean position, in meters; the wire value has a scale of 0.1 mm.", "m");
    private static readonly Field ObservationsField = UbxVisitor.Field(nameof(Observations), UInt32Type.Default, "Number of position observations included in the survey-in mean.");
    private static readonly Field ValidField = UbxVisitor.Field(nameof(Valid), BoolType.Default, "Whether the survey-in position meets the configured requirements and is valid.");
    private static readonly Field ActiveField = UbxVisitor.Field(nameof(Active), BoolType.Default, "Whether survey-in is currently collecting observations.");
    private static readonly Field LocationField = UbxVisitor.Field(nameof(Location), UbxVisitor.LocationType, "Mean position converted from ECEF to WGS84 latitude and longitude in degrees and ellipsoidal height in meters.");

    /// <summary>Describes the public navigation payload fields exposed to visitors.</summary>
    public static StructType StructType { get; } = new([
        Reserved32Field,
        Reserved31Field,
        Reserved2Field,
        Reserved13Field,
        Reserved12Field,
        Reserved11Field,
        VersionField,
        ITowField,
        DurationField,
        MeanXField,
        MeanYField,
        MeanZField,
        EcefField,
        MeanXhpField,
        MeanYhpField,
        MeanZhpField,
        AccuracyField,
        ObservationsField,
        ValidField,
        ActiveField,
        LocationField
    ]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        UInt8Type.Accept(visitor, Reserved32Field, Reserved32Field.DataType, ref _reserved32);
        UInt8Type.Accept(visitor, Reserved31Field, Reserved31Field.DataType, ref _reserved31);
        UInt8Type.Accept(visitor, Reserved2Field, Reserved2Field.DataType, ref _reserved2);
        UInt8Type.Accept(visitor, Reserved13Field, Reserved13Field.DataType, ref _reserved13);
        UInt8Type.Accept(visitor, Reserved12Field, Reserved12Field.DataType, ref _reserved12);
        UInt8Type.Accept(visitor, Reserved11Field, Reserved11Field.DataType, ref _reserved11);
        UInt8Type.Accept(visitor, VersionField, VersionField.DataType, ref _version);
        UInt32Type.Accept(visitor, ITowField, ITowField.DataType, ref _iTow);
        UInt32Type.Accept(visitor, DurationField, DurationField.DataType, ref _duration);
        Int32Type.Accept(visitor, MeanXField, MeanXField.DataType, ref _meanX);
        Int32Type.Accept(visitor, MeanYField, MeanYField.DataType, ref _meanY);
        Int32Type.Accept(visitor, MeanZField, MeanZField.DataType, ref _meanZ);
        UbxVisitor.VisitEcef(visitor, EcefField, ref _ecef);
        Int8Type.Accept(visitor, MeanXhpField, MeanXhpField.DataType, ref _meanXhp);
        Int8Type.Accept(visitor, MeanYhpField, MeanYhpField.DataType, ref _meanYhp);
        Int8Type.Accept(visitor, MeanZhpField, MeanZhpField.DataType, ref _meanZhp);
        DoubleType.Accept(visitor, AccuracyField, AccuracyField.DataType, ref _accuracy);
        UInt32Type.Accept(visitor, ObservationsField, ObservationsField.DataType, ref _observations);
        BoolType.Accept(visitor, ValidField, ValidField.DataType, ref _valid);
        BoolType.Accept(visitor, ActiveField, ActiveField.DataType, ref _active);
        UbxVisitor.VisitLocation(visitor, LocationField, ref _location);
    }
}
