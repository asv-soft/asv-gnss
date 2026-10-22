using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021HighPrecisionTime
{
    private static readonly Field VisitorFullSecondIndicationField = new Field.Builder().Name(nameof(FullSecondIndication)).Title(nameof(FullSecondIndication)).Description("FullSecondIndication").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorFractionalSecondsField = new Field.Builder().Name(nameof(FractionalSeconds)).Title(nameof(FractionalSeconds)).Description("FractionalSeconds").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorFullSecondIndicationField, VisitorFractionalSecondsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueFullSecondIndication = FullSecondIndication;
        UInt8Type.Accept(visitor, VisitorFullSecondIndicationField, ref valueFullSecondIndication);
        if (!EqualityComparer<byte>.Default.Equals(valueFullSecondIndication, FullSecondIndication)) FullSecondIndication = valueFullSecondIndication;
        var valueFractionalSeconds = FractionalSeconds;
        DoubleType.Accept(visitor, VisitorFractionalSecondsField, ref valueFractionalSeconds);
        if (!EqualityComparer<double>.Default.Equals(valueFractionalSeconds, FractionalSeconds)) FractionalSeconds = valueFractionalSeconds;
    }
}
