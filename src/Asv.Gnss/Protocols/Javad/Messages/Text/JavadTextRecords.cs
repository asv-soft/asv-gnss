using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

internal static class JavadTextRecordValidation
{
    public static void Validate(JavadValue value, string types, int minimum = -1)
    {
        var fields = JavadTextFields.List(value, minimum < 0 ? types.Length : minimum);
        for (var i = 0; i < Math.Min(types.Length, fields.Length); i++)
        {
            switch (types[i])
            {
                case 'i':
                    _ = JavadTextFields.Integer(fields[i]);
                    break;
                case 'f':
                    _ = JavadTextFields.Number(fields[i]);
                    break;
                case 'c':
                    _ = JavadTextFields.Character(fields[i]);
                    break;
                case 's':
                    _ = JavadTextFields.Scalar(fields[i]);
                    break;
                case 'l':
                    _ = JavadTextFields.List(fields[i]);
                    break;
            }
        }
    }
}

/// <summary>A satellite status group; Value retains any future fields in the group.</summary>
public sealed partial record JavadSatelliteTextStatus(
    JavadValue Value,
    bool HasFrequencyChannel = false
) : Asv.IO.IVisitable
{
    private JavadValue _value = Value;
    public JavadValue Value
    {
        get => _value;
        init => _value = value;
    }
    private ImmutableArray<JavadValue> Fields =>
        JavadTextFields.List(Value, HasFrequencyChannel ? 6 : 5);
    private int Offset => HasFrequencyChannel ? 1 : 0;
    public int? SatelliteNumber => JavadTextFields.Int(Fields, 0);
    public int? FrequencyChannel => HasFrequencyChannel ? JavadTextFields.Int(Fields, 1) : null;
    public int? ElevationDegrees => JavadTextFields.Int(Fields, 1 + Offset);
    public int? AzimuthDegrees => JavadTextFields.Int(Fields, 2 + Offset);
    public ImmutableArray<double?> SignalToNoiseDbHz =>
        JavadTextFields.List(Fields[3 + Offset]).Select(JavadTextFields.Number).ToImmutableArray();
    public int? NavigationStatus => JavadTextFields.Int(Fields, 4 + Offset);
    public ImmutableArray<JavadValue> ExtraFields => Fields[(5 + Offset)..];
}

/// <summary>A typed native GREIS text group; Value preserves omitted and future fields.</summary>
public sealed partial record JavadDataLinkStatus(JavadValue Value) : Asv.IO.IVisitable
{
    private JavadValue _value = Value;
    public JavadValue Value
    {
        get => _value;
        init => _value = value;
    }
    private ImmutableArray<JavadValue> Fields => JavadTextFields.List(Value, 7);
    public char? LinkId => JavadTextFields.Char(Fields, 0);
    public char? DecoderId => JavadTextFields.Char(Fields, 1);
    public string? ReferenceStationId => JavadTextFields.Text(Fields, 2);
    public int? AgeSeconds => JavadTextFields.Int(Fields, 3);
    public int? ReceivedMessages => JavadTextFields.Int(Fields, 4);
    public int? CorruptMessages => JavadTextFields.Int(Fields, 5);
    public double? QualityPercent => JavadTextFields.Double(Fields, 6);
    public double? LatencySeconds => JavadTextFields.Double(Fields, 7);
    public double? CorrectionPeriodSeconds => JavadTextFields.Double(Fields, 8);
}

/// <summary>A typed native GREIS text group; Value preserves omitted and future fields.</summary>
public sealed partial record JavadReferenceLocationStatus(JavadValue Value) : Asv.IO.IVisitable
{
    private JavadValue _value = Value;
    public JavadValue Value
    {
        get => _value;
        init => _value = value;
    }
    private ImmutableArray<JavadValue> Fields => JavadTextFields.List(Value, 4);
    public char? GpsApc => JavadTextFields.Char(Fields, 0);
    public char? GlonassApc => JavadTextFields.Char(Fields, 1);
    public char? GpsArp => JavadTextFields.Char(Fields, 2);
    public char? GlonassArp => JavadTextFields.Char(Fields, 3);
}

