using System;
using System.Linq;
using Xunit;

namespace Asv.Gnss;

public class RtcmV2EphemerisSerializationTest
{
    // Independently packed from RTCM 2.3 table 4-17, including both three-bit fill fields.
    private const string EphemerisPacket =
        "6645552695A2E97B2EAB3039A460ADCFC75BA0FB2E12345678F6D7A1234567D431" +
        "B669FD2E30393ADE68B1A460CBC103165BA0F6040F6543210FAAAD297905B9A6DD";

    private static readonly DateTime GpsEpoch = new(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Deserialize_Type17IndependentPacket_ShouldDecodeAllFieldsAndConsumeExactlyTwentyWords()
    {
        var packet = Convert.FromHexString(EphemerisPacket);
        ReadOnlySpan<byte> input = [.. packet, 0xDE, 0xAD, 0xBE];
        var message = new RtcmV2Message17();

        message.Deserialize(ref input);

        Assert.Equal(new byte[] { 0xDE, 0xAD, 0xBE }, input.ToArray());
        Assert.Equal(66, message.GetByteSize());
        Assert.Equal((ushort)341, message.ReferenceStationId);
        Assert.Equal(740.4, message.ZCount, 8);
        Assert.Equal((byte)5, message.SequenceNumber);
        Assert.Equal(0.5, message.Udre);
        Assert.Equal((uint)933, message.WeekNumberRaw);
        Assert.Equal(933, message.WeekNumber % 1024);
        Assert.Equal(32, message.Satellite);
        AssertScaled(-1234 * Math.ScaleB(Math.PI, -43), message.Idot);
        Assert.Equal((uint)171, message.Iode);
        Assert.Equal(197520.0, (message.Toc - GpsEpoch).TotalSeconds % 604800.0);
        AssertScaled(-23456 * Math.ScaleB(1.0, -43), message.AF1);
        AssertScaled(-83 * Math.ScaleB(1.0, -55), message.AF2);
        Assert.Equal(-12345 / 32.0, message.Crs);
        AssertScaled(23456 * Math.ScaleB(Math.PI, -43), message.DeltaN);
        AssertScaled(-1234 * Math.ScaleB(1.0, -29), message.Cuc);
        AssertScaled(0x12345678U * Math.ScaleB(1.0, -33), message.E);
        Assert.Equal((short)-2345, message.Cus);
        var sqrtA = 0xA1234567U * Math.ScaleB(1.0, -19);
        Assert.Equal(sqrtA * sqrtA, message.A);
        Assert.Equal((ushort)54321, message.Toes);
        Assert.Equal(54321.0, (message.Toe - GpsEpoch).TotalSeconds % 604800.0);
        AssertScaled(-1234567890 * Math.ScaleB(Math.PI, -31), message.Omega0);
        AssertScaled(12345 * Math.ScaleB(1.0, -29), message.Cic);
        AssertScaled(987654321 * Math.ScaleB(Math.PI, -31), message.I0);
        AssertScaled(-23456 * Math.ScaleB(1.0, -29), message.Cis);
        AssertScaled(-876543210 * Math.ScaleB(Math.PI, -31), message.Omega);
        Assert.Equal(23456 / 32.0, message.Crc);
        AssertScaled(-654321 * Math.ScaleB(Math.PI, -43), message.OmegaDot);
        AssertScaled(0x6543210F * Math.ScaleB(Math.PI, -31), message.M0);
        Assert.Equal((ushort)682, message.Iodc);
        AssertScaled(-1234567 * Math.ScaleB(1.0, -31), message.AF0);
        AssertScaled(-71 * Math.ScaleB(1.0, -31), message.Tgd);
        Assert.Equal((byte)2, message.CodeOnL2);
        Assert.Equal((byte)9, message.SVAccuracy);
        Assert.Equal((byte)45, message.SVHealth);
        Assert.Equal((byte)1, message.L2PDataFlag);
    }

    [Fact]
    public void Serialize_Type17TypedFields_ShouldMatchIndependentPacketAndLeaveFollowingBytesUntouched()
    {
        var message = CreateEphemeris();
        var output = new byte[68];
        output.AsSpan().Fill(0xEE);
        Span<byte> remaining = output;

        message.Serialize(ref remaining);

        Assert.Equal(2, remaining.Length);
        Assert.Equal(new byte[] { 0xEE, 0xEE }, remaining.ToArray());
        var expected = Convert.FromHexString(EphemerisPacket);
        WriteUnsigned(expected, 501, 3, 2);
        WriteUnsigned(expected, 525, 3, 2);
        Assert.Equal(expected, output[..66]);
        Assert.Equal(20U, ReadUnsigned(output, 40, 5));
        Assert.Equal(2U, ReadUnsigned(output, 501, 3));
        Assert.Equal(2U, ReadUnsigned(output, 525, 3));
    }

    [Fact]
    public void Serialize_Type17DecodedPacket_ShouldPreserveFillBitsAndApplyTypedChanges()
    {
        var packet = Convert.FromHexString(EphemerisPacket);
        WriteUnsigned(packet, 501, 3, 2);
        WriteUnsigned(packet, 525, 3, 6);
        ReadOnlySpan<byte> input = packet;
        var message = new RtcmV2Message17();
        message.Deserialize(ref input);
        Assert.Empty(input.ToArray());
        Assert.Equal(packet, Serialize(message));

        message.SVHealth = 7;
        message.Crs = -12.5;
        message.WeekNumber = 0;
        message.Toe = DateTime.MinValue;
        WriteUnsigned(packet, 518, 6, 7);
        WriteUnsigned(packet, 120, 16, unchecked((uint)-400));

        Assert.Equal(packet, Serialize(message));
        Assert.Equal(933U, ReadUnsigned(packet, 48, 10));
        Assert.Equal(54321U, ReadUnsigned(packet, 248, 16));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Serialize_Type17SignedLimits_ShouldUseTheWholeDeclaredBitWidth(bool maximum)
    {
        var message = CreateEphemeris();
        var idot = maximum ? 8191 : -8192;
        var af2 = maximum ? 127 : -128;
        var omega0 = maximum ? int.MaxValue : int.MinValue;
        var omegaDot = maximum ? 8388607 : -8388608;
        var af0 = maximum ? 2097151 : -2097152;
        message.Idot = idot * Math.ScaleB(Math.PI, -43);
        message.AF2 = af2 * Math.ScaleB(1.0, -55);
        message.Omega0 = omega0 * Math.ScaleB(Math.PI, -31);
        message.OmegaDot = omegaDot * Math.ScaleB(Math.PI, -43);
        message.AF0 = af0 * Math.ScaleB(1.0, -31);

        var packet = Serialize(message);

        Assert.Equal(idot, ReadSigned(packet, 58, 14));
        Assert.Equal(af2, ReadSigned(packet, 112, 8));
        Assert.Equal(omega0, ReadSigned(packet, 264, 32));
        Assert.Equal(omegaDot, ReadSigned(packet, 408, 24));
        Assert.Equal(af0, ReadSigned(packet, 474, 22));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void Serialize_Type17HalfScaleValues_ShouldRoundAwayFromZero(int sign)
    {
        var message = CreateEphemeris();
        message.Idot = sign * 0.5 * RtcmV3Protocol.P2_43 * RtcmV3Protocol.SC2RAD;
        message.AF0 = sign * 0.5 * RtcmV3Protocol.P2_31;

        var packet = Serialize(message);

        Assert.Equal(sign, ReadSigned(packet, 58, 14));
        Assert.Equal(sign, ReadSigned(packet, 474, 22));
    }

    [Theory]
    [InlineData("SatelliteZero")]
    [InlineData("Satellite33")]
    [InlineData("Week1024")]
    [InlineData("Iode256")]
    [InlineData("TocBeforeEpoch")]
    [InlineData("TocEndOfWeek")]
    [InlineData("NegativeAxis")]
    [InlineData("NonfiniteAxis")]
    [InlineData("AxisOverflow")]
    [InlineData("NegativeEccentricity")]
    [InlineData("EccentricityOverflow")]
    [InlineData("NonfiniteClock")]
    [InlineData("IdotOverflow")]
    [InlineData("CodeOverflow")]
    [InlineData("AccuracyOverflow")]
    [InlineData("HealthOverflow")]
    [InlineData("L2FlagOverflow")]
    public void Serialize_Type17UnrepresentableField_ShouldRejectInsteadOfTruncating(string invalidField)
    {
        var message = CreateEphemeris();
        switch (invalidField)
        {
            case "SatelliteZero": message.Satellite = 0; break;
            case "Satellite33": message.Satellite = 33; break;
            case "Week1024": message.WeekNumberRaw = 1024; break;
            case "Iode256": message.Iode = 256; break;
            case "TocBeforeEpoch": message.Toc = GpsEpoch.AddTicks(-1); break;
            case "TocEndOfWeek": message.Toc = GpsEpoch.AddSeconds(604785); break;
            case "NegativeAxis": message.A = -1; break;
            case "NonfiniteAxis": message.A = double.NaN; break;
            case "AxisOverflow": message.A = 8192.0 * 8192.0; break;
            case "NegativeEccentricity": message.E = -1; break;
            case "EccentricityOverflow": message.E = 0.5; break;
            case "NonfiniteClock": message.AF1 = double.PositiveInfinity; break;
            case "IdotOverflow": message.Idot = 8192 * Math.ScaleB(Math.PI, -43); break;
            case "CodeOverflow": message.CodeOnL2 = 4; break;
            case "AccuracyOverflow": message.SVAccuracy = 16; break;
            case "HealthOverflow": message.SVHealth = 64; break;
            case "L2FlagOverflow": message.L2PDataFlag = 2; break;
            default: throw new InvalidOperationException(invalidField);
        }

        Assert.ThrowsAny<ArgumentException>(() => Serialize(message));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(19)]
    [InlineData(21)]
    [InlineData(31)]
    public void Deserialize_Type17WrongDeclaredWordCount_ShouldReject(int words)
    {
        var packet = CreateRawPacket(17, new byte[words * 3]);

        Assert.Throws<ArgumentException>(() => DeserializeEphemeris(packet));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(93)]
    public void Serialize_Type21RawWords_ShouldMatchIndependentPacketAndConsumeDeclaredFrame(int payloadLength)
    {
        var data = Enumerable.Range(0, payloadLength).Select(index => (byte)(index * 37 + 0x80)).ToArray();
        var expected = CreateRawPacket(21, data);
        var message = new RtcmV2Message21 { RawData = data, ReferenceStationId = 341, ZCount = 740.4, SequenceNumber = 5, Udre = 0.5 };

        Assert.Equal(expected, Serialize(message));
        ReadOnlySpan<byte> input = [.. expected, 0xCA, 0xFE];
        var decoded = new RtcmV2Message21();
        decoded.Deserialize(ref input);

        Assert.Equal(data, decoded.RawData);
        Assert.Equal(new byte[] { 0xCA, 0xFE }, input.ToArray());
        Assert.Equal(6 + payloadLength, decoded.GetByteSize());
        Assert.Equal(expected, Serialize(decoded));
        decoded.RawData = [0x12, 0x34, 0x56];
        Assert.Equal(CreateRawPacket(21, decoded.RawData), Serialize(decoded));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(94)]
    [InlineData(96)]
    public void Serialize_Type21IncompleteOrOversizedRawWords_ShouldReject(int length)
    {
        var message = new RtcmV2Message21 { RawData = new byte[length] };

        Assert.Throws<ArgumentException>(() => message.GetByteSize());
        Assert.Throws<ArgumentException>(() => Serialize(message));
    }

    [Fact]
    public void Serialize_Type21NullRawWords_ShouldReject()
    {
        var message = new RtcmV2Message21 { RawData = null! };

        Assert.Throws<ArgumentNullException>(() => message.GetByteSize());
    }

    private static RtcmV2Message17 CreateEphemeris() => new()
    {
        ReferenceStationId = 341, ZCount = 740.4, SequenceNumber = 5, Udre = 0.5,
        WeekNumberRaw = 933, WeekNumber = 0, Satellite = 32,
        Idot = -1234 * Math.ScaleB(Math.PI, -43), Iode = 171,
        Toc = GpsEpoch.AddDays(2200 * 7).AddSeconds(197520),
        AF1 = -23456 * Math.ScaleB(1.0, -43), AF2 = -83 * Math.ScaleB(1.0, -55),
        Crs = -12345 / 32.0, DeltaN = 23456 * Math.ScaleB(Math.PI, -43),
        Cuc = -1234 * Math.ScaleB(1.0, -29), E = 0x12345678U * Math.ScaleB(1.0, -33),
        Cus = -2345, A = Math.Pow(0xA1234567U * Math.ScaleB(1.0, -19), 2), Toes = 54321,
        Omega0 = -1234567890 * Math.ScaleB(Math.PI, -31), Cic = 12345 * Math.ScaleB(1.0, -29),
        I0 = 987654321 * Math.ScaleB(Math.PI, -31), Cis = -23456 * Math.ScaleB(1.0, -29),
        Omega = -876543210 * Math.ScaleB(Math.PI, -31), Crc = 23456 / 32.0,
        OmegaDot = -654321 * Math.ScaleB(Math.PI, -43), M0 = 0x6543210F * Math.ScaleB(Math.PI, -31),
        Iodc = 682, AF0 = -1234567 * Math.ScaleB(1.0, -31), Tgd = -71 * Math.ScaleB(1.0, -31),
        CodeOnL2 = 2, SVAccuracy = 9, SVHealth = 45, L2PDataFlag = 1
    };

    private static byte[] Serialize(RtcmV2MessageBase message)
    {
        var result = new byte[message.GetByteSize()];
        Span<byte> output = result;
        message.Serialize(ref output);
        Assert.Empty(output.ToArray());
        return result;
    }

    private static void AssertScaled(double expected, double actual)
    {
        var tolerance = Math.Abs(expected) * 1e-13 + 1e-25;
        Assert.InRange(actual, expected - tolerance, expected + tolerance);
    }

    private static void DeserializeEphemeris(byte[] packet)
    {
        ReadOnlySpan<byte> input = packet;
        new RtcmV2Message17().Deserialize(ref input);
    }

    private static byte[] CreateRawPacket(uint type, byte[] payload)
    {
        var packet = new byte[6 + payload.Length];
        WriteUnsigned(packet, 0, 8, 0x66);
        WriteUnsigned(packet, 8, 6, type);
        WriteUnsigned(packet, 14, 10, 341);
        WriteUnsigned(packet, 24, 13, 1234);
        WriteUnsigned(packet, 37, 3, 5);
        WriteUnsigned(packet, 40, 5, (uint)(payload.Length / 3));
        WriteUnsigned(packet, 45, 3, 2);
        payload.CopyTo(packet, 6);
        return packet;
    }

    private static uint ReadUnsigned(byte[] packet, int offset, int bitCount)
    {
        uint value = 0;
        for (var index = offset; index < offset + bitCount; index++)
        {
            value = (value << 1) | (uint)((packet[index / 8] >> (7 - index % 8)) & 1);
        }
        return value;
    }

    private static int ReadSigned(byte[] packet, int offset, int bitCount)
    {
        var value = ReadUnsigned(packet, offset, bitCount);
        return bitCount == 32 ? unchecked((int)value) : (int)(value << (32 - bitCount)) >> (32 - bitCount);
    }

    private static void WriteUnsigned(byte[] packet, int offset, int bitCount, uint value)
    {
        for (var index = 0; index < bitCount; index++)
        {
            var bit = offset + index;
            var mask = 1 << (7 - bit % 8);
            packet[bit / 8] = (byte)((packet[bit / 8] & ~mask) | (((value >> (bitCount - index - 1)) & 1) != 0 ? mask : 0));
        }
    }
}
