using System;
using Asv.Common;
using Asv.IO;

namespace Asv.Gnss;

public class UbxNavSvinPool : UbxMessageBase
{
    public override string Name => "UBX-NAV-SVIN-POOL";
    public override byte Class => 0x01;
    public override byte SubClass => 0x3B;

    protected override void SerializeContent(ref Span<byte> buffer)
    {

    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
    }

    protected override int GetContentByteSize() => 0;

    public override void Randomize(Random random)
    {

    }
}

// [SerializationNotSupported]
public class UbxNavSvin : UbxMessageBase
{
    public override string Name => "UBX-NAV-SVIN";
    public override byte Class => 0x01;
    public override byte SubClass => 0x3B;

    protected override void SerializeContent(ref Span<byte> buffer)
    {
        throw new NotImplementedException();
    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        Version = BinSerialize.ReadByte(ref buffer);
        Reserved11 = BinSerialize.ReadByte(ref buffer);
        Reserved12 = BinSerialize.ReadByte(ref buffer);
        Reserved13 = BinSerialize.ReadByte(ref buffer);

        ITow = BinSerialize.ReadUInt(ref buffer);
        Duration = BinSerialize.ReadUInt(ref buffer);
        MeanX = BinSerialize.ReadInt(ref buffer);
        MeanY = BinSerialize.ReadInt(ref buffer);
        MeanZ = BinSerialize.ReadInt(ref buffer);
        MeanXhp = BinSerialize.ReadSByte(ref buffer);
        MeanYhp = BinSerialize.ReadSByte(ref buffer);
        MeanZhp = BinSerialize.ReadSByte(ref buffer);
        Reserved2 = BinSerialize.ReadByte(ref buffer);

        Ecef = (X: MeanX * 0.01 + MeanXhp * 0.0001, Y: MeanY * 0.01 + MeanYhp * 0.0001,
            Z: MeanZ * 0.01 + MeanZhp * 0.0001);
        Accuracy = BinSerialize.ReadUInt(ref buffer) / 10000.0;
        Observations = BinSerialize.ReadUInt(ref buffer);
        Valid = BinSerialize.ReadByte(ref buffer) != 0;
        Active = BinSerialize.ReadByte(ref buffer) != 0;
        Reserved31 = BinSerialize.ReadByte(ref buffer);
        Reserved32 = BinSerialize.ReadByte(ref buffer);

        var position = UbxProtocol.Ecef2Pos(Ecef);
        var lat = position.X * 180.0 / Math.PI;
        var lon = position.Y * 180.0 / Math.PI;
        var alt = position.Z;
        Location = new GeoPoint(lat, lon, alt);
    }

    public byte Reserved32 { get; set; }

    public byte Reserved31 { get; set; }

    public byte Reserved2 { get; set; }

    public byte Reserved13 { get; set; }

    public byte Reserved12 { get; set; }

    public byte Reserved11 { get; set; }

    protected override int GetContentByteSize()
    {
        return 40;
    }

    public override void Randomize(Random random)
    {
        // TODO: Randomize for tests
    }

    public byte Version { get; set; }
    public uint ITow { get; set; }
    public uint Duration { get; set; }
    public int MeanX { get; set; }
    public int MeanY { get; set; }
    public int MeanZ { get; set; }
    public (double X, double Y, double Z) Ecef { get; set; }
    public sbyte MeanXhp { get; set; }
    public sbyte MeanYhp { get; set; }
    public sbyte MeanZhp { get; set; }
    public double Accuracy { get; set; }
    public uint Observations { get; set; }
    public bool Valid { get; set; }
    public bool Active { get; set; }

    public GeoPoint? Location { get; set; }
}
    