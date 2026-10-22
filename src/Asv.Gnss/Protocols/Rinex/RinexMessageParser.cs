using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>A stateful ASCII file parser. Call Complete at EOF, including when the last line has no newline.</summary>
public sealed class RinexMessageParser : ProtocolParser<RinexMessageBase, RinexMessageId>
{
    private readonly ContextFactory _factory;
    private readonly StringBuilder _line = new();
    private readonly List<string> _pending = new();
    private bool _readingHeader;
    private bool _discardLine;
    private bool _afterCr;
    private int _recordBytes;
    private int _bomState;

    /// <summary>Creates an independent parser with the supplied factory, protocol context and statistics.</summary>
    public RinexMessageParser(IProtocolMessageFactory<RinexMessageBase, RinexMessageId> factory,
        IProtocolContext context, IStatisticHandler? statisticHandler)
        : this(new ContextFactory(factory), context, statisticHandler) { }

    private RinexMessageParser(ContextFactory factory, IProtocolContext context, IStatisticHandler? statisticHandler)
        : base(factory, context, statisticHandler) => _factory = factory;

    /// <inheritdoc />
    public override ProtocolInfo Info => RinexProtocol.Info;
    /// <summary>Current decoding context, or null before a complete file header is available.</summary>
    public RinexHeader? CurrentHeader => _factory.Header;
    /// <summary>Whether the parser is collecting a header or already has file context.</summary>
    public bool IsReadingFile => _readingHeader || CurrentHeader != null;

    /// <inheritdoc />
    public override bool Push(byte data)
    {
        if (_bomState < 3)
        {
            if ((_bomState == 0 && data == 0xEF) || (_bomState == 1 && data == 0xBB) ||
                (_bomState == 2 && data == 0xBF))
            {
                _bomState++;
                return false;
            }
            if (_bomState != 0) { _discardLine = true; _line.Clear(); }
            _bomState = 3;
        }
        if (data is 10 or 13)
        {
            if (data == 10 && _afterCr) { _afterCr = false; return false; }
            _afterCr = data == 13;
            var line = _line.ToString();
            _line.Clear();
            if (_discardLine) { _discardLine = false; return false; }
            try { return ProcessLine(line); }
            catch (Exception error) when (error is FormatException or NotSupportedException or ArgumentException or OverflowException)
            {
                Report(error);
                var next = !_readingHeader && _pending.Count > 1 && _pending[^1].StartsWith('>')
                    ? _pending[^1] : null;
                ClearRecord();
                if (_readingHeader) { _readingHeader = false; _factory.Header = null; }
                var marker = line.IndexOf("RINEX VERSION / TYPE", StringComparison.Ordinal);
                if (marker >= 60)
                {
                    _readingHeader = true;
                    _factory.Header = null;
                    AddLine(line[(marker - 60)..]);
                }
                else if (next != null && _factory.Header != null) AddLine(next);
                return false;
            }
        }
        _afterCr = false;
        if (_discardLine) return false;
        if (data > 127 || (data < 32 && data != 9) || _line.Length >= RinexProtocol.MaxLineLength)
        {
            if (_factory.Header != null || _readingHeader)
                Report(new FormatException("RINEX input contains non-ASCII data or an oversized line."));
            _discardLine = true;
            _line.Clear();
            ClearRecord();
            _factory.Header = null;
            _readingHeader = false;
            return false;
        }
        _line.Append((char)data);
        return false;
    }

    /// <summary>Flushes the final record and throws for incomplete or malformed trailing data.</summary>
    public bool Complete()
    {
        var published = false;
        try
        {
            if (_line.Length != 0)
            {
                var line = _line.ToString();
                _line.Clear();
                published = ProcessLine(line);
            }
            if (_discardLine) throw new FormatException("The final RINEX line is invalid.");
            if (_readingHeader) throw new FormatException("The RINEX header is incomplete.");
            published |= Drain(true);
            if (_pending.Count != 0) throw new FormatException("The final RINEX record is incomplete.");
            return published;
        }
        catch (Exception error) when (error is FormatException or NotSupportedException or ArgumentException or OverflowException)
        {
            Report(error);
            ClearRecord();
            throw;
        }
    }

