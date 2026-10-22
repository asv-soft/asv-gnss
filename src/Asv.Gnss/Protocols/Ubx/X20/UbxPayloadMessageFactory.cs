using System;
using System.Collections.Generic;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

internal interface IUbxPayloadMessageFactory
{
    UbxMessageBase? Create(ushort id, ReadOnlySpan<byte> payload);
}

// Selection belongs to one parser; shared protocol factories remain immutable.
internal sealed class UbxParserMessageFactory(IProtocolMessageFactory<UbxMessageBase, ushort> factory)
    : IProtocolMessageFactory<UbxMessageBase, ushort>
{
    private UbxMessageBase? _selected;
    private ushort _selectedId;
    private bool _hasSelection;

    public ProtocolInfo Info => factory.Info;

    public IEnumerable<ushort> GetSupportedIds() => factory.GetSupportedIds();

    internal void SelectPayload(ushort id, ReadOnlySpan<byte> payload)
    {
        _selected = null;
        _hasSelection = false;
        if (factory is not IUbxPayloadMessageFactory payloadFactory)
        {
            return;
        }

        _selected = payloadFactory.Create(id, payload);
        _selectedId = id;
        _hasSelection = true;
    }

    public UbxMessageBase? Create(ushort id)
    {
        if (!_hasSelection || id != _selectedId)
        {
            return factory.Create(id);
        }

        var selected = _selected;
        _selected = null;
        _hasSelection = false;
        return selected;
    }
}

internal sealed class UbxCompatibleMessageFactory
    : IProtocolMessageFactory<UbxMessageBase, ushort>, IUbxPayloadMessageFactory
{
    private readonly IProtocolMessageFactory<UbxMessageBase, ushort> _configured;
    private readonly UbxX20MessageFactory _x20 = new();
    private readonly HashSet<ushort> _x20Ids = [];

    internal UbxCompatibleMessageFactory(IProtocolMessageFactory<UbxMessageBase, ushort> configured)
    {
        _configured = configured;
        var generatedTypes = UbxX20Protocol.Messages.Select(message => message.MessageType).ToHashSet();
        foreach (var id in configured.GetSupportedIds())
        {
            if (configured.Create(id) is { } message && generatedTypes.Contains(message.GetType()))
            {
                _x20Ids.Add(id);
            }
        }
    }

    public ProtocolInfo Info => UbxProtocol.Info;

    public IEnumerable<ushort> GetSupportedIds() => _configured.GetSupportedIds();

    public UbxMessageBase? Create(ushort id) => _configured.Create(id);

    public UbxMessageBase? Create(ushort id, ReadOnlySpan<byte> payload) =>
        _x20Ids.Contains(id) ? _x20.Create(id, payload) : _configured.Create(id);
}
