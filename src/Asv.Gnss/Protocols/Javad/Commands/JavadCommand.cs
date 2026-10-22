using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>An outbound GREIS statement, as defined in GREIS 4.2.01 sections 2.2 and 2.3.</summary>
public sealed class JavadCommand : IProtocolMessage<string>, IVisitable
{
    private static readonly Field LabelField = JavadVisitor.Field(
        nameof(Label),
        new StringOptionalType(EncodingId.Ascii, 0, int.MaxValue),
        "Optional GREIS statement identifier."
    );
    private static readonly Field HasChecksumField = JavadVisitor.Field(
        nameof(HasChecksum),
        BoolType.Default,
        "Whether to append the GREIS statement checksum."
    );
    private ProtocolTags _tags = [];
    private string _statement = string.Empty;
    private string _commandText = string.Empty;
    private JavadValue? _visitorValue;
    private StructType? _visitorType;

    public JavadCommand() { }

    public JavadCommand(string commandName, params JavadValue[] arguments)
    {
        ArgumentNullException.ThrowIfNull(commandName);
        ArgumentNullException.ThrowIfNull(arguments);
        if (!JavadCommandCatalog.All.Any(x => x.Name == commandName))
            throw new ArgumentException("Unknown GREIS command name.", nameof(commandName));
        foreach (var argument in arguments)
        {
            ArgumentNullException.ThrowIfNull(argument);
            argument.ValidateForCommand();
        }
        CommandName = commandName;
        Arguments = [.. arguments];
        CommandValue = JavadValue.List([JavadValue.Atom(commandName), .. arguments]);
        _commandText = CommandValue.ToString()[1..^1];
        _statement = FormatStatement();
    }

    public string CommandName { get; private set; } = string.Empty;
    public ImmutableArray<JavadValue> Arguments { get; private set; } = [];
    public JavadValue CommandValue { get; private set; } = JavadValue.List();
    public string? Label { get; private set; }
    public bool HasChecksum { get; private set; }
    public string Id => CommandName;
    public string Name =>
        CommandName.Length == 0 ? "GREIS synchronization" : "GREIS " + CommandName;
    public ProtocolInfo Protocol => JavadProtocol.Info;
    public ref ProtocolTags Tags => ref _tags;

    /// <summary>Cached metadata for the statement's current immutable argument tree.</summary>
    public StructType VisitorType
    {
        get
        {
            if (ReferenceEquals(_visitorValue, CommandValue))
                return _visitorType!;
            _visitorType = new StructType([
                LabelField,
                HasChecksumField,
                JavadVisitor.Field(
                    nameof(CommandValue),
                    JavadValueVisitor.GetType(CommandValue),
                    "Command name, arguments and statement options."
                ),
            ]);
            _visitorValue = CommandValue;
            return _visitorType;
        }
    }

    /// <summary>Applies validated edits atomically. Inspection preserves the original wire spelling.</summary>
    public void Accept(IVisitor visitor)
    {
        var replacement = VisitCopy(visitor);
        if (!ReferenceEquals(replacement, this))
            CopyStateFrom(replacement);
    }

    internal JavadCommand VisitCopy(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        var type = VisitorType;
        var label = Label;
        StringOptionalType.Accept(visitor, LabelField, ref label);
        var checksum = HasChecksum;
        BoolType.Accept(visitor, HasChecksumField, ref checksum);
        var value = CommandValue;
        JavadValueVisitor.Visit(visitor, type[2], ref value);
        if (label == Label && checksum == HasChecksum && ReferenceEquals(value, CommandValue))
            return this;
        var replacement = this;
        if (!ReferenceEquals(value, CommandValue))
        {
            replacement = Parse(value.ToString());
            replacement.CommandValue = value;
            replacement.Arguments = [.. value.Items.Skip(1)];
        }
        replacement = replacement.WithLabel(label).WithChecksum(checksum);
        for (var index = 0; index < _tags.Count; index++)
            replacement._tags.Add(_tags[index]);
        return replacement;
    }

    private void CopyStateFrom(JavadCommand source)
    {
        if (!ReferenceEquals(CommandValue, source.CommandValue))
        {
            _visitorValue = null;
            _visitorType = null;
        }
        CommandName = source.CommandName;
        Arguments = source.Arguments;
        CommandValue = source.CommandValue;
        _commandText = source._commandText;
        Label = source.Label;
        HasChecksum = source.HasChecksum;
        _statement = source._statement;
    }

    public string GetIdAsString() => Id;

    public int GetByteSize() => _statement.Length + 2;

    public override string ToString() => _statement;

