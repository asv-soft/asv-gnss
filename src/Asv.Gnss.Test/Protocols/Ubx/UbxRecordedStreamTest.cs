using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class UbxRecordedStreamTest
{
    [Theory]
    [InlineData("pygpsdata-NAV.bin", 5, 23, "d931b83094f6698d6245f5bcdadae8a909839abfb6171f5044512a463ddd0d8b", "aa39c4e9b63a2e2960ea1c9b5e80cce9f6313019a81031d4702ec0c1bff27735")]
    [InlineData("pygpsdata-MON.bin", 5, 9, "057c5ba542d60136ecca81931b3517bd1df36035bd944200a8e94e3c9053c453", "83f971bceced7abe5abbccc244d5708f484bdcdf01546f4ed5ebf8735d2568cc")]
    [InlineData("ublox-zed-f9p-sfrbx.bin", 95, 57, "465e6f4fceac5ad70928242fd9baee43ea82508a35a3ef838e006e223ab2107e", "26026749296b2dc5c2e68e4bfbd98ef69799e81706143aa46295acbdc6098981")]
    public void Push_RecordedCapture_ShouldPreserveSupportedMessagesAndWireOrder(
        string fileName, int messageCount, int unknownCount, string fileHash, string orderHash)
    {
        var data = ReadResource(fileName);
        Assert.Equal(fileHash, Hash(data));
        using var stream = new RecordedGnssStream();

        stream.Push(data, 17);

        Assert.Equal(messageCount, stream.Messages.Count);
        Assert.Equal(orderHash, HashMessageOrder(stream.Messages));
        Assert.Equal(unknownCount, stream.Errors.Count);
        Assert.All(stream.Errors, error => Assert.IsType<ProtocolParserUnknownMessageException>(error));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(257)]
    [InlineData(4096)]
    public void Push_MixedRecordedProtocols_ShouldPreserveMessagesAcrossChunkBoundaries(int chunkSize)
    {
        byte[] data =
        [
            .. ReadResource("pygpsdata-NAV.bin"),
            .. ReadNmeaSentence("GPRMC"),
            .. ReadResource("pygpsdata-MON.bin"),
            .. ReadNmeaSentence("GPGGA"),
            .. ReadResource("ublox-zed-f9p-sfrbx.bin")
        ];
        using var stream = new RecordedGnssStream();

        stream.Push(data, chunkSize);

        Assert.Equal(107, stream.Messages.Count);
        Assert.Equal("190dfd1c1dc081d7e0c5c4823701de71dbd0b088c4da06a5045c158c366caefa", HashMessageOrder(stream.Messages));
        Assert.Equal(20, stream.Messages.OfType<UbxNavPvt>().Count());
        Assert.Equal(20, stream.Messages.OfType<UbxNavDop>().Count());
        Assert.Equal(3, stream.Messages.OfType<UbxNavSat>().Count());
        Assert.Equal(55, stream.Messages.OfType<UbxRxmSfrbx>().Count());
        Assert.Single(stream.Messages.OfType<NmeaMessageRmc>());
        Assert.Single(stream.Messages.OfType<NmeaMessageGga>());
        Assert.Equal(89, stream.Errors.Count);
        Assert.All(stream.Errors, error => Assert.IsType<ProtocolParserUnknownMessageException>(error));
    }

    [Fact]
    public void Push_RecordedNavigation_ShouldDecodeSignedCoordinatesAndLargeSatellitePayload()
    {
        using var stream = new RecordedGnssStream();

        stream.Push(ReadResource("pygpsdata-NAV.bin"), 11);

        var pvt = Assert.Single(stream.Messages.OfType<UbxNavPvt>());
        Assert.Equal(560117000UL, pvt.iTOW);
        Assert.Equal((ushort)2021, pvt.Year);
        Assert.Equal((byte)12, pvt.Month);
        Assert.Equal((byte)4, pvt.Day);
        Assert.Equal((byte)59, pvt.Sec);
        Assert.Equal(26, pvt.NumberOfSatellites);
        Assert.Equal(-2.2402855, pvt.Longitude, 7);
        Assert.Equal(53.4507228, pvt.Latitude, 7);
        Assert.Equal(91.184, pvt.AltElipsoid, 3);
        Assert.Equal(42.701, pvt.AltMsl, 3);
        Assert.Equal(-0.007, pvt.VelocityNorth, 3);
        Assert.True(pvt.GnssFixOK);
        Assert.True(pvt.IsAppliedDifferentialCorrections);

        var satellites = Assert.Single(stream.Messages.OfType<UbxNavSat>());
        Assert.Equal(560117000UL, satellites.iTOW);
        Assert.Equal(43, satellites.NumSvs);
        Assert.Equal(43, satellites.Items.Length);
        Assert.Equal(532, satellites.GetByteSize());
        Assert.Equal(2, satellites.Items[0].SvId);
        Assert.Equal(31, satellites.Items[0].CnobBHz);
        Assert.Equal(18, satellites.Items[0].ElevDeg);
        Assert.Equal(221, satellites.Items[0].AzimDeg);
        Assert.Equal(1.7, satellites.Items[0].PrResM, 1);
        Assert.Equal(13, satellites.Items[5].SvId);
        Assert.Equal(-8.1, satellites.Items[5].PrResM, 1);
        Assert.Equal(19, satellites.Items[^1].SvId);

        var dop = Assert.Single(stream.Messages.OfType<UbxNavDop>());
        Assert.Equal(1.14, dop.gDOP, 2);
        Assert.Equal(1.01, dop.pDOP, 2);
        Assert.Equal(0.58, dop.hDOP, 2);
    }

    [Fact]
    public void Push_RecordedMonitor_ShouldDecodeDistinctVersionExtensionsAndFixedBuffers()
    {
        using var stream = new RecordedGnssStream();

        stream.Push(ReadResource("pygpsdata-MON.bin"), 13);

        var version = Assert.Single(stream.Messages.OfType<UbxMonVer>());
        Assert.Equal("EXT CORE 1.00 (f17067)", version.Software);
        Assert.Equal("00190000", version.Hardware);
        Assert.Equal(new[]
        {
            "ROM BASE 0x118B2060", "FWVER=HPG 1.50", "PROTVER=27.50",
            "MOD=ZED-F9P", "GPS;GLO;GAL;BDS", "SBAS;QZSS"
        }, version.Extensions);
        var tx = Assert.Single(stream.Messages.OfType<UbxMonTxbuf>());
        Assert.Equal(new byte[] { 0, 12, 0, 24, 0, 0 }, tx.PeakUsage);
        Assert.Equal(24, tx.TPeakUsage);
        var rx = Assert.Single(stream.Messages.OfType<UbxMonRxbuf>());
        Assert.Equal(new byte[] { 0, 0, 0, 4, 0, 0 }, rx.PeakUsage);
        var hardware = Assert.Single(stream.Messages.OfType<UbxMonHw>());
        Assert.Equal(92, hardware.Noise);
        Assert.Equal(1404, hardware.AgcCnt);
        Assert.Equal(new byte[] { 255, 0, 1, 3, 2, 16, 255, 18, 19, 20, 21, 14, 10, 11, 15, 255, 53 }, hardware.VP);
    }

    [Fact]
    public void Push_RecordedF9P_ShouldDecodeNavigationWordsWithoutLosingUnsignedBits()
    {
        using var stream = new RecordedGnssStream();

        stream.Push(ReadResource("ublox-zed-f9p-sfrbx.bin"), 19);

        Assert.Equal(55, stream.Messages.OfType<UbxRxmSfrbx>().Count());
        var words = stream.Messages.OfType<UbxRxmSfrbx>().First();
        Assert.Equal((UbxGnssTypeEnum)2, words.UbxGnssId);
        Assert.Equal(13, words.SvId);
        Assert.Equal(-7, words.FreqId);
        Assert.Equal(69, words.Chn);
        Assert.Equal(2, words.Version);
        Assert.Equal(9, words.NumWords);
        Assert.Equal(new uint[]
        {
            0x00955555, 0x55555555, 0x55555555, 0x50F54000, 0x889B0000,
            0x0000002A, 0xAAAA86F0, 0xC3FF4000, 0x2D012429
        }, words.RawData);
        var pvt = stream.Messages.OfType<UbxNavPvt>().First();
        Assert.Equal(-121.3140948, pvt.Longitude, 7);
        Assert.Equal(44.0688655, pvt.Latitude, 7);
        Assert.Equal(1108.561, pvt.AltElipsoid, 3);
        var satellites = stream.Messages.OfType<UbxNavSat>().First();
        Assert.Equal(35, satellites.Items.Length);
        Assert.Equal(-0.2, satellites.Items[0].PrResM, 1);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(31)]
    public void Push_CorruptRecordedUbxAndNmea_ShouldRejectThemAndRecoverFollowingProtocols(int chunkSize)
    {
        var pvt = ReadResource("pygpsdata-NAV.bin")[..100];
        var corruptPvt = (byte[])pvt.Clone();
        corruptPvt[^1] ^= 0x80;
        var rmc = ReadNmeaSentence("GPRMC");
        var corruptRmc = (byte[])rmc.Clone();
        corruptRmc[^3] = corruptRmc[^3] == (byte)'0' ? (byte)'1' : (byte)'0';
        var dop = ReadResource("pygpsdata-NAV.bin")[1906..1932];
        byte[] data = [.. corruptPvt, .. rmc, .. corruptRmc, .. dop, .. pvt];
        using var stream = new RecordedGnssStream();

        stream.Push(data, chunkSize);

        Assert.Equal(new[] { "N:GPRMC", "U:0104", "U:0107" }, stream.Messages.Select(MessageKey));
        Assert.Equal(2, stream.Errors.Count);
        Assert.Equal(UbxProtocol.Info, Assert.IsType<ProtocolDeserializeMessageException>(stream.Errors[0]).Parser);
        Assert.Equal(NmeaProtocol.Info, Assert.IsType<ProtocolDeserializeMessageException>(stream.Errors[1]).Parser);
        Assert.All(stream.Errors, error => Assert.Contains("Invalid crc", error.ToString()));
    }

    [Fact]
    public void Push_IncompleteRecordedFrame_ShouldWaitForChecksumAndRecoverAfterReset()
    {
        var pvt = ReadResource("pygpsdata-NAV.bin")[..100];
        using var stream = new RecordedGnssStream();

        stream.Push(pvt.AsSpan(0, pvt.Length - 1), 7);
        Assert.Empty(stream.Messages);
        Assert.Empty(stream.Errors);
        stream.Push(pvt.AsSpan(pvt.Length - 1), 1);
        Assert.IsType<UbxNavPvt>(Assert.Single(stream.Messages));

        stream.Messages.Clear();
        stream.Push(pvt.AsSpan(0, 45), 7);
        stream.Reset();
        stream.Push(ReadNmeaSentence("GPRMC"), 3);
        stream.Push(ReadResource("pygpsdata-NAV.bin").AsSpan(1906, 26), 3);

        Assert.Equal(new[] { "N:GPRMC", "U:0104" }, stream.Messages.Select(MessageKey));
        Assert.Empty(stream.Errors);
    }

    private static byte[] ReadResource(string name) =>
        File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Ubx", name));

    private static byte[] ReadNmeaSentence(string id)
    {
        var data = ReadResource("ubx_20080526.ubx");
        var start = data.AsSpan().IndexOf(Encoding.ASCII.GetBytes("$" + id + ","));
        Assert.True(start >= 0);
        var length = data.AsSpan(start).IndexOf("\r\n"u8);
        Assert.True(length > 0);
        return data.AsSpan(start, length + 2).ToArray();
    }

    private static string MessageKey(IProtocolMessage message) => message switch
    {
        UbxMessageBase ubx => $"U:{ubx.Id:X4}",
        NmeaMessageBase nmea => $"N:{nmea.TalkerId}{nmea.Id}",
        _ => throw new InvalidOperationException($"Unexpected protocol {message.Protocol.Id}")
    };

    private static string HashMessageOrder(IEnumerable<IProtocolMessage> messages) =>
        Hash(Encoding.UTF8.GetBytes(string.Join('\n', messages.Select(MessageKey))));

    private static string Hash(ReadOnlySpan<byte> data) => Convert.ToHexStringLower(SHA256.HashData(data));
}
