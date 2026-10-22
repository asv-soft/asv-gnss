using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Asv.IO;
using Xunit;

namespace Asv.Gnss;

public class UbxLongRecordingTest
{
    [Fact]
    public void Replay_CompleteF9pRoverRecording_ShouldDecodeMixedProtocolsInOrderWithBoundedCapture()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Ubx", "rtkexplorer-rover-20201224.ubx");
        using var source = File.OpenRead(path);
        Assert.Equal(14_071_360, source.Length);
        using var stream = new RecordedGnssStream();
        using var sourceHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        using var orderHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        using var legacyOrderHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        var buffer = new byte[8192];
        var tokenBuffer = new byte[32];
        var counts = new Dictionary<Type, int>();
        var constellations = new Dictionary<UbxGnssTypeEnum, int>();
        var unknownMessages = 0;
        var decodedMessages = 0;
        var legacyMessages = 0;
        NmeaMessageRmc? firstRmc = null;
        NmeaMessageRmc? lastRmc = null;
        NmeaMessageGga? firstGga = null;
        NmeaMessageGga? lastGga = null;
        NmeaMessageGst? firstGst = null;
        NmeaMessageGst? lastGst = null;
        UbxRxmSfrbx? firstSfrbx = null;
        UbxRxmSfrbx? lastSfrbx = null;
        UbxX20RxmRawxData1? firstRawx = null;
        UbxX20RxmRawxData1? lastRawx = null;

        int read;
        while ((read = source.Read(buffer)) > 0)
        {
            sourceHash.AppendData(buffer.AsSpan(0, read));
            stream.Push(buffer.AsSpan(0, read), buffer.Length);
            foreach (var message in stream.Messages)
            {
                var type = message.GetType();
                counts[type] = counts.GetValueOrDefault(type) + 1;
                string token;
                switch (message)
                {
                    case UbxRxmSfrbx sfrbx:
                        firstSfrbx ??= sfrbx;
                        lastSfrbx = sfrbx;
                        constellations[sfrbx.UbxGnssId] = constellations.GetValueOrDefault(sfrbx.UbxGnssId) + 1;
                        token = $"U:{sfrbx.Class:X2}{sfrbx.SubClass:X2}";
                        break;
                    case UbxX20RxmRawxData1 rawx:
                        firstRawx ??= rawx;
                        lastRawx = rawx;
                        token = $"U:{rawx.Class:X2}{rawx.SubClass:X2}";
                        break;
                    case NmeaMessageRmc rmc:
                        firstRmc ??= rmc;
                        lastRmc = rmc;
                        token = $"N:{rmc.TalkerId.Id}{rmc.Name}";
                        break;
                    case NmeaMessageGga gga:
                        firstGga ??= gga;
                        lastGga = gga;
                        token = $"N:{gga.TalkerId.Id}{gga.Name}";
                        break;
                    case NmeaMessageGst gst:
                        firstGst ??= gst;
                        lastGst = gst;
                        token = $"N:{gst.TalkerId.Id}{gst.Name}";
                        break;
                    case NmeaMessageGll gll:
                        token = $"N:{gll.TalkerId.Id}{gll.Name}";
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected decoded message type: {type.FullName}");
                }
                if (decodedMessages > 0)
                {
                    tokenBuffer[0] = (byte)'\n';
                    orderHash.AppendData(tokenBuffer.AsSpan(0, 1));
                }
                var tokenLength = Encoding.ASCII.GetBytes(token, tokenBuffer);
                orderHash.AppendData(tokenBuffer.AsSpan(0, tokenLength));
                decodedMessages++;
                if (message is not UbxX20MessageBase)
                {
                    if (legacyMessages > 0)
                    {
                        legacyOrderHash.AppendData("\n"u8);
                    }
                    legacyOrderHash.AppendData(tokenBuffer.AsSpan(0, tokenLength));
                    legacyMessages++;
                }
            }
            foreach (var error in stream.Errors)
            {
                var unknown = Assert.IsType<ProtocolParserUnknownMessageException>(error);
                Assert.Equal(UbxProtocol.Info, unknown.Parser);
                unknownMessages++;
            }
            // Retain only aggregate counters and the first/last semantic samples across chunks.
            stream.Messages.Clear();
            stream.Errors.Clear();
        }

