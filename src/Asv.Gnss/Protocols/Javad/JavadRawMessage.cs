using System;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>An explicitly selected raw payload for vendor extensions or forwarding.</summary>
public sealed class JavadRawMessage : JavadMessageBase
{
    private byte[] _payload = [];
    private static readonly Field PayloadField = JavadVisitor.Field(
        nameof(Payload),
        new ListType(UInt8Type.Default, 0, JavadProtocol.MaxBodySize),
        "Raw payload bytes excluding the checksum."
    );

    public static StructType StructType { get; } = new([ExtensionDataField, PayloadField]);

    public JavadRawMessage(string id, JavadChecksumKind checksumKind = JavadChecksumKind.None)
    {
        JavadProtocol.ValidateIdentifier(id);
        Id = id;
        ChecksumKind = checksumKind;
    }

    public override string Id { get; }
    public override string Name => "Raw GREIS message";
    public override JavadChecksumKind ChecksumKind { get; }
    public byte[] Payload
    {
        get => _payload;
        set => _payload = value;
    }

    public override void Accept(IVisitor visitor)
    {
        base.Accept(visitor);
        JavadVisitor.VisitArray(
            visitor,
            PayloadField,
            ref _payload,
            static () => (byte)0,
            static (IVisitor v, Field f, IFieldType t, ref byte value) =>
                UInt8Type.Accept(v, f, t, ref value)
        );
    }

    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        Payload = buffer.ToArray();
        buffer = [];
    }

    protected override void SerializePayload(ref Span<byte> buffer) =>
        JavadBinary.WriteBytes(ref buffer, Payload);

    protected override int GetPayloadByteSize() => Payload.Length;
}
