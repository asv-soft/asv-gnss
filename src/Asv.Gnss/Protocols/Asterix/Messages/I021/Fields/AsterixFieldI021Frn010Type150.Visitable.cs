using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn010Type150
{
    private static readonly Field VisitorIsMachField = new Field.Builder().Name(nameof(IsMach)).Title(nameof(IsMach)).Description("IsMach").DataType(BoolType.Default).Build();
    private static readonly Field VisitorSpeedField = new Field.Builder().Name(nameof(Speed)).Title(nameof(Speed)).Description("Speed").DataType(DoubleType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorIsMachField, VisitorSpeedField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueIsMach = IsMach;
        BoolType.Accept(visitor, VisitorIsMachField, ref valueIsMach);
        if (!EqualityComparer<bool>.Default.Equals(valueIsMach, IsMach)) IsMach = valueIsMach;
        var valueSpeed = Speed;
        DoubleType.Accept(visitor, VisitorSpeedField, ref valueSpeed);
        if (!EqualityComparer<double>.Default.Equals(valueSpeed, Speed)) Speed = valueSpeed;
    }
}
