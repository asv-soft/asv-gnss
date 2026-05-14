using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Asv.Gnss.Test;

public class RtcmV3LegacyMessageTest
{
    private static readonly IReadOnlyDictionary<ushort, Func<RtcmV3MessageBase>> MessageFactory =
        new Dictionary<ushort, Func<RtcmV3MessageBase>>
        {
            [1004] = () => new RtcmV3Message1004(),
            [1005] = () => new RtcmV3Message1005(),
            [1006] = () => new RtcmV3Message1006(),
            [1007] = () => new RtcmV3Message1007(),
            [1008] = () => new RtcmV3Message1008(),
            [1012] = () => new RtcmV3Message1012(),
            [1013] = () => new RtcmV3Message1013(),
            [1019] = () => new RtcmV3Message1019(),
            [1020] = () => new RtcmV3Message1020(),
            [1029] = () => new RtcmV3Message1029(),
            [1030] = () => new RtcmV3Message1030(),
            [1031] = () => new RtcmV3Message1031(),
            [1032] = () => new RtcmV3Message1032(),
            [1033] = () => new RtcmV3Message1033(),
            [1042] = () => new RtcmV3Message1042(),
            [1046] = () => new RtcmV3Message1046(),
            [1073] = () => new RtcmV3Msm3Msg1073(),
            [1074] = () => new RtcmV3Msm4Msg1074(),
            [1075] = () => new RtcmV3Msm5Msg1075(),
            [1077] = () => new RtcmV3Msm7Msg1077(),
            [1083] = () => new RtcmV3Msm3Msg1083(),
            [1084] = () => new RtcmV3Msm4Msg1084(),
            [1085] = () => new RtcmV3Msm5Msg1085(),
            [1087] = () => new RtcmV3Msm7Msg1087(),
            [1093] = () => new RtcmV3Msm3Msg1093(),
            [1094] = () => new RtcmV3Msm4Msg1094(),
            [1095] = () => new RtcmV3Msm5Msg1095(),
            [1097] = () => new RtcmV3Msm7Msg1097(),
            [1104] = () => new RtcmV3Msm4Msg1104(),
            [1107] = () => new RtcmV3Msm7Msg1107(),
            [1114] = () => new RtcmV3Msm4Msg1114(),
            [1117] = () => new RtcmV3Msm7Msg1117(),
            [1123] = () => new RtcmV3Msm3Msg1123(),
            [1124] = () => new RtcmV3Msm4Msg1124(),
            [1125] = () => new RtcmV3Msm5Msg1125(),
            [1127] = () => new RtcmV3Msm7Msg1127(),
            [1230] = () => new RtcmV3Message1230(),
            [4094] = () => new RtcmV3Msg4094(),
        };

    [Fact]
    public void MessageFactory_ShouldContainV163Rtcm3Messages()
    {
        var expected = new ushort[]
        {
            1004, 1005, 1006, 1007, 1008, 1012, 1013, 1019, 1020, 1029,
            1030, 1031, 1032, 1033, 1042, 1046, 1073, 1074, 1075, 1077,
            1083, 1084, 1085, 1087, 1093, 1094, 1095, 1097, 1104, 1107,
            1114, 1117, 1123, 1124, 1125, 1127, 1230, 4094
        };

        Assert.Empty(expected.Except(MessageFactory.Keys));
    }

    [Theory]
    [MemberData(nameof(LegacyResourceData))]
    public void LegacyV163Resources_ShouldDeserializeKnownRtcm3Messages(string resourceName, IReadOnlyDictionary<ushort, int> expectedCounts)
    {
        var actualCounts = DeserializeResource(resourceName);

        Assert.Equal(expectedCounts.Sum(x => x.Value), actualCounts.Sum(x => x.Value));
        Assert.Empty(expectedCounts.Except(actualCounts));
        Assert.Empty(actualCounts.Except(expectedCounts));
    }

    [Theory]
    [InlineData(1073, Msm1073Frame)]
    [InlineData(1083, Msm1083Frame)]
    [InlineData(1093, Msm1093Frame)]
    [InlineData(1123, Msm1123Frame)]
    public void LegacyV163Msm3Frames_ShouldDeserialize(ushort expectedId, string hexFrame)
    {
        var message = DeserializeFrame(Hex(hexFrame));

        Assert.Equal(expectedId, message.Id);
    }

    [Fact]
    public void LegacyV163Msm1073Frame_ShouldPreserveDecodedProperties()
    {
        var message = Assert.IsType<RtcmV3Msm3Msg1073>(DeserializeFrame(Hex(Msm1073Frame)));

        Assert.Equal((uint)144187000, message.EpochTimeTow);
        Assert.Equal(1, message.MultipleMessageBit);
        Assert.Equal(8, message.Satellites.Length);
        Assert.Equal(27, message.Satellites.Sum(s => s.Signals.Length));
        Assert.Equal(5, message.Satellites[0].SatellitePrn);
        Assert.Equal("L1C", message.Satellites[0].Signals[0].RinexCode);
    }

