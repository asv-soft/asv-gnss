using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class NmeaMessageRmc
{
    private static readonly Field NrTimeField = NmeaRinexVisitor.Field(nameof(Time), TimeOnlyOptionalType.Default);
    private static readonly Field NrStatusField = NmeaRinexVisitor.Field(nameof(Status), UInt8OptionalType.Default, sourceType: typeof(NmeaPositionFixStatus));
    private static readonly Field NrLatitudeField = NmeaRinexVisitor.Field(nameof(Latitude), DoubleType.Default);
    private static readonly Field NrLongitudeField = NmeaRinexVisitor.Field(nameof(Longitude), DoubleType.Default);
    private static readonly Field NrSpeedOverGroundKnotsField = NmeaRinexVisitor.Field(nameof(SpeedOverGroundKnots), DoubleType.Default);
    private static readonly Field NrTrackMadeGoodDegreesTrueField = NmeaRinexVisitor.Field(nameof(TrackMadeGoodDegreesTrue), DoubleType.Default);
    private static readonly Field NrDateField = NmeaRinexVisitor.Field(nameof(Date), DateTimeOptionalType.Default);
    private static readonly Field NrMagneticVariationDegreesField = NmeaRinexVisitor.Field(nameof(MagneticVariationDegrees), DoubleType.Default);
    private static readonly Field NrMagneticVariationDirectionField = NmeaRinexVisitor.Field(nameof(MagneticVariationDirection), Int32OptionalType.Default, sourceType: typeof(NmeaMagneticVariationDirection));
    private static readonly Field NrPositioningSystemModeField = NmeaRinexVisitor.Field(nameof(PositioningSystemMode), Int32OptionalType.Default, sourceType: typeof(NmeaPositioningSystemMode));
    private static readonly Field NrNavigationStatusField = NmeaRinexVisitor.Field(nameof(NavigationStatus), UInt8OptionalType.Default, sourceType: typeof(NmeaNavigationStatus));

    internal new static readonly StructType NrVisitorType = new([..NmeaMessageBase.NrVisitorType.Fields, NrTimeField, NrStatusField, NrLatitudeField, NrLongitudeField, NrSpeedOverGroundKnotsField, NrTrackMadeGoodDegreesTrueField, NrDateField, NrMagneticVariationDegreesField, NrMagneticVariationDirectionField, NrPositioningSystemModeField, NrNavigationStatusField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueTime = Time;
        TimeOnlyOptionalType.Accept(visitor, NrTimeField, NrTimeField.DataType, ref valueTime);
        if (!EqualityComparer<TimeOnly?>.Default.Equals(valueTime, Time))
        {
            Time = valueTime;
        }

        var valueStatus = (byte?)Status;
        UInt8OptionalType.Accept(visitor, NrStatusField, NrStatusField.DataType, ref valueStatus);
        if (!EqualityComparer<byte?>.Default.Equals(valueStatus, (byte?)Status))
        {
            Status = (NmeaPositionFixStatus?)valueStatus;
        }

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

        var valueSpeedOverGroundKnots = SpeedOverGroundKnots;
        DoubleType.Accept(visitor, NrSpeedOverGroundKnotsField, NrSpeedOverGroundKnotsField.DataType, ref valueSpeedOverGroundKnots);
        if (!EqualityComparer<double>.Default.Equals(valueSpeedOverGroundKnots, SpeedOverGroundKnots))
        {
            SpeedOverGroundKnots = valueSpeedOverGroundKnots;
        }

        var valueTrackMadeGoodDegreesTrue = TrackMadeGoodDegreesTrue;
        DoubleType.Accept(visitor, NrTrackMadeGoodDegreesTrueField, NrTrackMadeGoodDegreesTrueField.DataType, ref valueTrackMadeGoodDegreesTrue);
        if (!EqualityComparer<double>.Default.Equals(valueTrackMadeGoodDegreesTrue, TrackMadeGoodDegreesTrue))
        {
            TrackMadeGoodDegreesTrue = valueTrackMadeGoodDegreesTrue;
        }

        var valueDate = Date;
        DateTimeOptionalType.Accept(visitor, NrDateField, NrDateField.DataType, ref valueDate);
        if (!EqualityComparer<DateTime?>.Default.Equals(valueDate, Date))
        {
            Date = valueDate;
        }

        var valueMagneticVariationDegrees = MagneticVariationDegrees;
        DoubleType.Accept(visitor, NrMagneticVariationDegreesField, NrMagneticVariationDegreesField.DataType, ref valueMagneticVariationDegrees);
        if (!EqualityComparer<double>.Default.Equals(valueMagneticVariationDegrees, MagneticVariationDegrees))
        {
            MagneticVariationDegrees = valueMagneticVariationDegrees;
        }

        var valueMagneticVariationDirection = (int?)MagneticVariationDirection;
        Int32OptionalType.Accept(visitor, NrMagneticVariationDirectionField, NrMagneticVariationDirectionField.DataType, ref valueMagneticVariationDirection);
        if (!EqualityComparer<int?>.Default.Equals(valueMagneticVariationDirection, (int?)MagneticVariationDirection))
        {
            MagneticVariationDirection = (NmeaMagneticVariationDirection?)valueMagneticVariationDirection;
        }

        var valuePositioningSystemMode = (int?)PositioningSystemMode;
        Int32OptionalType.Accept(visitor, NrPositioningSystemModeField, NrPositioningSystemModeField.DataType, ref valuePositioningSystemMode);
        if (!EqualityComparer<int?>.Default.Equals(valuePositioningSystemMode, (int?)PositioningSystemMode))
        {
            PositioningSystemMode = (NmeaPositioningSystemMode?)valuePositioningSystemMode;
        }

        var valueNavigationStatus = (byte?)NavigationStatus;
        UInt8OptionalType.Accept(visitor, NrNavigationStatusField, NrNavigationStatusField.DataType, ref valueNavigationStatus);
        if (!EqualityComparer<byte?>.Default.Equals(valueNavigationStatus, (byte?)NavigationStatus))
        {
            NavigationStatus = (NmeaNavigationStatus?)valueNavigationStatus;
        }
    }
}
