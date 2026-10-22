using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixFieldI010Frn014Type245
{
    private static readonly Field VisitorStiField = new Field.Builder().Name(nameof(Sti)).Title(nameof(Sti)).Description("Sti").DataType(UInt8Type.Default).Metadata("enumType", typeof(StiEnum).FullName).Build();
    private static readonly Field VisitorTargetIdentificationField = new Field.Builder().Name(nameof(TargetIdentification)).Title(nameof(TargetIdentification)).Description("TargetIdentification").DataType(StringOptionalType.Utf8).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorStiField, VisitorTargetIdentificationField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueSti = (byte)Sti;
        UInt8Type.Accept(visitor, VisitorStiField, ref valueSti);
        if (!EqualityComparer<byte>.Default.Equals(valueSti, (byte)Sti)) Sti = (StiEnum)valueSti;
        string? valueTargetIdentification = TargetIdentification;
        StringOptionalType.Accept(visitor, VisitorTargetIdentificationField, ref valueTargetIdentification);
        if (!EqualityComparer<string?>.Default.Equals(valueTargetIdentification, TargetIdentification)) TargetIdentification = valueTargetIdentification;
    }
}
