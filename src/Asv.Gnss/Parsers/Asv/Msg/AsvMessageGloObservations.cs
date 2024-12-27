using System;
using System.Linq;

namespace Asv.Gnss
{
    public class AsvMessageGloObservations : AsvMessageBase
    {
        public override ushort MessageId => 0x111;
        public override string Name => "GloObservation";

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            var bitIndex = 0;
            var tod = AsvHelper.GetBitU(buffer, ref bitIndex, 27) * 0.001;
            var day = (int)AsvHelper.GetBitU(buffer, ref bitIndex, 11);
            var cycle = (int)AsvHelper.GetBitU(buffer, ref bitIndex, 5);

            Tod = new DateTime(1996, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddYears((cycle - 1) * 4)
                .AddDays(day - 1)
                .AddSeconds(tod)
                .AddHours(-3);
            TimeOffset = AsvHelper.GetBitS(buffer, ref bitIndex, 22) * GpsRawHelper.P2_30;
            var svNum = AsvHelper.GetBitU(buffer, ref bitIndex, 5);
            bitIndex += 2;
            var byteIndex = bitIndex / 8;
            buffer = buffer.Slice(byteIndex, buffer.Length - byteIndex);

            Observations = new AsvGloObservation[svNum];
            for (var i = 0; i < svNum; i++)
            {
                var obs = new AsvGloObservation();
                obs.Deserialize(ref buffer);
                Observations[i] = obs;
            }
        }

        public AsvGloObservation[] Observations { get; set; }

        /// <summary>
        /// Gets or sets gPS Epoch Time.
        /// </summary>
        public DateTime Tod { get; set; }

        /// <summary>
        /// Gets or sets gPS Receiver Time Offset.
        /// </summary>
        public double TimeOffset { get; set; }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            var time = Tod.AddHours(3);
            var datum = new DateTime(1996, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var cycle = (int)((time - datum).TotalDays / 1461) + 1;
            var day = (uint)(time - datum.AddYears((cycle - 1) * 4)).TotalDays + 1;
            var tod = (time - datum.AddYears((cycle - 1) * 4).AddDays(day - 1)).TotalSeconds;

            var bitIndex = 0;
            AsvHelper.SetBitU(buffer, (uint)Math.Round(tod * 1000.0), ref bitIndex, 27);
            AsvHelper.SetBitU(buffer, day, ref bitIndex, 11);
            AsvHelper.SetBitU(buffer, (uint)cycle, ref bitIndex, 5);

            AsvHelper.SetBitS(
                buffer,
                (int)Math.Round(TimeOffset / GpsRawHelper.P2_30),
                ref bitIndex,
                22
            );
            AsvHelper.SetBitU(buffer, (uint)(Observations?.Length ?? 0), ref bitIndex, 5);
            bitIndex += 2;
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
            Tod = new DateTime(2014, 08, 20, 15, 0, 0, DateTimeKind.Utc);
            var length = (random.Next() % 6) + 4;
            var randomPrn = new int[length];
            var index = 0;
            while (index < length)
            {
                var prn = (random.Next() % 24) + 1;
                if (randomPrn.Any(_ => _ == prn))
                {
                    continue;
                }

                randomPrn[index] = prn;
                index++;
            }

            Observations = new AsvGloObservation[length];

            for (var i = 0; i < length; i++)
            {
                var obs = new AsvGloObservation();
                obs.Randomize(random, randomPrn[i]);
                Observations[i] = obs;
            }
        }
    }
}
