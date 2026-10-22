using System;
using System.Threading.Tasks;
using Asv.Common;
using Asv.IO;
using R3;

namespace Asv.Gnss;

/// <summary>Parses a standard GREIS stream; byte-order state belongs to this parser instance.</summary>
public sealed class JavadMessageParser : AsyncDisposableOnce, IProtocolParser
{
    private readonly IProtocolMessageFactory<JavadMessageBase, string> _factory;
    private readonly IStatisticHandler _statistic;
    private readonly Subject<IProtocolMessage> _messages = new();
    private readonly Subject<Exception> _errors = new();
    private readonly byte[] _buffer = new byte[JavadProtocol.MaxPacketSize];
    private int _start;
    private int _end;
    private bool _skippingText;
    private bool _recovering;

    public JavadMessageParser(
        IProtocolMessageFactory<JavadMessageBase, string> factory,
        IProtocolContext context,
        IStatisticHandler? statisticHandler = null
    )
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(context);
        _factory = factory;
        _statistic =
            statisticHandler == null ? new Statistic() : new InheritedStatistic(statisticHandler);
    }

    public IStatistic Statistic => _statistic;
    public ProtocolInfo Info => JavadProtocol.Info;
    public ProtocolTags Tags { get; } = [];
    public Observable<IProtocolMessage> OnMessage => _messages;
    public Observable<Exception> OnError => _errors;
    public JavadByteOrder ByteOrder { get; private set; }
    public int FormatMajorVersion { get; private set; } = 1;
    public int FormatMinorVersion { get; private set; }

    public bool Push(byte data)
    {
        ObjectDisposedException.ThrowIf(IsDisposed, this);
        if (_skippingText)
        {
            if (data is 13 or 10)
            {
                _skippingText = false;
            }
            else if (data > 127)
            {
                // Non-standard text is ASCII. A binary byte ends this damaged candidate.
                _skippingText = false;
                _recovering = true;
            }
            else
            {
                return false;
            }
        }
        if (_end == _buffer.Length)
        {
            _buffer.AsSpan(_start, _end - _start).CopyTo(_buffer);
            _end -= _start;
            _start = 0;
        }
        _buffer[_end++] = data;
        var published = false;
        while (_start < _end)
        {
            var available = _buffer.AsSpan(_start, _end - _start);
            if (available[0] is 13 or 10)
            {
                Consume(1);
                _recovering = false;
                continue;
            }
            if (!_recovering && available[0] is >= 0x21 and <= 0x2F)
            {
                _skippingText = true;
                Consume(1);
                // Normally this branch receives only the first byte, but recovery can leave a tail.
                while (_start < _end && _buffer[_start] is not (13 or 10))
                {
                    Consume(1);
                }
                if (_start < _end)
                {
                    _skippingText = false;
                    Consume(1);
                    continue;
                }
                break;
            }
            if (!JavadProtocol.IsIdentifierByte(available[0]))
            {
                RejectCandidate();
                continue;
            }
            if (available.Length < JavadProtocol.HeaderSize)
            {
                break;
            }
            if (!JavadProtocol.TryReadBodyLength(available, out var bodySize))
            {
                RejectCandidate();
                continue;
            }
            var frameSize = JavadProtocol.HeaderSize + bodySize;
            if (available.Length < frameSize)
            {
                break;
            }

            var id = string.Create(
                2,
                (available[0], available[1]),
                static (chars, state) =>
                {
                    chars[0] = (char)state.Item1;
                    chars[1] = (char)state.Item2;
                }
            );
            var message = _factory.Create(id);
            if (message == null)
            {
                Consume(frameSize);
                _recovering = false;
                _statistic.IncrementParserUnknownMessageError();
                _errors.OnErrorResume(new ProtocolParserUnknownMessageException(Info, id));
                continue;
            }
            if (FormatMajorVersion != 1 && id is not ("MF" or "JP"))
            {
                Consume(frameSize);
                _recovering = false;
                continue;
            }
            message.ByteOrder = ByteOrder;
            ReadOnlySpan<byte> packet = available[..frameSize];
            try
            {
                if (id == "MF")
                {
                    ValidateFormatMessage(packet);
                }
                message.Deserialize(ref packet);
                if (!packet.IsEmpty)
                {
                    throw new FormatException("The GREIS payload codec did not consume its frame.");
                }
            }
            catch (ProtocolParserCrcException error)
            {
                RejectCandidate();
                _statistic.IncrementParserBadCrcError();
                _errors.OnErrorResume(error);
                continue;
            }
            catch (Exception error)
            {
                RejectCandidate();
                _statistic.IncrementParserDeserializeError();
                _errors.OnErrorResume(
                    new ProtocolDeserializeMessageException(Info, message, error)
                );
                continue;
            }

            if (id == "MF")
            {
                // MF uses ASCII fields independently of the preceding stream byte order.
                FormatMajorVersion = (available[7] - '0') * 10 + available[8] - '0';
                FormatMinorVersion = (available[9] - '0') * 10 + available[10] - '0';
                ByteOrder =
                    available[11] == '0' ? JavadByteOrder.LittleEndian : JavadByteOrder.BigEndian;
            }
            Consume(frameSize);
            _recovering = false;
            _statistic.AddParserBytes(frameSize);
            _statistic.IncrementParsedMessage();
            published = true;
            try
            {
                _messages.OnNext(message);
            }
            catch (Exception error)
            {
                _statistic.IncrementParserPublishError();
                _errors.OnErrorResume(new ProtocolPublishMessageException(Info, message, error));
            }
            if (id == "MF" && FormatMajorVersion != 1)
            {
                _errors.OnErrorResume(
                    new ProtocolParserException(
                        Info,
                        $"GREIS format major version {FormatMajorVersion} is unsupported. Messages are skipped until a supported MF arrives."
                    )
                );
            }
        }
        return published;
    }

    private static void ValidateFormatMessage(ReadOnlySpan<byte> packet)
    {
        if (
            packet.Length != 14
            || packet[5] != 'J'
            || packet[6] != 'P'
            || packet[11] is not ((byte)'0' or (byte)'1')
        )
        {
            throw new FormatException("Invalid GREIS format announcement.");
        }
        for (var i = 7; i <= 10; i++)
        {
            if (packet[i] is < (byte)'0' or > (byte)'9')
            {
                throw new FormatException("GREIS format versions use two decimal digits.");
            }
        }
    }

    private void RejectCandidate()
    {
        // GREIS section 3.3 recommends retrying one byte later, including inside corrupt frames.
        _recovering = true;
        Consume(1);
    }

    private void Consume(int count)
    {
        _start += count;
        if (_start == _end)
        {
            _start = _end = 0;
        }
    }

    /// <summary>Discards an incomplete frame while retaining the stream's announced format.</summary>
    public void Reset()
    {
        _start = _end = 0;
        _skippingText = _recovering = false;
    }

    /// <summary>Starts a new stream with the default little-endian format 1.0.</summary>
    public void ResetStream()
    {
        Reset();
        ByteOrder = JavadByteOrder.LittleEndian;
        FormatMajorVersion = 1;
        FormatMinorVersion = 0;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _messages.Dispose();
            _errors.Dispose();
        }
        base.Dispose(disposing);
    }

    protected override async ValueTask DisposeAsyncCore()
    {
        _messages.Dispose();
        _errors.Dispose();
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }
}
