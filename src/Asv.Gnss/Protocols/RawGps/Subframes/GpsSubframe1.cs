using System;
using Newtonsoft.Json;

namespace Asv.Gnss;

/// <summary>
/// Represents a GPS subframe 1.
/// </summary>
public partial class GpsSubframe1 : GpsSubframeBase
{
    /// <summary>
    /// Gets the subframe identifier.
    /// </summary>
    /// <value>
    /// The subframe identifier.
    /// </value>
    public override byte SubframeId => 1;

    /// <summary>
    /// Deserialize the data to extract satellite information.
    /// </summary>
    /// <param name="dataWithoutParity">The byte array containing the data without parity.</param>
    public override void Deserialize(byte[] dataWithoutParity)
    {
        base.Deserialize(dataWithoutParity);
        var word1Start = 24U * 2;
        WeekNumber = (int)GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 10);
        word1Start += 10;
        Code = (int)GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 2);
        word1Start += 2;
        SatelliteAccuracy = (byte)GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 4);
        word1Start += 4;
        SatelliteHealth = (byte)GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 6);
        word1Start += 6;
        iodc = 0;
        iodc |= (int)GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 2) << 8;
        word1Start += 2;
        Flag = (int)GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 1);
        word1Start += 24U * 3 + 1 + 15;
        var tgd = GpsRawHelper.GetBitS(dataWithoutParity, word1Start, 8);
        word1Start += 8;
        iodc |= (int)GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 8);
        word1Start += 8;
        TocSec = GpsRawHelper.GetBitU(dataWithoutParity, word1Start, 16) * 16.0;
        word1Start += 16;
        Af2 = GpsRawHelper.GetBitS(dataWithoutParity, word1Start, 8) * GpsRawHelper.P2_55;
        word1Start += 8;
        Af1 = GpsRawHelper.GetBitS(dataWithoutParity, word1Start, 16) * GpsRawHelper.P2_43;
        word1Start += 16;
        Af0 = GpsRawHelper.GetBitS(dataWithoutParity, word1Start, 22) * GpsRawHelper.P2_31;
        word1Start += 24;

        Tgd[0] = tgd == -128 ? 0.0 : tgd * GpsRawHelper.P2_31;
    }

    private DateTime _toc;
    public DateTime Toc
    {
        get => _toc;
        set => _toc = value;
    }

    private int _flag;
    public int Flag
    {
        get => _flag;
        set => _flag = value;
    }

    private int _code;
    public int Code
    {
        get => _code;
        set => _code = value;
    }
    private double _af0;
    public double Af0
    {
        get => _af0;
        set => _af0 = value;
    }
    private double _af1;
    public double Af1
    {
        get => _af1;
        set => _af1 = value;
    }
    private double _af2;
    public double Af2
    {
        get => _af2;
        set => _af2 = value;
    }
    private double _tocSec;
    public double TocSec
    {
        get => _tocSec;
        set => _tocSec = value;
    }
    private int _iodc;
    public int iodc
    {
        get => _iodc;
        set => _iodc = value;
    }

    private double[] _tgd = new double[1];
    public double[] Tgd
    {
        get => _tgd;
        set => _tgd = value;
    }

    /// <summary>
    /// Gets or sets the satellite accuracy in byte value.
    /// </summary>
    private byte _satelliteAccuracy;
    public byte SatelliteAccuracy
    {
        get => _satelliteAccuracy;
        set => _satelliteAccuracy = value;
    }

    /// <summary>
    /// Gets or sets the health status of the satellite.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The raw six-bit health indication ranges from 0 to 63. Zero indicates healthy LNAV data
    /// and signals, as defined by IS-GPS-200N, section 20.3.3.3.1.4.
    /// </para>
    /// </remarks>
    /// <value>
    /// The health status of the satellite.
    /// </value>
    private byte _satelliteHealth;
    public byte SatelliteHealth
    {
        get => _satelliteHealth;
        set => _satelliteHealth = value;
    }

    /// <summary>
    /// Gets or sets the week number.
    /// </summary>
    /// <value>
    /// The week number.
    /// </value>
    private int _weekNumber;
    public int WeekNumber
    {
        get => _weekNumber;
        set => _weekNumber = value;
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
