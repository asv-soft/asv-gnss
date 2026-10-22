using Asv.IO;

namespace Asv.Gnss;

public abstract partial class JavadSatelliteStatusMessage
{
    private static readonly Field CountField = JavadVisitor.Field(
        "SatelliteCount",
        Int32Type.Default,
        "Satellite Count"
    );
    private static readonly Field SatellitesField = JavadVisitor.Field(
        "Satellites",
        new ListType(JavadSatelliteTextStatus.StructType, 0, 4095),
        "Satellites"
    );
    private static readonly Field ChannelSatellitesField = JavadVisitor.Field(
        "Satellites",
        new ListType(JavadSatelliteTextStatus.FrequencyChannelStructType, 0, 4095),
        "Satellites"
    );
    protected static StructType StandardVisitorType { get; } =
        new([ExtensionDataField, CountField, SatellitesField]);
    protected static StructType ChannelVisitorType { get; } =
        new([ExtensionDataField, CountField, ChannelSatellitesField]);

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        var fields = Fields;
        JavadTextVisitor.VisitRepeated(
            visitor,
            CountField,
            HasFrequencyChannel ? ChannelSatellitesField : SatellitesField,
            0,
            1,
            ref fields
        );
        if (fields != Fields)
            Fields = fields;
    }
}

public sealed partial class JavadGpsSatelliteStatusMessage
{
    public static StructType StructType { get; } = StandardVisitorType;
}

public sealed partial class JavadGalileoSatelliteStatusMessage
{
    public static StructType StructType { get; } = StandardVisitorType;
}

public sealed partial class JavadSbasSatelliteStatusMessage
{
    public static StructType StructType { get; } = StandardVisitorType;
}

public sealed partial class JavadGlonassSatelliteStatusMessage
{
    public static StructType StructType { get; } = ChannelVisitorType;
}

public sealed partial class JavadQzssSatelliteStatusMessage
{
    public static StructType StructType { get; } = StandardVisitorType;
}

public sealed partial class JavadBeiDouSatelliteStatusMessage
{
    public static StructType StructType { get; } = StandardVisitorType;
}

public sealed partial class JavadIrnssSatelliteStatusMessage
{
    public static StructType StructType { get; } = StandardVisitorType;
}

public sealed partial class JavadLBandSatelliteStatusMessage
{
    public static StructType StructType { get; } = ChannelVisitorType;
}
