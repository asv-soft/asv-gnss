using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn008Type200
{
    private static readonly Field VisitorGroundSpeedField = new Field.Builder().Name(nameof(GroundSpeed)).Title(nameof(GroundSpeed)).Description("GroundSpeed").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorTrackAngleField = new Field.Builder().Name(nameof(TrackAngle)).Title(nameof(TrackAngle)).Description("TrackAngle").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorGroundSpeedField, VisitorTrackAngleField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueGroundSpeed = GroundSpeed;
        DoubleType.Accept(visitor, VisitorGroundSpeedField, ref valueGroundSpeed);
        if (!EqualityComparer<double>.Default.Equals(valueGroundSpeed, GroundSpeed)) GroundSpeed = valueGroundSpeed;
        var valueTrackAngle = TrackAngle;
        DoubleType.Accept(visitor, VisitorTrackAngleField, ref valueTrackAngle);
        if (!EqualityComparer<double>.Default.Equals(valueTrackAngle, TrackAngle)) TrackAngle = valueTrackAngle;
    }
}
