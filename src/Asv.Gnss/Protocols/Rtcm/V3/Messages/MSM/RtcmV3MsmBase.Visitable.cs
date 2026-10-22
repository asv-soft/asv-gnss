using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class RtcmV3MsmBase
{
    private static readonly Field RtcmNavigationSystemField = RtcmV3Visitor.Field(nameof(NavigationSystem), Int32Type.Default, "NavigationSystem", enumType: typeof(NavigationSystemEnum));
    private static readonly Field RtcmObservableDataIsCompleteField = RtcmV3Visitor.Field(nameof(ObservableDataIsComplete), BoolType.Default, "Observable data complete flag (1:ok, 0:not complete)");
    private static readonly Field RtcmEpochTimeField = RtcmV3Visitor.Field(nameof(EpochTime), DateTimeType.Default, "EpochTime");
    private static readonly Field RtcmCellMaskField = RtcmV3Visitor.Field(nameof(CellMask), new ListType(new ListType(UInt8Type.Default, 0, RtcmV3Visitor.MaximumElements), 0, RtcmV3Visitor.MaximumElements), "CellMask");
    private static readonly Field RtcmSatelliteIdsField = RtcmV3Visitor.Field(nameof(SatelliteIds), new ListType(UInt8Type.Default, 0, RtcmV3Visitor.MaximumElements), "SatelliteIds");
    private static readonly Field RtcmSignalIdsField = RtcmV3Visitor.Field(nameof(SignalIds), new ListType(UInt8Type.Default, 0, RtcmV3Visitor.MaximumElements), "SignalIds");
    private static readonly Field RtcmReferenceStationIdField = RtcmV3Visitor.Field(nameof(ReferenceStationId), UInt32Type.Default, "The Reference Station ID is determined by the service provider. Its primary purpose is to link all message data to their unique sourceName. It is useful in distinguishing between desired and undesired data in cases where more than one service may be using the same data link frequency. It is also useful in accommodating multiple reference stations within a single data link transmission. In reference network applications the Reference Station ID plays an important role, because it is the link between the observation messages of a specific reference station and its auxiliary information contained in other messages for proper operation. Thus the Service Provider should ensure that the Reference Station ID is unique within the whole network, and that ID’s should be reassigned only when absolutely necessary. Service Providers may need to coordinate their Reference Station ID assignments with other Service Providers in their region in order to avoid conflicts. This may be especially critical for equipment accessing multiple services, depending on their services and means of information distribution.");
    private static readonly Field RtcmEpochTimeTowField = RtcmV3Visitor.Field(nameof(EpochTimeTow), UInt32Type.Default, "GNSS Epoch Time, specific for each GNSS GPS: Full seconds since the beginning of the GPS week GLONASS: Full seconds since the beginning of GLONASS day");
    private static readonly Field RtcmMultipleMessageBitField = RtcmV3Visitor.Field(nameof(MultipleMessageBit), UInt8Type.Default, "1 - indicates that more MSMs follow for given physical time and reference station ID. 0 - indicates that it is the last MSM for given physical time and reference station ID");
    private static readonly Field RtcmIodsField = RtcmV3Visitor.Field(nameof(Iods), UInt8Type.Default, "Issue of Data Station. This field is reserved to be used to link MSM with future site- description (receiver, antenna description, etc.) messages. A value of “0” indicates that this field is not utilized.");
    private static readonly Field RtcmSessionTimeField = RtcmV3Visitor.Field(nameof(SessionTime), UInt8Type.Default, "Cumulative session transmitting time");
    private static readonly Field RtcmClockSteeringIndicatorField = RtcmV3Visitor.Field(nameof(ClockSteeringIndicator), UInt32Type.Default, "0 – clock steering is not applied In this case receiver clock must be kept in the range of ± 1 ms (approximately ± 300 km). 1 – clock steering has been applied In this case receiver clock must be kept in the range of ± 1 microsecond (approximately ± 300 meters). 2 – unknown clock steering status. 3 – reserved");
    private static readonly Field RtcmExternalClockIndicatorField = RtcmV3Visitor.Field(nameof(ExternalClockIndicator), UInt32Type.Default, "0 – internal clock is used. 1 – external clock is used, clock status is “locked”. 2 – external clock is used, clock status is “not locked”, which may indicate external clock failure and that the transmitted data may not be reliable. 3 – unknown clock is used");
    private static readonly Field RtcmSmoothingIndicatorField = RtcmV3Visitor.Field(nameof(SmoothingIndicator), UInt32Type.Default, "GNSS Smoothing Type Indicator: 1 – Divergence-free smoothing is used. 0 – Other type of smoothing is used");
    private static readonly Field RtcmSmoothingIntervalField = RtcmV3Visitor.Field(nameof(SmoothingInterval), UInt32Type.Default, "The GNSS Smoothing Interval is the integration period over which the pseudorange code phase measurements are averaged using carrier phase information. Divergence-free smoothing may be continuous over the entire period for which the satellite is visible. Notice: A value of zero indicates no smoothing is used.");

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmNavigationSystemField, RtcmObservableDataIsCompleteField, RtcmEpochTimeField, RtcmCellMaskField, RtcmSatelliteIdsField, RtcmSignalIdsField, RtcmReferenceStationIdField, RtcmEpochTimeTowField, RtcmMultipleMessageBitField, RtcmIodsField, RtcmSessionTimeField, RtcmClockSteeringIndicatorField, RtcmExternalClockIndicatorField, RtcmSmoothingIndicatorField, RtcmSmoothingIntervalField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueNavigationSystem = (int)NavigationSystem;
        Int32Type.Accept(visitor, RtcmNavigationSystemField, RtcmNavigationSystemField.DataType, ref valueNavigationSystem);
        if (!EqualityComparer<int>.Default.Equals(valueNavigationSystem, (int)NavigationSystem))
        {
            NavigationSystem = (NavigationSystemEnum)valueNavigationSystem;
        }

        var valueObservableDataIsComplete = ObservableDataIsComplete;
        BoolType.Accept(visitor, RtcmObservableDataIsCompleteField, RtcmObservableDataIsCompleteField.DataType, ref valueObservableDataIsComplete);
        if (!EqualityComparer<bool>.Default.Equals(valueObservableDataIsComplete, ObservableDataIsComplete))
        {
            ObservableDataIsComplete = valueObservableDataIsComplete;
        }

        var valueEpochTime = EpochTime;
        DateTimeType.Accept(visitor, RtcmEpochTimeField, RtcmEpochTimeField.DataType, ref valueEpochTime);
        if (!EqualityComparer<DateTime>.Default.Equals(valueEpochTime, EpochTime))
        {
            EpochTime = valueEpochTime;
        }

        var valueCellMask = CellMask;
        RtcmV3Visitor.VisitArray(visitor, RtcmCellMaskField, RtcmCellMaskField.DataType, ref valueCellMask,
            static () => Array.Empty<byte>(),
            static (IVisitor v, Field f, IFieldType t, ref byte[] item) =>
            {
                byte[]? row = item;
                RtcmV3Visitor.VisitArray(v, f, t, ref row,
                    static () => default(byte),
                    static (IVisitor v2, Field f2, IFieldType t2, ref byte item2) =>
                    {
                        UInt8Type.Accept(v2, f2, t2, ref item2);
                    });
                item = row!;
            });
        if (!ReferenceEquals(valueCellMask, CellMask))
        {
            CellMask = valueCellMask!;
        }

        var valueSatelliteIds = SatelliteIds;
        RtcmV3Visitor.VisitArray(visitor, RtcmSatelliteIdsField, RtcmSatelliteIdsField.DataType, ref valueSatelliteIds,
            static () => default(byte),
            static (IVisitor v, Field f, IFieldType t, ref byte item) =>
            {
                UInt8Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueSatelliteIds, SatelliteIds))
        {
            SatelliteIds = valueSatelliteIds!;
        }

        var valueSignalIds = SignalIds;
        RtcmV3Visitor.VisitArray(visitor, RtcmSignalIdsField, RtcmSignalIdsField.DataType, ref valueSignalIds,
            static () => default(byte),
            static (IVisitor v, Field f, IFieldType t, ref byte item) =>
            {
                UInt8Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valueSignalIds, SignalIds))
        {
            SignalIds = valueSignalIds!;
        }

        var valueReferenceStationId = ReferenceStationId;
        UInt32Type.Accept(visitor, RtcmReferenceStationIdField, RtcmReferenceStationIdField.DataType, ref valueReferenceStationId);
        if (!EqualityComparer<uint>.Default.Equals(valueReferenceStationId, ReferenceStationId))
        {
            ReferenceStationId = valueReferenceStationId;
        }

        var valueEpochTimeTow = EpochTimeTow;
        UInt32Type.Accept(visitor, RtcmEpochTimeTowField, RtcmEpochTimeTowField.DataType, ref valueEpochTimeTow);
        if (!EqualityComparer<uint>.Default.Equals(valueEpochTimeTow, EpochTimeTow))
        {
            EpochTimeTow = valueEpochTimeTow;
        }

        var valueMultipleMessageBit = MultipleMessageBit;
        UInt8Type.Accept(visitor, RtcmMultipleMessageBitField, RtcmMultipleMessageBitField.DataType, ref valueMultipleMessageBit);
        if (!EqualityComparer<byte>.Default.Equals(valueMultipleMessageBit, MultipleMessageBit))
        {
            MultipleMessageBit = valueMultipleMessageBit;
        }

        var valueIods = Iods;
        UInt8Type.Accept(visitor, RtcmIodsField, RtcmIodsField.DataType, ref valueIods);
        if (!EqualityComparer<byte>.Default.Equals(valueIods, Iods))
        {
            Iods = valueIods;
        }

        var valueSessionTime = SessionTime;
        UInt8Type.Accept(visitor, RtcmSessionTimeField, RtcmSessionTimeField.DataType, ref valueSessionTime);
        if (!EqualityComparer<byte>.Default.Equals(valueSessionTime, SessionTime))
        {
            SessionTime = valueSessionTime;
        }

        var valueClockSteeringIndicator = ClockSteeringIndicator;
        UInt32Type.Accept(visitor, RtcmClockSteeringIndicatorField, RtcmClockSteeringIndicatorField.DataType, ref valueClockSteeringIndicator);
        if (!EqualityComparer<uint>.Default.Equals(valueClockSteeringIndicator, ClockSteeringIndicator))
        {
            ClockSteeringIndicator = valueClockSteeringIndicator;
        }

        var valueExternalClockIndicator = ExternalClockIndicator;
        UInt32Type.Accept(visitor, RtcmExternalClockIndicatorField, RtcmExternalClockIndicatorField.DataType, ref valueExternalClockIndicator);
        if (!EqualityComparer<uint>.Default.Equals(valueExternalClockIndicator, ExternalClockIndicator))
        {
            ExternalClockIndicator = valueExternalClockIndicator;
        }

        var valueSmoothingIndicator = SmoothingIndicator;
        UInt32Type.Accept(visitor, RtcmSmoothingIndicatorField, RtcmSmoothingIndicatorField.DataType, ref valueSmoothingIndicator);
        if (!EqualityComparer<uint>.Default.Equals(valueSmoothingIndicator, SmoothingIndicator))
        {
            SmoothingIndicator = valueSmoothingIndicator;
        }

        var valueSmoothingInterval = SmoothingInterval;
        UInt32Type.Accept(visitor, RtcmSmoothingIntervalField, RtcmSmoothingIntervalField.DataType, ref valueSmoothingInterval);
        if (!EqualityComparer<uint>.Default.Equals(valueSmoothingInterval, SmoothingInterval))
        {
            SmoothingInterval = valueSmoothingInterval;
        }
    }
}
