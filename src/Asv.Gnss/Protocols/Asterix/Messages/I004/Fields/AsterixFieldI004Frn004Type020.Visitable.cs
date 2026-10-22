using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI004Frn004Type020
{
    private static readonly Field VisitorSecondsField = new Field.Builder().Name(nameof(Seconds)).Title(nameof(Seconds)).Description("Seconds").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorSecondsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueSeconds = Seconds;
        DoubleType.Accept(visitor, VisitorSecondsField, ref valueSeconds);
        if (!EqualityComparer<double>.Default.Equals(valueSeconds, Seconds)) Seconds = valueSeconds;
    }
}
