using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn021Type070
{
    private static readonly Field VisitorMode3ACodeField = new Field.Builder().Name(nameof(Mode3ACode)).Title(nameof(Mode3ACode)).Description("Mode3ACode").DataType(UInt16Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorMode3ACodeField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueMode3ACode = Mode3ACode;
        UInt16Type.Accept(visitor, VisitorMode3ACodeField, ref valueMode3ACode);
        if (!EqualityComparer<ushort>.Default.Equals(valueMode3ACode, Mode3ACode)) Mode3ACode = valueMode3ACode;
    }
}
