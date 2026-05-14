using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Asv.Gnss;
using Asv.IO;
using Xunit;

namespace Asv.Gnss.Test;

public class ComNavProtocolTest
{
    private const string NovatelOem7PsrPosAFileName = "novatel-oem7-psrposa.txt";
    private const string NovatelBestPosBFileName = "novatel-bestposb.hex";

    [Fact]
    public void Serialize_SaveConfigCommand_ShouldWriteAsciiCommand()
    {
        var command = ComNavSaveConfigCommand.Default;
        var data = new byte[command.GetByteSize()];
        var buffer = new Span<byte>(data);

        command.Serialize(ref buffer);

        Assert.Equal(0, buffer.Length);
        Assert.Equal("SAVECONFIG\r\n", Encoding.ASCII.GetString(data));
        Assert.Equal(new ComNavAsciiMessageId("saveconfig"), ((IProtocolMessage<ComNavAsciiMessageId>)command).Id);
    }

    [Fact]
    public void Deserialize_SimpleOkAnswer_ShouldReadBody()
    {
        var data = Encoding.ASCII.GetBytes("OK! command accepted");
        var buffer = new ReadOnlySpan<byte>(data);
        var message = new ComNavSimpleOkMessage();

        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);
        Assert.Equal("command accepted", message.Body);
        Assert.Equal(ComNavProtocol.SimpleAnswerInfo, message.Protocol);
    }

    [Fact]
    public void Deserialize_BinaryPsrPos_ShouldMatchReferencePayload()
    {
        var data = CreatePsrPosPacket();
        var buffer = new ReadOnlySpan<byte>(data);
        var message = new ComNavBinaryPsrPosPacket();

        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);
        Assert.Equal(ComNavBinaryPsrPosPacket.ComNavMessageId, message.MessageId);
        Assert.Equal(ComNavPortEnum.COM1, message.Source);
        Assert.Equal(ComNavTimeStatusEnum.FINE, message.TimeStatus);
        Assert.Equal(2234, message.GpsWeek);
        Assert.Equal(250000u, message.GpsMSecs);
        Assert.Equal(ComNavSolutionStatus.SolComputed, message.SolutionStatus);
        Assert.Equal(ComNavPositionType.Single, message.PositionType);
        Assert.Equal(55.7558, message.Latitude, 7);
        Assert.Equal(37.6173, message.Longitude, 7);
        Assert.Equal(180.25, message.HeightMsl, 7);
        Assert.Equal(ComNavDatum.Wgs84, message.Datum);
        Assert.Equal("BASE", message.BaseStationId);
        Assert.Equal(12, message.TracketSats);
        Assert.Equal(10, message.SolutionSats);
    }

    [Fact]
    public void Deserialize_NovatelOem7PsrPosA_ShouldMatchPublishedReference()
    {
        var source = ReadComNavResource(NovatelOem7PsrPosAFileName);
        var data = ConvertPublishedAsciiLogToParserFrame(source);
        var buffer = new ReadOnlySpan<byte>(data);
        var message = new ComNavPsrPosAMessage();

        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);
        Assert.Equal(ComNavPsrPosAMessage.ComNavMessageId, message.MessageId);
        Assert.Equal(ComNavTimeStatusEnum.FINESTEERING, message.TimeStatus);
        Assert.Equal(2209, message.GpsTime.Subtract(new DateTime(1980, 1, 6, 0, 0, 0, DateTimeKind.Utc)).Days / 7);
        Assert.Equal(ComNavSolutionStatus.SolComputed, message.SolutionStatus);
        Assert.Equal("SOL_COMPUTED", message.SolutionStatusName);
        Assert.Equal(ComNavPositionType.Sbas, message.PositionType);
        Assert.Equal("WAAS", message.PositionTypeName);
        Assert.Equal(51.15043801969, message.Latitude, 11);
        Assert.Equal(-114.03066782703, message.Longitude, 11);
        Assert.Equal(1096.7864, message.HeightMsl, 4);
        Assert.Equal(-17.0000f, message.Undulation, 4);
        Assert.Equal(ComNavDatum.Wgs84, message.Datum);
        Assert.Equal("WGS84", message.DatumName);
        Assert.Equal(0.9069f, message.LatitudeSd, 4);
        Assert.Equal(0.8826f, message.LongitudeSd, 4);
        Assert.Equal(1.8779f, message.HeightMslSd, 4);
        Assert.Equal("133", message.BaseStationId);
        Assert.Equal(4.000f, message.DifferentialAgeSec, 3);
        Assert.Equal(0.000f, message.SolutionAgeSec, 3);
        Assert.Equal(45, message.TrackedSats);
        Assert.Equal(10, message.SolutionSats);
        Assert.Equal(0x06, message.ExtSolutionStatus);
        Assert.Equal(0x03, message.GpsGlonassSignalMask);
    }

    [Fact]
    public void Deserialize_NovatelBestPosB_ShouldMatchPublishedReference()
    {
        var data = ReadHexResource(NovatelBestPosBFileName);
        var expectedCrc = BitConverter.ToUInt32(data, data.Length - sizeof(uint));

        Assert.Equal(ComNavCrc32.Calc(data, 0, data.Length - sizeof(uint)), expectedCrc);

        var buffer = new ReadOnlySpan<byte>(data);
        var message = new ComNavBinaryBestPosPacket();

        message.Deserialize(ref buffer);

        Assert.Equal(0, buffer.Length);
        Assert.Equal(ComNavBinaryBestPosPacket.ComNavMessageId, message.MessageId);
        Assert.Equal(72, message.MessageLength);
        Assert.Equal((ComNavPortEnum)32, message.Source);
        Assert.Equal(ComNavTimeStatusEnum.FINESTEERING, message.TimeStatus);
        Assert.Equal(1803, message.GpsWeek);
        Assert.Equal(27_504_000u, message.GpsMSecs);
        Assert.Equal(9603, message.ReceiverSwVersion);
        Assert.Equal(ComNavSolutionStatus.SolComputed, message.SolutionStatus);
        Assert.Equal(ComNavPositionType.L1Float, message.PositionType);
        Assert.Equal(32.81519645735, message.Latitude, 11);
        Assert.Equal(35.00791046612, message.Longitude, 11);
        Assert.Equal(19.9359, message.HeightMsl, 4);
        Assert.Equal(20.3000f, message.Undulation, 4);
        Assert.Equal(ComNavDatum.Wgs84, message.Datum);
        Assert.Equal(0.0363f, message.LatitudeSd, 4);
        Assert.Equal(0.0538f, message.LongitudeSd, 4);
        Assert.Equal(0.0956f, message.HeightMslSd, 4);
        Assert.Equal("0", message.BaseStationId);
        Assert.Equal(3.000f, message.DifferentialAgeSec, 3);
        Assert.Equal(0.000f, message.SolutionAgeSec, 3);
        Assert.Equal(18, message.TrackedSats);
        Assert.Equal(9, message.SolutionSats);
        Assert.Equal(9, message.L1CarrierPhaseSats);
        Assert.Equal(0, message.L2CarrierPhaseSats);
        Assert.Equal(0, message.ExtSolutionStatus);
        Assert.Equal(1, message.SignalMask);
    }

    private static byte[] CreatePsrPosPacket()
    {
        var bytes = new List<byte>(ComNavBinaryPsrPosPacket.ComNavMessageId);

        WriteByte(bytes, ComNavBinaryParser.FirstSyncByte);
        WriteByte(bytes, ComNavBinaryParser.SecondSyncByte);
        WriteByte(bytes, ComNavBinaryParser.ThirdSyncByte);
        WriteByte(bytes, 28);
        WriteUInt16(bytes, ComNavBinaryPsrPosPacket.ComNavMessageId);
        WriteByte(bytes, 2);
        WriteByte(bytes, (byte)ComNavPortEnum.COM1);
        WriteUInt16(bytes, 72);
        WriteUInt16(bytes, 1);
        WriteByte(bytes, 0);
        WriteByte(bytes, (byte)ComNavTimeStatusEnum.FINE);
        WriteUInt16(bytes, 2234);
        WriteUInt32(bytes, 250000);
        WriteUInt32(bytes, 0);
        WriteUInt16(bytes, 0);
        WriteUInt16(bytes, 123);

        WriteUInt32(bytes, (uint)ComNavSolutionStatus.SolComputed);
        WriteUInt32(bytes, (uint)ComNavPositionType.Single);
        WriteDouble(bytes, 55.7558);
        WriteDouble(bytes, 37.6173);
        WriteDouble(bytes, 180.25);
        WriteSingle(bytes, 14.5f);
        WriteUInt32(bytes, (uint)ComNavDatum.Wgs84);
        WriteSingle(bytes, 0.11f);
        WriteSingle(bytes, 0.22f);
        WriteSingle(bytes, 0.33f);
        bytes.AddRange(Encoding.ASCII.GetBytes("BASE"));
        WriteSingle(bytes, 1.5f);
        WriteSingle(bytes, 2.5f);
        WriteByte(bytes, 12);
        WriteByte(bytes, 10);
        WriteByte(bytes, 0);
        WriteByte(bytes, 0);
        WriteByte(bytes, 0);
        WriteByte(bytes, 0);
        WriteByte(bytes, 0);
        WriteByte(bytes, 0);

        var crc = ComNavCrc32.Calc(bytes.ToArray(), bytes.Count);
        WriteUInt32(bytes, crc);
        return bytes.ToArray();
    }

    private static void WriteByte(List<byte> bytes, byte value) => bytes.Add(value);

    private static void WriteUInt16(List<byte> bytes, ushort value) => bytes.AddRange(BitConverter.GetBytes(value));

    private static void WriteUInt32(List<byte> bytes, uint value) => bytes.AddRange(BitConverter.GetBytes(value));

    private static void WriteSingle(List<byte> bytes, float value) => bytes.AddRange(BitConverter.GetBytes(value));

    private static void WriteDouble(List<byte> bytes, double value) => bytes.AddRange(BitConverter.GetBytes(value));

    private static string ReadComNavResource(string fileName)
    {
        return File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Resources", "ComNav", fileName))
            .First(line => line.StartsWith("#PSRPOSA", StringComparison.Ordinal));
    }

    private static byte[] ReadHexResource(string fileName)
    {
        return File.ReadLines(Path.Combine(AppContext.BaseDirectory, "Resources", "ComNav", fileName))
            .Where(line => !line.StartsWith('#') && !string.IsNullOrWhiteSpace(line))
            .SelectMany(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Select(x => byte.Parse(x, NumberStyles.HexNumber, CultureInfo.InvariantCulture))
            .ToArray();
    }

    private static byte[] ConvertPublishedAsciiLogToParserFrame(string source)
    {
        var starIndex = source.IndexOf('*');
        Assert.True(starIndex > 0);

        var dataWithoutCrc = source[..starIndex];
        var crcText = source[(starIndex + 1)..].Trim();
        var expectedCrc = uint.Parse(crcText, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        var payloadForCrc = Encoding.ASCII.GetBytes(dataWithoutCrc[1..]);
        Assert.Equal(expectedCrc, ComNavCrc32.Calc(payloadForCrc, payloadForCrc.Length));

        var data = new List<byte>(Encoding.ASCII.GetBytes(dataWithoutCrc));
        data.Add((byte)'*');
        WriteUInt32(data, expectedCrc);
        return data.ToArray();
    }
}
