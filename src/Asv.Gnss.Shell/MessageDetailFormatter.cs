using System.Globalization;
using System.Reflection;
using System.Text;
using Asv.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Asv.Gnss;

internal static class MessageDetailFormatter
{
    internal const int MaximumCharacters = 32768;
    private static readonly DetailContractResolver ContractResolver = new();

    public static string Format(IProtocolMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        var text = new StringBuilder();
        using var output = new LimitedTextWriter(text);
        using var writer = new DetailJsonWriter(output)
        {
            Formatting = Formatting.Indented,
            AutoCompleteOnClose = false,
            CloseOutput = false
        };
        var unavailable = new List<string>();
        var serializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            ContractResolver = ContractResolver,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Culture = CultureInfo.InvariantCulture,
            Error = (_, args) =>
            {
                if (output.LimitReached || writer.LimitReached)
                {
                    return;
                }

                if (unavailable.Count < 5)
                {
                    var member = args.ErrorContext.Member?.ToString() ?? "unknown";
                    var path = args.ErrorContext.Path;
                    unavailable.Add(string.IsNullOrEmpty(path) ? member : $"{path}.{member}");
                }
                args.ErrorContext.Handled = true;
            }
        });

        try
        {
            serializer.Serialize(writer, message);
            writer.Flush();
        }
        catch (Exception) when (output.LimitReached || writer.LimitReached)
        {
            text.Append("\n\n[Content truncated for display.]");
        }
        catch (Exception error)
        {
            text.Append("\n\n[Unable to format message: ").Append(error.GetType().Name).Append(']');
        }

        if (unavailable.Count > 0)
        {
            text.Append("\n\n[Unavailable fields: ").AppendJoin(", ", unavailable).Append(']');
        }

        return text.ToString();
    }

    private sealed class DetailContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            // Protocol tags are exposed by ref and cannot be read by a JSON value provider.
            if (member is PropertyInfo info && (info.PropertyType.IsByRef || info.GetIndexParameters().Length > 0))
            {
                property.Ignored = true;
            }
            return property;
        }
    }

    private sealed class DetailJsonWriter(TextWriter output) : JsonTextWriter(output)
    {
        public bool LimitReached { get; private set; }

        public override void WriteStartObject()
        {
            CheckDepth();
            base.WriteStartObject();
        }

        public override void WriteStartArray()
        {
            CheckDepth();
            base.WriteStartArray();
        }

        private void CheckDepth()
        {
            if (Top < 16)
            {
                return;
            }
            LimitReached = true;
            throw new DetailLimitException();
        }
    }

    private sealed class LimitedTextWriter(StringBuilder text) : StringWriter(text, CultureInfo.InvariantCulture)
    {
        public bool LimitReached { get; private set; }

        public override void Write(char value)
        {
            Span<char> characters = stackalloc char[1];
            characters[0] = value;
            Write(characters);
        }

        public override void Write(string? value) => Write(value.AsSpan());
        public override void Write(char[] buffer, int index, int count) => Write(buffer.AsSpan(index, count));

        public override void Write(ReadOnlySpan<char> buffer)
        {
            var remaining = MaximumCharacters - GetStringBuilder().Length;
            if (buffer.Length <= remaining)
            {
                GetStringBuilder().Append(buffer);
                return;
            }

            GetStringBuilder().Append(buffer[..remaining]);
            LimitReached = true;
            throw new DetailLimitException();
        }
    }

    private sealed class DetailLimitException : Exception;
}
