using System;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

public abstract class RtcmV3Msm4Base : RtcmV3MsmBase
{
    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
    {
        base.InternalDeserialize(buffer, ref bitIndex);
        var nCell = CellMask.SelectMany(_ => _).Count(_ => _ > 0);

        // Satellite data Nsat*(8 + 10) bit
        // Satellite  rough ranges
        RoughRangesRaw = new double[SatelliteIds.Length];

        // Signal data
        // Pseudoranges 15*Ncell
        PseudoRangeRaw = new double[nCell];
        // PhaseRange data 22*Ncell
        PhaseRangeRaw = new double[nCell];
        // signal CNRs 6*Ncell
        CnrRaw = new double[nCell];

        //PhaseRange LockTime Indicator 4*Ncell
        LockRaw = new byte[nCell];
        //Half-cycle ambiguityindicator 1*Ncell
        HalfCycleRaw = new byte[nCell];

        for (var i = 0; i < SatelliteIds.Length; i++) RoughRangesRaw[i] = 0.0;
        for (var i = 0; i < nCell; i++) PseudoRangeRaw[i] = PhaseRangeRaw[i] = -1E16;

        /* decode satellite data, rough ranges */
        for (var i = 0; i < SatelliteIds.Length; i++)
        {
            /* Satellite  rough ranges */
            var rng = SpanBitHelper.GetBitU(buffer, ref bitIndex, 8);
            if (rng != 255) RoughRangesRaw[i] = rng * RtcmV3Protocol.RANGE_MS;
        }

        for (var i = 0; i < SatelliteIds.Length; i++)
        {
            var rngM = SpanBitHelper.GetBitU(buffer, ref bitIndex, 10);
            if (RoughRangesRaw[i] != 0.0) RoughRangesRaw[i] += rngM * RtcmV3Protocol.P2_10 * RtcmV3Protocol.RANGE_MS;
        }

        /* decode signal data */
        for (var i = 0; i < nCell; i++)
        {
            /* pseudorange */
            var prv = SpanBitHelper.GetBitS(buffer, ref bitIndex, 15);
            if (prv != -16384) PseudoRangeRaw[i] = prv * RtcmV3Protocol.P2_24 * RtcmV3Protocol.RANGE_MS;
        }

        for (var i = 0; i < nCell; i++)
        {
            /* phase range */
            var cpv = SpanBitHelper.GetBitS(buffer, ref bitIndex, 22);
            if (cpv != -2097152) PhaseRangeRaw[i] = cpv * RtcmV3Protocol.P2_29 * RtcmV3Protocol.RANGE_MS;
        }

        for (var i = 0; i < nCell; i++)
        {
            /* lock time */
            LockRaw[i] = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 4);
        }

        for (var i = 0; i < nCell; i++)
        {
            /* half-cycle ambiguity */
            HalfCycleRaw[i] = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
        }

        for (var i = 0; i < nCell; i++)
        {
            /* cnr */
            /* GNSS signal CNR
             1–63 dBHz */
            CnrRaw[i] = SpanBitHelper.GetBitU(buffer, ref bitIndex, 6) * 1.0;
        }

