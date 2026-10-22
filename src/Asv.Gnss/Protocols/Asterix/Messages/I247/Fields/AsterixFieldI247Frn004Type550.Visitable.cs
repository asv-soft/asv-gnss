using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI247Frn004Type550
{
    private static readonly Field VisitorVersionsField = new Field.Builder().Name(nameof(Versions)).Title(nameof(Versions)).Description("Versions").DataType(new ListType(AsterixCategoryVersion.AsterixVisitorType, 0, ushort.MaxValue)).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorVersionsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueVersions = Versions;
        AsterixVisitor.VisitList(visitor, VisitorVersionsField, ref valueVersions,
            static () => new AsterixCategoryVersion(),
            static (IVisitor visitor, Field field, IFieldType type, ref AsterixCategoryVersion item) =>
            {
                StructType.Accept(visitor, field, type, item);
            });
    }
}
