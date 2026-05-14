using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn006Type045 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 6;
    public override string Name => "Alert Status";
    public override byte FieldReferenceNumber => StaticFrn;
    public byte AlertStatus { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        AlertStatus = (byte)((buffer[0] >> 1) & 0x07);
        buffer = buffer[1..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((AlertStatus & 0x07) << 1);
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}