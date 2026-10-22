using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>Selects the UBX layouts documented for u-blox X20 HPG 2.11.</summary>
/// <remarks>Message versions and subtypes are selected from the payload, not just its class and ID.</remarks>
public sealed class UbxX20MessageFactory : IProtocolMessageFactory<UbxMessageBase, ushort>, IUbxPayloadMessageFactory
{
    private static readonly ImmutableArray<ushort> SupportedIds = UbxX20GeneratedFactory.Messages
        .Select(message => message.Id).Distinct().Order().ToImmutableArray();

    /// <inheritdoc />
    public ProtocolInfo Info => UbxProtocol.Info;

    /// <summary>Creates the default layout for an ID, preferring a data message over a poll.</summary>
    /// <param name="id">Combined UBX class and message ID.</param>
    /// <returns>A new model, or null for an unsupported ID.</returns>
    public UbxX20MessageBase? Create(ushort id) => UbxX20GeneratedFactory.CreateDefault(id);

    /// <summary>Creates the layout selected by the payload's length, version and subtype fields.</summary>
    /// <param name="id">Combined UBX class and message ID.</param>
    /// <param name="payload">The UBX payload without its header or checksum.</param>
    /// <returns>A new model, or null if no documented variant matches. The payload is not deserialized.</returns>
    public UbxX20MessageBase? Create(ushort id, ReadOnlySpan<byte> payload) =>
        UbxX20GeneratedFactory.Create(id, payload);

    /// <inheritdoc />
    public IEnumerable<ushort> GetSupportedIds() => SupportedIds;

    UbxMessageBase? IProtocolMessageFactory<UbxMessageBase, ushort>.Create(ushort id) => Create(id);

    UbxMessageBase? IUbxPayloadMessageFactory.Create(ushort id, ReadOnlySpan<byte> payload) => Create(id, payload);
}
