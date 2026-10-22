using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn020Type550
{
    private static readonly Field VisitorRawStatusField = new Field.Builder().Name(nameof(RawStatus)).Title(nameof(RawStatus)).Description("RawStatus").DataType(UInt8Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorRawStatusField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueRawStatus = RawStatus;
        UInt8Type.Accept(visitor, VisitorRawStatusField, ref valueRawStatus);
        if (!EqualityComparer<byte>.Default.Equals(valueRawStatus, RawStatus)) RawStatus = valueRawStatus;
    }
}
