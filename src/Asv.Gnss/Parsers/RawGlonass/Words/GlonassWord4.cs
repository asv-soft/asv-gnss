namespace Asv.Gnss
{
    /// <summary>
    /// Represents a Glonass Word 4 object.
    /// </summary>
    public class GlonassWord4 : GlonassWordBase
    {
        /// <summary>
        /// Gets the unique identifier for the word.
        /// </summary>
        /// <remarks>
        /// The WordId property is an override of the base class property and returns the byte value 4.
        /// </remarks>
        /// <value>
        /// The unique identifier for the word. The value is always 4.
        /// </value>
        public override byte WordId => 4;

        /// <summary>
        /// Deserialize the given byte array and populate the object with the deserialized values.
        /// </summary>
        /// <param name="data">The byte array containing the serialized data.</param>
        public override void Deserialize(byte[] data)
        {
            base.Deserialize(data);
            var bitIndex = 8U;

            TauN = GlonassRawHelper.GetBitG(data, bitIndex, 22) * GlonassRawHelper.P2_30;
            bitIndex += 22;
            DelayTauN = GlonassRawHelper.GetBitG(data, bitIndex, 5) * GlonassRawHelper.P2_30;
            bitIndex += 5;
            Age = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 5);
            bitIndex += 5 + 14;
            P4 = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 1);
            bitIndex += 1;
            Accuracy = GetFt((byte)GlonassRawHelper.GetBitU(data, bitIndex, 4));
            bitIndex += 4 + 3;
            Nt = (ushort)GlonassRawHelper.GetBitU(data, bitIndex, 11);
            bitIndex += 11;
            Slot = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 5);
            bitIndex += 5;
            M = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 2);
        }

        /// <summary>
        /// Слово τn(tb) - сдвиг шкалы времени n-го НКА t n относительно шкалы времени системы ГЛОНАСС tc,
        /// равный смещению по фазе ПСПД излучаемого навигационного радиосигнала n-го НКА относительно
        /// системного опорного сигнала на момент времени tb, выраженный в единицах времени
        /// </summary>
        public double TauN { get; set; }

        /// <summary>
        /// Смещение излучаемого навигационного радиосигнала поддиапазона L2 относительно навигационного
        /// радиосигнала поддиапазона L1 для n-го НКА.
        /// </summary>
        public double DelayTauN { get; set; }

        /// <summary>
        /// En - характеризует "возраст" оперативной информации, то есть интервал времени, прошедший от момента расчета (закладки)
        /// оперативной информации до момента времени tb для n-го НКА. Слово En формируется на борту НКА.
        /// </summary>
        public byte Age { get; set; }

        /// <summary>
        /// Признак того, что на текущем интервале времени tb средствами ПКУ на НКА заложена (1)
        /// или не заложена (0) обновленная эфемеридная или частотно-временная информация.
        /// </summary>
        public byte P4 { get; set; }

        /// <summary>
        /// Фактор точности измерений, характеризующий в виде эквивалентной ошибки прогнозируемую ошибку измерения
        /// псевдодальности, обусловленную набором данных (эфемеридная и частотно-временная информация), излучаемых в
        /// навигационном сообщении на момент времени tb (см. таблицу 4.4)(1);
        /// </summary>
        public double Accuracy { get; set; }

        /// <summary>
        /// Текущая дата, календарный номер суток внутри четырехлетнего интервала, начиная с 1-го января
        /// високосного года (1). Алгоритм пересчета от номера суток внутри четырехлетнего интервала к общепринятой
        /// форме даты (чч.мм.гг.) приведен в разделе П 3.1.3;
        /// </summary>
        public ushort Nt { get; set; }

        /// <summary>
        /// Номер НКА, излучающего данный навигационный сигнал (1) и соответствующий его рабочей точке
        /// внутри орбитальной группировки ГЛОНАСС
        /// </summary>
        public byte Slot { get; set; }

        /// <summary>
        /// This method returns the time correction value (Ft) for a given navigation signal.
        /// The input parameter ft is a byte that represents the modification of the emitting satellite.
        /// A value of "00" indicates the satellite is "Glonass", "01" indicates the satellite is "Glonass-M".
        /// </summary>
        public byte M { get; set; }

        private double GetFt(byte ft)
        {
            switch (ft)
            {
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 2.5;
                case 3:
                    return 4;
                case 4:
                    return 5;
                case 5:
                    return 7;
                case 6:
                    return 10;
                case 7:
                    return 12;
                case 8:
                    return 14;
                case 9:
                    return 16;
                case 10:
                    return 32;
                case 11:
                    return 64;
                case 12:
                    return 128;
                case 13:
                    return 256;
                case 14:
                    return 512;
                default:
                    return double.NaN;
            }
        }
    }
}
