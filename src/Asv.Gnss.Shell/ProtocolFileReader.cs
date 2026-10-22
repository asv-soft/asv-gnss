using System.Buffers;
using Asv.IO;
using R3;

namespace Asv.Gnss;

internal sealed record ProtocolFileMessage(long Index, IProtocolMessage Message);

internal sealed record ProtocolFileCapture(
    string FilePath,
    long ByteCount,
    IReadOnlyList<ProtocolFileMessage> Messages,
    IReadOnlyList<string> Protocols,
    long ErrorCount);

internal static class ProtocolFileReader
{
    internal static async Task<ProtocolFileCapture> ReadAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        cancellationToken.ThrowIfCancellationRequested();
        var fullPath = Path.GetFullPath(path);
        await using var stream = new FileStream(fullPath, new FileStreamOptions
        {
            Mode = FileMode.Open,
            Access = FileAccess.Read,
            Share = FileShare.Read,
            Options = FileOptions.Asynchronous | FileOptions.SequentialScan
        });
        return await ReadAsync(stream, fullPath, cancellationToken).ConfigureAwait(false);
    }

    internal static async Task<ProtocolFileCapture> ReadAsync(
        Stream stream,
        string path,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        cancellationToken.ThrowIfCancellationRequested();
        IProtocolMessageFactory<AsterixMessage, byte>? asterixFactory = null;
        IProtocolMessageFactory<JavadMessageBase, string>? greisFactory = null;
        var protocol = Protocol.Create(builder =>
        {
            builder.Protocols.RegisterNmeaProtocol();
            builder.Protocols.RegisterRtcmV2Protocol();
            builder.Protocols.RegisterRtcmV3Protocol();
            builder.Protocols.RegisterUbxProtocol();
            builder.Protocols.RegisterAsterixProtocol(factory =>
                asterixFactory = ((ProtocolMessageFactoryBuilder<AsterixMessage, byte>)factory).Build());
            builder.Protocols.RegisterAsvProtocol();
            builder.Protocols.RegisterGbasProtocol();
            builder.Protocols.RegisterSbfProtocol();
            builder.Protocols.RegisterComNavProtocol();
            builder.Protocols.RegisterJavadProtocol(factory =>
                greisFactory = ((ProtocolMessageFactoryBuilder<JavadMessageBase, string>)factory).Build());
        });
        var messages = new List<ProtocolFileMessage>();
        var subscriptions = new List<IDisposable>();
        var parsers = new List<IProtocolParser>();
        long errors = 0;
        long byteCount = 0;
        Action countError = () => errors++;
        void AddMessage(IProtocolMessage message) =>
            messages.Add(new ProtocolFileMessage(messages.Count + 1L, message));
        var buffer = ArrayPool<byte>.Shared.Rent(8192);
        try
        {
            foreach (var info in protocol.AvailableProtocols)
            {
                var parser = protocol.CreateParser(info.Id);
                parsers.Add(parser);
                subscriptions.Add(parser.OnMessage.Subscribe(AddMessage));
                subscriptions.Add(parser.OnError.Subscribe(_ => errors++, _ => errors++, _ => { }));
            }

            var asterixParser = parsers.Single(parser => parser.Info.Id == AsterixProtocol.Info.Id);
            var greisParser = (JavadMessageParser)parsers.Single(parser => parser.Info.Id == JavadProtocol.Info.Id);
            var asterixScanner = new AsterixFileScanner(asterixFactory!, AddMessage, countError);
            var greisScanner = new GreisFileScanner(greisFactory!, greisParser, countError);
            var streamParsers = parsers.Where(parser => parser != asterixParser && parser != greisParser).ToArray();
            int count;
            while ((count = await stream.ReadAsync(buffer.AsMemory(0, 8192), cancellationToken)
                       .ConfigureAwait(false)) != 0)
            {
                for (var index = 0; index < count; index++)
                {
                    if ((index & 255) == 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    var value = buffer[index];
                    foreach (var parser in streamParsers)
                    {
                        Push(parser, value, countError);
                    }
                    asterixScanner.Push(value, byteCount);
                    greisScanner.Push(value, byteCount);
                    byteCount++;
                }
            }
            cancellationToken.ThrowIfCancellationRequested();
            return new ProtocolFileCapture(path, byteCount, messages.ToArray(),
                protocol.AvailableProtocols.Select(info => info.Id).Order(StringComparer.Ordinal).ToArray(), errors);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }
            foreach (var parser in parsers)
            {
                parser.Dispose();
            }
        }
    }

    private static void Push(IProtocolParser parser, byte value, Action onError)
    {
        try
        {
            parser.Push(value);
        }
        catch (Exception)
        {
            onError();
            parser.Reset();
        }
    }

    private sealed class GreisFileScanner
    {
        private const int Capacity = 8192;
        private readonly byte[] _buffer = new byte[Capacity * 2];
        private readonly Dictionary<ushort, string> _identifiers;
        private readonly PriorityQueue<(long Start, int Length, ushort Identifier), long> _pending = new();
        private readonly IProtocolMessageFactory<JavadMessageBase, string> _factory;
        private readonly JavadMessageParser _parser;
        private readonly Action _onError;

        internal GreisFileScanner(
            IProtocolMessageFactory<JavadMessageBase, string> factory,
            JavadMessageParser parser,
            Action onError)
        {
            _factory = factory;
            _parser = parser;
            _onError = onError;
            _identifiers = factory.GetSupportedIds().ToDictionary(
                id => (ushort)((id[0] << 8) | id[1]), id => id);
        }

        internal void Push(byte value, long offset)
        {
            var position = (int)(offset % Capacity);
            _buffer[position] = _buffer[position + Capacity] = value;
            if (offset >= JavadProtocol.HeaderSize - 1)
            {
                var start = offset - JavadProtocol.HeaderSize + 1;
                var header = _buffer.AsSpan((int)(start % Capacity), JavadProtocol.HeaderSize);
                var identifier = (ushort)((header[0] << 8) | header[1]);
                var high = HexValue(header[2]);
                var middle = HexValue(header[3]);
                var low = HexValue(header[4]);
                if (_identifiers.ContainsKey(identifier) && high >= 0 && middle >= 0 && low >= 0)
                {
                    var length = JavadProtocol.HeaderSize + ((high << 8) | (middle << 4) | low);
                    _pending.Enqueue((start, length, identifier), start + length - 1);
                }
            }
            while (_pending.TryPeek(out var candidate, out var end) && end <= offset)
            {
                _pending.Dequeue();
                var frame = _buffer.AsSpan((int)(candidate.Start % Capacity), candidate.Length);
                if (!IsValid(frame, _identifiers[candidate.Identifier]))
                {
                    continue;
                }
                // Reset only this candidate's framing; the production parser retains MF byte-order state.
                _parser.Reset();
                foreach (var item in frame)
                {
                    ProtocolFileReader.Push(_parser, item, _onError);
                }
            }
        }

        private bool IsValid(ReadOnlySpan<byte> frame, string identifier)
        {
            try
            {
                // Reject corrupt outer candidates before the stream parser can rediscover a nested frame.
                var message = _factory.Create(identifier)!;
                message.ByteOrder = _parser.ByteOrder;
                message.Deserialize(ref frame);
                if (frame.IsEmpty)
                {
                    return true;
                }
            }
            catch (Exception)
            {
                // Candidate validation uses the production payload and checksum codec.
            }
            _onError();
            return false;
        }

        private static int HexValue(byte value) => value switch
        {
            >= (byte)'0' and <= (byte)'9' => value - '0',
            >= (byte)'A' and <= (byte)'F' => value - 'A' + 10,
            _ => -1
        };
    }

    private sealed class AsterixFileScanner
    {
        private const int Capacity = 65536;
        // Mirroring the ring avoids copying a wrapped candidate before validating its payload.
        private readonly byte[] _buffer = new byte[Capacity * 2];
        private readonly bool[] _categories = new bool[256];
        private readonly PriorityQueue<(long Start, int Length), long> _pending = new();
        private readonly IProtocolMessageFactory<AsterixMessage, byte> _factory;
        private readonly Action<IProtocolMessage> _onMessage;
        private readonly Action _onError;

        internal AsterixFileScanner(
            IProtocolMessageFactory<AsterixMessage, byte> factory,
            Action<IProtocolMessage> onMessage,
            Action onError)
        {
            _factory = factory;
            _onMessage = onMessage;
            _onError = onError;
            foreach (var category in factory.GetSupportedIds())
            {
                _categories[category] = true;
            }
        }

        internal void Push(byte value, long offset)
        {
            var position = (int)(offset % Capacity);
            _buffer[position] = _buffer[position + Capacity] = value;
            if (offset >= 2)
            {
                var start = offset - 2;
                var header = _buffer.AsSpan((int)(start % Capacity), 3);
                var length = (header[1] << 8) | header[2];
                if (_categories[header[0]] && length >= 4)
                {
                    _pending.Enqueue((start, length), start + length - 1);
                }
            }
            while (_pending.TryPeek(out var candidate, out var end) && end <= offset)
            {
                _pending.Dequeue();
                var frame = _buffer.AsSpan((int)(candidate.Start % Capacity), candidate.Length);
                ParseCandidate(frame);
            }
        }

        private void ParseCandidate(ReadOnlySpan<byte> frame)
        {
            try
            {
                // A standalone ASTERIX block starts with its data source (FRN 1), not arbitrary field bytes.
                if ((frame[3] & 0x80) == 0)
                {
                    _onError();
                    return;
                }
                var length = frame.Length;
                var message = _factory.Create(frame[0])!;
                message.Deserialize(ref frame);
                if (frame.IsEmpty && message.GetByteSize() == length && HasNonemptyRecords(message))
                {
                    _onMessage(message);
                    return;
                }
            }
            catch (Exception)
            {
                // Invalid candidates are parser attempts, not necessarily damaged source packets.
            }
            _onError();
        }

        private static bool HasNonemptyRecords(AsterixMessage message)
        {
            if (message is not IEnumerable<AsterixRecord> records)
            {
                return false;
            }
            var hasRecords = false;
            foreach (var record in records)
            {
                if (!record.Any())
                {
                    return false;
                }
                hasRecords = true;
            }
            return hasRecords;
        }
    }
}
