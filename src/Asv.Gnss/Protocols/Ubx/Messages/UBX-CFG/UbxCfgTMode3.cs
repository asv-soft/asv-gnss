using System;
using Asv.Common;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// Receiver Mode
/// </summary>
public enum TMode3Enum : ushort
{
    Disabled = 0,
    SurveyIn = 1,
    FixedMode = 2

}

public class UbxCfgTMode3Pool : UbxMessageBase
{
    public override string Name => "UBX-CFG-TMODE3-POOL";
    public override byte Class => 0x06;
    public override byte SubClass => 0x71;

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

/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.10.37. Receiver and firmware support must be checked against the applicable interface description.</remarks>
/// <remarks>ZED-F9P integration manual UBX-18010802 R08, section 3.1.6, describes limited legacy UBX-CFG compatibility and recommends the configuration-item interface.</remarks>
/// <remarks>For ZED-F9P survey-in operation, see UBX-18010802 R08, section 3.1.5.5.1: both duration and accuracy requirements must be met. Do not feed RTCM corrections during survey-in.</remarks>
public partial class UbxCfgTMode3 : UbxMessageBase
{
    public override string Name => "UBX-CFG-TMODE3";
    public override byte Class => 0x06;
    public override byte SubClass => 0x71;

    private byte _version;

    /// <summary>
    /// Payload format version; the documented CFG-TMODE3 format uses version 0.
    /// </summary>
    public byte Version
    {
        get => _version;
        set => _version = value;
    }

    private bool _isGivenInLLA;

    /// <summary>
    /// Position is given in LAT/LON/ALT (default is ECEF)
    /// </summary>
    public bool IsGivenInLLA
    {
        get => _isGivenInLLA;
        set => _isGivenInLLA = value;
    }

    private TMode3Enum _mode;

    /// <summary>
    /// Receiver Mode
    /// </summary>
    public TMode3Enum Mode
    {
        get => _mode;
        set => _mode = value;
    }

    private double _surveyInPositionAccuracyLimit;

    /// <summary>
    /// Survey-in mean-position accuracy limit in meters; wire resolution is 0.1 mm. Survey-in completes only after this limit and the minimum duration are both satisfied.
    /// </summary>
    public double SurveyInPositionAccuracyLimit
    {
        get => _surveyInPositionAccuracyLimit;
        set => _surveyInPositionAccuracyLimit = value;
    }

    private uint _surveyInMinDuration;

    /// <summary>
    /// Minimum survey-in observation duration in seconds; completion also requires the configured position accuracy.
    /// </summary>
    public uint SurveyInMinDuration
    {
        get => _surveyInMinDuration;
        set => _surveyInMinDuration = value;
    }

    private double _fixedPosition3DAccuracy;

    /// <summary>
    /// Accuracy assigned to the fixed reference position, in meters; wire resolution is 0.1 mm.
    /// </summary>
    public double FixedPosition3DAccuracy
    {
        get => _fixedPosition3DAccuracy;
        set => _fixedPosition3DAccuracy = value;
    }

    private GeoPoint? _location;

    /// <summary>
    /// Fixed reference position: latitude and longitude in degrees and ellipsoidal height in meters. Required in FixedMode. The wire coordinates are geodetic or WGS84 ECEF according to IsGivenInLLA.
    /// </summary>
    public GeoPoint? Location
    {
        get => _location;
        set => _location = value;
    }

