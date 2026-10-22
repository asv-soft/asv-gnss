using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI048Frn009Type240
{
    private static readonly Field VisitorAircraftIdentificationField = new Field.Builder().Name(nameof(AircraftIdentification)).Title(nameof(AircraftIdentification)).Description("AircraftIdentification").DataType(StringOptionalType.Utf8).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorAircraftIdentificationField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        string? valueAircraftIdentification = AircraftIdentification;
        StringOptionalType.Accept(visitor, VisitorAircraftIdentificationField, ref valueAircraftIdentification);
        if (!EqualityComparer<string?>.Default.Equals(valueAircraftIdentification, AircraftIdentification)) AircraftIdentification = valueAircraftIdentification;
    }
}
