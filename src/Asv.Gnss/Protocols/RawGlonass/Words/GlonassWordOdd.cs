using System;

namespace Asv.Gnss;

/// <summary>
/// This class inherits from the GlonassWordBase class and represents a Glonass word with an odd word ID.
/// </summary>
public partial class GlonassWordOdd : GlonassWordBase
{
    /// <summary>
    /// Checks if the word ID meets the specified criteria.
    /// </summary>
    /// <param name="wordId">The word ID to be checked.</param>
    protected override void CheckWordId(byte wordId)
    {
        if (wordId <= 5 || wordId % 2 != 1)
            throw new Exception($"Word ID not equals: Word want > 5 and odd number. Got {wordId}");
    }

    /// <summary>
    /// Deserializes the provided byte array.
    /// </summary>
    /// <param name="data">The byte array containing the serialized data.</param>
    public override void Deserialize(byte[] data)
    {
        base.Deserialize(data);
        var bitIndex = 8U;
        omega = GlonassRawHelper.GetBitG(data, bitIndex, 16) * GlonassRawHelper.P2_15 * Math.PI;
        bitIndex += 16;
        ToaSec = GlonassRawHelper.GetBitU(data, bitIndex, 21) * GlonassRawHelper.P2_5;
        bitIndex += 21;
        DeltaT = GlonassRawHelper.GetBitG(data, bitIndex, 22) * GlonassRawHelper.P2_9;
        bitIndex += 22;
        DeltaDT = GlonassRawHelper.GetBitG(data, bitIndex, 7) * GlonassRawHelper.P2_14;
        bitIndex += 7;
        var h = (int)GlonassRawHelper.GetBitU(data, bitIndex, 5);
        bitIndex += 5;
        if (h is >= 25 and <= 31)
            h -= 32;
        Frequency = 1602000000 + h * 562500;
        ln = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 1);
    }

    /// <summary>Immediate satellite health flag: zero indicates usable navigation data.</summary>
    public byte ln
    {
        get => _ln;
        set => _ln = value;
    }
    private byte _ln;

    /// <summary>
    ///
    /// </summary>
    public double omega
    {
        get => _omega;
        set => _omega = value;
    }
    private double _omega;

    /// <summary>
    ///
    /// </summary>
    public double ToaSec
    {
        get => _toaSec;
        set => _toaSec = value;
    }
    private double _toaSec;

    /// <summary>
    ///
    /// </summary>
    public double DeltaT
    {
        get => _deltaT;
        set => _deltaT = value;
    }
    private double _deltaT;

    /// <summary>
    ///
    /// </summary>
    public double DeltaDT
    {
        get => _deltaDT;
        set => _deltaDT = value;
    }
    private double _deltaDT;

    /// <summary>
    ///
    /// </summary>
    public long Frequency
    {
        get => _frequency;
        set => _frequency = value;
    }
    private long _frequency;
}
