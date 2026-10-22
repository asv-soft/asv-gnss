using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn035Type220
{
    private static readonly Field VisitorWindSpeedField = new Field.Builder().Name(nameof(WindSpeed)).Title(nameof(WindSpeed)).Description("WindSpeed").DataType(DoubleOptionalType.Default).Build();
    private static readonly Field VisitorWindDirectionField = new Field.Builder().Name(nameof(WindDirection)).Title(nameof(WindDirection)).Description("WindDirection").DataType(DoubleOptionalType.Default).Build();
    private static readonly Field VisitorTemperatureField = new Field.Builder().Name(nameof(Temperature)).Title(nameof(Temperature)).Description("Temperature").DataType(DoubleOptionalType.Default).Build();
    private static readonly Field VisitorTurbulenceField = new Field.Builder().Name(nameof(Turbulence)).Title(nameof(Turbulence)).Description("Turbulence").DataType(UInt8OptionalType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorWindSpeedField, VisitorWindDirectionField, VisitorTemperatureField, VisitorTurbulenceField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueWindSpeed = WindSpeed;
        DoubleOptionalType.Accept(visitor, VisitorWindSpeedField, ref valueWindSpeed);
        if (!EqualityComparer<double?>.Default.Equals(valueWindSpeed, WindSpeed)) WindSpeed = valueWindSpeed;
        var valueWindDirection = WindDirection;
        DoubleOptionalType.Accept(visitor, VisitorWindDirectionField, ref valueWindDirection);
        if (!EqualityComparer<double?>.Default.Equals(valueWindDirection, WindDirection)) WindDirection = valueWindDirection;
        var valueTemperature = Temperature;
        DoubleOptionalType.Accept(visitor, VisitorTemperatureField, ref valueTemperature);
        if (!EqualityComparer<double?>.Default.Equals(valueTemperature, Temperature)) Temperature = valueTemperature;
        var valueTurbulence = Turbulence;
        UInt8OptionalType.Accept(visitor, VisitorTurbulenceField, ref valueTurbulence);
        if (!EqualityComparer<byte?>.Default.Equals(valueTurbulence, Turbulence)) Turbulence = valueTurbulence;
    }
}
