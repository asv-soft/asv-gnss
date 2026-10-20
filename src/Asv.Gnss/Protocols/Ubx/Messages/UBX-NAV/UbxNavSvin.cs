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
/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.17.26. Receiver and firmware support must be checked against the applicable interface description.</remarks>
/// <remarks>For ZED-F9P survey-in operation, see UBX-18010802 R08, section 3.1.5.5.1: both duration and accuracy requirements must be met. Do not feed RTCM corrections during survey-in.</remarks>
public partial class UbxNavSvin : UbxMessageBase
{
    private byte _reserved32;
    private byte _reserved31;
    private byte _reserved2;
    private byte _reserved13;
    private byte _reserved12;
    private byte _reserved11;
    private byte _version;
    private uint _iTow;
    private uint _duration;
    private int _meanX;
    private int _meanY;
    private int _meanZ;
    private (double X, double Y, double Z) _ecef;
    private sbyte _meanXhp;
    private sbyte _meanYhp;
    private sbyte _meanZhp;
    private double _accuracy;
    private uint _observations;
    private bool _valid;
    private bool _active;
    private GeoPoint? _location;

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

    /// <summary>
    /// Reserved payload byte; no application meaning is defined by this protocol version.
    /// </summary>
    public byte Reserved32
    {
        get => _reserved32;
        set => _reserved32 = value;
    }

    /// <summary>
    /// Reserved payload byte; no application meaning is defined by this protocol version.
    /// </summary>
    public byte Reserved31
    {
        get => _reserved31;
        set => _reserved31 = value;
    }

    /// <summary>
    /// Reserved payload byte; no application meaning is defined by this protocol version.
    /// </summary>
    public byte Reserved2
    {
        get => _reserved2;
        set => _reserved2 = value;
    }

    /// <summary>
    /// Reserved payload byte; no application meaning is defined by this protocol version.
    /// </summary>
    public byte Reserved13
    {
        get => _reserved13;
        set => _reserved13 = value;
    }

    /// <summary>
    /// Reserved payload byte; no application meaning is defined by this protocol version.
    /// </summary>
    public byte Reserved12
    {
        get => _reserved12;
        set => _reserved12 = value;
    }

    /// <summary>
    /// Reserved payload byte; no application meaning is defined by this protocol version.
    /// </summary>
    public byte Reserved11
    {
        get => _reserved11;
        set => _reserved11 = value;
    }

    protected override int GetContentByteSize()
    {
        return 40;
    }

    public override void Randomize(Random random)
    {
        // TODO: Randomize for tests
    }

    /// <summary>
    /// Payload format version; the documented NAV-SVIN format uses version 0.
    /// </summary>
    public byte Version
    {
        get => _version;
        set => _version = value;
    }
    /// <summary>
    /// Navigation epoch as GPS time of week, in milliseconds.
    /// </summary>
    public uint ITow
    {
        get => _iTow;
        set => _iTow = value;
    }
    /// <summary>
    /// Elapsed survey-in observation time, in seconds.
    /// </summary>
    public uint Duration
    {
        get => _duration;
        set => _duration = value;
    }
    /// <summary>
    /// Coarse ECEF X coordinate of the mean position, in centimeters; add the corresponding high-precision component.
    /// </summary>
    public int MeanX
    {
        get => _meanX;
        set => _meanX = value;
    }
    /// <summary>
    /// Coarse ECEF Y coordinate of the mean position, in centimeters; add the corresponding high-precision component.
    /// </summary>
    public int MeanY
    {
        get => _meanY;
        set => _meanY = value;
    }
    /// <summary>
    /// Coarse ECEF Z coordinate of the mean position, in centimeters; add the corresponding high-precision component.
    /// </summary>
    public int MeanZ
    {
        get => _meanZ;
        set => _meanZ = value;
    }
    /// <summary>
    /// Mean Earth-centered, Earth-fixed position in meters, combining the centimeter coordinates and their high-precision components.
    /// </summary>
    public (double X, double Y, double Z) Ecef
    {
        get => _ecef;
        set => _ecef = value;
    }
    /// <summary>
    /// High-precision correction to the ECEF X coordinate, in units of 0.1 mm; documented range is -99 to 99.
    /// </summary>
    public sbyte MeanXhp
    {
        get => _meanXhp;
        set => _meanXhp = value;
    }
    /// <summary>
    /// High-precision correction to the ECEF Y coordinate, in units of 0.1 mm; documented range is -99 to 99.
    /// </summary>
    public sbyte MeanYhp
    {
        get => _meanYhp;
        set => _meanYhp = value;
    }
    /// <summary>
    /// High-precision correction to the ECEF Z coordinate, in units of 0.1 mm; documented range is -99 to 99.
    /// </summary>
    public sbyte MeanZhp
    {
        get => _meanZhp;
        set => _meanZhp = value;
    }
    /// <summary>
    /// Estimated accuracy of the mean position, in meters; the wire value has a scale of 0.1 mm.
    /// </summary>
    public double Accuracy
    {
        get => _accuracy;
        set => _accuracy = value;
    }
    /// <summary>
    /// Number of position observations included in the survey-in mean.
    /// </summary>
    public uint Observations
    {
        get => _observations;
        set => _observations = value;
    }
    /// <summary>
    /// Whether the survey-in position meets the configured requirements and is valid.
    /// </summary>
    public bool Valid
    {
        get => _valid;
        set => _valid = value;
    }
    /// <summary>
    /// Whether survey-in is currently collecting observations.
    /// </summary>
    public bool Active
    {
        get => _active;
        set => _active = value;
    }

    /// <summary>
    /// Mean position converted from ECEF to WGS84 latitude and longitude in degrees and ellipsoidal height in meters.
    /// </summary>
    public GeoPoint? Location
    {
        get => _location;
        set => _location = value;
    }
}
    