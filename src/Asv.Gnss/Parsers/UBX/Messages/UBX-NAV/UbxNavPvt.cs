using System;
using Asv.IO;
using Geodesy;

namespace Asv.Gnss
{
    public class UbxNavPvtPool : UbxMessageBase
    {
        public override string Name => "UBX-NAV-PVT-POOL";
        public override byte Class => 0x01;
        public override byte SubClass => 0x07;

        protected override void SerializeContent(ref Span<byte> buffer) { }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer) { }

        protected override int GetContentByteSize() => 0;

        public override void Randomize(Random random) { }
    }

    [SerializationNotSupported]
    public class UbxNavPvt : UbxMessageBase
    {
        public override string Name => "UBX-NAV-PVT";
        public override byte Class => 0x01;
        public override byte SubClass => 0x07;

        /// <summary>
        /// Magnetic declination accuracy. Only supported in ADR 4.10 and later. deg
        /// </summary>
        public double MagneticDeclinationAccuracy { get; set; }

        /// <summary>
        /// Magnetic declination. Only supported in ADR 4.10 and later. deg
        /// </summary>
        public double MagneticDeclination { get; set; }

        /// <summary>
        /// Heading of vehicle (2-D), this is only valid when headVehValid is set, otherwise the output is set to the heading of motion
        /// </summary>
        public double HeadingOfVehicle2D { get; set; }

        /// <summary>
        /// Invalid lon, lat, height and hMSL
        /// </summary>
        public bool IsValidLLH { get; set; }

        /// <summary>
        /// Position DOP
        /// </summary>
        public double PositionDOP { get; set; }

        /// <summary>
        /// Heading accuracy estimate (both motion and vehicle), deg
        /// </summary>
        public double HeadingAccuracyEstimate { get; set; }

        /// <summary>
        /// Speed accuracy estimate, m\s
        /// </summary>
        public double SpeedAccuracyEstimate { get; set; }

        /// <summary>
        /// Heading of motion (2-D), deg
        /// </summary>
        public double HeadingOfMotion2D { get; set; }

        /// <summary>
        /// Ground Speed (2-D), m\s
        /// </summary>
        public double GroundSpeed2D { get; set; }

        /// <summary>
        /// NED down velocity, m\s
        /// </summary>
        public double VelocityDown { get; set; }

        /// <summary>
        /// NED east velocity, m\s
        /// </summary>
        public double VelocityEast { get; set; }

        /// <summary>
        /// NED north velocity, m\s
        /// </summary>
        public double VelocityNorth { get; set; }

        /// <summary>
        /// Vertical accuracy estimate, m
        /// </summary>
        public double VerticalAccuracyEstimate { get; set; }

        /// <summary>
        /// Horizontal accuracy estimate, m
        /// </summary>
        public double HorizontalAccuracyEstimate { get; set; }

        /// <summary>
        /// Height above mean sea level, m
        /// </summary>
        public double AltMsl { get; set; }

        /// <summary>
        /// Height above ellipsoid, m
        /// </summary>
        public double AltElipsoid { get; set; }

        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Number of satellites used in Nav Solution
        /// </summary>
        public byte NumberOfSatellites { get; set; }

        /// <summary>
        /// UTC Time of Day could be confirmed (see Time Validity section for details)
        /// </summary>
        public bool UTCConfirmedTime { get; set; }

        /// <summary>
        /// UTC Date validity could be confirmed (see Time Validity section for details)
        /// </summary>
        public bool UTCConfirmedDate { get; set; }

        /// <summary>
        /// 1 = information about UTC Date and Time of Day validity confirmation is available (see Time Validity section for details)
        /// This flag is only supported in Protocol Versions 19.00, 19.10, 20.10, 20.20, 20.30, 22.00, 23.00, 23.01, 27 and 28
        /// </summary>
        public bool UTCConfirmedAvailable { get; set; }

        /// <summary>
        /// ???
        /// </summary>
        public byte PsmState { get; set; }

        public UbxCarrierSolutionStatus CarrierSolution { get; set; }

        /// <summary>
        /// valid fix (i.e within DOP & accuracy masks)
        /// </summary>
        public bool GnssFixOK { get; set; }

        /// <summary>
        /// Differential corrections were applied
        /// </summary>
        public bool IsAppliedDifferentialCorrections { get; set; }

        /// <summary>
        /// Differential corrections were applied
        /// </summary>
        public bool IsValidVehicleHeading { get; set; }

        /// <summary>
        /// GNSSfix Type
        /// </summary>
        public UbxGnssFixType FixType { get; set; }

        /// <summary>
        /// Fraction of second, range -1e9 .. 1e9 (UTC)
        /// </summary>
        public double UTCFractionOfSecond { get; set; }

        /// <summary>
        /// Time accuracy estimate (UTC)
        /// </summary>
        public double UTCTimeAccuracyEstimate { get; set; }

        /// <summary>
        /// valid magnetic declination
        /// </summary>
        public bool IsValidMagneticDeclination { get; set; }

        /// <summary>
        /// = UTC time of day has been fully resolved (no seconds uncertainty). Cannot be used to check if time is completely solved.
        /// </summary>
        public bool UTCTimeOfDayIsFullyResolved { get; set; }

        /// <summary>
        /// valid UTC time of day (see Time Validity section for details)
        /// </summary>
        public bool UTCTimeIsConfirmation { get; set; }

        /// <summary>
        /// valid UTC Date (see Time Validity section for details)
        /// </summary>
        public bool UTCDateIsConfirmation { get; set; }

        /// <summary>
        /// Seconds of minute, range 0..60 (UTC)
        /// </summary>
        public byte Sec { get; set; }

        /// <summary>
        /// Minute of hour, range 0..59 (UTC)
        /// </summary>
        public byte Min { get; set; }

        /// <summary>
        /// Hour of day, range 0..23 (UTC)
        /// </summary>
        public byte Hour { get; set; }

        /// <summary>
        /// Day of month, range 1..31 (UTC)
        /// </summary>
        public byte Day { get; set; }

        /// <summary>
        /// Month, range 1..12 (UTC)
        /// </summary>
        public byte Month { get; set; }

        /// <summary>
        /// Year (UTC)
        /// </summary>
        public ushort Year { get; set; }

        /// <summary>
        /// GPS time of week of the navigation epoch.
        /// See the description of iTOW for details.
        /// </summary>
        public ulong iTOW { get; set; }

        public GlobalPosition MovingBaseLocation { get; set; }

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
        {
            iTOW = BinSerialize.ReadUInt(ref buffer);
            Year = BinSerialize.ReadUShort(ref buffer);
            Month = BinSerialize.ReadByte(ref buffer);
            Day = BinSerialize.ReadByte(ref buffer);
            Hour = BinSerialize.ReadByte(ref buffer);
            Min = BinSerialize.ReadByte(ref buffer);
            Sec = BinSerialize.ReadByte(ref buffer);
            var valid = BinSerialize.ReadByte(ref buffer);
            //UTC Date and Time Confirmation Status   Date: CONFIRMED, Time: CONFIRMED

            UTCDateIsConfirmation = (valid & 0b0000_0001) != 0;
            UTCTimeIsConfirmation = (valid & 0b0000_0010) != 0;
            UTCTimeOfDayIsFullyResolved = (valid & 0b0000_0100) != 0;
            IsValidMagneticDeclination = (valid & 0b0000_1000) != 0;

            UTCTimeAccuracyEstimate = BinSerialize.ReadUInt(ref buffer) * 1e-9;
            UTCFractionOfSecond = BinSerialize.ReadInt(ref buffer);
            FixType = (UbxGnssFixType)BinSerialize.ReadByte(ref buffer);

            var flags = BinSerialize.ReadByte(ref buffer);

            GnssFixOK = (flags & 0b0000_0001) != 0;
            IsAppliedDifferentialCorrections = (flags & 0b0000_0010) != 0;
            IsValidVehicleHeading = (flags & 0b0010_0000) != 0;
            PsmState = (byte)((flags & 0b0001_1100) >> 2);
            CarrierSolution = (UbxCarrierSolutionStatus)((flags & 0b1100_0000) >> 6);

            flags = BinSerialize.ReadByte(ref buffer);
            UTCConfirmedAvailable = (flags & 0b0010_0000) != 0;
            UTCConfirmedDate = (flags & 0b0100_0000) != 0;
            UTCConfirmedTime = (flags & 0b1000_0000) != 0;
            //
            NumberOfSatellites = BinSerialize.ReadByte(ref buffer);
            Longitude = BinSerialize.ReadInt(ref buffer) * 1e-7;
            Latitude = BinSerialize.ReadInt(ref buffer) * 1e-7;
            AltElipsoid = BinSerialize.ReadInt(ref buffer) * 0.001;
            AltMsl = BinSerialize.ReadInt(ref buffer) * 0.001;
            HorizontalAccuracyEstimate = BinSerialize.ReadUInt(ref buffer) * 0.001;
            VerticalAccuracyEstimate = BinSerialize.ReadUInt(ref buffer) * 0.001;
            VelocityNorth = BinSerialize.ReadInt(ref buffer) * 0.001;
            VelocityEast = BinSerialize.ReadInt(ref buffer) * 0.001;
            VelocityDown = BinSerialize.ReadInt(ref buffer) * 0.001;
            GroundSpeed2D = BinSerialize.ReadInt(ref buffer) * 0.001;
            HeadingOfMotion2D = BinSerialize.ReadInt(ref buffer) * 1e-5;
            SpeedAccuracyEstimate = BinSerialize.ReadUInt(ref buffer) * 0.001;
            HeadingAccuracyEstimate = BinSerialize.ReadUInt(ref buffer) * 1e-5;
            PositionDOP = BinSerialize.ReadUShort(ref buffer) * 0.01;
            flags = BinSerialize.ReadByte(ref buffer);
            IsValidLLH = (flags & 0b0000_0001) == 0;
            buffer = buffer[5..];
            HeadingOfVehicle2D = BinSerialize.ReadInt(ref buffer) * 1e-5;
            if (!IsValidVehicleHeading)
                HeadingOfVehicle2D = double.NaN;
            MagneticDeclination = BinSerialize.ReadShort(ref buffer) * 1e-2;
            MagneticDeclinationAccuracy = BinSerialize.ReadUShort(ref buffer) * 1e-2;
            //
            if (FixType >= UbxGnssFixType.Fix3D && GnssFixOK)
            {
                MovingBaseLocation = new GlobalPosition(
                    new GlobalCoordinates(Latitude, Longitude),
                    AltElipsoid
                );
            }

            UtcTime = new DateTime(Year, Month, Day, Hour, Min, Sec, DateTimeKind.Utc).AddSeconds(
                UTCFractionOfSecond * 1e-9
            );

            var week = 0;
            var towP = 0.0;
            var tow = iTOW * 1e-3;
            GpsRawHelper.Time2Gps(GpsRawHelper.Utc2Gps(UtcTime), ref week, ref towP);
            if (tow < towP - 302400.0)
                week += 1;
            else if (tow > towP + 302400.0)
                week -= 1;
            GpsTime = GpsRawHelper.Gps2Time(week, tow);
        }

        public DateTime GpsTime { get; set; }

        public DateTime UtcTime { get; set; }

        protected override int GetContentByteSize() => 92;

        public override void Randomize(Random random) { }
    }

    public enum UbxCarrierSolutionStatus
    {
        /// <summary>
        /// no carrier phase range solution
        /// </summary>
        NoCarrierSolution = 0,

        /// <summary>
        /// carrier phase range solution with floating ambiguities
        /// </summary>
        FloatingAmbiguities = 1,

        /// <summary>
        /// carrier phase range solution with fixed ambiguities
        /// </summary>
        FixedAmbiguities = 2,
    }

    public enum UbxGnssFixType : byte
    {
        NoFix = 0,
        DeadReckoningOnly = 1,
        Fix2D = 2,
        Fix3D = 3,
        GNSSDeadReckoning = 4,
        TimeOnlyFix = 5,
    }
}
