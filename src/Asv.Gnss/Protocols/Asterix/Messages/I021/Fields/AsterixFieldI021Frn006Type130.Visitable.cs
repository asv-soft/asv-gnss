using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn006Type130
{
    private static readonly Field VisitorLatitudeField = new Field.Builder().Name(nameof(Latitude)).Title(nameof(Latitude)).Description("Latitude").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorLongitudeField = new Field.Builder().Name(nameof(Longitude)).Title(nameof(Longitude)).Description("Longitude").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorLatitudeField, VisitorLongitudeField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueLatitude = Latitude;
        DoubleType.Accept(visitor, VisitorLatitudeField, ref valueLatitude);
        if (!EqualityComparer<double>.Default.Equals(valueLatitude, Latitude)) Latitude = valueLatitude;
        var valueLongitude = Longitude;
        DoubleType.Accept(visitor, VisitorLongitudeField, ref valueLongitude);
        if (!EqualityComparer<double>.Default.Equals(valueLongitude, Longitude)) Longitude = valueLongitude;
    }
}
