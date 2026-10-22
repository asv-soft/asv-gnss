using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV2Message15
{
    private static readonly Field PayloadDelaysField = ProtocolPayloadVisitor.Field(nameof(Delays), new ListType(IonosphericDelayItem.PayloadVisitorType, 0, int.MaxValue));

    internal new static readonly StructType PayloadVisitorType = new([..RtcmV2MessageBase.PayloadVisitorType.Fields, PayloadDelaysField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        IonosphericDelayItem[] valueDelays = Delays;
        ProtocolPayloadVisitor.VisitArray(visitor, PayloadDelaysField, PayloadDelaysField.DataType, ref valueDelays,
            static () => new IonosphericDelayItem(),
            static (IVisitor v, Field f, IFieldType t, ref IonosphericDelayItem item) =>
            {
                if (item is not null)
                {
                    StructType.Accept(v, f, t, item);
                }
                else
                {
                    v.VisitUnknown(f, t);
                }
            });
        if (!EqualityComparer<IonosphericDelayItem[]>.Default.Equals(valueDelays, Delays))
        {
            Delays = valueDelays;
        }
    }
}
