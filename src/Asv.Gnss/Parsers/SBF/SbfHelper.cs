using System;

namespace Asv.Gnss
{
    public enum SbfNavSysEnum
    {
        Unknown,
        GPS,
        SBAS,
        GLONASS,
        Galileo,
        QZSS,
        BeiDou,
        IRNS,
        LEO,
        MSS,
        NavIC,
    }

    public static class SbfHelper
    {
        public static string GetRinexSatteliteCode(byte svidOrPrn, out SbfNavSysEnum nav)
        {
            nav = SbfNavSysEnum.Unknown;

            if (37 >= svidOrPrn && svidOrPrn >= 1)
            {
                nav = SbfNavSysEnum.GPS;
                return $"G{svidOrPrn:00}";
            }

            if (61 >= svidOrPrn && svidOrPrn >= 38)
            {
                nav = SbfNavSysEnum.GLONASS;
                return $"R{svidOrPrn - 37:00}";
            }

            // 62: GLONASS satellite of which the slot number is not known
            if (svidOrPrn == 62)
            {
                nav = SbfNavSysEnum.GLONASS;
                return $"R??";
            }

            if (68 >= svidOrPrn && svidOrPrn >= 63)
            {
                nav = SbfNavSysEnum.GLONASS;
                return $"R{svidOrPrn - 38:00}";
            }

            if (106 >= svidOrPrn && svidOrPrn >= 71)
            {
                nav = SbfNavSysEnum.Galileo;
                return $"E{svidOrPrn - 70:00}";
            }

            // 107-119: L-Band (MSS) satellite. Corresponding
            // satellite name can be found in the LBandBeams block.
            if (119 >= svidOrPrn && svidOrPrn >= 107)
            {
                nav = SbfNavSysEnum.Unknown;
                return $"NA?";
            }

            if (140 >= svidOrPrn && svidOrPrn >= 120)
            {
                nav = SbfNavSysEnum.SBAS;
                return $"S{svidOrPrn - 100:00}";
            }

            if (180 >= svidOrPrn && svidOrPrn >= 141)
            {
                nav = SbfNavSysEnum.BeiDou;
                return $"C{svidOrPrn - 140:00}";
            }

            if (187 >= svidOrPrn && svidOrPrn >= 181)
            {
                nav = SbfNavSysEnum.QZSS;
                return $"J{svidOrPrn - 180:00}";
            }

            if (197 >= svidOrPrn && svidOrPrn >= 191)
            {
                nav = SbfNavSysEnum.IRNS;
                return $"I{svidOrPrn - 190:00}";
            }

            if (215 >= svidOrPrn && svidOrPrn >= 198)
            {
                nav = SbfNavSysEnum.SBAS;
                return $"S{svidOrPrn - 157:00}";
            }

            if (222 >= svidOrPrn && svidOrPrn >= 216)
            {
                nav = SbfNavSysEnum.IRNS;
                return $"I{svidOrPrn - 208:00}";
            }

            if (245 >= svidOrPrn && svidOrPrn >= 223)
            {
                nav = SbfNavSysEnum.BeiDou;
                return $"C{svidOrPrn - 182:00}";
            }

            return null;
        }

        public static int GetSattelitePrn(byte svidOrPrn)
        {
            if (37 >= svidOrPrn && svidOrPrn >= 1)
            {
                return svidOrPrn;
            }

            if (61 >= svidOrPrn && svidOrPrn >= 38)
            {
                return svidOrPrn - 37;
            }

            // 62: GLONASS satellite of which the slot number is not known
            if (svidOrPrn == 62)
            {
                return 0;
            }

            if (68 >= svidOrPrn && svidOrPrn >= 63)
            {
                return svidOrPrn - 38;
            }

            if (106 >= svidOrPrn && svidOrPrn >= 71)
            {
                return svidOrPrn - 70;
            }

            // 107-119: L-Band (MSS) satellite. Corresponding
            // satellite name can be found in the LBandBeams block.
            if (119 >= svidOrPrn && svidOrPrn >= 107)
            {
                return 0;
            }

            if (140 >= svidOrPrn && svidOrPrn >= 120)
            {
                return svidOrPrn - 100;
            }

            if (180 >= svidOrPrn && svidOrPrn >= 141)
            {
                return svidOrPrn - 140;
            }

            if (187 >= svidOrPrn && svidOrPrn >= 181)
            {
                return svidOrPrn - 180;
            }

            if (197 >= svidOrPrn && svidOrPrn >= 191)
            {
                return svidOrPrn - 190;
            }

            if (215 >= svidOrPrn && svidOrPrn >= 198)
            {
                return svidOrPrn - 157;
            }

            if (222 >= svidOrPrn && svidOrPrn >= 216)
            {
                return svidOrPrn - 208;
            }

            if (245 >= svidOrPrn && svidOrPrn >= 223)
            {
                return svidOrPrn - 182;
            }

            return 0;
        }