    /// <summary>Creates a synchronization statement that always elicits a reply.</summary>
    public static JavadCommand Synchronize(string label = "") =>
        new JavadCommand().WithLabel(label);

    public JavadCommand WithLabel(string? label)
    {
        if (label is not null)
        {
            JavadValue.ValidateAscii(label);
            if (
                label.IndexOfAny(['%', '#', ';', '@']) >= 0
                || label.Contains("&&", StringComparison.Ordinal)
                || label.Contains("||", StringComparison.Ordinal)
            )
                throw new ArgumentException(
                    "The label contains statement delimiters.",
                    nameof(label)
                );
        }
        return Copy(label, HasChecksum);
    }

    public JavadCommand WithChecksum(bool enabled = true) => Copy(Label, enabled);

    private JavadCommand Copy(string? label, bool checksum)
    {
        var result = new JavadCommand
        {
            CommandName = CommandName,
            Arguments = Arguments,
            CommandValue = CommandValue,
            _commandText = _commandText,
            Label = label,
            HasChecksum = checksum,
        };
        result._statement = result.FormatStatement();
        return result;
    }

    private string FormatStatement()
    {
        var builder = new StringBuilder();
        if (Label is not null)
            builder.Append('%').Append(Label).Append('%');
        builder.Append(_commandText);
        if (HasChecksum)
        {
            builder.Append('@');
            builder.Append(
                JavadChecksum
                    .Compute8(Encoding.ASCII.GetBytes(builder.ToString()))
                    .ToString("X2", CultureInfo.InvariantCulture)
            );
        }
        return builder.ToString();
    }

    public void Serialize(ref Span<byte> buffer)
    {
        var size = GetByteSize();
        if (buffer.Length < size)
            throw new ArgumentException("The buffer is too small.", nameof(buffer));
        Encoding.ASCII.GetBytes(_statement, buffer);
        buffer[_statement.Length] = 13;
        buffer[_statement.Length + 1] = 10;
        buffer = buffer[size..];
    }

