using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>
/// ComNav OEM receiver protocol registration helpers.
/// </summary>
public static class ComNavProtocol
{
    /// <summary>
    /// ComNav binary protocol information.
    /// </summary>
    public static ProtocolInfo BinaryInfo { get; } = new(ComNavBinaryParser.GnssProtocolId, "ComNav Binary");

    /// <summary>
    /// ComNav ASCII log protocol information.
    /// </summary>
    public static ProtocolInfo AsciiInfo { get; } = new(ComNavAsciiParser.GnssProtocolId, "ComNav ASCII");

    /// <summary>
    /// ComNav simple answer protocol information.
    /// </summary>
    public static ProtocolInfo SimpleAnswerInfo { get; } = new(ComNavSimpleAnswerParser.GnssProtocolId, "ComNav Simple Answer");

    /// <summary>
    /// Registers all restored ComNav parsers with their default messages.
    /// </summary>
    public static void RegisterComNavProtocol(this IProtocolParserBuilder builder)
    {
        builder.RegisterComNavBinaryProtocol();
        builder.RegisterComNavAsciiProtocol();
        builder.RegisterComNavSimpleAnswerProtocol();
    }

    /// <summary>
    /// Registers the ComNav binary parser.
    /// </summary>
    public static void RegisterComNavBinaryProtocol(this IProtocolParserBuilder builder, Action<IProtocolMessageFactoryBuilder<ComNavBinaryMessageBase, ushort>>? configure = null)
    {
        var factory = new ProtocolMessageFactoryBuilder<ComNavBinaryMessageBase, ushort>(BinaryInfo);
        ComNavBinaryFactory.RegisterDefaultMessages(factory);
        configure?.Invoke(factory);
        var messageFactory = factory.Build();
        builder.Register(BinaryInfo, (core, stat) => new ComNavBinaryParser(messageFactory, core, stat));
    }

    /// <summary>
    /// Registers the ComNav ASCII log parser.
    /// </summary>
    public static void RegisterComNavAsciiProtocol(this IProtocolParserBuilder builder, Action<IProtocolMessageFactoryBuilder<ComNavAsciiMessageBase, ComNavAsciiMessageId>>? configure = null)
    {
        var factory = new ProtocolMessageFactoryBuilder<ComNavAsciiMessageBase, ComNavAsciiMessageId>(AsciiInfo);
        ComNavAsciiFactory.RegisterDefaultMessages(factory);
        configure?.Invoke(factory);
        var messageFactory = factory.Build();
        builder.Register(AsciiInfo, (core, stat) => new ComNavAsciiParser(messageFactory, core, stat));
    }

    /// <summary>
    /// Registers the ComNav simple answer parser.
    /// </summary>
    public static void RegisterComNavSimpleAnswerProtocol(this IProtocolParserBuilder builder, Action<IProtocolMessageFactoryBuilder<ComNavSimpleAnswerMessageBase, ComNavAsciiMessageId>>? configure = null)
    {
        var factory = new ProtocolMessageFactoryBuilder<ComNavSimpleAnswerMessageBase, ComNavAsciiMessageId>(SimpleAnswerInfo);
        ComNavSimpleAnswerFactory.RegisterDefaultMessages(factory);
        configure?.Invoke(factory);
        var messageFactory = factory.Build();
        builder.Register(SimpleAnswerInfo, (core, stat) => new ComNavSimpleAnswerParser(messageFactory, core, stat));
    }
}

/// <summary>
/// Base class for restored ComNav protocol messages.
/// </summary>
/// <typeparam name="TId">Message identifier type.</typeparam>
public abstract class ComNavMessageBase<TId> : IProtocolMessage<TId>
    where TId : struct
{
    private ProtocolTags _tags = [];

    /// <summary>
    /// Gets the legacy ComNav protocol identifier.
    /// </summary>
    public abstract string ProtocolId { get; }

    /// <summary>
    /// Gets the typed message identifier used by the parser factory.
    /// </summary>
    public abstract TId MessageId { get; }

    /// <inheritdoc />
    public TId Id => MessageId;

    /// <inheritdoc />
    public abstract ProtocolInfo Protocol { get; }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public ref ProtocolTags Tags => ref _tags;

    /// <inheritdoc />
    public string GetIdAsString() => MessageId.ToString() ?? string.Empty;

    /// <inheritdoc />
    public abstract void Deserialize(ref ReadOnlySpan<byte> buffer);

    /// <inheritdoc />
    public abstract void Serialize(ref Span<byte> buffer);

    /// <inheritdoc />
    public abstract int GetByteSize();
}

/// <summary>
/// Case-insensitive ComNav ASCII message identifier.
/// </summary>
public readonly struct ComNavAsciiMessageId(string id) : IEquatable<ComNavAsciiMessageId>
{
    /// <summary>
    /// Gets the raw ASCII identifier.
    /// </summary>
    public string Id { get; } = id;

    /// <inheritdoc />
    public bool Equals(ComNavAsciiMessageId other) => string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ComNavAsciiMessageId other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Id);

    /// <inheritdoc />
    public override string ToString() => Id;

    public static implicit operator ComNavAsciiMessageId(string id) => new(id);
}

/// <summary>
/// Base class for restored ComNav messages identified by ASCII text.
/// </summary>
public abstract class ComNavStringMessageBase : IProtocolMessage<ComNavAsciiMessageId>
{
    private ProtocolTags _tags = [];

    /// <summary>
    /// Gets the legacy ComNav protocol identifier.
    /// </summary>
    public abstract string ProtocolId { get; }

    /// <summary>
    /// Gets the ASCII message identifier.
    /// </summary>
    public abstract string MessageId { get; }

    ComNavAsciiMessageId IProtocolMessage<ComNavAsciiMessageId>.Id => new(MessageId);

    /// <inheritdoc />
    public abstract ProtocolInfo Protocol { get; }

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public ref ProtocolTags Tags => ref _tags;

    /// <inheritdoc />
    public string GetIdAsString() => MessageId;

    /// <inheritdoc />
    public abstract void Deserialize(ref ReadOnlySpan<byte> buffer);

    /// <inheritdoc />
    public abstract void Serialize(ref Span<byte> buffer);

    /// <inheritdoc />
    public abstract int GetByteSize();
}

/// <summary>
/// Compatibility exception for restored ComNav parsers.
/// </summary>
public class GnssParserException(string protocolId, string message) : Exception($"{protocolId}: {message}");
