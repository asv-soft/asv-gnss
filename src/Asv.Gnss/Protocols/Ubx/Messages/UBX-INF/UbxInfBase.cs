using System;
using System.Text;
using Asv.IO;

namespace Asv.Gnss;

/// <remarks>Payload fields follow u-blox document UBX-13003221 R23, section 32.13. Receiver and firmware support must be checked against the applicable interface description.</remarks>
public abstract partial class UbxInfBase : UbxMessageBase
{
    private string? _message;

    protected override void SerializeContent(ref Span<byte> buffer)
    {
        BinSerialize.WriteBlock(ref buffer, Encoding.ASCII.GetBytes(Message+"\0"));
    }

    protected override int GetContentByteSize() => Encoding.ASCII.GetByteCount(Message) + 1 /* '\0' char at the end*/;
        

    protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer)
    {
        Message = buffer.Length == 0 ? string.Empty : buffer.GetString(Encoding.ASCII).Trim('\0');
        buffer = buffer.Slice(buffer.Length);
    }

    /// <summary>
    /// Receiver diagnostic text from the variable-length INF payload.
    /// </summary>
    public string Message
    {
        get => _message!;
        set => _message = value;
    }

    public override void Randomize(Random random)
    {
        Message = $"Test message {random.Next()}";
    }
}