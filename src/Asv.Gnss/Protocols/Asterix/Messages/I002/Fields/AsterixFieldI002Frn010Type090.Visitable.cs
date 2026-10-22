using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI002Frn010Type090
{
    private static readonly Field VisitorRangeErrorNmField = new Field.Builder().Name(nameof(RangeErrorNm)).Title(nameof(RangeErrorNm)).Description("RangeErrorNm").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorAzimuthErrorDegField = new Field.Builder().Name(nameof(AzimuthErrorDeg)).Title(nameof(AzimuthErrorDeg)).Description("AzimuthErrorDeg").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorRangeErrorNmField, VisitorAzimuthErrorDegField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueRangeErrorNm = RangeErrorNm;
        DoubleType.Accept(visitor, VisitorRangeErrorNmField, ref valueRangeErrorNm);
        if (!EqualityComparer<double>.Default.Equals(valueRangeErrorNm, RangeErrorNm)) RangeErrorNm = valueRangeErrorNm;
        var valueAzimuthErrorDeg = AzimuthErrorDeg;
        DoubleType.Accept(visitor, VisitorAzimuthErrorDegField, ref valueAzimuthErrorDeg);
        if (!EqualityComparer<double>.Default.Equals(valueAzimuthErrorDeg, AzimuthErrorDeg)) AzimuthErrorDeg = valueAzimuthErrorDeg;
    }
}
