using System;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss
{
    public abstract class RtcmV3Msm7 : RtcmV3MultipleSignalMessagesBase
    {
        public Satellite[] Satellites { get; set; }

        protected override void DeserializeContent(
            ReadOnlySpan<byte> buffer,
            ref int bitIndex,
            int messageLength
        )
        {
            base.DeserializeContent(buffer, ref bitIndex, messageLength);
            var nCell = CellMask.SelectMany(_ => _).Count(_ => _ > 0);

            // Satellite data Nsat*(8 + 10) bit
            // Satellite  rough ranges
            var roughRanges = new double[SatelliteIds.Length];
            var roughPhaseRangeRates = new double[SatelliteIds.Length];
            var extSatInfo = new byte[SatelliteIds.Length];

            // Signal data
            // Pseudoranges 15*Ncell
            var pseudorange = new double[nCell];
            // PhaseRange data 22*Ncell
            var phaseRange = new double[nCell];
            // signal CNRs 6*Ncell
            var cnr = new double[nCell];
            //  fine PhaseRangeRates data 15*nCell
            var phaseRangeRates = new double[nCell];

            //PhaseRange LockTime Indicator 4*Ncell
            var @lock = new ushort[nCell];
            //Half-cycle ambiguityindicator 1*Ncell
            var halfCycle = new byte[nCell];

            for (var i = 0; i < SatelliteIds.Length; i++)
            {
                roughRanges[i] = roughPhaseRangeRates[i] = 0.0;
                extSatInfo[i] = 15;
            }
            for (var i = 0; i < nCell; i++)
                pseudorange[i] = phaseRange[i] = phaseRangeRates[i] = -1E16;

            /* decode satellite data, rough ranges */
            for (var i = 0; i < SatelliteIds.Length; i++)
            {
                /* Satellite  rough ranges */
                var rng = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
                if (rng != 255)
                    roughRanges[i] = rng * RtcmV3Helper.RANGE_MS;
            }

            for (var j = 0; j < SatelliteIds.Length; j++)
            { /* extended info */
                extSatInfo[j] = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 4);
            }

            for (var i = 0; i < SatelliteIds.Length; i++)
            {
                var rngM = SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
                if (roughRanges[i] != 0.0)
                    roughRanges[i] += rngM * RtcmV3Helper.P2_10 * RtcmV3Helper.RANGE_MS;
            }

            for (var i = 0; i < SatelliteIds.Length; i++)
            { /* phaserangerate */
                var rate = SpanBitHelper.GetBitS(buffer, ref bitIndex, 14);
                if (rate != -8192)
                    roughPhaseRangeRates[i] = rate * 1.0;
            }

            /* decode signal data */
            for (var i = 0; i < nCell; i++)
            {
                /* pseudorange */
                var prv = SpanBitHelper.GetBitS(buffer, ref bitIndex, 20);
                if (prv != -524288)
                    pseudorange[i] = prv * RtcmV3Helper.P2_29 * RtcmV3Helper.RANGE_MS;
            }

            for (var i = 0; i < nCell; i++)
            {
                /* phase range */
                var cpv = SpanBitHelper.GetBitS(buffer, ref bitIndex, 24);
                if (cpv != -8388608)
                    phaseRange[i] = cpv * RtcmV3Helper.P2_31 * RtcmV3Helper.RANGE_MS;
            }

            for (var i = 0; i < nCell; i++)
            {
                /* lock time */
                @lock[i] = (ushort)SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
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
                cnr[i] = SpanBitHelper.GetBitU(buffer, ref bitIndex, 10) * 0.0625;
            }

            for (var j = 0; j < nCell; j++)
            { /* phaserangerate */
                var rrv = SpanBitHelper.GetBitS(buffer, ref bitIndex, 15);
                if (rrv != -16384)
                    phaseRangeRates[j] = rrv * 0.0001;
            }

            CreateMsmObservable(
                roughRanges,
                roughPhaseRangeRates,
                pseudorange,
                phaseRange,
                phaseRangeRates,
                extSatInfo,
                @lock,
                halfCycle,
                cnr
            );
        }

        private void CreateMsmObservable(
            double[] roughRanges,
            double[] roughPhaseRangeRates,
            double[] pseudorange,
            double[] phaseRange,
            double[] phaseRangeRates,
            byte[] extSatInfo,
            ushort[] @lock,
            byte[] halfCycle,
            double[] cnr
        )
        {
            var sig = new SignalRaw[SignalIds.Length];
            var sys = RtcmV3Helper.GetNavigationSystem(MessageId);
            Satellites = new Satellite[0];
            if (SatelliteIds.Length == 0)
                return;
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

                if (sys == NavigationSystemEnum.SYS_QZS)
                    prn += RtcmV3Helper.MINPRNQZS - 1;
                else if (sys == NavigationSystemEnum.SYS_SBS)
                    prn += RtcmV3Helper.MINPRNSBS - 1;

                var sat = RtcmV3Helper.satno(sys, prn);

                Satellites[i] = new Satellite
                {
                    SatellitePrn = prn,
                    SatelliteCode = RtcmV3Helper.Sat2Code(sat, prn),
                };

                var fcn = 0;
                if (sys == NavigationSystemEnum.SYS_GLO)
                {
                    #region SYS_GLO

                    // ToDo Нужны дополнительные данные по GLONASS Ephemeris, либо использовать сообщение MSM5, там есть ex[]
                    // fcn = -8; /* no glonass fcn info */
                    if (extSatInfo[i] <= 13)
                    {
                        fcn = extSatInfo[i] - 7;
                    }
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
                    if (CellMask[i][j] == 0)
                        continue;

                    Satellites[i].Signals[index] = new Signal();
                    if (sat != 0 && sig[j].ObservationIndex >= 0)
                    {
                        var freq =
                            fcn < -7.0
                                ? 0.0
                                : RtcmV3Helper.Code2Freq(sys, sig[j].ObservationCode, fcn);
                        if (Math.Abs(freq - 0.0) < 0.01) { }
                        /* pseudorange (m) */
                        if (roughRanges[i] != 0.0 && pseudorange[k] > -1E12)
                        {
                            Satellites[i].Signals[index].PseudoRange =
                                roughRanges[i] + pseudorange[k];
                        }

                        /* carrier-phase (cycle) */
                        if (roughRanges[i] != 0.0 && phaseRange[k] > -1E12)
                        {
                            Satellites[i].Signals[index].CarrierPhase =
                                (roughRanges[i] + phaseRange[k]) * freq / RtcmV3Helper.CLIGHT;
                        }

                        /* doppler (hz) */
                        if (roughPhaseRangeRates[i] != 0.0 && phaseRangeRates[k] > -1E12)
                        {
                            Satellites[i].Signals[index].PhaseRangeRate =
                                -(roughPhaseRangeRates[i] + phaseRangeRates[k])
                                * freq
                                / RtcmV3Helper.CLIGHT;
                        }

                        Satellites[i].Signals[index].MinLockTime = RtcmV3Helper.GetMinLockTimeEx(
                            @lock[k]
                        );
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
    }
}
