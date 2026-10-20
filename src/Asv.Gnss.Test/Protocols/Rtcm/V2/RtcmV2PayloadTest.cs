using System;
using Xunit;

namespace Asv.Gnss;

public class RtcmV2PayloadTest
{
    [Theory]
    [InlineData(1)]
    [InlineData(9)]
    [InlineData(31)]
    public void Serialize_CorrectionMessage_ShouldWriteHeaderAndSignedSatelliteCorrection(int type)
    {
        RtcmV2MessageBase message = type switch
        {
            1 => new RtcmV2Message1(),
            9 => new RtcmV2Message9(),
            _ => new RtcmV2Message31()
        };
        var item = new DObservationItem(type == 31 ? NavigationSystemEnum.SYS_GLO : NavigationSystemEnum.SYS_GPS)
        {
            Prn = 32, Prc = -0.02, Rrc = -0.002, Iod = 45, Udre = SatUdreEnum.BetweenOneAndFour
        };
        if (message is RtcmV2Message1 gps)
        {
            gps.ObservationItems = [item];
        }
        else
        {
            ((RtcmV2Message31)message).ObservationItems = [item];
        }
        message.ReferenceStationId = 1023;
        message.SequenceNumber = 7;
        message.ZCount = 12.6;
        message.Udre = 1;

        var encoded = Serialize(message);

        Assert.Equal(12, encoded.Length);
        Assert.Equal(new byte[] { 0x20, 0xFF, 0xFF, 0xFF, 0x2D }, encoded.AsSpan(6, 5).ToArray());
        var read = new ReadOnlySpan<byte>(encoded);
        message.Deserialize(ref read);
        Assert.True(read.IsEmpty);
        Assert.Equal(1023, message.ReferenceStationId);
        Assert.Equal((byte)7, message.SequenceNumber);
        Assert.Equal(12.6, message.ZCount, 10);
        var decoded = message is RtcmV2Message1 gpsResult
            ? Assert.Single(gpsResult.ObservationItems)
            : Assert.Single(((RtcmV2Message31)message).ObservationItems);
        Assert.Equal((byte)32, decoded.Prn);
        Assert.Equal(-0.02, decoded.Prc);
        Assert.Equal(-0.002, decoded.Rrc);
        Assert.Equal((byte)45, decoded.Iod);
        Assert.Equal(SatUdreEnum.BetweenOneAndFour, decoded.Udre);
    }

    [Theory]
    [InlineData(655.34, 0.254, 0, 655.34, 0.254)]
    [InlineData(-655.34, -0.254, 0, -655.34, -0.254)]
    [InlineData(10485.44, 4.064, 1, 10485.44, 4.064)]
    [InlineData(-10485.44, -4.064, 1, -10485.44, -4.064)]
    [InlineData(0.01, 0.001, 0, 0.02, 0.002)]
    [InlineData(-0.01, -0.001, 0, -0.02, -0.002)]
    public void Serialize_CorrectionBoundsAndMidpoints_ShouldChooseRepresentableScaleAndRoundAwayFromZero(
        double prc, double rrc, byte scale, double expectedPrc, double expectedRrc)
    {
        var item = new DObservationItem(NavigationSystemEnum.SYS_GPS) { Prn = 1, Prc = prc, Rrc = rrc };

        var data = SerializeItem(item);

        Assert.Equal(scale, data[0] >> 7);
        var decoded = new DObservationItem(NavigationSystemEnum.SYS_GPS);
        var index = 0;
        decoded.Deserialize(data, ref index);
        Assert.Equal(40, index);
        Assert.Equal(expectedPrc, decoded.Prc, 9);
        Assert.Equal(expectedRrc, decoded.Rrc, 9);
    }

    [Fact]
    public void Serialize_DecodedCoarseCorrection_ShouldPreserveScaleUntilChangedValuesNeedAnotherScale()
    {
        byte[] original = [0x83, 0, 10, 1, 77];
        var item = new DObservationItem(NavigationSystemEnum.SYS_GPS);
        var index = 0;
        item.Deserialize(original, ref index);

        Assert.Equal(original, SerializeItem(item));
        item.Prc = -1000;
        item.Rrc = 1.024;
        Assert.Equal(new byte[] { 0x83, 0xF3, 0xCB, 0x20, 77 }, SerializeItem(item));
    }