/// <summary>A typed native GREIS text group; Value preserves omitted and future fields.</summary>
public sealed partial record JavadCorrectionText(JavadValue Value) : Asv.IO.IVisitable
{
    private JavadValue _value = Value;
    public JavadValue Value
    {
        get => _value;
        init => _value = value;
    }
    private ImmutableArray<JavadValue> Fields => JavadTextFields.List(Value, 6);
    public string? MonthDay => JavadTextFields.Text(Fields, 0);
    public string? UtcHoursMinutesSeconds => JavadTextFields.Text(Fields, 1);
    public string? Text => JavadTextFields.Text(Fields, 2);
    public char? DecoderId => JavadTextFields.Char(Fields, 3);
    public char? DataLinkId => JavadTextFields.Char(Fields, 4);
    public string? ReferenceStationId => JavadTextFields.Text(Fields, 5);
}

/// <summary>A typed native GREIS text group; Value preserves omitted and future fields.</summary>
public sealed partial record JavadTimeResidual(JavadValue Value) : Asv.IO.IVisitable
{
    private JavadValue _value = Value;
    public JavadValue Value
    {
        get => _value;
        init => _value = value;
    }
    private ImmutableArray<JavadValue> Fields => JavadTextFields.List(Value, 3);
    public string? SatelliteId => JavadTextFields.Text(Fields, 0);
    public double? ResidualNanoseconds => JavadTextFields.Double(Fields, 1);
    public int? EphemerisIssue => JavadTextFields.Int(Fields, 2);
}

/// <summary>A typed native GREIS text group; Value preserves omitted and future fields.</summary>
public sealed partial record JavadClockOffset(JavadValue Value) : Asv.IO.IVisitable
{
    private JavadValue _value = Value;
    public JavadValue Value
    {
        get => _value;
        init => _value = value;
    }
    private ImmutableArray<JavadValue> Fields => JavadTextFields.List(Value, 3);
    public char? Validity => JavadTextFields.Char(Fields, 0);
    public double? OffsetMeters => JavadTextFields.Double(Fields, 1);
    public double? DerivativeMetersPerSecond => JavadTextFields.Double(Fields, 2);
}

/// <summary>A typed native GREIS text group; Value preserves omitted and future fields.</summary>
public sealed partial record JavadReferenceStationParameters(JavadValue Value) : Asv.IO.IVisitable
{
    private JavadValue _value = Value;
    public JavadValue Value
    {
        get => _value;
        init => _value = value;
    }
    private ImmutableArray<JavadValue> Fields => JavadTextFields.List(Value, 13);
    public string? GeodeticDatum => JavadTextFields.Text(Fields, 0);
    public string? Latitude => JavadTextFields.Text(Fields, 1);
    public string? Longitude => JavadTextFields.Text(Fields, 2);
    public double? AltitudeMeters => JavadTextFields.Double(Fields, 3);
    public char? AntennaHeightValidity => JavadTextFields.Char(Fields, 4);
    public double? AntennaHeightMeters => JavadTextFields.Double(Fields, 5);
    public char? AntennaOffsetsValidity => JavadTextFields.Char(Fields, 6);
    public double? AntennaNorthOffsetMeters => JavadTextFields.Double(Fields, 7);
    public double? AntennaEastOffsetMeters => JavadTextFields.Double(Fields, 8);
    public char? DecoderId => JavadTextFields.Char(Fields, 9);
    public char? DataLinkId => JavadTextFields.Char(Fields, 10);
    public string? ReferenceStationId => JavadTextFields.Text(Fields, 11);
    public string? AntennaId => JavadTextFields.Text(Fields, 12);
}
