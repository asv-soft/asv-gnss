using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class SbfPacketPvtGeodeticRev2 : SbfMessageBase
    {
        public override ushort MessageRevision => 2;
        public override ushort MessageType => 4007;

        public override string Name => "PVTGeodeticV2";

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            Mode = BinSerialize.ReadByte(ref buffer);
            Solution = (SbfPVTSolution)(Mode & 0b0000_1111);
            Flag2D3D = (Mode >> 7) != 0;
            Error = BinSerialize.ReadByte(ref buffer);
            Latitude = CheckNan(BinSerialize.ReadDouble(ref buffer)) * 180.0 / Math.PI;
            Longitude = CheckNan(BinSerialize.ReadDouble(ref buffer)) * 180.0 / Math.PI;
            Height = CheckNan(BinSerialize.ReadDouble(ref buffer));
            Undulation = CheckNan(BinSerialize.ReadFloat(ref buffer));
            Vn = CheckNan(BinSerialize.ReadFloat(ref buffer));
            Ve = CheckNan(BinSerialize.ReadFloat(ref buffer));
            Vu = CheckNan(BinSerialize.ReadFloat(ref buffer));
            COG = CheckNan(BinSerialize.ReadFloat(ref buffer));
            RxClkBias = CheckNan(BinSerialize.ReadDouble(ref buffer)) / 1000.0;
            RxClkDrift = CheckNan(BinSerialize.ReadFloat(ref buffer));
            TimeSystem = (SbfPVTSolutionTime)BinSerialize.ReadByte(ref buffer);
            Datum = (SbfDatum)BinSerialize.ReadByte(ref buffer);
            NrSV = BinSerialize.ReadByte(ref buffer);
            WACorrInfo = (SbfWACorrInfoEnum)BinSerialize.ReadByte(ref buffer);
            ReferenceID = BinSerialize.ReadUShort(ref buffer);
            MeanCorrAge = BinSerialize.ReadUShort(ref buffer) * 0.01;
            SignalInfo = BinSerialize.ReadUInt(ref buffer);
            AlertFlag = BinSerialize.ReadByte(ref buffer);
            Raim = (SbfRaimEnum)(AlertFlag & 0b0000_00011);
            IsIntegrityPerGalileoHPCAFailed = (AlertFlag & 0b0000_0100) != 0;
            IsGalileoIonosphericStormActive = (AlertFlag & 0b0000_1000) != 0;

            // REV 1
            NrBases = BinSerialize.ReadByte(ref buffer);
            PPPInfo = BinSerialize.ReadUShort(ref buffer);
            AgeOfLastSeed = PPPInfo & 0b0000_1111_1111_1111;
            TypeOdLastSeed = (SbfTypeOdLastSeedEnum)(PPPInfo >> 14);

            // REV 2
            Latency = BinSerialize.ReadUShort(ref buffer) * 0.0001;
            HAccuracy = BinSerialize.ReadUShort(ref buffer) * 0.01;
            VAccuracy = BinSerialize.ReadUShort(ref buffer) * 0.01;
            Misc = BinSerialize.ReadByte(ref buffer);
            DGPSorRTK = (Misc & 0b0000_0001) != 0;
            OffsetIsCompensated = (Misc & 0b0000_0010) != 0;
            IsMarkerPositionReported = (SbfMarkerPositionEnum)(Misc >> 6);
        }

        /// <summary>
        /// Flag indicating whether the marker position reported in this
        /// block is also the ARP position(i.e.whether the ARP-tomarker
        /// offset provided with the setAntennaOffset command is zero or not)
        /// </summary>
        public SbfMarkerPositionEnum IsMarkerPositionReported { get; set; }

        /// <summary>
        /// Set if the phase center offset is compensated for at the rover,
        /// unset if not or unknown
        /// </summary>
        public bool OffsetIsCompensated { get; set; }

        /// <summary>
        /// In DGNSS or RTK mode, set if the baseline points to the base
        /// station ARP. Unset if it points to the antenna phase center, or
        /// if unknown. [Misc]
        /// </summary>
        public bool DGPSorRTK { get; set; }

        public byte Misc { get; set; }

        /// <summary>
        /// 2-sigma vertical accuracy. The vertical distance between the true
        /// position and the computed position is expected to be lower than
        /// VAccuracy with a probability of at least 95%. The value is clipped to 65534 =655.34m
        /// </summary>
        public double VAccuracy { get; set; }

        /// <summary>
        /// 2DRMS horizontal accuracy: twice the root-mean-square of the horizontal distance error.
        /// The horizontal distance between the true position
        /// and the computed position is expected to be lower than HAccuracy
        /// with a probability of at least 95%. The value is clipped to 65534 =655.34m
        /// </summary>
        public double HAccuracy { get; set; }

        /// <summary>
        /// Time elapsed between the time of applicability of the position fix and
        /// the generation of this SBF block by the receiver.This time includes the
        /// receiver processing time, but not the communication latency
        /// </summary>
        public double Latency { get; set; }

        /// <summary>
        /// Type of last seed (PPP info)
        /// </summary>
        public SbfTypeOdLastSeedEnum TypeOdLastSeed { get; set; }

        /// <summary>
        /// Age of the last seed, in seconds. The age is clipped to 4091s.
        /// This field must be ignored when the seed type is 0 (see bits 13-15 below).
        /// (PPP info)
        /// </summary>
        public int AgeOfLastSeed { get; set; }

        /// <summary>
        /// Bit field containing PPP-related information
        /// </summary>
        public ushort PPPInfo { get; set; }

        /// <summary>
        /// Number of base stations used in the PVT computation
        /// </summary>
        public byte NrBases { get; set; }

        /// <summary>
        /// Set if Galileo ionospheric storm flag is active
        /// </summary>
        public bool IsGalileoIonosphericStormActive { get; set; }

        /// <summary>
        /// Set if integrity has failed as per Galileo HPCA (HMI Probability Computation Algorithm)
        /// </summary>
        public bool IsIntegrityPerGalileoHPCAFailed { get; set; }

        /// <summary>
        /// RAIM integrity flag
        /// </summary>
        public SbfRaimEnum Raim { get; set; }

        public byte AlertFlag { get; set; }

        /// <summary>
        /// Bit field indicating the type of GNSS signals having been used in the PVT
        /// computations.If a bit i is set, the signal type having index i has been
        /// used.The signal numbers are listed in section 4.1.10. Bit 0 (GPS-C/A) is
        /// the LSB of SignalInfo.
        /// </summary>
        public uint SignalInfo { get; set; }

        /// <summary>
        /// In case of DGPS or RTK, this field is the mean age of the differential corrections.
        /// In case of SBAS operation, this field is the mean age of the ’fast corrections’ provided by the SBAS satellites.
        /// </summary>
        public double MeanCorrAge { get; set; }

        /// <summary>
        /// This field indicates the reference ID of the differential information used.
        /// In case of DGPS or RTK operation, this field is to be interpreted as the
        /// base station identifier.In SBAS operation, this field is to be interpreted
        /// as the PRN of the geostationary satellite used (from 120 to 158).
        /// If multiple base stations or multiple geostationary satellites are used the value is set to 65534
        /// </summary>
        public ushort ReferenceID { get; set; }

        /// <summary>
        /// Bit field providing information about which wide area corrections have been applied
        /// </summary>
        public SbfWACorrInfoEnum WACorrInfo { get; set; }

        /// <summary>
        /// Total number of satellites used in the PVT computation.
        /// </summary>
        public byte NrSV { get; set; }

        /// <summary>
        /// This field defines in which datum the coordinates are expressed
        /// </summary>
        public SbfDatum Datum { get; set; }

        /// <summary>
        /// Time system of which the offset is provided in this sub-block
        /// </summary>
        public SbfPVTSolutionTime TimeSystem { get; set; }

        /// <summary>
        /// Receiver clock drift relative to the GNSS system time (relative frequency
        /// error). Positive when the receiver clock runs faster than the system time.
        /// 1 ppm
        /// </summary>
        public float RxClkDrift { get; set; }

        /// <summary>
        /// Receiver clock bias relative to the GNSS system time reported in the
        /// TimeSystem field.Positive when the receiver time is ahead of the system time.
        /// To transfer the receiver time to the system time, use:tGPS/GST = trx - RxClkBias
        /// 1 ms
        /// </summary>
        public double RxClkBias { get; set; }

        /// <summary>
        /// Course over ground: this is defined as the angle of the vehicle with respect to the local level North,
        /// ranging from 0 to 360, and increasing towards east. Set to the Do-Not-Use value when the speed is lower than0.1m/s
        /// </summary>
        public float COG { get; set; }

        /// <summary>
        /// Velocity in the ’Up’ direction
        /// </summary>
        public float Vu { get; set; }

        /// <summary>
        /// Velocity in the East direction
        /// </summary>
        public float Ve { get; set; }

        /// <summary>
        /// Velocity in the North direction
        /// </summary>
        public float Vn { get; set; }

        /// <summary>
        /// Geoid undulation. See the setGeoidUndulation command.
        /// </summary>
        public float Undulation { get; set; }

        /// <summary>
        /// Ellipsoidal height (with respect to the ellipsoid specified by Datum)
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Longitude, from −π to +π, positive East of Greenwich
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Latitude, from −π/2 to +π/2, positive North of Equator
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// if 0, no error
        /// </summary>
        public byte Error { get; set; }
        public bool Flag2D3D { get; set; }
        public SbfPVTSolution Solution { get; set; }
        public byte Mode { get; set; }
        public string ErrorString => GetError(Error);

        private static float CheckNan(float toSingle)
        {
            return toSingle == -2E10 ? Single.NaN : toSingle;
        }

        private static double CheckNan(double toSingle)
        {
            return toSingle == -2E10 ? Single.NaN : toSingle;
        }

        private static string GetError(byte error)
        {
            switch (error)
            {
                case 0:
                    return "No Error";
                case 1:
                    return "Not enough measurements";
                case 2:
                    return "Not enough ephemerides available";
                case 3:
                    return "DOP too large (larger than 15)";
                case 4:
                    return "Sum of squared residuals too large";
                case 5:
                    return "No convergence";
                case 6:
                    return "Not enough measurements after outlier rejection";
                case 7:
                    return "Position output prohibited due to export laws";
                case 8:
                    return "Not enough differential corrections available";
                case 9:
                    return "Base station coordinates unavailable";
                case 10:
                    return "Ambiguities not fixed and user requested to only output RTK-fixed positions";
                default:
                    return "Unknown error !!!";
            }
        }
    }

    public enum SbfPVTSolution
    {
        NoPVT = 0,
        StandAlonePVT = 1,
        DifferentialPVT = 2,
        FixedLocation = 3,
        RTKWithFixedAmbiguities = 4,
        RTKWithFloatAmbiguities = 5,
        SBASaidedPVT = 6,
        MovingBaseRTKwithFixedAmbiguities = 7,
        MovingBaseRTKwithFloatAmbiguities = 8,
        PrecisePointPositioning = 10,
        Reserved = 12,
    }

    /// <summary>
    /// Time system of which the offset is provided in this sub-block:
    /// </summary>
    public enum SbfPVTSolutionTime
    {
        GPSTime = 0,
        GalileoTime = 1,
        GLONASSTime = 3,
        BeiDouTime = 4,
    }

    public enum SbfDatum
    {
        /// <summary>
        /// WGS84/ITRS
        /// </summary>
        WGS84 = 0,

        /// <summary>
        /// Datum equal to that used by the DGNSS/RTK base station
        /// </summary>
        DGNSS = 19,

        /// <summary>
        /// ETRS89 (ETRF2000 realization)
        /// </summary>
        ETRS89 = 30,

        /// <summary>
        /// NAD83(2011), North American Datum (2011)
        /// </summary>
        NAD832011 = 31,

        /// <summary>
        /// NAD83(PA11), North American Datum, Pacific plate (2011)
        /// </summary>
        NAD83PA11 = 32,

        /// <summary>
        /// NAD83(MA11), North American Datum, Marianas plate (2011)
        /// </summary>
        NAD83MA11 = 33,

        /// <summary>
        /// GDA94(2010), Geocentric Datum of Australia (2010)
        /// </summary>
        GDA942010 = 34,

        /// <summary>
        /// GDA2020, Geocentric Datum of Australia 2020
        /// </summary>
        GDA2020 = 35,

        /// <summary>
        /// First user-defined datum
        /// </summary>
        User1 = 250,

        /// <summary>
        /// Second user-defined datum
        /// </summary>
        User2 = 251,
    }

    [Flags]
    public enum SbfWACorrInfoEnum : byte
    {
        OrbitClockCorrectionUsed,
        RangeCorrectionUsed,
        IonoInfoUsed,
        OrbitAccuracyUereSisa,
        DO229PrecisionApproachModeActive,
        Reserved1,
        Reserved2,
        Reserved3,
    }

    public enum SbfRaimEnum
    {
        /// <summary>
        /// RAIM not active (integrity not monitored)
        /// </summary>
        RaimNotActive = 0,

        /// <summary>
        /// RAIM integrity test successful
        /// </summary>
        RaimIntegrityTestSuccessful = 1,

        /// <summary>
        /// RAIM integrity test failed
        /// </summary>
        RaimIntegrityTestFailed = 2,

        /// <summary>
        /// Reserved
        /// </summary>
        Reserved = 3,
    }

    public enum SbfTypeOdLastSeedEnum
    {
        /// <summary>
        /// Not seeded or not in PPP positioning mode
        /// </summary>
        NotSeeded = 0,

        /// <summary>
        ///  Manual seed
        /// </summary>
        ManualSeed = 1,

        /// <summary>
        /// Seeded from DGPS
        /// </summary>
        SeededFromDGPS = 2,

        /// <summary>
        /// Seeded from RTKFixed
        /// </summary>
        SeededFromRTKFixed = 3,
    }

    public enum SbfMarkerPositionEnum
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The ARP-to-marker offset is zero
        /// </summary>
        ARPtoMarkerOffsetIsZero = 1,

        /// <summary>
        /// The ARP-to-marker offset is not zero
        /// </summary>
        ARPtoMarkerOffsetIsNonZero = 2,
    }
}
