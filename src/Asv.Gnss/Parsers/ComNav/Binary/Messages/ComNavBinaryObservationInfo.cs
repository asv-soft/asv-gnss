using System;
using Asv.IO;

namespace Asv.Gnss
{
    public class ComNavBinaryObservationInfo : ComNavBinaryMessageBase
    {
        public const ushort ComNavMessageId = 43;
        public override ushort MessageId => ComNavMessageId;
        public override string Name => "RANGE";

        public Observation[] Observations { get; set; }

        protected override void InternalContentDeserialize(ref ReadOnlySpan<byte> buffer)
        {
            var num = BinSerialize.ReadInt(ref buffer);
            Observations = new Observation[num];
            for (var i = 0; i < num; i++)
            {
                Observations[i] = new Observation();
                Observations[i].Deserialize(ref buffer);
            }
        }

        protected override void InternalContentSerialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        protected override int InternalGetContentByteSize()
        {
            throw new NotImplementedException();
        }
    }
}
