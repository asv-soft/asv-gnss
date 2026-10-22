using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asv.IO;
using R3;

namespace Asv.Gnss;

/// <summary>Offline RINEX OBS/NAV/MET file I/O. Input streams are left open; gzip is detected by its magic bytes.</summary>
public static class RinexFile
{
    /// <summary>Reads a plain or gzip RINEX file; caller-owned streams remain open.</summary>
    public static IReadOnlyList<RinexMessageBase> Read(string path)
    {
        using var stream = File.OpenRead(path);
        return Read(stream);
    }

    /// <summary>Reads a plain or gzip RINEX file; caller-owned streams remain open.</summary>
    public static IReadOnlyList<RinexMessageBase> Read(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        var prefix = new byte[2];
        var count = 0;
        while (count < prefix.Length)
        {
            var read = stream.Read(prefix, count, prefix.Length - count);
            if (read == 0) break;
            count += read;
        }
        using var input = OpenInput(stream, prefix.AsMemory(0, count));
        using var session = new ReadSession();
        var buffer = ArrayPool<byte>.Shared.Rent(8192);
        try
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) != 0)
                session.Push(buffer.AsSpan(0, read));
            return session.Complete();
        }
        finally { ArrayPool<byte>.Shared.Return(buffer); }
    }

    /// <summary>Reads a plain or gzip RINEX file asynchronously; caller-owned streams remain open.</summary>
    public static async Task<IReadOnlyList<RinexMessageBase>> ReadAsync(string path, CancellationToken cancellationToken = default)
    {
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 8192,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        return await ReadAsync(stream, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Reads a plain or gzip RINEX file asynchronously; caller-owned streams remain open.</summary>
    public static async Task<IReadOnlyList<RinexMessageBase>> ReadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        cancellationToken.ThrowIfCancellationRequested();
        var prefix = new byte[2];
        var count = 0;
        while (count < prefix.Length)
        {
            var read = await stream.ReadAsync(prefix.AsMemory(count), cancellationToken).ConfigureAwait(false);
            if (read == 0) break;
            count += read;
        }
        await using var input = OpenInput(stream, prefix.AsMemory(0, count));
        using var session = new ReadSession();
        var buffer = ArrayPool<byte>.Shared.Rent(8192);
        try
        {
            int read;
            while ((read = await input.ReadAsync(buffer.AsMemory(), cancellationToken).ConfigureAwait(false)) != 0)
            {
                cancellationToken.ThrowIfCancellationRequested();
                session.Push(buffer.AsSpan(0, read));
            }
            cancellationToken.ThrowIfCancellationRequested();
            return session.Complete();
        }
        finally { ArrayPool<byte>.Shared.Return(buffer); }
    }

    /// <summary>Writes a header and compatible native records as canonical RINEX text.</summary>
    public static void Write(string path, IEnumerable<RinexMessageBase> messages)
    {
        using var stream = File.Create(path);
        Write(stream, messages);
    }

    /// <summary>Writes a header and compatible native records as canonical RINEX text.</summary>
    public static void Write(Stream stream, IEnumerable<RinexMessageBase> messages)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(messages);
        RinexHeader? currentHeader = null;
        var first = true;
        foreach (var message in messages)
        {
            if (first && message is not RinexHeaderMessage)
                throw new ArgumentException("A RINEX file must start with a header message.", nameof(messages));
            ValidateContext(message, ref currentHeader);
            first = false;
            var bytes = new byte[message.GetByteSize()];
            var output = bytes.AsSpan();
            message.Serialize(ref output);
            stream.Write(bytes);
        }
        if (first) throw new ArgumentException("A RINEX file requires a header.", nameof(messages));
    }

    /// <summary>Writes canonical records asynchronously, validating their header context and leaving the stream open.</summary>
    public static async Task WriteAsync(Stream stream, IEnumerable<RinexMessageBase> messages,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(messages);
        RinexHeader? currentHeader = null;
        var first = true;
        foreach (var message in messages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (first && message is not RinexHeaderMessage)
                throw new ArgumentException("A RINEX file must start with a header message.", nameof(messages));
            ValidateContext(message, ref currentHeader);
            first = false;
            var bytes = new byte[message.GetByteSize()];
            var output = bytes.AsSpan();
            message.Serialize(ref output);
            await stream.WriteAsync(bytes, cancellationToken).ConfigureAwait(false);
        }
        if (first) throw new ArgumentException("A RINEX file requires a header.", nameof(messages));
    }

    private static void ValidateContext(RinexMessageBase message, ref RinexHeader? currentHeader)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(message.Header);
        if (message is RinexHeaderMessage)
        {
            currentHeader = message.Header;
            return;
        }
        var header = message.Header;
        var expected = currentHeader;
        if (currentHeader == null || header.Version != currentHeader.Version || header.FileType != currentHeader.FileType ||
            header.SatelliteSystem != currentHeader.SatelliteSystem || header.TimeSystem != currentHeader.TimeSystem ||
            !header.LegacyObservationTypes.SequenceEqual(currentHeader.LegacyObservationTypes) ||
            header.ObservationTypes.Count != currentHeader.ObservationTypes.Count ||
            header.ObservationTypes.Any(pair => !expected!.ObservationTypes.TryGetValue(pair.Key, out var codes) ||
                !pair.Value.SequenceEqual(codes)) || message.Id != RinexRecordFraming.GetBodyId(currentHeader))
            throw new ArgumentException("The RINEX record context does not match the current file header.", nameof(message));
        if (message is RinexObservationEpochMessage { EpochFlag: 4 } update)
            currentHeader = currentHeader.WithUpdates(update.SpecialRecords);
    }

    private static Stream OpenInput(Stream stream, ReadOnlyMemory<byte> prefix)
    {
        var input = new PrefixStream(stream, prefix);
        return prefix.Length == 2 && prefix.Span[0] == 0x1F && prefix.Span[1] == 0x8B
            ? new GZipStream(input, CompressionMode.Decompress) : input;
    }

    private sealed class ReadSession : IDisposable
    {
        private readonly List<RinexMessageBase> _messages = [];
        private readonly List<Exception> _errors = [];
        private readonly RinexMessageParser _parser;
        private readonly IDisposable _messageSubscription;
        private readonly IDisposable _errorSubscription;
        internal ReadSession()
        {
            var protocol = Protocol.Create(builder => builder.Protocols.RegisterRinexProtocol());
            _parser = (RinexMessageParser)protocol.CreateParser(RinexProtocol.Info.Id);
            _messageSubscription = _parser.OnMessage.Subscribe(message => _messages.Add((RinexMessageBase)message));
            _errorSubscription = _parser.OnError.Subscribe(_errors.Add, _errors.Add, _ => { });
        }
        internal void Push(ReadOnlySpan<byte> bytes)
        {
            foreach (var value in bytes) _parser.Push(value);
            ThrowIfErrors();
        }
        internal IReadOnlyList<RinexMessageBase> Complete()
        {
            _parser.Complete();
            ThrowIfErrors();
            if (_messages.All(message => message is not RinexHeaderMessage))
                throw new FormatException("No complete RINEX header was found.");
            return _messages.ToArray();
        }
        private void ThrowIfErrors()
        {
            if (_errors.Count != 0) throw new FormatException("The RINEX file contains an invalid or unsupported record.", _errors[0]);
        }
        public void Dispose() { _messageSubscription.Dispose(); _errorSubscription.Dispose(); _parser.Dispose(); }
    }

    private sealed class PrefixStream(Stream inner, ReadOnlyMemory<byte> prefix) : Stream
    {
        private int _offset;
        public override bool CanRead => inner.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override int Read(byte[] buffer, int offset, int count) => Read(buffer.AsSpan(offset, count));
        public override int Read(Span<byte> buffer)
        {
            if (_offset == prefix.Length) return inner.Read(buffer);
            var count = Math.Min(buffer.Length, prefix.Length - _offset);
            prefix.Span.Slice(_offset, count).CopyTo(buffer);
            _offset += count;
            return count;
        }
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return _offset == prefix.Length ? inner.ReadAsync(buffer, cancellationToken) : ValueTask.FromResult(Read(buffer.Span));
        }
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
