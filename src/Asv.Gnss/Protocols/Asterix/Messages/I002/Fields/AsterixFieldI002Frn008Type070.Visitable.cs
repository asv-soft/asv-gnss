using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI002Frn008Type070
{
    private static readonly Field VisitorItemsField = new Field.Builder().Name(nameof(Items)).Title(nameof(Items)).Description("Items").DataType(new ListType(AsterixI002PlotCountValue.AsterixVisitorType, 0, ushort.MaxValue)).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorItemsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueItems = Items;
        AsterixVisitor.VisitList(visitor, VisitorItemsField, ref valueItems,
            static () => new AsterixI002PlotCountValue(),
            static (IVisitor visitor, Field field, IFieldType type, ref AsterixI002PlotCountValue item) =>
            {
                StructType.Accept(visitor, field, type, item);
            });
    }
}
