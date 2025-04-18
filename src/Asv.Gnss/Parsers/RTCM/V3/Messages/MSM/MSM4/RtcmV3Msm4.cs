﻿using System;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class RtcmV3Msm4 : RtcmV3MultipleSignalMessagesBase
    {
        protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, int messageLength)
        {
            base.DeserializeContent(buffer, ref bitIndex, messageLength);
            var nCell = CellMask.SelectMany(_ => _).Count(_ => _ > 0);

            // Satellite data Nsat*(8 + 10) bit
            // Satellite  rough ranges
            var roughRanges = new double[SatelliteIds.Length];

            // Signal data
            // Pseudoranges 15*Ncell
            var pseudorange = new double[nCell];
            // PhaseRange data 22*Ncell
            var phaseRange = new double[nCell];
            // signal CNRs 6*Ncell
            var cnr = new double[nCell];

            //PhaseRange LockTime Indicator 4*Ncell
            var @lock = new byte[nCell];
            //Half-cycle ambiguityindicator 1*Ncell
            var halfCycle = new byte[nCell];

            for (var i = 0; i < SatelliteIds.Length; i++) roughRanges[i] = 0.0;
            for (var i = 0; i < nCell; i++) pseudorange[i] = phaseRange[i] = -1E16;

            /* decode satellite data, rough ranges */
            for (var i = 0; i < SatelliteIds.Length; i++)
            {
                /* Satellite  rough ranges */
                var rng = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
                if (rng != 255) roughRanges[i] = rng * RtcmV3Helper.RANGE_MS;
            }

            for (var i = 0; i < SatelliteIds.Length; i++)
            {
                var rngM = SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
                if (roughRanges[i] != 0.0) roughRanges[i] += rngM * RtcmV3Helper.P2_10 * RtcmV3Helper.RANGE_MS;
            }

            /* decode signal data */
            for (var i = 0; i < nCell; i++)
            {
                /* pseudorange */
                var prv = SpanBitHelper.GetBitS(buffer, ref bitIndex, 15);
                if (prv != -16384) pseudorange[i] = prv * RtcmV3Helper.P2_24 * RtcmV3Helper.RANGE_MS;
            }

            for (var i = 0; i < nCell; i++)
            {
                /* phase range */
                var cpv = SpanBitHelper.GetBitS(buffer, ref bitIndex, 22);
                if (cpv != -2097152) phaseRange[i] = cpv * RtcmV3Helper.P2_29 * RtcmV3Helper.RANGE_MS;
            }

            for (var i = 0; i < nCell; i++)
            {
                /* lock time */
                @lock[i] = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 4);
            }

            for (var i = 0; i < nCell; i++)
            {
                /* half-cycle ambiguity */
                halfCycle[i] = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            }

            for (var i = 0; i < nCell; i++)
            {
                /* cnr */
                /* GNSS signal CNR
                 1–63 dBHz */
                cnr[i] = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6) * 1.0;
            }

            CreateMsmObservable(roughRanges, pseudorange, phaseRange, @lock, halfCycle, cnr);
        }

       

        private void CreateMsmObservable(double[] roughRanges, double[] pseudorange, double[] phaseRange, byte[] @lock,
            byte[] halfCycle, double[] cnr)
        {
            var sig = new SignalRaw[SignalIds.Length];
            var sys = RtcmV3Helper.GetNavigationSystem(MessageId);

            Satellites = Array.Empty<Satellite>();
            if (SatelliteIds.Length == 0) return;
            Satellites = new Satellite[SatelliteIds.Length];

            /* id to signal */
            for (var i = 0; i < SignalIds.Length; i++)
            {
                sig[i] = new SignalRaw();
                sig[i].RinexCode = RtcmV3Helper.GetRinexCodeFromMsm(sys, SignalIds[i] - 1);

                /* signal to rinex obs type */
                sig[i].ObservationCode = RtcmV3Helper.Obs2Code(sig[i].RinexCode);
                sig[i].ObservationIndex = RtcmV3Helper.Code2Idx(sys, sig[i].ObservationCode);
            }


            var k = 0;
            for (var i = 0; i < SatelliteIds.Length; i++)
            {
                var prn = SatelliteIds[i];

                if (sys == NavigationSystemEnum.SYS_QZS) prn += RtcmV3Helper.MINPRNQZS - 1;
                else if (sys == NavigationSystemEnum.SYS_SBS) prn += RtcmV3Helper.MINPRNSBS - 1;

                
                var sat = RtcmV3Helper.satno(sys, prn);

                Satellites[i] = new Satellite {SatellitePrn = prn, SatelliteCode = RtcmV3Helper.Sat2Code(sat, prn)};


                var fcn = 0;
                if (sys == NavigationSystemEnum.SYS_GLO)
                {
                    #region SYS_GLO

                    // ToDo Нужны дополнительные данные по GLONASS Ephemeris, либо использовать сообщение MSM5, там есть ex[]
                    // fcn = -8; /* no glonass fcn info */
                    // if (ex && ex[i] <= 13)
                    // {
                    //     fcn = ex[i] - 7;
                    //     if (!rtcm->nav.glo_fcn[prn - 1])
                    //     {
                    //         rtcm->nav.glo_fcn[prn - 1] = fcn + 8; /* fcn+8 */
                    //     }
                    // }
                    // else if (rtcm->nav.geph[prn - 1].sat == sat)
                    // {
                    //     fcn = rtcm->nav.geph[prn - 1].frq;
                    // }
                    // else if (rtcm->nav.glo_fcn[prn - 1] > 0)
                    // {
                    //     fcn = rtcm->nav.glo_fcn[prn - 1] - 8;
                    // }

                    #endregion

                }

                var index = 0;
                Satellites[i].Signals = new Signal[CellMask[i].Count(_ => _ != 0)];

                for (var j = 0; j < SignalIds.Length; j++)
                {
                    if (CellMask[i][j] == 0) continue;

                    Satellites[i].Signals[index] = new Signal();
                    if (sat != 0 && sig[j].ObservationIndex >= 0)
                    {

                        var freq = fcn < -7.0 ? 0.0 : RtcmV3Helper.Code2Freq(sys, sig[j].ObservationCode, fcn);

                        /* pseudorange (m) */
                        if (roughRanges[i] != 0.0 && pseudorange[k] > -1E12)
                        {
                            Satellites[i].Signals[index].PseudoRange = roughRanges[i] + pseudorange[k];
                        }

                        /* carrier-phase (cycle) */
                        if (roughRanges[i] != 0.0 && phaseRange[k] > -1E12)
                        {
                            Satellites[i].Signals[index].CarrierPhase = (roughRanges[i] + phaseRange[k]) * freq / RtcmV3Helper.CLIGHT;
                        }

                        Satellites[i].Signals[index].MinLockTime = RtcmV3Helper.GetMinLockTime(@lock[k]);
                        Satellites[i].Signals[index].LockTime = @lock[k];
                        Satellites[i].Signals[index].HalfCycle = halfCycle[k];
                        // rtcm->obs.data[index].LLI[idx[k]] =
                        //     LossOfLock(rtcm, sat, idx[k],lock[j]) +(halfCycle[j] ? 3 : 0);
                        // rtcm->obs.data[index].SNR[idx[k]] = (uint16_t)(cnr[j] / SNR_UNIT + 0.5);
                        Satellites[i].Signals[index].Cnr = cnr[k] + 0.5;
                        Satellites[i].Signals[index].ObservationCode = sig[j].ObservationCode;
                        Satellites[i].Signals[index].RinexCode = $"L{sig[j].RinexCode}";
                    }

                    k++;
                    index++;
                }
            }
        }

        public Satellite[] Satellites { get; set; }

        
    }

    public class Satellite
    {
        public byte SatellitePrn { get; set; }
        public Signal[] Signals { get; set; }
        public string SatelliteCode { get; set; }
    }

    public class Signal
    {
        public string RinexCode { get; set; }

        /// <summary>
        /// Observation data PseudoRange (m)
        /// </summary>
        public double PseudoRange { get; set; }

        /// <summary>
        /// Observation data carrier-phase (m)
        /// </summary>
        public double CarrierPhase { get; set; }

        /// <summary>
        /// Observation data PhaseRangeRate (hz)
        /// </summary>
        public double PhaseRangeRate { get; set; }

        /// <summary>
        /// Signal strength (0.001 dBHz)
        /// </summary>
        public double Cnr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ushort LockTime { get; set; }

        /// <summary>
        /// Min lock time in min
        /// </summary>
        public double MinLockTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte HalfCycle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte ObservationCode { get; set; }
    }

}