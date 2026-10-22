# Javad / GREIS

The implementation follows `src/Asv.Gnss/Protocols/Javad/Docs/GREIS-4.2.01.pdf`. It provides streaming parsing and serialization for **303 native receiver-message identifiers**: 271 binary models and 32 text, reply, parameter, wrapper, and event models. The default registration includes the compatibility messages in Appendix A.4 as well as the current Chapter 3.4 messages.

GREIS receiver commands, command lines, response labels, compound parameter values, and the documented receiver-object catalog are also available. Protocol identifiers are case-sensitive: for example, `PG`, `Pg`, `gd`, and `GD` are distinct messages.

## Connect the parser

```c#
using System;
using Asv.Gnss;
using Asv.IO;
using R3;

var protocol = Protocol.Create(builder =>
{
    builder.Protocols.RegisterJavadProtocol();
});

using var parser = protocol.CreateParser(JavadProtocol.Info.Id);
using var subscription = parser.OnMessage.Subscribe(message =>
{
    if (message is JavadReceiverTimeMessage time)
    {
        Console.WriteLine($"Receiver time: {time.TimeOfDay} ms");
    }
});

// A complete ~~ receiver-time frame with its checksum.
foreach (var value in Convert.FromHexString("7E7E30303504030201BC"))
{
    parser.Push(value);
}
```

Use the same registration with a router and transport from [Quick Start](quick-start.md). `RegisterJavadProtocol` accepts a factory callback after adding the defaults; it can remove a message or add a custom `JavadMessageBase` implementation. Registration selects decoders. Configure the receiver's output separately with GREIS commands.

`JavadMessageParser` accepts fragmented and adjacent frames, validates each message's checksum type, and recovers after malformed candidates. A standard frame has two ASCII identifier characters, three uppercase hexadecimal body-length digits, and a body of up to 4095 bytes, including its checksum. The maximum complete frame is 4100 bytes. Line endings between frames are optional.

A standard GREIS stream can contain non-standard ASCII lines such as NMEA sentences. The GREIS parser skips those lines; register the [NMEA parser](protocol-nmea.md) to decode them. Unwrapped RTCM, BINEX, and CMR binary streams are separate wire protocols. `JavadWrapperMessage` (`>>`) carries arbitrary bytes inside a GREIS frame and preserves its `SourceId` and `Data`.

## Typed messages and serialization

All native models derive from `JavadMessageBase`. They consume one complete frame from the supplied span, leaving any subsequent bytes available to the caller. `GetByteSize()` includes the header and checksum.

```c#
using System;
using Asv.Gnss;

var position = new JavadGeodeticPositionMessage
{
    Latitude = 55.75 * Math.PI / 180.0,  // PG uses radians.
    Longitude = 37.5 * Math.PI / 180.0,
    Altitude = 150.25,                  // Ellipsoidal height in meters.
    PositionSigma = 0.5f,
    SolutionType = 1,
};

var bytes = new byte[position.GetByteSize()];
Span<byte> destination = bytes;
position.Serialize(ref destination);

ReadOnlySpan<byte> source = bytes;
var decoded = new JavadGeodeticPositionMessage();
decoded.Deserialize(ref source);
```

Binary properties retain the specification's raw units, scaling, bit fields, and missing-value sentinels. For example, integer measurement encodings remain integer values and IEEE NaNs remain NaNs. The codecs preserve these values for callers that need exact data or subsequent calculations. They do not assemble observation epochs or apply satellite-index, relative-measurement, clock, or GNSS navigation calculations. Applications can build that processing from `~~`, `::`, `RD`, `ST`, `SX`/`SI`, and the measurement models.

`MF` sets the incoming stream's major/minor format version and byte order. Each parser has independent state; decoded messages retain the `ByteOrder` used for their payload. The default is little endian. `Reset()` clears partial framing while retaining the established format, because `Asv.IO` resets parsers at packet boundaries. Call `ResetStream()` when starting a different receiver stream to restore format 1.0 and little-endian defaults. Unsupported major versions suspend decoding of ordinary messages until a supported `MF` is received; `JP` and `MF` remain recognizable. When decoding a single binary frame directly, set its `ByteOrder` first if it is big endian. When writing a stream, set each binary message's `ByteOrder` consistently with the emitted `MF` message; serialization does not maintain a separate outbound session.

For extensible fixed layouts, trailing bytes before the checksum are captured in `ExtensionData` and serialized again. Known optional groups are represented by the corresponding model properties. Variable records retain their specified counts, lengths, and extensions. The exceptions are `JP` and `MF`, whose sizes are fixed by the specification.

Historical recordings can omit the final `Edop` field in `DP`, `Nt` in `NU`, `Flags` in `WE`, or `DAf0` in `EN`. Their existing scalar properties remain available; `HasEdop`, `HasNt`, `HasFlags`, and `HasDAf0` record whether the field is present on the wire. New messages include these fields by default. Decoding an older layout sets its presence flag to false and resets the absent scalar to zero. Set the flag to true when adding that field. An absent field cannot be followed by `ExtensionData`, because those bytes would be interpreted as the field on the next decode.

`JavadTextMessage` exposes typed properties and immutable `Fields` after the message title. Nested satellite, data-link, correction-text, timing, and reference-station groups have typed views. Omitted fields, nested lists, future fields, and original text spelling survive a decode/encode cycle. Editing a property updates the corresponding field while retaining unrelated fields. Decimal text values use invariant culture; a padded value such as `009` means decimal nine. Short forms selected by position or RAIM status are accepted.

An unregistered identifier is skipped using its declared length and reported through `OnError` as an unknown message. The parser does not guess a checksum for unknown layouts. For explicit forwarding or inspection when the layout is known, `JavadRawMessage` accepts an identifier and `JavadChecksumKind`; its `Payload` excludes the selected checksum. This raw utility is separate from the concrete native models in the catalog below.

## Inspect and edit fields with a visitor

Native receiver messages implement `IVisitable`. Call `Accept(IVisitor)` to inspect or edit the payload through the standard `Asv.IO` visitor interfaces. Message headers and checksums are rebuilt by serialization; `ExtensionData` is visited as a byte list.

```c#
using System.Text;
using Asv.Gnss;
using Asv.IO;

var attitude = new JavadImuAttitudeMessage
{
    SolutionValidity = 'V',
    RollDegrees = 1.5,
    PitchDegrees = -0.25,
    HeadingDegrees = 90.0,
};

var text = new StringBuilder();
attitude.Accept(new PrintValueVisitor(text, skipUnknown: false));
StructType metadata = JavadImuAttitudeMessage.StructType;
```

