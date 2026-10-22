using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[RS] ReferenceStationStatus; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadReferenceStationStatusMessage()
    : JavadTextMessage("REFST", "N,000000.00,{N,N,N,N}")
{
    public override string Id => "RS";
    public override string Name => "REFST";
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
    public JavadReferenceLocationStatus ReferenceLocation => new(Field(2));

    protected override void ValidateFields()
    {
        Validate("ctl");
        JavadTextRecordValidation.Validate(Field(2), "cccc");
    }
}
