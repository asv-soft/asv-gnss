using System;
using System.Collections.Generic;
using System.Text;
using Asv.Gnss;
using Asv.IO;
using Xunit;

namespace Asv.Gnss.Test;

public class ComNavProtocolTest
{
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
}
