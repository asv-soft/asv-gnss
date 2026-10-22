using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using Asv.IO;

namespace Asv.Gnss;

public enum JavadStatementSeparator
{
    Always,
    OnSuccess,
    OnFailure,
}

/// <summary>A GREIS line with sequential, success-conditional or failure-conditional statements.</summary>
public sealed class JavadCommandLine : IProtocolMessage<string>, IVisitable
{
    private ProtocolTags _tags = [];
    private string _line = string.Empty;
    private StructType? _visitorType;
    private ImmutableArray<string> _statementWire = [];
    public ImmutableArray<JavadCommand> Statements { get; private set; } = [];
    public ImmutableArray<JavadStatementSeparator> Separators { get; private set; } = [];
    public string Id => "command-line";
    public string Name => "GREIS command line";
    public ProtocolInfo Protocol => JavadProtocol.Info;
    public ref ProtocolTags Tags => ref _tags;

    /// <summary>Cached, finite metadata for the statements and conditional separators.</summary>
    public StructType VisitorType
    {
        get
        {
            if (_visitorType is { } cached)
            {
                var statementType = (StructType)cached[0].DataType;
                var matches =
                    statementType.Fields.Length == Statements.Length
                    && ((StructType)cached[1].DataType).Fields.Length == Separators.Length;
                for (var index = 0; matches && index < Statements.Length; index++)
                    matches = ReferenceEquals(
                        statementType[index].DataType,
                        Statements[index].VisitorType
                    );
                if (matches)
                    return cached;
            }
            var statements = ImmutableArray.CreateBuilder<Field>(Statements.Length);
            for (var index = 0; index < Statements.Length; index++)
                statements.Add(
                    JavadVisitor.Field(
                        index.ToString(CultureInfo.InvariantCulture),
                        Statements[index].VisitorType,
                        "GREIS statement."
                    )
                );
            var separators = ImmutableArray.CreateBuilder<Field>(Separators.Length);
            for (var index = 0; index < Separators.Length; index++)
                separators.Add(
                    JavadVisitor.Field(
                        index.ToString(CultureInfo.InvariantCulture),
                        Int32Type.Default,
                        "Statement condition: 0 always, 1 on success, 2 on failure."
                    )
                );
            return _visitorType = new StructType([
                JavadVisitor.Field(
                    nameof(Statements),
                    new StructType(statements.MoveToImmutable()),
                    "Ordered GREIS statements."
                ),
                JavadVisitor.Field(
                    nameof(Separators),
                    new StructType(separators.MoveToImmutable()),
                    "Conditions between adjacent statements."
                ),
            ]);
        }
    }

