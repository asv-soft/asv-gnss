# RINEX

RINEX (Receiver Independent Exchange Format) exchanges GNSS observations, broadcast navigation data, and meteorological measurements as ASCII files. The codec uses the same `Asv.IO` registration, factory, message, and parser interfaces as the other protocols. A parser instance retains the current file header because subsequent records depend on its version, observation codes, and satellite systems.

## Coverage

| File family | Versions | Native model | Reading and writing |
|-------------|----------|--------------|---------------------|
| File header | 2.10/2.11, 3.00–3.05, 4.00–4.02 | `RinexHeaderMessage` / `RinexHeader` | Known context fields plus original metadata records |
| Observations | Same | `RinexObservationEpochMessage` | Epochs, receiver clock offset, satellite observations, LLI/SSI, event flags 0–6 |
| Navigation | Same | `RinexNavigationMessage` | Legacy GPS/GLO/Galileo/QZSS/BDS/NavIC/SBAS ephemerides and RINEX 4 EPH/STO/EOP/ION records |
| Meteorology | Same | `RinexMeteorologicalMessage` | Header-defined observables, blank values, and continuation records |

The default registration includes all four model IDs in `RinexMessageId`. RINEX 2 `G`/`H`/`L` navigation file types select their native satellite systems. Navigation values remain ordered numeric fields (`Values`), with `ValueFieldCounts` retaining row layouts. STO text identifiers, the RINEX 4 navigation message type and optional subtype are separate properties. The codec validates supported record shapes; it does not calculate orbits or convert ephemerides into receiver-specific messages.

Observation support includes legacy satellite-list continuations, five observation fields per RINEX 2 line, modern long satellite records, optional wrapped records, missing observations, receiver events, and cycle-slip flag 6. Flag-4 header updates replace the context for later epochs while earlier published messages keep their original context. RINEX 4.02 picosecond epoch digits are preserved in their separate five-column extension.

## Read files

```c#
using System;
using System.Linq;
using Asv.Gnss;

var records = await RinexFile.ReadAsync("station.rnx.gz");
var header = records.OfType<RinexHeaderMessage>().First().Header;
Console.WriteLine($"RINEX {header.Version}, {header.FileType}, {header.MarkerName}");

foreach (var epoch in records.OfType<RinexObservationEpochMessage>())
{
    foreach (var satellite in epoch.Satellites)
    {
        foreach (var observation in satellite.Observations)
        {
            Console.WriteLine($"{satellite.Satellite} {observation.Code}: {observation.Value}");
        }
    }
}

RinexFile.Write("station-copy.rnx", records);
```

`Read` / `ReadAsync` accept a path or readable stream and return the header plus decoded records. Gzip is detected by magic bytes, independent of the filename; non-seekable streams are supported. `Write` accepts a path or stream; `WriteAsync` accepts a stream. Caller-provided streams remain open. File methods collect records in memory and fail on malformed or unsupported data. For large recordings, subscribe to the incremental parser instead of retaining every record. Async methods accept cancellation tokens.

Compression is a transport concern: wrap an output stream in `GZipStream` to create compressed output. Compact RINEX / Hatanaka (`.crx`, `.d`), Unix `.Z`, ZIP, clock (`C`), IONEX, SP3, ANTEX, and RINEX 1/2.00/2.01/2.12 are not implemented. Expand Hatanaka or unsupported compression before feeding the parser. An unsupported RINEX family/version is rejected explicitly.

## Incremental parsing

```c#
using System;
using System.IO;
using Asv.Gnss;
using Asv.IO;
using R3;

var protocol = Protocol.Create(builder => builder.Protocols.RegisterRinexProtocol());
using var parser = (RinexMessageParser)protocol.CreateParser(RinexProtocol.Info.Id);
using var subscription = parser.OnMessage.Subscribe(message =>
{
    Console.WriteLine($"{message.Name}: {message.GetIdAsString()}");
});

await using var stream = File.OpenRead("station.rnx");
var buffer = new byte[8192];
int count;
while ((count = await stream.ReadAsync(buffer)) != 0)
{
    for (var index = 0; index < count; index++) parser.Push(buffer[index]);
}
parser.Complete();
```

