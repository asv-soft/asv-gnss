using System;
using System.Buffers.Binary;

namespace Asv.Gnss;

public sealed class AsterixFieldI002Frn009Type100 : AsterixFieldI002Fixed
{
    public const byte StaticFrn = 9;
    public override string Name => "Dynamic Window - Type 1";
    public override byte FieldReferenceNumber => StaticFrn;
    public double RhoStartNm { get; set; }
    public double RhoEndNm { get; set; }
    public double ThetaStartDeg { get; set; }
    public double ThetaEndDeg { get; set; }

    public override void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        RhoStartNm = BinaryPrimitives.ReadUInt16BigEndian(buffer) / 128.0;
        buffer = buffer[2..];
        RhoEndNm = BinaryPrimitives.ReadUInt16BigEndian(buffer) / 128.0;
        buffer = buffer[2..];
        ThetaStartDeg = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 360.0 / 65536.0;
        buffer = buffer[2..];
        ThetaEndDeg = BinaryPrimitives.ReadUInt16BigEndian(buffer) * 360.0 / 65536.0;
        buffer = buffer[2..];
    }

    public override void Serialize(ref Span<byte> buffer)
    {
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(RhoStartNm * 128.0));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(RhoEndNm * 128.0));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(ThetaStartDeg / (360.0 / 65536.0)));
        buffer = buffer[2..];
        BinaryPrimitives.WriteUInt16BigEndian(buffer, (ushort)Math.Round(ThetaEndDeg / (360.0 / 65536.0)));
        buffer = buffer[2..];
    }

    public override int GetByteSize() => 8;
}