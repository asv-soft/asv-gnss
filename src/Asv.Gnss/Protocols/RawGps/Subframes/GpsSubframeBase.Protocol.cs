using System;
using Asv.IO;
using Newtonsoft.Json;

namespace Asv.Gnss;

public abstract partial class GpsSubframeBase : IProtocolMessage<byte>, IVisitable
{
    private readonly byte[] _sourceData = new byte[30];
    private ProtocolTags _tags = [];
    private byte _previousWordParity;
    private byte _sourceWordParity;
    private bool _pagePayloadChanged;
    private ushort _telemetryMessage;
    private bool _integrityStatusFlag;
    private bool _alertFlag;
    private bool _antiSpoofFlag;
    private static readonly Field TelemetryMessageField = GpsNavigationVisitor.Field(
        nameof(TelemetryMessage),
        UInt16Type.Default
    );
    private static readonly Field IntegrityStatusFlagField = GpsNavigationVisitor.Field(
        nameof(IntegrityStatusFlag),
        BoolType.Default
    );
    private static readonly Field AlertFlagField = GpsNavigationVisitor.Field(
        nameof(AlertFlag),
        BoolType.Default
    );
    private static readonly Field AntiSpoofFlagField = GpsNavigationVisitor.Field(
        nameof(AntiSpoofFlag),
        BoolType.Default
    );
    private static readonly Field TowField = GpsNavigationVisitor.Field(
        nameof(TOW1_5Epoh),
        UInt32Type.Default
    );
    private static readonly Field PreviousParityField = GpsNavigationVisitor.Field(
        nameof(PreviousWordParity),
        UInt8Type.Default
    );

    /// <summary>Last two transmitted bits of the preceding word: bit 1 is D29*, bit 0 is D30*.</summary>
    public byte PreviousWordParity
    {
        get => _previousWordParity;
        set => _previousWordParity = value;
    }

    /// <summary>The 14-bit TLM message, without the preamble, integrity flag or reserved bit.</summary>
    public ushort TelemetryMessage
    {
        get => _telemetryMessage;
        set => _telemetryMessage = value;
    }

    /// <summary>The TLM integrity status flag (word 1, bit 23).</summary>
    public bool IntegrityStatusFlag
    {
        get => _integrityStatusFlag;
        set => _integrityStatusFlag = value;
    }

    /// <summary>The HOW alert flag (word 2, bit 18).</summary>
    public bool AlertFlag
    {
        get => _alertFlag;
        set => _alertFlag = value;
    }

    /// <summary>The HOW anti-spoof flag (word 2, bit 19).</summary>
    public bool AntiSpoofFlag
    {
        get => _antiSpoofFlag;
        set => _antiSpoofFlag = value;
    }

    [JsonIgnore]
    public byte Id => SubframeId;

    [JsonIgnore]
    public virtual string Name => $"GPS-LNAV-{SubframeId}";

    [JsonIgnore]
    public ProtocolInfo Protocol => RawGpsProtocol.Info;

    [JsonIgnore]
    public ref ProtocolTags Tags => ref _tags;

    public string GetIdAsString() => Id.ToString(System.Globalization.CultureInfo.InvariantCulture);

    public int GetByteSize() => RawGpsProtocol.PacketSize;

    /// <summary>Reads four zero padding bits followed by 300 transmitted LNAV bits, checking all ten word parities.</summary>
    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < RawGpsProtocol.PacketSize)
            throw new FormatException("Truncated GPS LNAV subframe.");
        Span<byte> source = stackalloc byte[30];
        if (!RawGpsCodec.TryDecode(buffer[..RawGpsProtocol.PacketSize], source, out var previous))
            throw new ProtocolParserCrcException(Protocol);
        Deserialize(source.ToArray());
        PreviousWordParity = previous;
        _sourceWordParity = previous;
        buffer = buffer[RawGpsProtocol.PacketSize..];
    }

    /// <summary>Writes four zero padding bits and 300 transmitted bits, recomputing parity after typed edits.</summary>
    public void Serialize(ref Span<byte> buffer)
    {
        if (buffer.Length < RawGpsProtocol.PacketSize)
            throw new ArgumentException("GPS LNAV requires 38 bytes.", nameof(buffer));
        Span<byte> source = stackalloc byte[30];
        _sourceData.CopyTo(source);
        source[0] = GpsRawHelper.GpsSubframePreamble;
        RawGpsCodec.Unsigned(source, 8, 14, TelemetryMessage);
        RawGpsCodec.Unsigned(source, 22, 1, IntegrityStatusFlag ? 1 : 0);
        RawGpsCodec.Unsigned(source, 24, 17, TOW1_5Epoh);
        RawGpsCodec.Unsigned(source, 41, 1, AlertFlag ? 1 : 0);
        RawGpsCodec.Unsigned(source, 42, 1, AntiSpoofFlag ? 1 : 0);
        RawGpsCodec.Unsigned(source, 43, 3, SubframeId);
        SerializeFields(source);
        RawGpsCodec.Encode(
            source,
            buffer[..RawGpsProtocol.PacketSize],
            PreviousWordParity,
            _pagePayloadChanged
                || !source.SequenceEqual(_sourceData)
                || PreviousWordParity != _sourceWordParity
        );
        buffer = buffer[RawGpsProtocol.PacketSize..];
    }

    protected virtual void SerializeFields(Span<byte> source) { }

    /// <summary>Retains words 3–10 when a page is reinterpreted, including fields that become reserved.</summary>
    protected void RetainPagePayload(ReadOnlySpan<byte> payload)
    {
        if (payload.Length != 24)
            throw new ArgumentException(
                "A GPS page payload must contain 24 bytes.",
                nameof(payload)
            );
        _pagePayloadChanged |= !payload.SequenceEqual(_sourceData.AsSpan(6));
        payload.CopyTo(_sourceData.AsSpan(6));
    }

    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        UInt16Type.Accept(visitor, TelemetryMessageField, ref _telemetryMessage);
        BoolType.Accept(visitor, IntegrityStatusFlagField, ref _integrityStatusFlag);
        BoolType.Accept(visitor, AlertFlagField, ref _alertFlag);
        BoolType.Accept(visitor, AntiSpoofFlagField, ref _antiSpoofFlag);
        UInt32Type.Accept(visitor, TowField, TowField.DataType, ref _tOW1_5Epoh);
        UInt8Type.Accept(
            visitor,
            PreviousParityField,
            PreviousParityField.DataType,
            ref _previousWordParity
        );
    }
}
