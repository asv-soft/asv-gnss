using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI002Frn013TypeSp
{
    private static readonly Field VisitorDataField = new Field.Builder().Name(nameof(Data)).Title(nameof(Data)).Description("Data").DataType(new ListType(UInt8Type.Default, 0, ushort.MaxValue)).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorDataField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueData = Data;
        AsterixVisitor.VisitArray(visitor, VisitorDataField, ref valueData,
            static () => new byte(),
            static (IVisitor visitor, Field field, IFieldType type, ref byte item) =>
            {
                UInt8Type.Accept(visitor, field, type, ref item);
            });
        if (!ReferenceEquals(valueData, Data)) Data = valueData!;
    }
}
