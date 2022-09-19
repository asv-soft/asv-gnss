﻿namespace Asv.Gnss
{
    public class GlonassWord4 : GlonassWordBase
    {
        public override byte WordId => 4;

        public override void Deserialize(byte[] data)
        {
            base.Deserialize(data);
            // var bitIndex = 8U;
            //
            // TauN = GlonassRawHelper.GetBitG(data, bitIndex, 22) * GlonassRawHelper.P2_30; bitIndex += 22;
            // DeltaTauN = GlonassRawHelper.GetBitG(data, bitIndex, 5) * GlonassRawHelper.P2_30; bitIndex += 5;
            // En = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 5); bitIndex += 5 + 14;
            // P4 = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 1); bitIndex += 1;
            // Ft = GetFt((byte) GlonassRawHelper.GetBitU(data, bitIndex, 4)); bitIndex += 4 + 3;
            // Nt = (ushort)GlonassRawHelper.GetBitU(data, bitIndex, 11); bitIndex += 11;
            // n = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 5); bitIndex += 5;
            // M = (byte)GlonassRawHelper.GetBitU(data, bitIndex, 2);
        }

        /// <summary>
        /// Слово τn(tb) - сдвиг шкалы времени n-го НКА t n относительно шкалы времени системы ГЛОНАСС tc,
        /// равный смещению по фазе ПСПД излучаемого навигационного радиосигнала n-го НКА относительно
        /// системного опорного сигнала на момент времени tb, выраженный в единицах времени
        /// </summary>
        // public double TauN { get; set; }

        /// <summary>
        /// Смещение излучаемого навигационного радиосигнала поддиапазона L2 относительно навигационного
        /// радиосигнала поддиапазона L1 для n-го НКА.
        /// </summary>
        // public double DeltaTauN { get; set; }

        /// <summary>
        /// Характеризует "возраст" оперативной информации, то есть интервал времени, прошедший от момента расчета (закладки)
        /// оперативной информации до момента времени t b для n-го НКА. Слово E n формируется на борту НКА.
        /// </summary>
        // public byte En { get; set; }

        /// <summary>
        /// Признак того, что на текущем интервале времени tb средствами ПКУ на НКА заложена (1)
        /// или не заложена (0) обновленная эфемеридная или частотно-временная информация. 
        /// </summary>
        // public byte P4 { get; set; }


        /// <summary>
        /// Фактор точности измерений, характеризующий в виде эквивалентной ошибки прогнозируемую ошибку измерения
        /// псевдодальности, обусловленную набором данных (эфемеридная и частотно-временная информация), излучаемых в
        /// навигационном сообщении на момент времени tb (см. таблицу 4.4)(1);
        /// </summary>
        // public double Ft { get; set; }

        /// <summary>
        /// Текущая дата, календарный номер суток внутри четырехлетнего интервала, начиная с 1-го января
        /// високосного года (1). Алгоритм пересчета от номера суток внутри четырехлетнего интервала к общепринятой
        /// форме даты (чч.мм.гг.) приведен в разделе П 3.1.3;
        /// </summary>
        // public ushort Nt { get; set; }

        /// <summary>
        /// Номер НКА, излучающего данный навигационный сигнал (1) и соответствующий его рабочей точке
        /// внутри орбитальной группировки ГЛОНАСС
        /// </summary>
        // public byte n { get; set; }

        /// <summary>
        /// Модификация НКА, излучающего данный навигационный сигнал. Значение "00" означает НКА «Глонасс», "01" – НКА «Глонасс-М»
        /// </summary>
        // public byte M { get; set; }

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