using System;
using System.Globalization;

namespace Asv.Gnss;

/// <summary>A single RE or ER payload. A print/list response can span several such messages.</summary>
public sealed record JavadCommandResponse(bool IsError, string? Label, string Text)
{
    public static JavadCommandResponse Parse(string messageId, string payload)
    {
        if (messageId is not ("RE" or "ER"))
            throw new ArgumentException("A command response must be RE or ER.", nameof(messageId));
        ArgumentNullException.ThrowIfNull(payload);
        var text = payload.AsSpan();
        string? label = null;
        if (text.StartsWith("%", StringComparison.Ordinal))
        {
            var end = text[1..].IndexOf('%');
            if (end < 0)
                throw new FormatException("Unclosed GREIS reply identifier.");
            label = text.Slice(1, end).ToString();
            text = text[(end + 2)..];
        }
        return new(messageId == "ER", label, text.ToString());
    }

    /// <summary>Matches only an explicit statement identifier; unlabelled replies are ambiguous.</summary>
    public bool Matches(JavadCommand command) =>
        command.Label is not null && Label == command.Label;

    public bool TryGetValue(out JavadValue? value)
    {
        try
        {
            value = JavadValue.Parse(Text);
            return true;
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException)
        {
            value = null;
            return false;
        }
    }

    public int? ErrorCode
    {
        get
        {
            if (
                !IsError
                || !TryGetValue(out var value)
                || value is not { IsList: true }
                || value.Items.IsEmpty
            )
                return null;
            return int.TryParse(
                value.Items[0].Scalar,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var code
            )
                ? code
                : null;
        }
    }
}
