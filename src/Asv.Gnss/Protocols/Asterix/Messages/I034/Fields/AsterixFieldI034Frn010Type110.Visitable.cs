using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI034Frn010Type110
{
    private static readonly Field VisitorFilterField = new Field.Builder().Name(nameof(Filter)).Title(nameof(Filter)).Description("Filter").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorFilterField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueFilter = Filter;
        UInt8Type.Accept(visitor, VisitorFilterField, ref valueFilter);
        if (!EqualityComparer<byte>.Default.Equals(valueFilter, Filter)) Filter = valueFilter;
    }
}
