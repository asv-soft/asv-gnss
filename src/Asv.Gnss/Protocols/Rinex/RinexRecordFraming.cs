using System;
using System.Collections.Generic;

namespace Asv.Gnss;

internal static class RinexRecordFraming
{
    internal static int? GetLineCount(IReadOnlyList<string> lines, RinexHeader header, RinexMessageId id,
        bool endOfFile = false)
    {
        if (lines.Count == 0) return null;
        if (id == RinexMessageId.Header)
        {
            for (var index = 0; index < lines.Count; index++)
                if (RinexHeader.Label(lines[index]) == "END OF HEADER") return index + 1;
            return null;
        }
        return id switch
        {
            RinexMessageId.ObservationEpoch => RinexObservationEpochMessage.GetRecordLineCount(lines, header, endOfFile),
            RinexMessageId.Navigation => RinexNavigationMessage.GetRecordLineCount(lines, header) ??
                (endOfFile ? lines.Count : null),
            RinexMessageId.Meteorological => RinexMeteorologicalMessage.GetRecordLineCount(lines, header),
            _ => throw new ArgumentOutOfRangeException(nameof(id))
        };
    }

    internal static RinexMessageId GetBodyId(RinexHeader header) => header.FileType switch
    {
        'O' => RinexMessageId.ObservationEpoch,
        'N' or 'G' or 'H' or 'L' => RinexMessageId.Navigation,
        'M' => RinexMessageId.Meteorological,
        _ => throw new NotSupportedException($"Unsupported RINEX file type '{header.FileType}'.")
    };
}
