using System;
using Asv.IO;

namespace Asv.Gnss;

public class AsvMessageGpsRawCa : AsvMessageBase
{
    private const int NavBitsU32Length = 10;

    public override ushort MessageId => 0x112;
    public override string Name => "GpsRawCa";

    public DateTime UtcTime { get; set; }
    public int Prn { get; set; }
    public int SatelliteId { get; set; }
    public bool CrcPassed { get; set; }
    public byte L1Code { get; set; }
    public ushort SvId { get; set; }
    public string RinexSatCode { get; set; } = string.Empty;
    public GnssSignalTypeEnum SignalType { get; set; }
    public string RindexSignalCode { get; set; } = string.Empty;
    public double Frequency { get; set; }
    public uint SubFrameId { get; set; }
    public uint[] NAVBitsU32 { get; set; } = [];
    public GpsSubframeBase? GpsSubFrame { get; set; }

    protected override void InternalDeserialize(ref ReadOnlySpan<byte> buffer)
    {
        var bitIndex = 0;
        var tow = AsvHelper.GetBitU(buffer, ref bitIndex, 30) * 0.001;
        var week = AsvHelper.GetBitU(buffer, ref bitIndex, 10);
        var cycle = AsvHelper.GetBitU(buffer, ref bitIndex, 4);
        var gpsTime = GpsRawHelper.Gps2Time((int)(cycle * 1024 + week), tow);
        UtcTime = AsvHelper.Gps2Utc(gpsTime);
        Prn = (int)AsvHelper.GetBitU(buffer, ref bitIndex, 6);
        CrcPassed = AsvHelper.GetBitU(buffer, ref bitIndex, 1) != 0;
        var code1 = AsvHelper.GetBitU(buffer, ref bitIndex, 1);
        bitIndex += 4;

        SignalType = GnssSignalTypeEnum.L1CA;
        RindexSignalCode = "1C";
        RinexSatCode = $"G{Prn}";
        SatelliteId = AsvHelper.satno(NavigationSystemEnum.SYS_GPS, Prn);

        buffer = buffer[(bitIndex / 8)..];
        NAVBitsU32 = new uint[NavBitsU32Length];
        for (var i = 0; i < NavBitsU32Length; i++)
        {
            NAVBitsU32[i] = BinSerialize.ReadUInt(ref buffer);
        }

        GpsSubFrame = GpsSubFrameFactory.Create(NAVBitsU32);
        L1Code = code1 != 0 ? AsvHelper.CODE_L1P : AsvHelper.CODE_L1C;
    }

    protected override void InternalSerialize(ref Span<byte> buffer)
    {
        var week = 0;
        var tow = 0.0;
        GpsRawHelper.Time2Gps(AsvHelper.Utc2Gps(UtcTime), ref week, ref tow);
        var cycle = (uint)(week / 1024);
        week %= 1024;
        var bitIndex = 0;
        AsvHelper.SetBitU(buffer, (uint)Math.Round(tow * 1000.0), ref bitIndex, 30);
        AsvHelper.SetBitU(buffer, (uint)week, ref bitIndex, 10);
        AsvHelper.SetBitU(buffer, cycle, ref bitIndex, 4);
        AsvHelper.SetBitU(buffer, (uint)Prn, ref bitIndex, 6);
        AsvHelper.SetBitU(buffer, CrcPassed ? 1u : 0u, ref bitIndex, 1);
        AsvHelper.SetBitU(buffer, L1Code == AsvHelper.CODE_L1C ? 0u : 1u, ref bitIndex, 1);
        bitIndex += 4;
        buffer = buffer[(bitIndex / 8)..];

        for (var i = 0; i < NavBitsU32Length; i++)
        {
            BinSerialize.WriteUInt(ref buffer, NAVBitsU32[i]);
        }
    }

    protected override int GetPayloadByteSize() => 7 + NAVBitsU32.Length * sizeof(uint);

    public override void Randomize(Random random)
    {
        UtcTime = new DateTime(2014, 08, 20, 15, 0, 0, DateTimeKind.Utc);
        Prn = random.Next() % 32 + 1;
        SatelliteId = AsvHelper.satno(NavigationSystemEnum.SYS_GPS, Prn);
        L1Code = AsvHelper.CODE_L1C;
        SignalType = GnssSignalTypeEnum.L1CA;
        RindexSignalCode = "1C";
        RinexSatCode = $"G{Prn}";
        NAVBitsU32 = new uint[NavBitsU32Length];
        NAVBitsU32[0] = (uint)GpsRawHelper.GpsSubframePreamble << 22;
        NAVBitsU32[1] = (uint)(random.Next() % 5 + 1) << 8;
        GpsSubFrame = GpsSubFrameFactory.Create(NAVBitsU32);
    }

    public GpsRawCa GetGnssRawNavMsg()
    {
        var msg = new GpsRawCa
        {
            NavSystem = NavSysEnum.GPS,
            CarrierFreq = Frequency,
            SignalType = SignalType,
            UtcTime = UtcTime,
            RawData = new uint[NAVBitsU32.Length],
            SatId = SvId,
            SatPrn = Prn,
            RinexSatCode = RinexSatCode,
            RindexSignalCode = RindexSignalCode,
            GpsSubFrame = GpsSubFrame
        };

        Array.Copy(NAVBitsU32, msg.RawData, NAVBitsU32.Length);
        return msg;
    }
}
