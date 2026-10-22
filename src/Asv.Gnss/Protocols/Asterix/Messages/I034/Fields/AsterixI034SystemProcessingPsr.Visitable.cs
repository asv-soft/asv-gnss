using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixI034SystemProcessingPsr : IVisitable
{
    private static readonly Field VisitorPolarizationField = new Field.Builder().Name(nameof(Polarization)).Title(nameof(Polarization)).Description("Polarization").DataType(BoolType.Default).Build();
    private static readonly Field VisitorReducedRadarField = new Field.Builder().Name(nameof(ReducedRadar)).Title(nameof(ReducedRadar)).Description("ReducedRadar").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSensitivityTimeControlField = new Field.Builder().Name(nameof(SensitivityTimeControl)).Title(nameof(SensitivityTimeControl)).Description("SensitivityTimeControl").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorPolarizationField, VisitorReducedRadarField, VisitorSensitivityTimeControlField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valuePolarization = Polarization;
        BoolType.Accept(visitor, VisitorPolarizationField, ref valuePolarization);
        if (!EqualityComparer<bool>.Default.Equals(valuePolarization, Polarization)) Polarization = valuePolarization;
        var valueReducedRadar = ReducedRadar;
        BoolType.Accept(visitor, VisitorReducedRadarField, ref valueReducedRadar);
        if (!EqualityComparer<bool>.Default.Equals(valueReducedRadar, ReducedRadar)) ReducedRadar = valueReducedRadar;
        var valueSensitivityTimeControl = SensitivityTimeControl;
        BoolType.Accept(visitor, VisitorSensitivityTimeControlField, ref valueSensitivityTimeControl);
        if (!EqualityComparer<bool>.Default.Equals(valueSensitivityTimeControl, SensitivityTimeControl)) SensitivityTimeControl = valueSensitivityTimeControl;
    }
}
