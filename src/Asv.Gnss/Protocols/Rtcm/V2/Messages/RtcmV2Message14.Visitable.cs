using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV2Message14
{
    private static readonly Field PayloadLeapSecondsField = ProtocolPayloadVisitor.Field(nameof(LeapSeconds), UInt8Type.Default);

    internal new static readonly StructType PayloadVisitorType = new([..RtcmV2MessageBase.PayloadVisitorType.Fields, PayloadLeapSecondsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        byte valueLeapSeconds = LeapSeconds;
        UInt8Type.Accept(visitor, PayloadLeapSecondsField, PayloadLeapSecondsField.DataType, ref valueLeapSeconds);
        if (!EqualityComparer<byte>.Default.Equals(valueLeapSeconds, LeapSeconds))
        {
            LeapSeconds = valueLeapSeconds;
        }
    }
}
