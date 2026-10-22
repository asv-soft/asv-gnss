using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn037Type148
{
    private static readonly Field VisitorManageVerticalModeField = new Field.Builder().Name(nameof(ManageVerticalMode)).Title(nameof(ManageVerticalMode)).Description("ManageVerticalMode").DataType(BoolType.Default).Build();
    private static readonly Field VisitorAltitudeHoldModeField = new Field.Builder().Name(nameof(AltitudeHoldMode)).Title(nameof(AltitudeHoldMode)).Description("AltitudeHoldMode").DataType(BoolType.Default).Build();
    private static readonly Field VisitorApproachModeField = new Field.Builder().Name(nameof(ApproachMode)).Title(nameof(ApproachMode)).Description("ApproachMode").DataType(BoolType.Default).Build();

    internal new static readonly StructType AsterixVisitorType = new([..AsterixFieldI021SelectedAltitude.AsterixVisitorType.Fields, VisitorManageVerticalModeField, VisitorAltitudeHoldModeField, VisitorApproachModeField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);
        var valueManageVerticalMode = ManageVerticalMode;
        BoolType.Accept(visitor, VisitorManageVerticalModeField, ref valueManageVerticalMode);
        if (!EqualityComparer<bool>.Default.Equals(valueManageVerticalMode, ManageVerticalMode)) ManageVerticalMode = valueManageVerticalMode;
        var valueAltitudeHoldMode = AltitudeHoldMode;
        BoolType.Accept(visitor, VisitorAltitudeHoldModeField, ref valueAltitudeHoldMode);
        if (!EqualityComparer<bool>.Default.Equals(valueAltitudeHoldMode, AltitudeHoldMode)) AltitudeHoldMode = valueAltitudeHoldMode;
        var valueApproachMode = ApproachMode;
        BoolType.Accept(visitor, VisitorApproachModeField, ref valueApproachMode);
        if (!EqualityComparer<bool>.Default.Equals(valueApproachMode, ApproachMode)) ApproachMode = valueApproachMode;
    }
}