    private bool ProcessLine(string line)
    {
        var marker = line.IndexOf("RINEX VERSION / TYPE", StringComparison.Ordinal);
        if (marker >= 60)
        {
            var published = Drain(true);
            if (_readingHeader || _pending.Count > 0)
                throw new FormatException("A new RINEX file starts before the preceding record is complete.");
            _readingHeader = true;
            _factory.Header = null;
            AddLine(line[(marker - 60)..]);
            return published;
        }
        if (line.Contains("CRINEX VERS   / TYPE", StringComparison.Ordinal))
            throw new NotSupportedException("Compact RINEX (Hatanaka) must be expanded before parsing.");
        if (_readingHeader)
        {
            AddLine(line);
            if (RinexHeader.Label(line) != "END OF HEADER") return false;
            _factory.Header = RinexHeader.Parse(_pending);
            _readingHeader = false;
            Dispatch(RinexMessageId.Header, _pending.ToArray());
            ClearRecord();
            return true;
        }
        if (_factory.Header == null || (_pending.Count == 0 && string.IsNullOrWhiteSpace(line))) return false;
        AddLine(line);
        return Drain(false);
    }

    private void AddLine(string line)
    {
        _recordBytes = checked(_recordBytes + line.Length + 1);
        if (_pending.Count >= RinexProtocol.MaxRecordLines || _recordBytes > RinexProtocol.MaxRecordBytes)
            throw new FormatException("RINEX record exceeds the parser's configured safety limit.");
        _pending.Add(line);
    }

    private bool Drain(bool endOfFile)
    {
        var published = false;
        while (_factory.Header != null && _pending.Count > 0 && !_readingHeader)
        {
            var id = RinexRecordFraming.GetBodyId(_factory.Header);
            var count = RinexRecordFraming.GetLineCount(_pending, _factory.Header, id, endOfFile);
            if (!count.HasValue || count > _pending.Count) break;
            if (count <= 0) throw new FormatException("Invalid RINEX record length.");
            var lines = _pending.Take(count.Value).ToArray();
            if (id == RinexMessageId.ObservationEpoch)
            {
                // Parse events independently of factory filtering: header updates still affect later epochs.
                ReadOnlySpan<byte> data = Encoding.ASCII.GetBytes(string.Join("\n", lines) + "\n");
                var observation = new RinexObservationEpochMessage { Header = _factory.Header };
                observation.Deserialize(ref data);
                var nextHeader = observation.EpochFlag == 4
                    ? _factory.Header.WithUpdates(observation.SpecialRecords) : _factory.Header;
                Dispatch(id, lines);
                _factory.Header = nextHeader;
            }
            else Dispatch(id, lines);
            _pending.RemoveRange(0, count.Value);
            _recordBytes = _pending.Sum(line => line.Length + 1);
            published = true;
        }
        return published;
    }

    private void Dispatch(RinexMessageId id, IReadOnlyList<string> lines)
    {
        ReadOnlySpan<byte> data = Encoding.ASCII.GetBytes(string.Join("\n", lines) + "\n");
        InternalParsePacket(id, ref data);
        if (!data.IsEmpty && _factory.IsRegistered(id))
            throw new FormatException($"Invalid RINEX {id} record.");
    }

    private void Report(Exception error) => InternalOnError(new ProtocolParserException(Info, error.Message, error));
    private void ClearRecord() { _pending.Clear(); _recordBytes = 0; }
    /// <inheritdoc />
    public override void Reset()
    {
        _line.Clear();
        ClearRecord();
        _factory.Header = null;
        _readingHeader = _discardLine = _afterCr = false;
        _bomState = 0;
    }

    private sealed class ContextFactory(IProtocolMessageFactory<RinexMessageBase, RinexMessageId> inner)
        : IProtocolMessageFactory<RinexMessageBase, RinexMessageId>
    {
        internal RinexHeader? Header { get; set; }
        public ProtocolInfo Info => inner.Info;
        public RinexMessageBase? Create(RinexMessageId id)
        {
            var result = inner.Create(id);
            if (result != null && Header != null) result.Header = Header;
            return result;
        }
        public IEnumerable<RinexMessageId> GetSupportedIds() => inner.GetSupportedIds();
        internal bool IsRegistered(RinexMessageId id) => inner.GetSupportedIds().Contains(id);
    }
}
