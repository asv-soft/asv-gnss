using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn013Type220
{
    private static readonly Field VisitorTargetAddressField = new Field.Builder().Name(nameof(TargetAddress)).Title(nameof(TargetAddress)).Description("TargetAddress").DataType(Int32Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorTargetAddressField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueTargetAddress = TargetAddress;
        Int32Type.Accept(visitor, VisitorTargetAddressField, ref valueTargetAddress);
        if (!EqualityComparer<int>.Default.Equals(valueTargetAddress, TargetAddress)) TargetAddress = valueTargetAddress;
    }
}
