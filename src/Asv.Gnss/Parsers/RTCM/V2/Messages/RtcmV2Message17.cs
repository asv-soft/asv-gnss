using System;
using Asv.IO;

namespace Asv.Gnss
{
    /// <summary>
    /// Represents a GPS ephemeris message in RTCM version 2 format.
    /// </summary>
    public class RtcmV2Message17 : RtcmV2MessageBase
    {
        /// <summary>
        /// Adjusts the GPS week number based on the current system time.
        /// </summary>
        /// <param name="week">The GPS week number to be adjusted.</param>
        /// <returns>The adjusted GPS week number.</returns>
        private int adjgpsweek(int week)
        {
            var now = DateTime.UtcNow;
            var w = 0;
            double sec = 0;
            RtcmV3Helper.GetFromTime(RtcmV3Helper.Utc2Gps(now), ref w, ref sec);
            if (w < 1560) w = 1560; /* use 2009/12/1 if time is earlier than 2009/12/1 */
            return week + (w - week + 1) / 1024 * 1024;
        }

        /// <summary>
        /// Represents the identifier for an RTCM message.
        /// </summary>
        /// <remarks>
        /// The RTCM message identifier is a constant value used to differentiate between different RTCM messages.
        /// </remarks>
        public const int RtcmMessageId = 17;

        /// <summary>
        /// Gets the MessageId of the RTCM message.
        /// </summary>
        /// <value>
        /// The MessageId as an unsigned short.
        /// </value>
        public override ushort MessageId => RtcmMessageId;

        /// <summary>
        /// Gets the name of the GPS ephemeris message.
        /// </summary>
        /// <returns>The name of the GPS ephemeris message.</returns>
        public override string Name => "GPS ephemeris message";

        /// <summary>
        /// Gets or sets the value of the Satellite property.
        /// </summary>
        /// <value>
        /// An integer representing the value of the Satellite property.
        /// </value>
        public int Satellite { get; set; }

        /// <summary>
        /// Gets or sets the raw week number.
        /// </summary>
        /// <value>
        /// The raw week number represented as an unsigned integer.
        /// </value>
        public uint WeekNumberRaw { get; set; }

        /// <summary>
        /// Gets or sets the week number.
        /// </summary>
        public int WeekNumber { get; set; }

