using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[TX] CorrectionText; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadCorrectionTextMessage() : JavadTextMessage("TEXT", "0")
{
    public override string Id => "TX";
    public override string Name => "TEXT";
    public int MessageCount => Count(0);
    public ImmutableArray<JavadCorrectionText> Messages
    {
        get => Repeated(0, 1, 6).Select(x => new JavadCorrectionText(x)).ToImmutableArray();
        set => SetRepeated(0, 1, value.Select(x => x.Value).ToImmutableArray());
    }

    protected override void ValidateFields()
    {
        Validate("i");
        foreach (var value in Repeated(0, 1, 6))
            JavadTextRecordValidation.Validate(value, "sssccs");
    }
}
