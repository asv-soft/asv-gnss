using System;

namespace Asv.Gnss;

/// <summary>
/// Represents a derived class of GlonassWordBase that checks for even word IDs greater than 5 during deserialization.
/// </summary>
public partial class GlonassWordEven : GlonassWordBase
{
    /// <summary>
    /// Checks the word ID to ensure it meets certain criteria.
    /// </summary>
    /// <param name="wordId">The word ID to be checked.</param>
    /// <exception cref="Exception">Thrown when the word ID is less than or equal to 5 or not an even number.</exception>
    protected override void CheckWordId(byte wordId)
    {
        if (wordId <= 5 || wordId % 2 != 0)
            throw new Exception($"Word ID not equals: Word want > 5 and even number. Got {wordId}");
    }

    /// <summary>
    /// Deserializes the provided byte array and populates the object's state.
    /// </summary>
    /// <param name="data">The byte array containing the serialized data.</param>
    public override void Deserialize(byte[] data)
    {
        base.Deserialize(data);
        var bitIndex = 8U;

        С = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 1);
        bitIndex += 1;
        M = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 2);
        bitIndex += 2;
        Prn = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 5);
        bitIndex += 5;
        DelaT2 = GlonassRawHelper.GetBitG(data, bitIndex, 10) * GlonassRawHelper.P2_18;
        bitIndex += 10;
        LOmega = GlonassRawHelper.GetBitG(data, bitIndex, 21) * GlonassRawHelper.P2_20 * Math.PI;
        bitIndex += 21;
        i =
            (63.0 / 180.0 + GlonassRawHelper.GetBitG(data, bitIndex, 18) * GlonassRawHelper.P2_20)
            * Math.PI;
        bitIndex += 18;
        e = GlonassRawHelper.GetBitU(data, bitIndex, 15) * GlonassRawHelper.P2_20;
        bitIndex += 15;
    }

    /// <summary>
    /// 0 - non-operability
    /// 1 - operability
    /// </summary>
    public byte С
    {
        get => _с;
        set => _с = value;
    }
    private byte _с;

    /// <summary>
    /// 0 - GLONASS
    /// 1 - GLONASS-M
    /// </summary>
    public byte M
    {
        get => _m;
        set => _m = value;
    }
    private byte _m;

    /// <summary>
    /// Satellite number
    /// [1-24]
    /// </summary>
    public byte Prn
    {
        get => _prn;
        set => _prn = value;
    }
    private byte _prn;

    /// <summary>
    /// Correction to onboard time scale
    /// [s]
    /// </summary>
    public double DelaT2
    {
        get => _delaT2;
        set => _delaT2 = value;
    }
    private double _delaT2;

    /// <summary>
    /// Longitude of the first (within the NA -day) ascending node of orbit in PZ-90.02 coordinate system
    /// [Rad]
    /// </summary>
    public double LOmega
    {
        get => _lOmega;
        set => _lOmega = value;
    }
    private double _lOmega;

    /// <summary>
    /// Orbital inclination
    /// [Rad]
    /// </summary>
    public double i
    {
        get => _i;
        set => _i = value;
    }
    private double _i;

    /// <summary>
    /// Eccentricity
    /// </summary>
    public double e
    {
        get => _e;
        set => _e = value;
    }
    private double _e;
}
