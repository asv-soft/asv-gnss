using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021VerticalRate
{
    private static readonly Field VisitorRangeExceededField = new Field.Builder().Name(nameof(RangeExceeded)).Title(nameof(RangeExceeded)).Description("RangeExceeded").DataType(BoolType.Default).Build();
    private static readonly Field VisitorRateFtPerMinuteField = new Field.Builder().Name(nameof(RateFtPerMinute)).Title(nameof(RateFtPerMinute)).Description("RateFtPerMinute").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorRangeExceededField, VisitorRateFtPerMinuteField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueRangeExceeded = RangeExceeded;
        BoolType.Accept(visitor, VisitorRangeExceededField, ref valueRangeExceeded);
        if (!EqualityComparer<bool>.Default.Equals(valueRangeExceeded, RangeExceeded)) RangeExceeded = valueRangeExceeded;
        var valueRateFtPerMinute = RateFtPerMinute;
        DoubleType.Accept(visitor, VisitorRateFtPerMinuteField, ref valueRateFtPerMinute);
        if (!EqualityComparer<double>.Default.Equals(valueRateFtPerMinute, RateFtPerMinute)) RateFtPerMinute = valueRateFtPerMinute;
    }
}
