using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class RtcmV2EncoderTest
{
    private const string LegacyNative = "66040031631018FE8B002AAA";
    private const string LegacyWire = "664142404E4C5A4C42686743684B52405055555B";

    [Fact]
    public void Encode_LegacyFrame_ShouldMatchIndependentWireBytesAndPreserveFollowingStorage()
    {
        var message = new NativeMessage(Convert.FromHexString(LegacyNative));
        var encoder = new RtcmV2Encoder();
        var destination = Enumerable.Repeat((byte)0xCC, 25).ToArray();

        Assert.Equal(20, encoder.GetMaxByteSize(message));
        var written = encoder.Encode(message, destination);

        Assert.Equal(20, written);
        Assert.Equal(Convert.FromHexString(LegacyWire), destination[..written]);
        Assert.All(destination[written..], value => Assert.Equal(0xCC, value));
        Assert.All(destination[..written], value => Assert.InRange(value, (byte)0x40, (byte)0x7F));
        Assert.Equal(Convert.FromHexString(LegacyNative), message.Payload);
        Assert.Equal(1, message.SerializeCalls);
    }

    [Theory]
    [InlineData(42, "664142404E4C5A4C42686743684B52405055555B", "664142406B7365737D726743684B777F6F6A6A41")]
    [InlineData(43, "664142404E4C5A4C42686743684B5240505D5547", LegacyWire)]
    [InlineData(44, "664142404E4C5A4C42686743684B52405053557A", "597E7D7F717365737D57587C57746D7F6F6A6A64")]
    [InlineData(45, "664142404E4C5A4C42686743684B5240505B5566", "597E7D7F544C5A4C424D587C577448405055557E")]
    public void Encode_ConsecutiveMessages_ShouldCarryEveryD29D30State(
        byte iod,
        string firstExpected,
        string secondExpected)
    {
        var firstPayload = Convert.FromHexString(LegacyNative);
        firstPayload[^2] = iod;
        var encoder = new RtcmV2Encoder();

        Assert.Equal(Convert.FromHexString(firstExpected), Encode(encoder, new NativeMessage(firstPayload)));
        Assert.Equal(
            Convert.FromHexString(secondExpected),
            Encode(encoder, new NativeMessage(Convert.FromHexString(LegacyNative))));
    }

    [Fact]
    public void GetMaxByteSize_RepeatedQueries_ShouldNotSerializeOrAdvanceState()
    {
        var encoder = new RtcmV2Encoder();
        var first = Convert.FromHexString(LegacyNative);
        first[^2] = 44;
        Encode(encoder, new NativeMessage(first));
        var message = new NativeMessage(Convert.FromHexString(LegacyNative));

        for (var i = 0; i < 10; i++)
        {
            Assert.Equal(20, encoder.GetMaxByteSize(message));
        }

        Assert.Equal(0, message.SerializeCalls);
        Assert.Equal(
            Convert.FromHexString("597E7D7F717365737D57587C57746D7F6F6A6A64"),
            Encode(encoder, message));
        Assert.Equal(1, message.SerializeCalls);
    }

    [Fact]
    public void Encode_NewInstances_ShouldStartIndependentStreams()
    {
        var first = new RtcmV2Encoder();
        var second = new RtcmV2Encoder();
        var message = new NativeMessage(Convert.FromHexString(LegacyNative));

        Assert.Equal(Convert.FromHexString(LegacyWire), Encode(first, message));
        Assert.Equal(Convert.FromHexString(LegacyWire), Encode(second, message));
        Assert.Equal(
            Convert.FromHexString("664142406B7365737D726743684B777F6F6A6A41"),
            Encode(first, message));
        Assert.Equal(Convert.FromHexString(LegacyWire), Encode(new RtcmV2Encoder(), message));
        Assert.Equal(
            Convert.FromHexString("664142406B7365737D726743684B777F6F6A6A41"),
            Encode(second, message));
    }

    [Fact]
    public void Encode_InsufficientCapacity_ShouldPreserveExistingParityState()
    {
        var encoder = new RtcmV2Encoder();
        var message = new NativeMessage(Convert.FromHexString(LegacyNative));
        Encode(encoder, message);

        Assert.ThrowsAny<ArgumentException>(() => encoder.Encode(message, new byte[19]));

        Assert.Equal(
            Convert.FromHexString("664142406B7365737D726743684B777F6F6A6A41"),
            Encode(encoder, message));
    }

    [Fact]
    public void Encoder_NullMessage_ShouldRejectWithoutAdvancingState()
    {
        var encoder = new RtcmV2Encoder();

        Assert.Throws<ArgumentNullException>(() => encoder.GetMaxByteSize(null!));
        Assert.Throws<ArgumentNullException>(() => encoder.Encode(null!, new byte[20]));

        Assert.Equal(
            Convert.FromHexString(LegacyWire),
            Encode(encoder, new NativeMessage(Convert.FromHexString(LegacyNative))));
    }

    [Theory]
    [InlineData("too-short")]
    [InlineData("unaligned")]
    [InlineData("too-long")]
    [InlineData("protocol")]
    [InlineData("preamble")]
    [InlineData("length")]
    [InlineData("z-count")]
    [InlineData("incomplete-serialization")]
    [InlineData("serialization-exception")]
    public void Encode_InvalidMessage_ShouldPreserveExistingParityState(string failure)
    {
        var encoder = new RtcmV2Encoder();
        var first = Convert.FromHexString(LegacyNative);
        first[^2] = 44;
        Encode(encoder, new NativeMessage(first));
        var invalid = CreateInvalidMessage(failure);

        Assert.ThrowsAny<Exception>(() => encoder.Encode(invalid, new byte[170]));

        Assert.Equal(
            Convert.FromHexString("597E7D7F717365737D57587C57746D7F6F6A6A64"),
            Encode(encoder, new NativeMessage(Convert.FromHexString(LegacyNative))));
    }

    [Fact]
    public void Encode_VaryingPayloads_ShouldMatchGpsParityEquationsAcrossAllStateTransitions()
    {
        var encoder = new RtcmV2Encoder();
        var state = 0;
        var transitions = new HashSet<int>();
        uint random = 0x10423;

        for (var sample = 0; sample < 128; sample++)
        {
            var payloadWords = sample % 32;
            var native = new byte[(payloadWords + 2) * 3];
            for (var i = 0; i < native.Length; i++)
            {
                random = unchecked(random * 1664525 + 1013904223);
                native[i] = (byte)(random >> 24);
            }

            // Type 16 has opaque text data; the encoder must not interpret payload words.
            var zCount = sample * 47 % 6000;
            native[0] = 0x66;
            native[1] = (byte)(0x40 | (sample >> 8));
            native[2] = (byte)sample;
            native[3] = (byte)(zCount >> 5);
            native[4] = (byte)((zCount << 3) | (sample & 7));
            native[5] = (byte)((payloadWords << 3) | (sample & 7));
            var message = new NativeMessage(native);
            var snapshot = native.ToArray();
            var expected = EncodeWithIndependentParity(native, ref state, transitions);

            Assert.Equal((payloadWords + 2) * 5, encoder.GetMaxByteSize(message));
            Assert.Equal(expected, Encode(encoder, message));
            Assert.Equal(snapshot, native);
        }

        Assert.Equal(Enumerable.Range(0, 16), transitions.OrderBy(value => value));
    }

    [Fact]
    public void Encode_ParsedModel_ShouldPreserveModelAndNormalizedSerialization()
    {
        var message = new RtcmV2Message1();
        ReadOnlySpan<byte> source = Convert.FromHexString(LegacyNative);
        message.Deserialize(ref source);
        var observations = message.ObservationItems;
        var observation = Assert.Single(observations);
        var gpsTime = message.GpsTime;
        var tk = observation.Tk;
        var satellite = observation.SatelliteId;
        var native = Serialize(message);
        var encoder = new RtcmV2Encoder();

        Assert.Equal(Convert.FromHexString(LegacyWire), Encode(encoder, message));
        Encode(encoder, message);

        Assert.Equal(Convert.FromHexString(LegacyNative), native);
        Assert.Equal(native, Serialize(message));
        Assert.Same(observations, message.ObservationItems);
        Assert.Same(observation, Assert.Single(message.ObservationItems));
        Assert.Equal(948.0, message.ZCount);
        Assert.Equal(3, message.SequenceNumber);
        Assert.Equal(0, message.ReferenceStationId);
        Assert.Equal(1.0, message.Udre);
        Assert.Equal(gpsTime, message.GpsTime);
        Assert.Equal(24, observation.Prn);
        Assert.Equal(-7.46, observation.Prc, 8);
        Assert.Equal(0, observation.Rrc);
        Assert.Equal(42, observation.Iod);
        Assert.Equal(SatUdreEnum.LessOne, observation.Udre);
        Assert.Equal(satellite, observation.SatelliteId);
        Assert.Equal(tk, observation.Tk);
    }

    [Fact]
    public async Task RegisterRtcmV2Protocol_VirtualPortSend_ShouldUseIndependentEncodersPerPort()
    {
        var protocol = Protocol.Create(builder => builder.Protocols.RegisterRtcmV2Protocol());
        var context = (IProtocolContext)protocol;
        using var first = new VirtualPort("rtcm2-encoder-first", context, static _ => true, new Statistic());
        using var second = new VirtualPort("rtcm2-encoder-second", context, static _ => true, new Statistic());
        var firstWire = new List<byte[]>();
        var secondWire = new List<byte[]>();
        var errors = new List<Exception>();
        using var firstTx = first.Tx.Subscribe(firstWire.Add);
        using var secondTx = second.Tx.Subscribe(secondWire.Add);
        using var firstError = first.OnTxError.Subscribe(errors.Add);
        using var secondError = second.OnTxError.Subscribe(errors.Add);
        var firstNative = Convert.FromHexString(LegacyNative);
        firstNative[^2] = 44;
        var original = new NativeMessage(Convert.FromHexString(LegacyNative));

        await first.Send(new NativeMessage(firstNative), TestContext.Current.CancellationToken);
        await second.Send(original, TestContext.Current.CancellationToken);
        await first.Send(original, TestContext.Current.CancellationToken);
        await second.Send(original, TestContext.Current.CancellationToken);

        Assert.Empty(errors);
        Assert.Equal(2, firstWire.Count);
        Assert.Equal(2, secondWire.Count);
        Assert.Equal(Convert.FromHexString("664142404E4C5A4C42686743684B52405053557A"), firstWire[0]);
        Assert.Equal(Convert.FromHexString("597E7D7F717365737D57587C57746D7F6F6A6A64"), firstWire[1]);
        Assert.Equal(Convert.FromHexString(LegacyWire), secondWire[0]);
        Assert.Equal(Convert.FromHexString("664142406B7365737D726743684B777F6F6A6A41"), secondWire[1]);
    }

    [Fact]
    public async Task RegisterRtcmV2Protocol_Type9_ShouldRoundTripThroughVirtualPort()
    {
        var protocol = Protocol.Create(builder => builder.Protocols.RegisterRtcmV2Protocol());
        using var port = new VirtualPort("rtcm2-type9", (IProtocolContext)protocol, static _ => true, new Statistic());
        var received = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        var wire = new List<byte[]>();
        using var rx = port.OnRxMessage.Subscribe(received.Add);
        using var txError = port.OnTxError.Subscribe(errors.Add);
        using var rxError = port.OnRxError.Subscribe(errors.Add);
        using var parserErrors = port.Parsers.Single().OnError.Subscribe(errors.Add, errors.Add, _ => { });
        using var tx = port.Tx.Subscribe(bytes =>
        {
            wire.Add(bytes);
            port.Rx.OnNext(bytes);
        });
        var native = Convert.FromHexString(LegacyNative);
        native[1] = 0x24;
        var message = new RtcmV2Message9();
        ReadOnlySpan<byte> source = native;
        message.Deserialize(ref source);

        await port.Send(message, TestContext.Current.CancellationToken);

        Assert.Empty(errors);
        Assert.Equal(Convert.FromHexString("665142406D7365737D726743684B777F6F6A6A41"), Assert.Single(wire));
        var actual = Assert.IsType<RtcmV2Message9>(Assert.Single(received));
        Assert.Equal(948.0, actual.ZCount);
        Assert.Equal(3, actual.SequenceNumber);
        Assert.Equal(0, actual.ReferenceStationId);
        var observation = Assert.Single(actual.ObservationItems);
        Assert.Equal(24, observation.Prn);
        Assert.Equal(-7.46, observation.Prc, 8);
        Assert.Equal(0, observation.Rrc);
        Assert.Equal(42, observation.Iod);
    }

    [Fact]
    public void RegisterRtcmV2Protocol_ParserOnlyBuilder_ShouldPreserveRegistrationCompatibility()
    {
        var builder = new ParserOnlyBuilder();

        builder.RegisterRtcmV2Protocol();

        Assert.Same(RtcmV2Protocol.Info, Assert.Single(builder.Protocols));
    }

    private static byte[] Encode(IProtocolEncoder encoder, IProtocolMessage message)
    {
        var destination = new byte[encoder.GetMaxByteSize(message)];
        var written = encoder.Encode(message, destination);
        Assert.InRange(written, 0, destination.Length);
        return destination[..written];
    }

    private static byte[] Serialize(IProtocolMessage message)
    {
        var native = new byte[message.GetByteSize()];
        Span<byte> destination = native;
        message.Serialize(ref destination);
        Assert.True(destination.IsEmpty);
        return native;
    }

    private static NativeMessage CreateInvalidMessage(string failure)
    {
        var message = new NativeMessage(Convert.FromHexString(LegacyNative));
        switch (failure)
        {
            case "too-short":
                return new NativeMessage(new byte[3]);
            case "unaligned":
                return new NativeMessage(new byte[7]);
            case "too-long":
                return new NativeMessage(new byte[102]);
            case "protocol":
                message.Protocol = new ProtocolInfo("different-protocol", "Different protocol");
                break;
            case "preamble":
                message.Payload[0] = 0x67;
                break;
            case "length":
                message.Payload[5] = 0x18;
                break;
            case "z-count":
                message.Payload[3] = 0xBB;
                message.Payload[4] = 0x83;
                break;
            case "incomplete-serialization":
                message.UnconsumedBytes = 3;
                break;
            case "serialization-exception":
                message.FailSerialization = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(failure));
        }
        return message;
    }

    private static byte[] EncodeWithIndependentParity(ReadOnlySpan<byte> native, ref int state, ISet<int> transitions)
    {
        // GPS SPS Signal Specification (1995), Table 2-14, numbers d1..d24 from the MSB.
        // These explicit XOR equations are independent of the decoder's Hamming masks.
        int[][] equations =
        [
            [1, 2, 3, 5, 6, 10, 11, 12, 13, 14, 17, 18, 20, 23],
            [2, 3, 4, 6, 7, 11, 12, 13, 14, 15, 18, 19, 21, 24],
            [1, 3, 4, 5, 7, 8, 12, 13, 14, 15, 16, 19, 20, 22],
            [2, 4, 5, 6, 8, 9, 13, 14, 15, 16, 17, 20, 21, 23],
            [1, 3, 5, 6, 7, 9, 10, 14, 15, 16, 17, 18, 21, 22, 24],
            [3, 5, 6, 8, 9, 10, 11, 13, 15, 19, 22, 23, 24]
        ];
        var result = new byte[native.Length / 3 * 5];
        var output = 0;
        for (var offset = 0; offset < native.Length; offset += 3)
        {
            var raw = (uint)((native[offset] << 16) | (native[offset + 1] << 8) | native[offset + 2]);
            uint parity = 0;
            for (var check = 0; check < equations.Length; check++)
            {
                var bit = check is 0 or 2 or 5 ? (uint)(state >> 1) : (uint)(state & 1);
                foreach (var index in equations[check])
                {
                    bit ^= (raw >> (24 - index)) & 1;
                }
                parity = (parity << 1) | bit;
            }

            var transmittedData = (state & 1) == 0 ? raw : raw ^ 0xFFFFFF;
            var word = (transmittedData << 6) | parity;
            var next = (int)(word & 3);
            transitions.Add((state << 2) | next);
            state = next;
            for (var shift = 24; shift >= 0; shift -= 6)
            {
                var chunk = (word >> shift) & 0x3F;
                var rolled = 0;
                for (var bit = 0; bit < 6; bit++)
                {
                    rolled = (rolled << 1) | (int)(chunk & 1);
                    chunk >>= 1;
                }
                result[output++] = (byte)(0x40 | rolled);
            }
        }
        return result;
    }

    private sealed class NativeMessage(byte[] payload) : IProtocolMessage
    {
        private ProtocolTags _tags = [];

        public byte[] Payload { get; } = payload;
        public int SerializeCalls { get; private set; }
        public int UnconsumedBytes { get; set; }
        public bool FailSerialization { get; set; }
        public ProtocolInfo Protocol { get; set; } = RtcmV2Protocol.Info;
        public string Name => nameof(NativeMessage);
        public ref ProtocolTags Tags => ref _tags;
        public string GetIdAsString() => (Payload[1] >> 2).ToString();
        public int GetByteSize() => Payload.Length;

        public void Serialize(ref Span<byte> buffer)
        {
            SerializeCalls++;
            Payload.AsSpan().CopyTo(buffer);
            if (FailSerialization)
            {
                throw new InvalidOperationException("Synthetic serialization failure.");
            }
            buffer = buffer[(Payload.Length - UnconsumedBytes)..];
        }

        public void Deserialize(ref ReadOnlySpan<byte> buffer) => throw new NotSupportedException();
    }

    private sealed class ParserOnlyBuilder : IProtocolParserBuilder
    {
        public List<ProtocolInfo> Protocols { get; } = [];
        public void Clear() => Protocols.Clear();
        public void Register(ProtocolInfo info, ParserFactoryDelegate factory) => Protocols.Add(info);
    }
}
