using System;
using System.Buffers.Binary;
using Xunit;

namespace Asv.Gnss;

public class UbxNavPvtTest
{
    [Theory]
    [InlineData(0, 0L)]
    [InlineData(250000000, 2500000L)]
    [InlineData(-250000000, -2500000L)]
    [InlineData(123456789, 1234568L)]
    [InlineData(-123456789, -1234568L)]
    public void Deserialize_ValidTimeAndSignedNanoseconds_ShouldPopulateUtcAndNavigation(int nanoseconds, long ticks)
    {
        var payload = CreatePayload();
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(16), nanoseconds);

        var message = UbxTestHelper.Deserialize<UbxNavPvt>(UbxTestHelper.CreateFrame(0x01, 0x07, payload));

        Assert.Equal(new DateTime(2024, 1, 7, 12, 34, 56, DateTimeKind.Utc).AddTicks(ticks), message.UtcTime);
        Assert.Equal(DateTimeKind.Utc, message.UtcTime.Kind);
        Assert.Equal(new DateTime(2024, 1, 7, 12, 35, 14, DateTimeKind.Utc), message.GpsTime);
        Assert.Equal((double)nanoseconds, message.UTCFractionOfSecond);
        AssertNavigation(message);
    }

    [Theory]
    [InlineData(0, 0L)]
    [InlineData(250000000, 2500000L)]
    [InlineData(-250000000, -2500000L)]
    public void Deserialize_LeapSecond_ShouldNormalizeUtcAndPreserveNavigation(int nanoseconds, long ticks)
    {
        var payload = CreatePayload();
        BinaryPrimitives.WriteUInt32LittleEndian(payload, 17000);
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(4), 2016);
        payload[6] = 12;
        payload[7] = 31;
        payload[8] = 23;
        payload[9] = 59;
        payload[10] = 60;
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(16), nanoseconds);

        var message = UbxTestHelper.Deserialize<UbxNavPvt>(UbxTestHelper.CreateFrame(0x01, 0x07, payload));

        Assert.Equal(new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddTicks(ticks), message.UtcTime);
        Assert.Equal((byte)60, message.Sec);
        Assert.Equal(new DateTime(2017, 1, 1, 0, 0, 17, DateTimeKind.Utc), message.GpsTime);
        AssertNavigation(message);
    }

    [Theory]
    [InlineData(2024, 1, 7, 12, 34, 56, 0)]
    [InlineData(2024, 1, 7, 12, 34, 56, 1)]
    [InlineData(2024, 1, 7, 12, 34, 56, 2)]
    [InlineData(0, 1, 7, 12, 34, 56, 7)]
    [InlineData(2024, 0, 7, 12, 34, 56, 7)]
    [InlineData(2024, 13, 7, 12, 34, 56, 7)]
    [InlineData(2024, 2, 30, 12, 34, 56, 7)]
    [InlineData(2024, 1, 0, 12, 34, 56, 7)]
    [InlineData(2024, 1, 7, 24, 34, 56, 7)]
    [InlineData(2024, 1, 7, 12, 60, 56, 7)]
    [InlineData(2024, 1, 7, 12, 34, 61, 7)]
    [InlineData(9999, 12, 31, 23, 59, 60, 7)]
    public void Deserialize_InvalidOrUnconfirmedCalendar_ShouldKeepNavigationAndUseUnknownTime(
        ushort year, byte month, byte day, byte hour, byte minute, byte second, byte validity)
    {
        var payload = CreatePayload();
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(4), year);
        payload[6] = month;
        payload[7] = day;
        payload[8] = hour;
        payload[9] = minute;
        payload[10] = second;
        payload[11] = validity;

        var message = UbxTestHelper.Deserialize<UbxNavPvt>(UbxTestHelper.CreateFrame(0x01, 0x07, payload));

        AssertUnknownTime(message);
        Assert.Equal(year, message.Year);
        Assert.Equal(second, message.Sec);
        AssertNavigation(message);
    }

    [Fact]
    public void Deserialize_ReusedMessageWithInvalidTime_ShouldClearPreviousDerivedTimestamps()
    {
        var payload = CreatePayload();
        var message = UbxTestHelper.Deserialize<UbxNavPvt>(UbxTestHelper.CreateFrame(0x01, 0x07, payload));
        Assert.NotEqual(DateTime.MinValue, message.UtcTime);
        payload[11] = 0;
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(28), 502500000);

        UbxTestHelper.Deserialize(message, UbxTestHelper.CreateFrame(0x01, 0x07, payload));

        AssertUnknownTime(message);
        Assert.Equal(50.25, message.Latitude, 7);
        Assert.Equal(37.5, message.Longitude, 7);
        Assert.True(message.GnssFixOK);
    }

    private static byte[] CreatePayload()
    {
        var payload = new byte[92];
        BinaryPrimitives.WriteUInt32LittleEndian(payload, 45314000);
        BinaryPrimitives.WriteUInt16LittleEndian(payload.AsSpan(4), 2024);
        payload[6] = 1;
        payload[7] = 7;
        payload[8] = 12;
        payload[9] = 34;
        payload[10] = 56;
        payload[11] = 7;
        payload[20] = 3;
        payload[21] = 0x81;
        payload[23] = 12;
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(24), 375000000);
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(28), -332500000);
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(32), 123456);
        BinaryPrimitives.WriteInt32LittleEndian(payload.AsSpan(36), 100000);
        return payload;
    }

    private static void AssertNavigation(UbxNavPvt message)
    {
        Assert.Equal(-33.25, message.Latitude, 7);
        Assert.Equal(37.5, message.Longitude, 7);
        Assert.Equal(123.456, message.AltElipsoid, 3);
        Assert.Equal(100.0, message.AltMsl, 3);
        Assert.True(message.GnssFixOK);
        Assert.Equal(UbxCarrierSolutionStatus.FixedAmbiguities, message.CarrierSolution);
        Assert.Equal((byte)12, message.NumberOfSatellites);
    }

    private static void AssertUnknownTime(UbxNavPvt message)
    {
        Assert.Equal(DateTime.MinValue, message.UtcTime);
        Assert.Equal(DateTime.MinValue, message.GpsTime);
        Assert.Equal(DateTimeKind.Utc, message.UtcTime.Kind);
        Assert.Equal(DateTimeKind.Utc, message.GpsTime.Kind);
    }
}
