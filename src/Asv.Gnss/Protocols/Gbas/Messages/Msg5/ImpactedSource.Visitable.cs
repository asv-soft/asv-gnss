using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class ImpactedSource : IVisitable
{
    private static readonly Field PayloadRangingSourceIdField = ProtocolPayloadVisitor.Field(nameof(RangingSourceId), UInt8Type.Default);
    private static readonly Field PayloadSourceAvailabilitySenseField = ProtocolPayloadVisitor.Field(nameof(SourceAvailabilitySense), UInt8Type.Default, enumType: typeof(SourceAvailabilitySenseEnum));
    private static readonly Field PayloadSourceAvailabilityDurationField = ProtocolPayloadVisitor.Field(nameof(SourceAvailabilityDuration), UInt16Type.Default);

    internal static readonly StructType PayloadVisitorType = new([PayloadRangingSourceIdField, PayloadSourceAvailabilitySenseField, PayloadSourceAvailabilityDurationField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        byte valueRangingSourceId = RangingSourceId;
        UInt8Type.Accept(visitor, PayloadRangingSourceIdField, PayloadRangingSourceIdField.DataType, ref valueRangingSourceId);
        if (!EqualityComparer<byte>.Default.Equals(valueRangingSourceId, RangingSourceId))
        {
            RangingSourceId = valueRangingSourceId;
        }

        SourceAvailabilitySenseEnum valueSourceAvailabilitySense = SourceAvailabilitySense;
        var numericSourceAvailabilitySense = (byte)valueSourceAvailabilitySense;
        UInt8Type.Accept(visitor, PayloadSourceAvailabilitySenseField, PayloadSourceAvailabilitySenseField.DataType, ref numericSourceAvailabilitySense);
        valueSourceAvailabilitySense = (SourceAvailabilitySenseEnum)numericSourceAvailabilitySense;
        if (!EqualityComparer<SourceAvailabilitySenseEnum>.Default.Equals(valueSourceAvailabilitySense, SourceAvailabilitySense))
        {
            SourceAvailabilitySense = valueSourceAvailabilitySense;
        }

        ushort valueSourceAvailabilityDuration = SourceAvailabilityDuration;
        UInt16Type.Accept(visitor, PayloadSourceAvailabilityDurationField, PayloadSourceAvailabilityDurationField.DataType, ref valueSourceAvailabilityDuration);
        if (!EqualityComparer<ushort>.Default.Equals(valueSourceAvailabilityDuration, SourceAvailabilityDuration))
        {
            SourceAvailabilityDuration = valueSourceAvailabilityDuration;
        }
    }
}
