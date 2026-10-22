using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV2Message1
{
    private static readonly Field PayloadObservationItemsField = ProtocolPayloadVisitor.Field(nameof(ObservationItems), new ListType(DObservationItem.PayloadVisitorType, 0, int.MaxValue));

    internal new static readonly StructType PayloadVisitorType = new([..RtcmV2MessageBase.PayloadVisitorType.Fields, PayloadObservationItemsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        DObservationItem[] valueObservationItems = ObservationItems;
        ProtocolPayloadVisitor.VisitArray(visitor, PayloadObservationItemsField, PayloadObservationItemsField.DataType, ref valueObservationItems,
            static () => new DObservationItem(NavigationSystemEnum.SYS_GPS),
            static (IVisitor v, Field f, IFieldType t, ref DObservationItem item) =>
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
        if (!EqualityComparer<DObservationItem[]>.Default.Equals(valueObservationItems, ObservationItems))
        {
            ObservationItems = valueObservationItems;
        }
    }
}
