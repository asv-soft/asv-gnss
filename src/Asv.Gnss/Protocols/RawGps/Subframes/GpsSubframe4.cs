using System;

namespace Asv.Gnss;

/// <summary>
/// Represents the GPS Subframe 4.
/// </summary>
public partial class GpsSubframe4 : GpsSubframeBase
{
    public enum Subframe4Type
    {
        Almanac,
        IonoAndUtcData,
        HealthAndConfigurations,
        None,
        Nmct,
        SpecialMessage,
    }

    public partial class Ionospheric
    {
        private double _alpha0;
        public double Alpha0
        {
            get => _alpha0;
            set => _alpha0 = value;
        }
        private double _alpha1;
        public double Alpha1
        {
            get => _alpha1;
            set => _alpha1 = value;
        }
        private double _alpha2;
        public double Alpha2
        {
            get => _alpha2;
            set => _alpha2 = value;
        }
        private double _alpha3;
        public double Alpha3
        {
            get => _alpha3;
            set => _alpha3 = value;
        }

        private double _betta0;
        public double Betta0
        {
            get => _betta0;
            set => _betta0 = value;
        }
        private double _betta1;
        public double Betta1
        {
            get => _betta1;
            set => _betta1 = value;
        }
        private double _betta2;
        public double Betta2
        {
            get => _betta2;
            set => _betta2 = value;
        }
        private double _betta3;
        public double Betta3
        {
            get => _betta3;
            set => _betta3 = value;
        }
    }

    public partial class UtcGpsRelationship
    {
        /// <summary>
        /// constant of polynomial
        /// </summary>
        private double _a0;
        public double A0
        {
            get => _a0;
            set => _a0 = value;
        }

        /// <summary>
        /// first order terms of polynomial;
        /// </summary>
        private double _a1;
        public double A1
        {
            get => _a1;
            set => _a1 = value;
        }

        /// <summary>
        /// reference time for UTC data
        /// </summary>
        private double _tot;
        public double Tot
        {
            get => _tot;
            set => _tot = value;
        }

        /// <summary>
        /// UTC reference week number
        /// </summary>
        private int _wNt;
        public int WNt
        {
            get => _wNt;
            set => _wNt = value;
        }

        /// <summary>
        /// delta time due to leap seconds
        /// </summary>
        private double _deltaT_LS;
        public double DeltaT_LS
        {
            get => _deltaT_LS;
            set => _deltaT_LS = value;
        }

        private int _wN_LSF;

        /// <summary>Eight low bits of the week in which the leap-second transition occurs.</summary>
        public int WN_LSF
        {
            get => _wN_LSF;
            set => _wN_LSF = value;
        }
        private int _dN;

        /// <summary>Day number of the leap-second transition; the transmitted field is retained verbatim.</summary>
        public int DN
        {
            get => _dN;
            set => _dN = value;
        }
        private int _deltaT_LSF;

        /// <summary>Signed GPS to UTC leap-second offset after the transition, in seconds.</summary>
        public int DeltaT_LSF
        {
            get => _deltaT_LSF;
            set => _deltaT_LSF = value;
        }

        /// <summary>
        /// UTC/GPS-time relationship
        /// </summary>
        /// <param name="te">GPS time as estimated by the user on the basis of correcting tsv for factors
        /// described in paragraph 2.5.5.2 (SPS) as well as for ionospheric and SA (dither) effects</param>
        /// <param name="wn">Current week number (derived from subframe 1)</param>
        /// <returns></returns>
        public double GetUtc(double te, int wn)
        {
            var deltaTUtc = DeltaT_LS + A0 + A1 * (te - Tot + 604800 * (wn - WNt));
            GpsRawHelper.Gps2Time(wn, deltaTUtc % 604800.0);
            return (te - deltaTUtc) % 86400.0;
        }
    }

    /// <summary>
    /// Gets the ID of the subframe.
    /// </summary>
    /// <value>
    /// The ID of the subframe.
    /// </value>
    public override byte SubframeId => 4;

