using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI002ExtendableValues
{
    private static readonly Field VisitorValuesField = new Field.Builder().Name(nameof(Values)).Title(nameof(Values)).Description("Values").DataType(new ListType(UInt8Type.Default, 0, ushort.MaxValue)).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorValuesField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueValues = Values;
        AsterixVisitor.VisitList(visitor, VisitorValuesField, ref valueValues,
            static () => new byte(),
            static (IVisitor visitor, Field field, IFieldType type, ref byte item) =>
            {
                UInt8Type.Accept(visitor, field, type, ref item);
            });
    }
}
