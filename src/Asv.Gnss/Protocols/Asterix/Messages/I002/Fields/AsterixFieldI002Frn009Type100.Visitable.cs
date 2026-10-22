using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI002Frn009Type100
{
    private static readonly Field VisitorRhoStartNmField = new Field.Builder().Name(nameof(RhoStartNm)).Title(nameof(RhoStartNm)).Description("RhoStartNm").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorRhoEndNmField = new Field.Builder().Name(nameof(RhoEndNm)).Title(nameof(RhoEndNm)).Description("RhoEndNm").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorThetaStartDegField = new Field.Builder().Name(nameof(ThetaStartDeg)).Title(nameof(ThetaStartDeg)).Description("ThetaStartDeg").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorThetaEndDegField = new Field.Builder().Name(nameof(ThetaEndDeg)).Title(nameof(ThetaEndDeg)).Description("ThetaEndDeg").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorRhoStartNmField, VisitorRhoEndNmField, VisitorThetaStartDegField, VisitorThetaEndDegField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueRhoStartNm = RhoStartNm;
        DoubleType.Accept(visitor, VisitorRhoStartNmField, ref valueRhoStartNm);
        if (!EqualityComparer<double>.Default.Equals(valueRhoStartNm, RhoStartNm)) RhoStartNm = valueRhoStartNm;
        var valueRhoEndNm = RhoEndNm;
        DoubleType.Accept(visitor, VisitorRhoEndNmField, ref valueRhoEndNm);
        if (!EqualityComparer<double>.Default.Equals(valueRhoEndNm, RhoEndNm)) RhoEndNm = valueRhoEndNm;
        var valueThetaStartDeg = ThetaStartDeg;
        DoubleType.Accept(visitor, VisitorThetaStartDegField, ref valueThetaStartDeg);
        if (!EqualityComparer<double>.Default.Equals(valueThetaStartDeg, ThetaStartDeg)) ThetaStartDeg = valueThetaStartDeg;
        var valueThetaEndDeg = ThetaEndDeg;
        DoubleType.Accept(visitor, VisitorThetaEndDegField, ref valueThetaEndDeg);
        if (!EqualityComparer<double>.Default.Equals(valueThetaEndDeg, ThetaEndDeg)) ThetaEndDeg = valueThetaEndDeg;
    }
}
