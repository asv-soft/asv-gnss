using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI001Frn003Type040
{
    private static readonly Field VisitorRhoField = new Field.Builder().Name(nameof(Rho)).Title(nameof(Rho)).Description("Rho").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorThetaField = new Field.Builder().Name(nameof(Theta)).Title(nameof(Theta)).Description("Theta").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorRhoField, VisitorThetaField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueRho = Rho;
        DoubleType.Accept(visitor, VisitorRhoField, ref valueRho);
        if (!EqualityComparer<double>.Default.Equals(valueRho, Rho)) Rho = valueRho;
        var valueTheta = Theta;
        DoubleType.Accept(visitor, VisitorThetaField, ref valueTheta);
        if (!EqualityComparer<double>.Default.Equals(valueTheta, Theta)) Theta = valueTheta;
    }
}
