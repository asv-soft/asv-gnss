using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI048Frn008Type220
{
    private static readonly Field VisitorAircraftAddressField = new Field.Builder().Name(nameof(AircraftAddress)).Title(nameof(AircraftAddress)).Description("AircraftAddress").DataType(UInt32Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorAircraftAddressField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueAircraftAddress = AircraftAddress;
        UInt32Type.Accept(visitor, VisitorAircraftAddressField, ref valueAircraftAddress);
        if (!EqualityComparer<uint>.Default.Equals(valueAircraftAddress, AircraftAddress)) AircraftAddress = valueAircraftAddress;
    }
}
