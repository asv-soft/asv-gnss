using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn030Type165
{
    private static readonly Field VisitorTrackAngleRateDegPerSecondField = new Field.Builder().Name(nameof(TrackAngleRateDegPerSecond)).Title(nameof(TrackAngleRateDegPerSecond)).Description("TrackAngleRateDegPerSecond").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorTrackAngleRateDegPerSecondField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueTrackAngleRateDegPerSecond = TrackAngleRateDegPerSecond;
        DoubleType.Accept(visitor, VisitorTrackAngleRateDegPerSecondField, ref valueTrackAngleRateDegPerSecond);
        if (!EqualityComparer<double>.Default.Equals(valueTrackAngleRateDegPerSecond, TrackAngleRateDegPerSecond)) TrackAngleRateDegPerSecond = valueTrackAngleRateDegPerSecond;
    }
}
