using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asv.IO;

namespace Asv.Gnss;

/// <summary>Native record families exposed through the shared protocol message factory.</summary>
public enum RinexMessageId : byte
{
    /// <summary>File header and record-decoding context.</summary>
    Header,
    /// <summary>Observation, event or cycle-slip epoch.</summary>
    ObservationEpoch,
    /// <summary>Broadcast navigation or system data record.</summary>
    Navigation,
    /// <summary>Meteorological observation record.</summary>
    Meteorological
}

/// <summary>A native RINEX record. Body records require the corresponding file header as context.</summary>
public abstract partial class RinexMessageBase : IProtocolMessage<RinexMessageId>
{
    private ProtocolTags _tags = [];
    /// <inheritdoc />
    public abstract RinexMessageId Id { get; }
    /// <inheritdoc />
    public string Name => $"RINEX {Id}";
    /// <inheritdoc />
    public ProtocolInfo Protocol => RinexProtocol.Info;
    /// <inheritdoc />
    public ref ProtocolTags Tags => ref _tags;
    /// <inheritdoc />
    public string GetIdAsString() => Id.ToString();
    /// <summary>Header context required to interpret and serialize this native record.</summary>
    public RinexHeader Header { get; set; } = new();

    /// <inheritdoc />
    public void Deserialize(ref ReadOnlySpan<byte> buffer)
    {
        var lines = new List<string>();
        var ends = new List<int>();
        var offset = 0;
        while (offset < buffer.Length)
        {
            var stop = offset;
            while (stop < buffer.Length && buffer[stop] is not (10 or 13)) stop++;
            lines.Add(Encoding.ASCII.GetString(buffer[offset..stop]));
            if (stop < buffer.Length)
            {
                var separator = buffer[stop++];
                if (separator == 13 && stop < buffer.Length && buffer[stop] == 10) stop++;
            }
            ends.Add(stop);
            offset = stop;
        }
        var count = RinexRecordFraming.GetLineCount(lines, Header, Id, true);
        if (count is null || count == 0 || count > lines.Count)
            throw new FormatException("A RINEX record is incomplete.");
        var consumed = ends[count.Value - 1];
        foreach (var value in buffer[..consumed])
            if (value > 127 || (value < 32 && value is not (9 or 10 or 13)))
                throw new FormatException("RINEX records must contain ASCII text.");
        ReadLines(lines.Take(count.Value).ToArray());
        buffer = buffer[consumed..];
    }

    /// <inheritdoc />
    public void Serialize(ref Span<byte> buffer)
    {
        var text = GetText();
        if (buffer.Length < text.Length)
            throw new ArgumentException("The destination is shorter than the RINEX record.", nameof(buffer));
        Encoding.ASCII.GetBytes(text, buffer);
        buffer = buffer[text.Length..];
    }

    /// <inheritdoc />
    public int GetByteSize() => GetText().Length;

    private string GetText()
    {
        var text = string.Join("\n", WriteLines()) + "\n";
        foreach (var value in text)
            if (value > 127 || (value < 32 && value is not ('\t' or '\n')))
                throw new FormatException("RINEX records must contain ASCII text.");
        return text;
    }

    /// <summary>Decodes exactly one native record using the assigned header context.</summary>
    protected abstract void ReadLines(IReadOnlyList<string> lines);
    /// <summary>Formats the current typed record as native ASCII lines without line terminators.</summary>
    protected abstract IEnumerable<string> WriteLines();
}
