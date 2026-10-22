using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn034Type020
{
    private static readonly Field VisitorEmitterCategoryField = new Field.Builder().Name(nameof(EmitterCategory)).Title(nameof(EmitterCategory)).Description("EmitterCategory").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorEmitterCategoryField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueEmitterCategory = EmitterCategory;
        UInt8Type.Accept(visitor, VisitorEmitterCategoryField, ref valueEmitterCategory);
        if (!EqualityComparer<byte>.Default.Equals(valueEmitterCategory, EmitterCategory)) EmitterCategory = valueEmitterCategory;
    }
}
