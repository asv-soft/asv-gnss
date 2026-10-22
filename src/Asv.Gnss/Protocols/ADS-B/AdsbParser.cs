using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>Bounded streaming parser for AVR and escaped Beast Mode S frames.</summary>
public class AdsbParser : ProtocolParser<AdsbDfMessage, ushort>
{
    private enum State { Idle, Avr, BeastType, BeastData }
    private readonly byte[] _buffer = new byte[AdsbFrameCodec.MaximumFrameSize];
    private State _state;
    private int _length;
    private int _remaining;
    private bool _escaped;

    public AdsbParser(IProtocolMessageFactory<AdsbDfMessage, ushort> messageFactory, IProtocolContext context, IStatisticHandler? statisticHandler)
        : base(messageFactory, context, statisticHandler) { }

    public override ProtocolInfo Info => AdsbProtocol.Info;

    public override bool Push(byte data)
    {
        if (_state == State.Idle) { Start(data); return false; }
        if (_state == State.BeastType)
        {
            _remaining = data switch { 0x31 or 0x34 => 9, 0x32 => 14, 0x33 => 21, _ => 0 };
            if (_remaining == 0) { Reset(); Start(data); return false; }
            _buffer[_length++] = data;
            _state = State.BeastData;
            return false;
        }
        if (_length >= _buffer.Length)
        {
            Fail("ADS-B receiver frame exceeds the maximum length.");
            Start(data);
            return false;
        }
        if (_state == State.Avr)
        {
            if (AdsbFrameCodec.IsAvrStart(data) || data == AdsbFrameCodec.Escape)
            {
                Fail("Interrupted AVR frame.");
                Start(data);
                return false;
            }
            _buffer[_length++] = data;
            if (data == ';') return Complete();
            if (AdsbFrameCodec.Hex(data) < 0) Fail("Invalid AVR hexadecimal digit.");
            return false;
        }
        if (_escaped)
        {
            if (data != AdsbFrameCodec.Escape)
            {
                Fail("Interrupted Beast frame.");
                Start(AdsbFrameCodec.Escape);
                return Push(data);
            }
            _escaped = false;
            _remaining--;
        }
        else if (data == AdsbFrameCodec.Escape) _escaped = true;
        else _remaining--;
        _buffer[_length++] = data;
        return _remaining == 0 ? Complete() : false;
    }

    private bool Complete()
    {
        var length = _length;
        var state = _state;
        Reset();
        // Mode A/C and receiver status are framed and skipped, never mistaken for Mode S.
        if (state == State.BeastData && _buffer[1] is 0x31 or 0x34) return false;
        if (state == State.Avr)
        {
            var prefix = _buffer[0] switch { (byte)'@' or (byte)'%' => 13, (byte)'<' => 15, _ => 1 };
            if (length - prefix - 1 == 4) return false;
        }
        try
        {
            Span<byte> raw = stackalloc byte[14];
            var header = AdsbFrameCodec.Read(_buffer.AsSpan(0, length), raw);
            var payload = raw[..header.RawSize];
            var valid = AdsbDfMessage.ValidLength(payload) && AdsbDfMessage.ValidParity(payload);
            ReadOnlySpan<byte> frame = _buffer.AsSpan(0, length);
            InternalParsePacket((ushort)(payload[0] >> 3), ref frame);
            return valid;
        }
        catch (Exception error) when (error is FormatException or ArgumentException)
        {
            InternalOnError(new ProtocolParserException(Info, "Invalid ADS-B receiver frame.", error));
            return false;
        }
    }

    private void Start(byte data)
    {
        if (AdsbFrameCodec.IsAvrStart(data)) _state = State.Avr;
        else if (data == AdsbFrameCodec.Escape) _state = State.BeastType;
        else return;
        _buffer[0] = data;
        _length = 1;
    }

    private void Fail(string message)
    {
        Reset();
        InternalOnError(new ProtocolParserException(Info, message));
    }

    public override void Reset()
    {
        _state = State.Idle;
        _length = 0;
        _remaining = 0;
        _escaped = false;
    }
}
