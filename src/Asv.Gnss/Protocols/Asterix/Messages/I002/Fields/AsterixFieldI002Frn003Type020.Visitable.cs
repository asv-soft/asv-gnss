using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI002Frn003Type020
{
    private static readonly Field VisitorSectorNumberDegField = new Field.Builder().Name(nameof(SectorNumberDeg)).Title(nameof(SectorNumberDeg)).Description("SectorNumberDeg").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorSectorNumberDegField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueSectorNumberDeg = SectorNumberDeg;
        DoubleType.Accept(visitor, VisitorSectorNumberDegField, ref valueSectorNumberDeg);
        if (!EqualityComparer<double>.Default.Equals(valueSectorNumberDeg, SectorNumberDeg)) SectorNumberDeg = valueSectorNumberDeg;
    }
}
