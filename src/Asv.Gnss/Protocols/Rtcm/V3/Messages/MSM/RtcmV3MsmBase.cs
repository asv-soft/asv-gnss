using System;
using System.Collections.Generic;
using Asv.IO;
using Microsoft.Extensions.Logging;

namespace Asv.Gnss;

public struct MsmHeader
{
    public int time_s;   // 7 бит
    public int clk_str;  // 2 бита
    public int clk_ext;  // 2 бита
    public int smooth;   // 1 бит
    public int tint_s;   // 3 бита

    public byte[] sats;  // список PRN (1..64), длина nsat
    public int nsat;

    public byte[] sigs;  // список сигналов (1..32), длина nsig
    public int nsig;

    // Плоская маска ячеек размером nsat*nsig (0/1) в порядке: сначала все сигналы для 1-го спутника, затем для 2-го, ...
    public byte[] cellmask;
}

public static class RtcmMsm
{
    /// <summary>
    /// Аналог RTKLIB decode_msm_head().
    /// Возвращает количество активных ячеек (ncell) либо -1 при ошибке.
    /// </summary>
    /// <param name="buffer">Полный RTCM3 буфер сообщения (с заголовком RTCM).</param>
    /// <param name="lenBytes">Длина сообщения в байтах (rtcm->len в RTKLIB).</param>
    /// <param name="sys">Система (GPS/GLO/…)</param>
    /// <param name="sync">Выход: бит sync (1 бит)</param>
    /// <param name="iod">Выход: IOD (3 бита)</param>
    /// <param name="h">Выход: заголовок MSM</param>
    /// <param name="hsize">Выход: позиция bitIndex на конец заголовка (в битах от начала сообщения)</param>
    /// <param name="testStaId">Делегат проверки staid (аналог test_staid). Должен вернуть true, если staid допустим.</param>
    /// <param name="logger">Логгер для ошибок (опционально)</param>
    /// <returns>ncell (&gt;=0) или -1 при ошибке</returns>
    public static int DecodeMsmHead(
        ReadOnlySpan<byte> buffer,
        int lenBytes,
        NavigationSystemEnum sys,
        out int sync,
        out int iod,
        out MsmHeader h,
        out int hsize,
        Func<int, bool> testStaId,
        ILogger? logger = null)
    {
        // Инициализация результата
        h = new MsmHeader
        {
            time_s = 0, clk_str = 0, clk_ext = 0, smooth = 0, tint_s = 0,
            sats = [], nsat = 0,
            sigs = [], nsig = 0,
            cellmask = []
        };
        sync = 0;
        iod = 0;
        hsize = 0;

        var i = 24; // как в RTKLIB: пропускаем префикс RTCM (первые 24 бита заняты preamble+len+rtcm header)
        if (i + 12 > lenBytes * 8)
        {
            logger?.LogWarning("rtcm3 length error: not enough bits to read type (len={len})", lenBytes);
            return -1;
        }

        var type = (int)SpanBitHelper.GetBitU(buffer, ref i, 12); // msg type

        // Минимум под заголовок MSM до масок (по RTKLIB: i+157 <= len*8)
        if (i + 157 > lenBytes * 8)
        {
            logger?.LogWarning("rtcm3 {type} length error: len={len}", type, lenBytes);
            return -1;
        }

        int staid = (int)SpanBitHelper.GetBitU(buffer, ref i, 12);

        // Время по системе
        if (sys == NavigationSystemEnum.SYS_GLO)
        {
            int dow = (int)SpanBitHelper.GetBitU(buffer, ref i, 3);
            double tod = SpanBitHelper.GetBitU(buffer, ref i, 27) * 0.001; // секунды внутри суток
            // В RTKLIB тут adjday_glot(rtcm, tod) — коррекция rtcm->time,
            // здесь оставляем пользователю (или добавьте свой аналог, если нужно).
            _ = dow; _ = tod;
        }
        else if (sys == NavigationSystemEnum.SYS_CMP)
        {
            double tow = SpanBitHelper.GetBitU(buffer, ref i, 30) * 0.001;
            tow += 14.0; // BDT -> GPST
            // В RTKLIB тут adjweek(rtcm, tow); здесь оставляем пользователю.
            _ = tow;
        }
        else
        {
            double tow = SpanBitHelper.GetBitU(buffer, ref i, 30) * 0.001;
            // adjweek(rtcm, tow)
            _ = tow;
        }

        sync        = (int)SpanBitHelper.GetBitU(buffer, ref i, 1);
        iod         = (int)SpanBitHelper.GetBitU(buffer, ref i, 3);
        h.time_s    = (int)SpanBitHelper.GetBitU(buffer, ref i, 7);
        h.clk_str   = (int)SpanBitHelper.GetBitU(buffer, ref i, 2);
        h.clk_ext   = (int)SpanBitHelper.GetBitU(buffer, ref i, 2);
        h.smooth    = (int)SpanBitHelper.GetBitU(buffer, ref i, 1);
        h.tint_s    = (int)SpanBitHelper.GetBitU(buffer, ref i, 3);

        // Маска спутников (64 бита)
        var sats = new byte[64];
        int nsat = 0;
        for (int j = 1; j <= 64; j++)
        {
            int mask = (int)SpanBitHelper.GetBitU(buffer, ref i, 1);
            if (mask != 0) sats[nsat++] = (byte)j;
        }
        h.nsat = nsat;
        h.sats = nsat > 0 ? sats.AsSpan(0, nsat).ToArray() : [];

        // Маска сигналов (32 бита)
        var sigs = new byte[32];
        int nsig = 0;
        for (int j = 1; j <= 32; j++)
        {
            int mask = (int)SpanBitHelper.GetBitU(buffer, ref i, 1);
            if (mask != 0) sigs[nsig++] = (byte)j;
        }
        h.nsig = nsig;
        h.sigs = nsig > 0 ? sigs.AsSpan(0, nsig).ToArray() : [];

        // Проверка station id
        if (!testStaId(staid))
        {
            // как RTKLIB: возвращаем -1
            return -1;
        }

        // nsat * nsig <= 64
        long cells = (long)nsat * (long)nsig;
        if (cells > 64)
        {
            logger?.LogWarning("rtcm3 {type} number of sats and sigs error: nsat={nsat} nsig={nsig}", type, nsat, nsig);
            return -1;
        }

        // Проверка длины под cellmask
        if (i + cells > lenBytes * 8)
        {
            logger?.LogWarning("rtcm3 {type} length error: len={len} nsat={nsat} nsig={nsig}", type, lenBytes, nsat, nsig);
            return -1;
        }

        // Чтение cellmask (nsat*nsig бит) и подсчёт ncell
        int ncell = 0;
        var cellmask = new byte[cells];
        for (int j = 0; j < cells; j++)
        {
            int bit = (int)SpanBitHelper.GetBitU(buffer, ref i, 1);
            cellmask[j] = (byte)bit;
            if (bit != 0) ncell++;
        }
        h.cellmask = cellmask;

        hsize = i; // позиция конца заголовка в битах

        // Лог уровня trace(4) в RTKLIB:
        // time строка там формируется из rtcm->time; здесь её нет — оставляем краткий лог.
        logger?.LogDebug("decode_msm_head: sys={sys} staid={staid} nsat={nsat} nsig={nsig} sync={sync} iod={iod} ncell={ncell}",
            sys, staid, nsat, nsig, sync, iod, ncell);

        return ncell;
    }
}


