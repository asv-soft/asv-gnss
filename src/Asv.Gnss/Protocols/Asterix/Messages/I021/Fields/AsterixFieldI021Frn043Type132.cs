using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn043Type132 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 38;
    public override string Name => "Message Amplitude";
    public override byte FieldReferenceNumber => StaticFrn;
    public sbyte MessageAmplitude { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        MessageAmplitude = unchecked((sbyte)buffer[0]);
        buffer = buffer[1..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = unchecked((byte)MessageAmplitude);
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}