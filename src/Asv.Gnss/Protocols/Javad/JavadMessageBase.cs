using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>A complete GREIS frame. Payload codecs use raw wire units and preserve extension bytes.</summary>
public abstract class JavadMessageBase : IProtocolMessage<string>, IVisitable
{
    private ProtocolTags _tags = [];
    private byte[] _extensionData = [];
    protected static readonly Field ExtensionDataField = JavadVisitor.Field(
        nameof(ExtensionData),
        new ListType(UInt8Type.Default, 0, JavadProtocol.MaxBodySize),
        "Unmodeled payload bytes before the checksum, retained for forward compatibility."
    );

    public abstract string Id { get; }
    public abstract string Name { get; }
    public abstract JavadChecksumKind ChecksumKind { get; }
    public JavadByteOrder ByteOrder { get; set; }
    public byte[] ExtensionData
    {
        get => _extensionData;
        set => _extensionData = value;
    }
    public ProtocolInfo Protocol => JavadProtocol.Info;
    public ref ProtocolTags Tags => ref _tags;

    public string GetIdAsString() => Id;

    /// <summary>Visits payload fields independently of the GREIS frame header and checksum.</summary>
    public virtual void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        JavadVisitor.VisitArray(
            visitor,
            ExtensionDataField,
            ref _extensionData,
            static () => (byte)0,
            static (IVisitor v, Field f, IFieldType t, ref byte value) =>
                UInt8Type.Accept(v, f, t, ref value)
        );
    }

    protected abstract void DeserializePayload(ref ReadOnlySpan<byte> buffer);
    protected abstract void SerializePayload(ref Span<byte> buffer);
    protected abstract int GetPayloadByteSize();

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        ValidateByteOrder();
        if (!JavadProtocol.TryReadBodyLength(buffer, out var bodySize))
        {
            throw new FormatException("Invalid GREIS header.");
        }
        JavadProtocol.ValidateIdentifier(Id);
        if (buffer[0] != Id[0] || buffer[1] != Id[1])
        {
            throw new FormatException($"Expected GREIS message {Id}.");
        }
        var size = JavadProtocol.HeaderSize + bodySize;
        var checksumSize = JavadChecksum.Size(ChecksumKind);
        ValidateBodySize(bodySize);
        if (buffer.Length < size || bodySize < checksumSize)
        {
            throw new FormatException("Truncated GREIS frame.");
        }
        var frame = buffer[..size];
        ValidateChecksum(frame, checksumSize);
        var payload = frame.Slice(JavadProtocol.HeaderSize, bodySize - checksumSize);
        DeserializePayload(ref payload);
        ExtensionData = payload.ToArray();
        buffer = buffer[size..];
    }

    public void Serialize(ref Span<byte> buffer)
    {
        ValidateByteOrder();
        var payloadSize = GetPayloadByteSize();
        var size = GetByteSize();
        if (buffer.Length < size)
        {
            throw new ArgumentException(
                "The destination is too short for the GREIS frame.",
                nameof(buffer)
            );
        }
        var frame = buffer[..size];
        JavadProtocol.WriteHeader(frame, Id, size - JavadProtocol.HeaderSize);
        var payload = frame.Slice(JavadProtocol.HeaderSize, payloadSize);
        SerializePayload(ref payload);
        if (!payload.IsEmpty)
        {
            throw new InvalidOperationException($"{Name} wrote fewer payload bytes than declared.");
        }
        ExtensionData.CopyTo(frame[(JavadProtocol.HeaderSize + payloadSize)..]);
        WriteChecksum(frame);
        buffer = buffer[size..];
    }

    public int GetByteSize()
    {
        JavadProtocol.ValidateIdentifier(Id);
        ArgumentNullException.ThrowIfNull(ExtensionData);
        var bodySize = checked(
            GetPayloadByteSize() + ExtensionData.Length + JavadChecksum.Size(ChecksumKind)
        );
        if (bodySize is < 0 or > JavadProtocol.MaxBodySize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bodySize),
                "A GREIS body cannot exceed 4095 bytes."
            );
        }
        ValidateBodySize(bodySize);
        return JavadProtocol.HeaderSize + bodySize;
    }

    private void ValidateBodySize(int size)
    {
        // These two messages are explicitly excluded from the extensible-layout rule.
        if ((Id == "JP" && size != 85) || (Id == "MF" && size != 9))
        {
            throw new FormatException($"GREIS {Id} has a fixed body size.");
        }
    }

    private void ValidateByteOrder()
    {
        if (ByteOrder is not (JavadByteOrder.LittleEndian or JavadByteOrder.BigEndian))
        {
            throw new ArgumentOutOfRangeException(nameof(ByteOrder));
        }
    }

    private void ValidateChecksum(ReadOnlySpan<byte> frame, int size)
    {
        var data = frame[..^size];
        var valid = ChecksumKind switch
        {
            JavadChecksumKind.None => true,
            JavadChecksumKind.Binary8 => JavadChecksum.Compute8(data) == frame[^1],
            JavadChecksumKind.AsciiHex8 => JavadProtocol.HexValue(frame[^2]) >= 0
                && JavadProtocol.HexValue(frame[^1]) >= 0
                && JavadChecksum.Compute8(data)
                    == (
                        (JavadProtocol.HexValue(frame[^2]) << 4) | JavadProtocol.HexValue(frame[^1])
                    ),
            JavadChecksumKind.Crc16 => JavadChecksum.Compute16(data) == ReadCrc16(frame[^2..]),
            _ => false,
        };
        if (!valid)
        {
            throw new ProtocolParserCrcException(Protocol);
        }
    }

    private ushort ReadCrc16(ReadOnlySpan<byte> data) => JavadBinary.ReadU2(ref data, ByteOrder);

    private void WriteChecksum(Span<byte> frame)
    {
        var size = JavadChecksum.Size(ChecksumKind);
        var data = frame[..^size];
        switch (ChecksumKind)
        {
            case JavadChecksumKind.None:
                return;
            case JavadChecksumKind.Binary8:
                frame[^1] = JavadChecksum.Compute8(data);
                break;
            case JavadChecksumKind.AsciiHex8:
                var checksum = JavadChecksum.Compute8(data);
                frame[^2] = JavadProtocol.HexDigit(checksum >> 4);
                frame[^1] = JavadProtocol.HexDigit(checksum & 15);
                break;
            case JavadChecksumKind.Crc16:
                var tail = frame[^2..];
                JavadBinary.WriteU2(ref tail, JavadChecksum.Compute16(data), ByteOrder);
                break;
        }
    }
}
