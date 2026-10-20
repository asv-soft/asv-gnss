using System;
using Asv.IO;

namespace Asv.Gnss;

/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.8. Receiver and firmware support must be checked against the applicable interface description.</remarks>
public abstract partial class UbxAckBase : UbxMessageBase
{
    private byte _ackClassId;
    private byte _ackSubclassId;

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        AckClassId = BinSerialize.ReadByte(ref buffer);
        AckSubclassId = BinSerialize.ReadByte(ref buffer);
    }

    protected override void SerializeContent(ref Span<byte> buffer)
    {
        BinSerialize.WriteByte(ref buffer, AckClassId);
        BinSerialize.WriteByte(ref buffer, AckSubclassId);
    }

    protected override int GetContentByteSize() => 2;
    /// <summary>
    /// Class ID of the Acknowledged Message
    /// </summary>
    public byte AckClassId
    {
        get => _ackClassId;
        set => _ackClassId = value;
    }
    /// <summary>
    /// Message ID of the Acknowledged Message
    /// </summary>
    public byte AckSubclassId
    {
        get => _ackSubclassId;
        set => _ackSubclassId = value;
    }

    public override void Randomize(Random random)
    {
        AckClassId = (byte)random.Next(0, byte.MaxValue);
        AckSubclassId = (byte)random.Next(0, byte.MaxValue);
    }
}