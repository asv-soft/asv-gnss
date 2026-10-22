using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

internal partial class SatelliteRtkResidual : IVisitable
{
    private static readonly Field RtcmSatelliteIdField = RtcmV3Visitor.Field(nameof(SatelliteId), UInt8Type.Default, "GPS: Satellite ID number from 1 to 32 refers to the PRN code of the GPS satellite.Satellite ID’s higher than 32 are reserved for satellite signals from Satellite-Based Augmentation Systems (SBAS’s) such as the FAA’s Wide-Area Augmentation System(WAAS). SBAS PRN codes cover the range 120-138. The Satellite ID’s reserved for SBAS satellites are 40-58, so that the SBAS PRN codes are derived from the Version 3 Satellite ID codes by adding 80. GLONASS: Satellite ID number from 1 to 24 refers to the slot number of the GLONASS satellite.A Satellite ID of zero indicates that the slot number is unknown.Satellite ID’s higher than 32 are reserved for satellite signals from Satellite-Based Augmentation Systems(SBAS’s). SBAS PRN codes cover the range 120-138. The Satellite ID’s reserved for SBAS satellites are 40-58, so that the SBAS PRN codes are derived from the Version 3 GLONASS Satellite ID codes by adding 80. 0 – The slot number is unknown 1 to 24 – Slot number of the GLONASS satellite >32 – Reserved for Satellite-Based Augmentation Systems(SBAS). Note: For GLONASS-M satellites this data field has to contain the GLONASS-M word “n”, thus the Satellite Slot Number is always known(cannot be equal to zero) for GLONASS-M satellites.");
    private static readonly Field RtcmSOcField = RtcmV3Visitor.Field(nameof(SOc), DoubleType.Default, "Constant term of standard deviation (1 sigma) for non-dispersive interpolation residuals, mm.", readOnly: true);
    private static readonly Field RtcmSOdField = RtcmV3Visitor.Field(nameof(SOd), DoubleType.Default, "Distance dependent term of standard deviation(1 sigma) for non- dispersive interpolation residuals, ppm.", readOnly: true);
    private static readonly Field RtcmSOhField = RtcmV3Visitor.Field(nameof(SOh), DoubleType.Default, "Height dependent term of standard deviation (1 sigma) for nondispersive interpolation residuals, ppm. The complete standard deviation for the expected non-dispersive interpolation residual is computed from DF218,DF219 and DF220 using the formula.", readOnly: true);
    private static readonly Field RtcmSIcField = RtcmV3Visitor.Field(nameof(SIc), DoubleType.Default, "Constant term of standard deviation (1 sigma) for dispersive interpolation residuals (as affecting GPS L1 frequency)", readOnly: true);
    private static readonly Field RtcmSIdField = RtcmV3Visitor.Field(nameof(SId), DoubleType.Default, "Distance dependent term of standard deviation (1 sigma) for dispersive interpolation residuals. (as affecting GPS L1 frequency) The complete standard deviation for the expected dispersive interpolation residual is computed from DF221 and DF222 using the formula.", readOnly: true);

    internal static readonly StructType RtcmVisitorType = new([RtcmSatelliteIdField, RtcmSOcField, RtcmSOdField, RtcmSOhField, RtcmSIcField, RtcmSIdField]);

    /// <inheritdoc />
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

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
