using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[TM] ClockOffsets; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadClockOffsetsMessage() : JavadTextMessage("TIMING", "N,000000.00,3")
{
    public override string Id => "TM";
    public override string Name => "TIMING";
    public char? UtcValidity
    {
        get => Character(0);
        set => Set(0, value);
    }
    public TimeSpan? UtcTime
    {
        get => Time(1);
        set => SetTime(1, value);
    }
    public int? PositionStatus
    {
        get => Integer(2);
        set => Set(2, value);
    }
    public char? ComputationMode
    {
        get => Character(3);
        set => Set(3, value);
    }
    public double? Tdop
    {
        get => Number(6);
        set => Set(6, value);
    }
    public double? ClockOffsetRmsMeters
    {
        get => Number(7);
        set => Set(7, value);
    }
    public double? ClockDerivativeRmsMetersPerSecond
    {
        get => Number(8);
        set => Set(8, value);
    }
    public int? ImprovedTimingMode
    {
        get => Integer(9);
        set => Set(9, value);
    }
    public ImmutableArray<int?> SatellitesPerSystem =>
        Field(4).IsOmitted
            ? []
            : JavadTextFields.List(Field(4)).Select(JavadTextFields.Integer).ToImmutableArray();
    public JavadClockOffset? GpsClock =>
        Field(5).IsOmitted ? null : new(JavadTextFields.List(Field(5), 2)[0]);
    public JavadClockOffset? GlonassClock =>
        Field(5).IsOmitted ? null : new(JavadTextFields.List(Field(5), 2)[1]);

    protected override void ValidateFields()
    {
        Validate("cti");
        if (PositionStatus != 0)
            return;
        Validate("cticllfffi");
        var systems = JavadTextFields.List(Field(5), 2);
        JavadTextRecordValidation.Validate(systems[0], "cff");
        JavadTextRecordValidation.Validate(systems[1], "cff");
    }
}
