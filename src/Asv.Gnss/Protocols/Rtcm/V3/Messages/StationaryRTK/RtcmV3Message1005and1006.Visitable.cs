using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class RtcmV3Message1005and1006
{
    private static readonly Field RtcmGpsIndicatorField = RtcmV3Visitor.Field(nameof(GpsIndicator), BoolType.Default, "GpsIndicator");
    private static readonly Field RtcmGlonassIndicatorField = RtcmV3Visitor.Field(nameof(GlonassIndicator), BoolType.Default, "GlonassIndicator");
    private static readonly Field RtcmGalileoIndicatorField = RtcmV3Visitor.Field(nameof(GalileoIndicator), BoolType.Default, "GalileoIndicator");
    private static readonly Field RtcmRefStationIndicatorField = RtcmV3Visitor.Field(nameof(RefStationIndicator), BoolType.Default, "RefStationIndicator");
    private static readonly Field RtcmQuarterCycleIndicatorField = RtcmV3Visitor.Field(nameof(QuarterCycleIndicator), UInt8Type.Default, "QuarterCycleIndicator");
    private static readonly Field RtcmSingleReceiverOscillatorIndicatorField = RtcmV3Visitor.Field(nameof(SingleReceiverOscillatorIndicator), BoolType.Default, "False - All raw data observations in messages 1001-1004 and 1009-1012 may be measured at different instants.This indicator should be set to “0” unless all the conditions for “1” are clearly met. True - All raw data observations in messages 1001-1004 and 1009-1012 are measured at the same instant, as described in Section 3.1.4.");
    private static readonly Field RtcmHeightField = RtcmV3Visitor.Field(nameof(Height), DoubleType.Default, "Antenna Height");
    private static readonly Field RtcmXField = RtcmV3Visitor.Field(nameof(X), DoubleType.Default, "Antenna Reference Point ECEF-X");
    private static readonly Field RtcmYField = RtcmV3Visitor.Field(nameof(Y), DoubleType.Default, "Antenna Reference Point ECEF-Y");
    private static readonly Field RtcmZField = RtcmV3Visitor.Field(nameof(Z), DoubleType.Default, "Antenna Reference Point ECEF-Z");
    private static readonly Field RtcmLatitudeField = RtcmV3Visitor.Field(nameof(Latitude), DoubleType.Default, "Antenna Reference Point WGS84-Latitude");
    private static readonly Field RtcmLongitudeField = RtcmV3Visitor.Field(nameof(Longitude), DoubleType.Default, "Antenna Reference Point WGS84-Longitude");
    private static readonly Field RtcmAltitudeField = RtcmV3Visitor.Field(nameof(Altitude), DoubleType.Default, "Antenna Reference Point WGS84-Altitude");
    private static readonly Field RtcmITRFField = RtcmV3Visitor.Field(nameof(ITRF), UInt8Type.Default, "Since this field is reserved, all bits should be set to zero for now. However, since the value is subject to change in future versions, decoding should not rely on a zero value. The ITRF realization year identifies the datum definition used for coordinates in the message.");
    private static readonly Field RtcmReferenceStationIDField = RtcmV3Visitor.Field(nameof(ReferenceStationID), UInt32Type.Default, "The Reference Station ID is determined by the service provider. Its primary purpose is to link all message data to their unique sourceName. It is useful in distinguishing between desired and undesired data in cases where more than one service may be using the same data link frequency. It is also useful in accommodating multiple reference stations within a single data link transmission. In reference network applications the Reference Station ID plays an important role, because it is the link between the observation messages of a specific reference station and its auxiliary information contained in other messages for proper operation. Thus the Service Provider should ensure that the Reference Station ID is unique within the whole network, and that ID’s should be reassigned only when absolutely necessary. Service Providers may need to coordinate their Reference Station ID assignments with other Service Providers in their region in order to avoid conflicts. This may be especially critical for equipment accessing multiple services, depending on their services and means of information distribution.");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmGpsIndicatorField, RtcmGlonassIndicatorField, RtcmGalileoIndicatorField, RtcmRefStationIndicatorField, RtcmQuarterCycleIndicatorField, RtcmSingleReceiverOscillatorIndicatorField, RtcmHeightField, RtcmXField, RtcmYField, RtcmZField, RtcmLatitudeField, RtcmLongitudeField, RtcmAltitudeField, RtcmITRFField, RtcmReferenceStationIDField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueGpsIndicator = GpsIndicator;
        BoolType.Accept(visitor, RtcmGpsIndicatorField, RtcmGpsIndicatorField.DataType, ref valueGpsIndicator);
        if (!EqualityComparer<bool>.Default.Equals(valueGpsIndicator, GpsIndicator))
        {
            GpsIndicator = valueGpsIndicator;
        }

        var valueGlonassIndicator = GlonassIndicator;
        BoolType.Accept(visitor, RtcmGlonassIndicatorField, RtcmGlonassIndicatorField.DataType, ref valueGlonassIndicator);
        if (!EqualityComparer<bool>.Default.Equals(valueGlonassIndicator, GlonassIndicator))
        {
            GlonassIndicator = valueGlonassIndicator;
        }

        var valueGalileoIndicator = GalileoIndicator;
        BoolType.Accept(visitor, RtcmGalileoIndicatorField, RtcmGalileoIndicatorField.DataType, ref valueGalileoIndicator);
        if (!EqualityComparer<bool>.Default.Equals(valueGalileoIndicator, GalileoIndicator))
        {
            GalileoIndicator = valueGalileoIndicator;
        }

        var valueRefStationIndicator = RefStationIndicator;
        BoolType.Accept(visitor, RtcmRefStationIndicatorField, RtcmRefStationIndicatorField.DataType, ref valueRefStationIndicator);
        if (!EqualityComparer<bool>.Default.Equals(valueRefStationIndicator, RefStationIndicator))
        {
            RefStationIndicator = valueRefStationIndicator;
        }

        var valueQuarterCycleIndicator = QuarterCycleIndicator;
        UInt8Type.Accept(visitor, RtcmQuarterCycleIndicatorField, RtcmQuarterCycleIndicatorField.DataType, ref valueQuarterCycleIndicator);
        if (!EqualityComparer<byte>.Default.Equals(valueQuarterCycleIndicator, QuarterCycleIndicator))
        {
            QuarterCycleIndicator = valueQuarterCycleIndicator;
        }

        var valueSingleReceiverOscillatorIndicator = SingleReceiverOscillatorIndicator;
        BoolType.Accept(visitor, RtcmSingleReceiverOscillatorIndicatorField, RtcmSingleReceiverOscillatorIndicatorField.DataType, ref valueSingleReceiverOscillatorIndicator);
        if (!EqualityComparer<bool>.Default.Equals(valueSingleReceiverOscillatorIndicator, SingleReceiverOscillatorIndicator))
        {
            SingleReceiverOscillatorIndicator = valueSingleReceiverOscillatorIndicator;
        }

        var valueHeight = Height;
        DoubleType.Accept(visitor, RtcmHeightField, RtcmHeightField.DataType, ref valueHeight);
        if (!EqualityComparer<double>.Default.Equals(valueHeight, Height))
        {
            Height = valueHeight;
        }

        var valueX = X;
        DoubleType.Accept(visitor, RtcmXField, RtcmXField.DataType, ref valueX);
        if (!EqualityComparer<double>.Default.Equals(valueX, X))
        {
            X = valueX;
        }

        var valueY = Y;
        DoubleType.Accept(visitor, RtcmYField, RtcmYField.DataType, ref valueY);
        if (!EqualityComparer<double>.Default.Equals(valueY, Y))
        {
            Y = valueY;
        }

        var valueZ = Z;
        DoubleType.Accept(visitor, RtcmZField, RtcmZField.DataType, ref valueZ);
        if (!EqualityComparer<double>.Default.Equals(valueZ, Z))
        {
            Z = valueZ;
        }

        var valueLatitude = Latitude;
        DoubleType.Accept(visitor, RtcmLatitudeField, RtcmLatitudeField.DataType, ref valueLatitude);
        if (!EqualityComparer<double>.Default.Equals(valueLatitude, Latitude))
        {
            Latitude = valueLatitude;
        }

        var valueLongitude = Longitude;
        DoubleType.Accept(visitor, RtcmLongitudeField, RtcmLongitudeField.DataType, ref valueLongitude);
        if (!EqualityComparer<double>.Default.Equals(valueLongitude, Longitude))
        {
            Longitude = valueLongitude;
        }

        var valueAltitude = Altitude;
        DoubleType.Accept(visitor, RtcmAltitudeField, RtcmAltitudeField.DataType, ref valueAltitude);
        if (!EqualityComparer<double>.Default.Equals(valueAltitude, Altitude))
        {
            Altitude = valueAltitude;
        }

        var valueITRF = ITRF;
        UInt8Type.Accept(visitor, RtcmITRFField, RtcmITRFField.DataType, ref valueITRF);
        if (!EqualityComparer<byte>.Default.Equals(valueITRF, ITRF))
        {
            ITRF = valueITRF;
        }

        var valueReferenceStationID = ReferenceStationID;
        UInt32Type.Accept(visitor, RtcmReferenceStationIDField, RtcmReferenceStationIDField.DataType, ref valueReferenceStationID);
        if (!EqualityComparer<uint>.Default.Equals(valueReferenceStationID, ReferenceStationID))
        {
            ReferenceStationID = valueReferenceStationID;
        }
    }
}
