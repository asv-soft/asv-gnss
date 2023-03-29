using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class EcefPoint
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
    
    public class RtcmV3Message1020 : RtcmV3MessageBase
    {
        public const int RtcmMessageId = 1020;
        public override ushort MessageId => RtcmMessageId;
        public override string Name => "GLONASS Satellite Ephemeris Data";
    
        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            var utc = DateTime.UtcNow;
    
            var sys = NavigationSystemEnum.SYS_GLO;
    
            var prn = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
            FrequencyNumber = (int)SpanBitHelper.GetBitU(buffer, ref bitIndex, 5) - 7;

            AlmanacHealth = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            AlmanacHealthAvailabilityIndicator = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            P1Word = SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);



            var tkH = SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
            var tkM = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6);
            var tkS = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1) * 30.0;
    
            var bn = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1); 
            P2Word = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            var tb = SpanBitHelper.GetBitU(buffer, ref bitIndex, 7);
    
            Velocity.X = RtcmV3Helper.GetBitG(buffer,ref bitIndex, 24) * RtcmV3Helper.P2_20 * 1E3;
            Position.X = RtcmV3Helper.GetBitG(buffer, ref bitIndex, 27) * RtcmV3Helper.P2_11 * 1E3;
            Acceleration.X = RtcmV3Helper.GetBitG(buffer, ref bitIndex, 5) * RtcmV3Helper.P2_30 * 1E3;

            Velocity.Y = RtcmV3Helper.GetBitG(buffer, ref bitIndex, 24) * RtcmV3Helper.P2_20 * 1E3;
            Position.Y = RtcmV3Helper.GetBitG(buffer, ref bitIndex, 27) * RtcmV3Helper.P2_11 * 1E3;
            Acceleration.Y = RtcmV3Helper.GetBitG(buffer, ref bitIndex, 5) * RtcmV3Helper.P2_30 * 1E3;

            Velocity.Z = RtcmV3Helper.GetBitG(buffer, ref bitIndex, 24) * RtcmV3Helper.P2_20 * 1E3;
            Position.Z = RtcmV3Helper.GetBitG(buffer, ref bitIndex, 27) * RtcmV3Helper.P2_11 * 1E3;
            Acceleration.Z = RtcmV3Helper.GetBitG(buffer, ref bitIndex, 5) * RtcmV3Helper.P2_30 * 1E3;

            P3Word = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);

            // γn(tb)
            RelativeFreqBias = RtcmV3Helper.GetBitG(buffer, ref bitIndex, 11) * RtcmV3Helper.P2_40;

            MPWord = SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
            MLThirdStringWord = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);

            
            // τn(tb)
            SvClockBias = RtcmV3Helper.GetBitG(buffer, ref bitIndex, 22) * RtcmV3Helper.P2_30;
            // Δτn
            DelayL1ToL2 = RtcmV3Helper.GetBitG(buffer, ref bitIndex, 5) * RtcmV3Helper.P2_30;
            // 
            OperationAge = (int)SpanBitHelper.GetBitU(buffer, ref bitIndex, 5); 
            

            MP4Word = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            MFtWord = SpanBitHelper.GetBitU(buffer, ref bitIndex, 4);
            MNtWord = SpanBitHelper.GetBitU(buffer, ref bitIndex, 11);
            MMWord = SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
            AvailabilityOfAdditionalData = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            Na = SpanBitHelper.GetBitU(buffer, ref bitIndex, 11);
            Tc = SpanBitHelper.GetBitS(buffer, ref bitIndex, 32);
            MN4 = SpanBitHelper.GetBitU(buffer, ref bitIndex, 5);
            MTgps = SpanBitHelper.GetBitS(buffer, ref bitIndex, 22);
            MLFifthStringWord = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);

            var reserved = SpanBitHelper.GetBitS(buffer, ref bitIndex, 7);

            var sat = RtcmV3Helper.satno(sys, (int)prn);
            if (sat == 0)
            {
                throw new Exception($"rtcm3 1020 satellite number error: prn={prn}");
            }
    
            SatelliteNumber = sat;
            OperationHealth = (int)bn;
            Iode = (int)(tb & 0x7F);
    
            var week = 0;
            var tow = 0.0;
            RtcmV3Helper.GetFromTime(utc, ref week, ref tow);
    
            var tod = tow % 86400.0; tow -= tod;
            var tof = tkH * 3600.0 + tkM * 60.0 + tkS - 10800.0; /* lt->utc */
    
            if (tof < tod - 43200.0) tof += 86400.0;
            else if (tof > tod + 43200.0) tof -= 86400.0;
    
            FrameTime = RtcmV3Helper.GetFromGps(week, tow + tof);
            var toe = tb * 900.0 - 10800.0; /* lt->utc */
    
            if (toe < tod - 43200.0) toe += 86400.0;
            else if (toe > tod + 43200.0) toe -= 86400.0;
            EphemerisEpoch = RtcmV3Helper.GetFromGps(week, tow + toe);

            SatellitePrn = (int)prn;
            SatelliteCode = RtcmV3Helper.Sat2Code(sat, (int)prn);
        }

        public uint MLFifthStringWord { get; set; }

        public int MTgps { get; set; }

        public uint MN4 { get; set; }

        public int Tc { get; set; }

        public uint Na { get; set; }

        public uint AvailabilityOfAdditionalData { get; set; }

        public uint MMWord { get; set; }

        public uint MNtWord { get; set; }

        public uint MFtWord { get; set; }

        public uint MP4Word { get; set; }

        public uint MLThirdStringWord { get; set; }

        public uint MPWord { get; set; }

        public uint P3Word { get; set; }

        public uint P2Word { get; set; }

        public uint P1Word { get; set; }

        public uint AlmanacHealthAvailabilityIndicator { get; set; }

        public uint AlmanacHealth { get; set; }

        /// <summary>
        /// satellite number
        /// </summary>
        public int SatelliteNumber { get; set; }

        public int SatellitePrn { get; set; }

        public string SatelliteCode { get; set; }
        /// <summary>
        /// IODE (0-6 bit of tb field). Временной интервал внутри текущих суток по шкале UTC(SU) + 03 ч 00 мин (tb - 03 ч 00 мин)
        /// </summary>
        public int Iode { get; set; }
        /// <summary>
        /// satellite frequency number
        /// </summary>
        public int FrequencyNumber { get; set; }
        /// <summary>
        /// satellite health of operation
        /// </summary>
        public int OperationHealth { get; set; }
        /// <summary>
        /// satellite accuracy of operation
        /// </summary>
        public int OperationAccuracy { get; set; }
        /// <summary>
        /// satellite age of operation
        /// </summary>
        public int OperationAge { get; set; }
        /// <summary>
        /// epoch of epherides (gpst). Временной интервал внутри текущих суток по шкале UTC(SU) + 03 ч 00 мин (tb)
        /// </summary>
        public DateTime EphemerisEpoch { get; set; }
        /// <summary>
        /// message frame time (gpst). Время начала кадра в рамках текущих суток  (tk)
        /// </summary>
        public DateTime FrameTime { get; set; }
    
        /// <summary>
        /// satellite position (ecef) (m). Координаты n-го спутника в системе координат ПЗ-90 на момент времени tb
        /// </summary>
        public EcefPoint Position { get; } = new EcefPoint();
    
        /// <summary>
        /// satellite velocity (ecef) (m/s). составляющие вектора скорости n-го спутника в системе координат ПЗ-90 на момент
        /// времени tb
        /// </summary>
        public EcefPoint Velocity { get; } = new EcefPoint();
    
        /// <summary>
        /// satellite acceleration (ecef) (m/s^2). Составляющие ускорения n-го спутника в системе координат ПЗ-90 на момент времени tb,
        /// обусловленные действием луны и солнца
        /// </summary>
        public EcefPoint Acceleration { get; } = new EcefPoint();
    
        /// <summary>
        /// SV clock bias τn(tb) (s). Сдвиг шкалы времени n-го спутника tn относительно шкалы времени системы ГЛОНАСС
        /// tc на момент tb, т. е.τn(tb) = tc(tb) – tn(tb);
        /// </summary>
        public double SvClockBias { get; set; }
        /// <summary>
        /// relative freq bias (γn(tb)). Относительное отклонение прогнозируемого значения несущей частоты излучаемого сигнала n-го
        /// спутника от номинального значения на момент времени tb
        /// </summary>
        public double RelativeFreqBias { get; set; }
        /// <summary>
        /// delay between L1 and L2 Δτn (s). Временная разница между навигационным радиосигналом, переданным в поддиапазоне L2, и
        /// навигационным радиосигналом, переданным в поддиапазоне L1, заданным спутником:
        /// Δτn = tf2 – tf1, где tf1, tf2 – задержки в аппаратуре для поддиапазонов L1 и L2, соответственно выраженные в единицах
        /// времени.
        /// </summary>
        public double DelayL1ToL2 { get; set; }
    
        
    }
}