        /// <summary>
        /// Speed of change of inclination angle.
        /// </summary>
        public double Idot { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the dataset (ephemeris).
        /// </summary>
        /// <value>
        /// The identifier of the dataset.
        /// </value>
        public uint Iode { get; set; }

        /// <summary>
        /// Gets or sets the time of creation (TOC).
        /// </summary>
        /// <value>
        /// The time of creation.
        /// </value>
        public DateTime Toc { get; set; }

        /// <summary>
        /// Gets or sets the AF1 property.
        /// </summary>
        /// <value>
        /// The value of the AF1 property.
        /// </value>
        public double AF1 { get; set; }

        /// Gets or sets the value of AF2.
        /// /
        public double AF2 { get; set; }

        /// <summary>
        /// Gets or sets the amplitude of the sine harmonic correction to the orbit radius.
        /// </summary>
        /// <value>
        /// The amplitude of the sine harmonic correction to the orbit radius.
        /// </value>
        public double Crs { get; set; }

        /// <summary>
        /// Gets or sets the difference between the mean motion and the computed value.
        /// </summary>
        public double DeltaN { get; set; }

        /// <summary>
        /// Amplitude of the cosine harmonic correction to the argument of latitude.
        /// </summary>
        /// <value>
        /// The amplitude of the cosine harmonic correction to the argument of latitude.
        /// </value>
        public double Cuc { get; set; }

        /// <summary>
        /// Gets or sets the eccentricity.
        /// </summary>
        /// <value>
        /// The eccentricity.
        /// </value>
        public double E { get; set; }

        /// <summary>
        /// The amplitude of the sinusoidal harmonic correction to the latitude argument
        /// </summary>
        /// <remarks>
        /// This property represents the amplitude of the sinusoidal harmonic correction to the latitude argument.
        /// </remarks>
        /// <value>
        /// The amplitude of the sinusoidal harmonic correction to the latitude argument.
        /// </value>
        public short Cus { get; set; }

        /// <summary>
        /// Gets or sets the half of the major axis.
        /// </summary>
        /// <value>The half of the major axis.</value>
        public double A { get; set; }

        /// <summary>
        /// The time of ephemeris binding (Toes).
        /// </summary>
        /// <value>
        /// The time of ephemeris binding.
        /// </value>
        /// <remarks>
        /// This property represents the time of ephemeris binding. It is measured in ushort (16-bit unsigned integer) units.
        /// </remarks>
        public ushort Toes { get; set; }

        /// <summary>
        /// Reference time of binding (ephemeris)
        /// </summary>
        /// <value>
        /// Returns or sets the reference time of the binding.
        /// </value>
        /// <example>
        /// <code>
        /// var obj = new MyClass();
        /// DateTime toe = obj.Toe;
        /// obj.Toe = DateTime.UtcNow;
        /// </code>
        /// </example>
        public DateTime Toe { get; set; }

        /// <summary>
        /// Gets or sets the longitude of ascendent node of the orbital plane at weekly epoch.
        /// </summary>
        /// <value>
        /// The longitude of ascendent node of the orbital plane.
        /// </value>
        public double Omega0 { get; set; }

        /// <summary>
        /// Represents the amplitude of the cosine harmonic correction to the inclination angle.
        /// </summary>
        public double Cic { get; set; }

        /// <summary>
        /// Gets or sets the inclination angle at the time of binding.
        /// </summary>
        /// <value>
        /// The inclination angle.
        /// </value>
        public double I0 { get; set; }

        /// <summary>
        /// Amplitude of sine harmonic correction to the inclination angle
        /// </summary>
        public double Cis { get; set; }

        /// <summary>
        /// Gets or sets the argument of perigee.
        /// </summary>
        /// <value>
        /// The argument of perigee.
        /// </value>
        public double Omega { get; set; }

        /// <summary>
        /// Gets or sets the amplitude of the cosine harmonic correction to the orbit radius.
        /// </summary>
        /// <value>
        /// The amplitude of the cosine harmonic correction to the orbit radius.
        /// </value>
        public double Crc { get; set; }

        /// <summary>
        /// Gets or sets the rate of change of right ascension.
        /// </summary>
        /// <value>
        /// The rate of change of right ascension.
        /// </value>
        public double OmegaDot { get; set; }

        /// <summary>
        /// Gets or sets the average anomaly at the time of binding.
        /// </summary>
        /// <value>
        /// The average anomaly at the time of binding.
        /// </value>
        public double M0 { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the time parameters set.
        /// </summary>
        public ushort Iodc { get; set; }

        /// <summary>
        /// Gets or sets the temporal parameters of the almanac.
        /// </summary>
        public double AF0 { get; set; }

        /// <summary>
        /// Gets or sets the estimate of the total group delay.
        /// </summary>
        /// <value>
        /// The estimate of the total group delay.
        /// </value>
        public double Tgd { get; set; }

        /// Gets or sets the value of the CodeOnL2 property.
        /// </summary>
        /// <remarks>
        /// This property represents a byte value that specifies the code on level 2.
        /// The CodeOnL2 property can be used to store and retrieve information regarding the code on level 2.
        /// </remarks>
        public byte CodeOnL2 { get; set; }

        /// <summary>
        /// Gets or sets the SVAccuracy property.
        /// </summary>
        /// <value>
        /// The accuracy of SV (satellite vehicle) measurements, represented as a byte.
        /// </value>
        public byte SVAccuracy { get; set; }

        /// <summary>
        /// Gets or sets the SVHealth property.
        /// </summary>
        /// <value>
        /// The SVHealth property represents the health status of an object.
        /// It is a byte value that indicates the health level.
        /// A higher value represents a healthier state, whereas a lower value indicates a less healthy state.
        /// </value>
        public byte SVHealth { get; set; }

        /// <summary>
        /// Gets or sets the L2PDataFlag property.
        /// </summary>
        public byte L2PDataFlag { get; set; }

        /// <summary>
        /// Deserialize the content of the buffer into the object properties
        /// </summary>
        /// <param name="buffer">The buffer containing the serialized data</param>
        /// <param name="bitIndex">The starting bit index within the buffer</param>
        /// <param name="payloadLength">The length of the payload</param>
        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
        {
            var week = SpanBitHelper.GetBitU(buffer, ref bitIndex, 10); bitIndex += 10;
            WeekNumberRaw = week;
            Idot = SpanBitHelper.GetBitS(buffer, ref bitIndex, 14) * RtcmV3Helper.P2_43 * RtcmV3Helper.SC2RAD;
            Iode = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            var toc = SpanBitHelper.GetBitU(buffer, ref bitIndex, 16) * 16.0;
            AF1 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_43;
            AF2 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 8) * RtcmV3Helper.P2_55;
            Crs = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_5;
            DeltaN = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_43 * RtcmV3Helper.SC2RAD;
            Cuc = (short)SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_29;
            E = SpanBitHelper.GetBitU(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_33;
            Cus = (short)SpanBitHelper.GetBitS(buffer, ref bitIndex, 16);
            var sqrtA = SpanBitHelper.GetBitU(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_19;
            Toes = (ushort)SpanBitHelper.GetBitU(buffer, ref bitIndex, 16);
            Omega0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_31 * RtcmV3Helper.SC2RAD;
            Cic = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_29;
            I0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_31 * RtcmV3Helper.SC2RAD;
            Cis = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_29;
            Omega = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_31 * RtcmV3Helper.SC2RAD;
            Crc = SpanBitHelper.GetBitS(buffer, ref bitIndex, 16) * RtcmV3Helper.P2_5;
            OmegaDot = SpanBitHelper.GetBitS(buffer, ref bitIndex, 24) * RtcmV3Helper.P2_43 * RtcmV3Helper.SC2RAD;
            M0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32) * RtcmV3Helper.P2_31 * RtcmV3Helper.SC2RAD;
            Iodc = (ushort)SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
            AF0 = SpanBitHelper.GetBitS(buffer, ref bitIndex, 22) * RtcmV3Helper.P2_31;
            var prn = SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
            bitIndex += 3; // FILL
            Tgd = SpanBitHelper.GetBitS(buffer, ref bitIndex, 8) * RtcmV3Helper.P2_31; bitIndex += 8;
            CodeOnL2 = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 2); bitIndex += 2;
            SVAccuracy = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 4); bitIndex += 4;
            SVHealth = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 6); bitIndex += 6;
            L2PDataFlag = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1); bitIndex += 1;

            if (prn == 0) prn = 32;
            Satellite = RtcmV3Helper.satno(NavigationSystemEnum.SYS_GPS, (int)prn);

            WeekNumber = adjgpsweek((int)week);
            Toe = RtcmV3Helper.GetFromGps(WeekNumber, Toes);
            Toc = RtcmV3Helper.GetFromGps(WeekNumber, toc);
            A = sqrtA * sqrtA;
        }
    }
}