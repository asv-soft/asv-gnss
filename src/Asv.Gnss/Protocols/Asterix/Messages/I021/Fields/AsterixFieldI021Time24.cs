using System;

namespace Asv.Gnss;

public abstract class AsterixFieldI021Time24 : AsterixFieldI021Fixed
{
    public double Seconds { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        Seconds = AsterixI021Binary.ReadTimeSeconds(ref buffer);
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        AsterixI021Binary.WriteTimeSeconds(ref buffer, Seconds);
    }

    public override int GetByteSize() => 3;
}