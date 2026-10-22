using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI021Frn033Type170
{
    private static readonly Field VisitorTargetIdentificationField = new Field.Builder().Name(nameof(TargetIdentification)).Title(nameof(TargetIdentification)).Description("TargetIdentification").DataType(StringOptionalType.Utf8).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorTargetIdentificationField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        string? valueTargetIdentification = TargetIdentification;
        StringOptionalType.Accept(visitor, VisitorTargetIdentificationField, ref valueTargetIdentification);
        if (!EqualityComparer<string?>.Default.Equals(valueTargetIdentification, TargetIdentification)) TargetIdentification = valueTargetIdentification;
    }
}
