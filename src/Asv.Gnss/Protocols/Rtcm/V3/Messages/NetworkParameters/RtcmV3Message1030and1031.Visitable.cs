using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public abstract partial class RtcmV3Message1030and1031
{
    private static readonly Field RtcmSatelliteResidualsField = RtcmV3Visitor.Field(nameof(SatelliteResiduals), new ListType(RtcmV3NetworkResidual.RtcmVisitorType, 0, RtcmV3Visitor.MaximumElements), "SatelliteResiduals");
    private static readonly Field RtcmResidualsEpochField = RtcmV3Visitor.Field(nameof(ResidualsEpoch), UInt32Type.Default, "GPS Residuals Epoch Time(TOW) - 0 – 604800 s GLONASS Residuals Epoch Time(tk) - 0 – 86400 s");
    private static readonly Field RtcmNRefsField = RtcmV3Visitor.Field(nameof(NRefs), UInt8Type.Default, "Number of reference stations used to derive residual statistics (1 to 127, use 127 for 127 or more stations). The number of reference stations should never be zero.If zero is encountered the rover should ignore the message.");
    private static readonly Field RtcmNumberSatelliteSignalsField = RtcmV3Visitor.Field(nameof(NumberSatelliteSignals), UInt8Type.Default, "The Number of Satellite Signals Processed refers to the number of satellites in the message.It does not necessarily equal the number of satellites visible to the Reference Station.");
    private static readonly Field RtcmReferenceStationIDField = RtcmV3Visitor.Field(nameof(ReferenceStationID), UInt32Type.Default, "The Reference Station ID is determined by the service provider. Its primary purpose is to link all message data to their unique sourceName. It is useful in distinguishing between desired and undesired data in cases where more than one service may be using the same data link frequency. It is also useful in accommodating multiple reference stations within a single data link transmission. In reference network applications the Reference Station ID plays an important role, because it is the link between the observation messages of a specific reference station and its auxiliary information contained in other messages for proper operation. Thus the Service Provider should ensure that the Reference Station ID is unique within the whole network, and that ID’s should be reassigned only when absolutely necessary. Service Providers may need to coordinate their Reference Station ID assignments with other Service Providers in their region in order to avoid conflicts. This may be especially critical for equipment accessing multiple services, depending on their services and means of information distribution. May be the ID of a physical or non-physical station");
    private static readonly Field RtcmSatelliteIdField = RtcmV3Visitor.Field(nameof(SatelliteId), UInt8Type.Default, "GPS: Satellite ID number from 1 to 32 refers to the PRN code of the GPS satellite.Satellite ID’s higher than 32 are reserved for satellite signals from Satellite-Based Augmentation Systems (SBAS’s) such as the FAA’s Wide-Area Augmentation System(WAAS). SBAS PRN codes cover the range 120-138. The Satellite ID’s reserved for SBAS satellites are 40-58, so that the SBAS PRN codes are derived from the Version 3 Satellite ID codes by adding 80. GLONASS: Satellite ID number from 1 to 24 refers to the slot number of the GLONASS satellite.A Satellite ID of zero indicates that the slot number is unknown.Satellite ID’s higher than 32 are reserved for satellite signals from Satellite-Based Augmentation Systems(SBAS’s). SBAS PRN codes cover the range 120-138. The Satellite ID’s reserved for SBAS satellites are 40-58, so that the SBAS PRN codes are derived from the Version 3 GLONASS Satellite ID codes by adding 80. 0 – The slot number is unknown 1 to 24 – Slot number of the GLONASS satellite >32 – Reserved for Satellite-Based Augmentation Systems(SBAS). Note: For GLONASS-M satellites this data field has to contain the GLONASS-M word “n”, thus the Satellite Slot Number is always known(cannot be equal to zero) for GLONASS-M satellites.");
    private static readonly Field RtcmSOcField = RtcmV3Visitor.Field(nameof(SOc), DoubleType.Default, "Constant term of standard deviation (1 sigma) for non-dispersive interpolation residuals, mm.", readOnly: true);
    private static readonly Field RtcmSOdField = RtcmV3Visitor.Field(nameof(SOd), DoubleType.Default, "Distance dependent term of standard deviation(1 sigma) for non- dispersive interpolation residuals, ppm.", readOnly: true);
    private static readonly Field RtcmSOhField = RtcmV3Visitor.Field(nameof(SOh), DoubleType.Default, "Height dependent term of standard deviation (1 sigma) for nondispersive interpolation residuals, ppm. The complete standard deviation for the expected non-dispersive interpolation residual is computed from DF218,DF219 and DF220 using the formula.", readOnly: true);
    private static readonly Field RtcmSIcField = RtcmV3Visitor.Field(nameof(SIc), DoubleType.Default, "Constant term of standard deviation (1 sigma) for dispersive interpolation residuals (as affecting GPS L1 frequency)", readOnly: true);
    private static readonly Field RtcmSIdField = RtcmV3Visitor.Field(nameof(SId), DoubleType.Default, "Distance dependent term of standard deviation (1 sigma) for dispersive interpolation residuals. (as affecting GPS L1 frequency) The complete standard deviation for the expected dispersive interpolation residual is computed from DF221 and DF222 using the formula.", readOnly: true);

    internal new static readonly StructType RtcmVisitorType = new([..RtcmV3MessageBase.RtcmVisitorType.Fields, RtcmSatelliteResidualsField, RtcmResidualsEpochField, RtcmNRefsField, RtcmNumberSatelliteSignalsField, RtcmReferenceStationIDField, RtcmSatelliteIdField, RtcmSOcField, RtcmSOdField, RtcmSOhField, RtcmSIcField, RtcmSIdField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueSatelliteResiduals = SatelliteResiduals;
        RtcmV3Visitor.VisitArray(visitor, RtcmSatelliteResidualsField, RtcmSatelliteResidualsField.DataType, ref valueSatelliteResiduals,
            static () => new RtcmV3NetworkResidual(),
            static (IVisitor v, Field f, IFieldType t, ref RtcmV3NetworkResidual item) =>
            {
                if (item != null)
                {
                    StructType.Accept(v, f, t, item);
                }
                else
                {
                    v.VisitUnknown(f, t);
                }
            });
        if (!ReferenceEquals(valueSatelliteResiduals, SatelliteResiduals))
        {
            SatelliteResiduals = valueSatelliteResiduals!;
        }

        var valueResidualsEpoch = ResidualsEpoch;
        UInt32Type.Accept(visitor, RtcmResidualsEpochField, RtcmResidualsEpochField.DataType, ref valueResidualsEpoch);
        if (!EqualityComparer<uint>.Default.Equals(valueResidualsEpoch, ResidualsEpoch))
        {
            ResidualsEpoch = valueResidualsEpoch;
        }

        var valueNRefs = NRefs;
        UInt8Type.Accept(visitor, RtcmNRefsField, RtcmNRefsField.DataType, ref valueNRefs);
        if (!EqualityComparer<byte>.Default.Equals(valueNRefs, NRefs))
        {
            NRefs = valueNRefs;
        }

        var valueNumberSatelliteSignals = NumberSatelliteSignals;
        UInt8Type.Accept(visitor, RtcmNumberSatelliteSignalsField, RtcmNumberSatelliteSignalsField.DataType, ref valueNumberSatelliteSignals);
        if (!EqualityComparer<byte>.Default.Equals(valueNumberSatelliteSignals, NumberSatelliteSignals))
        {
            NumberSatelliteSignals = valueNumberSatelliteSignals;
        }

        var valueReferenceStationID = ReferenceStationID;
        UInt32Type.Accept(visitor, RtcmReferenceStationIDField, RtcmReferenceStationIDField.DataType, ref valueReferenceStationID);
        if (!EqualityComparer<uint>.Default.Equals(valueReferenceStationID, ReferenceStationID))
        {
            ReferenceStationID = valueReferenceStationID;
        }

        var valueSatelliteId = SatelliteId;
        UInt8Type.Accept(visitor, RtcmSatelliteIdField, RtcmSatelliteIdField.DataType, ref valueSatelliteId);
        if (!EqualityComparer<byte>.Default.Equals(valueSatelliteId, SatelliteId))
        {
            SatelliteId = valueSatelliteId;
        }

        var valueSOc = SOc;
        DoubleType.Accept(visitor, RtcmSOcField, RtcmSOcField.DataType, ref valueSOc);

        var valueSOd = SOd;
        DoubleType.Accept(visitor, RtcmSOdField, RtcmSOdField.DataType, ref valueSOd);

        var valueSOh = SOh;
        DoubleType.Accept(visitor, RtcmSOhField, RtcmSOhField.DataType, ref valueSOh);

        var valueSIc = SIc;
        DoubleType.Accept(visitor, RtcmSIcField, RtcmSIcField.DataType, ref valueSIc);

        var valueSId = SId;
        DoubleType.Accept(visitor, RtcmSIdField, RtcmSIdField.DataType, ref valueSId);
    }
}
