using System;

namespace Asv.Gnss;

public partial class RtcmV2Message21 : RtcmV2MessageBase
{
    public const int RtcmMessageId = 21;

    public override ushort MessageId => RtcmMessageId;
    public override string Name => "RTK/High Accuracy Pseudorange Corrections (Fixed*)";

    /// <summary>
    /// Unmodeled payload bytes, excluding the two header words and transport parity.
    /// The payload must consist of complete three-byte data words, up to 31 words.
    /// </summary>
    public byte[] RawData { get; set; } = [];

    /// <inheritdoc />
    protected override int GetContentBitSize()
    {
        ArgumentNullException.ThrowIfNull(RawData);
        if (RawData.Length > 93 || RawData.Length % 3 != 0)
        {
            throw new ArgumentException("The raw payload must contain at most 31 complete three-byte data words.", nameof(RawData));
        }

        return RawData.Length * 8;
    }

    /// <inheritdoc />
    protected override void SerializeContent(Span<byte> buffer, ref int bitIndex)
    {
        var bitSize = GetContentBitSize();
        RawData.CopyTo(buffer[(bitIndex / 8)..]);
        bitIndex += bitSize;
    }

    protected override void DeserializeContent(ReadOnlySpan<byte> buffer, ref int bitIndex, byte payloadLength)
    {
        RawData = buffer.Slice(bitIndex / 8, payloadLength).ToArray();
        bitIndex += payloadLength * 8;
    }
}
