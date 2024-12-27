using System;
using System.Linq;

namespace Asv.Gnss
{
    public class AsvMessageGpsObservations : AsvMessageBase
    {
        public override ushort MessageId => 0x110;
        public override string Name => "GpsObservation";

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var tow = AsvHelper.GetBitU(buffer, ref bitIndex, 30) * 0.001;
            var week = AsvHelper.GetBitU(buffer, ref bitIndex, 10);
            var cycle = AsvHelper.GetBitU(buffer, ref bitIndex, 4);

            Tow = GpsRawHelper.Gps2Time((int)((cycle * 1024) + week), tow);
            TimeOffset = AsvHelper.GetBitS(buffer, ref bitIndex, 22) * GpsRawHelper.P2_30;
            var svNum = AsvHelper.GetBitU(buffer, ref bitIndex, 5);
            bitIndex += 1;
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);

            Observations = new AsvGpsObservation[svNum];
            for (var i = 0; i < svNum; i++)
            {
                var obs = new AsvGpsObservation();
                obs.Deserialize(ref buffer);
                Observations[i] = obs;
            }
        }

        public AsvGpsObservation[] Observations { get; set; }

        /// <summary>
        /// Gets or sets gPS Epoch Time.
        /// </summary>
        public DateTime Tow { get; set; }

        /// <summary>
        /// Gets or sets gPS Receiver Time Offset.
        /// </summary>
        public double TimeOffset { get; set; }

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
            AsvHelper.SetBitS(
                buffer,
                (int)Math.Round(TimeOffset / GpsRawHelper.P2_30),
                ref bitIndex,
                22
            );
            AsvHelper.SetBitU(buffer, (uint)(Observations?.Length ?? 0), ref bitIndex, 5);
            bitIndex += 1;
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);

            if (Observations == null)
            {
                return;
            }

            foreach (var obs in Observations)
            {
                obs.Serialize(ref buffer);
            }
        }

        protected override int InternalGetContentByteSize()
        {
            return 9 + (Observations?.Sum(_ => _.GetByteSize()) ?? 0);
        }

        public override void Randomize(Random random)
        {
            Tow = new DateTime(2014, 08, 20, 15, 0, 0, DateTimeKind.Utc);
            var length = (random.Next() % 6) + 4;
            var randomPrn = new int[length];
            var index = 0;
            while (index < length)
            {
                var prn = (random.Next() % 32) + 1;
                if (randomPrn.Any(_ => _ == prn))
                {
                    continue;
                }

                randomPrn[index] = prn;
                index++;
            }

            Observations = new AsvGpsObservation[length];

            for (var i = 0; i < length; i++)
            {
                var obs = new AsvGpsObservation();
                obs.Randomize(random, randomPrn[i]);
                Observations[i] = obs;
            }
        }
    }
}