        CreateMsmObservable(RoughRangesRaw, PseudoRangeRaw, PhaseRangeRaw, @LockRaw, HalfCycleRaw, CnrRaw);
    }

    public double[] RoughRangesRaw { get; set; }
    public double[] PseudoRangeRaw { get; set; }
    public double[] PhaseRangeRaw { get; set; }
    public double[] CnrRaw { get; set; }
    public byte[] LockRaw { get; set; }
    public byte[] HalfCycleRaw { get; set; }
    
    
    private static int  RoundI (double x) => (int)Math.Floor(x + 0.5);
    private static uint RoundU (double x) => (uint)Math.Floor(x + 0.5);
    protected override void InternalSerialize(Span<byte> buffer, ref int bitIndex)
    {
        // Заголовок/маски пишет базовый класс (ReferenceStationId, время, маски и т.п.)
        base.InternalSerialize(buffer, ref bitIndex);

        // Число активных ячеек (CellMask[i][j] == 1)
        var nCell = 0;
        for (var i = 0; i < SatelliteIds.Length; i++)
        for (var j = 0; j < SignalIds.Length; j++)
            if (CellMask[i][j] != 0)
                nCell++;
        
        // ---------- rough range: целые миллисекунды (8 бит) ----------
        for (int j = 0; j < SatelliteIds.Length; j++)
        {
            uint int_ms;
            if (RoughRangesRaw[j] == 0.0)
            {
                int_ms = 255;
            }
            else if (RoughRangesRaw[j] < 0.0 || RoughRangesRaw[j] > RtcmV3Protocol.RANGE_MS * 255.0)
            {
                int_ms = 255;
            }
            else
            {
                // ROUND_U(rrng/RANGE_MS/P2_10) >> 10
                uint q = RoundU(RoughRangesRaw[j] / RtcmV3Protocol.RANGE_MS / RtcmV3Protocol.P2_10);
                int_ms = q >> 10;
            }
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 8, int_ms);
        }
        
        // ---------- rough range: дробь 1/1024 мс (10 бит) ----------
        for (int j = 0; j < SatelliteIds.Length; j++)
        {
            uint mod_ms;
            if (RoughRangesRaw[j] <= 0.0 || RoughRangesRaw[j] > RtcmV3Protocol.RANGE_MS * 255.0)
            {
                mod_ms = 0;
            }
            else
            {
                // низшие 10 бит квантизованной в 1/1024 мс величины
                mod_ms = RoundU(RoughRangesRaw[j] / RtcmV3Protocol.RANGE_MS / RtcmV3Protocol.P2_10) & 0x3FFU;
            }
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 10, mod_ms);
        }
        
        // ---------- fine pseudorange: 15 бит знаковое, шаг P2_24*RANGE_MS ----------
        for (var j = 0; j < nCell; j++)
        {
            int val;
            if (PseudoRangeRaw[j] == 0.0)
                val = -16384; // no data
            else if (Math.Abs(PseudoRangeRaw[j]) > 292.7)
                val = -16384; // overflow -> no data
            else
                val = RoundI(PseudoRangeRaw[j] / RtcmV3Protocol.RANGE_MS / RtcmV3Protocol.P2_24);
            SpanBitHelper.SetBitS(buffer, ref bitIndex, 15, val);
        }
        
        // ---------- fine phase-range: 22 бита знаковое, шаг P2_29*RANGE_MS ----------
        for (var j = 0; j < nCell; j++)
        {
            int val;
            if (PhaseRangeRaw[j] == 0.0)
                val = -2097152; // no data
            else if (Math.Abs(PhaseRangeRaw[j]) > 1171.0)
                val = -2097152;
            else
                val = RoundI(PhaseRangeRaw[j] / RtcmV3Protocol.RANGE_MS / RtcmV3Protocol.P2_29);
            SpanBitHelper.SetBitS(buffer, ref bitIndex, 22, val);
        }
        
        // ---------- lock-time (MSM4: 4 бита код). Если уже есть коды 0..15. ----------
        for (var j = 0; j < nCell; j++)
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 4, (uint)(LockRaw[j] & 0x0F));
        
        // ---------- half-cycle ambiguity: 1 бит ----------
        for (var j = 0; j < nCell; j++)
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 1, (uint)(HalfCycleRaw[j] != 0 ? 1 : 0));
        
        // ---------- CNR: 6 бит (квант 1 дБГц) ----------
        for (int j = 0; j < nCell; j++)
        {
            var v = RoundI(CnrRaw[j] / 1.0);
            if (v < 0) v = 0;
            if (v > 63) v = 63;
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 6, (uint)v);
        }

    }

    protected override int InternalGetBitSize()
    {
        // Число активных ячеек (CellMask[i][j] == 1)
        var nCell = 0;
        for (var i = 0; i < SatelliteIds.Length; i++)
        for (var j = 0; j < SignalIds.Length; j++)
            if (CellMask[i][j] != 0)
                nCell++;
        // Число спутников
        var nSv = SatelliteIds.Length;
        return base.InternalGetBitSize() + nSv * (8 + 10) + nCell * (15 + 22 + 4 + 1 + 6);
        // return 8 * RawData.Length - (3 * 8 + 12 + 3 * 8);
    }

    private void CreateMsmObservable(double[] roughRanges, double[] pseudorange, double[] phaseRange, byte[] @lock,
        byte[] halfCycle, double[] cnr)
    {
        var sig = new SignalRaw[SignalIds.Length];
        var sys = RtcmV3Protocol.GetNavigationSystem(Id);

        Satellites = Array.Empty<Satellite>();
        if (SatelliteIds.Length == 0) return;
        Satellites = new Satellite[SatelliteIds.Length];

        /* id to signal */
        for (var i = 0; i < SignalIds.Length; i++)
        {
            sig[i] = new SignalRaw
            {
                RinexCode = RtcmV3Protocol.GetRinexCodeFromMsm(sys, SignalIds[i] - 1)
            };

            /* signal to rinex obs type */
            sig[i].ObservationCode = RtcmV3Protocol.Obs2Code(sig[i].RinexCode);
            sig[i].ObservationIndex = RtcmV3Protocol.Code2Idx(sys, sig[i].ObservationCode);
        }


        var k = 0;
        for (var i = 0; i < SatelliteIds.Length; i++)
        {
            var prn = SatelliteIds[i];

            if (sys == NavigationSystemEnum.SYS_QZS) prn += RtcmV3Protocol.MINPRNQZS - 1;
            else if (sys == NavigationSystemEnum.SYS_SBS) prn += RtcmV3Protocol.MINPRNSBS - 1;


            var sat = RtcmV3Protocol.satno(sys, prn);

            Satellites[i] = new Satellite { SatellitePrn = prn, SatelliteCode = RtcmV3Protocol.Sat2Code(sat, prn) };


            var fcn = 0;
            
            var index = 0;
            Satellites[i].Signals = new Signal[CellMask[i].Count(_ => _ != 0)];

            for (var j = 0; j < SignalIds.Length; j++)
            {
                if (CellMask[i][j] == 0) continue;

                Satellites[i].Signals[index] = new Signal();
                if (sat != 0 && sig[j].ObservationIndex >= 0)
                {

                    var freq = fcn < -7.0 ? 0.0 : RtcmV3Protocol.Code2Freq(sys, sig[j].ObservationCode, fcn);

                    /* pseudorange (m) */
                    if (roughRanges[i] != 0.0 && pseudorange[k] > -1E12)
                    {
                        Satellites[i].Signals[index].PseudoRange = roughRanges[i] + pseudorange[k];
                    }

                    /* carrier-phase (cycle) */
                    if (roughRanges[i] != 0.0 && phaseRange[k] > -1E12)
                    {
                        Satellites[i].Signals[index].CarrierPhase =
                            (roughRanges[i] + phaseRange[k]) * freq / RtcmV3Protocol.CLIGHT;
                    }

                    Satellites[i].Signals[index].MinLockTime = RtcmV3Protocol.GetMinLockTime(@lock[k]);
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