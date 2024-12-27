using System;
using Asv.IO;

namespace Asv.Gnss
{
    public enum AsvPosTypeEnum
    {
        NoPvt,
        StandAlonePvt,
        DifferentialPvt,
        FixedLocation,
    }

    public enum AsvTimeSystemEnum
    {
        Gps,
        Glonass,
    }

    public enum AsvDatumEnum
    {
        WGS84,
        PZ90_02,
    }

    public class AsvMessagePvtGeo : AsvMessageBase
    {
        public override ushort MessageId => 0x114;
        public override string Name => "PvtGeo";

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var tow = AsvHelper.GetBitU(buffer, ref bitIndex, 30) * 0.001;
            var week = AsvHelper.GetBitU(buffer, ref bitIndex, 10);
            var cycle = AsvHelper.GetBitU(buffer, ref bitIndex, 4);
            Tow = GpsRawHelper.Gps2Time((int)(cycle * 1024 + week), tow);

            PosType = (AsvPosTypeEnum)AsvHelper.GetBitU(buffer, ref bitIndex, 4);
            Error = AsvHelper.GetBitU(buffer, ref bitIndex, 4);
            Latitude = AsvHelper.GetBitS(buffer, ref bitIndex, 32) * 0.0005 / 3600.0;
            Longitude = AsvHelper.GetBitS(buffer, ref bitIndex, 32) * 0.0005 / 3600.0;
            Height = AsvHelper.GetBitS(buffer, ref bitIndex, 24) * 0.01;
            Undulation = AsvHelper.GetBitS(buffer, ref bitIndex, 16) * 0.01;
            RxClkBias = AsvHelper.GetBitS(buffer, ref bitIndex, 22) * GpsRawHelper.P2_30;
            RxClkDrift = AsvHelper.GetBitS(buffer, ref bitIndex, 12) * GpsRawHelper.P2_30;
            TimeSystem = (AsvTimeSystemEnum)AsvHelper.GetBitU(buffer, ref bitIndex, 2);
            Datum = (AsvDatumEnum)AsvHelper.GetBitU(buffer, ref bitIndex, 2);
            NrSv = AsvHelper.GetBitU(buffer, ref bitIndex, 6);
            MeanCorrAge = AsvHelper.GetBitU(buffer, ref bitIndex, 16) * 0.01;
            HAccuracy = AsvHelper.GetBitU(buffer, ref bitIndex, 16) * 0.01;
            VAccuracy = AsvHelper.GetBitU(buffer, ref bitIndex, 16) * 0.01;
            buffer = buffer[(bitIndex / 8)..];
        }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            var week = 0;
            double tow = 0;
            GpsRawHelper.Time2Gps(Tow, ref week, ref tow);
            var cycle = (uint)(week / 1024);
            week %= 1024;
            var bitIndex = 0;
            AsvHelper.SetBitU(buffer, (uint)Math.Round(tow * 1000.0), ref bitIndex, 30);
            AsvHelper.SetBitU(buffer, (uint)week, ref bitIndex, 10);
            AsvHelper.SetBitU(buffer, cycle, ref bitIndex, 4);
            AsvHelper.SetBitU(buffer, (uint)PosType, ref bitIndex, 4);
            AsvHelper.SetBitU(buffer, Error, ref bitIndex, 4);
            AsvHelper.SetBitS(
                buffer,
                (int)Math.Round(Latitude * 3600.0 / 0.0005),
                ref bitIndex,
                32
            );
            AsvHelper.SetBitS(
                buffer,
                (int)Math.Round(Longitude * 3600.0 / 0.0005),
                ref bitIndex,
                32
            );
            AsvHelper.SetBitS(buffer, (int)Math.Round(Height / 0.01), ref bitIndex, 24);
            AsvHelper.SetBitS(buffer, (int)Math.Round(Undulation / 0.01), ref bitIndex, 16);
            AsvHelper.SetBitS(
                buffer,
                (int)Math.Round(RxClkBias / GpsRawHelper.P2_30),
                ref bitIndex,
                22
            );
            AsvHelper.SetBitS(
                buffer,
                (int)Math.Round(RxClkDrift / GpsRawHelper.P2_30),
                ref bitIndex,
                12
            );
            AsvHelper.SetBitU(buffer, (uint)TimeSystem, ref bitIndex, 2);
            AsvHelper.SetBitU(buffer, (uint)Datum, ref bitIndex, 2);
            AsvHelper.SetBitU(buffer, NrSv, ref bitIndex, 6);
            AsvHelper.SetBitU(buffer, (uint)Math.Round(MeanCorrAge / 0.01), ref bitIndex, 16);
            AsvHelper.SetBitU(buffer, (uint)Math.Round(HAccuracy / 0.01), ref bitIndex, 16);
            AsvHelper.SetBitU(buffer, (uint)Math.Round(VAccuracy / 0.01), ref bitIndex, 16);
            buffer = buffer[(bitIndex / 8)..];
        }

        protected override int InternalGetContentByteSize()
        {
            return 31;
        }

        public override void Randomize(Random random)
        {
            Tow = GpsRawHelper.Utc2Gps(new DateTime(2014, 08, 20, 15, 0, 0, DateTimeKind.Utc));
            PosType = AsvPosTypeEnum.DifferentialPvt;
            TimeSystem = AsvTimeSystemEnum.Gps;
            Datum = AsvDatumEnum.WGS84;
            Latitude = random.NextDouble() * 180.0 - 90.0;
            Longitude = random.NextDouble() * 360.0 - 180.0;
            Height = random.NextDouble() * 2000.0;
        }

        /// <summary>
        /// Gets or sets gPS Epoch Time.
        /// </summary>
        public DateTime Tow { get; set; }

        public AsvPosTypeEnum PosType { get; set; }

        public uint Error { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Height { get; set; }

        public double Undulation { get; set; }
        public double RxClkBias { get; set; }
        public double RxClkDrift { get; set; }

        public AsvTimeSystemEnum TimeSystem { get; set; }

        public AsvDatumEnum Datum { get; set; }

        public uint NrSv { get; set; }

        public double MeanCorrAge { get; set; }

        public double HAccuracy { get; set; }

        public double VAccuracy { get; set; }
    }
}
