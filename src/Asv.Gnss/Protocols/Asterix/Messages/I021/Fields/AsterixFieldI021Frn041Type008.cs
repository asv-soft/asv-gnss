using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn041Type008 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 36;
    public override string Name => "Aircraft Operational Status";
    public override byte FieldReferenceNumber => StaticFrn;
    public bool Ra { get; set; }
    public byte Tc { get; set; }
    public bool Ts { get; set; }
    public bool Arv { get; set; }
    public bool CdtiA { get; set; }
    public bool NotTcas { get; set; }
    public bool Sa { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var b = buffer[0];
        buffer = buffer[1..];
        Ra = (b & 0x80) != 0;
        Tc = (byte)((b >> 5) & 0x03);
        Ts = (b & 0x10) != 0;
        Arv = (b & 0x08) != 0;
        CdtiA = (b & 0x04) != 0;
        NotTcas = (b & 0x02) != 0;
        Sa = (b & 0x01) != 0;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        buffer[0] = (byte)((Ra ? 0x80 : 0) | ((Tc & 0x03) << 5) | (Ts ? 0x10 : 0) | (Arv ? 0x08 : 0) |
                           (CdtiA ? 0x04 : 0) | (NotTcas ? 0x02 : 0) | (Sa ? 0x01 : 0));
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1;
}