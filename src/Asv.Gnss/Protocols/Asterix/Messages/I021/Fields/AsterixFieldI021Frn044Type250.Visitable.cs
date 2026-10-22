using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn044Type250
{
    private static readonly Field VisitorDataField = new Field.Builder().Name(nameof(Data)).Title(nameof(Data)).Description("Data").DataType(new ListType(AsterixI021ModeSData.AsterixVisitorType, 0, ushort.MaxValue)).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorDataField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueData = Data;
        AsterixVisitor.VisitList(visitor, VisitorDataField, ref valueData,
            static () => new AsterixI021ModeSData(),
            static (IVisitor visitor, Field field, IFieldType type, ref AsterixI021ModeSData item) =>
            {
                StructType.Accept(visitor, field, type, item);
            });
    }
}
