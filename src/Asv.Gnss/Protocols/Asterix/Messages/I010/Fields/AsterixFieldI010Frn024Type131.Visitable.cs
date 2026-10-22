using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn024Type131
{
    private static readonly Field VisitorAmplitudeField = new Field.Builder().Name(nameof(Amplitude)).Title(nameof(Amplitude)).Description("Amplitude").DataType(Int8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorAmplitudeField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueAmplitude = Amplitude;
        Int8Type.Accept(visitor, VisitorAmplitudeField, ref valueAmplitude);
        if (!EqualityComparer<sbyte>.Default.Equals(valueAmplitude, Amplitude)) Amplitude = valueAmplitude;
    }
}