    protected override void SerializeContent(ref Span<byte> buffer)
    {
        BinSerialize.WriteByte(ref buffer, Version);
        BinSerialize.WriteByte(ref buffer, 0); // reserved

        BinSerialize.WriteUShort(ref buffer, (ushort)((ushort)Mode | ((IsGivenInLLA ? 1 : 0) << 8)));

        if (Mode == TMode3Enum.FixedMode)
        {
            if (Location.HasValue == false)
            {
                throw new Exception(
                    $"{nameof(Location)} must be not null, when {nameof(Mode)} == {nameof(TMode3Enum.FixedMode)}");
            }

            var location = Location.Value;
            var x = location.Latitude * 1e7;
            var y = location.Longitude * 1e7;
            var z = location.Altitude * 100.0;
            if (!IsGivenInLLA)
            {
                const double semiMajorAxis = 6378137.0;
                const double flattening = 1.0 / 298.257223563;
                const double eccentricitySquared = flattening * (2.0 - flattening);
                var latitude = location.Latitude * Math.PI / 180.0;
                var longitude = location.Longitude * Math.PI / 180.0;
                var sinLatitude = Math.Sin(latitude);
                var radius = semiMajorAxis / Math.Sqrt(1.0 - eccentricitySquared * sinLatitude * sinLatitude);
                x = (radius + location.Altitude) * Math.Cos(latitude) * Math.Cos(longitude) * 100.0;
                y = (radius + location.Altitude) * Math.Cos(latitude) * Math.Sin(longitude) * 100.0;
                z = (radius * (1.0 - eccentricitySquared) + location.Altitude) * sinLatitude * 100.0;
            }

            var coarseX = checked((int)Math.Round(x));
            var coarseY = checked((int)Math.Round(y));
            var coarseZ = checked((int)Math.Round(z));
            var highPrecisionX = checked((sbyte)(Math.Round(x * 100.0) - (long)coarseX * 100));
            var highPrecisionY = checked((sbyte)(Math.Round(y * 100.0) - (long)coarseY * 100));
            var highPrecisionZ = checked((sbyte)(Math.Round(z * 100.0) - (long)coarseZ * 100));

            BinSerialize.WriteInt(ref buffer, coarseX);
            BinSerialize.WriteInt(ref buffer, coarseY);
            BinSerialize.WriteInt(ref buffer, coarseZ);

            BinSerialize.WriteByte(ref buffer, unchecked((byte)highPrecisionX));
            BinSerialize.WriteByte(ref buffer, unchecked((byte)highPrecisionY));
            BinSerialize.WriteByte(ref buffer, unchecked((byte)highPrecisionZ));
        }
        else
        {
            for (int i = 0; i < 15; i++)
            {
                BinSerialize.WriteByte(ref buffer, 0);
            }
        }

        BinSerialize.WriteByte(ref buffer, 0); // reserved2

        BinSerialize.WriteUInt(ref buffer, (uint)Math.Round(FixedPosition3DAccuracy * 10000.0));
        BinSerialize.WriteUInt(ref buffer, SurveyInMinDuration);
        BinSerialize.WriteUInt(ref buffer, (uint)Math.Round(SurveyInPositionAccuracyLimit * 10000.0));

        // reserved3
        for (int i = 0; i < 8; i++)
        {
            BinSerialize.WriteByte(ref buffer, 0);
        }

    }

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        Version = BinSerialize.ReadByte(ref buffer);
        var reserved = BinSerialize.ReadByte(ref buffer);
        var flags = BinSerialize.ReadUShort(ref buffer);
        Mode = (TMode3Enum)(flags & 0xFF);
        IsGivenInLLA = (flags & 0x100) != 0;

        if (Mode == TMode3Enum.FixedMode)
        {
            var ecefXorLat = BinSerialize.ReadInt(ref buffer);
            var ecefYorLon = BinSerialize.ReadInt(ref buffer);
            var ecefZorAlt = BinSerialize.ReadInt(ref buffer);
            var ecefXOrLatHP = (sbyte)BinSerialize.ReadByte(ref buffer);
            var ecefYOrLonHP = (sbyte)BinSerialize.ReadByte(ref buffer);
            var ecefZOrAltHP = (sbyte)BinSerialize.ReadByte(ref buffer);
            BinSerialize.ReadByte(ref buffer); // reserved 2
            if (!IsGivenInLLA)
            {
                (double X, double Y, double Z) ecef;
                ecef.X = ecefXorLat * 0.01 + ecefXOrLatHP * 0.0001;
                ecef.Y = ecefYorLon * 0.01 + ecefYOrLonHP * 0.0001;
                ecef.Z = ecefZorAlt * 0.01 + ecefZOrAltHP * 0.0001;

                var position = UbxProtocol.Ecef2Pos(ecef);
                var lat = position.X * 180.0 / Math.PI;
                var lon = position.Y * 180.0 / Math.PI;
                var alt = position.Z;
                Location = new GeoPoint(lat, lon, alt);
            }
            else
            {
                var lat = ecefXorLat * 1e-7 + ecefXOrLatHP * 1e-9;
                var lon = ecefYorLon * 1e-7 + ecefYOrLonHP * 1e-9;
                var alt = ecefZorAlt * 0.01 + ecefZOrAltHP * 0.0001;
                Location = new GeoPoint(lat, lon, alt);
            }
        }
        else
        {
            buffer = buffer.Slice(16);
            Location = null;
        }

        FixedPosition3DAccuracy = BinSerialize.ReadUInt(ref buffer) * 0.0001;
        SurveyInMinDuration = BinSerialize.ReadUInt(ref buffer);
        SurveyInPositionAccuracyLimit = BinSerialize.ReadUInt(ref buffer) * 0.0001;

        buffer = buffer.Slice(8);

    }

    protected override int GetContentByteSize() => 40;

    public override void Randomize(Random random)
    {
        Location = new GeoPoint(random.Next(-90, 90), random.Next(0, 180), random.Next(-1000, 1000));
        FixedPosition3DAccuracy = Math.Round(random.NextDouble() * 10, 1);
        IsGivenInLLA = true;
        Mode = TMode3Enum.FixedMode;
        SurveyInPositionAccuracyLimit = Math.Round(random.NextDouble() * 10, 1);
        SurveyInMinDuration = (uint)random.Next(1, 1000);
    }
}