Binary models expose their wire scalar types, arrays, lists, and nested records. Computed lengths and legacy layout selectors carry `readOnly=true` metadata; attempting to change them throws `InvalidOperationException`. Change their source arrays or flags instead. Native text models expose nullable integers, doubles, ASCII strings, characters, and `TimeSpan` values. Satellite, signal-strength, data-link, timing, and reference-station groups are structured fields. Visitors can change values through `ref` parameters; resizing a counted text list also updates its wire count and retains trailing future fields. Text record views implement `IVisitable` too. When visiting a whole message, changes to nested records are written back into its immutable `Fields` tree. When visiting a detached record view, assign the edited view back through the corresponding message property or `Fields`.

Known layouts have cached static `StructType` metadata. `TD` and complete `PM` payloads have arbitrary nested shapes: `GetVisitorType()` returns cached metadata for their current value tree, and `Accept` traverses that structure. An incomplete `PM` fragment is visited as its literal `Parameters` string. `RE` and `ER` expose literal `Text`, preserving response labels and incomplete fragments; use `Response` to interpret their command semantics.

A visitor that leaves values unchanged preserves the original text bytes, decimal padding, whitespace, omitted fields, and immutable node identities. Editing a text field rebuilds only the affected nodes; unrelated known and future fields remain available. Derived convenience properties, such as latitude converted from a text angle into degrees, are not a second editable representation of the same wire field.

`JavadCommand` and `JavadCommandLine` also support `Accept`. They validate edited command values before publishing changes; inspection retains original whitespace, comments, and checksum spelling. Their argument and option trees use the same immutable-value visitor described below.

The typed text traversal covers the documented fields. To inspect or edit all fields, including unknown extensions and heterogeneous lists, use `JavadValueVisitor.VisitFields(visitor, ref value)` on a `JavadValue.List` created from `message.Fields`, then assign `value.Items` back only if the root value changed. `JavadValueVisitor.GetType(value)` returns the exact finite schema for an immutable value and reuses metadata while that value is unchanged. `JavadValueVisitor.Visit(visitor, field, ref value)` adds an enclosing structure callback; the field must use that value's current metadata. Use value factories to replace a tree's shape or add options before visiting it.

## Receiver commands and responses

`JavadCommand` represents one statement, and `JavadCommandLine` represents statements separated by `;`, `&&`, or `||`. They serialize ASCII with CRLF and support the optional rotate/XOR checksum. The command catalog contains all 13 command names from Chapter 2.3: `set`, `print`, `list`, `em`, `out`, `dm`, `init`, `create`, `remove`, `event`, `get`, `put`, and `fld`.

```c#
using Asv.Gnss;

var query = JavadCommand.Print(new JavadObjectPath("/par/rcv/id"))
    .WithLabel("identify")
    .WithChecksum();

var schedule = JavadCommand.EnableMessages(
    JavadValue.Parse("jps/{RT,RD,PG,ET}")
        .WithOptions(JavadParameterValues.Scheduling(period: 1.0)));

var reply = new JavadReplyMessage { Text = "%identify% QP01234TR45" };
JavadCommandResponse response = reply.Response;
if (response.Matches(query))
{
    System.Console.WriteLine(response.Text);
}
```
{ignore-vars="true"}

Send a command through the selected endpoint or serialize it into the receiver's input stream. An explicit `%\label%` forces a response and enables correlation with `JavadCommandResponse.Matches`. Unlabelled replies cannot identify which outstanding command produced them. `JavadReplyMessage` and `JavadErrorMessage` retain their complete ASCII payloads; the parsed response exposes `Label`, `Text`, and, for a complete error value, `ErrorCode`.

A `print` or `list` result can span multiple `RE` messages. `TryGetValue` succeeds only when the response text is a complete GREIS value. Accumulate the relevant fragments according to the request and application workflow before parsing a full object tree; one successful `RE` frame does not imply a complete response. `PM` retains the parameter payload and supports the same complete-value parsing approach.

For compatibility with recorded receiver streams, `PM` accepts both the documented `,@` checksum delimiter and a bare `@`. A decoded message retains its delimiter when serialized or edited; newly constructed messages use `,@`.

`JavadValue` represents quoted strings, omitted values, empty lists, nested or prefixed lists, named values, and options. `JavadParameterValues` supplies scheduling, IPv4, datum, Cartesian-position, and geodetic-position values. `JavadTimeSpecification` handles recurring time fields and wildcards. Receiver-only `TextAtom` values require explicit quoting before they can be used in commands.

## Data Transfer Protocol

The Appendix A.2 Data Transfer Protocol (DTP) is an explicit transfer mode used after a suitable `get`, `put`, or `fld` command. It has its own framing; route transfer bytes to the DTP helpers while that mode is active. `JavadMessageParser` handles ordinary GREIS receiver messages.

`JavadDtpCodec` encodes and decodes ordinal, last, error, and abort blocks. DTP integers are always little endian, independently of `MF`. Its CRC16 continues from the previous accepted block, and a last block carries its actual byte count plus zero padding. The codec accepts the negotiated block size; both ends must use the same value.

`JavadDtpReceiver` accepts bytes incrementally. Start with `InitialRequest`, process the returned `JavadDtpReceiveResult`, deliver each new block's data once, and send `Request` only when it is non-null. It tracks sequence numbers, cumulative checksums, completion, and duplicate blocks. Call `RequestRetry()` after a deadline to discard a partial block and request retransmission.

`JavadDtpTransmitter` prepares one block with `Prepare` and interprets incoming requests with `ProcessRequest`. A retransmission uses the unchanged `CurrentBlock`; the checksum and sequence number advance when the current ordinal block is acknowledged. Call `MarkCurrentBlockSent()` after the transport finishes writing `CurrentBlock`. Mark the final block with `isLast: true`, including an empty final block when needed; sending that last block completes the transmitter without waiting for a final acknowledgement.

The `get` command's `attempts` option selects its output format; it is not always DTP:

| `attempts` | Output handling |
| --- | --- |
| Greater than 1 | Ordinary DTP with receiver requests and retransmission attempts. |
| `1` | Streaming DTP: the sender does not wait for ACKs, and a NACK aborts the transfer. The same block decoder accepts the stream. |
| `0` | Raw object bytes; the application handles their boundaries. |
| `-1` through `-256` | GREIS `>>` wrappers with source ID `-1 - attempts`. The first data byte is a sequence character from `0` through `o`, modulo 64; a final wrapper without data indicates completion. |
| `-257` | GREIS `RE` chunks, retaining the command label and payload text. |

These helpers maintain block and transfer state. The application owns stream I/O, initiating commands, negotiated options, deadlines, pacing, file access, and switching back to GREIS messages after transfer completion. No receiver connection or file transfer starts merely by constructing a helper.

## Receiver-object catalog

`JavadParameterCatalog.All` contains **1227 documented object declarations**, including **1209 `/par` declarations** and compatibility entries. Each descriptor retains the path template, original declaration, access, type, values, default, options, and PDF page. These are specification metadata; receiver firmware and installed options determine actual availability and accepted ranges.

