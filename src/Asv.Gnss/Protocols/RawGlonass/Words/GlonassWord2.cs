namespace Asv.Gnss;

/// <summary>
/// Represents a Glonass Word 2 object.
/// </summary>
public class GlonassWord2 : GlonassWordBase
{
    /// <summary>
    /// Gets the ID of the word.
    /// </summary>
    /// <value>
    /// The ID of the word.
    /// </value>
    public override byte WordId => 2;

    /// <summary>
    /// Deserializes the given byte array and assigns the extracted values to the corresponding properties.
    /// </summary>
    /// <param name="data">The byte array to deserialize.</param>
    public override void Deserialize(byte[] data)
    {
        base.Deserialize(data);
        var bitIndex = 8U;

        Health = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 3);
        bitIndex += 3;
        P2 = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 1);
        bitIndex += 1;
        Tb = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 7);
        bitIndex += 7 + 5;

        VelocityY = GlonassRawHelper.GetBitG(data, bitIndex, 24) * GlonassRawHelper.P2_20 * 1E3;
        bitIndex += 24;
        AccelerationY = GlonassRawHelper.GetBitG(data, bitIndex, 5) * GlonassRawHelper.P2_30 * 1E3;
        bitIndex += 5;
        PositionY = GlonassRawHelper.GetBitG(data, bitIndex, 27) * GlonassRawHelper.P2_11 * 1E3;
        bitIndex += 27;
    }

    public byte Tb { get; set; }

    public byte Health { get; set; }

    /// <summary>
    /// Gets or sets the value of property P2.
    /// </summary>
    /// <remarks>
    /// This property is of type byte and can store values ranging from 0 to 255.
    /// </remarks>
    /// <value>The value of property P2.</value>
    public byte P2 { get; set; }

    /// <summary>
    /// satellite position (ecef) (m). Координаты n-го спутника в системе координат ПЗ-90 на момент времени tb
    /// </summary>
    public double PositionY { get; set; }

    /// <summary>
    /// satellite velocity (ecef) (m/s). составляющие вектора скорости n-го спутника в системе координат ПЗ-90 на момент
    /// времени tb
    /// </summary>
    public double VelocityY { get; set; }

    /// <summary>
    /// satellite acceleration (ecef) (m/s^2). Составляющие ускорения n-го спутника в системе координат ПЗ-90 на момент времени tb,
    /// обусловленные действием луны и солнца
    /// </summary>
    public double AccelerationY { get; set; }

}