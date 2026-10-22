using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn029Type160
{
    private static readonly Field VisitorRangeExceededField = new Field.Builder().Name(nameof(RangeExceeded)).Title(nameof(RangeExceeded)).Description("RangeExceeded").DataType(BoolType.Default).Build();
    private static readonly Field VisitorGroundSpeedField = new Field.Builder().Name(nameof(GroundSpeed)).Title(nameof(GroundSpeed)).Description("GroundSpeed").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorTrackAngleDegField = new Field.Builder().Name(nameof(TrackAngleDeg)).Title(nameof(TrackAngleDeg)).Description("TrackAngleDeg").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorRangeExceededField, VisitorGroundSpeedField, VisitorTrackAngleDegField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueRangeExceeded = RangeExceeded;
        BoolType.Accept(visitor, VisitorRangeExceededField, ref valueRangeExceeded);
        if (!EqualityComparer<bool>.Default.Equals(valueRangeExceeded, RangeExceeded)) RangeExceeded = valueRangeExceeded;
        var valueGroundSpeed = GroundSpeed;
        DoubleType.Accept(visitor, VisitorGroundSpeedField, ref valueGroundSpeed);
        if (!EqualityComparer<double>.Default.Equals(valueGroundSpeed, GroundSpeed)) GroundSpeed = valueGroundSpeed;
        var valueTrackAngleDeg = TrackAngleDeg;
        DoubleType.Accept(visitor, VisitorTrackAngleDegField, ref valueTrackAngleDeg);
        if (!EqualityComparer<double>.Default.Equals(valueTrackAngleDeg, TrackAngleDeg)) TrackAngleDeg = valueTrackAngleDeg;
    }
}
