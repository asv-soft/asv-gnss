using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class RawNavigationPortTest
{
    // The existing UBX GPS source words, independently parity-encoded using IS-GPS-200
    // Table 20-XIV, with D29/D30 zero at the HOW and subframe boundaries.
    private const string GpsFrame =
        "08B0164F226AEA68E4FC070731985F1D6FC17140DD7FD283B83C3FC37D7ADF3364E6661260CC";

    // The original UBX-RXM-SFRBX sample, including all eight GLONASS check bits.
    private const string GlonassFrame = "01003896A022064406BEA0";

    [Fact]
    public void Register_NavigationProtocols_ExposeEveryTypedMessageAndProtocolMetadata()
    {
        IProtocolMessageFactory<GpsSubframeBase, byte>? gps = null;
        IProtocolMessageFactory<GlonassWordBase, byte>? glonass = null;
        var protocol = Protocol.Create(builder =>
        {
            builder.Protocols.RegisterRawGpsProtocol(factory =>
                gps = ((ProtocolMessageFactoryBuilder<GpsSubframeBase, byte>)factory).Build()
            );
            builder.Protocols.RegisterRawGlonassProtocol(factory =>
                glonass = ((ProtocolMessageFactoryBuilder<GlonassWordBase, byte>)factory).Build()
            );
        });

        Assert.NotNull(gps);
        Assert.NotNull(glonass);
        Assert.Equal(
            new[] { "RawGlonass", "RawGps" },
            protocol.AvailableProtocols.Select(x => x.Id).Order()
        );
        Assert.Equal("GPS LNAV words", gps.Info.Name);
        Assert.Equal("GLONASS navigation strings", glonass.Info.Name);
        AssertCatalog(
            gps,
            38,
            typeof(GpsSubframe1),
            typeof(GpsSubframe2),
            typeof(GpsSubframe3),
            typeof(GpsSubframe4),
            typeof(GpsSubFrame5)
        );
        AssertCatalog(
            glonass,
            11,
            typeof(GlonassWord1),
            typeof(GlonassWord2),
            typeof(GlonassWord3),
            typeof(GlonassWord4),
            typeof(GlonassWord5),
            typeof(GlonassWord6),
            typeof(GlonassWord7),
            typeof(GlonassWord8),
            typeof(GlonassWord9),
            typeof(GlonassWord10),
            typeof(GlonassWord11),
            typeof(GlonassWord12),
            typeof(GlonassWord13),
            typeof(GlonassWord14),
            typeof(GlonassWord15)
        );
        using var gpsParser = protocol.CreateParser("RawGps");
        using var glonassParser = protocol.CreateParser("RawGlonass");
        Assert.IsType<GpsSubframeParser>(gpsParser);
        Assert.IsType<GlonassWordParser>(glonassParser);
        Assert.Equal(gps.Info, gpsParser.Info);
        Assert.Equal(glonass.Info, glonassParser.Info);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SendAndReceive_NativeNavigationCheckBits_UseExistingPortContracts(bool glonass)
    {
        var frame = Convert.FromHexString(glonass ? GlonassFrame : GpsFrame);
        IProtocolMessage message = glonass ? new GlonassWord1() : new GpsSubframe2();
        ReadOnlySpan<byte> read = frame;
        message.Deserialize(ref read);
        Assert.True(read.IsEmpty);

        var protocol = CreateProtocol(glonass);
        using var port = new VirtualPort(
            "native-navigation",
            (IProtocolContext)protocol,
            static _ => true,
            new Statistic()
        );
        var wire = new List<byte[]>();
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var tx = port.Tx.Subscribe(wire.Add);
        using var rx = port.OnRxMessage.Subscribe(messages.Add);
        using var txError = port.OnTxError.Subscribe(errors.Add);
        using var rxError = port.OnRxError.Subscribe(errors.Add);
        using var parserError = port
            .Parsers.Single()
            .OnError.Subscribe(errors.Add, errors.Add, _ => { });

        await port.Send(message, TestContext.Current.CancellationToken);
        port.Rx.OnNext(frame);

        Assert.Empty(errors);
        Assert.Equal(frame, Assert.Single(wire));
        var decoded = Assert.Single(messages);
        Assert.Equal(glonass ? RawGlonassProtocol.Info : RawGpsProtocol.Info, decoded.Protocol);
        AssertTypedFrame(decoded, frame, glonass);
    }

    [Theory]
    [InlineData(false, 1)]
    [InlineData(false, 7)]
    [InlineData(false, 4096)]
    [InlineData(true, 1)]
    [InlineData(true, 7)]
    [InlineData(true, 4096)]
    public void Receive_ContiguousNativeNavigationFrames_PreservesBitsAcrossDispatcherResets(
        bool glonass,
        int chunkSize
    )
    {
        var frame = Convert.FromHexString(glonass ? GlonassFrame : GpsFrame);
        var count = glonass ? 8 : 2;
        var stream = PackNativeBits(frame, glonass ? 3 : 4, count);
        Assert.Equal(glonass ? 85 : 75, stream.Length);
        var protocol = CreateProtocol(glonass);
        using var port = new VirtualPort(
            "navigation-contiguous",
            (IProtocolContext)protocol,
            static _ => true,
            new Statistic()
        );
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var rx = port.OnRxMessage.Subscribe(messages.Add);
        using var rxError = port.OnRxError.Subscribe(errors.Add);
        using var parserError = port
            .Parsers.Single()
            .OnError.Subscribe(errors.Add, errors.Add, _ => { });

        Receive(port, stream, chunkSize);

        Assert.Empty(errors);
        Assert.Equal(count, messages.Count);
        Assert.All(messages, message => AssertTypedFrame(message, frame, glonass));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Deserialize_SbfReceiverWords_RetainsPermissiveLegacyNavigationDecoding(bool glonass)
    {
        // These independently framed SBF blocks report failed parity. Receiver views must still
        // decode them; strict native-bit validation belongs to the new dedicated protocols.
        var frame = Convert.FromHexString(
            glonass
                ? "2440DE64BA0F200040E20100FC0829000008080CB5C40108203210010008F535"
                : "24408524B10F3C0040E20100FC0804000000080C3C59C02268EA6A22C1013F390E7AE68C3AFA408A2D8022BF0F0FEEA05228C803666C3283339FED99"
        );
        SbfPacketGnssRawNavMsgBase message = glonass
            ? new SbfPacketGloRawCa()
            : new SbfPacketGpsRawCa();
        ReadOnlySpan<byte> read = frame;

        message.Deserialize(ref read);

        Assert.True(read.IsEmpty);
        Assert.False(message.CrcPassed);
        Assert.Equal(123456U, message.TOW);
        Assert.Equal(2300, message.WNc);
        if (glonass)
        {
            Assert.Equal(new uint[] { 0x0801C4B5, 0x01103220, 0x35F50800 }, message.NAVBitsU32);
            var decoded = Assert.IsType<GlonassWord1>(
                Assert.IsType<SbfPacketGloRawCa>(message).GlonassWord
            );
            Assert.Equal(Convert.FromHexString("01003896A022064406BEA1"), decoded.RawData);
        }
        else
        {
            Assert.Equal(
                new uint[]
                {
                    0x22C0593C,
                    0x226AEA68,
                    0x393F01C1,
                    0x8CE67A0E,
                    0x8A40FA3A,
                    0xBF22802D,
                    0xA0EE0F0F,
                    0x03C82852,
                    0x83326C66,
                    0x99ED9F33,
                },
                message.NAVBitsU32
            );
            var decoded = Assert.IsType<GpsSubframe2>(
                Assert.IsType<SbfPacketGpsRawCa>(message).GpsSubFrame
            );
            Assert.Equal(70487U, decoded.TOW1_5Epoh);
        }
    }

    private static void AssertCatalog<T>(
        IProtocolMessageFactory<T, byte> factory,
        int byteSize,
        params Type[] types
    )
        where T : IProtocolMessage<byte>
    {
        Assert.Equal(
            Enumerable.Range(1, types.Length).Select(x => (byte)x),
            factory.GetSupportedIds().Order()
        );
        Assert.Null(factory.Create(0));
        Assert.Null(factory.Create(checked((byte)(types.Length + 1))));
        for (byte id = 1; id <= types.Length; id++)
        {
            var message = factory.Create(id);
            Assert.NotNull(message);
            Assert.Equal(types[id - 1], message.GetType());
            Assert.Equal(id, message.Id);
            Assert.Equal(id.ToString(CultureInfo.InvariantCulture), message.GetIdAsString());
            Assert.Equal(factory.Info, message.Protocol);
            Assert.False(string.IsNullOrWhiteSpace(message.Name));
            Assert.Equal(byteSize, message.GetByteSize());
            Assert.IsAssignableFrom<IVisitable>(message);
        }
    }

    private static byte[] Serialize(IProtocolMessage message)
    {
        var bytes = new byte[message.GetByteSize()];
        var buffer = bytes.AsSpan();
        message.Serialize(ref buffer);
        Assert.True(buffer.IsEmpty);
        return bytes;
    }

    private static IProtocolFactory CreateProtocol(bool glonass) =>
        Protocol.Create(builder =>
        {
            if (glonass)
                builder.Protocols.RegisterRawGlonassProtocol();
            else
                builder.Protocols.RegisterRawGpsProtocol();
        });

    private static void AssertTypedFrame(IProtocolMessage message, byte[] frame, bool glonass)
    {
        if (glonass)
        {
            Assert.Equal(1, Assert.IsType<GlonassWord1>(message).WordId);
        }
        else
        {
            var subframe = Assert.IsType<GpsSubframe2>(message);
            Assert.Equal(70487U, subframe.TOW1_5Epoh);
            Assert.Equal(0, subframe.PreviousWordParity);
        }
        Assert.Equal(frame, Serialize(message));
    }

    private static byte[] PackNativeBits(byte[] frame, int padding, int copies)
    {
        var frameBits = frame.Length * 8 - padding;
        var bytes = new byte[(frameBits * copies + 7) / 8];
        var destination = 0;
        for (var copy = 0; copy < copies; copy++)
        {
            for (var source = padding; source < frame.Length * 8; source++, destination++)
            {
                var bit = (frame[source / 8] >> (7 - source % 8)) & 1;
                bytes[destination / 8] |= (byte)(bit << (7 - destination % 8));
            }
        }
        return bytes;
    }

    private static void Receive(VirtualPort port, byte[] stream, int chunkSize)
    {
        for (var offset = 0; offset < stream.Length; offset += chunkSize)
        {
            port.Rx.OnNext(
                stream.AsSpan(offset, Math.Min(chunkSize, stream.Length - offset)).ToArray()
            );
        }
    }
}
