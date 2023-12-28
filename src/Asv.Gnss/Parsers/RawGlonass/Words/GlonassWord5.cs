namespace Asv.Gnss
{
    /// GlonassWord5 represents a Glonass Word 5 message.
    /// /
    public class GlonassWord5 : GlonassWordBase
    {
        /// <summary>
        /// Gets the unique identifier for the word.
        /// </summary>
        /// <value>
        /// The unique identifier for the word.
        /// </value>
        public override byte WordId
        {
            get => 5;
            protected set { }
        }

        /// <summary>
        /// Deserialize method used to deserialize data from byte array.
        /// </summary>
        /// <param name="data">The byte array containing the serialized data.</param>
        public override void Deserialize(byte[] data)
        {
            base.Deserialize(data);
            // var bitIndex = 8U;
            //
            // Na = (ushort)GlonassRawHelper.GetBitU(data, bitIndex, 11); bitIndex += 11;
            // TauC = GlonassRawHelper.GetBitG(data, bitIndex, 32) * GlonassRawHelper.P2_31; bitIndex += 32 + 1;
            // N4 = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 5); bitIndex += 5;
            // TauGPS = GlonassRawHelper.GetBitG(data, bitIndex, 22) * GlonassRawHelper.P2_30; bitIndex += 22;
            // ln = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 1);
        }

        /// <summary>
        /// Календарный номер суток внутри четырехлетнего периода, начиная с високосного года,
        /// к которым относятся поправка τc и данные альманаха системы (альманах орбит и альманах фаз);
        /// </summary>
        // public ushort Na { get; set; }

        /// <summary>
        /// Поправка к шкале времени системы ГЛОНАСС относительно UTC(SU). Поправка τc дана на начало суток с номером Na.
        /// τc  = TUTC(SU) + 03 час 00 мин - TГЛ
        /// </summary>
        // public double TauC { get; set; }

        /// <summary>
        /// Номер четырехлетнего периода, первый год первого четырехлетия  соответствует 1996 году
        /// </summary>
        // public byte N4 { get; set; }

        /// <summary>
        /// Поправка на расхождение системных шкал времени GPS(TGPS) и ГЛОНАСС (ТГЛ) в соответствии со следующим выражением:
        /// TGPS – TГЛ = ΔΤ + τGPS
        /// </summary>
        // public double TauGPS { get; set; }

        /// <summary>
        /// Признак недостоверности кадра n-го НКА; ln = 0 свидетельствует о пригодности НКА для навигации;  ln = 1 означает факт непригодности данного НКА для навигации
        /// </summary>
        // public byte ln { get; set; }
    }
}