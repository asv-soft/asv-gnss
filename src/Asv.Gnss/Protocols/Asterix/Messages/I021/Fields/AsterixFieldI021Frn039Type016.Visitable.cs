using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn039Type016
{
    private static readonly Field VisitorReportPeriodField = new Field.Builder().Name(nameof(ReportPeriod)).Title(nameof(ReportPeriod)).Description("ReportPeriod").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorReportPeriodField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueReportPeriod = ReportPeriod;
        DoubleType.Accept(visitor, VisitorReportPeriodField, ref valueReportPeriod);
        if (!EqualityComparer<double>.Default.Equals(valueReportPeriod, ReportPeriod)) ReportPeriod = valueReportPeriod;
    }
}
