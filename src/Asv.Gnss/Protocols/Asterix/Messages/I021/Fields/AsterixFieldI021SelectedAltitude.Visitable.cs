using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021SelectedAltitude
{
    private static readonly Field VisitorSourceAvailabilityField = new Field.Builder().Name(nameof(SourceAvailability)).Title(nameof(SourceAvailability)).Description("SourceAvailability").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSourceField = new Field.Builder().Name(nameof(Source)).Title(nameof(Source)).Description("Source").DataType(UInt8Type.Default).Build();
    private static readonly Field VisitorAltitudeFtField = new Field.Builder().Name(nameof(AltitudeFt)).Title(nameof(AltitudeFt)).Description("AltitudeFt").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorSourceAvailabilityField, VisitorSourceField, VisitorAltitudeFtField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueSourceAvailability = SourceAvailability;
        BoolType.Accept(visitor, VisitorSourceAvailabilityField, ref valueSourceAvailability);
        if (!EqualityComparer<bool>.Default.Equals(valueSourceAvailability, SourceAvailability)) SourceAvailability = valueSourceAvailability;
        var valueSource = Source;
        UInt8Type.Accept(visitor, VisitorSourceField, ref valueSource);
        if (!EqualityComparer<byte>.Default.Equals(valueSource, Source)) Source = valueSource;
        var valueAltitudeFt = AltitudeFt;
        DoubleType.Accept(visitor, VisitorAltitudeFtField, ref valueAltitudeFt);
        if (!EqualityComparer<double>.Default.Equals(valueAltitudeFt, AltitudeFt)) AltitudeFt = valueAltitudeFt;
    }
}