        Assert.Equal("BA2E782FE06B0B2F6C4BF0E54D5819AA921DCD8699DCA1264BC686CA9295156A", Convert.ToHexString(sourceHash.GetHashAndReset()));
        Assert.Equal("C800FE1DA23B89E78F7BA5E4D3D2D55669515F988E580E2231B668A55EE42567", Convert.ToHexString(orderHash.GetHashAndReset()));
        Assert.Equal("19889AB7C4703B8BF71D18D957671F6C01B8247FC04833202CEF2E78EBF4ED86", Convert.ToHexString(legacyOrderHash.GetHashAndReset()));
        Assert.Equal(115_567, decodedMessages);
        Assert.Equal(111_046, legacyMessages);
        Assert.Equal(0, unknownMessages);
        Assert.Equal(6, counts.Count);
        Assert.Equal(92_962, counts[typeof(UbxRxmSfrbx)]);
        Assert.Equal(4_521, counts[typeof(NmeaMessageRmc)]);
        Assert.Equal(4_521, counts[typeof(NmeaMessageGga)]);
        Assert.Equal(4_521, counts[typeof(NmeaMessageGll)]);
        Assert.Equal(4_521, counts[typeof(NmeaMessageGst)]);
        Assert.Equal(4_521, counts[typeof(UbxX20RxmRawxData1)]);
        Assert.Equal(5, constellations.Count);
        Assert.Equal(8_762, constellations[UbxGnssTypeEnum.GPS]);
        Assert.Equal(13_369, constellations[UbxGnssTypeEnum.SBAS]);
        Assert.Equal(32_624, constellations[UbxGnssTypeEnum.Galileo]);
        Assert.Equal(5_810, constellations[UbxGnssTypeEnum.BeiDou]);
        Assert.Equal(32_397, constellations[UbxGnssTypeEnum.GLONASS]);

        Assert.NotNull(firstRmc);
        Assert.NotNull(lastRmc);
        Assert.Equal(new TimeOnly(21, 28, 24), firstRmc.Time);
        Assert.Equal(new TimeOnly(22, 43, 44), lastRmc.Time);
        Assert.Equal(new DateTime(2020, 12, 24), firstRmc.Date);
        Assert.Equal(firstRmc.Date, lastRmc.Date);
        Assert.Equal(40.097029666666664, firstRmc.Latitude, 10);
        Assert.Equal(-105.14726033333334, firstRmc.Longitude, 10);
        Assert.Equal(40.09666033333333, lastRmc.Latitude, 10);
        Assert.Equal(-105.1473205, lastRmc.Longitude, 10);
        Assert.Equal(0.005, firstRmc.SpeedOverGroundKnots);
        Assert.Equal(0.014, lastRmc.SpeedOverGroundKnots);
        Assert.Equal(NmeaPositionFixStatus.Valid, firstRmc.Status);
        Assert.Equal(NmeaPositioningSystemMode.Differential, firstRmc.PositioningSystemMode);
        Assert.Equal(NmeaNavigationStatus.NotValid, firstRmc.NavigationStatus);
        Assert.True(double.IsNaN(firstRmc.TrackMadeGoodDegreesTrue));

        Assert.NotNull(firstGga);
        Assert.NotNull(lastGga);
        Assert.Equal(firstRmc.Time, firstGga.Time);
        Assert.Equal(lastRmc.Time, lastGga.Time);
        Assert.Equal(NmeaGpsQuality.Dgnss, firstGga.GpsQuality);
        Assert.Equal(12, firstGga.NumberOfSatellites);
        Assert.Equal(0.7, firstGga.HorizontalDilutionPrecision);
        Assert.Equal(0.49, lastGga.HorizontalDilutionPrecision);
        Assert.Equal(1599.7, firstGga.AntennaAltitudeMsl);
        Assert.Equal(1599.8, lastGga.AntennaAltitudeMsl);
        Assert.Equal(-21.4, firstGga.GeoidalSeparation);
        Assert.Equal(-21.4, lastGga.GeoidalSeparation);