    /// <summary>
    /// Deserializes data without parity.
    /// </summary>
    /// <param name="dataWithoutParity">The byte array of data to be deserialized.</param>
    public override void Deserialize(byte[] dataWithoutParity)
    {
        base.Deserialize(dataWithoutParity);
        DeserializePage(dataWithoutParity);
    }

    private void DeserializePage(byte[] dataWithoutParity)
    {
        ResetPage();
        dataWithoutParity.AsSpan(7).CopyTo(_rawPageData);
        _dataId = (byte)GpsRawHelper.GetBitU(dataWithoutParity, 48, 2);
        _pageId = (byte)GpsRawHelper.GetBitU(dataWithoutParity, 50, 6);
        var word4Start = 24U * 2;

        var dataId = GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 2);
        word4Start += 2;

        if (dataId != 1) // GPS data/ If dataId == 3 => QZSS data
        {
            _type = Subframe4Type.None;
            return;
        }

        // page number
        var svid = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 6);
        word4Start += 6;

        if (svid is >= 25 and <= 32) // page 2,3,4,5,7,8,9,10
        {
            _type = Subframe4Type.Almanac;
            SatelliteNumber = svid;
            e = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 16) * GpsRawHelper.P2_21;
            word4Start += 16;
            ToaSec = GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8) * 4096.0;
            word4Start += 8;
            i0 =
                (0.3 + GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 16) * GpsRawHelper.P2_19)
                * GpsRawHelper.SC2RAD;
            word4Start += 16;
            OMGd =
                GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 16)
                * GpsRawHelper.P2_38
                * GpsRawHelper.SC2RAD;
            word4Start += 16;
            Health = new[] { (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8) };
            word4Start += 8;
            A = Math.Pow(
                GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 24) * GpsRawHelper.P2_11,
                2
            );
            word4Start += 24;
            OMG0 =
                GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 24)
                * GpsRawHelper.P2_23
                * GpsRawHelper.SC2RAD;
            word4Start += 24;
            omg =
                GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 24)
                * GpsRawHelper.P2_23
                * GpsRawHelper.SC2RAD;
            word4Start += 24;
            M0 =
                GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 24)
                * GpsRawHelper.P2_23
                * GpsRawHelper.SC2RAD;
            word4Start += 24;
            var af0 = GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8);
            word4Start += 8;
            Af1 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 11) * GpsRawHelper.P2_38;
            word4Start += 11;
            Af0 =
                GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 3) * GpsRawHelper.P2_17
                + af0 * GpsRawHelper.P2_20;

            return;
        }

        if (svid == 0x3F) // Page 25
        {
            _type = Subframe4Type.HealthAndConfigurations;

            /* decode as and sv config */
            SvConfig = new int[32];
            for (var sat = 1; sat <= 32; sat++)
            {
                SvConfig[sat - 1] = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 4);
                word4Start += 4;
            }

            word4Start += 2;
            /* decode sv health */
            Health = new int[8];
            for (var sat = 25; sat <= 32; sat++)
            {
                Health[sat - 25] = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 6);
                word4Start += 6;
            }

            return;
        }

        if (svid == 0x38) // Page 18
        {
            _type = Subframe4Type.IonoAndUtcData;
            Iono = new Ionospheric();
            Iono.Alpha0 =
                GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * GpsRawHelper.P2_30;
            word4Start += 8;
            Iono.Alpha1 =
                GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * GpsRawHelper.P2_27;
            word4Start += 8;
            Iono.Alpha2 =
                GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * GpsRawHelper.P2_24;
            word4Start += 8;
            Iono.Alpha3 =
                GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * GpsRawHelper.P2_24;
            word4Start += 8;

            Iono.Betta0 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * Math.Pow(2, 11);
            word4Start += 8;
            Iono.Betta1 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * Math.Pow(2, 14);
            word4Start += 8;
            Iono.Betta2 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * Math.Pow(2, 16);
            word4Start += 8;
            Iono.Betta3 = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * Math.Pow(2, 16);
            word4Start += 8;

            UtcGps = new UtcGpsRelationship();

            UtcGps.A1 =
                GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 24) * GpsRawHelper.P2_50;
            word4Start += 24;
            UtcGps.A0 =
                GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 32) * GpsRawHelper.P2_30;
            word4Start += 32;
            UtcGps.Tot = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8) * Math.Pow(2, 12);
            word4Start += 8;
            UtcGps.WNt = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8);
            word4Start += 8;
            UtcGps.DeltaT_LS = GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8);
            word4Start += 8;
            UtcGps.WN_LSF = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8);
            word4Start += 8;
            UtcGps.DN = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8);
            word4Start += 8;
            UtcGps.DeltaT_LSF = GpsRawHelper.GetBitS(dataWithoutParity, word4Start, 8);

            return;
        }

        if (svid == 52)
        {
            _type = Subframe4Type.Nmct;
            NmctAvailabilityIndicator = (byte)GpsRawHelper.GetBitU(dataWithoutParity, 56, 2);
            NmctEstimatedRangeDeviations = new int[30];
            for (var index = 0; index < 30; index++)
                NmctEstimatedRangeDeviations[index] = GpsRawHelper.GetBitS(
                    dataWithoutParity,
                    (uint)(58 + index * 6),
                    6
                );
            return;
        }

        if (svid == 55)
        {
            _type = Subframe4Type.SpecialMessage;
            SpecialMessageData = dataWithoutParity[7..29];
            return;
        }

        _type = Subframe4Type.None;
    }

    private byte _nmctAvailabilityIndicator;

    /// <summary>NMCT availability: 0 clear, 1 encrypted, 2 unavailable, 3 reserved/unavailable.</summary>
    public byte NmctAvailabilityIndicator
    {
        get => _nmctAvailabilityIndicator;
        set => _nmctAvailabilityIndicator = value;
    }
    private int[] _nmctEstimatedRangeDeviations = null!;

    /// <summary>
    /// Thirty signed six-bit wire values. For clear NMCT, one unit is 0.3 meters and -32 means unavailable.
    /// Encrypted or unavailable slots are retained without interpreting their physical meaning.
    /// </summary>
    public int[] NmctEstimatedRangeDeviations
    {
        get => _nmctEstimatedRangeDeviations;
        set => _nmctEstimatedRangeDeviations = value;
    }
    private byte[] _specialMessageData = null!;

    /// <summary>The exact 22 transmitted special-message character bytes; the degree character is 0xF8.</summary>
    public byte[] SpecialMessageData
    {
        get => _specialMessageData;
        set => _specialMessageData = value;
    }

    private byte _dataId;
    private byte _pageId;
    private readonly byte[] _rawPageData = new byte[23];

    /// <summary>Transmitted two-bit data ID. Changing it reinterprets the current page payload.</summary>
    public byte DataId
    {
        get => Type == Subframe4Type.None ? _dataId : (byte)1;
        set
        {
            if (value > 3)
                throw new ArgumentOutOfRangeException(nameof(value));
            ChangePageIdentity(value, PageId);
        }
    }

    /// <summary>
    /// Transmitted six-bit SV/page ID, not the page's position in the 25-page cycle.
    /// An out-of-width legacy SatelliteNumber leaves the retained ID visible; serialization still validates that value.
    /// </summary>
    public byte PageId
    {
        get =>
            Type switch
            {
                Subframe4Type.Almanac => SatelliteNumber is >= 0 and <= 63
                    ? (byte)SatelliteNumber
                    : _pageId,
                Subframe4Type.HealthAndConfigurations => 63,
                Subframe4Type.IonoAndUtcData => 56,
                Subframe4Type.Nmct => 52,
                Subframe4Type.SpecialMessage => 55,
                _ => _pageId,
            };
        set
        {
            if (value > 63)
                throw new ArgumentOutOfRangeException(nameof(value));
            ChangePageIdentity(DataId, value);
        }
    }

    /// <summary>
    /// The 23 parity-stripped bytes after the page ID, including the final two parity-solving bits.
    /// Available only for an unmodeled page; changing the identity reinterprets these bytes as the selected layout.
    /// </summary>
    public byte[]? RawPageData
    {
        get => Type == Subframe4Type.None ? _rawPageData : null;
        set
        {
            if (Type != Subframe4Type.None)
                throw new InvalidOperationException("Raw page data requires an unmodeled page.");
            ArgumentNullException.ThrowIfNull(value);
            GpsNavigationVisitor.RequireLength(value, 23, nameof(value));
            value.CopyTo(_rawPageData, 0);
        }
    }

    private void ChangePageIdentity(byte dataId, byte pageId)
    {
        if (DataId == dataId && PageId == pageId)
            return;
        var source = new byte[30];
        _rawPageData.CopyTo(source, 7);
        // A fresh legacy almanac object has no active payload until its fields or identity are supplied.
        if (
            Type != Subframe4Type.None
            && !(Type == Subframe4Type.Almanac && SatelliteNumber == 0 && Health is null)
        )
            SerializeFields(source);
        RawGpsCodec.Unsigned(source, 48, 2, dataId);
        RawGpsCodec.Unsigned(source, 50, 6, pageId);
        DeserializePage(source);
        RetainPagePayload(source.AsSpan(6));
    }

    private Subframe4Type _type;
    public Subframe4Type Type
    {
        get => _type;
        set
        {
            if (
                value == Subframe4Type.None
                && _type != Subframe4Type.None
                && !(_type == Subframe4Type.Almanac && SatelliteNumber == 0 && Health is null)
            )
            {
                var source = new byte[30];
                _rawPageData.CopyTo(source, 7);
                try
                {
                    SerializeFields(source);
                    source.AsSpan(7).CopyTo(_rawPageData);
                }
                catch (ArgumentException)
                {
                    // Legacy Type assignments also allow incomplete or out-of-range fields.
                    // Retain the last wire payload until the caller supplies a serializable layout.
                }
            }
            _dataId = DataId;
            _pageId = PageId;
            _type = value;
        }
    }

    private Ionospheric _iono = null!;
    public Ionospheric Iono
    {
        get => _iono;
        set => _iono = value;
    }

    /// <summary>
    /// UTC/GPS-time relationship
    /// </summary>
    private UtcGpsRelationship _utcGps = null!;
    public UtcGpsRelationship UtcGps
    {
        get => _utcGps;
        set => _utcGps = value;
    }

    /// <summary>
    /// Satellite number
    /// </summary>
    private int _satelliteNumber;
    public int SatelliteNumber
    {
        get => _satelliteNumber;
        set => _satelliteNumber = value;
    }

    /// <summary>
    /// sv health (0:ok).
    /// For Type=Almanac, length=1 (current satellite)
    /// For Type=HealthAndConfigurations, length=8 Sv [25:32]
    /// </summary>
    private int[] _health = null!;
    public int[] Health
    {
        get => _health;
        set => _health = value;
    }

    /// <summary>
    /// as and sv config. For Sv [1:32]
    /// </summary>
    private int[] _svConfig = null!;
    public int[] SvConfig
    {
        get => _svConfig;
        set => _svConfig = value;
    }

    #region SV orbit parameters

    private double _a;
    public double A
    {
        get => _a;
        set => _a = value;
    }
    private double _e;
    public double e
    {
        get => _e;
        set => _e = value;
    }
    private double _i0;
    public double i0
    {
        get => _i0;
        set => _i0 = value;
    }
    private double _oMG0;
    public double OMG0
    {
        get => _oMG0;
        set => _oMG0 = value;
    }
    private double _omg;
    public double omg
    {
        get => _omg;
        set => _omg = value;
    }
    private double _m0;
    public double M0
    {
        get => _m0;
        set => _m0 = value;
    }
    private double _oMGd;
    public double OMGd
    {
        get => _oMGd;
        set => _oMGd = value;
    }

    #endregion

    /// <summary>
    /// Almanac time (s) in week
    /// </summary>
    private double _toaSec;
    public double ToaSec
    {
        get => _toaSec;
        set => _toaSec = value;
    }

    /// <summary>
    /// SV clock parameters af0
    /// </summary>
    private double _af0;
    public double Af0
    {
        get => _af0;
        set => _af0 = value;
    }

    /// <summary>
    /// SV clock parameters af1
    /// </summary>
    private double _af1;
    public double Af1
    {
        get => _af1;
        set => _af1 = value;
    }
}
