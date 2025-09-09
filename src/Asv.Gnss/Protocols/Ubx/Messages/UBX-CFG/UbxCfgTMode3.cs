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

public class UbxCfgTMode3 : UbxMessageBase
{
    public override string Name => "UBX-CFG-TMODE3";
    public override byte Class => 0x06;
    public override byte SubClass => 0x71;

    public byte Version { get; set; }

    /// <summary>
    /// Position is given in LAT/LON/ALT (default is ECEF)
    /// </summary>
    public bool IsGivenInLLA { get; set; }

    /// <summary>
    /// Receiver Mode
    /// </summary>
    public TMode3Enum Mode { get; set; }

    /// <summary>
    /// Survey-in position accuracy limit
    /// </summary>
    public double SurveyInPositionAccuracyLimit { get; set; }

    /// <summary>
    /// Survey-in minimum duration
    /// </summary>
    public uint SurveyInMinDuration { get; set; }

    /// <summary>
    /// Fixed position 3D accuracy
    /// </summary>
    public double FixedPosition3DAccuracy { get; set; }

    public GeoPoint? Location { get; set; }

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

            var lat = (int)Math.Round(Location.Value.Latitude * 1e7);
            var lon = (int)Math.Round(Location.Value.Longitude * 1e7);
            var alt = (int)Math.Round(Location.Value.Altitude * 100.0);
            var xpX = (long)Math.Round(Location.Value.Latitude * 1e9);
            var xpY = (long)Math.Round(Location.Value.Longitude * 1e9);
            var xpZ = (long)Math.Round(Location.Value.Altitude * 10000.0);
            var latHp = (byte)(xpX - (long)lat * 100);
            var lonHp = (byte)(xpY - (long)lon * 100);
            var altHp = (byte)(xpZ - (long)alt * 100);

            BinSerialize.WriteInt(ref buffer, lat);
            BinSerialize.WriteInt(ref buffer, lon);
            BinSerialize.WriteInt(ref buffer, alt);

            BinSerialize.WriteByte(ref buffer, latHp);
            BinSerialize.WriteByte(ref buffer, lonHp);
            BinSerialize.WriteByte(ref buffer, altHp);
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