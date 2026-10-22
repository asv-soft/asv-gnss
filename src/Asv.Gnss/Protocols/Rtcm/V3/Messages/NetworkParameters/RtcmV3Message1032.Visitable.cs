using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class RtcmV3Message1032
{
    private static readonly Field RtcmXField = RtcmV3Visitor.Field(nameof(X), DoubleType.Default, "Antenna Reference Point ECEF-X");
    private static readonly Field RtcmYField = RtcmV3Visitor.Field(nameof(Y), DoubleType.Default, "Antenna Reference Point ECEF-Y");
    private static readonly Field RtcmZField = RtcmV3Visitor.Field(nameof(Z), DoubleType.Default, "Antenna Reference Point ECEF-Z");
    private static readonly Field RtcmLatitudeField = RtcmV3Visitor.Field(nameof(Latitude), DoubleType.Default, "Antenna Reference Point WGS84-Latitude");
    private static readonly Field RtcmLongitudeField = RtcmV3Visitor.Field(nameof(Longitude), DoubleType.Default, "Antenna Reference Point WGS84-Longitude");
    private static readonly Field RtcmAltitudeField = RtcmV3Visitor.Field(nameof(Altitude), DoubleType.Default, "Antenna Reference Point WGS84-Altitude");
    private static readonly Field RtcmITRFEpochYearField = RtcmV3Visitor.Field(nameof(ITRFEpochYear), UInt8Type.Default, "Since this field is reserved, all bits should be set to zero for now. However, since the value is subject to change in future versions, decoding should not rely on a zero value. The ITRF realization year identifies the datum definition used for coordinates in the message.");
    private static readonly Field RtcmNonPhysicalReferenceStationIDField = RtcmV3Visitor.Field(nameof(NonPhysicalReferenceStationID), UInt32Type.Default, "The Reference Station ID is determined by the service provider. Its primary purpose is to link all message data to their unique sourceName. It is useful in distinguishing between desired and undesired data in cases where more than one service may be using the same data link frequency. It is also useful in accommodating multiple reference stations within a single data link transmission. In reference network applications the Reference Station ID plays an important role, because it is the link between the observation messages of a specific reference station and its auxiliary information contained in other messages for proper operation. Thus the Service Provider should ensure that the Reference Station ID is unique within the whole network, and that ID’s should be reassigned only when absolutely necessary. Service Providers may need to coordinate their Reference Station ID assignments with other Service Providers in their region in order to avoid conflicts. This may be especially critical for equipment accessing multiple services, depending on their services and means of information distribution.");
    private static readonly Field RtcmPhysicalReferenceStationIDField = RtcmV3Visitor.Field(nameof(PhysicalReferenceStationID), UInt32Type.Default, "The Physical Reference Station ID specifies the station ID of a real reference station, when the data stream itself is based on a nonphysical reference station. Consequently, for the Physical Reference Station ID the same notes apply as for DF003.");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmXField, RtcmYField, RtcmZField, RtcmLatitudeField, RtcmLongitudeField, RtcmAltitudeField, RtcmITRFEpochYearField, RtcmNonPhysicalReferenceStationIDField, RtcmPhysicalReferenceStationIDField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

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

        var valueITRFEpochYear = ITRFEpochYear;
        UInt8Type.Accept(visitor, RtcmITRFEpochYearField, RtcmITRFEpochYearField.DataType, ref valueITRFEpochYear);
        if (!EqualityComparer<byte>.Default.Equals(valueITRFEpochYear, ITRFEpochYear))
        {
            ITRFEpochYear = valueITRFEpochYear;
        }

        var valueNonPhysicalReferenceStationID = NonPhysicalReferenceStationID;
        UInt32Type.Accept(visitor, RtcmNonPhysicalReferenceStationIDField, RtcmNonPhysicalReferenceStationIDField.DataType, ref valueNonPhysicalReferenceStationID);
        if (!EqualityComparer<uint>.Default.Equals(valueNonPhysicalReferenceStationID, NonPhysicalReferenceStationID))
        {
            NonPhysicalReferenceStationID = valueNonPhysicalReferenceStationID;
        }

        var valuePhysicalReferenceStationID = PhysicalReferenceStationID;
        UInt32Type.Accept(visitor, RtcmPhysicalReferenceStationIDField, RtcmPhysicalReferenceStationIDField.DataType, ref valuePhysicalReferenceStationID);
        if (!EqualityComparer<uint>.Default.Equals(valuePhysicalReferenceStationID, PhysicalReferenceStationID))
        {
            PhysicalReferenceStationID = valuePhysicalReferenceStationID;
        }
    }
}
