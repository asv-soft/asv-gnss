using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class JavadFramingTest
{
    // Independently calculated from the Appendix A.1 algorithm, not from the library encoder.
    private const string LittleTime = "7E7E30303504030201BC";
    private const string BigTime = "7E7E30303501020304FD";
    private const string LittleFormat = "4D463030394A5030313030303846";
    private const string BigFormat = "4D463030394A5030313030313842";

    [Fact]
    public void Checksum_UsesFinalRotationAndCrc16CcittPolynomial()
    {
        Assert.Equal(0x31, JavadChecksum.Compute8("123456789"u8));
        Assert.Equal(0x31C3, JavadChecksum.Compute16("123456789"u8));
        Assert.Equal(0, JavadChecksum.Compute8([]));
        Assert.Equal(0, JavadChecksum.Compute16([]));
    }

    [Theory]
    [InlineData("TD036,CGGTTS,{FY=2005,REF=UTC(USNO),CH=816 (UNIVERSAL)},@7C", "FY", "2005")]
    [InlineData("TD034,CGGTTS,{LL=TL,RR=09,LAB=Test CGGTTS Laboratory},@E6", "RR", "09")]
    [InlineData("TD026,CGGTTS,{COMMENTS=Example Comment},@80", "COMMENTS", "Example Comment")]
    public void Push_PdfPage393Vectors_DecodesAndPreservesExactBytes(
        string source,
        string key,
        string value
    )
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);

        Assert.Equal(1, Push(parser, Encoding.ASCII.GetBytes(source)));
        var message = Assert.IsType<JavadTextDataMessage>(Assert.Single(messages));
        Assert.Equal(value, message.CggttsHeaders[key]);
        Assert.Equal(Encoding.ASCII.GetBytes(source), Serialize(message));
        Assert.Equal(1U, parser.Statistic.ParsedMessages);
        Assert.Equal((uint)source.Length, parser.Statistic.ParsedBytes);
    }

    [Fact]
    public void Push_FragmentedConcatenatedAndMixedText_PublishesOnlyCompleteGreisFrames()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var time = Convert.FromHexString(LittleTime);
        Assert.Equal(0, Push(parser, time.AsSpan(0, 6)));
        Assert.Empty(messages);
        Assert.Equal(1, Push(parser, time.AsSpan(6)));
        Assert.Equal(1, Push(parser, "||001F"u8));
        Assert.Equal(0, Push(parser, "$GPGGA,TD026,CGGTTS,{COMMENTS=Example Comment},@80\r\n"u8));
        Assert.Equal(1, Push(parser, time));

        Assert.Equal(new[] { "~~", "||", "~~" }, messages.Select(x => x.GetIdAsString()));
        Assert.Equal(0x01020304U, Assert.IsType<JavadReceiverTimeMessage>(messages[0]).TimeOfDay);
    }

    [Fact]
    public void Push_LineEndingAfterNoise_RestoresNonstandardTextBoundary()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);

        Push(parser, "\0\r\n!TD026,CGGTTS,{COMMENTS=Example Comment},@80\r\n"u8);
        Assert.Empty(messages);
        Push(parser, Convert.FromHexString(LittleTime));
        Assert.IsType<JavadReceiverTimeMessage>(Assert.Single(messages));
    }

    [Fact]
    public void Push_FormatByteOrderIsLocalToParserAndResetStreamRestoresDefaults()
    {
        var protocol = Protocol.Create(builder => builder.Protocols.RegisterJavadProtocol());
        using var first = (JavadMessageParser)protocol.CreateParser(JavadProtocol.Info.Id);
        using var second = (JavadMessageParser)protocol.CreateParser(JavadProtocol.Info.Id);
        var firstMessages = new List<IProtocolMessage>();
        var secondMessages = new List<IProtocolMessage>();
        using var firstSubscription = first.OnMessage.Subscribe(firstMessages.Add);
        using var secondSubscription = second.OnMessage.Subscribe(secondMessages.Add);
        Assert.Equal(1, Push(first, Convert.FromHexString(BigFormat)));
        Assert.Equal(JavadByteOrder.BigEndian, first.ByteOrder);
        Assert.Equal(JavadByteOrder.LittleEndian, second.ByteOrder);
        Push(first, Convert.FromHexString(BigTime));
        Push(second, Convert.FromHexString(LittleTime));
        Assert.Equal(
            0x01020304U,
            Assert.IsType<JavadReceiverTimeMessage>(firstMessages[1]).TimeOfDay
        );
        Assert.Equal(
            0x01020304U,
            Assert.IsType<JavadReceiverTimeMessage>(Assert.Single(secondMessages)).TimeOfDay
        );

        first.Reset();
        Assert.Equal(JavadByteOrder.BigEndian, first.ByteOrder);
        first.ResetStream();
        Assert.Equal(JavadByteOrder.LittleEndian, first.ByteOrder);
        Push(first, Convert.FromHexString(LittleTime));
        Assert.Equal(
            0x01020304U,
            Assert.IsType<JavadReceiverTimeMessage>(firstMessages[2]).TimeOfDay
        );
    }

    [Fact]
    public void Push_UnsupportedMajorFormat_DoesNotMisinterpretLaterMeasurements()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(_ => { }, errors.Add, _ => { });
        Push(parser, Convert.FromHexString("4D463030394A5030323030313838"));
        Assert.Single(errors);
        Assert.Equal(0, Push(parser, Convert.FromHexString(BigTime)));
        Assert.Single(messages);
        Push(parser, Convert.FromHexString(LittleFormat));
        Assert.Equal(1, Push(parser, Convert.FromHexString(LittleTime)));
        Assert.Equal(0x01020304U, Assert.IsType<JavadReceiverTimeMessage>(messages[^1]).TimeOfDay);
    }

    [Fact]
    public void Push_CorruptFrameContainingValidFrame_RecoversAtInnerBoundary()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(_ => { }, errors.Add, _ => { });
        byte[] corrupt = [.. "~~00E"u8, 0, 0, 0, 0, .. Convert.FromHexString(LittleTime)];

        Assert.Equal(1, Push(parser, corrupt));
        Assert.IsType<ProtocolParserCrcException>(Assert.Single(errors));
        Assert.Equal(1U, parser.Statistic.BadCrcError);
        Assert.Equal(
            0x01020304U,
            Assert.IsType<JavadReceiverTimeMessage>(Assert.Single(messages)).TimeOfDay
        );
    }

    [Fact]
    public void Push_UnknownMessage_IsSkippedByDeclaredLengthWithoutParsingItsContents()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(_ => { }, errors.Add, _ => { });
        byte[] unknown = [.. "zz00A"u8, .. Convert.FromHexString(LittleTime)];

        Assert.Equal(0, Push(parser, unknown));
        Assert.Empty(messages);
        Assert.IsType<ProtocolParserUnknownMessageException>(Assert.Single(errors));
        Assert.Equal(1, Push(parser, Convert.FromHexString(LittleTime)));
        Assert.Single(messages);
        Assert.Equal(1U, parser.Statistic.UnknownMessages);
    }

    [Fact]
    public void Push_UnknownFrameAfterNoise_RestoresTextBoundaries()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(_ => { }, errors.Add, _ => { });
        Push(parser, [0]);
        Assert.Equal(0, Push(parser, "zz000"u8));
        Assert.Equal(0, Push(parser, "$GPGGA,TD026,CGGTTS,{COMMENTS=Example Comment},@80\r\n"u8));
        Assert.Empty(messages);
        Assert.Equal(1, Push(parser, Convert.FromHexString(LittleTime)));
        Assert.IsType<JavadReceiverTimeMessage>(Assert.Single(messages));
        Assert.IsType<ProtocolParserUnknownMessageException>(Assert.Single(errors));
    }

    [Fact]
    public void Push_OverlappingHeaderAndInvalidHex_RecoversWithoutAnExtraSeparator()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        Assert.Equal(0, Push(parser, "zz00gA"u8));
        Assert.Equal(1, Push(parser, Convert.FromHexString(LittleTime)));
        Assert.IsType<JavadReceiverTimeMessage>(Assert.Single(messages));
    }

    [Fact]
    public void Push_BadFormatChecksum_DoesNotChangeStreamByteOrder()
    {
        using var parser = CreateParser();
        var errors = new List<Exception>();
        using var errorSubscription = parser.OnError.Subscribe(_ => { }, errors.Add, _ => { });
        var format = Convert.FromHexString(BigFormat);
        format[^1] = (byte)'0';
        Push(parser, format);
        Assert.Equal(JavadByteOrder.LittleEndian, parser.ByteOrder);
        Assert.Contains(errors, error => error is ProtocolParserCrcException);
        parser.Reset();
        Assert.Equal(1, Push(parser, Convert.FromHexString(LittleTime)));
    }

    [Fact]
    public void Deserialize_FixedPayloadExtension_PreservesTailAndRemainingInput()
    {
        var source = Convert.FromHexString("7E7E30303804030201AABBCC00");
        byte[] data = [.. source, 0xFE, 0xFF];
        ReadOnlySpan<byte> input = data;
        var message = new JavadReceiverTimeMessage();

        message.Deserialize(ref input);

        Assert.Equal(0x01020304U, message.TimeOfDay);
        Assert.Equal(new byte[] { 0xAA, 0xBB, 0xCC }, message.ExtensionData);
        Assert.Equal(new byte[] { 0xFE, 0xFF }, input.ToArray());
        Assert.Equal(source, Serialize(message));
        input = Convert.FromHexString(LittleTime);
        message.Deserialize(ref input);
        Assert.Empty(message.ExtensionData);
    }

    [Theory]
    [InlineData(JavadChecksumKind.None)]
    [InlineData(JavadChecksumKind.Binary8)]
    [InlineData(JavadChecksumKind.AsciiHex8)]
    [InlineData(JavadChecksumKind.Crc16)]
    public void RawFrame_AllChecksumKindsAndMaximumLength_RoundTripWithBoundedBuffer(
        JavadChecksumKind kind
    )
    {
        var checksumSize =
            kind == JavadChecksumKind.None ? 0
            : kind == JavadChecksumKind.Binary8 ? 1
            : 2;
        var payload = Enumerable
            .Range(0, JavadProtocol.MaxBodySize - checksumSize)
            .Select(x => (byte)x)
            .ToArray();
        var message = new JavadRawMessage("zz", kind)
        {
            Payload = payload,
            ByteOrder = JavadByteOrder.BigEndian,
        };
        var bytes = Serialize(message);
        Assert.Equal(JavadProtocol.MaxPacketSize, bytes.Length);
        Assert.Equal("zzFFF", Encoding.ASCII.GetString(bytes.AsSpan(0, 5)));
        var copy = new JavadRawMessage("zz", kind) { ByteOrder = JavadByteOrder.BigEndian };
        ReadOnlySpan<byte> input = bytes;
        copy.Deserialize(ref input);
        Assert.True(input.IsEmpty);
        Assert.Equal(payload, copy.Payload);
        if (kind != JavadChecksumKind.None)
        {
            bytes[5] ^= 1;
            Assert.Throws<ProtocolParserCrcException>(() => Deserialize(copy, bytes));
        }
        message.Payload = new byte[JavadProtocol.MaxBodySize + 1];
        Assert.Throws<ArgumentOutOfRangeException>(() => message.GetByteSize());
    }

    [Fact]
    public void Serialize_ShortDestination_DoesNotWritePartialHeader()
    {
        var message = new JavadReceiverTimeMessage { TimeOfDay = 0x01020304 };
        var bytes = Enumerable.Repeat((byte)0xEE, 9).ToArray();
        Assert.Throws<ArgumentException>(() => SerializeInto(message, bytes));
        Assert.All(bytes, value => Assert.Equal(0xEE, value));
    }

    [Fact]
    public void Deserialize_BadChecksumOrTruncatedFrame_DoesNotConsumeInput()
    {
        var bytes = Convert.FromHexString(LittleTime);
        bytes[^1] ^= 1;
        var message = new JavadReceiverTimeMessage();
        Assert.Throws<ProtocolParserCrcException>(() => Deserialize(message, bytes));
        Assert.Throws<FormatException>(() => Deserialize(message, bytes[..6]));
    }

    [Fact]
    public void Registration_CustomConfigurationRemovesDefaultMessage()
    {
        using var parser = Protocol
            .Create(builder =>
                builder.Protocols.RegisterJavadProtocol(factory => factory.Remove("~~"))
            )
            .CreateParser(JavadProtocol.Info.Id);
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(_ => { }, errors.Add, _ => { });
        Assert.Equal(0, Push(parser, Convert.FromHexString(LittleTime)));
        Assert.Empty(messages);
        Assert.IsType<ProtocolParserUnknownMessageException>(Assert.Single(errors));
    }

    private static JavadMessageParser CreateParser() =>
        (JavadMessageParser)
            Protocol
                .Create(builder => builder.Protocols.RegisterJavadProtocol())
                .CreateParser(JavadProtocol.Info.Id);

    private static int Push(IProtocolParser parser, ReadOnlySpan<byte> data)
    {
        var count = 0;
        foreach (var value in data)
        {
            if (parser.Push(value))
                count++;
        }
        return count;
    }

    private static byte[] Serialize(IProtocolMessage message)
    {
        var bytes = new byte[message.GetByteSize()];
        var output = bytes.AsSpan();
        message.Serialize(ref output);
        Assert.True(output.IsEmpty);
        return bytes;
    }

    private static void SerializeInto(IProtocolMessage message, byte[] bytes)
    {
        var output = bytes.AsSpan();
        message.Serialize(ref output);
    }

    private static void Deserialize(IProtocolMessage message, byte[] bytes)
    {
        ReadOnlySpan<byte> input = bytes;
        try
        {
            message.Deserialize(ref input);
        }
        catch
        {
            Assert.Equal(bytes.Length, input.Length);
            throw;
        }
    }
}
