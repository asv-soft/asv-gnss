using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class NmeaMessageGll
{
    private static readonly Field NrLatitudeField = NmeaRinexVisitor.Field(nameof(Latitude), DoubleType.Default);
    private static readonly Field NrLongitudeField = NmeaRinexVisitor.Field(nameof(Longitude), DoubleType.Default);
    private static readonly Field NrTimeField = NmeaRinexVisitor.Field(nameof(Time), TimeOnlyOptionalType.Default);
    private static readonly Field NrStatusField = NmeaRinexVisitor.Field(nameof(Status), Int32OptionalType.Default, sourceType: typeof(NmeaDataStatus));
    private static readonly Field NrPositioningModeField = NmeaRinexVisitor.Field(nameof(PositioningMode), Int32OptionalType.Default, sourceType: typeof(NmeaPositioningSystemMode));

    internal new static readonly StructType NrVisitorType = new([..NmeaMessageBase.NrVisitorType.Fields, NrLatitudeField, NrLongitudeField, NrTimeField, NrStatusField, NrPositioningModeField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueLatitude = Latitude;
        DoubleType.Accept(visitor, NrLatitudeField, NrLatitudeField.DataType, ref valueLatitude);
        if (!EqualityComparer<double>.Default.Equals(valueLatitude, Latitude))
        {
            Latitude = valueLatitude;
        }

        var valueLongitude = Longitude;
        DoubleType.Accept(visitor, NrLongitudeField, NrLongitudeField.DataType, ref valueLongitude);
        if (!EqualityComparer<double>.Default.Equals(valueLongitude, Longitude))
        {
            Longitude = valueLongitude;
        }

        var valueTime = Time;
        TimeOnlyOptionalType.Accept(visitor, NrTimeField, NrTimeField.DataType, ref valueTime);
        if (!EqualityComparer<TimeOnly?>.Default.Equals(valueTime, Time))
        {
            Time = valueTime;
        }

        var valueStatus = (int?)Status;
        Int32OptionalType.Accept(visitor, NrStatusField, NrStatusField.DataType, ref valueStatus);
        if (!EqualityComparer<int?>.Default.Equals(valueStatus, (int?)Status))
        {
            Status = (NmeaDataStatus?)valueStatus;
        }

        var valuePositioningMode = (int?)PositioningMode;
        Int32OptionalType.Accept(visitor, NrPositioningModeField, NrPositioningModeField.DataType, ref valuePositioningMode);
        if (!EqualityComparer<int?>.Default.Equals(valuePositioningMode, (int?)PositioningMode))
        {
            PositioningMode = (NmeaPositioningSystemMode?)valuePositioningMode;
        }
    }
}
