using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn025Type152
{
    private static readonly Field VisitorMagneticHeadingDegField = new Field.Builder().Name(nameof(MagneticHeadingDeg)).Title(nameof(MagneticHeadingDeg)).Description("MagneticHeadingDeg").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorMagneticHeadingDegField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueMagneticHeadingDeg = MagneticHeadingDeg;
        DoubleType.Accept(visitor, VisitorMagneticHeadingDegField, ref valueMagneticHeadingDeg);
        if (!EqualityComparer<double>.Default.Equals(valueMagneticHeadingDeg, MagneticHeadingDeg)) MagneticHeadingDeg = valueMagneticHeadingDeg;
    }
}