    /// <summary>Consumes one CR- or LF-terminated statement, accepting CRLF as a single terminator.</summary>
    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var end = buffer.IndexOfAny((byte)13, (byte)10);
        if (end < 0)
            throw new FormatException("A GREIS command requires a line terminator.");
        foreach (var b in buffer[..end])
            if (b > 127)
                throw new FormatException("GREIS commands are ASCII.");
        var parsed = Parse(Encoding.ASCII.GetString(buffer[..end]));
        CopyStateFrom(parsed);
        var consumed = end + 1;
        if (buffer[end] == 13 && consumed < buffer.Length && buffer[consumed] == 10)
            consumed++;
        buffer = buffer[consumed..];
    }

    public static JavadCommand Parse(string statement)
    {
        ArgumentNullException.ThrowIfNull(statement);
        JavadValue.ValidateAscii(statement);
        var original = statement.Trim();
        var text = original;
        var checksumAt = FindOutsideQuotes(text, '@');
        var hasChecksum = checksumAt >= 0;
        if (hasChecksum)
        {
            var suffix = text[(checksumAt + 1)..].Trim();
            if (
                suffix.Length != 2
                || !byte.TryParse(
                    suffix,
                    NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture,
                    out var checksum
                )
            )
                throw new FormatException("Invalid GREIS command checksum field.");
            if (
                JavadChecksum.Compute8(Encoding.ASCII.GetBytes(text[..(checksumAt + 1)]))
                != checksum
            )
                throw new FormatException("GREIS command checksum mismatch.");
            text = text[..checksumAt].TrimEnd();
        }
        string? label = null;
        if (text.StartsWith('%'))
        {
            var end = text.IndexOf('%', 1);
            if (end < 0)
                throw new FormatException("Unclosed GREIS statement identifier.");
            label = text[1..end];
            text = text[(end + 1)..].TrimStart();
        }
        var result = new JavadCommand();
        if (text.Length != 0)
        {
            var values = JavadValue.Parse(text.StartsWith('{') ? text : "{" + text + "}");
            if (
                !values.IsList
                || values.Items.IsEmpty
                || values.Items[0].IsList
                || values.Items[0].Scalar is not { } name
            )
                throw new FormatException("A GREIS command must start with a command name.");
            if (values.Prefix.Length != 0 || values.Name is not null)
                throw new FormatException("Invalid GREIS command list.");
            var arguments = values.Items.Skip(1).ToArray();
            result = new JavadCommand(name, arguments);
            result.CommandValue = values;
            result._commandText = text;
        }
        result = result.WithLabel(label).WithChecksum(hasChecksum);
        // Preserve the original byte spelling: whitespace and list braces participate in the checksum.
        result._statement = original;
        return result;
    }

    internal static int FindOutsideQuotes(string text, char target)
    {
        var quoted = false;
        var label = false;
        for (var i = 0; i < text.Length; i++)
        {
            if (quoted && text[i] == '\\')
            {
                i++;
                continue;
            }
            if (!quoted && text[i] == '%')
            {
                label = !label;
                continue;
            }
            if (label)
                continue;
            if (text[i] == '"')
                quoted = !quoted;
            else if (!quoted && text[i] == target)
                return i;
        }
        return -1;
    }

    public static JavadCommand Set(JavadObjectPath path, JavadValue value) =>
        new("set", path.ToValue(), value);

    public static JavadCommand Print(JavadObjectPath path, bool names = false) =>
        new("print", names ? path.ToValue().WithOptions(JavadValue.Boolean(true)) : path.ToValue());

    public static JavadCommand List(JavadObjectPath? path = null) =>
        path.HasValue ? new("list", path.Value.ToValue()) : new("list");

    public static JavadCommand EnableMessages(
        JavadValue messages,
        JavadObjectPath? target = null
    ) => new("em", target?.ToValue() ?? JavadValue.Omitted, messages);

    public static JavadCommand OutputMessages(
        JavadValue messages,
        JavadObjectPath? target = null
    ) => new("out", target?.ToValue() ?? JavadValue.Omitted, messages);

    public static JavadCommand DisableMessages(
        JavadObjectPath? target = null,
        JavadValue? messages = null
    ) =>
        messages is not null ? new("dm", target?.ToValue() ?? JavadValue.Omitted, messages)
        : target.HasValue ? new("dm", target.Value.ToValue())
        : new("dm");

    public static JavadCommand Initialize(JavadObjectPath path) => new("init", path.ToValue());

    public static JavadCommand Create(JavadObjectPath? path = null, string? log = null)
    {
        var value = path?.ToValue() ?? JavadValue.Omitted;
        return log is null
            ? path.HasValue
                ? new("create", value)
                : new("create")
            : new("create", value.WithOptions(JavadValue.Atom(log)));
    }

    public static JavadCommand Remove(JavadObjectPath path) => new("remove", path.ToValue());

    public static JavadCommand Event(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (text.Length > 63)
            throw new ArgumentOutOfRangeException(
                nameof(text),
                "GREIS event strings contain at most 63 characters."
            );
        return new("event", JavadValue.String(text));
    }

    public static JavadCommand Get(
        JavadObjectPath path,
        ulong? offset = null,
        JavadValue? options = null
    ) => Transfer("get", path, offset, options);

    public static JavadCommand Put(
        JavadObjectPath path,
        ulong? offset = null,
        JavadValue? options = null
    ) => Transfer("put", path, offset, options);

    public static JavadCommand FirmwareLoad(
        string receiverId,
        JavadObjectPath source,
        JavadValue? options = null
    ) =>
        new(
            "fld",
            JavadValue.String(receiverId),
            options is null ? source.ToValue() : source.ToValue().WithOptions(options)
        );

    private static JavadCommand Transfer(
        string name,
        JavadObjectPath path,
        ulong? offset,
        JavadValue? options
    )
    {
        var value = options is null ? path.ToValue() : path.ToValue().WithOptions(options);
        return offset.HasValue
            ? new(name, value, JavadValue.UnsignedInteger(offset.Value))
            : new(name, value);
    }
}

public sealed record JavadCommandDescriptor(
    string Name,
    string Syntax,
    string Options,
    int PdfPage
);

public static class JavadCommandCatalog
{
    public static ImmutableArray<JavadCommandDescriptor> All { get; } =
    [
        new("set", "set,object,value", "", 32),
        new("print", "print,object", "{names}", 33),
        new("list", "list[,object]", "", 35),
        new("em", "em,[target],messages", "{period,phase,count,flags}", 37),
        new("out", "out,[target],messages", "{period,phase,count,flags}", 37),
        new("dm", "dm[,[target][,messages]]", "", 41),
        new("init", "init,object[/]", "", 43),
        new("create", "create[,object]", "{log}", 45),
        new("remove", "remove,object[/]", "", 48),
        new("event", "event,string", "", 50),
        new("get", "get,object[,offset]", "{timeout,block_size,period,phase,attempts}", 52),
        new("put", "put,object[,offset]", "{timeout,block_size}", 56),
        new("fld", "fld,id,object", "{timeout,block_size}", 58),
    ];
}
