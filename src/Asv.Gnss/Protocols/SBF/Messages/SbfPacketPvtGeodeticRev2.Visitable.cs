using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial class SbfPacketPvtGeodeticRev2
{
    private static readonly Field PayloadIsMarkerPositionReportedField = SbfComNavVisitor.Field(nameof(IsMarkerPositionReported), Int32Type.Default, "Flag indicating whether the marker position reported in this block is also the ARP position(i.e.whether the ARP-tomarker offset provided with the setAntennaOffset command is zero or not)", enumType: typeof(SbfMarkerPositionEnum));
    private static readonly Field PayloadOffsetIsCompensatedField = SbfComNavVisitor.Field(nameof(OffsetIsCompensated), BoolType.Default, "Set if the phase center offset is compensated for at the rover, unset if not or unknown");
    private static readonly Field PayloadDGPSorRTKField = SbfComNavVisitor.Field(nameof(DGPSorRTK), BoolType.Default, "In DGNSS or RTK mode, set if the baseline points to the base station ARP. Unset if it points to the antenna phase center, or if unknown. [Misc]");
    private static readonly Field PayloadMiscField = SbfComNavVisitor.Field(nameof(Misc), UInt8Type.Default, "Misc");
    private static readonly Field PayloadVAccuracyField = SbfComNavVisitor.Field(nameof(VAccuracy), DoubleType.Default, "2-sigma vertical accuracy. The vertical distance between the true position and the computed position is expected to be lower than VAccuracy with a probability of at least 95%. The value is clipped to 65534 =655.34m");
    private static readonly Field PayloadHAccuracyField = SbfComNavVisitor.Field(nameof(HAccuracy), DoubleType.Default, "2DRMS horizontal accuracy: twice the root-mean-square of the horizontal distance error. The horizontal distance between the true position and the computed position is expected to be lower than HAccuracy with a probability of at least 95%. The value is clipped to 65534 =655.34m");
    private static readonly Field PayloadLatencyField = SbfComNavVisitor.Field(nameof(Latency), DoubleType.Default, "Time elapsed between the time of applicability of the position fix and the generation of this SBF block by the receiver.This time includes the receiver processing time, but not the communication latency");
    private static readonly Field PayloadTypeOdLastSeedField = SbfComNavVisitor.Field(nameof(TypeOdLastSeed), Int32Type.Default, "Type of last seed (PPP info)", enumType: typeof(SbfTypeOdLastSeedEnum));
    private static readonly Field PayloadAgeOfLastSeedField = SbfComNavVisitor.Field(nameof(AgeOfLastSeed), Int32Type.Default, "Age of the last seed, in seconds. The age is clipped to 4091s. This field must be ignored when the seed type is 0 (see bits 13-15 below). (PPP info)");
    private static readonly Field PayloadPPPInfoField = SbfComNavVisitor.Field(nameof(PPPInfo), UInt16Type.Default, "Bit field containing PPP-related information");
    private static readonly Field PayloadNrBasesField = SbfComNavVisitor.Field(nameof(NrBases), UInt8Type.Default, "Number of base stations used in the PVT computation");
    private static readonly Field PayloadIsGalileoIonosphericStormActiveField = SbfComNavVisitor.Field(nameof(IsGalileoIonosphericStormActive), BoolType.Default, "Set if Galileo ionospheric storm flag is active");
    private static readonly Field PayloadIsIntegrityPerGalileoHPCAFailedField = SbfComNavVisitor.Field(nameof(IsIntegrityPerGalileoHPCAFailed), BoolType.Default, "Set if integrity has failed as per Galileo HPCA (HMI Probability Computation Algorithm)");
    private static readonly Field PayloadRaimField = SbfComNavVisitor.Field(nameof(Raim), Int32Type.Default, "RAIM integrity flag", enumType: typeof(SbfRaimEnum));
    private static readonly Field PayloadAlertFlagField = SbfComNavVisitor.Field(nameof(AlertFlag), UInt8Type.Default, "AlertFlag");
    private static readonly Field PayloadSignalInfoField = SbfComNavVisitor.Field(nameof(SignalInfo), UInt32Type.Default, "Bit field indicating the type of GNSS signals having been used in the PVT computations.If a bit i is set, the signal type having index i has been used.The signal numbers are listed in section 4.1.10. Bit 0 (GPS-C/A) is the LSB of SignalInfo.");
    private static readonly Field PayloadMeanCorrAgeField = SbfComNavVisitor.Field(nameof(MeanCorrAge), DoubleType.Default, "In case of DGPS or RTK, this field is the mean age of the differential corrections. In case of SBAS operation, this field is the mean age of the \u2019fast corrections\u2019 provided by the SBAS satellites.");
    private static readonly Field PayloadReferenceIDField = SbfComNavVisitor.Field(nameof(ReferenceID), UInt16Type.Default, "This field indicates the reference ID of the differential information used. In case of DGPS or RTK operation, this field is to be interpreted as the base station identifier.In SBAS operation, this field is to be interpreted as the PRN of the geostationary satellite used (from 120 to 158). If multiple base stations or multiple geostationary satellites are used the value is set to 65534");
    private static readonly Field PayloadWACorrInfoField = SbfComNavVisitor.Field(nameof(WACorrInfo), UInt8Type.Default, "Bit field providing information about which wide area corrections have been applied", enumType: typeof(SbfWACorrInfoEnum));
    private static readonly Field PayloadNrSVField = SbfComNavVisitor.Field(nameof(NrSV), UInt8Type.Default, "Total number of satellites used in the PVT computation.");
    private static readonly Field PayloadDatumField = SbfComNavVisitor.Field(nameof(Datum), Int32Type.Default, "This field defines in which datum the coordinates are expressed", enumType: typeof(SbfDatum));
    private static readonly Field PayloadTimeSystemField = SbfComNavVisitor.Field(nameof(TimeSystem), Int32Type.Default, "Time system of which the offset is provided in this sub-block", enumType: typeof(SbfPVTSolutionTime));
    private static readonly Field PayloadRxClkDriftField = SbfComNavVisitor.Field(nameof(RxClkDrift), FloatType.Default, "Receiver clock drift relative to the GNSS system time (relative frequency error). Positive when the receiver clock runs faster than the system time. 1 ppm");
    private static readonly Field PayloadRxClkBiasField = SbfComNavVisitor.Field(nameof(RxClkBias), DoubleType.Default, "Receiver clock bias relative to the GNSS system time reported in the TimeSystem field.Positive when the receiver time is ahead of the system time. To transfer the receiver time to the system time, use:tGPS/GST = trx - RxClkBias 1 ms");
    private static readonly Field PayloadCOGField = SbfComNavVisitor.Field(nameof(COG), FloatType.Default, "Course over ground: this is defined as the angle of the vehicle with respect to the local level North, ranging from 0 to 360, and increasing towards east. Set to the Do-Not-Use value when the speed is lower than0.1m/s");
    private static readonly Field PayloadVuField = SbfComNavVisitor.Field(nameof(Vu), FloatType.Default, "Velocity in the \u2019Up\u2019 direction");
    private static readonly Field PayloadVeField = SbfComNavVisitor.Field(nameof(Ve), FloatType.Default, "Velocity in the East direction");
    private static readonly Field PayloadVnField = SbfComNavVisitor.Field(nameof(Vn), FloatType.Default, "Velocity in the North direction");
    private static readonly Field PayloadUndulationField = SbfComNavVisitor.Field(nameof(Undulation), FloatType.Default, "Geoid undulation. See the setGeoidUndulation command.");
    private static readonly Field PayloadHeightField = SbfComNavVisitor.Field(nameof(Height), DoubleType.Default, "Ellipsoidal height (with respect to the ellipsoid specified by Datum)");
    private static readonly Field PayloadLongitudeField = SbfComNavVisitor.Field(nameof(Longitude), DoubleType.Default, "Longitude, from \u2212\u03c0 to +\u03c0, positive East of Greenwich");
    private static readonly Field PayloadLatitudeField = SbfComNavVisitor.Field(nameof(Latitude), DoubleType.Default, "Latitude, from \u2212\u03c0/2 to +\u03c0/2, positive North of Equator");
    private static readonly Field PayloadErrorField = SbfComNavVisitor.Field(nameof(Error), UInt8Type.Default, "if 0, no error");
    private static readonly Field PayloadFlag2D3DField = SbfComNavVisitor.Field(nameof(Flag2D3D), BoolType.Default, "Flag2D3D");
    private static readonly Field PayloadSolutionField = SbfComNavVisitor.Field(nameof(Solution), Int32Type.Default, "Solution", enumType: typeof(SbfPVTSolution));
    private static readonly Field PayloadModeField = SbfComNavVisitor.Field(nameof(Mode), UInt8Type.Default, "Mode");
    private static readonly Field PayloadErrorStringField = SbfComNavVisitor.Field(nameof(ErrorString), StringOptionalType.Utf8, "ErrorString", readOnly: true);

    internal new static readonly StructType PayloadVisitorType = new([..SbfMessageBase.PayloadVisitorType.Fields, PayloadIsMarkerPositionReportedField, PayloadOffsetIsCompensatedField, PayloadDGPSorRTKField, PayloadMiscField, PayloadVAccuracyField, PayloadHAccuracyField, PayloadLatencyField, PayloadTypeOdLastSeedField, PayloadAgeOfLastSeedField, PayloadPPPInfoField, PayloadNrBasesField, PayloadIsGalileoIonosphericStormActiveField, PayloadIsIntegrityPerGalileoHPCAFailedField, PayloadRaimField, PayloadAlertFlagField, PayloadSignalInfoField, PayloadMeanCorrAgeField, PayloadReferenceIDField, PayloadWACorrInfoField, PayloadNrSVField, PayloadDatumField, PayloadTimeSystemField, PayloadRxClkDriftField, PayloadRxClkBiasField, PayloadCOGField, PayloadVuField, PayloadVeField, PayloadVnField, PayloadUndulationField, PayloadHeightField, PayloadLongitudeField, PayloadLatitudeField, PayloadErrorField, PayloadFlag2D3DField, PayloadSolutionField, PayloadModeField, PayloadErrorStringField]);

    /// <inheritdoc />
    public override void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        base.Accept(visitor);

        var valueIsMarkerPositionReported = IsMarkerPositionReported;
        var numericvalueIsMarkerPositionReported = (int)valueIsMarkerPositionReported;
        Int32Type.Accept(visitor, PayloadIsMarkerPositionReportedField, PayloadIsMarkerPositionReportedField.DataType, ref numericvalueIsMarkerPositionReported);
        valueIsMarkerPositionReported = (SbfMarkerPositionEnum)numericvalueIsMarkerPositionReported;
        if (!EqualityComparer<SbfMarkerPositionEnum>.Default.Equals(valueIsMarkerPositionReported, IsMarkerPositionReported))
        {
            IsMarkerPositionReported = valueIsMarkerPositionReported;
        }

        var valueOffsetIsCompensated = OffsetIsCompensated;
        BoolType.Accept(visitor, PayloadOffsetIsCompensatedField, PayloadOffsetIsCompensatedField.DataType, ref valueOffsetIsCompensated);
        if (!EqualityComparer<bool>.Default.Equals(valueOffsetIsCompensated, OffsetIsCompensated))
        {
            OffsetIsCompensated = valueOffsetIsCompensated;
        }

        var valueDGPSorRTK = DGPSorRTK;
        BoolType.Accept(visitor, PayloadDGPSorRTKField, PayloadDGPSorRTKField.DataType, ref valueDGPSorRTK);
        if (!EqualityComparer<bool>.Default.Equals(valueDGPSorRTK, DGPSorRTK))
        {
            DGPSorRTK = valueDGPSorRTK;
        }

        var valueMisc = Misc;
        UInt8Type.Accept(visitor, PayloadMiscField, PayloadMiscField.DataType, ref valueMisc);
        if (!EqualityComparer<byte>.Default.Equals(valueMisc, Misc))
        {
            Misc = valueMisc;
        }

        var valueVAccuracy = VAccuracy;
        DoubleType.Accept(visitor, PayloadVAccuracyField, PayloadVAccuracyField.DataType, ref valueVAccuracy);
        if (!EqualityComparer<double>.Default.Equals(valueVAccuracy, VAccuracy))
        {
            VAccuracy = valueVAccuracy;
        }

        var valueHAccuracy = HAccuracy;
        DoubleType.Accept(visitor, PayloadHAccuracyField, PayloadHAccuracyField.DataType, ref valueHAccuracy);
        if (!EqualityComparer<double>.Default.Equals(valueHAccuracy, HAccuracy))
        {
            HAccuracy = valueHAccuracy;
        }

        var valueLatency = Latency;
        DoubleType.Accept(visitor, PayloadLatencyField, PayloadLatencyField.DataType, ref valueLatency);
        if (!EqualityComparer<double>.Default.Equals(valueLatency, Latency))
        {
            Latency = valueLatency;
        }

        var valueTypeOdLastSeed = TypeOdLastSeed;
        var numericvalueTypeOdLastSeed = (int)valueTypeOdLastSeed;
        Int32Type.Accept(visitor, PayloadTypeOdLastSeedField, PayloadTypeOdLastSeedField.DataType, ref numericvalueTypeOdLastSeed);
        valueTypeOdLastSeed = (SbfTypeOdLastSeedEnum)numericvalueTypeOdLastSeed;
        if (!EqualityComparer<SbfTypeOdLastSeedEnum>.Default.Equals(valueTypeOdLastSeed, TypeOdLastSeed))
        {
            TypeOdLastSeed = valueTypeOdLastSeed;
        }

        var valueAgeOfLastSeed = AgeOfLastSeed;
        Int32Type.Accept(visitor, PayloadAgeOfLastSeedField, PayloadAgeOfLastSeedField.DataType, ref valueAgeOfLastSeed);
        if (!EqualityComparer<int>.Default.Equals(valueAgeOfLastSeed, AgeOfLastSeed))
        {
            AgeOfLastSeed = valueAgeOfLastSeed;
        }

        var valuePPPInfo = PPPInfo;
        UInt16Type.Accept(visitor, PayloadPPPInfoField, PayloadPPPInfoField.DataType, ref valuePPPInfo);
        if (!EqualityComparer<ushort>.Default.Equals(valuePPPInfo, PPPInfo))
        {
            PPPInfo = valuePPPInfo;
        }

        var valueNrBases = NrBases;
        UInt8Type.Accept(visitor, PayloadNrBasesField, PayloadNrBasesField.DataType, ref valueNrBases);
        if (!EqualityComparer<byte>.Default.Equals(valueNrBases, NrBases))
        {
            NrBases = valueNrBases;
        }

        var valueIsGalileoIonosphericStormActive = IsGalileoIonosphericStormActive;
        BoolType.Accept(visitor, PayloadIsGalileoIonosphericStormActiveField, PayloadIsGalileoIonosphericStormActiveField.DataType, ref valueIsGalileoIonosphericStormActive);
        if (!EqualityComparer<bool>.Default.Equals(valueIsGalileoIonosphericStormActive, IsGalileoIonosphericStormActive))
        {
            IsGalileoIonosphericStormActive = valueIsGalileoIonosphericStormActive;
        }

        var valueIsIntegrityPerGalileoHPCAFailed = IsIntegrityPerGalileoHPCAFailed;
        BoolType.Accept(visitor, PayloadIsIntegrityPerGalileoHPCAFailedField, PayloadIsIntegrityPerGalileoHPCAFailedField.DataType, ref valueIsIntegrityPerGalileoHPCAFailed);
        if (!EqualityComparer<bool>.Default.Equals(valueIsIntegrityPerGalileoHPCAFailed, IsIntegrityPerGalileoHPCAFailed))
        {
            IsIntegrityPerGalileoHPCAFailed = valueIsIntegrityPerGalileoHPCAFailed;
        }

        var valueRaim = Raim;
        var numericvalueRaim = (int)valueRaim;
        Int32Type.Accept(visitor, PayloadRaimField, PayloadRaimField.DataType, ref numericvalueRaim);
        valueRaim = (SbfRaimEnum)numericvalueRaim;
        if (!EqualityComparer<SbfRaimEnum>.Default.Equals(valueRaim, Raim))
        {
            Raim = valueRaim;
        }

        var valueAlertFlag = AlertFlag;
        UInt8Type.Accept(visitor, PayloadAlertFlagField, PayloadAlertFlagField.DataType, ref valueAlertFlag);
        if (!EqualityComparer<byte>.Default.Equals(valueAlertFlag, AlertFlag))
        {
            AlertFlag = valueAlertFlag;
        }

        var valueSignalInfo = SignalInfo;
        UInt32Type.Accept(visitor, PayloadSignalInfoField, PayloadSignalInfoField.DataType, ref valueSignalInfo);
        if (!EqualityComparer<uint>.Default.Equals(valueSignalInfo, SignalInfo))
        {
            SignalInfo = valueSignalInfo;
        }

        var valueMeanCorrAge = MeanCorrAge;
        DoubleType.Accept(visitor, PayloadMeanCorrAgeField, PayloadMeanCorrAgeField.DataType, ref valueMeanCorrAge);
        if (!EqualityComparer<double>.Default.Equals(valueMeanCorrAge, MeanCorrAge))
        {
            MeanCorrAge = valueMeanCorrAge;
        }

        var valueReferenceID = ReferenceID;
        UInt16Type.Accept(visitor, PayloadReferenceIDField, PayloadReferenceIDField.DataType, ref valueReferenceID);
        if (!EqualityComparer<ushort>.Default.Equals(valueReferenceID, ReferenceID))
        {
            ReferenceID = valueReferenceID;
        }

        var valueWACorrInfo = WACorrInfo;
        var numericvalueWACorrInfo = (byte)valueWACorrInfo;
        UInt8Type.Accept(visitor, PayloadWACorrInfoField, PayloadWACorrInfoField.DataType, ref numericvalueWACorrInfo);
        valueWACorrInfo = (SbfWACorrInfoEnum)numericvalueWACorrInfo;
        if (!EqualityComparer<SbfWACorrInfoEnum>.Default.Equals(valueWACorrInfo, WACorrInfo))
        {
            WACorrInfo = valueWACorrInfo;
        }

        var valueNrSV = NrSV;
        UInt8Type.Accept(visitor, PayloadNrSVField, PayloadNrSVField.DataType, ref valueNrSV);
        if (!EqualityComparer<byte>.Default.Equals(valueNrSV, NrSV))
        {
            NrSV = valueNrSV;
        }

        var valueDatum = Datum;
        var numericvalueDatum = (int)valueDatum;
        Int32Type.Accept(visitor, PayloadDatumField, PayloadDatumField.DataType, ref numericvalueDatum);
        valueDatum = (SbfDatum)numericvalueDatum;
        if (!EqualityComparer<SbfDatum>.Default.Equals(valueDatum, Datum))
        {
            Datum = valueDatum;
        }

        var valueTimeSystem = TimeSystem;
        var numericvalueTimeSystem = (int)valueTimeSystem;
        Int32Type.Accept(visitor, PayloadTimeSystemField, PayloadTimeSystemField.DataType, ref numericvalueTimeSystem);
        valueTimeSystem = (SbfPVTSolutionTime)numericvalueTimeSystem;
        if (!EqualityComparer<SbfPVTSolutionTime>.Default.Equals(valueTimeSystem, TimeSystem))
        {
            TimeSystem = valueTimeSystem;
        }

        var valueRxClkDrift = RxClkDrift;
        FloatType.Accept(visitor, PayloadRxClkDriftField, PayloadRxClkDriftField.DataType, ref valueRxClkDrift);
        if (!EqualityComparer<float>.Default.Equals(valueRxClkDrift, RxClkDrift))
        {
            RxClkDrift = valueRxClkDrift;
        }

        var valueRxClkBias = RxClkBias;
        DoubleType.Accept(visitor, PayloadRxClkBiasField, PayloadRxClkBiasField.DataType, ref valueRxClkBias);
        if (!EqualityComparer<double>.Default.Equals(valueRxClkBias, RxClkBias))
        {
            RxClkBias = valueRxClkBias;
        }

        var valueCOG = COG;
        FloatType.Accept(visitor, PayloadCOGField, PayloadCOGField.DataType, ref valueCOG);
        if (!EqualityComparer<float>.Default.Equals(valueCOG, COG))
        {
            COG = valueCOG;
        }

        var valueVu = Vu;
        FloatType.Accept(visitor, PayloadVuField, PayloadVuField.DataType, ref valueVu);
        if (!EqualityComparer<float>.Default.Equals(valueVu, Vu))
        {
            Vu = valueVu;
        }

        var valueVe = Ve;
        FloatType.Accept(visitor, PayloadVeField, PayloadVeField.DataType, ref valueVe);
        if (!EqualityComparer<float>.Default.Equals(valueVe, Ve))
        {
            Ve = valueVe;
        }

        var valueVn = Vn;
        FloatType.Accept(visitor, PayloadVnField, PayloadVnField.DataType, ref valueVn);
        if (!EqualityComparer<float>.Default.Equals(valueVn, Vn))
        {
            Vn = valueVn;
        }

        var valueUndulation = Undulation;
        FloatType.Accept(visitor, PayloadUndulationField, PayloadUndulationField.DataType, ref valueUndulation);
        if (!EqualityComparer<float>.Default.Equals(valueUndulation, Undulation))
        {
            Undulation = valueUndulation;
        }

        var valueHeight = Height;
        DoubleType.Accept(visitor, PayloadHeightField, PayloadHeightField.DataType, ref valueHeight);
        if (!EqualityComparer<double>.Default.Equals(valueHeight, Height))
        {
            Height = valueHeight;
        }

        var valueLongitude = Longitude;
        DoubleType.Accept(visitor, PayloadLongitudeField, PayloadLongitudeField.DataType, ref valueLongitude);
        if (!EqualityComparer<double>.Default.Equals(valueLongitude, Longitude))
        {
            Longitude = valueLongitude;
        }

        var valueLatitude = Latitude;
        DoubleType.Accept(visitor, PayloadLatitudeField, PayloadLatitudeField.DataType, ref valueLatitude);
        if (!EqualityComparer<double>.Default.Equals(valueLatitude, Latitude))
        {
            Latitude = valueLatitude;
        }

        var valueError = Error;
        UInt8Type.Accept(visitor, PayloadErrorField, PayloadErrorField.DataType, ref valueError);
        if (!EqualityComparer<byte>.Default.Equals(valueError, Error))
        {
            Error = valueError;
        }

        var valueFlag2D3D = Flag2D3D;
        BoolType.Accept(visitor, PayloadFlag2D3DField, PayloadFlag2D3DField.DataType, ref valueFlag2D3D);
        if (!EqualityComparer<bool>.Default.Equals(valueFlag2D3D, Flag2D3D))
        {
            Flag2D3D = valueFlag2D3D;
        }

        var valueSolution = Solution;
        var numericvalueSolution = (int)valueSolution;
        Int32Type.Accept(visitor, PayloadSolutionField, PayloadSolutionField.DataType, ref numericvalueSolution);
        valueSolution = (SbfPVTSolution)numericvalueSolution;
        if (!EqualityComparer<SbfPVTSolution>.Default.Equals(valueSolution, Solution))
        {
            Solution = valueSolution;
        }

        var valueMode = Mode;
        UInt8Type.Accept(visitor, PayloadModeField, PayloadModeField.DataType, ref valueMode);
        if (!EqualityComparer<byte>.Default.Equals(valueMode, Mode))
        {
            Mode = valueMode;
        }

        string? valueErrorString = ErrorString;
        StringOptionalType.Accept(visitor, PayloadErrorStringField, PayloadErrorStringField.DataType, ref valueErrorString);
    }
}