```c#
using System.Collections.Generic;
using System.Linq;
using Asv.Gnss;

var descriptor = JavadParameterCatalog.Find("/par/dev/tcpo/X/port").Single();
var port = descriptor.Bind(new Dictionary<string, string> { ["X"] = "a" });
var readPort = JavadCommand.Print(port);

var rate = new JavadObjectPath("/par/dev/ser/a").Child("rate");
var readDefaultRate = JavadCommand.Print(rate.Attribute("def"));
var setRate = JavadCommand.Set(rate, JavadValue.Integer(115200));
```

`Find` returns every declaration with the exact template. `Under` selects a path prefix, and `Bind` substitutes explicit template identifiers such as `X`, `SYS`, or `[port]`. `JavadObjectPath` also supports relative paths, dynamic objects, and `&attribute` access, so object access is not restricted to this manual's catalog.

## Specification details and validation

The schema records the following inconsistencies in GREIS 4.2.01:

- The S4 and SigmaPhi body-size expressions on page 103 conflict with their `f4` arrays. The codecs follow the field declarations: `4 * nSats + 1` bytes including checksum.
- The `Sp` size expression omits `n`. Its payload uses `8 + n * m * 2` bytes and the documented optional 33-byte extension.
- `IE` uses the required 122-byte GPS ephemeris data plus `navType` and checksum. `QE` follows the GPS ephemeris optional layouts, including CNAV2, despite its abbreviated printed length list.
- The `SvDelay` reference on page 119 points to the preceding `SvDelays` record.
- `sj` appears on page 130 but is absent from the table of contents; it is included in the implementation and catalog.
- The access field for `/par/[cport]/sid/out/first` on page 514 is mislabelled `Name: rw`; the parameter generator treats it as access metadata.

Tests cover the three `TD` frames printed on page 393, checksum algorithms, fragmentation and recovery, byte-order isolation, each native model's round trip, optional/variable structures, command syntax, parameter declarations, and preservation of extensions.

Five public receiver recordings and upstream excerpts from RTKLIB, GPSD, and EarthScope gnsstools are included in `src/Asv.Gnss.Test/Resources/Javad`. They contain 6376 complete frames in total, plus the original incomplete tail of the RTKLIB recording. Replay tests verify file hashes, independently inventoried boundaries, message order, exact reserialization, selected dates/times/positions, historical layouts, and unchanged out-of-range ionosphere time fields. Tests use local files without network access. The resource README records pinned source URLs, SHA-256 hashes, provenance, and licenses. No physical JAVAD receiver was used for validation.

The binary source of truth is `Protocols/Javad/Schema/binary-messages.json`; custom variable codecs are maintained alongside the generated models. Regenerate and verify from the repository root:

```bash
python3 src/Asv.Gnss/Protocols/Javad/Schema/generate_binary.py
python3 src/Asv.Gnss/Protocols/Javad/Schema/generate_binary.py --check
pdftotext -layout src/Asv.Gnss/Protocols/Javad/Docs/GREIS-4.2.01.pdf /tmp/greis-layout.txt
python3 src/Asv.Gnss/Protocols/Javad/Parameters/generate_catalog.py /tmp/greis-layout.txt
```

Edit the schema, maintained codecs, or generator when changing their behavior. Do not edit `Generated/JavadBinaryMessages.g.cs` or `Parameters/JavadParameterCatalog.Generated.cs` by hand.

## Specification message catalog

The table lists every native identifier covered by the supplied GREIS 4.2.01 specification, including measurement families, the omitted `sj` contents entry, legacy `SI`, and Appendix A.4 messages. **Model present** means the concrete model implements decoding and serialization of its documented layout. **Default** indicates inclusion in `RegisterJavadProtocol`; it does not promise support by a particular receiver firmware. Foreign NMEA/RTCM/CMR messages remain separate protocols or payloads of `>>`.

Page references are one-based PDF pages; printed page numbers coincide in this document.

