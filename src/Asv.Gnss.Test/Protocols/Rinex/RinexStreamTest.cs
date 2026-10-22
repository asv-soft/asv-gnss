using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class RinexStreamTest
{
    [Fact]
    public void Register_DefaultFactory_ContainsEveryNativeRecordType()
    {
        var registered = new List<RinexMessageId>();
        using var parser = CreateParser(factory =>
        {
            foreach (var id in Enum.GetValues<RinexMessageId>())
            {
                Assert.True(factory.Remove(id));
                registered.Add(id);
            }
            factory.Add<RinexHeaderMessage>().Add<RinexObservationEpochMessage>();
        });
        Assert.Equal(new[] { RinexMessageId.Header, RinexMessageId.ObservationEpoch,
            RinexMessageId.Navigation, RinexMessageId.Meteorological }, registered);
        Assert.Equal(RinexProtocol.Info, parser.Info);
    }

    [Fact]
    public void Register_CustomFactory_ObservationOnlyStillReceivesHeaderContext()
    {
        using var parser = CreateParser(factory =>
        {
            factory.Clear();
            factory.Add<RinexObservationEpochMessage>();
        });
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var header = Header(3.04m, "C1C");
        Push(parser, File(header, Observation(header, 123.456)));
        parser.Complete();
        var observation = Assert.IsType<RinexObservationEpochMessage>(Assert.Single(messages));
        Assert.Equal("C1C", Assert.Single(Assert.Single(observation.Satellites).Observations).Code);
        Assert.Equal(123.456, observation.Satellites[0].Observations[0].Value);
    }

    [Theory]
    [InlineData("\n", 1)]
    [InlineData("\r\n", 7)]
    [InlineData("\r", 113)]
    public void Push_ChunkedAsciiAcrossLineEndings_PublishesTypedEpochs(string newline, int chunkSize)
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        var header = Header(3.04m, "C1C");
        var first = Observation(header, 12.125);
        var second = Observation(header, 24.250);
        second.Epoch = second.Epoch with { Seconds = 30m };
        var bytes = Encoding.ASCII.GetBytes(File(header, first, second).Replace("\n", newline, StringComparison.Ordinal));
        for (var offset = 0; offset < bytes.Length; offset += chunkSize)
            Push(parser, bytes.AsSpan(offset, Math.Min(chunkSize, bytes.Length - offset)));
        Assert.Equal(2, messages.Count);
        Assert.True(parser.Complete());
        Assert.Equal(3, messages.Count);
        Assert.IsType<RinexHeaderMessage>(messages[0]);
        Assert.Equal(new double?[] { 12.125, 24.250 }, messages.OfType<RinexObservationEpochMessage>()
            .Select(message => Assert.Single(Assert.Single(message.Satellites).Observations).Value));
        Assert.Empty(errors);
    }

    [Fact]
    public void Complete_FinalLineWithoutNewline_PublishesOnceAndIsIdempotent()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var header = Header(3.04m, "C1C");
        Push(parser, File(header, Observation(header, 123.456)).TrimEnd('\n'));
        Assert.IsType<RinexHeaderMessage>(Assert.Single(messages));
        Assert.True(parser.Complete());
        Assert.Equal(2, messages.Count);
        Assert.False(parser.Complete());
        Assert.Equal(2, messages.Count);
    }

    [Fact]
    public void Push_Utf8BomSplitAcrossBytes_ParsesAsciiFileAndResetAllowsAnotherBom()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        var header = Header(3.04m, "C1C");
        for (var i = 0; i < 2; i++)
        {
            parser.Reset();
            foreach (var value in new byte[] { 0xEF, 0xBB, 0xBF }) parser.Push(value);
            Push(parser, File(header, Observation(header, i + 0.125)));
            parser.Complete();
        }
        Assert.Equal(4, messages.Count);
        Assert.Equal(new double?[] { 0.125, 1.125 }, messages.OfType<RinexObservationEpochMessage>()
            .Select(message => message.Satellites[0].Observations[0].Value));
        Assert.Empty(errors);
    }

    [Fact]
    public void Complete_TruncatedLegacyEpoch_ReportsErrorAndThrows()
    {
        using var parser = CreateParser();
        var errors = new List<Exception>();
        using var subscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        var header = Header(2.11m, "C1");
        var record = Text(Observation(header, 123.456));
        Push(parser, Text(new RinexHeaderMessage { Header = header }) + record.Split('\n')[0]);
        Assert.Throws<FormatException>(() => parser.Complete());
        Assert.Single(errors);
    }

    [Fact]
    public void Reset_PartialHeaderAndBody_DiscardsOldContextAndAcceptsNewFile()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var firstHeader = Header(3.04m, "C1C");
        var firstFile = File(firstHeader, Observation(firstHeader, 12.125));
        Push(parser, firstFile[..20]);
        parser.Reset();
        Assert.Null(parser.CurrentHeader);
        Push(parser, firstFile);
        Assert.Single(messages);
        parser.Reset();
        Assert.Null(parser.CurrentHeader);
        Push(parser, Text(Observation(firstHeader, 99.999)));
        Assert.False(parser.Complete());
        Assert.Single(messages);
        var secondHeader = Header(3.05m, "L2W");
        Push(parser, File(secondHeader, Observation(secondHeader, 24.250)));
        parser.Complete();
        Assert.Equal(3, messages.Count);
        var actual = Assert.IsType<RinexObservationEpochMessage>(messages[^1]);
        Assert.Equal(3.05m, actual.Header.Version);
        Assert.Equal("L2W", Assert.Single(Assert.Single(actual.Satellites).Observations).Code);
        Assert.Equal(24.250, actual.Satellites[0].Observations[0].Value);
    }

    [Fact]
    public void Push_ConcatenatedFiles_SeparatesHeaderAndObservationContexts()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var firstHeader = Header(2.11m, "C1");
        var secondHeader = Header(4.02m, "L2W");
        Push(parser, File(firstHeader, Observation(firstHeader, 1.25)) + File(secondHeader, Observation(secondHeader, 2.5)));
        parser.Complete();
        Assert.Equal(4, messages.Count);
        var first = Assert.IsType<RinexObservationEpochMessage>(messages[1]);
        var second = Assert.IsType<RinexObservationEpochMessage>(messages[3]);
        Assert.Equal(2.11m, first.Header.Version);
        Assert.Equal(4.02m, second.Header.Version);
        Assert.NotSame(first.Header, second.Header);
        Assert.Equal("C1", first.Satellites[0].Observations[0].Code);
        Assert.Equal("L2W", second.Satellites[0].Observations[0].Code);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Push_SpecialEventWithBlankEpoch_PreservesRecordsAndFollowingObservation(int flag)
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var header = Header(3.04m, "C1C");
        var special = new RinexObservationEpochMessage { Header = header, EpochFlag = flag };
        special.SpecialRecords.Add(RinexHeader.FormatLine("Event station", flag is 3 or 4 ? "MARKER NAME" : "COMMENT"));
        Push(parser, File(header, special, Observation(header, 123.456)));
        parser.Complete();
        Assert.Equal(3, messages.Count);
        var actual = Assert.IsType<RinexObservationEpochMessage>(messages[1]);
        Assert.Equal(flag, actual.EpochFlag);
        Assert.Equal(default, actual.Epoch);
        Assert.Equal(special.SpecialRecords, actual.SpecialRecords);
        Assert.Equal(123.456, Assert.IsType<RinexObservationEpochMessage>(messages[2]).Satellites[0].Observations[0].Value);
    }

    [Fact]
    public void Push_HeaderUpdateEvent_UsesNewObservationCodesWithoutMutatingPreviousEpoch()
    {
        using var parser = CreateParser(factory => factory.Remove(RinexMessageId.Header));
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var original = Header(3.04m, "C1C");
        var updated = Header(3.04m, "L1C", "S1C");
        var change = new RinexObservationEpochMessage { Header = original, EpochFlag = 4 };
        change.SpecialRecords.Add(RinexHeader.FormatLine("G    2 L1C S1C", "SYS / # / OBS TYPES"));
        var last = Observation(updated, 77.125);
        last.Satellites[0].Observations.Add(new RinexObservationValue("S1C", 45.5));
        Push(parser, File(original, Observation(original, 12.25), change, last));
        parser.Complete();
        Assert.Equal(3, messages.Count);
        var before = Assert.IsType<RinexObservationEpochMessage>(messages[0]);
        var after = Assert.IsType<RinexObservationEpochMessage>(messages[2]);
        Assert.Equal("C1C", Assert.Single(before.Satellites[0].Observations).Code);
        Assert.Equal(new[] { "C1C" }, before.Header.ObservationTypes['G']);
        Assert.Equal(new[] { "L1C", "S1C" }, after.Satellites[0].Observations.Select(value => value.Code));
        Assert.Equal(new double?[] { 77.125, 45.5 }, after.Satellites[0].Observations.Select(value => value.Value));
        Assert.Equal(new[] { "L1C", "S1C" }, parser.CurrentHeader!.ObservationTypes['G']);
    }

    [Theory]
    [InlineData("2.11")]
    [InlineData("3.04")]
    public void Observation_NullAndZeroAndIndicators_RemainDistinct(string version)
    {
        var modern = version == "3.04";
        var codes = modern ? new[] { "C1C", "L1C", "S1C" } : new[] { "C1", "L1", "S1" };
        var header = Header(decimal.Parse(version, System.Globalization.CultureInfo.InvariantCulture), codes);
        var source = Observation(header, null);
        source.Satellites[0].Observations.Add(new RinexObservationValue(codes[1], 0, 0, 0));
        source.Satellites[0].Observations.Add(new RinexObservationValue(codes[2], 45.25, 7, 9));
        var actual = DecodeObservation(header, Encode(source));
        Assert.Equal(source.Satellites[0].Observations, actual.Satellites[0].Observations);
        Assert.Null(actual.Satellites[0].Observations[0].Value);
        Assert.Null(actual.Satellites[0].Observations[0].LossOfLockIndicator);
        Assert.Equal(0d, actual.Satellites[0].Observations[1].Value);
        Assert.Equal((byte)0, actual.Satellites[0].Observations[1].LossOfLockIndicator);
        Assert.Equal((byte)0, actual.Satellites[0].Observations[1].SignalStrengthIndicator);
        Assert.Equal((byte)7, actual.Satellites[0].Observations[2].LossOfLockIndicator);
        Assert.Equal((byte)9, actual.Satellites[0].Observations[2].SignalStrengthIndicator);
    }

    [Fact]
    public void Observation402_PicosecondEpoch_UsesIndependentFiveDigitExtensionColumns()
    {
        var header = Header(4.02m, "C1C");
        var source = Observation(header, 123.456);
        source.Epoch = new RinexEpoch(2025, 9, 1, 12, 30, 23.123456789012m);
        source.ReceiverClockOffset = -0.123456789012;
        var text = Text(source);
        var epochLine = text.Split('\n')[0];
        Assert.Equal(" 23.1234567", epochLine.Substring(18, 11));
        Assert.Equal("-0.123456789012", epochLine.Substring(41, 15).Trim());
        Assert.Equal(' ', epochLine[56]);
        Assert.Equal("89012", epochLine.Substring(57, 5));
        var actual = DecodeObservation(header, Encoding.ASCII.GetBytes(text));
        Assert.Equal(source.Epoch, actual.Epoch);
        Assert.Equal(source.ReceiverClockOffset, actual.ReceiverClockOffset);
    }

    [Theory]
    [InlineData("60.99999994", false)]
    [InlineData("60.99999999", true)]
    public void Observation3_SecondsQuantization_PreservesLeapSecondOrRejectsCarryOutsideInterval(string seconds, bool invalid)
    {
        var header = Header(3.04m, "C1C");
        var source = Observation(header, 123.456);
        source.Epoch = new RinexEpoch(2025, 9, 1, 0, 2, decimal.Parse(seconds, System.Globalization.CultureInfo.InvariantCulture));
        if (invalid)
        {
            Assert.Throws<FormatException>(() => Encode(source));
            return;
        }
        var text = Text(source);
        Assert.StartsWith("> 2025 09 01 00 02", text, StringComparison.Ordinal);
        var actual = DecodeObservation(header, Encoding.ASCII.GetBytes(text));
        Assert.Equal(60.9999999m, actual.Epoch.Seconds);
    }

    [Fact]
    public void Deserialize_ConcatenatedHeaderAndEpochs_ConsumesExactlyOneRecord()
    {
        var header = Header(3.04m, "C1C");
        var first = Observation(header, 1.25);
        var second = Observation(header, 2.5);
        var firstBytes = Encode(first);
        var secondBytes = Encode(second);
        ReadOnlySpan<byte> read = Encoding.ASCII.GetBytes(File(header, first, second));
        var headerMessage = new RinexHeaderMessage();
        headerMessage.Deserialize(ref read);
        Assert.Equal(firstBytes.Length + secondBytes.Length, read.Length);
        var firstResult = new RinexObservationEpochMessage { Header = headerMessage.Header };
        firstResult.Deserialize(ref read);
        Assert.Equal(secondBytes, read.ToArray());
        var secondResult = new RinexObservationEpochMessage { Header = headerMessage.Header };
        secondResult.Deserialize(ref read);
        Assert.True(read.IsEmpty);
        Assert.Equal(1.25, firstResult.Satellites[0].Observations[0].Value);
        Assert.Equal(2.5, secondResult.Satellites[0].Observations[0].Value);
    }

    [Fact]
    public void Observation_FieldWidthOverflowAndUndeclaredCodes_RejectInvalidValues()
    {
        var header = Header(3.04m, "C1C");
        var source = Observation(header, 123456789012345d);
        Assert.Throws<FormatException>(() => Encode(source));
        source.Satellites[0].Observations[0] = new RinexObservationValue("L2W", 1);
        Assert.Throws<FormatException>(() => Encode(source));
        source.Satellites[0].Observations[0] = new RinexObservationValue("C1C", 1, 8);
        Assert.Throws<FormatException>(() => Encode(source));
    }

    [Fact]
    public void Observation211_SatelliteAndObservationContinuations_PreserveThirteenSatellites()
    {
        var header = Header(2.11m, "C1", "L1", "D1", "S1", "C2", "L2");
        var source = Observation(header, 1);
        source.Satellites.Clear();
        for (var i = 1; i <= 13; i++)
        {
            var satellite = new RinexSatelliteObservation { Satellite = $"G{i:00}" };
            foreach (var code in header.LegacyObservationTypes)
                satellite.Observations.Add(new RinexObservationValue(code, i + 0.125));
            source.Satellites.Add(satellite);
        }
        var text = Text(source);
        var lines = text.Split('\n');
        Assert.Equal("G13", lines[1][32..]);
        Assert.Equal(2 + 13 * 2 + 1, lines.Length);
        var actual = DecodeObservation(header, Encoding.ASCII.GetBytes(text));
        Assert.Equal(13, actual.Satellites.Count);
        for (var i = 0; i < actual.Satellites.Count; i++)
        {
            Assert.Equal(source.Satellites[i].Satellite, actual.Satellites[i].Satellite);
            Assert.Equal(source.Satellites[i].Observations, actual.Satellites[i].Observations);
        }
    }

    [Fact]
    public void Observation3_WrappedAndPaddedSatelliteLine_KeepsFieldsAligned()
    {
        var header = Header(3.04m, "C1C", "L1C", "D1C", "S1C", "C2W");
        var source = Observation(header, 1);
        for (var i = 1; i < header.ObservationTypes['G'].Count; i++)
            source.Satellites[0].Observations.Add(new RinexObservationValue(header.ObservationTypes['G'][i], i + 1));
        var lines = Text(source).Split('\n');
        var satellite = lines[1];
        var wrapped = lines[0] + "\n" + satellite[..67].PadRight(80) + "\n   " + satellite[67..] + "\n";
        var actual = DecodeObservation(header, Encoding.ASCII.GetBytes(wrapped));
        Assert.Equal(source.Satellites[0].Observations, actual.Satellites[0].Observations);
    }

    [Theory]
    [InlineData("2.11")]
    [InlineData("3.04")]
    public void Observation_CycleSlipFlag6_UsesObservationLayout(string version)
    {
        var header = Header(decimal.Parse(version, System.Globalization.CultureInfo.InvariantCulture), version == "2.11" ? "L1" : "L1C");
        var source = Observation(header, 2);
        source.EpochFlag = 6;
        source.Satellites[0].Observations[0] = source.Satellites[0].Observations[0] with { LossOfLockIndicator = 0 };
        var actual = DecodeObservation(header, Encode(source));
        Assert.Equal(6, actual.EpochFlag);
        Assert.Empty(actual.SpecialRecords);
        Assert.Equal(source.Satellites[0].Observations, actual.Satellites[0].Observations);
    }

    private static RinexHeader Header(decimal version, params string[] codes)
    {
        var header = new RinexHeader { Version = version, FileType = 'O', SatelliteSystem = 'G' };
        if (version < 3) header.LegacyObservationTypes.AddRange(codes);
        else header.ObservationTypes['G'] = codes.ToList();
        return header;
    }

    private static RinexObservationEpochMessage Observation(RinexHeader header, double? value)
    {
        var message = new RinexObservationEpochMessage
        {
            Header = header,
            Epoch = new RinexEpoch(2025, 9, 1, 12, 30, 0)
        };
        var satellite = new RinexSatelliteObservation { Satellite = "G07" };
        satellite.Observations.Add(new RinexObservationValue(header.Version < 3 ? header.LegacyObservationTypes[0] : header.ObservationTypes['G'][0], value));
        message.Satellites.Add(satellite);
        return message;
    }

    private static RinexObservationEpochMessage DecodeObservation(RinexHeader header, byte[] bytes)
    {
        var result = new RinexObservationEpochMessage { Header = header };
        ReadOnlySpan<byte> read = bytes;
        result.Deserialize(ref read);
        Assert.True(read.IsEmpty);
        return result;
    }

    private static byte[] Encode(RinexMessageBase message)
    {
        var result = new byte[message.GetByteSize()];
        var write = result.AsSpan();
        message.Serialize(ref write);
        Assert.True(write.IsEmpty);
        return result;
    }

    private static string Text(RinexMessageBase message) => Encoding.ASCII.GetString(Encode(message));
    private static string File(RinexHeader header, params RinexMessageBase[] records) =>
        Text(new RinexHeaderMessage { Header = header }) + string.Concat(records.Select(Text));

    private static RinexMessageParser CreateParser(Action<IProtocolMessageFactoryBuilder<RinexMessageBase, RinexMessageId>>? configure = null) =>
        Assert.IsType<RinexMessageParser>(Protocol.Create(builder => builder.Protocols.RegisterRinexProtocol(configure)).CreateParser(RinexProtocol.Info.Id));

    private static void Push(IProtocolParser parser, string text) => Push(parser, Encoding.ASCII.GetBytes(text));
    private static void Push(IProtocolParser parser, ReadOnlySpan<byte> bytes)
    {
        foreach (var value in bytes) parser.Push(value);
    }
}
