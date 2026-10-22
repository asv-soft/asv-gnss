using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[RP] ReferenceStationParameters; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadReferenceStationParametersMessage()
    : JavadTextMessage("REFPAR", "0")
{
    public override string Id => "RP";
    public override string Name => "REFPAR";
    public int StationCount => Count(0);
    public ImmutableArray<JavadReferenceStationParameters> Stations
    {
        get =>
            Repeated(0, 1, 13)
                .Select(x => new JavadReferenceStationParameters(x))
                .ToImmutableArray();
        set => SetRepeated(0, 1, value.Select(x => x.Value).ToImmutableArray());
    }

    protected override void ValidateFields()
    {
        Validate("i");
        foreach (var value in Repeated(0, 1, 13))
            JavadTextRecordValidation.Validate(value, "sssfcfcffccss");
    }
}