| GREIS ID | Message | Status | C# model | Default | PDF page |
| --- | --- | --- | --- | --- | --- |
| `JP` | File Identifier | Model present | `JavadFileIdentifierMessage` | Yes | 73 |
| `MF` | Messages Format | Model present | `JavadMessagesFormatMessage` | Yes | 74 |
| `~~` | Receiver Time | Model present | `JavadReceiverTimeMessage` | Yes | 74 |
| `::` | Epoch Time | Model present | `JavadEpochTimeMessage` | Yes | 75 |
| `RD` | Receiver Date | Model present | `JavadReceiverDateMessage` | Yes | 75 |
| `TO` | Reference Time to Receiver Time Offset | Model present | `JavadReferenceTimeOffsetMessage` | Yes | 75 |
| `DO` | Derivative of Receiver Time Offset | Model present | `JavadReferenceTimeOffsetDerivativeMessage` | Yes | 75 |
| `BP` | Rough Accuracy of Time Approximation | Model present | `JavadTimeApproximationAccuracyMessage` | Yes | 76 |
| `GT` | GPS Time | Model present | `JavadGpsTimeMessage` | Yes | 76 |
| `GO` | GPS to Receiver Time Offset | Model present | `JavadGpsTimeOffsetMessage` | Yes | 76 |
| `NT` | GLONASS Time | Model present | `JavadGlonassTimeMessage` | Yes | 76 |
| `NO` | GLONASS to Receiver Time Offset | Model present | `JavadGlonassTimeOffsetMessage` | Yes | 76 |
| `EO` | GALILEO to Receiver Time Offset | Model present | `JavadGalileoTimeOffsetMessage` | Yes | 76 |
| `WO` | SBAS to Receiver Time Offset | Model present | `JavadSbasTimeOffsetMessage` | Yes | 77 |
| `QO` | QZSS to Receiver Time Offset | Model present | `JavadQzssTimeOffsetMessage` | Yes | 77 |
| `CO` | BeiDou to Receiver Time Offset | Model present | `JavadBeiDouTimeOffsetMessage` | Yes | 77 |
| `Io` | IRNSS to Receiver Time Offset | Model present | `JavadIrnssTimeOffsetMessage` | Yes | 77 |
| `UO` | GPS UTC Time Parameters | Model present | `JavadGpsUtcParametersMessage` | Yes | 77 |
| `WU` | SBAS UTC Time Parameters | Model present | `JavadSbasUtcParametersMessage` | Yes | 78 |
| `EU` | GALILEO UTC and GPS Time Parameters | Model present | `JavadGalileoUtcGpsParametersMessage` | Yes | 78 |
| `QU` | QZSS UTC Time Parameters | Model present | `JavadQzssUtcParametersMessage` | Yes | 78 |
| `CU` | BeiDou UTC Time Parameters | Model present | `JavadBeiDouUtcParametersMessage` | Yes | 79 |
| `IU` | IRNSS UTC Time Parameters | Model present | `JavadIrnssUtcParametersMessage` | Yes | 79 |
| `NU` | GLONASS UTC and GPS Time Parameters | Model present | `JavadGlonassUtcGpsParametersMessage` | Yes | 79 |
| `ST` | Solution Time-Tag | Model present | `JavadSolutionTimeMessage` | Yes | 79 |
| `PO` | Cartesian Position | Model present | `JavadCartesianPositionMessage` | Yes | 80 |
| `Po` | (PoWgs,PoLoc) Cartesian Position in Specific System | Model present | `JavadSpecificCartesianPositionMessage` | Yes | 80 |
| `VE` | Cartesian Velocity | Model present | `JavadCartesianVelocityMessage` | Yes | 80 |
| `PV` | Cartesian Position and Velocity | Model present | `JavadCartesianPositionVelocityMessage` | Yes | 80 |
| `PG` | Geodetic Position | Model present | `JavadGeodeticPositionMessage` | Yes | 81 |
| `Pg` | (PgWgs,PgLoc) Geodetic Position in Specific System | Model present | `JavadSpecificGeodeticPositionMessage` | Yes | 81 |
| `VG` | Geodetic Velocity | Model present | `JavadGeodeticVelocityMessage` | Yes | 81 |
| `SG` | Position and Velocity RMS Errors | Model present | `JavadPositionVelocityRmsMessage` | Yes | 81 |
| `mp` | Position in Local Plane | Model present | `JavadLocalPlanePositionMessage` | Yes | 82 |
| `bp` | Reference Station Position in Local Plane | Model present | `JavadReferenceStationLocalPlanePositionMessage` | Yes | 82 |
| `DP` | Dilution of Precision (DOP) | Model present | `JavadDilutionOfPrecisionMessage` | Yes | 83 |
| `SP` | Position Covariance Matrix | Model present | `JavadPositionCovarianceMessage` | Yes | 83 |
| `SV` | Velocity Covariance Matrix | Model present | `JavadVelocityCovarianceMessage` | Yes | 83 |
| `BL` | Baseline | Model present | `JavadBaselineMessage` | Yes | 83 |
| `bL` | Attitude Baselines | Model present | `JavadAttitudeBaselinesMessage` | Yes | 84 |
| `mR` | Attitude Full Rotation Matrix | Model present | `JavadAttitudeRotationMatrixMessage` | Yes | 84 |
| `PS` | Position Statistics | Model present | `JavadPositionStatisticsMessage` | Yes | 84 |
| `PT` | Time of Continuous Position Computation | Model present | `JavadContinuousPositionTimeMessage` | Yes | 85 |
| `SX` | Extended Satellite Indices | Model present | `JavadExtendedSatelliteIndicesMessage` | Yes | 92 |
| `AN` | Antenna Names | Model present | `JavadAntennaNamesMessage` | Yes | 92 |
| `NN` | GLONASS Satellite System Numbers | Model present | `JavadGlonassSatelliteNumbersMessage` | Yes | 93 |
| `EL` | Satellite Elevations | Model present | `JavadSatelliteElevationsMessage` | Yes | 93 |
| `AZ` | Satellite Azimuths | Model present | `JavadSatelliteAzimuthsMessage` | Yes | 93 |
| `RX` | Pseudo-ranges | Model present | `JavadPseudoRangesRXMessage` | Yes | 93 |
| `RC` | Pseudo-ranges | Model present | `JavadPseudoRangesRCMessage` | Yes | 93 |
| `R1` | Pseudo-ranges | Model present | `JavadPseudoRangesR1Message` | Yes | 93 |
| `R2` | Pseudo-ranges | Model present | `JavadPseudoRangesR2Message` | Yes | 93 |
| `R3` | Pseudo-ranges | Model present | `JavadPseudoRangesR3Message` | Yes | 93 |
| `R5` | Pseudo-ranges | Model present | `JavadPseudoRangesR5Message` | Yes | 93 |
| `Rl` | Pseudo-ranges | Model present | `JavadPseudoRangesRlMessage` | Yes | 93 |
| `rx` | Integer Pseudo-ranges | Model present | `JavadIntegerPseudoRangesrxMessage` | Yes | 94 |
| `rc` | Integer Pseudo-ranges | Model present | `JavadIntegerPseudoRangesrcMessage` | Yes | 94 |
| `r1` | Integer Pseudo-ranges | Model present | `JavadIntegerPseudoRangesr1Message` | Yes | 94 |
| `r2` | Integer Pseudo-ranges | Model present | `JavadIntegerPseudoRangesr2Message` | Yes | 94 |
| `r3` | Integer Pseudo-ranges | Model present | `JavadIntegerPseudoRangesr3Message` | Yes | 94 |
| `r5` | Integer Pseudo-ranges | Model present | `JavadIntegerPseudoRangesr5Message` | Yes | 94 |
| `rl` | Integer Pseudo-ranges | Model present | `JavadIntegerPseudoRangesrlMessage` | Yes | 94 |
| `CR` | Relative Pseudo-ranges | Model present | `JavadRelativePseudoRangesCRMessage` | Yes | 95 |
| `1R` | Relative Pseudo-ranges | Model present | `JavadRelativePseudoRanges1RMessage` | Yes | 95 |
| `2R` | Relative Pseudo-ranges | Model present | `JavadRelativePseudoRanges2RMessage` | Yes | 95 |
| `3R` | Relative Pseudo-ranges | Model present | `JavadRelativePseudoRanges3RMessage` | Yes | 95 |
| `5R` | Relative Pseudo-ranges | Model present | `JavadRelativePseudoRanges5RMessage` | Yes | 95 |
| `lR` | Relative Pseudo-ranges | Model present | `JavadRelativePseudoRangeslRMessage` | Yes | 95 |
| `cr` | Integer Relative Pseudo-ranges | Model present | `JavadIntegerRelativePseudoRangescrMessage` | Yes | 95 |
| `1r` | Integer Relative Pseudo-ranges | Model present | `JavadIntegerRelativePseudoRanges1rMessage` | Yes | 95 |
| `2r` | Integer Relative Pseudo-ranges | Model present | `JavadIntegerRelativePseudoRanges2rMessage` | Yes | 95 |
| `3r` | Integer Relative Pseudo-ranges | Model present | `JavadIntegerRelativePseudoRanges3rMessage` | Yes | 95 |
| `5r` | Integer Relative Pseudo-ranges | Model present | `JavadIntegerRelativePseudoRanges5rMessage` | Yes | 95 |
| `lr` | Integer Relative Pseudo-ranges | Model present | `JavadIntegerRelativePseudoRangeslrMessage` | Yes | 95 |
| `cm` | Pseudo-range Corrections | Model present | `JavadPseudoRangeCorrectionscmMessage` | Yes | 96 |
| `1m` | Pseudo-range Corrections | Model present | `JavadPseudoRangeCorrections1mMessage` | Yes | 96 |
| `2m` | Pseudo-range Corrections | Model present | `JavadPseudoRangeCorrections2mMessage` | Yes | 96 |
| `3m` | Pseudo-range Corrections | Model present | `JavadPseudoRangeCorrections3mMessage` | Yes | 96 |
| `5m` | Pseudo-range Corrections | Model present | `JavadPseudoRangeCorrections5mMessage` | Yes | 96 |
| `lm` | Pseudo-range Corrections | Model present | `JavadPseudoRangeCorrectionslmMessage` | Yes | 96 |
| `CC` | Smoothing Corrections | Model present | `JavadSmoothingCorrectionsCCMessage` | Yes | 96 |
| `C1` | Smoothing Corrections | Model present | `JavadSmoothingCorrectionsC1Message` | Yes | 96 |
| `C2` | Smoothing Corrections | Model present | `JavadSmoothingCorrectionsC2Message` | Yes | 96 |
| `C3` | Smoothing Corrections | Model present | `JavadSmoothingCorrectionsC3Message` | Yes | 96 |
| `C5` | Smoothing Corrections | Model present | `JavadSmoothingCorrectionsC5Message` | Yes | 96 |
| `Cl` | Smoothing Corrections | Model present | `JavadSmoothingCorrectionsClMessage` | Yes | 96 |
| `cc` | Smoothing Corrections | Model present | `JavadIntegerSmoothingCorrectionsccMessage` | Yes | 97 |
| `c1` | Smoothing Corrections | Model present | `JavadIntegerSmoothingCorrectionsc1Message` | Yes | 97 |
| `c2` | Smoothing Corrections | Model present | `JavadIntegerSmoothingCorrectionsc2Message` | Yes | 97 |
| `c3` | Smoothing Corrections | Model present | `JavadIntegerSmoothingCorrectionsc3Message` | Yes | 97 |
| `c5` | Smoothing Corrections | Model present | `JavadIntegerSmoothingCorrectionsc5Message` | Yes | 97 |
| `cl` | Smoothing Corrections | Model present | `JavadIntegerSmoothingCorrectionsclMessage` | Yes | 97 |
| `PC` | Carrier Phases | Model present | `JavadCarrierPhasesPCMessage` | Yes | 97 |
| `P1` | Carrier Phases | Model present | `JavadCarrierPhasesP1Message` | Yes | 97 |
| `P2` | Carrier Phases | Model present | `JavadCarrierPhasesP2Message` | Yes | 97 |
| `P3` | Carrier Phases | Model present | `JavadCarrierPhasesP3Message` | Yes | 97 |
| `P5` | Carrier Phases | Model present | `JavadCarrierPhasesP5Message` | Yes | 97 |
| `Pl` | Carrier Phases | Model present | `JavadCarrierPhasesPlMessage` | Yes | 97 |
| `pc` | Integer Carrier Phases | Model present | `JavadIntegerCarrierPhasespcMessage` | Yes | 97 |
| `p1` | Integer Carrier Phases | Model present | `JavadIntegerCarrierPhasesp1Message` | Yes | 97 |
| `p2` | Integer Carrier Phases | Model present | `JavadIntegerCarrierPhasesp2Message` | Yes | 97 |
| `p3` | Integer Carrier Phases | Model present | `JavadIntegerCarrierPhasesp3Message` | Yes | 97 |
| `p5` | Integer Carrier Phases | Model present | `JavadIntegerCarrierPhasesp5Message` | Yes | 97 |
| `pl` | Integer Carrier Phases | Model present | `JavadIntegerCarrierPhasesplMessage` | Yes | 97 |
| `CP` | Relative Carrier Phases | Model present | `JavadRelativeCarrierPhasesCPMessage` | Yes | 98 |
| `1P` | Relative Carrier Phases | Model present | `JavadRelativeCarrierPhases1PMessage` | Yes | 98 |
| `2P` | Relative Carrier Phases | Model present | `JavadRelativeCarrierPhases2PMessage` | Yes | 98 |
| `3P` | Relative Carrier Phases | Model present | `JavadRelativeCarrierPhases3PMessage` | Yes | 98 |
| `5P` | Relative Carrier Phases | Model present | `JavadRelativeCarrierPhases5PMessage` | Yes | 98 |
| `lP` | Relative Carrier Phases | Model present | `JavadRelativeCarrierPhaseslPMessage` | Yes | 98 |
| `cp` | Integer Relative Carrier Phases | Model present | `JavadIntegerRelativeCarrierPhasescpMessage` | Yes | 98 |
| `1p` | Integer Relative Carrier Phases | Model present | `JavadIntegerRelativeCarrierPhases1pMessage` | Yes | 98 |
| `2p` | Integer Relative Carrier Phases | Model present | `JavadIntegerRelativeCarrierPhases2pMessage` | Yes | 98 |
| `3p` | Integer Relative Carrier Phases | Model present | `JavadIntegerRelativeCarrierPhases3pMessage` | Yes | 98 |
| `5p` | Integer Relative Carrier Phases | Model present | `JavadIntegerRelativeCarrierPhases5pMessage` | Yes | 98 |
| `lp` | Integer Relative Carrier Phases | Model present | `JavadIntegerRelativeCarrierPhaseslpMessage` | Yes | 98 |
| `cf` | Phase Corrections | Model present | `JavadPhaseCorrectionscfMessage` | Yes | 99 |
| `1f` | Phase Corrections | Model present | `JavadPhaseCorrections1fMessage` | Yes | 99 |
| `2f` | Phase Corrections | Model present | `JavadPhaseCorrections2fMessage` | Yes | 99 |
| `3f` | Phase Corrections | Model present | `JavadPhaseCorrections3fMessage` | Yes | 99 |
| `5f` | Phase Corrections | Model present | `JavadPhaseCorrections5fMessage` | Yes | 99 |
| `lf` | Phase Corrections | Model present | `JavadPhaseCorrectionslfMessage` | Yes | 99 |
| `DX` | Doppler | Model present | `JavadDopplerDXMessage` | Yes | 99 |
| `DC` | Doppler | Model present | `JavadDopplerDCMessage` | Yes | 99 |
| `D1` | Doppler | Model present | `JavadDopplerD1Message` | Yes | 99 |
| `D2` | Doppler | Model present | `JavadDopplerD2Message` | Yes | 99 |
| `D3` | Doppler | Model present | `JavadDopplerD3Message` | Yes | 99 |
| `D5` | Doppler | Model present | `JavadDopplerD5Message` | Yes | 99 |
| `Dl` | Doppler | Model present | `JavadDopplerDlMessage` | Yes | 99 |
| `0d` | Relative Doppler | Model present | `JavadRelativeDoppler0dMessage` | Yes | 100 |
| `1d` | Relative Doppler | Model present | `JavadRelativeDoppler1dMessage` | Yes | 100 |
| `2d` | Relative Doppler | Model present | `JavadRelativeDoppler2dMessage` | Yes | 100 |
| `3d` | Relative Doppler | Model present | `JavadRelativeDoppler3dMessage` | Yes | 100 |
| `5d` | Relative Doppler | Model present | `JavadRelativeDoppler5dMessage` | Yes | 100 |
| `ld` | Relative Doppler | Model present | `JavadRelativeDopplerldMessage` | Yes | 100 |
| `EC` | SNR | Model present | `JavadSignalToNoiseECMessage` | Yes | 100 |
| `E1` | SNR | Model present | `JavadSignalToNoiseE1Message` | Yes | 100 |
| `E2` | SNR | Model present | `JavadSignalToNoiseE2Message` | Yes | 100 |
| `E3` | SNR | Model present | `JavadSignalToNoiseE3Message` | Yes | 100 |
| `E5` | SNR | Model present | `JavadSignalToNoiseE5Message` | Yes | 100 |
| `El` | SNR | Model present | `JavadSignalToNoiseElMessage` | Yes | 100 |
| `CE` | SNR x 4 | Model present | `JavadSignalToNoiseQuarterCEMessage` | Yes | 101 |
| `1E` | SNR x 4 | Model present | `JavadSignalToNoiseQuarter1EMessage` | Yes | 101 |
| `2E` | SNR x 4 | Model present | `JavadSignalToNoiseQuarter2EMessage` | Yes | 101 |
| `3E` | SNR x 4 | Model present | `JavadSignalToNoiseQuarter3EMessage` | Yes | 101 |
| `5E` | SNR x 4 | Model present | `JavadSignalToNoiseQuarter5EMessage` | Yes | 101 |
| `lE` | SNR x 4 | Model present | `JavadSignalToNoiseQuarterlEMessage` | Yes | 101 |
| `s0` | SNR x 256 | Model present | `JavadSignalToNoise256s0Message` | Yes | 101 |
| `s1` | SNR x 256 | Model present | `JavadSignalToNoise256s1Message` | Yes | 101 |
| `s2` | SNR x 256 | Model present | `JavadSignalToNoise256s2Message` | Yes | 101 |
| `s3` | SNR x 256 | Model present | `JavadSignalToNoise256s3Message` | Yes | 101 |
| `s5` | SNR x 256 | Model present | `JavadSignalToNoise256s5Message` | Yes | 101 |
| `sl` | SNR x 256 | Model present | `JavadSignalToNoise256slMessage` | Yes | 101 |
| `j0` | Data SNR x 256 | Model present | `JavadDataSignalToNoise256j0Message` | Yes | 101 |
| `j1` | Data SNR x 256 | Model present | `JavadDataSignalToNoise256j1Message` | Yes | 101 |
| `j2` | Data SNR x 256 | Model present | `JavadDataSignalToNoise256j2Message` | Yes | 101 |
| `j3` | Data SNR x 256 | Model present | `JavadDataSignalToNoise256j3Message` | Yes | 101 |
| `j5` | Data SNR x 256 | Model present | `JavadDataSignalToNoise256j5Message` | Yes | 101 |
| `jl` | Data SNR x 256 | Model present | `JavadDataSignalToNoise256jlMessage` | Yes | 101 |
| `FC` | Signal Lock Loop Flags | Model present | `JavadSignalLockFlagsFCMessage` | Yes | 101 |
| `F1` | Signal Lock Loop Flags | Model present | `JavadSignalLockFlagsF1Message` | Yes | 101 |
| `F2` | Signal Lock Loop Flags | Model present | `JavadSignalLockFlagsF2Message` | Yes | 101 |
| `F3` | Signal Lock Loop Flags | Model present | `JavadSignalLockFlagsF3Message` | Yes | 101 |
| `F5` | Signal Lock Loop Flags | Model present | `JavadSignalLockFlagsF5Message` | Yes | 101 |
| `Fl` | Signal Lock Loop Flags | Model present | `JavadSignalLockFlagsFlMessage` | Yes | 101 |
| `ec` | Raw Inphases (I) | Model present | `JavadInphaseAmplitudeecMessage` | Yes | 103 |
| `e1` | Raw Inphases (I) | Model present | `JavadInphaseAmplitudee1Message` | Yes | 103 |
| `e2` | Raw Inphases (I) | Model present | `JavadInphaseAmplitudee2Message` | Yes | 103 |
| `e3` | Raw Inphases (I) | Model present | `JavadInphaseAmplitudee3Message` | Yes | 103 |
| `e5` | Raw Inphases (I) | Model present | `JavadInphaseAmplitudee5Message` | Yes | 103 |
| `qc` | Raw Quadratures (Q) | Model present | `JavadQuadratureAmplitudeqcMessage` | Yes | 103 |
| `q1` | Raw Quadratures (Q) | Model present | `JavadQuadratureAmplitudeq1Message` | Yes | 103 |
| `q2` | Raw Quadratures (Q) | Model present | `JavadQuadratureAmplitudeq2Message` | Yes | 103 |
| `q3` | Raw Quadratures (Q) | Model present | `JavadQuadratureAmplitudeq3Message` | Yes | 103 |
| `q5` | Raw Quadratures (Q) | Model present | `JavadQuadratureAmplitudeq5Message` | Yes | 103 |
| `x0` | S4 | Model present | `JavadScintillationS4x0Message` | Yes | 103 |
| `x1` | S4 | Model present | `JavadScintillationS4x1Message` | Yes | 103 |
| `x2` | S4 | Model present | `JavadScintillationS4x2Message` | Yes | 103 |
| `x3` | S4 | Model present | `JavadScintillationS4x3Message` | Yes | 103 |
| `x5` | S4 | Model present | `JavadScintillationS4x5Message` | Yes | 103 |
| `x4` | S4 | Model present | `JavadScintillationS4x4Message` | Yes | 103 |
| `y0` | SigmaPhi | Model present | `JavadScintillationSigmaPhiy0Message` | Yes | 103 |
| `y1` | SigmaPhi | Model present | `JavadScintillationSigmaPhiy1Message` | Yes | 103 |
| `y2` | SigmaPhi | Model present | `JavadScintillationSigmaPhiy2Message` | Yes | 103 |
| `y3` | SigmaPhi | Model present | `JavadScintillationSigmaPhiy3Message` | Yes | 103 |
| `y5` | SigmaPhi | Model present | `JavadScintillationSigmaPhiy5Message` | Yes | 103 |
| `y4` | SigmaPhi | Model present | `JavadScintillationSigmaPhiy4Message` | Yes | 103 |
| `IQ` | 1-millisecond I and Q Samples | Model present | `JavadIqSamplesMessage` | Yes | 104 |
| `TC` | CA/L1 Continuous Tracking Time | Model present | `JavadContinuousTrackingTimeMessage` | Yes | 104 |
| `SS` | Satellite Navigation Status | Model present | `JavadSatelliteNavigationStatusMessage` | Yes | 104 |
| `ID` | Ionospheric Delays | Model present | `JavadIonosphericDelaysMessage` | Yes | 105 |
| `rr` | Satellite Range Residuals | Model present | `JavadRangeResidualsMessage` | Yes | 105 |
| `vr` | Satellite Velocity Residuals | Model present | `JavadVelocityResidualsMessage` | Yes | 105 |
| `GA` | GPS Almanac | Model present | `JavadGpsAlmanacMessage` | Yes | 105 |
| `EA` | GALILEO Almanac | Model present | `JavadGalileoAlmanacMessage` | Yes | 106 |
| `QA` | QZSS Almanac | Model present | `JavadQzssAlmanacMessage` | Yes | 106 |
| `CA` | BeiDou Almanac | Model present | `JavadBeiDouAlmanacMessage` | Yes | 106 |
| `IA` | IRNSS Almanac | Model present | `JavadIrnssAlmanacMessage` | Yes | 106 |
| `NA` | GLONASS Almanac | Model present | `JavadGlonassAlmanacMessage` | Yes | 107 |
| `WA` | SBAS Almanac | Model present | `JavadSbasAlmanacMessage` | Yes | 107 |
| `GE` | GPS Ephemeris | Model present | `JavadGpsEphemerisMessage` | Yes | 108 |
| `EN` | GALILEO Ephemeris | Model present | `JavadGalileoEphemerisMessage` | Yes | 109 |
| `QE` | QZSS Ephemeris | Model present | `JavadQzssEphemerisMessage` | Yes | 110 |
| `CN` | BeiDou Ephemeris | Model present | `JavadBeiDouEphemerisMessage` | Yes | 110 |
| `NE` | GLONASS Ephemeris | Model present | `JavadGlonassEphemerisMessage` | Yes | 111 |
| `WE` | SBAS Ephemeris | Model present | `JavadSbasEphemerisMessage` | Yes | 112 |
| `IE` | IRNSS Ephemeris | Model present | `JavadIrnssEphemerisMessage` | Yes | 112 |
| `gd` | GPS Raw Navigation Data | Model present | `JavadGpsRawNavigationDataMessage` | Yes | 113 |
| `qd` | QZSS Raw Navigation Data | Model present | `JavadQzssRawNavigationDataMessage` | Yes | 114 |
| `lD` | GLONASS Raw Navigation Data | Model present | `JavadGlonassRawNavigationDataMessage` | Yes | 114 |
| `WD` | SBAS Raw Navigation Data | Model present | `JavadSbasRawNavigationDataMessage` | Yes | 114 |
| `ED` | GALILEO Raw Navigation Data | Model present | `JavadGalileoRawNavigationDataMessage` | Yes | 115 |
| `cd` | BeiDou Raw Navigation Data | Model present | `JavadBeiDouRawNavigationDataMessage` | Yes | 116 |
| `id` | IRNSS Raw Navigation Data | Model present | `JavadIrnssRawNavigationDataMessage` | Yes | 117 |
| `xd` | QZSS L6 Raw Navigation Data | Model present | `JavadQzssL6RawNavigationDataMessage` | Yes | 117 |
| `sp` | Spectrum | Model present | `JavadSpectrumMessage` | Yes | 118 |
| `sP` | Extended Spectrum | Model present | `JavadExtendedSpectrumMessage` | Yes | 118 |
| `Sp` | Single Spectrum | Model present | `JavadSingleSpectrumMessage` | Yes | 118 |
| `ms` | Modem Spectrum | Model present | `JavadModemSpectrumMessage` | Yes | 119 |
| `gC` | GLONASS Delays | Model present | `JavadGlonassDelaysgCMessage` | Yes | 119 |
| `g1` | GLONASS Delays | Model present | `JavadGlonassDelaysg1Message` | Yes | 119 |
| `g2` | GLONASS Delays | Model present | `JavadGlonassDelaysg2Message` | Yes | 119 |
| `g3` | GLONASS Delays | Model present | `JavadGlonassDelaysg3Message` | Yes | 119 |
| `gR` | Code Delays of Receiver RF Bands | Model present | `JavadReceiverBandDelaysMessage` | Yes | 120 |
| `MR` | Rotation Matrix | Model present | `JavadRotationMatrixMessage` | Yes | 121 |
| `mr` | Rotation Matrix and Vectors | Model present | `JavadRotationMatrixVectorsMessage` | Yes | 121 |
| `AR` | Rotation Angles | Model present | `JavadRotationAnglesMessage` | Yes | 121 |
| `AV` | Angular Velocities | Model present | `JavadAngularVelocityMessage` | Yes | 121 |
| `IM` | Inertial Measurements | Model present | `JavadInertialMeasurementsMessage` | Yes | 122 |
| `MA` | Accelerometer and Magnetometer Measurements | Model present | `JavadAccelerometerMagnetometerMessage` | Yes | 122 |
| `fA` | Accelerometer Raw Data | Model present | `JavadAccelerometerRawMessage` | Yes | 122 |
| `lA` | Accelerometer Raw Data | Model present | `JavadAccelerometerLRawMessage` | Yes | 123 |
| `tA` | Accelerometer and Gyroscope calibrated Data | Model present | `JavadCalibratedInertialMeasurementsMessage` | Yes | 123 |
| `aV` | Gyroscope Raw Data | Model present | `JavadGyroscopeRawMessage` | Yes | 123 |
| `mA` | Magnetometer Raw Data | Model present | `JavadMagnetometerRawMessage` | Yes | 123 |
| `dV` | Acceleration in ENU | Model present | `JavadEnuAccelerationMessage` | Yes | 124 |
| `PE` | WGS84 Coordinates of the Pole End | Model present | `JavadPoleEndCoordinatesMessage` | Yes | 124 |
| `pV` | Local Coordinates of Antenna Phase Center | Model present | `JavadAntennaPhaseCenterLocalCoordinatesMessage` | Yes | 124 |
| `pE` | Tilt-compensated Solution (full) | Model present | `JavadTiltCompensatedSolutionMessage` | Yes | 124 |
| `pe` | Tilt-compensated Solution (short) | Model present | `JavadTiltCompensatedSolutionShortMessage` | Yes | 126 |
| `XA` | External Event | Model present | `JavadExternalEventXAMessage` | Yes | 127 |
| `XB` | External Event | Model present | `JavadExternalEventXBMessage` | Yes | 127 |
| `ZA` | PPS Offset | Model present | `JavadPpsOffsetZAMessage` | Yes | 127 |
| `ZB` | PPS Offset | Model present | `JavadPpsOffsetZBMessage` | Yes | 127 |
| `YA` | Time Offset at PPS Generation Time | Model present | `JavadPpsTimeOffsetYAMessage` | Yes | 128 |
| `YB` | Time Offset at PPS Generation Time | Model present | `JavadPpsTimeOffsetYBMessage` | Yes | 128 |
| `ha` | Heading and Pitch | Model present | `JavadHeadingPitchMessage` | Yes | 128 |
| `RO` | Lever Arm Cartesian Position | Model present | `JavadLeverArmCartesianPositionMessage` | Yes | 128 |
| `RG` | Lever Arm Geodetic Position | Model present | `JavadLeverArmGeodeticPositionMessage` | Yes | 128 |
| `RE` | Reply | Model present | `JavadReplyMessage` | Yes | 129 |
| `ER` | Error | Model present | `JavadErrorMessage` | Yes | 129 |
| `IO` | GPS Ionospheric Parameters | Model present | `JavadGpsIonosphericParametersMessage` | Yes | 129 |
| `QI` | QZSS Ionospheric Parameters | Model present | `JavadQzssIonosphericParametersMessage` | Yes | 130 |
| `CI` | BeiDou Ionospheric Parameters | Model present | `JavadBeiDouIonosphericParametersMessage` | Yes | 130 |
| `II` | IRNSS Ionospheric Parameters | Model present | `JavadIrnssIonosphericParametersMessage` | Yes | 130 |
| `==` | Event | Model present | `JavadEventMessage` | Yes | 131 |
| `LT` | Message Output Latency | Model present | `JavadMessageOutputLatencyMessage` | Yes | 131 |
| `>>` | Wrapper | Model present | `JavadWrapperMessage` | Yes | 131 |
| `PM` | Parameters | Model present | `JavadParametersMessage` | Yes | 132 |
| `LH` | Logging History | Model present | `JavadLoggingHistoryMessage` | Yes | 133 |
| `AI` | Antenna Information | Model present | `JavadAntennaInformationMessage` | Yes | 134 |
| `BI` | Base Station Information | Model present | `JavadBaseStationInformationMessage` | Yes | 134 |
| `SE` | Security | Model present | `JavadSecurityMessage` | Yes | 134 |
| `SM` | Security for [rM] | Model present | `JavadSecurityRmMessage` | Yes | 134 |
| `TT` | CA/L1 Overall Continuous Tracking Time | Model present | `JavadOverallContinuousTrackingTimeMessage` | Yes | 134 |
| `OO` | Oscillator Offset | Model present | `JavadOscillatorOffsetMessage` | Yes | 135 |
| `&#124;&#124;` | Epoch End | Model present | `JavadEpochEndMessage` | Yes | 135 |
| `DL` | Data Link Status | Model present | `JavadDataLinkStatusMessage` | Yes | 137 |
| `GS` | GPS SVs Status | Model present | `JavadGpsSatelliteStatusMessage` | Yes | 138 |
| `ES` | GALILEO SVs Status | Model present | `JavadGalileoSatelliteStatusMessage` | Yes | 139 |
| `WS` | SBAS SVs Status | Model present | `JavadSbasSatelliteStatusMessage` | Yes | 140 |
| `NS` | GLONASS SVs Status | Model present | `JavadGlonassSatelliteStatusMessage` | Yes | 140 |
| `QS` | QZSS SVs Status | Model present | `JavadQzssSatelliteStatusMessage` | Yes | 141 |
| `CS` | BeiDou SVs Status | Model present | `JavadBeiDouSatelliteStatusMessage` | Yes | 142 |
| `Is` | IRNSS SVs Status | Model present | `JavadIrnssSatelliteStatusMessage` | Yes | 142 |
| `LS` | L-band SVs Status | Model present | `JavadLBandSatelliteStatusMessage` | Yes | 143 |
| `RS` | Reference Station Status | Model present | `JavadReferenceStationStatusMessage` | Yes | 144 |
| `TX` | RTCM/CMR Text Message | Model present | `JavadCorrectionTextMessage` | Yes | 144 |
| `RM` | Results of RAIM Processing | Model present | `JavadRaimMessage` | Yes | 145 |
| `NP` | Navigation Position | Model present | `JavadNavigationPositionMessage` | Yes | 146 |
| `MP` | Position in Map Projection | Model present | `JavadMapProjectionMessage` | Yes | 148 |
| `NR` | Lever Arm Position | Model present | `JavadLeverArmPositionTextMessage` | Yes | 150 |
| `SY` | Geographic and Quasi Geographic Position | Model present | `JavadQuasiGeographicPositionMessage` | Yes | 151 |
| `TR` | Time Residuals | Model present | `JavadTimeResidualsMessage` | Yes | 151 |
| `TM` | Clock Offsets and Time Derivatives | Model present | `JavadClockOffsetsMessage` | Yes | 153 |
| `RP` | Reference Station Parameters | Model present | `JavadReferenceStationParametersMessage` | Yes | 154 |
| `RK` | RTK Solution Parameters | Model present | `JavadRtkParametersMessage` | Yes | 155 |
| `AP` | Position Covariance Matrix | Model present | `JavadPositionCovarianceTextMessage` | Yes | 156 |
| `AB` | Baseline | Model present | `JavadBaselineTextMessage` | Yes | 157 |
| `QQ` | IMU Attitude Angles | Model present | `JavadImuAttitudeMessage` | Yes | 157 |
| `WW` | IMU Measurements | Model present | `JavadImuMeasurementsMessage` | Yes | 158 |
| `ZZ` | IMU Integrated Antenna Velocities | Model present | `JavadImuAntennaVelocityMessage` | Yes | 159 |
| `TD` | Text Data | Model present | `JavadTextDataMessage` | Yes | 159 |
| `SI` | Satellite indices (obsolete USI form; compatibility section) | Model present | `JavadSatelliteIndicesMessage` | Yes | 87 |
| `sj` | Spoofing/Jamming Information | Model present | `JavadSpoofingJammingMessage` | Yes | 130 |
| `rE` | Reference Epoch (obsolete) | Model present | `JavadLegacyReferenceEpochMessage` | Yes | 556 |
| `rM` | Raw Measurements (obsolete) | Model present | `JavadLegacyRawMeasurementsMessage` | Yes | 557 |
| `rV` | Receiver Position and Velocity (obsolete) | Model present | `JavadLegacyPositionVelocityMessage` | Yes | 559 |
| `rT` | Receiver Clock Offsets (obsolete) | Model present | `JavadLegacyClockOffsetsMessage` | Yes | 560 |
| `GD` | GPS Raw Navigation Data (obsolete) | Model present | `JavadLegacyGpsNavigationGDMessage` | Yes | 561 |
| `QD` | QZSS Raw Navigation Data (obsolete) | Model present | `JavadLegacyGpsNavigationQDMessage` | Yes | 561 |
| `LD` | GLONASS Raw Navigation Data (obsolete) | Model present | `JavadLegacyGlonassNavigationMessage` | Yes | 562 |
| `MS` | RTCM 2.x Status (obsolete) | Model present | `JavadRtcmStatusMessage` | Yes | 562 |

[All protocols](supported-protocols.md)
