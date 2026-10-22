using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI034Frn005Type041
{
    private static readonly Field VisitorAntennaRotationPeriodField = new Field.Builder().Name(nameof(AntennaRotationPeriod)).Title(nameof(AntennaRotationPeriod)).Description("AntennaRotationPeriod").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorAntennaRotationPeriodField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueAntennaRotationPeriod = AntennaRotationPeriod;
        DoubleType.Accept(visitor, VisitorAntennaRotationPeriodField, ref valueAntennaRotationPeriod);
        if (!EqualityComparer<double>.Default.Equals(valueAntennaRotationPeriod, AntennaRotationPeriod)) AntennaRotationPeriod = valueAntennaRotationPeriod;
    }
}
