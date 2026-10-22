using System;
using Asv.IO;
using Newtonsoft.Json;

namespace Asv.Gnss;

public abstract partial class GlonassWordBase
{
    private ProtocolTags _tags = [];

    [JsonIgnore]
    public byte Id => WordId;

    [JsonIgnore]
    public string Name => $"GLONASS string {WordId}";

    [JsonIgnore]
    public ProtocolInfo Protocol => RawGlonassProtocol.Info;

    [JsonIgnore]
    public ref ProtocolTags Tags => ref _tags;

    public string GetIdAsString() => Id.ToString();

    public int GetByteSize() => RawGlonassProtocol.PacketSize;

    /// <summary>Reads a canonical string and strictly checks its eight Hamming bits.</summary>
    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        if (buffer.Length < RawGlonassProtocol.PacketSize)
            throw new FormatException("A GLONASS string requires eleven bytes.");
        var frame = buffer[..RawGlonassProtocol.PacketSize];
        if (frame[0] is < 1 or > 15)
            throw new FormatException("Invalid GLONASS idle bit, padding, or string number.");
        if (!GlonassWordChecksum.IsValid(frame))
            throw new ProtocolParserCrcException(Protocol);
        CheckWordId(frame[0]);
        // The established byte[] overload remains permissive for receiver payload integration.
        // Deserialize performs the virtual typed dispatch without allocating another frame array.
        frame.CopyTo(RawData);
        Deserialize(RawData);
        buffer = buffer[RawGlonassProtocol.PacketSize..];
    }

    /// <summary>Writes three zero padding bits followed by bits 85..1, including newly computed Hamming bits.</summary>
    public void Serialize(ref Span<byte> buffer)
    {
        if (WordId is < 1 or > 15)
            throw new InvalidOperationException(
                "A GLONASS string number must be between one and fifteen."
            );
        if (buffer.Length < RawGlonassProtocol.PacketSize)
            throw new ArgumentException(
                "The destination is too short for a GLONASS string.",
                nameof(buffer)
            );
        Span<byte> frame = stackalloc byte[RawGlonassProtocol.PacketSize];
        RawData.CopyTo(frame);
        frame[0] = WordId;
        SerializeContent(frame);
        frame[^1] = GlonassWordChecksum.Compute(frame);
        frame.CopyTo(buffer);
        buffer = buffer[RawGlonassProtocol.PacketSize..];
    }

    protected virtual void SerializeContent(Span<byte> buffer) { }

    protected void ChangeFrameNumber(ref byte frameNumber, byte value)
    {
        if (value > 5)
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "The frame number must be zero (unknown) or between one and five."
            );
        if ((frameNumber == 5) == (value == 5))
        {
            frameNumber = value;
            return;
        }

        Span<byte> payload = stackalloc byte[RawGlonassProtocol.PacketSize];
        RawData.CopyTo(payload);
        // Constructor defaults are retained for legacy callers and may not form a valid almanac yet.
        if (HasRawData)
            SerializeContent(payload);
        payload[0] = WordId;
        payload[^1] = GlonassWordChecksum.Compute(payload);
        frameNumber = value;
        payload.CopyTo(RawData);
        Deserialize(RawData);
    }

    /// <summary>Visits modeled payload fields. The raw received snapshot and immutable string ID are metadata.</summary>
    public virtual void Accept(IVisitor visitor) => ArgumentNullException.ThrowIfNull(visitor);
}
