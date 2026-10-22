using System;
using System.Collections.Immutable;
using System.Linq;

namespace Asv.Gnss;

/// <summary>[TD] TextData; GREIS 4.2.01 section 3.4.18.</summary>
public sealed partial class JavadTextDataMessage() : JavadTextMessage("CGGTTS", "{}")
{
    public override string Id => "TD";
    public override string Name => "CGGTTS";
    public string MessageType
    {
        get => Title;
        set => SetTitle(value);
    }
    public ImmutableArray<JavadValue> Data
    {
        get => Fields;
        set => Fields = value;
    }

    protected override bool IsValidTitle(string title) => title.Length != 0;

    /// <summary>Returns CGGTTS header name/value pairs while retaining their original field spelling in Data.</summary>
    public ImmutableDictionary<string, string> CggttsHeaders
    {
        get
        {
            var result = ImmutableDictionary.CreateBuilder<string, string>(StringComparer.Ordinal);
            if (MessageType != "CGGTTS")
                return result.ToImmutable();
            foreach (var value in JavadTextFields.List(Field(0)))
            {
                var text = JavadTextFields.Scalar(value) ?? "";
                var separator = text.IndexOf('=');
                if (separator < 1)
                    throw new FormatException("A CGGTTS header must contain NAME=VALUE.");
                result.Add(text[..separator], text[(separator + 1)..]);
            }
            return result.ToImmutable();
        }
    }

    protected override void ValidateFields()
    {
        if (MessageType == "CGGTTS")
        {
            Require(1);
            _ = JavadTextFields.List(Field(0));
        }
    }
}
