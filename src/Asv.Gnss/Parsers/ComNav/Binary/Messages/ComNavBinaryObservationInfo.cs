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

    public class Observation : ISpanSerializable
    {
        public int Prn { get; set; }
        public double PseudoRange { get; set; }

        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            Prn = BinSerialize.ReadUShort(ref buffer);
            BinSerialize.ReadUShort(ref buffer);
            PseudoRange = BinSerialize.ReadDouble(ref buffer);
            BinSerialize.ReadUInt(ref buffer);
            BinSerialize.ReadULong(ref buffer);
            BinSerialize.ReadUInt(ref buffer);
            BinSerialize.ReadUInt(ref buffer);
            BinSerialize.ReadUInt(ref buffer);
            BinSerialize.ReadUInt(ref buffer);
            BinSerialize.ReadUInt(ref buffer);
        }

        public void Serialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }
    }
}
