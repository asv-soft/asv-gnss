namespace Asv.Gnss;

/// <summary>
/// Represents a GPS Subframe 3.
/// </summary>
public partial class GpsSubframe3 : GpsSubframeBase
{
    /// <summary>
    /// Gets the Id of the subframe.
    /// </summary>
    /// <value>
    /// The Id of the subframe.
    /// </value>
    public override byte SubframeId => 3;

    /// <summary>
    /// Deserializes the given data without parity.
    /// </summary>
    /// <param name="dataWithoutParity">The data to be deserialized.</param>
    public override void Deserialize(byte[] dataWithoutParity)
    {
        base.Deserialize(dataWithoutParity);
        var word3Start = 24U * 2;
        Cic = GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 16) * GpsRawHelper.P2_29;
        word3Start += 16;
        OMG0 =
            GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 32)
            * GpsRawHelper.P2_31
            * GpsRawHelper.SC2RAD;
        word3Start += 32;
        Cis = GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 16) * GpsRawHelper.P2_29;
        word3Start += 16;
        i0 =
            GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 32)
            * GpsRawHelper.P2_31
            * GpsRawHelper.SC2RAD;
        word3Start += 32;
        Crc = GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 16) * GpsRawHelper.P2_5;
        word3Start += 16;
        omg =
            GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 32)
            * GpsRawHelper.P2_31
            * GpsRawHelper.SC2RAD;
        word3Start += 32;
        OMGd =
            GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 24)
            * GpsRawHelper.P2_43
            * GpsRawHelper.SC2RAD;
        word3Start += 24;
        iode = (int)GpsRawHelper.GetBitU(dataWithoutParity, word3Start, 8);
        word3Start += 8;
        idot =
            GpsRawHelper.GetBitS(dataWithoutParity, word3Start, 14)
            * GpsRawHelper.P2_43
            * GpsRawHelper.SC2RAD;
        word3Start += 14;
    }

    private int _iode;
    public int iode
    {
        get => _iode;
        set => _iode = value;
    }

    private double _oMG0;
    public double OMG0
    {
        get => _oMG0;
        set => _oMG0 = value;
    }
    private double _i0;
    public double i0
    {
        get => _i0;
        set => _i0 = value;
    }
    private double _omg;
    public double omg
    {
        get => _omg;
        set => _omg = value;
    }
    private double _oMGd;
    public double OMGd
    {
        get => _oMGd;
        set => _oMGd = value;
    }
    private double _idot;
    public double idot
    {
        get => _idot;
        set => _idot = value;
    }

    private double _crc;
    public double Crc
    {
        get => _crc;
        set => _crc = value;
    }
    private double _cic;
    public double Cic
    {
        get => _cic;
        set => _cic = value;
    }
    private double _cis;
    public double Cis
    {
        get => _cis;
        set => _cis = value;
    }
}