        Assert.NotNull(firstGst);
        Assert.NotNull(lastGst);
        Assert.Equal(firstRmc.Time, firstGst.Time);
        Assert.Equal(lastRmc.Time, lastGst.Time);
        Assert.Equal(18, firstGst.Rms);
        Assert.Equal(17, lastGst.Rms);
        Assert.Equal(0.72, firstGst.MajorAxis);
        Assert.Equal(0.63, lastGst.MajorAxis);
        Assert.Equal(0.57, firstGst.AltStdDev);
        Assert.Equal(0.61, lastGst.AltStdDev);

        Assert.NotNull(firstSfrbx);
        Assert.NotNull(lastSfrbx);
        Assert.Equal(UbxGnssTypeEnum.SBAS, firstSfrbx.UbxGnssId);
        Assert.Equal(138, firstSfrbx.SvId);
        Assert.Equal(9, firstSfrbx.NumWords);
        Assert.Equal(23, firstSfrbx.Chn);
        Assert.Equal(2, firstSfrbx.Version);
        Assert.Equal(255, firstSfrbx.Reserved2);
        Assert.Equal(0x9A663028u, firstSfrbx.RawData[0]);
        Assert.Equal(0xBF000000u, firstSfrbx.RawData[^1]);
        Assert.Equal(UbxGnssTypeEnum.BeiDou, lastSfrbx.UbxGnssId);
        Assert.Equal(12, lastSfrbx.SvId);
        Assert.Equal(10, lastSfrbx.NumWords);
        Assert.Equal(80, lastSfrbx.Chn);
        Assert.Equal(0x38903684u, lastSfrbx.RawData[0]);
        Assert.Equal(0x37B54E80u, lastSfrbx.RawData[^1]);

        Assert.NotNull(firstRawx);
        Assert.NotNull(lastRawx);
        Assert.Equal(422922.005, firstRawx.RcvTow);
        Assert.Equal(427442.006, lastRawx.RcvTow);
        Assert.Equal((ushort)2137, firstRawx.Week);
        Assert.Equal(firstRawx.Week, lastRawx.Week);
        Assert.Equal((sbyte)18, firstRawx.LeapS);
        Assert.Equal((byte)62, firstRawx.NumMeas);
        Assert.Equal((byte)60, lastRawx.NumMeas);
        Assert.Equal((byte)1, firstRawx.Version);
        Assert.Equal((byte)1, firstRawx.RecStat);
        Assert.Equal(new byte[] { 0xB7, 0x11 }, firstRawx.Reserved0);
        Assert.Equal(21756763.55634079, firstRawx.Meas[0].PrMes);
        Assert.Equal(114332579.33629781, firstRawx.Meas[0].CpMes);
        Assert.Equal(-1109.247802734375f, firstRawx.Meas[0].DoMes);
        Assert.Equal((byte)4, firstRawx.Meas[0].SvId);
        Assert.Equal((ushort)64500, firstRawx.Meas[0].Locktime);
        Assert.Equal((byte)46, firstRawx.Meas[0].Cno);
        Assert.Equal(23257012.950790454, lastRawx.Meas[^1].PrMes);
        Assert.Equal((byte)6, lastRawx.Meas[^1].GnssId);
        Assert.Equal((byte)8, lastRawx.Meas[^1].SvId);
        Assert.Equal((byte)2, lastRawx.Meas[^1].SigId);
        Assert.Equal("74729565CB391DB7B6719756D47D5F6DC2A7F3231A6E1C66FD4CB6AA7ED9E040",
            Convert.ToHexString(SHA256.HashData(UbxTestHelper.Serialize(firstRawx).AsSpan(6, firstRawx.GetByteSize() - 8))));
        Assert.Equal("369CC3840184BBD4999CAC8354C0AE3AFD7A9FE9F5535AB14161F7C2A17E6002",
            Convert.ToHexString(SHA256.HashData(UbxTestHelper.Serialize(lastRawx).AsSpan(6, lastRawx.GetByteSize() - 8))));
    }
}
