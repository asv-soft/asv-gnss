using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class EcefPoint : IVisitable
{
    private static readonly Field RtcmXField = RtcmV3Visitor.Field(nameof(X), DoubleType.Default, "X");
    private static readonly Field RtcmYField = RtcmV3Visitor.Field(nameof(Y), DoubleType.Default, "Y");
    private static readonly Field RtcmZField = RtcmV3Visitor.Field(nameof(Z), DoubleType.Default, "Z");

    internal static readonly StructType RtcmVisitorType = new([RtcmXField, RtcmYField, RtcmZField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valueX = X;
        DoubleType.Accept(visitor, RtcmXField, RtcmXField.DataType, ref valueX);
        if (!EqualityComparer<double>.Default.Equals(valueX, X))
        {
            X = valueX;
        }

        var valueY = Y;
        DoubleType.Accept(visitor, RtcmYField, RtcmYField.DataType, ref valueY);
        if (!EqualityComparer<double>.Default.Equals(valueY, Y))
        {
            Y = valueY;
        }

        var valueZ = Z;
        DoubleType.Accept(visitor, RtcmZField, RtcmZField.DataType, ref valueZ);
        if (!EqualityComparer<double>.Default.Equals(valueZ, Z))
        {
            Z = valueZ;
        }
    }
}