public abstract class RtcmV3MsmBase : RtcmV3MessageBase
{
    protected override void InternalDeserialize(ReadOnlySpan<byte> buffer, ref int bitIndex)
    {
        var utc = DateTime.UtcNow;
        ReferenceStationId = SpanBitHelper.GetBitU(buffer, ref bitIndex, 12);
        var sys = NavigationSystem = RtcmV3Protocol.GetNavigationSystem(Id);

        switch (sys)
        {
            case NavigationSystemEnum.SYS_GLO:
            {
                var dow = SpanBitHelper.GetBitU(buffer, ref bitIndex, 3);
                var tod = SpanBitHelper.GetBitU(buffer, ref bitIndex, 27);
                EpochTimeTow = dow * 86400000 + tod;
                EpochTime = RtcmV3Protocol.AdjustDailyRoverGlonassTime(utc, tod * 0.001);
                break;
            }
            case NavigationSystemEnum.SYS_CMP:
            {
                EpochTimeTow = SpanBitHelper.GetBitU(buffer, ref bitIndex, 30);
                var tow = EpochTimeTow * 0.001;
                tow += 14.0; /* BDT -> GPS Time */
                EpochTime = RtcmV3Protocol.AdjustWeekly(utc, tow);
                break;
            }
            default:
            {
                EpochTimeTow = SpanBitHelper.GetBitU(buffer, ref bitIndex, 30);
                var tow = EpochTimeTow * 0.001;
                EpochTime = RtcmV3Protocol.AdjustWeekly(utc, tow);
                break;
            }
        }


        MultipleMessageBit = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
        ObservableDataIsComplete = MultipleMessageBit == 0 ? true : false;

        Iods = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 3);

