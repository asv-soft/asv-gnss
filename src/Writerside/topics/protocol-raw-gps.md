# Raw GPS navigation

`RegisterRawGpsProtocol` registers legacy GPS LNAV subframes 1–5 as ordinary `Asv.IO` messages. `GpsSubframeBase` implements `IProtocolMessage<byte>` and `IVisitable`: messages have protocol metadata, typed fields, span serialization and visitor support. The streaming parser checks all ten words, including chained parity and data inversion.

## Register the protocol

```c#
using Asv.Gnss;
using Asv.IO;

var protocol = Protocol.Create(builder => builder.Protocols.RegisterRawGpsProtocol());
using var parser = protocol.CreateParser(RawGpsProtocol.Info.Id);
```

The registration accepts a factory configuration callback after the five default models have been added. Use a separate navigation stream for each satellite, retaining satellite identity and reception time from the receiver or demodulator. Neither identity is supplied by the LNAV subframe itself.

## Native bits and byte representation

One subframe contains 300 transmitted bits: ten 30-bit words, each with 24 data bits and six parity bits. Input to `GpsSubframeParser.Push(byte)` is MSB-first. It searches bit boundaries, so adjacent subframes can share a byte and input chunks need not coincide with subframes. It handles the previous word's D29*/D30* parity state and undoes the D30*-controlled data inversion before decoding fields.

`Serialize(ref Span<byte>)` and `Deserialize(ref ReadOnlySpan<byte>)` use a fixed 38-byte representation: four leading zero padding bits followed by the 300 transmitted bits, MSB-first. This padding belongs to the byte representation, not the satellite signal. Concatenated padded messages are accepted by the parser. To build a continuous navigation bit stream, concatenate only the 300 meaningful bits from each message; do not transmit the four padding bits.

`PreviousWordParity` stores the two preceding parity bits (bit 1 is D29*, bit 0 is D30*). Deserialization determines this state while validating parity. A parity error rejects the subframe; decoding does not silently correct it. Typed field edits are reflected in serialization, with parity and transmitted data inversion recalculated. Unmodeled and reserved payload bits are retained. For new or edited frames, the serializer selects the non-information bits d23/d24 of the HOW and final word so their D29/D30 bits are zero; an unchanged received frame retains its original bits.

A parser publishes each decoded message immediately. Its `Push` return value is `true` only when a message ends at the end of the supplied byte. A message ending inside that byte is still published, while `Push` returns `false` and retains the next subframe's initial bits. This lets the normal `Asv.IO` dispatcher preserve those bits. `Reset()` discards incomplete framing state.

## Receiver compatibility

The existing receiver-oriented API remains available:

```c#
using Asv.Gnss;

// navigationWords contains the receiver's ten uint navigation words.
GpsSubframeBase decoded = GpsSubFrameFactory.Create(navigationWords);
```

That API keeps its existing behavior: it extracts 24 data bits from each receiver word and dispatches the subframe without adding a parity requirement. Existing `Deserialize(byte[])` calls still consume exactly 30 parity-stripped data bytes. These legacy entry points initialize `PreviousWordParity` to zero because they did not retain the preceding word's parity; callers can supply a known state before native serialization. Some receiver messages have already de-inverted their data or do not retain valid parity; their uint arrays must not be treated as a packed native bit stream.

The receiver-oriented decoder is used by [UBX](protocol-ubx.md), [SBF](protocol-sbf.md), [ComNav](protocol-comnav.md), and [ASV](protocol-asv.md). For example, `UbxRxmSfrbx.GpsSubFrame` now also supports the common message and visitor contracts. No receiver envelope is added to native GPS serialization.

## Fields and pages

Every subframe exposes the 14-bit `TelemetryMessage`, `IntegrityStatusFlag`, HOW `AlertFlag` and `AntiSpoofFlag`, together with the existing TOW and preceding-word parity state. The reserved TLM bit is retained. Subframe 2 adds `Aodo`: the transmitted five-bit age-of-data offset in 900-second units. Value 31 indicates invalid NMCT data; it is retained as a wire code, not converted into an ordinary age.

Subframe 4 has the following layouts. Page positions describe the 25-page navigation cycle; `PageId` is the transmitted six-bit SV/page identifier, which is a different number.

| Cycle page position | PageId | Typed content |
| --- | --- | --- |
| 2–5, 7–10 | 25–32 | Satellite almanacs |
| 13 | 52 | NMCT availability indicator and 30 estimated range deviations |
| 17 | 55 | 22-byte special message |
| 18 | 56 | Ionosphere and UTC, including future leap-second parameters |
| 25 | 63 | Anti-spoof/configuration and health data |
| Remaining positions | 53, 54, 57–62 | Reserved content retained as raw page data |

NMCT exposes `NmctAvailabilityIndicator` and `NmctEstimatedRangeDeviations`. AI=0 means unencrypted data, AI=1 encrypted data, and AI=2/3 unavailable data. Each array element retains the signed six-bit wire code: multiply by 0.3 for metres when the data is usable; -32 denotes unavailable ERD. The array contains 30 transmitted slots. Associating slots with satellites requires the transmitting satellite identity from the enclosing receiver message. Encrypted slots are retained without attempting decryption.

