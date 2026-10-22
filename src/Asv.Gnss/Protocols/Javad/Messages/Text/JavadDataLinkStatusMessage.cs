using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[DL] DataLinkStatus; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadDataLinkStatusMessage() : JavadTextMessage("DLINK", "0")
{
    public override string Id => "DL";
    public override string Name => "DLINK";
    public int LinkCount => Count(0);
    public ImmutableArray<JavadDataLinkStatus> Links
    {
        get => Repeated(0, 1, 7).Select(x => new JavadDataLinkStatus(x)).ToImmutableArray();
        set => SetRepeated(0, 1, value.Select(x => x.Value).ToImmutableArray());
    }

    protected override void ValidateFields()
    {
        Validate("i");
        foreach (var value in Repeated(0, 1, 7))
            JavadTextRecordValidation.Validate(value, "ccsiiifff", 7);
    }
}
