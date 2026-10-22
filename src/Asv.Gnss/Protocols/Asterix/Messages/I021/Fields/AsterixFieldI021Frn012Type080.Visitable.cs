using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn012Type080
{
    private static readonly Field VisitorTargetAddressField = new Field.Builder().Name(nameof(TargetAddress)).Title(nameof(TargetAddress)).Description("TargetAddress").DataType(UInt32Type.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorTargetAddressField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueTargetAddress = TargetAddress;
        UInt32Type.Accept(visitor, VisitorTargetAddressField, ref valueTargetAddress);
        if (!EqualityComparer<uint>.Default.Equals(valueTargetAddress, TargetAddress)) TargetAddress = valueTargetAddress;
    }
}
