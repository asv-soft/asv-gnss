using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI004Frn018Type110
{
    private static readonly Field VisitorItemsField = new Field.Builder().Name(nameof(Items)).Title(nameof(Items)).Description("Items").DataType(new ListType(AsterixI004FdpsSectorControl.AsterixVisitorType, 0, ushort.MaxValue)).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorItemsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueItems = Items;
        AsterixVisitor.VisitList(visitor, VisitorItemsField, ref valueItems,
            static () => new AsterixI004FdpsSectorControl(),
            static (IVisitor visitor, Field field, IFieldType type, ref AsterixI004FdpsSectorControl item) =>
            {
                StructType.Accept(visitor, field, type, item);
            });
    }
}
