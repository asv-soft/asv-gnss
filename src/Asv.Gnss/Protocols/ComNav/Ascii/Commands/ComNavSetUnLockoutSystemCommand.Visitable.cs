using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ComNavSetUnLockoutSystemCommand
{
    private static readonly Field PayloadSatelliteSystemField = SbfComNavVisitor.Field(nameof(SatelliteSystem), Int32Type.Default, "SatelliteSystem", enumType: typeof(ComNavSatelliteSystemEnum));

    internal new static readonly StructType PayloadVisitorType = new([..ComNavAsciiCommandBase.PayloadVisitorType.Fields, PayloadSatelliteSystemField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueSatelliteSystem = SatelliteSystem;
        var numericvalueSatelliteSystem = (int)valueSatelliteSystem;
        Int32Type.Accept(visitor, PayloadSatelliteSystemField, PayloadSatelliteSystemField.DataType, ref numericvalueSatelliteSystem);
        valueSatelliteSystem = (ComNavSatelliteSystemEnum)numericvalueSatelliteSystem;
        if (!EqualityComparer<ComNavSatelliteSystemEnum>.Default.Equals(valueSatelliteSystem, SatelliteSystem))
        {
            SatelliteSystem = valueSatelliteSystem;
        }
    }
}
