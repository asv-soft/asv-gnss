using Asv.IO;

namespace Asv.Gnss;

public sealed partial record JavadDataLinkStatus
{
    public static StructType StructType { get; } =
        new([
            JavadVisitor.Field("LinkId", JavadTextVisitor.CharacterType, "Link Id"),
            JavadVisitor.Field("DecoderId", JavadTextVisitor.CharacterType, "Decoder Id"),
            JavadVisitor.Field(
                "ReferenceStationId",
                JavadTextVisitor.TextType,
                "Reference Station Id"
            ),
            JavadVisitor.Field("AgeSeconds", Int32OptionalType.Default, "Age Seconds", "s"),
            JavadVisitor.Field("ReceivedMessages", Int32OptionalType.Default, "Received Messages"),
            JavadVisitor.Field("CorruptMessages", Int32OptionalType.Default, "Corrupt Messages"),
            JavadVisitor.Field(
                "QualityPercent",
                DoubleOptionalType.Default,
                "Quality Percent",
                "%"
            ),
            JavadVisitor.Field(
                "LatencySeconds",
                DoubleOptionalType.Default,
                "Latency Seconds",
                "s"
            ),
            JavadVisitor.Field(
                "CorrectionPeriodSeconds",
                DoubleOptionalType.Default,
                "Correction Period Seconds",
                "s"
            ),
        ]);

    public void Accept(IVisitor visitor) =>
        JavadTextVisitor.VisitRecord(visitor, StructType, ref _value);
}

public sealed partial record JavadReferenceLocationStatus
{
    public static StructType StructType { get; } =
        new([
            JavadVisitor.Field("GpsApc", JavadTextVisitor.CharacterType, "Gps Apc"),
            JavadVisitor.Field("GlonassApc", JavadTextVisitor.CharacterType, "Glonass Apc"),
            JavadVisitor.Field("GpsArp", JavadTextVisitor.CharacterType, "Gps Arp"),
            JavadVisitor.Field("GlonassArp", JavadTextVisitor.CharacterType, "Glonass Arp"),
        ]);

    public void Accept(IVisitor visitor) =>
        JavadTextVisitor.VisitRecord(visitor, StructType, ref _value);
}

public sealed partial record JavadCorrectionText
{
    public static StructType StructType { get; } =
        new([
            JavadVisitor.Field("MonthDay", JavadTextVisitor.TextType, "Month Day"),
            JavadVisitor.Field(
                "UtcHoursMinutesSeconds",
                JavadTextVisitor.TextType,
                "Utc Hours Minutes Seconds"
            ),
            JavadVisitor.Field("Text", JavadTextVisitor.TextType, "Text"),
            JavadVisitor.Field("DecoderId", JavadTextVisitor.CharacterType, "Decoder Id"),
            JavadVisitor.Field("DataLinkId", JavadTextVisitor.CharacterType, "Data Link Id"),
            JavadVisitor.Field(
                "ReferenceStationId",
                JavadTextVisitor.TextType,
                "Reference Station Id"
            ),
        ]);

    public void Accept(IVisitor visitor) =>
        JavadTextVisitor.VisitRecord(visitor, StructType, ref _value);
}

public sealed partial record JavadTimeResidual
{
    public static StructType StructType { get; } =
        new([
            JavadVisitor.Field("SatelliteId", JavadTextVisitor.TextType, "Satellite Id"),
            JavadVisitor.Field(
                "ResidualNanoseconds",
                DoubleOptionalType.Default,
                "Residual Nanoseconds",
                "ns"
            ),
            JavadVisitor.Field("EphemerisIssue", Int32OptionalType.Default, "Ephemeris Issue"),
        ]);

    public void Accept(IVisitor visitor) =>
        JavadTextVisitor.VisitRecord(visitor, StructType, ref _value);
}

public sealed partial record JavadClockOffset
{
    public static StructType StructType { get; } =
        new([
            JavadVisitor.Field("Validity", JavadTextVisitor.CharacterType, "Validity"),
            JavadVisitor.Field("OffsetMeters", DoubleOptionalType.Default, "Offset Meters", "m"),
            JavadVisitor.Field(
                "DerivativeMetersPerSecond",
                DoubleOptionalType.Default,
                "Derivative Meters Per Second",
                "m/s"
            ),
        ]);

