using System;
using Asv.IO;

namespace Asv.Gnss
{
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
