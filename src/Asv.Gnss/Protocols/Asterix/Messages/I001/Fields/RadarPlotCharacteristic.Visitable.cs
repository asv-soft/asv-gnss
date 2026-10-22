using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial record struct RadarPlotCharacteristic : IVisitable
{
    private static readonly Field VisitorValueField = new Field.Builder().Name(nameof(Value)).Title(nameof(Value)).Description("Value").DataType(UInt8Type.Default).Metadata("readOnly", true).Build();
    private static readonly Field VisitorExtendField = new Field.Builder().Name(nameof(Extend)).Title(nameof(Extend)).Description("Extend").DataType(BoolType.Default).Metadata("readOnly", true).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorValueField, VisitorExtendField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueValue = Value;
        UInt8Type.Accept(visitor, VisitorValueField, ref valueValue);
        var valueExtend = Extend;
        BoolType.Accept(visitor, VisitorExtendField, ref valueExtend);
    }
}
