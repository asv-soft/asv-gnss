using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn009Type202
{
    private static readonly Field VisitorIsSensisEncodingField = new Field.Builder().Name(nameof(IsSensisEncoding)).Title(nameof(IsSensisEncoding)).Description("IsSensisEncoding").DataType(BoolType.Default).Build();
    private static readonly Field VisitorVxField = new Field.Builder().Name(nameof(Vx)).Title(nameof(Vx)).Description("Vx").DataType(DoubleType.Default).Build();
    private static readonly Field VisitorVyField = new Field.Builder().Name(nameof(Vy)).Title(nameof(Vy)).Description("Vy").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorIsSensisEncodingField, VisitorVxField, VisitorVyField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueIsSensisEncoding = IsSensisEncoding;
        BoolType.Accept(visitor, VisitorIsSensisEncodingField, ref valueIsSensisEncoding);
        if (!EqualityComparer<bool>.Default.Equals(valueIsSensisEncoding, IsSensisEncoding)) IsSensisEncoding = valueIsSensisEncoding;
        var valueVx = Vx;
        DoubleType.Accept(visitor, VisitorVxField, ref valueVx);
        if (!EqualityComparer<double>.Default.Equals(valueVx, Vx)) Vx = valueVx;
        var valueVy = Vy;
        DoubleType.Accept(visitor, VisitorVyField, ref valueVy);
        if (!EqualityComparer<double>.Default.Equals(valueVy, Vy)) Vy = valueVy;
    }
}
