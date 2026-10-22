using System;

namespace Asv.Gnss;

/// <summary>
/// Represents a GPS Subframe 2.
/// </summary>
public partial class GpsSubframe2 : GpsSubframeBase
{
    /// <summary>
    /// Gets the subframe identifier.
    /// </summary>
    /// <value>
    /// The subframe identifier.
    /// </value>
    public override byte SubframeId => 2;

    /// <summary>
    /// Deserializes the given byte array without parity.
    /// </summary>
    /// <param name="dataWithoutParity">The byte array to deserialize.</param>
    public override void Deserialize(byte[] dataWithoutParity)
    {
        base.Deserialize(dataWithoutParity);
        var word2Start = 24U * 2;
        iode = (int)GpsRawHelper.GetBitU(dataWithoutParity, word2Start, 8);
        word2Start += 8;
        Crs = GpsRawHelper.GetBitS(dataWithoutParity, word2Start, 16) * GpsRawHelper.P2_5;
        word2Start += 16;
        deln =
            GpsRawHelper.GetBitS(dataWithoutParity, word2Start, 16)
            * GpsRawHelper.P2_43
            * GpsRawHelper.SC2RAD;
        word2Start += 16;
        M0 =
            GpsRawHelper.GetBitS(dataWithoutParity, word2Start, 32)
            * GpsRawHelper.P2_31
            * GpsRawHelper.SC2RAD;
        word2Start += 32;
        Cuc = GpsRawHelper.GetBitS(dataWithoutParity, word2Start, 16) * GpsRawHelper.P2_29;
        word2Start += 16;
        e = GpsRawHelper.GetBitU(dataWithoutParity, word2Start, 32) * GpsRawHelper.P2_33;
        word2Start += 32;
        Cus = GpsRawHelper.GetBitS(dataWithoutParity, word2Start, 16) * GpsRawHelper.P2_29;
        word2Start += 16;
        A = Math.Pow(
            GpsRawHelper.GetBitU(dataWithoutParity, word2Start, 32) * GpsRawHelper.P2_19,
            2
        );
        word2Start += 32;
        ToeSec = GpsRawHelper.GetBitU(dataWithoutParity, word2Start, 16) * 16.0;
        word2Start += 16;
        Fit = GpsRawHelper.GetBitU(dataWithoutParity, word2Start, 1) == 0 ? 0.0 : 4.0;
        word2Start++;
        Aodo = (byte)GpsRawHelper.GetBitU(dataWithoutParity, word2Start, 5);
    }

    private byte _aodo;

    /// <summary>Age of data offset in 900-second units; the five-bit value 31 indicates invalid NMCT data.</summary>
    public byte Aodo
    {
        get => _aodo;
        set => _aodo = value;
    }

    private double _fit;
    public double Fit
    {
        get => _fit;
        set => _fit = value;
    }

    private double _m0;
    public double M0
    {
        get => _m0;
        set => _m0 = value;
    }
    private double _deln;
    public double deln
    {
        get => _deln;
        set => _deln = value;
    }
    private double _e;
    public double e
    {
        get => _e;
        set => _e = value;
    }
    private double _a;
    public double A
    {
        get => _a;
        set => _a = value;
    }

    private double _cuc;
    public double Cuc
    {
        get => _cuc;
        set => _cuc = value;
    }
    private double _cus;
    public double Cus
    {
        get => _cus;
        set => _cus = value;
    }
    private double _crs;
    public double Crs
    {
        get => _crs;
        set => _crs = value;
    }

    private double _toeSec;
    public double ToeSec
    {
        get => _toeSec;
        set => _toeSec = value;
    }
    private int _iode;
    public int iode
    {
        get => _iode;
        set => _iode = value;
    }
}