    [Fact]
    public void Serialize_ReusedGlonassCorrection_ShouldPreserveInvalidRawPairAndIodHighBitWithoutStaleState()
    {
        byte[] invalid = [0xE0, 0x80, 0, 7, 0xD5];
        var item = new DObservationItem(NavigationSystemEnum.SYS_GLO);
        var index = 0;
        item.Deserialize(invalid, ref index);
        Assert.True(double.IsNaN(item.Prc));
        Assert.True(double.IsNaN(item.Rrc));
        Assert.Equal((byte)0x55, item.Iod);
        Assert.Equal(invalid, SerializeItem(item));
        item.Iod = 0x56;
        Assert.Equal((byte)0xD6, SerializeItem(item)[4]);

        byte[] valid = [1, 0, 1, 2, 3];
        index = 0;
        item.Deserialize(valid, ref index);
        Assert.Equal(valid, SerializeItem(item));
        Assert.Equal(0.02, item.Prc);
        Assert.Equal(0.004, item.Rrc);
    }

    [Theory]
    [InlineData(double.PositiveInfinity, 0)]
    [InlineData(0, double.NegativeInfinity)]
    [InlineData(10486, 0)]
    [InlineData(0, 4.1)]
    public void Serialize_UnrepresentableCorrection_ShouldRejectInsteadOfWrapping(double prc, double rrc)
    {
        var item = new DObservationItem(NavigationSystemEnum.SYS_GPS) { Prn = 1, Prc = prc, Rrc = rrc };

        Assert.Throws<ArgumentOutOfRangeException>(() => SerializeItem(item));
    }

    [Fact]
    public void Serialize_NewUnavailableCorrection_ShouldUseReservedSentinels()
    {
        var item = new DObservationItem(NavigationSystemEnum.SYS_GPS) { Prn = 1, Prc = double.NaN, Rrc = double.NaN };

        Assert.Equal(new byte[] { 1, 0x80, 0, 0x80, 0 }, SerializeItem(item));
    }

    [Fact]
    public void Serialize_TimeOfWeek_ShouldUseGpsCalendarAndExplicitLeapSeconds()
    {
        var message = new RtcmV2Message14
        {
            GpsTime = new DateTime(2024, 1, 7, 12, 34, 56, DateTimeKind.Utc), LeapSeconds = 18, ZCount = 123.6, Udre = 1
        };

        var encoded = Serialize(message);

        Assert.Equal(new byte[] { 0x3E, 0x03, 0x12 }, encoded.AsSpan(6).ToArray());
        var read = new ReadOnlySpan<byte>(encoded);
        message.Deserialize(ref read);
        Assert.True(read.IsEmpty);
        Assert.Equal((byte)18, message.LeapSeconds);
        Assert.Equal(12, message.GpsTime.Hour);
        Assert.Equal(123.6, message.ZCount, 9);
        Assert.Equal(encoded, Serialize(message));
    }

