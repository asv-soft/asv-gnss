using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[TR] TimeResiduals; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadTimeResidualsMessage()
    : JavadTextMessage("TIMERES", "N,000000.00,0")
{
    public override string Id => "TR";
    public override string Name => "TIMERES";
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
    public int SatelliteCount => Count(2);
    public ImmutableArray<JavadTimeResidual> Residuals
    {
        get => Repeated(2, 3, 3).Select(x => new JavadTimeResidual(x)).ToImmutableArray();
        set => SetRepeated(2, 3, value.Select(x => x.Value).ToImmutableArray());
    }

    protected override void ValidateFields()
    {
        Validate("cti");
        foreach (var value in Repeated(2, 3, 3))
            JavadTextRecordValidation.Validate(value, "sfi");
    }
}