`Push` accepts ASCII bytes, LF/CRLF/CR line endings, and a UTF-8 BOM at stream start. It publishes a header after `END OF HEADER`. For modern observation and RINEX 4 navigation records, the next record marker may be needed to resolve a continuation boundary; call `Complete()` at EOF even when the file ends with a newline. It also handles a final line without a newline and throws on incomplete trailing records. `Reset()` discards partial data and file context. Concatenated complete files are accepted with independent headers.

Malformed records are reported through the standard parser error stream. With R3, subscribe to both values and recoverable errors, for example `parser.OnError.Subscribe(onNext: Report, onErrorResume: Report, onCompleted: _ => { })`, where `Report` accepts an `Exception`. The file API converts parse failures into `FormatException` and never returns a silently truncated successful result.

The parser bounds a line to 1 MiB and a record/header to 16 MiB / 32768 lines to avoid unbounded buffering on malformed streams. These are implementation limits, not format limits. Registering only selected message types through the factory callback still retains header context and event updates internally.

## Data semantics and writing

- `RinexEpoch` preserves calendar fields and decimal seconds, including leap seconds and up to picosecond precision. No implicit GPS/UTC/GLO/BDT conversion occurs. `ToDateTime()` returns `DateTimeKind.Unspecified`, truncates sub-tick fractions, and rejects a leap-second value that .NET cannot represent.
- `RinexObservationValue.Value` is nullable: blank and zero are different. LLI and SSI are separate nullable digits; blank and `0` are different. Values remain in their stored units. Scale factors, phase shifts, antenna corrections, receiver-clock corrections and wavelength metadata are retained in the header and are not applied automatically.
- Unknown header records remain in `HeaderLines`. Known context fields and descriptors are serialized from the typed model. Setting `TimeSystem` requires a retained `TIME OF FIRST OBS` record with its calendar fields; the writer rejects an identifier that would otherwise have no place in the native header. Metadata records retain their relative order; observation descriptors precede metadata that depends on them.
- Navigation `D`/`E` exponents are accepted and writing uses invariant culture and fixed-width RINEX numeric fields. Null numeric fields remain blank. Values with invalid widths, non-finite numbers or inconsistent record layouts are rejected.
- Serialization emits canonical LF text from current typed values. It preserves record meaning rather than original whitespace or exponent spelling. Include a `RinexHeaderMessage` before body records when writing a file. Body contexts must match the active header, including observation descriptors and any event updates; incompatible contexts are rejected. Direct body `Deserialize` requires its corresponding `Header` to be assigned first and consumes only that native record.

## Console inspector

```bash
dotnet run --project src/Asv.Gnss.Shell -- inspect --file "station.rnx"
```

The existing inspector registers RINEX alongside the other implemented formats and completes pending RINEX records at EOF. Selected header, observation, navigation and meteorological records appear through the normal message-details panel. This command takes plain input; use `RinexFile` for gzip decoding.

## Specifications and regression data

The implementation follows the [IGS RINEX 2.11 format](https://files.igs.org/pub/data/format/rinex211.txt), [RINEX 3.04](https://files.igs.org/pub/data/format/rinex304.pdf), and [RINEX 4.02](https://files.igs.org/pub/data/format/rinex_4.02.pdf). Later-version and record-family support is stated explicitly above rather than inferred from file extensions.

Offline tests use original public files pinned from [GeoRinex](https://github.com/geospace-code/georinex) and [NAV Solutions data](https://github.com/nav-solutions/data), covering OBS2/3, GPS/GLO/Galileo/QZSS/SBAS NAV2/3, modern NAV4.02 and MET2/4. The test-resource README records upstream revisions, SHA-256 hashes, licenses and independent expected fields/counts. One malformed public Galileo sample is retained as a rejection test. Synthetic cases complement recordings for RINEX 4.02 picosecond epochs, event header changes, chunk boundaries, EOF validation and missing-value intersections.
