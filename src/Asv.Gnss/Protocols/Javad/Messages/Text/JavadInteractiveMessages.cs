using System;
using System.Text;

namespace Asv.Gnss;

/// <summary>The unchecksummed ASCII body of a RE or ER response, including partial replies.</summary>
public abstract partial class JavadResponseMessage : JavadMessageBase
{
    private string _text = "";
    public string Text
    {
        get => _text;
        set => _text = value;
    }
    public override JavadChecksumKind ChecksumKind => JavadChecksumKind.None;
    public JavadCommandResponse Response => JavadCommandResponse.Parse(Id, Text);

    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        Text = ReadAscii(buffer);
        buffer = buffer[buffer.Length..];
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        ValidateAscii(Text);
        Encoding.ASCII.GetBytes(Text, buffer);
        buffer = buffer[Text.Length..];
    }

    protected override int GetPayloadByteSize()
    {
        ValidateAscii(Text);
        return Text.Length;
    }

    internal static string ReadAscii(ReadOnlySpan<byte> buffer)
    {
        foreach (var value in buffer)
            if (value > 127)
                throw new FormatException("A GREIS ASCII field contains a non-ASCII byte.");
        return Encoding.ASCII.GetString(buffer);
    }

    internal static void ValidateAscii(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        foreach (var c in value)
            if (c > 127)
                throw new ArgumentException(
                    "A GREIS ASCII field contains a non-ASCII character.",
                    nameof(value)
                );
    }
}

/// <summary>[RE] Reply, GREIS section 3.4.16. An empty reply is valid.</summary>
public sealed partial class JavadReplyMessage : JavadResponseMessage
{
    public override string Id => "RE";
    public override string Name => "Reply";
}

/// <summary>[ER] Error, GREIS section 3.4.16.</summary>
public sealed partial class JavadErrorMessage : JavadResponseMessage
{
    public override string Id => "ER";
    public override string Name => "Error";
}

/// <summary>[PM] Parameter descriptions. A receiver may split the full parameter tree across messages.</summary>
public sealed partial class JavadParametersMessage : JavadMessageBase
{
    private bool _checksumDelimiterIncludesComma = true;

    public override string Id => "PM";
    public override string Name => "Parameters";
    public override JavadChecksumKind ChecksumKind => JavadChecksumKind.AsciiHex8;
    public string Parameters { get; set; } = "{}";

    public bool TryGetValue(out JavadValue? value)
    {
        try
        {
            value = JavadValue.Parse(Parameters);
            return true;
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException)
        {
            value = null;
            return false;
        }
    }

    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        var text = JavadResponseMessage.ReadAscii(buffer);
        if (!text.EndsWith('@'))
            throw new FormatException("A GREIS PM message must end with @ before its checksum.");
        // GREIS 4.2.01 specifies ,@; recorded receiver streams also use a bare @.
        _checksumDelimiterIncludesComma = text.EndsWith(",@", StringComparison.Ordinal);
        Parameters = text[..^(_checksumDelimiterIncludesComma ? 2 : 1)];
        buffer = buffer[buffer.Length..];
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        JavadResponseMessage.ValidateAscii(Parameters);
        Encoding.ASCII.GetBytes(Parameters, buffer);
        var offset = Parameters.Length;
        if (_checksumDelimiterIncludesComma)
            buffer[offset++] = (byte)',';
        buffer[offset++] = (byte)'@';
        buffer = buffer[offset..];
    }

    protected override int GetPayloadByteSize()
    {
        JavadResponseMessage.ValidateAscii(Parameters);
        return checked(Parameters.Length + (_checksumDelimiterIncludesComma ? 2 : 1));
    }
}

/// <summary>[&gt;&gt;] Arbitrary data wrapper, GREIS section 3.4.17.</summary>
public sealed partial class JavadWrapperMessage : JavadMessageBase
{
    public override string Id => ">>";
    public override string Name => "Wrapper";
    public override JavadChecksumKind ChecksumKind => JavadChecksumKind.AsciiHex8;
    private byte _sourceId;
    public byte SourceId
    {
        get => _sourceId;
        set => _sourceId = value;
    }
    private byte[] _data = [];
    public byte[] Data
    {
        get => _data;
        set => _data = value;
    }

    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        SourceId = JavadBinary.ReadU1(ref buffer);
        Data = buffer.ToArray();
        buffer = buffer[buffer.Length..];
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        JavadBinary.WriteU1(ref buffer, SourceId);
        Data.CopyTo(buffer);
        buffer = buffer[Data.Length..];
    }

    protected override int GetPayloadByteSize()
    {
        ArgumentNullException.ThrowIfNull(Data);
        return checked(1 + Data.Length);
    }
}

/// <summary>[==] Event, GREIS section 3.4.17. Time is receiver milliseconds modulo one day.</summary>
public sealed partial class JavadEventMessage : JavadMessageBase
{
    public override string Id => "==";
    public override string Name => "Event";
    public override JavadChecksumKind ChecksumKind => JavadChecksumKind.Binary8;
    private uint _receiverTimeMilliseconds;
    public uint ReceiverTimeMilliseconds
    {
        get => _receiverTimeMilliseconds;
        set => _receiverTimeMilliseconds = value;
    }
    private byte _eventType;
    public byte EventType
    {
        get => _eventType;
        set => _eventType = value;
    }
    private byte[] _data = [];
    public byte[] Data
    {
        get => _data;
        set => _data = value;
    }

    protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)
    {
        ReceiverTimeMilliseconds = JavadBinary.ReadU4(ref buffer, ByteOrder);
        EventType = JavadBinary.ReadU1(ref buffer);
        Data = buffer.ToArray();
        buffer = buffer[buffer.Length..];
    }

    protected override void SerializePayload(ref Span<byte> buffer)
    {
        JavadBinary.WriteU4(ref buffer, ReceiverTimeMilliseconds, ByteOrder);
        JavadBinary.WriteU1(ref buffer, EventType);
        Data.CopyTo(buffer);
        buffer = buffer[Data.Length..];
    }

    protected override int GetPayloadByteSize()
    {
        ArgumentNullException.ThrowIfNull(Data);
        return checked(5 + Data.Length);
    }
}