        SessionTime = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 7);

        ClockSteeringIndicator = SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);
        ExternalClockIndicator = SpanBitHelper.GetBitU(buffer, ref bitIndex, 2);

        SmoothingIndicator = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
        SmoothingInterval = SpanBitHelper.GetBitU(buffer, ref bitIndex, 3);


        var satellites = new List<byte>();
        var signals = new List<byte>();

        for (byte i = 1; i <= 64; i++)
        {
            var mask = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            if (mask > 0) satellites.Add(i);
        }

        for (byte i = 1; i <= 32; i++)
        {
            var mask = SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            if (mask > 0) signals.Add(i);
        }

        SatelliteIds = satellites.ToArray();
        SignalIds = signals.ToArray();
        var cellMaskCount = SatelliteIds.Length * SignalIds.Length;

        if (cellMaskCount > 64)
        {
            throw new Exception(
                $"RtcmV3Message{Id} number of Satellite and Signals error: Satellite={SatelliteIds.Length} Signals={SignalIds.Length}");
        }

        // CellMask = new byte[cellMaskCount];
        CellMask = new byte[SatelliteIds.Length][];
        for (var i = 0; i < SatelliteIds.Length; i++)
        {
            CellMask[i] = new byte[SignalIds.Length];
            for (var j = 0; j < SignalIds.Length; j++)
            {
                CellMask[i][j] = (byte)SpanBitHelper.GetBitU(buffer, ref bitIndex, 1);
            }
        }
    }

    protected override void InternalSerialize(Span<byte> buffer, ref int bitIndex)
    {
        // 1) Header / time
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 12, ReferenceStationId);

        var sys = NavigationSystem = RtcmV3Protocol.GetNavigationSystem(Id);

        switch (sys)
        {
            case NavigationSystemEnum.SYS_GLO:
            {
                // В десериализации: dow(3) + tod(27), затем:
                // EpochTimeTow = dow * 86400000 + tod
                // Здесь используем EpochTimeTow как источник (мс).
                // Требование: 0 <= dow < 7; 0 <= tod < 86400000.
                // Если у вас EpochTimeTow не инициализирован, заполните его заранее.
                var towMs = (ulong)EpochTimeTow;
                var dow = (uint)(towMs / 86400000UL) & 0x7U; // 3 бита
                var tod = (uint)(towMs % 86400000UL); // 27 бит

                SpanBitHelper.SetBitU(buffer, ref bitIndex, 3, dow);
                SpanBitHelper.SetBitU(buffer, ref bitIndex, 27, tod);
                break;
            }

            case NavigationSystemEnum.SYS_CMP:
            {
                // В десериализации: читается 30 бит (мс BDT), потом при вычислении EpochTime добавляется +14с (BDT->GPS).
                // Здесь просто пишем EpochTimeTow как 30-битное значение (мс BDT).
                // Убедитесь, что EpochTimeTow уже в шкале BDT (а не GPS).
                SpanBitHelper.SetBitU(buffer, ref bitIndex, 30, (uint)EpochTimeTow);
                break;
            }

            default:
            {
                // Для остальных систем: читается 30 бит (мс TOW по их шкале; у вас далее делается AdjustWeekly).
                // Аналогично — пишем EpochTimeTow как есть.
                SpanBitHelper.SetBitU(buffer, ref bitIndex, 30, (uint)EpochTimeTow);
                break;
            }
        }

        // 2) Флаги/поля статуса
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 1, MultipleMessageBit);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 3, Iods);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 7, SessionTime);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 2, ClockSteeringIndicator);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 2, ExternalClockIndicator);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 1, SmoothingIndicator);
        SpanBitHelper.SetBitU(buffer, ref bitIndex, 3, SmoothingInterval);

        // 3) Satellite mask (64 бита: индексы 1..64)
        //    Проверим входные данные.
        if (SatelliteIds is null) SatelliteIds = [];
        foreach (var sv in SatelliteIds)
        {
            if (sv is < 1 or > 64)
                throw new ArgumentOutOfRangeException(nameof(SatelliteIds), $"SatelliteId {sv} out of [1..64]");
        }

        // Быстрый lookup
        var satSet = new bool[65]; // 1..64
        foreach (var sv in SatelliteIds) satSet[sv] = true;

        for (byte i = 1; i <= 64; i++)
        {
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 1, satSet[i] ? 1U : 0U);
        }

        // 4) Signal mask (32 бита: индексы 1..32)
        if (SignalIds is null) SignalIds = [];
        foreach (var sg in SignalIds)
        {
            if (sg is < 1 or > 32)
                throw new ArgumentOutOfRangeException(nameof(SignalIds), $"SignalId {sg} out of [1..32]");
        }

        var sigSet = new bool[33]; // 1..32
        foreach (var sg in SignalIds) sigSet[sg] = true;

        for (byte i = 1; i <= 32; i++)
        {
            SpanBitHelper.SetBitU(buffer, ref bitIndex, 1, sigSet[i] ? 1U : 0U);
        }

        // 5) Cell mask (|Sats| * |Signals|, максимум 64 по вашей проверке)
        var satCount = SatelliteIds.Length;
        var sigCount = SignalIds.Length;
        var cellMaskCount = satCount * sigCount;

        if (cellMaskCount > 64)
            throw new Exception(
                $"RtcmV3Message{Id} number of Satellite and Signals error: Satellite={satCount} Signals={sigCount}");

        // Ожидаем структуру как в десериализации: byte[satCount][sigCount]
        if (CellMask == null || CellMask.Length != satCount)
            throw new InvalidOperationException("CellMask is null or has invalid first dimension size.");

        for (var i = 0; i < satCount; i++)
        {
            if (CellMask[i] == null || CellMask[i].Length != sigCount)
                throw new InvalidOperationException(
                    $"CellMask[{i}] is null or has invalid length (expected {sigCount}).");

            for (var j = 0; j < sigCount; j++)
            {
                // каждый элемент — 1 бит (0/1)
                var bit = (uint)(CellMask[i][j] != 0 ? 1 : 0);
                SpanBitHelper.SetBitU(buffer, ref bitIndex, 1, bit);
            }
        }
    }

    protected override int InternalGetBitSize()
    {
        return 12 + 30 + 1 + 3 + 7 + 2 + 2 + 1 + 3 + 64 + 32 + SatelliteIds.Length * SignalIds.Length;
    }

    public NavigationSystemEnum NavigationSystem { get; set; }

    /// <summary>
    /// Observable data complete flag (1:ok, 0:not complete)
    /// </summary>
    public bool ObservableDataIsComplete { get; set; }

    public DateTime EpochTime { get; set; }

    // protected byte[] CellMask { get; set; }
    protected byte[][] CellMask { get; set; }

    protected byte[] SatelliteIds { get; set; }

    protected byte[] SignalIds { get; set; }


    /// <summary>
    /// The Reference Station ID is determined by the service provider. Its 
    /// primary purpose is to link all message data to their unique sourceName. It is 
    /// useful in distinguishing between desired and undesired data in cases 
    /// where more than one service may be using the same data link 
    /// frequency. It is also useful in accommodating multiple reference 
    /// stations within a single data link transmission. 
    /// In reference network applications the Reference Station ID plays an 
    /// important role, because it is the link between the observation messages 
    /// of a specific reference station and its auxiliary information contained in 
    /// other messages for proper operation. Thus the Service Provider should 
    /// ensure that the Reference Station ID is unique within the whole 
    /// network, and that ID’s should be reassigned only when absolutely 
    /// necessary. 
    /// Service Providers may need to coordinate their Reference Station ID
    /// assignments with other Service Providers in their region in order to 
    /// avoid conflicts. This may be especially critical for equipment 
    /// accessing multiple services, depending on their services and means of 
    /// information distribution.
    /// </summary>
    public uint ReferenceStationId { get; set; }

    /// <summary>
    /// GNSS Epoch Time, specific for each GNSS
    /// GPS: Full seconds since the beginning of the GPS week
    /// GLONASS: Full seconds since the beginning of GLONASS day
    /// </summary>
    public uint EpochTimeTow { get; set; }

    /// <summary>
    /// 
    /// 1 - indicates that more MSMs follow for given physical time and reference station ID.
    /// 0 - indicates that it is the last MSM for given physical time and reference station ID
    /// </summary>
    public byte MultipleMessageBit { get; set; }

    /// <summary>
    /// Issue of Data Station.
    /// This field is reserved to be used to link MSM with future site- description (receiver, antenna description, etc.) messages. A value of “0” indicates that this field is not utilized.
    /// </summary>
    public byte Iods { get; set; }

    /// <summary>
    /// Cumulative session transmitting time
    /// </summary>
    public byte SessionTime { get; set; }

    /// <summary>
    /// 0 – clock steering is not applied In this case receiver clock must be kept in the range of ± 1 ms (approximately ± 300 km).
    /// 1 – clock steering has been applied In this case receiver clock must be kept in the range of ± 1 microsecond (approximately ± 300 meters).
    /// 2 – unknown clock steering status. 
    /// 3 – reserved
    /// </summary>
    public uint ClockSteeringIndicator { get; set; }

    /// <summary>
    /// 0 – internal clock is used.
    /// 1 – external clock is used, clock status is “locked”.
    /// 2 – external clock is used, clock status is “not locked”, which may indicate external clock failure and that the transmitted data may not be reliable.
    /// 3 – unknown clock is used
    /// </summary>
    public uint ExternalClockIndicator { get; set; }

    /// <summary>
    /// GNSS Smoothing Type Indicator:
    /// 1 – Divergence-free smoothing is used.
    /// 0 – Other type of smoothing is used 
    /// </summary>
    public uint SmoothingIndicator { get; set; }

    /// <summary>
    /// The GNSS Smoothing Interval is the integration period over which the
    /// pseudorange code phase measurements are averaged using carrier phase
    /// information.
    /// Divergence-free smoothing may be continuous over the entire period
    /// for which the satellite is visible.
    /// Notice: A value of zero indicates no smoothing is used. 
    /// </summary>
    public uint SmoothingInterval { get; set; }
}