    [Fact]
    public void Serialize_TimeBeforeGpsEpochOrOversizedLeapOffset_ShouldReject()
    {
        var message = new RtcmV2Message14 { GpsTime = DateTime.MinValue };
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(message));
        message.GpsTime = new DateTime(2024, 1, 7);
        message.LeapSeconds = 64;
        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(message));
    }

    [Fact]
    public void Deserialize_IonosphericRecord_ShouldIncludeReservedBitsAndPreserveLegacyNumericConversions()
    {
        var original = CreateFrame(15, [0xA0, 0x13, 0x4B, 0xFF, 0xBA, 0xAA]);
        var message = new RtcmV2Message15();
        var read = new ReadOnlySpan<byte>(original);

        message.Deserialize(ref read);

        Assert.True(read.IsEmpty);
        var item = Assert.Single(message.Delays);
        Assert.Equal(NavigationSystemEnum.SYS_GLO, item.NavigationSystem);
        Assert.Equal((byte)32, item.Prn);
        Assert.Equal(1.234, item.IonosphericDelay);
        Assert.Equal(-0.25, item.IonoRateOfChange);
        Assert.Equal(original, Serialize(message));
    }

    [Fact]
    public void Serialize_IonosphericRecords_ShouldPackTwoThirtySixBitRecordsWithoutGaps()
    {
        var message = new RtcmV2Message15
        {
            Delays =
            [
                new IonosphericDelayItem { NavigationSystem = NavigationSystemEnum.SYS_GPS, Prn = 2, IonosphericDelay = 16.383, IonoRateOfChange = 409.55 },
                new IonosphericDelayItem { NavigationSystem = NavigationSystemEnum.SYS_GLO, Prn = 32, IonosphericDelay = 0.001, IonoRateOfChange = -409.55 }
            ]
        };

        var encoded = Serialize(message);

        Assert.Equal(15, encoded.Length);
        Assert.Equal(new byte[] { 0x02, 0xFF, 0xFD, 0xFF }, encoded.AsSpan(6, 4).ToArray());
        var decoded = new RtcmV2Message15();
        var read = new ReadOnlySpan<byte>(encoded);
        decoded.Deserialize(ref read);
        Assert.True(read.IsEmpty);
        Assert.Equal(2, decoded.Delays.Length);
        Assert.Equal((byte)2, decoded.Delays[0].Prn);
        Assert.Equal(16.383, decoded.Delays[0].IonosphericDelay);
        Assert.Equal(409.55, decoded.Delays[0].IonoRateOfChange);
        Assert.Equal(NavigationSystemEnum.SYS_GLO, decoded.Delays[1].NavigationSystem);
        Assert.Equal((byte)32, decoded.Delays[1].Prn);
        Assert.Equal(0.001, decoded.Delays[1].IonosphericDelay);
        Assert.Equal(-409.55, decoded.Delays[1].IonoRateOfChange);
    }

    [Fact]
    public void Serialize_ReusedIonosphericRecord_ShouldPreserveInvalidDelayBitsAndClearThemAfterValidRead()
    {
        byte[] invalid = [0xC1, 0xC0, 0xE6, 0, 0x0A, 0xAA];
        var item = new IonosphericDelayItem();
        var index = 0;
        item.Deserialize(invalid, ref index);
        Assert.True(double.IsNaN(item.IonosphericDelay));
        Assert.True(double.IsNaN(item.IonoRateOfChange));
        var data = new byte[6];
        index = 0;
        item.Serialize(data, ref index);
        Assert.Equal(invalid.AsSpan(0, 4).ToArray(), data.AsSpan(0, 4).ToArray());
        Assert.Equal(invalid[4] & 0xF0, data[4] & 0xF0);

        byte[] valid = [0xA0, 0x13, 0x4B, 0xFF, 0xBA, 0xAA];
        index = 0;
        item.Deserialize(valid, ref index);
        Assert.Equal(1.234, item.IonosphericDelay);
        Assert.Equal(-0.25, item.IonoRateOfChange);
        index = 0;
        item.Serialize(data, ref index);
        Assert.Equal(valid.AsSpan(0, 4).ToArray(), data.AsSpan(0, 4).ToArray());
    }

    [Theory]
    [InlineData(-0.001, 0)]
    [InlineData(16.384, 0)]
    [InlineData(0, -409.6)]
    [InlineData(0, 409.6)]
    [InlineData(double.PositiveInfinity, 0)]
    public void Serialize_UnrepresentableIonosphericValues_ShouldRejectInsteadOfUsingInvalidSentinel(double delay, double rate)
    {
        var message = new RtcmV2Message15
        {
            Delays = [new IonosphericDelayItem { NavigationSystem = NavigationSystemEnum.SYS_GPS, Prn = 1, IonosphericDelay = delay, IonoRateOfChange = rate }]
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => Serialize(message));
    }

    private static byte[] Serialize(RtcmV2MessageBase message)
    {
        var data = new byte[message.GetByteSize()];
        var write = data.AsSpan();
        message.Serialize(ref write);
        Assert.True(write.IsEmpty);
        return data;
    }

    private static byte[] SerializeItem(DObservationItem item)
    {
        var data = new byte[5];
        var index = 0;
        item.Serialize(data, ref index);
        Assert.Equal(40, index);
        return data;
    }

    private static byte[] CreateFrame(ushort type, ReadOnlySpan<byte> payload)
    {
        var data = new byte[payload.Length + 6];
        data[0] = 0x66;
        data[1] = (byte)(type << 2);
        data[2] = 1;
        data[5] = (byte)(payload.Length / 3 << 3);
        payload.CopyTo(data.AsSpan(6));
        return data;
    }
}
