using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn016Type300
{
    private static readonly Field VisitorVehicleFleetIdentificationField = new Field.Builder().Name(nameof(VehicleFleetIdentification)).Title(nameof(VehicleFleetIdentification)).Description("VehicleFleetIdentification").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorVehicleFleetIdentificationField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueVehicleFleetIdentification = VehicleFleetIdentification;
        UInt8Type.Accept(visitor, VisitorVehicleFleetIdentificationField, ref valueVehicleFleetIdentification);
        if (!EqualityComparer<byte>.Default.Equals(valueVehicleFleetIdentification, VehicleFleetIdentification)) VehicleFleetIdentification = valueVehicleFleetIdentification;
    }
}
