using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RinexObservationEpochMessage
{
    private static readonly Field NrEpochField = NmeaRinexVisitor.Field(nameof(Epoch), RinexEpoch.NrVisitorType);
    private static readonly Field NrEpochFlagField = NmeaRinexVisitor.Field(nameof(EpochFlag), Int32Type.Default);
    private static readonly Field NrReceiverClockOffsetField = NmeaRinexVisitor.Field(nameof(ReceiverClockOffset), DoubleOptionalType.Default);
    private static readonly Field NrSatellitesField = NmeaRinexVisitor.Field(nameof(Satellites), new ListType(RinexSatelliteObservation.NrVisitorType, 0, 32768), readOnly: true);
    private static readonly Field NrSpecialRecordsField = NmeaRinexVisitor.Field(nameof(SpecialRecords), new ListType(StringOptionalType.Utf8, 0, 32768), readOnly: true);

    internal new static readonly StructType NrVisitorType = new([..RinexMessageBase.NrVisitorType.Fields, NrEpochField, NrEpochFlagField, NrReceiverClockOffsetField, NrSatellitesField, NrSpecialRecordsField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueEpoch = Epoch;
        StructType.Accept(visitor, NrEpochField, NrEpochField.DataType, valueEpoch);

        var valueEpochFlag = EpochFlag;
        Int32Type.Accept(visitor, NrEpochFlagField, NrEpochFlagField.DataType, ref valueEpochFlag);
        if (!EqualityComparer<int>.Default.Equals(valueEpochFlag, EpochFlag))
        {
            EpochFlag = valueEpochFlag;
        }

        var valueReceiverClockOffset = ReceiverClockOffset;
        DoubleOptionalType.Accept(visitor, NrReceiverClockOffsetField, NrReceiverClockOffsetField.DataType, ref valueReceiverClockOffset);
        if (!EqualityComparer<double?>.Default.Equals(valueReceiverClockOffset, ReceiverClockOffset))
        {
            ReceiverClockOffset = valueReceiverClockOffset;
        }

        var valueSatellites = Satellites;
        NmeaRinexVisitor.VisitList(visitor, NrSatellitesField, NrSatellitesField.DataType, valueSatellites,
            static (IVisitor visitor, Field field, IFieldType type, ref RinexSatelliteObservation item) =>
            {
                if (item != null) StructType.Accept(visitor, field, type, item);
                else visitor.VisitUnknown(field, type);
            });

        var valueSpecialRecords = SpecialRecords;
        NmeaRinexVisitor.VisitList(visitor, NrSpecialRecordsField, NrSpecialRecordsField.DataType, valueSpecialRecords,
            static (IVisitor visitor, Field field, IFieldType type, ref string item) =>
            {
                string? value = item;
                        StringOptionalType.Accept(visitor, field, type, ref value);
                        item = value!;
            });
    }
}
