using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class JavadRecordedStreamTest
{
    [Theory]
    [InlineData("javad_20110115.jps", 1)]
    [InlineData("javad_20110115.jps", 257)]
    [InlineData("gpsd-greis-def.bin", 1)]
    [InlineData("gpsd-greis-def.bin", 257)]
    [InlineData("TDCO00SHN_20260826_10epochs.jps", 1)]
    [InlineData("TDCO00SHN_20260826_10epochs.jps", 257)]
    [InlineData("TDCO00SHN_20260826_nav.jps", 1)]
    [InlineData("TDCO00SHN_20260826_nav.jps", 257)]
    [InlineData("STEP_20260917_iono.jps", 1)]
    [InlineData("STEP_20260917_iono.jps", 257)]
    public void Replay_PublicRecording_PreservesEveryCompleteFrameAcrossReadSizes(
        string fileName,
        int readSize
    )
    {
        var data = ReadRecording(fileName, out var recording);
        var frames = ReadIndependentFrames(data, out var completeEnd);
        Assert.Equal(recording.CompleteEnd, completeEnd);
        Assert.Equal(recording.MessageCount, frames.Count);
        Assert.Equal(recording.MessageTypes, frames.Select(frame => frame.Id).Distinct().Count());
        Assert.Equal(recording.ParsedBytes, frames.Sum(frame => frame.Length));

        using var parser = CreateParser();
        var errors = new List<Exception>();
        var decoded = 0;
        var encoded = new byte[JavadProtocol.MaxPacketSize];
        using var errorsSubscription = parser.OnError.Subscribe(_ => { }, errors.Add, _ => { });
        using var messagesSubscription = parser.OnMessage.Subscribe(message =>
        {
            try
            {
                // Keep only counters and one frame buffer while replaying the recording.
                var expected = frames[decoded++];
                var typed = Assert.IsAssignableFrom<JavadMessageBase>(message);
                Assert.IsNotType<JavadRawMessage>(typed);
                Assert.Equal(expected.Id, typed.Id);
                Assert.Equal(expected.Length, typed.GetByteSize());
                var destination = encoded.AsSpan(0, expected.Length);
                typed.Serialize(ref destination);
                Assert.True(destination.IsEmpty);
                if (
                    !data.AsSpan(expected.Offset, expected.Length)
                        .SequenceEqual(encoded.AsSpan(0, expected.Length))
                )
                    Assert.Fail(
                        $"{fileName}: {expected.Id} at byte {expected.Offset} did not preserve the recorded frame."
                    );
            }
            catch (Exception error)
            {
                // Observer dispatch must not divert assertion failures to a global R3 handler.
                errors.Add(error);
            }
        });
        using var source = new MemoryStream(data, writable: false);
        var buffer = new byte[readSize];
        int read;
        while ((read = source.Read(buffer)) != 0)
            Push(parser, buffer.AsSpan(0, read));

        Assert.Empty(errors);
        Assert.Equal(recording.MessageCount, decoded);
        Assert.Equal((uint)recording.MessageCount, parser.Statistic.ParsedMessages);
        Assert.Equal((uint)recording.ParsedBytes, parser.Statistic.ParsedBytes);
    }

    [Theory]
    [InlineData(
        "javad_20110115.jps",
        2011,
        1,
        15,
        8_803_000U,
        8_932_000U,
        -3961904.1758905863,
        3348969.9682606705,
        3698226.855480116
    )]
    [InlineData(
        "gpsd-greis-def.bin",
        2018,
        3,
        30,
        72_119_000U,
        72_130_000U,
        -2386054.2708508726,
        -3922197.2258505463,
        4414357.277001726
    )]
    [InlineData(
        "TDCO00SHN_20260826_10epochs.jps",
        2026,
        8,
        26,
        0U,
        9_000U,
        4978301.738436176,
        -1086814.078446299,
        -3823381.103857969
    )]
    public void Replay_PublicObservationRecording_DecodesRecordedDatesTimesAndPositions(
        string fileName,
        int year,
        int month,
        int day,
        uint firstTime,
        uint lastTime,
        double x,
        double y,
        double z
    )
    {
        var data = ReadRecording(fileName, out _);
        using var parser = CreateParser();
        var errors = new List<Exception>();
        JavadReceiverDateMessage? date = null;
        JavadReceiverTimeMessage? first = null;
        JavadReceiverTimeMessage? last = null;
        JavadCartesianPositionVelocityMessage? position = null;
        using var errorsSubscription = parser.OnError.Subscribe(_ => { }, errors.Add, _ => { });
        using var messagesSubscription = parser.OnMessage.Subscribe(message =>
        {
            switch (message)
            {
                case JavadReceiverDateMessage value:
                    date ??= value;
                    break;
                case JavadReceiverTimeMessage value:
                    first ??= value;
                    last = value;
                    break;
                case JavadCartesianPositionVelocityMessage value:
                    position ??= value;
                    break;
            }
        });

        Push(parser, data);

        Assert.Empty(errors);
        Assert.NotNull(date);
        Assert.Equal(year, date.Year);
        Assert.Equal(month, date.Month);
        Assert.Equal(day, date.Day);
        Assert.Equal(0, date.ReferenceTimeScale);
        Assert.NotNull(first);
        Assert.NotNull(last);
        Assert.Equal(firstTime, first.TimeOfDay);
        Assert.Equal(lastTime, last.TimeOfDay);
        Assert.NotNull(position);
        Assert.Equal(x, position.X);
        Assert.Equal(y, position.Y);
        Assert.Equal(z, position.Z);
        Assert.Equal(1, position.SolutionType);
    }

    [Fact]
    public void Replay_OriginalTruncatedTail_DoesNotPublishOrFailAndResetDiscardsPartialFrame()
    {
        var data = ReadRecording("javad_20110115.jps", out var recording);
        var tail = data.AsSpan(recording.CompleteEnd);
        Assert.Equal(88, tail.Length);
        Assert.True(tail[..5].SequenceEqual("1p055"u8));
        using var parser = CreateParser();
        var count = 0;
        IProtocolMessage? last = null;
        var errors = new List<Exception>();
        using var errorsSubscription = parser.OnError.Subscribe(_ => { }, errors.Add, _ => { });
        using var messagesSubscription = parser.OnMessage.Subscribe(message =>
        {
            count++;
            last = message;
        });
        Push(parser, data.AsSpan(0, recording.CompleteEnd));
        Assert.Equal(5280, count);

        Push(parser, tail);

        Assert.Equal(5280, count);
        Assert.Empty(errors);
        parser.Reset();
        // First complete ~~ frame from the same unchanged recording.
        Push(parser, data.AsSpan(1455, 10));
        Assert.Equal(5281, count);
        Assert.Equal(8_803_000U, Assert.IsType<JavadReceiverTimeMessage>(last).TimeOfDay);
        Assert.Empty(errors);
    }

    [Fact]
    public void Replay_RecordedIonosphereTimes_PreservesOutOfRangeWireValues()
    {
        var data = ReadRecording("STEP_20260917_iono.jps", out _);
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var errorsSubscription = parser.OnError.Subscribe(_ => { }, errors.Add, _ => { });
        using var messagesSubscription = parser.OnMessage.Subscribe(messages.Add);

        Push(parser, data);

        Assert.Empty(errors);
        Assert.Equal(
            new uint[] { 0xFFFFFFFD, 503808, 30, 589824, 813694980 },
            messages.Select(message =>
                Assert.IsType<JavadGpsIonosphericParametersMessage>(message).Tot
            )
        );
    }

    private static byte[] ReadRecording(string fileName, out Recording recording)
    {
        recording = fileName switch
        {
            "javad_20110115.jps" => new(
                262144,
                "2A500C9BAB93F506E4F90FAC3DCC4E9CA309E57B54F12F6EE494E0929DDBEE26",
                5280,
                62,
                256698,
                262056
            ),
            "gpsd-greis-def.bin" => new(
                16188,
                "77189E70AE922BC690F3B6D8E7D1859805AC9718382265803F057F325D734F02",
                450,
                40,
                15527,
                16188
            ),
            "TDCO00SHN_20260826_10epochs.jps" => new(
                45775,
                "15072F226B749584DF21F71940F51DC3A8DF187CF8466794825995C761F16D03",
                491,
                48,
                45263,
                45775
            ),
            "TDCO00SHN_20260826_nav.jps" => new(
                14407,
                "EE2BC9BDB7B07CACE75D4B5B467D9A50EAD1E83AEF2DECD8084F81F5281EF2F0",
                150,
                11,
                14407,
                14407
            ),
            "STEP_20260917_iono.jps" => new(
                220,
                "93513B9A695E02D506D647C15A41DDB189ABCE77A010DEF7B5EC5D97F0BBFF45",
                5,
                1,
                220,
                220
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(fileName)),
        };
        var data = File.ReadAllBytes(
            Path.Combine(AppContext.BaseDirectory, "Resources", "Javad", fileName)
        );
        Assert.Equal(recording.Length, data.Length);
        Assert.Equal(recording.Sha256, Convert.ToHexString(SHA256.HashData(data)));
        return data;
    }

    // Boundary inventory is independent of JavadMessageParser and the production header reader.
    private static List<Frame> ReadIndependentFrames(byte[] data, out int completeEnd)
    {
        var frames = new List<Frame>();
        var offset = 0;
        while (offset < data.Length)
        {
            if (data[offset] is 13 or 10)
            {
                offset++;
                continue;
            }
            if (data[offset] == '#')
            {
                var lineEnd = Array.IndexOf(data, (byte)'\n', offset);
                Assert.True(lineEnd >= offset);
                offset = lineEnd + 1;
                continue;
            }
            Assert.True(data.Length - offset >= 5);
            Assert.InRange(data[offset], (byte)0x30, (byte)0x7E);
            Assert.InRange(data[offset + 1], (byte)0x30, (byte)0x7E);
            var length =
                5
                + int.Parse(
                    Encoding.ASCII.GetString(data, offset + 2, 3),
                    NumberStyles.AllowHexSpecifier,
                    CultureInfo.InvariantCulture
                );
            if (length > data.Length - offset)
                break;
            frames.Add(new(Encoding.ASCII.GetString(data, offset, 2), offset, length));
            offset += length;
        }
        completeEnd = offset;
        return frames;
    }

    private static IProtocolParser CreateParser() =>
        Protocol
            .Create(builder => builder.Protocols.RegisterJavadProtocol())
            .CreateParser(JavadProtocol.Info.Id);

    private static void Push(IProtocolParser parser, ReadOnlySpan<byte> data)
    {
        foreach (var value in data)
            parser.Push(value);
    }

    private readonly record struct Frame(string Id, int Offset, int Length);

    private readonly record struct Recording(
        int Length,
        string Sha256,
        int MessageCount,
        int MessageTypes,
        int ParsedBytes,
        int CompleteEnd
    );
}
