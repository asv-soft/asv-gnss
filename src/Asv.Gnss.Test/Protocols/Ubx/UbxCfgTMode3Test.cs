using System;
using System.Buffers.Binary;
using Asv.Common;
using Xunit;

namespace Asv.Gnss;

public class UbxCfgTMode3Test
{
    [Theory]
    [InlineData(55.123456789, -37.987654321, 123.4567, 551234568, -379876543, 12346, -11, -21, -33)]
    [InlineData(-55.123456789, 37.987654321, -123.4567, -551234568, 379876543, -12346, 11, 21, 33)]
    public void Serialize_GeodeticPosition_ShouldEncodeDocumentedUnitsAndSignedPrecision(
        double latitude, double longitude, double altitude, int x, int y, int z, sbyte hpX, sbyte hpY, sbyte hpZ)
    {
        var message = new UbxCfgTMode3
        {
            Mode = TMode3Enum.FixedMode,
            IsGivenInLLA = true,
            Location = new GeoPoint(latitude, longitude, altitude),
            FixedPosition3DAccuracy = 0.1234
        };

        var frame = UbxTestHelper.Serialize(message);
        var payload = frame.AsSpan(6, 40);

        Assert.Equal(0x0102, BinaryPrimitives.ReadUInt16LittleEndian(payload[2..]));
        AssertWirePosition(payload, x, y, z, hpX, hpY, hpZ);
        Assert.Equal(1234U, BinaryPrimitives.ReadUInt32LittleEndian(payload[20..]));
        var decoded = UbxTestHelper.Deserialize<UbxCfgTMode3>(frame);
        AssertLocation(decoded.Location, latitude, longitude, altitude);
    }

    [Theory]
    [InlineData(0, 0, 0.1234, 637813712, 0, 0, 34, 0, 0)]
    [InlineData(0, 90, -0.1234, 0, 637813688, 0, 0, -34, 0)]
    [InlineData(0, 180, 0.1234, -637813712, 0, 0, -34, 0, 0)]
    [InlineData(45, 45, 100, 319446915, 319446915, 448741912, -49, -49, -5)]
    public void Serialize_EcefPosition_ShouldConvertGeodeticLocationToWgs84WireCoordinates(
        double latitude, double longitude, double altitude, int x, int y, int z, sbyte hpX, sbyte hpY, sbyte hpZ)
    {
        var message = new UbxCfgTMode3
        {
            Mode = TMode3Enum.FixedMode,
            Location = new GeoPoint(latitude, longitude, altitude)
        };

        var frame = UbxTestHelper.Serialize(message);
        var payload = frame.AsSpan(6, 40);

        Assert.Equal(0x0002, BinaryPrimitives.ReadUInt16LittleEndian(payload[2..]));
        AssertWirePosition(payload, x, y, z, hpX, hpY, hpZ);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Deserialize_IndependentPositionFixture_ShouldInterpretCoordinateFlagAndSignedPrecision(bool geodetic)
    {
        var payload = new byte[40];
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(2), (ushort)(geodetic ? 0x0102 : 0x0002));
        if (geodetic)
        {
            BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(4), -551234568);
            BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(8), 379876543);
            BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(12), -12346);
            payload[16] = 11;
            payload[17] = 21;
            payload[18] = 33;
        }
        else
        {
            BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(4), 637813712);
            payload[16] = 34;
        }

        var frame = UbxTestHelper.CreateFrame(0x06, 0x71, payload);
        var message = UbxTestHelper.Deserialize<UbxCfgTMode3>(frame);

        Assert.Equal(geodetic, message.IsGivenInLLA);
        Assert.Equal(TMode3Enum.FixedMode, message.Mode);
        if (geodetic)
        {
            AssertLocation(message.Location, -55.123456789, 37.987654321, -123.4567);
        }
        else
        {
            AssertLocation(message.Location, 0, 0, 0.1234);
        }

        Assert.Equal(frame, UbxTestHelper.Serialize(message));
    }

    [Theory]
    [InlineData(TMode3Enum.Disabled)]
    [InlineData(TMode3Enum.SurveyIn)]
    public void Deserialize_NonFixedMode_ShouldClearPreviousFixedLocationAndReadSurveyLimits(TMode3Enum mode)
    {
        var message = new UbxCfgTMode3 { Location = new GeoPoint(45, 45, 100) };
        var payload = new byte[40];
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(2), (ushort)mode);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(20), 1234);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(24), 300);
        BinaryPrimitives.WriteUInt32LittleEndian(payload.AsSpan(28), 25000);

        UbxTestHelper.Deserialize(message, UbxTestHelper.CreateFrame(0x06, 0x71, payload));

        Assert.Null(message.Location);
        Assert.Equal(mode, message.Mode);
        Assert.Equal(0.1234, message.FixedPosition3DAccuracy, 8);
        Assert.Equal(300U, message.SurveyInMinDuration);
        Assert.Equal(2.5, message.SurveyInPositionAccuracyLimit, 8);
    }

    private static void AssertWirePosition(ReadOnlySpan<byte> payload, int x, int y, int z, sbyte hpX, sbyte hpY, sbyte hpZ)
    {
        Assert.Equal(x, BinaryPrimitives.ReadInt32LittleEndian(payload[4..]));
        Assert.Equal(y, BinaryPrimitives.ReadInt32LittleEndian(payload[8..]));
        Assert.Equal(z, BinaryPrimitives.ReadInt32LittleEndian(payload[12..]));
        Assert.Equal(hpX, unchecked((sbyte)payload[16]));
        Assert.Equal(hpY, unchecked((sbyte)payload[17]));
        Assert.Equal(hpZ, unchecked((sbyte)payload[18]));
    }

    private static void AssertLocation(GeoPoint? location, double latitude, double longitude, double altitude)
    {
        Assert.True(location.HasValue);
        var value = location.GetValueOrDefault();
        Assert.Equal(latitude, value.Latitude, 8);
        Assert.Equal(longitude, value.Longitude, 8);
        Assert.Equal(altitude, value.Altitude, 4);
    }
}
