using System;
using Asv.IO;

namespace Asv.Gnss
{
    public enum AsvPosTypeEnum
    {
        NoPvt,
        StandAlonePvt,
        DifferentialPvt,
        FixedLocation
    }

    public enum AsvTimeSystemEnum
    {
        Gps,
        Glonass
    }

    public enum AsvDatumEnum
    {
        WGS84,
        PZ90_02
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
        }


        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        protected override int InternalGetContentByteSize()
        {
            throw new NotImplementedException();
        }

        public override void Randomize(Random random)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// GPS Epoch Time
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