    public void Accept(IVisitor visitor) =>
        JavadTextVisitor.VisitRecord(visitor, StructType, ref _value);
}

public sealed partial record JavadReferenceStationParameters
{
    public static StructType StructType { get; } =
        new([
            JavadVisitor.Field("GeodeticDatum", JavadTextVisitor.TextType, "Geodetic Datum"),
            JavadVisitor.Field("Latitude", JavadTextVisitor.TextType, "Latitude"),
            JavadVisitor.Field("Longitude", JavadTextVisitor.TextType, "Longitude"),
            JavadVisitor.Field(
                "AltitudeMeters",
                DoubleOptionalType.Default,
                "Altitude Meters",
                "m"
            ),
            JavadVisitor.Field(
                "AntennaHeightValidity",
                JavadTextVisitor.CharacterType,
                "Antenna Height Validity"
            ),
            JavadVisitor.Field(
                "AntennaHeightMeters",
                DoubleOptionalType.Default,
                "Antenna Height Meters",
                "m"
            ),
            JavadVisitor.Field(
                "AntennaOffsetsValidity",
                JavadTextVisitor.CharacterType,
                "Antenna Offsets Validity"
            ),
            JavadVisitor.Field(
                "AntennaNorthOffsetMeters",
                DoubleOptionalType.Default,
                "Antenna North Offset Meters",
                "m"
            ),
            JavadVisitor.Field(
                "AntennaEastOffsetMeters",
                DoubleOptionalType.Default,
                "Antenna East Offset Meters",
                "m"
            ),
            JavadVisitor.Field("DecoderId", JavadTextVisitor.CharacterType, "Decoder Id"),
            JavadVisitor.Field("DataLinkId", JavadTextVisitor.CharacterType, "Data Link Id"),
            JavadVisitor.Field(
                "ReferenceStationId",
                JavadTextVisitor.TextType,
                "Reference Station Id"
            ),
            JavadVisitor.Field("AntennaId", JavadTextVisitor.TextType, "Antenna Id"),
        ]);

    public void Accept(IVisitor visitor) =>
        JavadTextVisitor.VisitRecord(visitor, StructType, ref _value);
}

public sealed partial record JavadSatelliteTextStatus
{
    public static StructType StructType { get; } =
        new([
            JavadVisitor.Field("SatelliteNumber", Int32OptionalType.Default, "Satellite Number"),
            JavadVisitor.Field(
                "ElevationDegrees",
                Int32OptionalType.Default,
                "Elevation Degrees",
                "deg"
            ),
            JavadVisitor.Field(
                "AzimuthDegrees",
                Int32OptionalType.Default,
                "Azimuth Degrees",
                "deg"
            ),
            JavadVisitor.Field(
                "SignalToNoiseDbHz",
                new ListType(DoubleOptionalType.Default, 0, 4095),
                "Signal To Noise Db Hz",
                "dB-Hz"
            ),
            JavadVisitor.Field("NavigationStatus", Int32OptionalType.Default, "Navigation Status"),
        ]);
    public static StructType FrequencyChannelStructType { get; } =
        new([
            JavadVisitor.Field("SatelliteNumber", Int32OptionalType.Default, "Satellite Number"),
            JavadVisitor.Field("FrequencyChannel", Int32OptionalType.Default, "Frequency Channel"),
            JavadVisitor.Field(
                "ElevationDegrees",
                Int32OptionalType.Default,
                "Elevation Degrees",
                "deg"
            ),
            JavadVisitor.Field(
                "AzimuthDegrees",
                Int32OptionalType.Default,
                "Azimuth Degrees",
                "deg"
            ),
            JavadVisitor.Field(
                "SignalToNoiseDbHz",
                new ListType(DoubleOptionalType.Default, 0, 4095),
                "Signal To Noise Db Hz",
                "dB-Hz"
            ),
            JavadVisitor.Field("NavigationStatus", Int32OptionalType.Default, "Navigation Status"),
        ]);

    public void Accept(IVisitor visitor) =>
        JavadTextVisitor.VisitRecord(
            visitor,
            HasFrequencyChannel ? FrequencyChannelStructType : StructType,
            ref _value
        );
}
