using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI021Frn002Type040 : AsterixFieldI021Fixed
{
    public const byte StaticFrn = 2;
    public override string Name => "Target Report Descriptor";
    public override byte FieldReferenceNumber => StaticFrn;
    public byte Atp { get; set; }
    public byte Arc { get; set; }
    public bool Rc { get; set; }
    public bool Rab { get; set; }
    public bool Dcr { get; set; }
    public bool Gbs { get; set; }
    public bool Sim { get; set; }
    public bool Tst { get; set; }
    public bool Saa { get; set; }
    public byte Cl { get; set; }
    public bool Ipc { get; set; }
    public bool Nogo { get; set; }
    public bool Cpr { get; set; }
    public bool Ldpj { get; set; }
    public bool Rcf { get; set; }
    public bool HasFirstExtension { get; set; }
    public bool HasSecondExtension { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var b = buffer[0];
        buffer = buffer[1..];
        Atp = (byte)((b >> 5) & 0x07);
        Arc = (byte)((b >> 3) & 0x03);
        Rc = (b & 0x04) != 0;
        Rab = (b & 0x02) != 0;
        HasFirstExtension = (b & 0x01) != 0;
        if (!HasFirstExtension)
        {
            HasSecondExtension = false;
            return;
        }

        b = buffer[0];
        buffer = buffer[1..];
        Dcr = (b & 0x80) != 0;
        Gbs = (b & 0x40) != 0;
        Sim = (b & 0x20) != 0;
        Tst = (b & 0x10) != 0;
        Saa = (b & 0x08) != 0;
        Cl = (byte)((b >> 1) & 0x03);
        HasSecondExtension = (b & 0x01) != 0;
        if (!HasSecondExtension)
        {
            return;
        }

        b = buffer[0];
        buffer = buffer[1..];
        Ipc = (b & 0x20) != 0;
        Nogo = (b & 0x10) != 0;
        Cpr = (b & 0x08) != 0;
        Ldpj = (b & 0x04) != 0;
        Rcf = (b & 0x02) != 0;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var hasSecond = HasSecondExtension || Ipc || Nogo || Cpr || Ldpj || Rcf;
        var hasFirst = HasFirstExtension || hasSecond || Dcr || Gbs || Sim || Tst || Saa || Cl != 0;
        buffer[0] = (byte)((Atp << 5) | (Arc << 3) | (Rc ? 0x04 : 0) | (Rab ? 0x02 : 0) | (hasFirst ? 0x01 : 0));
        buffer = buffer[1..];
        if (!hasFirst)
        {
            return;
        }

        buffer[0] = (byte)((Dcr ? 0x80 : 0) | (Gbs ? 0x40 : 0) | (Sim ? 0x20 : 0) | (Tst ? 0x10 : 0) |
                           (Saa ? 0x08 : 0) | ((Cl & 0x03) << 1) | (hasSecond ? 0x01 : 0));
        buffer = buffer[1..];
        if (!hasSecond)
        {
            return;
        }

        buffer[0] = (byte)((Ipc ? 0x20 : 0) | (Nogo ? 0x10 : 0) | (Cpr ? 0x08 : 0) | (Ldpj ? 0x04 : 0) |
                           (Rcf ? 0x02 : 0));
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1 + (HasFirstExtension || HasSecondExtension || Dcr || Gbs || Sim || Tst || Saa || Cl != 0 ? 1 : 0)
                                           + (HasSecondExtension || Ipc || Nogo || Cpr || Ldpj || Rcf ? 1 : 0);
}