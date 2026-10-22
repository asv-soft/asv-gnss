using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class RawNavigationRecordedStreamTest
{
    private const string GpsName = "gfz-gps-l1ca-2025-001-prn01-000000-001224";
    private const string GlonassName = "gpsd-glonass-sv12-superframe2158";

    [Theory]
    [InlineData(false, 1)]
    [InlineData(false, 7)]
    [InlineData(false, 257)]
    [InlineData(false, 4096)]
    [InlineData(true, 1)]
    [InlineData(true, 7)]
    [InlineData(true, 257)]
    [InlineData(true, 4096)]
    public void Replay_RecordedNativeBits_PreservesEveryFrameAcrossReadSizes(
        bool glonass,
        int readSize
    )
    {
        var data = ReadNativeStream(glonass);
        var messages = Replay(data, glonass, readSize);
        var count = glonass ? 75 : 125;
        var bitSize = glonass ? 85 : 300;
        var prefix = glonass ? 3 : 4;
        Assert.Equal(count, messages.Count);
        for (var index = 0; index < count; index++)
        {
            var message = messages[index];
            Assert.Equal(glonass ? RawGlonassProtocol.Info : RawGpsProtocol.Info, message.Protocol);
            if (glonass)
                Assert.Equal(
                    index % 15 + 1,
                    Assert.IsAssignableFrom<GlonassWordBase>(message).WordId
                );
            else
            {
                var gps = Assert.IsAssignableFrom<GpsSubframeBase>(message);
                Assert.Equal(index % 5 + 1, gps.SubframeId);
                Assert.Equal((uint)(43201 + index), gps.TOW1_5Epoh);
            }
            // Compare the native bits from the recording, without regenerating an expected checksum.
            var expected = SliceBits(data, index * bitSize, bitSize, prefix);
            Assert.Equal(expected, Serialize(message));
        }
    }

    [Fact]
    public void GpsRecording_OriginalRowsRetainAllParityAndD30Inversions()
    {
        // The original downloaded Arrow file is retained; the maintenance script extracts its rows.
        ReadVerified(
            "Sources/NAVBIT-GPS-L1CA-2025-001-01-14400.arrow",
            153250,
            "1f1a0798833f655c3d350b1cd3662640978a1ef4e767efefbf41f312e6131c01"
        );
        var rows = ReadVerified(
            GpsName + ".frames.bin",
            4750,
            "bb376e8c5e58443d1dd8e29f695487ee47e8dfc6e80798e92f79d11a08f2e8fa"
        );
        var stream = ReadNativeStream(false);
        var invertedWords = 0;
        for (var frame = 0; frame < 125; frame++)
        {
            for (var bit = 0; bit < 300; bit++)
                Assert.Equal(
                    ReadBits(rows, frame * 304 + bit, 1),
                    ReadBits(stream, frame * 300 + bit, 1)
                );
            Assert.Equal(0u, ReadBits(rows, frame * 304 + 300, 4));
            uint previous = 0;
            for (var wordIndex = 0; wordIndex < 10; wordIndex++)
            {
                var word = ReadBits(rows, frame * 304 + wordIndex * 30, 30);
                var decoded = word >> 6;
                if ((previous & 1) != 0)
                {
                    decoded ^= 0xFFFFFF;
                    invertedWords++;
                }
                // IS-GPS-200N Table 20-XIV, independent of the production parity masks.
                for (var equation = 0; equation < 6; equation++)
                {
                    var sum = (previous >> (equation is 0 or 2 or 5 ? 1 : 0)) & 1;
                    foreach (var position in GpsParityDataBits[equation])
                        sum ^= (decoded >> (24 - position)) & 1;
                    Assert.Equal(sum, (word >> (5 - equation)) & 1);
                }
                if (wordIndex == 0)
                    Assert.Equal(0x8Bu, decoded >> 16);
                previous = word & 3;
                if (wordIndex is 1 or 9)
                    Assert.Equal(0u, previous);
            }
        }
        Assert.Equal(397, invertedWords);
        Assert.Equal(0u, ReadBits(stream, 37500, 4));
    }

    [Fact]
    public void GpsRecording_CompletePageCycle_DecodesRecordedEphemerisSpecialMessageAndUtc()
    {
        var messages = Replay(ReadNativeStream(false), false, 17);
        Assert.Equal(125, messages.Count);
        var first = Assert.IsType<GpsSubframe1>(messages[0]);
        Assert.Equal(299, first.WeekNumber); // Ten transmitted bits of GPS week 2347.
        Assert.Equal(63, first.SatelliteHealth);
        Assert.Equal(40, first.iodc);
        Assert.Equal(266400, first.TocSec);
        var second = Assert.IsType<GpsSubframe2>(messages[1]);
        Assert.Equal(40, second.iode);
        Assert.Equal(266400, second.ToeSec);
        Assert.Equal(31, second.Aodo);
        Assert.Equal(
            new byte[]
            {
                57,
                55,
                56,
                58,
                59,
                57,
                60,
                61,
                62,
                63,
                57,
                25,
                26,
                27,
                28,
                57,
                29,
                30,
                31,
                32,
                57,
                62,
                52,
                53,
                54,
            },
            messages.OfType<GpsSubframe4>().Select(message => message.PageId)
        );
        Assert.Equal(
            new byte[]
            {
                16,
                17,
                18,
                19,
                20,
                21,
                0,
                23,
                24,
                51,
                1,
                2,
                3,
                4,
                5,
                6,
                7,
                8,
                9,
                10,
                11,
                12,
                13,
                14,
                15,
            },
            messages.OfType<GpsSubFrame5>().Select(message => message.PageId)
        );
        var special = Assert.IsType<GpsSubframe4>(messages[8]);
        Assert.Equal(GpsSubframe4.Subframe4Type.SpecialMessage, special.Type);
        Assert.Equal(
            Convert.FromHexString("5445585452414E2E494A273932462F55472B50384E4F"),
            special.SpecialMessageData
        );
        var utc = Assert.IsType<GpsSubframe4>(messages[13]).UtcGps;
        Assert.Equal(137, utc.WN_LSF);
        Assert.Equal(7, utc.DN);
        Assert.Equal(18, utc.DeltaT_LSF);
        var nmct = Assert.IsType<GpsSubframe4>(messages[113]);
        Assert.Equal(GpsSubframe4.Subframe4Type.Nmct, nmct.Type);
        Assert.Equal(2, nmct.NmctAvailabilityIndicator);
        Assert.Equal(Enumerable.Repeat(-32, 30), nmct.NmctEstimatedRangeDeviations);
        var dummy = messages.OfType<GpsSubFrame5>().Single(message => message.PageId == 0);
        Assert.Equal(GpsSubFrame5.Subframe5Type.None, dummy.Type);
        Assert.Equal(23, dummy.RawPageData!.Length);
    }

    [Fact]
    public void GlonassRecording_EveryNativeBitMatchesOriginalReceiverPacketAndFrameContext()
    {
        var source = ReadVerified(
            "Sources/gpsd-ublox-glo.bin",
            168595,
            "767aa0fb33233c56d592bcb7ddce25d7ac88cbb768cb176296face092703f5b2"
        );
        var stream = ReadNativeStream(true);
        var selected = 0;
        var totalGlonass = 0;
        var firstOffset = -1;
        var lastOffset = -1;
        for (var offset = 0; offset < source.Length; )
        {
            if (offset + 2 > source.Length || source[offset] != 0xB5 || source[offset + 1] != 0x62)
            {
                offset++;
                continue;
            }
            var length = BinaryPrimitives.ReadUInt16LittleEndian(source.AsSpan(offset + 4));
            Assert.True(offset + length + 8 <= source.Length);
            var packet = source.AsSpan(offset, length + 8);
            byte checksumA = 0,
                checksumB = 0;
            foreach (var value in packet[2..^2])
            {
                checksumA = unchecked((byte)(checksumA + value));
                checksumB = unchecked((byte)(checksumB + checksumA));
            }
            Assert.Equal(checksumA, packet[^2]);
            Assert.Equal(checksumB, packet[^1]);
            if (packet[2] == 0x02 && packet[3] == 0x13 && packet[6] == 6)
            {
                totalGlonass++;
                var payload = packet[6..^2];
                Assert.Equal(4, payload[4]);
                Assert.Equal(2, payload[6]);
                var firstWord = BinaryPrimitives.ReadUInt32LittleEndian(payload[8..]);
                var secondWord = BinaryPrimitives.ReadUInt32LittleEndian(payload[12..]);
                var thirdWord = BinaryPrimitives.ReadUInt32LittleEndian(payload[16..]);
                var context = BinaryPrimitives.ReadUInt32LittleEndian(payload[20..]);
                var native =
                    ((UInt128)firstWord << 53) | ((UInt128)secondWord << 21) | (thirdWord >> 11);
                AssertHamming(native);
                if (payload[1] == 12 && context >> 16 == 2158)
                {
                    if (selected == 0)
                        firstOffset = offset;
                    lastOffset = offset;
                    Assert.Equal(6, payload[3]); // Receiver channel offset 7 + actual channel -1.
                    Assert.Equal((uint)(selected / 15 + 1), context & 255);
                    Assert.Equal((uint)(selected % 15 + 1), (uint)(native >> 80));
                    for (var bit = 0; bit < 85; bit++)
                        Assert.Equal(
                            (uint)((native >> (84 - bit)) & 1),
                            ReadBits(stream, selected * 85 + bit, 1)
                        );
                    selected++;
                }
            }
            offset += length + 8;
        }
        Assert.Equal(584, totalGlonass);
        Assert.Equal(75, selected);
        Assert.Equal(12766, firstOffset);
        Assert.Equal(95590, lastOffset);
        Assert.Equal(0u, ReadBits(stream, 6375, 1));
    }

    [Fact]
    public void GlonassRecording_UsesCapturedFifthFrameContextAndRetainsReservedData()
    {
        var stream = ReadNativeStream(true);
        var messages = Replay(stream, true, 17);
        Assert.Equal(75, messages.Count);
        var first = Assert.IsType<GlonassWord1>(messages[0]);
        Assert.Equal(64350, first.TofLocalSec);
        Assert.Equal(0, first.P1);
        Assert.Equal(162.63294219970703, first.VelocityX);
        Assert.Equal(-8971239.2578125, first.PositionX);
        var fourth = Assert.IsType<GlonassWord4>(messages[3]);
        Assert.Equal(1275, fourth.Nt);
        Assert.Equal(12, fourth.Slot);
        Assert.Equal(1, fourth.M);
        var fifth = Assert.IsType<GlonassWord5>(messages[4]);
        Assert.Equal(1275, fifth.Na);
        Assert.Equal(7, fifth.N4);
        var special14 = Assert.IsType<GlonassWord14>(messages[73]);
        var special15 = Assert.IsType<GlonassWord15>(messages[74]);
        Assert.Equal(0, special14.FrameNumber);
        Assert.Equal(0, special15.FrameNumber);
        // Captured UBX context identifies the fifth frame; it is not encoded in an 85-bit string.
        special14.FrameNumber = 5;
        special15.FrameNumber = 5;
        Assert.Equal(-38.0 / 1024, special14.B1);
        Assert.Equal(18.0 / 65536, special14.B2);
        Assert.Equal(0, special14.KP);
        Assert.Equal(0, special15.ln);
        Assert.Equal(Convert.FromHexString("639B3320D53908DA3B"), special15.ReservedData);
        Assert.Equal(SliceBits(stream, 73 * 85, 85, 3), Serialize(special14));
        Assert.Equal(SliceBits(stream, 74 * 85, 85, 3), Serialize(special15));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Replay_RecordingTruncatedInsideLastMessage_PublishesOnlyCompleteMessages(
        bool glonass
    )
    {
        var messages = Replay(ReadNativeStream(glonass)[..^1], glonass, 7);
        Assert.Equal(glonass ? 74 : 124, messages.Count);
        if (glonass)
            Assert.IsType<GlonassWord14>(messages[^1]);
        else
            Assert.IsType<GpsSubframe4>(messages[^1]);
    }

    private static List<IProtocolMessage> Replay(byte[] data, bool glonass, int readSize)
    {
        var protocol = Protocol.Create(builder =>
        {
            if (glonass)
                builder.Protocols.RegisterRawGlonassProtocol();
            else
                builder.Protocols.RegisterRawGpsProtocol();
        });
        using var parser = protocol.CreateParser(
            glonass ? RawGlonassProtocol.Info.Id : RawGpsProtocol.Info.Id
        );
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        using var source = new MemoryStream(data, writable: false);
        var buffer = new byte[readSize];
        int count;
        while ((count = source.Read(buffer)) != 0)
            for (var index = 0; index < count; index++)
                if (parser.Push(buffer[index]))
                    parser.Reset(); // Match the ordinary Asv.IO dispatcher's completion contract.
        Assert.Empty(errors);
        return messages;
    }

    private static byte[] ReadNativeStream(bool glonass) =>
        glonass
            ? ReadVerified(
                GlonassName + ".bin",
                797,
                "21d2822aa684c57c96b412691cf2286ec354d3da1acfb66b63decdf40adbcde3"
            )
            : ReadVerified(
                GpsName + ".bin",
                4688,
                "8fb240904f8bd56c1bfc7606c80ed82101c26c389eb8df74c7cd8c079562c958"
            );

    private static byte[] ReadVerified(string file, int size, string sha256)
    {
        var data = File.ReadAllBytes(
            Path.Combine(AppContext.BaseDirectory, "Resources", "RawNavigation", file)
        );
        Assert.Equal(size, data.Length);
        Assert.Equal(sha256, Convert.ToHexStringLower(SHA256.HashData(data)));
        return data;
    }

    private static byte[] Serialize(IProtocolMessage message)
    {
        var data = new byte[message.GetByteSize()];
        Span<byte> output = data;
        message.Serialize(ref output);
        Assert.True(output.IsEmpty);
        return data;
    }

    private static byte[] SliceBits(byte[] data, int start, int length, int leadingPadding)
    {
        var result = new byte[(length + leadingPadding + 7) / 8];
        for (var index = 0; index < length; index++)
        {
            var destination = leadingPadding + index;
            result[destination / 8] |= (byte)(
                ReadBits(data, start + index, 1) << (7 - destination % 8)
            );
        }
        return result;
    }

    private static uint ReadBits(byte[] data, int start, int length)
    {
        uint result = 0;
        for (var bit = start; bit < start + length; bit++)
            result = (result << 1) | (uint)((data[bit / 8] >> (7 - bit % 8)) & 1);
        return result;
    }

    private static readonly int[][] GpsParityDataBits =
    [
        [1, 2, 3, 5, 6, 10, 11, 12, 13, 14, 17, 18, 20, 23],
        [2, 3, 4, 6, 7, 11, 12, 13, 14, 15, 18, 19, 21, 24],
        [1, 3, 4, 5, 7, 8, 12, 13, 14, 15, 16, 19, 20, 22],
        [2, 4, 5, 6, 8, 9, 13, 14, 15, 16, 17, 20, 21, 23],
        [1, 3, 5, 6, 7, 9, 10, 14, 15, 16, 17, 18, 21, 22, 24],
        [3, 5, 6, 8, 9, 10, 11, 13, 15, 19, 22, 23, 24],
    ];

    // One-based ICD bit positions; b85 is first on the wire, b1 is the final check bit.
    private static readonly int[][] GlonassHammingDataBits =
    [
        [
            9,
            10,
            12,
            13,
            15,
            17,
            19,
            20,
            22,
            24,
            26,
            28,
            30,
            32,
            34,
            35,
            37,
            39,
            41,
            43,
            45,
            47,
            49,
            51,
            53,
            55,
            57,
            59,
            61,
            63,
            65,
            66,
            68,
            70,
            72,
            74,
            76,
            78,
            80,
            82,
            84,
        ],
        [
            9,
            11,
            12,
            14,
            15,
            18,
            19,
            21,
            22,
            25,
            26,
            29,
            30,
            33,
            34,
            36,
            37,
            40,
            41,
            44,
            45,
            48,
            49,
            52,
            53,
            56,
            57,
            60,
            61,
            64,
            65,
            67,
            68,
            71,
            72,
            75,
            76,
            79,
            80,
            83,
            84,
        ],
        [10, 12, 16, 19, 23, 26, 31, 34, 38, 41, 46, 49, 54, 57, 62, 65, 69, 72, 77, 80, 85, 85],
        [13, 19, 27, 34, 42, 49, 58, 65, 73, 80],
        [20, 34, 50, 65, 81, 85],
        [35, 65],
        [66, 85],
    ];

    private static void AssertHamming(UInt128 value)
    {
        Assert.Equal((UInt128)0, value >> 84);
        for (var check = 0; check < 7; check++)
        {
            var parity = (int)((value >> check) & 1);
            var indices = GlonassHammingDataBits[check];
            if (check < 2)
                foreach (var bit in indices)
                    parity ^= (int)((value >> (bit - 1)) & 1);
            else
                for (var range = 0; range < indices.Length; range += 2)
                for (var bit = indices[range]; bit <= indices[range + 1]; bit++)
                    parity ^= (int)((value >> (bit - 1)) & 1);
            Assert.Equal(0, parity);
        }
        var overall = 0;
        for (var bit = 0; bit < 85; bit++)
            overall ^= (int)((value >> bit) & 1);
        Assert.Equal(0, overall);
    }
}