`SpecialMessageData` contains exactly 22 bytes. This avoids losing the specification's degree symbol (`0xF8`) and preserves received control or reserved character codes. The special-message character table extends ordinary ASCII; do not use a lossy ASCII conversion to round-trip its bytes.

`UtcGps` adds `WN_LSF`, `DN` and signed `DeltaT_LSF`. The week is the transmitted eight-bit week number, `DN` is the transmitted day code, and `DeltaT_LSF` is the future GPS–UTC leap-second offset in seconds. Resolving a truncated week or applying the leap-second schedule remains the caller's responsibility.

Subframe 5 covers almanac pages 1–24 and page 25 (`PageId=51`) containing reference time, week and 24 satellite health codes. All typed fields, nested UTC/ionosphere fields, arrays and raw page bytes participate in `IVisitor` traversal and editing.

## Page identity and reserved data

Subframes 4 and 5 expose `DataId` (two transmitted bits) and `PageId` (six transmitted bits). The existing layouts use `DataId=1`. Other data identifiers and reserved page IDs are retained as `Type.None`. Repeated reserved IDs mean a unique cycle position cannot be inferred from `PageId` alone. The original enum values remain unchanged; the new subframe-4 values are appended as `Nmct` and `SpecialMessage`.

For a known layout, the existing `Type` and `SatelliteNumber` properties remain authoritative. For an unknown or reserved layout, `RawPageData` exposes the editable 23-byte backing array after the page identifiers. Its getter returns null for a typed layout; assigning raw bytes requires `Type.None` and exactly 23 bytes. These include the final word's two non-information bits; edited native serialization may recompute those two bits to satisfy parity. Selecting an explicit unknown `PageId` or nonstandard `DataId` supplies the identity needed to create a raw page.

## Coverage and compatibility limits

This implementation covers legacy LNAV, not CNAV, CNAV-2, RF samples, signal acquisition or tracking. Reserved and unknown page data is preserved without inventing a semantic layout. Existing public numerical representations are retained for compatibility; adding fields does not replace the existing ephemeris or time calculations.

In particular, the legacy almanac `Af0` property combines its split wire fields using the existing decoder convention, rather than exposing a new signed 11-bit value. Unchanged encodings are preserved; edited values must fit that convention. Existing `Fit`, almanac `e`, UTC `Tot` and `DeltaT_LS` representations also retain their prior decoder semantics. Do not infer corrected navigation calculations from the addition of stream framing and parity validation.

Initialize the required fields and arrays before serializing a newly constructed page model. Changing `Type` to a supported page layout requires that layout's fields. `None` represents an unmodeled page. Changing a known page to `None` without supplying an explicit raw-page identity remains rejected because it does not specify a new page identifier.

## Recorded-stream tests

`Asv.Gnss.Test/Resources/RawNavigation` includes a real GFZ GPS C/A recording from 2025-01-01, PRN 1: 125 consecutive subframes covering one complete 25-frame page cycle. The 4,688-byte stream retains all 37,500 native bits, including original parity and D30* inversion, with only four final zero padding bits. The unchanged source Arrow container, selected source rows, provenance, licenses and an offline extraction script are retained alongside it.

`RawNavigationRecordedStreamTest` checks all recorded subframes and page IDs, exact native-bit preservation through serialization, independent parity equations, read sizes from 1 to 4096 bytes, and an incomplete final subframe. It also checks the recorded NMCT, special message, future UTC fields and dummy page with `PageId=0`. These are recorded navigation bits after demodulation, not RF samples. The [fixture documentation](https://github.com/asv-soft/asv-gnss/tree/main/src/Asv.Gnss.Test/Resources/RawNavigation) describes sources and reproducible extraction; tests run offline.

## Specification message catalog

The legacy models originated from the local `Protocols/Rtcm/V3/Docs/1995-SPS-signal-specification.pdf`. Header fields, AODO, page layouts, NMCT, special messages and leap-second fields are checked against [IS-GPS-200N, sections 20.3.3.1–2, 20.3.3.4 and 20.3.3.5](https://prod-01-alb-www-gps.woc.noaa.gov/sites/default/files/2025-07/IS-GPS-200N.pdf). Native parity follows section 20.3.5.2 and Table 20-XIV of the same specification. NMCT is defined by this newer baseline, rather than the reserved page shown in the 1995 document.

| Subframe | Content group | C# model | Default registration |
| --- | --- | --- | --- |
| 1 | Clock and health | `GpsSubframe1` | Yes |
| 2 | Ephemeris, first part, and AODO | `GpsSubframe2` | Yes |
| 3 | Ephemeris, second part | `GpsSubframe3` | Yes |
| 4 | Almanac, NMCT, special message, ionosphere/UTC and health/configuration | `GpsSubframe4` | Yes |
| 5 | Almanac and health/time pages | `GpsSubFrame5` | Yes |

Registration does not imply that every page or future navigation extension has a dedicated typed view.

[All protocols](supported-protocols.md)
