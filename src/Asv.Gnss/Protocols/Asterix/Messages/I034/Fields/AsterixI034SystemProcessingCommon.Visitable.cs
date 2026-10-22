using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class AsterixI034SystemProcessingCommon : IVisitable
{
    private static readonly Field VisitorReducedRdpField = new Field.Builder().Name(nameof(ReducedRdp)).Title(nameof(ReducedRdp)).Description("ReducedRdp").DataType(BoolType.Default).Build();
    private static readonly Field VisitorReducedTransmissionField = new Field.Builder().Name(nameof(ReducedTransmission)).Title(nameof(ReducedTransmission)).Description("ReducedTransmission").DataType(BoolType.Default).Build();

    internal static readonly StructType AsterixVisitorType = new([VisitorReducedRdpField, VisitorReducedTransmissionField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var valueReducedRdp = ReducedRdp;
        BoolType.Accept(visitor, VisitorReducedRdpField, ref valueReducedRdp);
        if (!EqualityComparer<bool>.Default.Equals(valueReducedRdp, ReducedRdp)) ReducedRdp = valueReducedRdp;
        var valueReducedTransmission = ReducedTransmission;
        BoolType.Accept(visitor, VisitorReducedTransmissionField, ref valueReducedTransmission);
        if (!EqualityComparer<bool>.Default.Equals(valueReducedTransmission, ReducedTransmission)) ReducedTransmission = valueReducedTransmission;
    }
}
