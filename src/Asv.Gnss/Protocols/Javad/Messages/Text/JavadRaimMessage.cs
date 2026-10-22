using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[RM] Raim; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadRaimMessage() : JavadTextMessage("RAIM", "N,000000.00,3")
{
    public override string Id => "RM";
    public override string Name => "RAIM";
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
    public int? RaimStatus
    {
        get => Integer(2);
        set => Set(2, value);
    }
    public int ExcludedMeasurementCount => RaimStatus == 2 ? Count(3) : 0;
    public ImmutableArray<string> ExcludedSatellites =>
        RaimStatus == 2
            ? Fields
                .Slice(4, Count(3))
                .Select(x => JavadTextFields.Scalar(x) ?? "")
                .ToImmutableArray()
            : [];

    protected override void ValidateFields()
    {
        Validate("cti");
        if (Integer(2) != 2)
            return;
        Require(4);
        if (Count(3) > Fields.Length - 4)
            throw new FormatException("The RAIM satellite count exceeds available fields.");
        foreach (var value in Fields.Slice(4, Count(3)))
            _ = JavadTextFields.Scalar(value);
    }
}
