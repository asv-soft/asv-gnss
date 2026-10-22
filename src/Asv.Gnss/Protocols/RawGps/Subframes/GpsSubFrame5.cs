using System;

namespace Asv.Gnss;

/// <summary>
/// Represents the GPS SubFrame 5.
/// </summary>
public partial class GpsSubFrame5 : GpsSubframeBase
{
    /// <summary>
    /// Gets the Subframe Id.
    /// </summary>
    /// <value>
    /// The Subframe Id value.
    /// </value>
    public override byte SubframeId => 5;

    public enum Subframe5Type
    {
        Almanac,
        ToaAndHealth,
        None,
    }

    /// <summary>
    /// Deserializes the provided data without parity.
    /// </summary>
    /// <param name="dataWithoutParity">The data to be deserialized.</param>
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
            _type = Subframe5Type.None;
            return;
        }

        // page number
        var svid = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 6);
        word4Start += 6;

        if (svid is >= 1 and <= 24) /* page 1-24 */
        {
            _type = Subframe5Type.Almanac;
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

        if (svid == 0x33) /* page 25 */
        {
            _type = Subframe5Type.ToaAndHealth;
            ToaSec = GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8) * 4096.0;
            word4Start += 8;
            Week = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 8);
            word4Start += 8;

            /* decode sv health */
            Health = new int[24];
            for (var sat = 1; sat <= 24; sat++)
            {
                Health[sat - 1] = (int)GpsRawHelper.GetBitU(dataWithoutParity, word4Start, 6);
                word4Start += 6;
            }

            return;
        }

        _type = Subframe5Type.None;
    }

    private byte _dataId;
    private byte _pageId;
    private readonly byte[] _rawPageData = new byte[23];

    /// <summary>Transmitted two-bit data ID. Changing it reinterprets the current page payload.</summary>
    public byte DataId
    {
        get => Type == Subframe5Type.None ? _dataId : (byte)1;
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
                Subframe5Type.Almanac => SatelliteNumber is >= 0 and <= 63
                    ? (byte)SatelliteNumber
                    : _pageId,
                Subframe5Type.ToaAndHealth => 51,
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
        get => Type == Subframe5Type.None ? _rawPageData : null;
        set
        {
            if (Type != Subframe5Type.None)
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
            Type != Subframe5Type.None
            && !(Type == Subframe5Type.Almanac && SatelliteNumber == 0 && Health is null)
        )
            SerializeFields(source);
        RawGpsCodec.Unsigned(source, 48, 2, dataId);
        RawGpsCodec.Unsigned(source, 50, 6, pageId);
        DeserializePage(source);
        RetainPagePayload(source.AsSpan(6));
    }

    private Subframe5Type _type;
    public Subframe5Type Type
    {
        get => _type;
        set
        {
            if (
                value == Subframe5Type.None
                && _type != Subframe5Type.None
                && !(_type == Subframe5Type.Almanac && SatelliteNumber == 0 && Health is null)
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
    /// For Type=ToaAndHealth, length=24 Sv [1:24]
    /// </summary>
    private int[] _health = null!;
    public int[] Health
    {
        get => _health;
        set => _health = value;
    }

    /// <summary>
    /// GPS/QZS: gps week for all 32 satellites
    /// </summary>
    private int _week;
    public int Week
    {
        get => _week;
        set => _week = value;
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