        /// <summary>
        /// 4.1.10 Signal Type
        /// Some sub-blocks contain a signal type field, which identifies the type of signal and modulation the sub-blocks applies to.The signal numbering is defined as follows:
        /// </summary>
        /// <returns></returns>
        public static SbfSignalTypeEnum GetSignalType(
            byte signalNumber,
            byte freqNr,
            out SbfNavSysEnum constellation,
            out double carrierFreq,
            out string rinexCode
        )
        {
            var signalType = SbfSignalTypeEnum.Unknown;
            constellation = SbfNavSysEnum.Unknown;
            carrierFreq = Double.NaN;
            rinexCode = null;
            switch (signalNumber)
            {
                case 0:
                    signalType = SbfSignalTypeEnum.L1CA;
                    constellation = SbfNavSysEnum.GPS;
                    carrierFreq = 1575.42;
                    rinexCode = "1C";
                    break;
                case 1:
                    signalType = SbfSignalTypeEnum.L1P;
                    constellation = SbfNavSysEnum.GPS;
                    carrierFreq = 1575.42;
                    rinexCode = "1W";
                    break;
                case 2:
                    signalType = SbfSignalTypeEnum.L2P;
                    constellation = SbfNavSysEnum.GPS;
                    carrierFreq = 1227.6;
                    rinexCode = "2W";
                    break;
                case 3:
                    signalType = SbfSignalTypeEnum.L2C;
                    constellation = SbfNavSysEnum.GPS;
                    carrierFreq = 1227.6;
                    rinexCode = "2L";
                    break;
                case 4:
                    signalType = SbfSignalTypeEnum.L5;
                    constellation = SbfNavSysEnum.GPS;
                    carrierFreq = 1176.45;
                    rinexCode = "5Q";
                    break;
                case 5:
                    signalType = SbfSignalTypeEnum.L1C;
                    constellation = SbfNavSysEnum.GPS;
                    carrierFreq = 1575.42;
                    rinexCode = "1L";
                    break;
                case 6:
                    signalType = SbfSignalTypeEnum.L1CA;
                    constellation = SbfNavSysEnum.QZSS;
                    carrierFreq = 1575.42;
                    rinexCode = "1C";
                    break;
                case 7:
                    signalType = SbfSignalTypeEnum.L2C;
                    constellation = SbfNavSysEnum.QZSS;
                    carrierFreq = 1227.6;
                    rinexCode = "2L";
                    break;
                case 8:
                    signalType = SbfSignalTypeEnum.L1CA;
                    constellation = SbfNavSysEnum.GLONASS;
                    carrierFreq = 1602.00 + (freqNr - 8f) * 9 / 16;
                    rinexCode = "1C";
                    break;
                case 9:
                    signalType = SbfSignalTypeEnum.L1P;
                    constellation = SbfNavSysEnum.GLONASS;
                    carrierFreq = 1602.00 + (freqNr - 8f) * 9 / 16;
                    rinexCode = "1P";
                    break;
                case 10:
                    signalType = SbfSignalTypeEnum.L2P;
                    constellation = SbfNavSysEnum.GLONASS;
                    carrierFreq = 1246.00 + (freqNr - 8f) * 7 / 16;
                    rinexCode = "2P";
                    break;
                case 11:
                    signalType = SbfSignalTypeEnum.L2CA;
                    constellation = SbfNavSysEnum.GLONASS;
                    carrierFreq = 1246.00 + (freqNr - 8f) * 7 / 16;
                    rinexCode = "2C";
                    break;
                case 12:
                    signalType = SbfSignalTypeEnum.L3;
                    constellation = SbfNavSysEnum.GLONASS;
                    carrierFreq = 1202.025;
                    rinexCode = "3Q";
                    break;
                case 13:
                    signalType = SbfSignalTypeEnum.B1C;
                    constellation = SbfNavSysEnum.BeiDou;
                    carrierFreq = 1575.42;
                    rinexCode = "1P";
                    break;
                case 14:
                    signalType = SbfSignalTypeEnum.B2a;
                    constellation = SbfNavSysEnum.BeiDou;
                    carrierFreq = 1176.45;
                    rinexCode = "5P";
                    break;
                case 15:
                    signalType = SbfSignalTypeEnum.L5;
                    constellation = SbfNavSysEnum.NavIC;
                    carrierFreq = 1176.45;
                    rinexCode = "5A";
                    break;
                case 16:
                    signalType = SbfSignalTypeEnum.Reserved;
                    constellation = SbfNavSysEnum.Unknown;
                    carrierFreq = Double.NaN;
                    rinexCode = string.Empty;
                    break;
                case 17:
                    signalType = SbfSignalTypeEnum.E1_L1BC;
                    constellation = SbfNavSysEnum.Galileo;
                    carrierFreq = 1575.42;
                    rinexCode = "1C";
                    break;
                case 18:
                    signalType = SbfSignalTypeEnum.Reserved;
                    constellation = SbfNavSysEnum.Unknown;
                    carrierFreq = Double.NaN;
                    rinexCode = string.Empty;
                    break;
                case 19:
                    signalType = SbfSignalTypeEnum.E6_E6BC;
                    constellation = SbfNavSysEnum.Galileo;
                    carrierFreq = 1278.75;
                    rinexCode = "6C";
                    break;
                case 20:
                    signalType = SbfSignalTypeEnum.E5a;
                    constellation = SbfNavSysEnum.Galileo;
                    carrierFreq = 1176.45;
                    rinexCode = "5Q";
                    break;
                case 21:
                    signalType = SbfSignalTypeEnum.E5b;
                    constellation = SbfNavSysEnum.Galileo;
                    carrierFreq = 1207.14;
                    rinexCode = "7Q";
                    break;
                case 22:
                    signalType = SbfSignalTypeEnum.E5_AltBoc;
                    constellation = SbfNavSysEnum.Galileo;
                    carrierFreq = 1191.795;
                    rinexCode = "8Q";
                    break;
                case 23:
                    signalType = SbfSignalTypeEnum.LBand;
                    constellation = SbfNavSysEnum.MSS;
                    carrierFreq = Double.NaN;
                    rinexCode = "NA";
                    break;
                case 24:
                    signalType = SbfSignalTypeEnum.L1CA;
                    constellation = SbfNavSysEnum.SBAS;
                    carrierFreq = 1575.42;
                    rinexCode = "1C";
                    break;
                case 25:
                    signalType = SbfSignalTypeEnum.L5;
                    constellation = SbfNavSysEnum.SBAS;
                    carrierFreq = 1176.45;
                    rinexCode = "5I";
                    break;
                case 26:
                    signalType = SbfSignalTypeEnum.L5;
                    constellation = SbfNavSysEnum.QZSS;
                    carrierFreq = 1176.45;
                    rinexCode = "5Q";
                    break;
                case 27:
                    signalType = SbfSignalTypeEnum.L6;
                    constellation = SbfNavSysEnum.QZSS;
                    carrierFreq = 1278.75;
                    rinexCode = string.Empty;
                    break;
                case 28:
                    signalType = SbfSignalTypeEnum.B1I;
                    constellation = SbfNavSysEnum.BeiDou;
                    carrierFreq = 1561.098;
                    rinexCode = "2I";
                    break;
                case 29:
                    signalType = SbfSignalTypeEnum.B2I;
                    constellation = SbfNavSysEnum.BeiDou;
                    carrierFreq = 1207.14;
                    rinexCode = "7I";
                    break;
                case 30:
                    signalType = SbfSignalTypeEnum.B3I;
                    constellation = SbfNavSysEnum.BeiDou;
                    carrierFreq = 1268.52;
                    rinexCode = "6I";
                    break;
                case 31:
                    signalType = SbfSignalTypeEnum.Reserved;
                    constellation = SbfNavSysEnum.Unknown;
                    carrierFreq = Double.NaN;
                    rinexCode = string.Empty;
                    break;
                case 32:
                    signalType = SbfSignalTypeEnum.L1C;
                    constellation = SbfNavSysEnum.QZSS;
                    carrierFreq = 1575.42;
                    rinexCode = "1L";
                    break;
                case 33:
                    signalType = SbfSignalTypeEnum.L1S;
                    constellation = SbfNavSysEnum.QZSS;
                    carrierFreq = 1575.42;
                    rinexCode = "1Z";
                    break;
                case 34:
                    signalType = SbfSignalTypeEnum.B2b;
                    constellation = SbfNavSysEnum.BeiDou;
                    carrierFreq = 1207.14;
                    rinexCode = "7D";
                    break;
                case 35:
                    signalType = SbfSignalTypeEnum.Reserved;
                    constellation = SbfNavSysEnum.Unknown;
                    carrierFreq = Double.NaN;
                    rinexCode = string.Empty;
                    break;
                case 36:
                    signalType = SbfSignalTypeEnum.S;
                    constellation = SbfNavSysEnum.IRNS;
                    carrierFreq = 2492.028;
                    rinexCode = "9A";
                    break;
            }

            return signalType;
        }
    }

    public enum SbfSignalTypeEnum
    {
        Unknown,
        L1CA,
        L1P,
        L2P,
        L2C,
        L5,
        L1C,
        L2CA,
        L3,
        B1C,
        B2a,
        E1_L1BC,
        Reserved,
        E6_E6BC,
        E5a,
        E5b,
        LBand,
        E5_AltBoc,
        L6,
        B1I,
        B2I,
        B3I,
        L1S,
        B2b,
        S,
    }
}