    /// <summary>Visits statements and conditions; failed edits leave the entire line unchanged.</summary>
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);
        EnsureCurrentLine();
        var type = VisitorType;
        if (visitor is not StructType.IVisitor nested)
        {
            visitor.VisitUnknown(type[0], type[0].DataType);
            visitor.VisitUnknown(type[1], type[1].DataType);
            return;
        }
        JavadCommand[]? statements = null;
        var statementType = (StructType)type[0].DataType;
        nested.BeginStruct(type[0], statementType);
        for (var index = 0; index < Statements.Length; index++)
        {
            nested.BeginStruct(statementType[index], (StructType)statementType[index].DataType);
            var edited = Statements[index].VisitCopy(visitor);
            nested.EndStruct();
            if (ReferenceEquals(edited, Statements[index]))
                continue;
            statements ??= Statements.ToArray();
            statements[index] = edited;
        }
        nested.EndStruct();
        JavadStatementSeparator[]? separators = null;
        var separatorType = (StructType)type[1].DataType;
        nested.BeginStruct(type[1], separatorType);
        for (var index = 0; index < Separators.Length; index++)
        {
            var edited = (int)Separators[index];
            Int32Type.Accept(visitor, separatorType[index], ref edited);
            if (edited is < 0 or > 2)
                throw new ArgumentOutOfRangeException(
                    nameof(visitor),
                    "Invalid GREIS statement condition."
                );
            if (edited == (int)Separators[index])
                continue;
            separators ??= Separators.ToArray();
            separators[index] = (JavadStatementSeparator)edited;
        }
        nested.EndStruct();
        if (statements is null && separators is null)
            return;
        var editedStatements = statements is null ? Statements : [.. statements];
        var editedSeparators = separators is null ? Separators : [.. separators];
        var validated = Parse(FormatLine(editedStatements, editedSeparators));
        Statements = editedStatements;
        Separators = editedSeparators;
        _line = validated._line;
        RememberStatementWire();
    }

    private string FormatLine(
        ImmutableArray<JavadCommand> statements,
        ImmutableArray<JavadStatementSeparator> separators
    )
    {
        var text = new StringBuilder();
        for (var index = 0; index < statements.Length; index++)
        {
            if (index != 0)
                text.Append(
                    separators[index - 1] switch
                    {
                        JavadStatementSeparator.Always => ";",
                        JavadStatementSeparator.OnSuccess => "&&",
                        _ => "||",
                    }
                );
            text.Append(statements[index]);
        }
        var commentAt = JavadCommand.FindOutsideQuotes(_line, '#');
        if (commentAt >= 0)
            text.Append(' ').Append(_line.AsSpan(commentAt));
        return text.ToString();
    }

    private void RememberStatementWire()
    {
        var snapshot = ImmutableArray.CreateBuilder<string>(Statements.Length);
        foreach (var statement in Statements)
            snapshot.Add(statement.ToString());
        _statementWire = snapshot.MoveToImmutable();
    }

    private void EnsureCurrentLine()
    {
        var changed = _statementWire.Length != Statements.Length;
        for (var index = 0; !changed && index < Statements.Length; index++)
            changed = _statementWire[index] != Statements[index].ToString();
        if (!changed)
            return;
        _line = FormatLine(Statements, Separators);
        RememberStatementWire();
    }

    public string GetIdAsString() => Id;

    public int GetByteSize()
    {
        EnsureCurrentLine();
        return _line.Length + 2;
    }

    public override string ToString()
    {
        EnsureCurrentLine();
        return _line;
    }

    public JavadCommandLine() { }

    public JavadCommandLine(JavadCommand first)
    {
        ArgumentNullException.ThrowIfNull(first);
        Statements = [first];
        _line = first.ToString();
        _statementWire = [_line];
    }

    public JavadCommandLine Then(
        JavadCommand next,
        JavadStatementSeparator separator = JavadStatementSeparator.Always
    )
    {
        ArgumentNullException.ThrowIfNull(next);
        var delimiter = separator switch
        {
            JavadStatementSeparator.Always => ";",
            JavadStatementSeparator.OnSuccess => "&&",
            JavadStatementSeparator.OnFailure => "||",
            _ => throw new ArgumentOutOfRangeException(nameof(separator)),
        };
        var prefix = new StringBuilder();
        for (var i = 0; i < Statements.Length; i++)
        {
            if (i != 0)
                prefix.Append(
                    Separators[i - 1] switch
                    {
                        JavadStatementSeparator.Always => ";",
                        JavadStatementSeparator.OnSuccess => "&&",
                        _ => "||",
                    }
                );
            prefix.Append(Statements[i]);
        }
        var result = Statements.IsEmpty
            ? new JavadCommandLine(next)
            : new JavadCommandLine()
            {
                Statements = Statements.Add(next),
                Separators = Separators.Add(separator),
                _line = prefix.ToString() + delimiter + next,
            };
        result.RememberStatementWire();
        return result;
    }

    public static JavadCommandLine Parse(string line)
    {
        ArgumentNullException.ThrowIfNull(line);
        JavadValue.ValidateAscii(line);
        var statements = new List<JavadCommand>();
        var separators = new List<JavadStatementSeparator>();
        var quoted = false;
        var label = false;
        var begin = 0;
        var end = line.Length;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (quoted && c == '\\')
            {
                i++;
                continue;
            }
            if (c == '"' && !label)
            {
                quoted = !quoted;
                continue;
            }
            if (quoted)
                continue;
            if (c == '%')
            {
                label = !label;
                continue;
            }
            if (label)
                continue;
            if (c == '#')
            {
                end = i;
                break;
            }
            var width =
                c == ';' ? 1
                : (c is '&' or '|') && i + 1 < line.Length && line[i + 1] == c ? 2
                : 0;
            if (width == 0)
                continue;
            statements.Add(JavadCommand.Parse(line[begin..i]));
            separators.Add(
                c == ';' ? JavadStatementSeparator.Always
                : c == '&' ? JavadStatementSeparator.OnSuccess
                : JavadStatementSeparator.OnFailure
            );
            i += width - 1;
            begin = i + 1;
        }
        if (quoted || label)
            throw new FormatException("An input line contains an unclosed quoted string or label.");
        if (begin < end || statements.Count != 0)
            statements.Add(JavadCommand.Parse(line[begin..end]));
        return new()
        {
            Statements = [.. statements],
            Separators = [.. separators],
            _line = line,
            _statementWire = [.. statements.Select(statement => statement.ToString())],
        };
    }

    public void Serialize(ref Span<byte> buffer)
    {
        if (buffer.Length < GetByteSize())
            throw new ArgumentException("The buffer is too small.", nameof(buffer));
        Encoding.ASCII.GetBytes(_line, buffer);
        buffer[_line.Length] = 13;
        buffer[_line.Length + 1] = 10;
        buffer = buffer[GetByteSize()..];
    }

    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var end = buffer.IndexOfAny((byte)13, (byte)10);
        if (end < 0)
            throw new FormatException("A GREIS line requires a terminator.");
        foreach (var b in buffer[..end])
            if (b > 127)
                throw new FormatException("GREIS input is ASCII.");
        var parsed = Parse(Encoding.ASCII.GetString(buffer[..end]));
        Statements = parsed.Statements;
        Separators = parsed.Separators;
        _line = parsed._line;
        _statementWire = parsed._statementWire;
        var count = end + 1;
        if (buffer[end] == 13 && count < buffer.Length && buffer[count] == 10)
            count++;
        buffer = buffer[count..];
    }
}
