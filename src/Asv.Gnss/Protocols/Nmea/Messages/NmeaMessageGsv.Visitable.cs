using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class NmeaMessageGsv
{
    private static readonly Field NrSatellitesField = NmeaRinexVisitor.Field(nameof(Satellites), new ListType(SatelliteInfo.NrVisitorType, 0, 32768), readOnly: true);
    private static readonly Field NrTotalMessagesField = NmeaRinexVisitor.Field(nameof(TotalMessages), Int32OptionalType.Default);
    private static readonly Field NrMessageNumberField = NmeaRinexVisitor.Field(nameof(MessageNumber), Int32OptionalType.Default);
    private static readonly Field NrSatellitesInViewField = NmeaRinexVisitor.Field(nameof(SatellitesInView), Int32OptionalType.Default);
    private static readonly Field NrSystemIdField = NmeaRinexVisitor.Field(nameof(SystemId), Int32OptionalType.Default);

    internal new static readonly StructType NrVisitorType = new([..NmeaMessageBase.NrVisitorType.Fields, NrSatellitesField, NrTotalMessagesField, NrMessageNumberField, NrSatellitesInViewField, NrSystemIdField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueSatellites = Satellites;
        NmeaRinexVisitor.VisitList(visitor, NrSatellitesField, NrSatellitesField.DataType, valueSatellites,
            static (IVisitor visitor, Field field, IFieldType type, ref SatelliteInfo item) =>
            {
                if (item != null) StructType.Accept(visitor, field, type, item);
                else visitor.VisitUnknown(field, type);
            });

        var valueTotalMessages = TotalMessages;
        Int32OptionalType.Accept(visitor, NrTotalMessagesField, NrTotalMessagesField.DataType, ref valueTotalMessages);
        if (!EqualityComparer<int?>.Default.Equals(valueTotalMessages, TotalMessages))
        {
            TotalMessages = valueTotalMessages;
        }

        var valueMessageNumber = MessageNumber;
        Int32OptionalType.Accept(visitor, NrMessageNumberField, NrMessageNumberField.DataType, ref valueMessageNumber);
        if (!EqualityComparer<int?>.Default.Equals(valueMessageNumber, MessageNumber))
        {
            MessageNumber = valueMessageNumber;
        }

        var valueSatellitesInView = SatellitesInView;
        Int32OptionalType.Accept(visitor, NrSatellitesInViewField, NrSatellitesInViewField.DataType, ref valueSatellitesInView);
        if (!EqualityComparer<int?>.Default.Equals(valueSatellitesInView, SatellitesInView))
        {
            SatellitesInView = valueSatellitesInView;
        }

        var valueSystemId = SystemId;
        Int32OptionalType.Accept(visitor, NrSystemIdField, NrSystemIdField.DataType, ref valueSystemId);
        if (!EqualityComparer<int?>.Default.Equals(valueSystemId, SystemId))
        {
            SystemId = valueSystemId;
        }
    }
}
