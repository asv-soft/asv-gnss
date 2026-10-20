using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;
using R3;

namespace Asv.Gnss;

internal sealed class RecordedGnssStream : IDisposable
{
    private readonly VirtualPort _port;
    private readonly List<IDisposable> _subscriptions = [];

    public RecordedGnssStream()
    {
        var protocol = Protocol.Create(builder =>
        {
            builder.Protocols.RegisterUbxProtocol();
            builder.Protocols.RegisterNmeaProtocol();
        });
        _port = new VirtualPort("recorded-gnss", (IProtocolContext)protocol, static _ => true, new Statistic());
        Parsers = _port.Parsers.ToArray();
        _subscriptions.Add(_port.OnRxMessage.Subscribe(Messages.Add));
        _subscriptions.Add(_port.OnRxError.Subscribe(Errors.Add));
        foreach (var parser in Parsers)
        {
            _subscriptions.Add(parser.OnError.Subscribe(Errors.Add, Errors.Add, _ => { }));
        }
    }

    public List<IProtocolMessage> Messages { get; } = [];
    public List<Exception> Errors { get; } = [];
    public IReadOnlyList<IProtocolParser> Parsers { get; }

    public void Push(ReadOnlySpan<byte> bytes, int chunkSize = 4096)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(chunkSize);
        for (var offset = 0; offset < bytes.Length; offset += chunkSize)
        {
            _port.Rx.OnNext(bytes.Slice(offset, Math.Min(chunkSize, bytes.Length - offset)).ToArray());
        }
    }

    public void Reset()
    {
        foreach (var parser in Parsers)
        {
            parser.Reset();
        }
    }

    public void Dispose()
    {
        foreach (var subscription in _subscriptions)
        {
            subscription.Dispose();
        }
        _port.Dispose();
    }
}
