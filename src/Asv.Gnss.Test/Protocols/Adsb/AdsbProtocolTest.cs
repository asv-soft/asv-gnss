using System;
using System.Collections.Generic;
using System.Text;
using Asv.IO;
using R3;
using Xunit;

namespace Asv.Gnss;

public class AdsbProtocolTest
{
    // Published identification example: https://mode-s.org/1090mhz/content/ads-b/2-identification.html
    private const string Identification = "8D4840D6202CC371C32CE0576098";
    // Published short frame: https://wiki.jetvision.de/wiki/Mode-S_Beast:Data_Output_Formats
    private const string ShortFrame = "02E99619FACDAE";

    [Theory]
    [InlineData('*', "")]
    [InlineData('@', "0123456789AB")]
    [InlineData('%', "0123456789AB")]
    [InlineData('<', "0123456789AB7F")]
    public void Push_AvrFormats_PreservesRawFrameAndReceiverMetadata(char marker, string prefix)
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var frame = Encoding.ASCII.GetBytes($"{marker}{prefix}{Identification};");
        Assert.Equal(1, Push(parser, frame));
        var message = Assert.IsType<AdsbDfMessage>(Assert.Single(messages));
        Assert.Equal(Convert.FromHexString(Identification), message.RawData.ToArray());
        Assert.Equal((ushort)17, message.Id);
        Assert.Equal((byte)17, message.DownlinkFormat);
        Assert.Equal(0x4840D6u, message.IcaoAddress);
        Assert.Equal(AdsbFrameFormat.Avr, message.FrameFormat);
        Assert.Equal(marker, message.AvrMarker);
        Assert.Equal(marker == '*' ? null : (ulong?)0x0123456789AB, message.ReceiverTimestamp);
        Assert.Equal(marker == '<' ? (byte?)0x7F : null, message.SignalLevel);
        Assert.Equal(AdsbParityStatus.Valid, message.ParityStatus);
        Assert.Equal(frame, Serialize(message));
    }

    [Theory]
    [InlineData(ShortFrame, 0x32)]
    [InlineData(Identification, 0x33)]
    public void Push_BeastFormats_PreservesEscapedTimestampSignalAndPayload(string hex, byte type)
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var bytes = Beast(type, Convert.FromHexString(hex), [0x1A, 2, 0x1A, 4, 5, 6], 0x1A);
        Assert.Equal(1, Push(parser, bytes));
        var message = Assert.IsType<AdsbDfMessage>(Assert.Single(messages));
        Assert.Equal(Convert.FromHexString(hex), message.RawData.ToArray());
        Assert.Equal(AdsbFrameFormat.Beast, message.FrameFormat);
        Assert.Equal(0x1A021A040506UL, message.ReceiverTimestamp);
        Assert.Equal((byte)0x1A, message.SignalLevel);
        Assert.Equal(bytes, Serialize(message));
    }

    [Fact]
    public void Push_BeastEscapedPayload_PreservesLiteralEscapeByte()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        byte[] raw = [0x20, 0x1A, 2, 3, 4, 5, 6];
        var frame = Beast(0x32, raw, [0, 0, 0, 0, 0, 1], 2);
        Assert.Equal(1, Push(parser, frame));
        var message = Assert.IsType<AdsbDfMessage>(Assert.Single(messages));
        Assert.Equal(raw, message.RawData.ToArray());
        Assert.Equal(AdsbParityStatus.AddressParityUnverified, message.ParityStatus);
        Assert.Equal(frame, Serialize(message));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Push_EveryPossibleChunkBoundary_ParsesOneCompleteFrame(bool beast)
    {
        var bytes = beast
            ? Beast(0x33, Convert.FromHexString(Identification), [0x1A, 2, 3, 4, 5, 6], 0x1A)
            : Encoding.ASCII.GetBytes($"@0123456789AB{Identification};");
        for (var split = 1; split < bytes.Length; split++)
        {
            using var parser = CreateParser();
            var messages = new List<IProtocolMessage>();
            using var subscription = parser.OnMessage.Subscribe(messages.Add);
            Assert.Equal(0, Push(parser, bytes.AsSpan(0, split)));
            Assert.Empty(messages);
            Assert.Equal(1, Push(parser, bytes.AsSpan(split)));
            Assert.Equal(Convert.FromHexString(Identification), Assert.IsType<AdsbDfMessage>(Assert.Single(messages)).RawData.ToArray());
        }
    }

    [Fact]
    public void Push_MixedConcatenatedFormatsAndNoise_PublishesOnlyModeSFramesInOrder()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        byte[] bytes = [0, 0xFF, .. Encoding.ASCII.GetBytes("noise*7700;"),
            .. Beast(0x31, [0x77, 0], [0, 0, 0, 0, 0, 1], 2),
            .. Encoding.ASCII.GetBytes($"*{ShortFrame};\r\n"),
            .. Beast(0x33, Convert.FromHexString(Identification), [0, 0, 0, 0, 0, 2], 3)];
        Assert.Equal(2, Push(parser, bytes));
        Assert.Equal(2, messages.Count);
        Assert.Equal(Convert.FromHexString(ShortFrame), Assert.IsType<AdsbDfMessage>(messages[0]).RawData.ToArray());
        Assert.Equal(Convert.FromHexString(Identification), Assert.IsType<AdsbDfMessage>(messages[1]).RawData.ToArray());
    }

    [Theory]
    [InlineData("*8D4840D6202CC371C32CE057609Z;")]
    [InlineData("*8D4840D6202CC371C32CE057609;")]
    [InlineData("*8D4840D6202CC3;")]
    [InlineData("*8D4840D6202CC371C32CE057609800;")]
    public void Push_MalformedAvr_RecoversAtFollowingFrame(string malformed)
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        Push(parser, Encoding.ASCII.GetBytes(malformed + $"*{Identification};"));
        Assert.Equal(Convert.FromHexString(Identification), Assert.IsType<AdsbDfMessage>(Assert.Single(messages)).RawData.ToArray());
    }

    [Fact]
    public void Push_OverlongAvrAndUnknownBeastType_RecoversWithoutPublishingGarbage()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        Push(parser, Encoding.ASCII.GetBytes("*" + new string('A', 10000) + ";"));
        Push(parser, [0x1A, 0x7F, 1, 2, 3]);
        Assert.Empty(messages);
        Assert.Equal(1, Push(parser, Encoding.ASCII.GetBytes($"*{Identification};")));
        Assert.Single(messages);
    }

    [Fact]
    public void Push_InterruptedBeastFrame_ResynchronizesAtNextFramePrefix()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        Push(parser, [0x1A, 0x33, 1, 2, 3]);
        var next = Beast(0x33, Convert.FromHexString(Identification), [0, 0, 0, 0, 0, 1], 2);
        Assert.Equal(1, Push(parser, next));
        Assert.Equal(Convert.FromHexString(Identification), Assert.IsType<AdsbDfMessage>(Assert.Single(messages)).RawData.ToArray());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Reset_TruncatedFrame_ClearsFramingState(bool beast)
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var frame = beast ? Beast(0x33, Convert.FromHexString(Identification), [0, 0, 0, 0, 0, 1], 2)
            : Encoding.ASCII.GetBytes($"*{Identification};");
        Assert.Equal(0, Push(parser, frame.AsSpan(0, 5)));
        parser.Reset();
        Assert.Equal(1, Push(parser, frame));
        Assert.Single(messages);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Push_CorruptExtendedSquitter_ReportsErrorAndPreservesFollowingValidFrame(bool beast)
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        var corrupt = Convert.FromHexString(Identification);
        corrupt[5] ^= 1;
        var invalid = beast ? Beast(0x33, corrupt, [0, 0, 0, 0, 0, 1], 2)
            : Encoding.ASCII.GetBytes($"*{Convert.ToHexString(corrupt)};");
        Assert.Equal(0, Push(parser, invalid));
        Assert.Empty(messages);
        Assert.NotEmpty(errors);
        Assert.Equal(1, Push(parser, Encoding.ASCII.GetBytes($"*{Identification};")));
        Assert.Single(messages);
    }

    [Fact]
    public void Register_RemovedDownlinkFormat_IsNotRestoredByFallback()
    {
        using var parser = Protocol.Create(builder => builder.Protocols.RegisterAdsbProtocol(factory =>
            Assert.True(factory.Remove(17)))).CreateParser(AdsbProtocol.Info.Id);
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        Push(parser, Encoding.ASCII.GetBytes($"*{Identification};*{ShortFrame};"));
        Assert.Equal((ushort)0, Assert.IsType<AdsbDfMessage>(Assert.Single(messages)).Id);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void Push_AddressParityReply_DoesNotInventAnIcaoAddressOrRejectUnverifiedParity()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        Assert.Equal(1, Push(parser, Encoding.ASCII.GetBytes("*20010203040506;")));
        var message = Assert.IsType<AdsbDfMessage>(Assert.Single(messages));
        Assert.Equal((ushort)4, message.Id);
        Assert.Null(message.IcaoAddress);
        Assert.Equal(AdsbParityStatus.AddressParityUnverified, message.ParityStatus);
    }


    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Deserialize_FramedMessage_ConsumesExactlyOneFrameAndRoundTrips(bool beast)
    {
        var frame = beast ? Beast(0x33, Convert.FromHexString(Identification), [1, 2, 3, 4, 5, 6], 7)
            : Encoding.ASCII.GetBytes($"@010203040506{Identification};");
        ReadOnlySpan<byte> input = (byte[])[.. frame, 0x55, 0x66];
        var message = new AdsbDfMessage();
        message.Deserialize(ref input);
        Assert.Equal(new byte[] { 0x55, 0x66 }, input.ToArray());
        Assert.Equal(frame, Serialize(message));
    }

    [Theory]
    [InlineData(0u)]
    [InlineData(42u)]
    [InlineData(127u)]
    public void Push_AllCallInterrogatorParity_AcceptsSevenBitOverlay(uint overlay)
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        byte[] raw = [0x5D, 0x48, 0x40, 0xD6, 0, 0, 0];
        SetParity(raw, overlay);
        Assert.Equal(1, Push(parser, Encoding.ASCII.GetBytes($"*{Convert.ToHexString(raw)};")));
        var message = Assert.IsType<AdsbDfMessage>(Assert.Single(messages));
        Assert.Equal(0x4840D6u, message.IcaoAddress);
        Assert.Equal(overlay, message.ParitySyndrome);
        Assert.Equal(AdsbParityStatus.InterrogatorParity, message.ParityStatus);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, false)]
    [InlineData(6, false)]
    public void Push_Df18AddressType_DoesNotLabelNonIcaoIdentityAsIcao(byte control, bool isIcao)
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        var raw = Convert.FromHexString(Identification);
        raw[0] = (byte)(18 << 3 | control);
        SetParity(raw, 0);
        Assert.Equal(1, Push(parser, Encoding.ASCII.GetBytes($"*{Convert.ToHexString(raw)};")));
        var message = Assert.IsType<AdsbDfMessage>(Assert.Single(messages));
        Assert.Equal(isIcao ? 0x4840D6u : (uint?)null, message.IcaoAddress);
        Assert.Equal(AdsbParityStatus.Valid, message.ParityStatus);
    }

    [Fact]
    public void Push_LowercaseAvr_ProducesCanonicalUppercaseSerialization()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        Assert.Equal(1, Push(parser, Encoding.ASCII.GetBytes($"*{Identification.ToLowerInvariant()};")));
        Assert.Equal(Encoding.ASCII.GetBytes($"*{Identification};"),
            Serialize(Assert.IsType<AdsbDfMessage>(Assert.Single(messages))));
    }

    [Fact]
    public void Register_ClearedFactory_PublishesNoFramesAndReportsUnknownFormats()
    {
        using var parser = Protocol.Create(builder => builder.Protocols.RegisterAdsbProtocol(factory =>
            factory.Clear())).CreateParser(AdsbProtocol.Info.Id);
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        Push(parser, Encoding.ASCII.GetBytes($"*{Identification};*{ShortFrame};"));
        Assert.Empty(messages);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void Push_AllCallInvalidOverlay_RejectsRemainderOutsideSevenBits()
    {
        using var parser = CreateParser();
        var messages = new List<IProtocolMessage>();
        var errors = new List<Exception>();
        using var subscription = parser.OnMessage.Subscribe(messages.Add);
        using var errorSubscription = parser.OnError.Subscribe(errors.Add, errors.Add, _ => { });
        byte[] raw = [0x5D, 0x48, 0x40, 0xD6, 0, 0, 0];
        SetParity(raw, 128);
        Push(parser, Encoding.ASCII.GetBytes($"*{Convert.ToHexString(raw)};"));
        Assert.Empty(messages);
        Assert.NotEmpty(errors);
    }

    private static void SetParity(byte[] raw, uint overlay)
    {
        // Independent bitwise polynomial division, with the parity bytes cleared.
        raw[^3] = raw[^2] = raw[^1] = 0;
        uint remainder = 0;
        foreach (var value in raw)
        {
            for (var bit = 7; bit >= 0; bit--)
            {
                remainder = (remainder << 1) | (uint)((value >> bit) & 1);
                if ((remainder & 0x1000000) != 0) remainder ^= 0x1FFF409;
            }
        }
        remainder ^= overlay;
        raw[^3] = (byte)(remainder >> 16);
        raw[^2] = (byte)(remainder >> 8);
        raw[^1] = (byte)remainder;
    }

    private static IProtocolParser CreateParser() => Protocol.Create(builder =>
        builder.Protocols.RegisterAdsbProtocol()).CreateParser(AdsbProtocol.Info.Id);

    private static int Push(IProtocolParser parser, ReadOnlySpan<byte> bytes)
    {
        var count = 0;
        foreach (var value in bytes) if (parser.Push(value)) count++;
        return count;
    }

    private static byte[] Serialize(AdsbDfMessage message)
    {
        var bytes = new byte[message.GetByteSize()];
        var span = bytes.AsSpan();
        message.Serialize(ref span);
        Assert.Empty(span.ToArray());
        return bytes;
    }

    private static byte[] Beast(byte type, byte[] raw, byte[] timestamp, byte signal)
    {
        var result = new List<byte> { 0x1A, type };
        foreach (var value in (byte[])[.. timestamp, signal, .. raw])
        {
            result.Add(value);
            if (value == 0x1A) result.Add(value);
        }
        return result.ToArray();
    }
}