    public static IEnumerable<object[]> LegacyResourceData()
    {
        yield return
        [
            "testglo.rtcm3",
            new Dictionary<ushort, int>
            {
                [1004] = 186,
                [1005] = 19,
                [1012] = 186,
                [1019] = 19,
                [1020] = 19,
            }
        ];
        yield return
        [
            "fw206mrtk_rtcm",
            new Dictionary<ushort, int>
            {
                [1006] = 69,
                [1008] = 69,
                [1075] = 1400,
                [1085] = 700,
                [1095] = 700,
                [1125] = 2058,
            }
        ];
        yield return
        [
            "imu_rtcm",
            new Dictionary<ushort, int>
            {
                [1006] = 13,
                [1008] = 13,
                [1019] = 9,
                [1020] = 19,
                [1033] = 13,
                [1042] = 18,
                [1046] = 6,
                [1074] = 134,
                [1084] = 134,
                [1094] = 134,
                [1124] = 268,
            }
        ];
        yield return
        [
            "GMSD7_20121014.rtcm3",
            new Dictionary<ushort, int>
            {
                [1007] = 28,
                [1008] = 28,
                [1019] = 15,
                [1020] = 16,
                [1033] = 28,
                [1077] = 257,
                [1087] = 257,
                [1117] = 257,
                [1127] = 257,
            }
        ];
    }

    private static Dictionary<ushort, int> DeserializeResource(string resourceName)
    {
        var data = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", resourceName));
        var counts = new Dictionary<ushort, int>();

        foreach (var frame in EnumerateRtcm3Frames(data))
        {
            var message = DeserializeFrame(frame);
            if (counts.ContainsKey(message.Id))
            {
                counts[message.Id]++;
            }
            else
            {
                counts.Add(message.Id, 1);
            }
        }

        return counts;
    }

    private static RtcmV3MessageBase DeserializeFrame(byte[] frame)
    {
        var id = GetMessageId(frame);
        Assert.True(MessageFactory.TryGetValue(id, out var factory), $"RTCM3 message {id} is not registered in test factory.");

        var message = factory();
        ReadOnlySpan<byte> span = frame;
        message.Deserialize(ref span);
        Assert.Equal(0, span.Length);
        return message;
    }

    private static IEnumerable<byte[]> EnumerateRtcm3Frames(byte[] data)
    {
        for (var i = 0; i < data.Length - 5;)
        {
            if (data[i] != RtcmV3Protocol.SyncByte)
            {
                i++;
                continue;
            }

            var payloadLength = ((data[i + 1] & 0x03) << 8) | data[i + 2];
            var frameLength = 3 + payloadLength + 3;
            if (payloadLength > 1023 || i + frameLength > data.Length)
            {
                i++;
                continue;
            }

            var frame = data[i..(i + frameLength)];
            var calculatedCrc = RtcmV3Crc24.Calc(frame, payloadLength + 3, 0);
            var sourceCrc = ((uint)frame[^3] << 16) | ((uint)frame[^2] << 8) | frame[^1];
            if (calculatedCrc != sourceCrc)
            {
                i++;
                continue;
            }

            yield return frame;
            i += frameLength;
        }
    }

    private static ushort GetMessageId(byte[] frame)
    {
        return (ushort)((frame[3] << 4) | ((frame[4] & 0xF0) >> 4));
    }

    private static byte[] Hex(string value)
    {
        var bytes = new byte[value.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
        }

        return bytes;
    }

    private const string Msm1073Frame =
        "D300B3431001226079E20020041D28400000000028204080F9FF90481F1F464878CB4C35D8962C101367270A4C4C990822104120850100834F045BB5776BBED59DABF8FCBCC27A88D367A457FA3FF17FD9A136425F05440A9C178C26FBF09B9A826E5D09B29803ABB00F040039D500E75403A3C00C057DA2C6F6906BDA24BF68243C2AB5FB199BF229DF43187D4AA9FF8813FE4AFFF8D98098140255B8094230250980937BEEEEEEEEEE6666DBC4DEEE6666600000003951BD";

    private const string Msm1083Frame =
        "D300B143B00130591EA20020418A03800000000030C000007FFE7FE7630506058FE39F7F70B86B7CD7F1BCBF7A0097F12942D3059AF384E755D693AC770E0E0EDAB6F5726CF7D9678FD71FC65BACB7BD877B239C4D36F2745BD61A2F5DF9BD7723F5CF2C129B904B15012E4F04B27FE72CEF9BB83E6AF4F9BFFBC741CF6B9B7D2A38F521A7D5CA8F5652BC7CC3F1E89BC7E46F1EBABEC61AFAF904409050DC50836E93BBB4EEEEEEEEDDCCA8EEEDEE98600000000B679C";

    private const string Msm1093Frame =
        "D3006C445001226079E20020000800A140000000040080807FFFD76ADAF55998B19262B1C7004D14999941A106E209447C9D443ABC792BCC6F914F405E2B0178B4C3E2D6504B7321314304C562084E281FB9007EF685D9F01CDB0073665F94177E551FF95817777777777777770000BA1F50";

    private const string Msm1123Frame =
        "D300B3463001225F9F200020000008168846000020000090F7AFFFF84ECD65E940A4A5280B5FF1E263D0BE51760C39987B70906119EA27D2645588B2114C82843BA87834EEBBDAEAFE3E220C2577BBF207E6D5CC378E7843B86BD0C7A58343360CAF07C1FF4F0CEDBC1E2CEDE553E977DFBF5341658B05781C14F520364A3ED15CFC28E7EE49BFB946FBF133F0AB57C21C2EFADEBC8AD1F3551FCC15CF1BEAC12A5D03AB3157777B72FB97BBBBBBBBBBBBAA40000000040DC3";
}
