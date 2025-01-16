namespace Asv.Gnss
{
    /// <summary>
    /// Represents a Glonass Word 3 message.
    /// </summary>
    public class GlonassWord3 : GlonassWordBase
    {
        /// <summary>
        /// Gets the identifier of the word.
        /// </summary>
        /// <value>
        /// The identifier of the word.
        /// </value>
        public override byte WordId => 3;

        /// <summary>
        /// Deserializes the byte array data.
        /// </summary>
        /// <param name="data">The byte array data to deserialize.</param>
        public override void Deserialize(byte[] data)
        {
            base.Deserialize(data);
            var bitIndex = 8U;

            P3 = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 1);
            bitIndex++;
            GamN = GlonassRawHelper.GetBitG(data, bitIndex, 11) * GlonassRawHelper.P2_40;
            bitIndex += 11 + 1;
            P = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 2);
            bitIndex += 2;
            Ln = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 1);
            bitIndex++;

            VelocityZ = GlonassRawHelper.GetBitG(data, bitIndex, 24) * GlonassRawHelper.P2_20 * 1E3;
            bitIndex += 24;
            AccelerationZ =
                GlonassRawHelper.GetBitG(data, bitIndex, 5) * GlonassRawHelper.P2_30 * 1E3;
            bitIndex += 5;
            PositionZ = GlonassRawHelper.GetBitG(data, bitIndex, 27) * GlonassRawHelper.P2_11 * 1E3;
            bitIndex += 27;
        }

        public byte P3 { get; set; }

        /// <summary>
        /// Gets or sets relative freq bias (γn(tb)). Относительное отклонение прогнозируемого значения несущей частоты излучаемого сигнала n-го
        /// спутника от номинального значения на момент времени tb.
        /// </summary>
        public double GamN { get; set; }

        /// <summary>
        /// Gets or sets признак режима работы НКА по ЧВИ.
        /// </summary>
        public byte P { get; set; }

        /// <summary>
        /// Gets or sets признак недостоверности кадра n-го НКА; ln = 0 свидетельствует о пригодности НКА для навигации;  ln = 1 означает факт непригодности данного НКА для навигации.
        /// </summary>
        public byte Ln { get; set; }

        /// <summary>
        /// Gets or sets satellite position (ecef) (m). Координаты n-го спутника в системе координат ПЗ-90 на момент времени tb.
        /// </summary>
        public double PositionZ { get; set; }

        /// <summary>
        /// Gets or sets satellite velocity (ecef) (m/s). составляющие вектора скорости n-го спутника в системе координат ПЗ-90 на момент
        /// времени tb.
        /// </summary>
        public double VelocityZ { get; set; }

        /// <summary>
        /// Gets or sets satellite acceleration (ecef) (m/s^2). Составляющие ускорения n-го спутника в системе координат ПЗ-90 на момент времени tb,
        /// обусловленные действием луны и солнца.
        /// </summary>
        public double AccelerationZ { get; set; }
    }
}
