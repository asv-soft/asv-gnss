using System;

namespace Asv.Gnss;

public sealed class AsterixFieldI004Frn007Type060 : AsterixFieldI004Fixed
{
    public const byte StaticFrn = 7;
    public override string Name => "Safety Net Function & System Status";
    public override byte FieldReferenceNumber => StaticFrn;
    public bool Mrva { get; set; }
    public bool Ramld { get; set; }
    public bool Ramhd { get; set; }
    public bool Msaw { get; set; }
    public bool Apw { get; set; }
    public bool Clam { get; set; }
    public bool Stca { get; set; }
    public bool Afda { get; set; }
    public bool Rimca { get; set; }
    public bool Overflow { get; set; }
    public bool Overload { get; set; }
    public bool HasFirstExtension { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var b = buffer[0];
        buffer = buffer[1..];
        Mrva = (b & 0x80) != 0;
        Ramld = (b & 0x40) != 0;
        Ramhd = (b & 0x20) != 0;
        Msaw = (b & 0x10) != 0;
        Apw = (b & 0x08) != 0;
        Clam = (b & 0x04) != 0;
        Stca = (b & 0x02) != 0;
        HasFirstExtension = (b & 0x01) != 0;
        if (!HasFirstExtension)
        {
            return;
        }

        b = buffer[0];
        buffer = buffer[1..];
        Afda = (b & 0x80) != 0;
        Rimca = (b & 0x40) != 0;
        Overflow = (b & 0x04) != 0;
        Overload = (b & 0x02) != 0;
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        var hasFirst = HasFirstExtension || Afda || Rimca || Overflow || Overload;
        buffer[0] = (byte)((Mrva ? 0x80 : 0) | (Ramld ? 0x40 : 0) | (Ramhd ? 0x20 : 0) |
                           (Msaw ? 0x10 : 0) | (Apw ? 0x08 : 0) | (Clam ? 0x04 : 0) |
                           (Stca ? 0x02 : 0) | (hasFirst ? 0x01 : 0));
        buffer = buffer[1..];
        if (!hasFirst)
        {
            return;
        }

        buffer[0] = (byte)((Afda ? 0x80 : 0) | (Rimca ? 0x40 : 0) | (Overflow ? 0x04 : 0) |
                           (Overload ? 0x02 : 0));
        buffer = buffer[1..];
    }

    public override int GetByteSize() => 1 + (HasFirstExtension || Afda || Rimca || Overflow || Overload ? 1 : 0);
}