using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace Asv.Gnss;

/// <summary>A documented object declaration. Ranges and availability are firmware-dependent metadata.</summary>
public sealed record JavadParameterDescriptor(
    string PathTemplate,
    string Declaration,
    string Access,
    string Type,
    string Values,
    string Default,
    string Options,
    int PdfPage
)
{
    public bool CanRead => Access.Contains('r');
    public bool CanWrite => Access.Contains('w');

    /// <summary>Substitutes template identifiers such as X, SYS, N and [port] with actual object names.</summary>
    public JavadObjectPath Bind(IReadOnlyDictionary<string, string> substitutions)
    {
        ArgumentNullException.ThrowIfNull(substitutions);
        var result = Regex.Replace(
            PathTemplate,
            "[A-Z][A-Z0-9_]*|\\[[a-z]+\\]",
            placeholder =>
                substitutions.TryGetValue(placeholder.Value, out var value)
                && !string.IsNullOrEmpty(value)
                    ? value
                    : throw new ArgumentException(
                        "The object declaration contains an unbound template identifier: "
                            + placeholder.Value,
                        nameof(substitutions)
                    ),
            RegexOptions.CultureInvariant
        );
        return new(result);
    }
}

/// <summary>All explicit object declarations in the supplied GREIS 4.2.01 manual, retaining its path templates.</summary>
public static partial class JavadParameterCatalog
{
    public const string SpecificationVersion = "4.2.01";
    public static ImmutableArray<JavadParameterDescriptor> All { get; } = Create();

    /// <summary>Returns all declarations for a template; the manual can describe one object more than once.</summary>
    public static IEnumerable<JavadParameterDescriptor> Find(string pathTemplate) =>
        All.Where(x => string.Equals(x.PathTemplate, pathTemplate, StringComparison.Ordinal));

    public static IEnumerable<JavadParameterDescriptor> Under(string prefix) =>
        All.Where(x => x.PathTemplate.StartsWith(prefix, StringComparison.Ordinal));
}
