using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class RtcmV3RTKObservableMessagesBase
{
    private static readonly Field RtcmReferenceStationIDField = RtcmV3Visitor.Field(nameof(ReferenceStationID), UInt32Type.Default, "The Reference Station ID is determined by the service provider. Its primary purpose is to link all message data to their unique sourceName. It is useful in distinguishing between desired and undesired data in cases where more than one service may be using the same data link frequency. It is also useful in accommodating multiple reference stations within a single data link transmission. In reference network applications the Reference Station ID plays an important role, because it is the link between the observation messages of a specific reference station and its auxiliary information contained in other messages for proper operation. Thus the Service Provider should ensure that the Reference Station ID is unique within the whole network, and that ID’s should be reassigned only when absolutely necessary. Service Providers may need to coordinate their Reference Station ID assignments with other Service Providers in their region in order to avoid conflicts. This may be especially critical for equipment accessing multiple services, depending on their services and means of information distribution.");
    private static readonly Field RtcmEpochTimeField = RtcmV3Visitor.Field(nameof(EpochTime), DateTimeType.Default, "GPS Epoch Time is provided in milliseconds from the beginning of the GPS week, which begins at midnight GMT on Saturday night/Sunday morning, measured in GPS time (as opposed to UTC). GLONASS Epoch Time of measurement is defined by the GLONASS ICD as UTC(SU) + 3.0 hours. It rolls over at 86,400 seconds for GLONASS, except for the leap second, where it rolls over at 86,401.");
    private static readonly Field RtcmSynchronousGNSSFlagField = RtcmV3Visitor.Field(nameof(SynchronousGNSSFlag), UInt8Type.Default, "0 - No further GNSS observables referenced to the same Epoch Time will be transmitted. This enables the receiver to begin processing the data immediately after decoding the message. 1 - The next message will contain observables of another GNSS sourceName referenced to the same Epoch Time. Note: “Synchronous\" here means that the measurements are taken within one microsecond of each other");
    private static readonly Field RtcmSatelliteCountField = RtcmV3Visitor.Field(nameof(SatelliteCount), UInt8Type.Default, "The Number of GPS/GLONASS Satellite Signals Processed refers to the number of satellites in the message. It does not necessarily equal the number of satellites visible to the Reference Station.");
    private static readonly Field RtcmSmoothingIndicatorField = RtcmV3Visitor.Field(nameof(SmoothingIndicator), UInt8Type.Default, "0 - Divergence-free smoothing not used 1 - Divergence-free smoothing used");
    private static readonly Field RtcmSmoothingIntervalField = RtcmV3Visitor.Field(nameof(SmoothingInterval), UInt8Type.Default, "The GPS/GLONASS Smoothing Interval is the integration period over which reference station pseudorange code phase measurements are averaged using carrier phase information. Divergence-free smoothing may be continuous over the entire period the satellite is visible.");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmReferenceStationIDField, RtcmEpochTimeField, RtcmSynchronousGNSSFlagField, RtcmSatelliteCountField, RtcmSmoothingIndicatorField, RtcmSmoothingIntervalField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueReferenceStationID = ReferenceStationID;
        UInt32Type.Accept(visitor, RtcmReferenceStationIDField, RtcmReferenceStationIDField.DataType, ref valueReferenceStationID);
        if (!EqualityComparer<uint>.Default.Equals(valueReferenceStationID, ReferenceStationID))
        {
            ReferenceStationID = valueReferenceStationID;
        }

        var valueEpochTime = EpochTime;
        DateTimeType.Accept(visitor, RtcmEpochTimeField, RtcmEpochTimeField.DataType, ref valueEpochTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueEpochTime, EpochTime))
        {
            EpochTime = valueEpochTime;
        }

        var valueSynchronousGNSSFlag = SynchronousGNSSFlag;
        UInt8Type.Accept(visitor, RtcmSynchronousGNSSFlagField, RtcmSynchronousGNSSFlagField.DataType, ref valueSynchronousGNSSFlag);
        if (!EqualityComparer<byte>.Default.Equals(valueSynchronousGNSSFlag, SynchronousGNSSFlag))
        {
            SynchronousGNSSFlag = valueSynchronousGNSSFlag;
        }

        var valueSatelliteCount = SatelliteCount;
        UInt8Type.Accept(visitor, RtcmSatelliteCountField, RtcmSatelliteCountField.DataType, ref valueSatelliteCount);
        if (!EqualityComparer<byte>.Default.Equals(valueSatelliteCount, SatelliteCount))
        {
            SatelliteCount = valueSatelliteCount;
        }

        var valueSmoothingIndicator = SmoothingIndicator;
        UInt8Type.Accept(visitor, RtcmSmoothingIndicatorField, RtcmSmoothingIndicatorField.DataType, ref valueSmoothingIndicator);
        if (!EqualityComparer<byte>.Default.Equals(valueSmoothingIndicator, SmoothingIndicator))
        {
            SmoothingIndicator = valueSmoothingIndicator;
        }

        var valueSmoothingInterval = SmoothingInterval;
        UInt8Type.Accept(visitor, RtcmSmoothingIntervalField, RtcmSmoothingIntervalField.DataType, ref valueSmoothingInterval);
        if (!EqualityComparer<byte>.Default.Equals(valueSmoothingInterval, SmoothingInterval))
        {
            SmoothingInterval = valueSmoothingInterval;
        }
    }
}
