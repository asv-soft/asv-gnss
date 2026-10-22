using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI034Frn008Type070
{
    private static readonly Field VisitorItemsField = new Field.Builder().Name(nameof(Items)).Title(nameof(Items)).Description("Items").DataType(new ListType(AsterixI034MessageCountValue.AsterixVisitorType, 0, ushort.MaxValue)).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorItemsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueItems = Items;
        AsterixVisitor.VisitList(visitor, VisitorItemsField, ref valueItems,
            static () => new AsterixI034MessageCountValue(),
            static (IVisitor visitor, Field field, IFieldType type, ref AsterixI034MessageCountValue item) =>
            {
                StructType.Accept(visitor, field, type, item);
            });
    }
}
