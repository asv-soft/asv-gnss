using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn011Type151
{
    private static readonly Field VisitorRangeExceededField = new Field.Builder().Name(nameof(RangeExceeded)).Title(nameof(RangeExceeded)).Description("RangeExceeded").DataType(BoolType.Default).Build();
    private static readonly Field VisitorTrueAirSpeedField = new Field.Builder().Name(nameof(TrueAirSpeed)).Title(nameof(TrueAirSpeed)).Description("TrueAirSpeed").DataType(UInt16Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorRangeExceededField, VisitorTrueAirSpeedField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueRangeExceeded = RangeExceeded;
        BoolType.Accept(visitor, VisitorRangeExceededField, ref valueRangeExceeded);
        if (!EqualityComparer<bool>.Default.Equals(valueRangeExceeded, RangeExceeded)) RangeExceeded = valueRangeExceeded;
        var valueTrueAirSpeed = TrueAirSpeed;
        UInt16Type.Accept(visitor, VisitorTrueAirSpeedField, ref valueTrueAirSpeed);
        if (!EqualityComparer<ushort>.Default.Equals(valueTrueAirSpeed, TrueAirSpeed)) TrueAirSpeed = valueTrueAirSpeed;
    }
}
