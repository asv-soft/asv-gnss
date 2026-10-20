# Raw GPS navigation

This module decodes GPS navigation subframes 1–5 from receiver-provided `uint[]` navigation words. It is a payload decoder, not an independent byte-stream protocol registered with `Asv.IO`.

## Decode navigation words

```csharp
using Asv.Gnss;

// navigationWords is a uint[] obtained from a compatible receiver payload.
GpsSubframeBase decoded = GpsSubFrameFactory.Create(navigationWords);
```

Supply the word layout expected by the factory, rather than arbitrary RF samples or bytes from a receiver port. The module does not perform signal acquisition or tracking.

## Coverage

The factory checks the preamble, extracts the subframe ID and time of week, removes parity bits, and dispatches to `GpsSubframe1` through `GpsSubFrame5`. Removing parity bits is not a claim of full parity validation.

## Receiver integration

The decoder is used by the corresponding raw-navigation messages in [UBX](protocol-ubx.md), [SBF](protocol-sbf.md), [ComNav](protocol-comnav.md), and [ASV](protocol-asv.md). For example, `UbxRxmSfrbx` exposes decoded `GpsSubFrame` data when applicable. Keep the satellite and time context supplied by the enclosing message alongside the decoded navigation content.

## Implementation reference

See `src/Asv.Gnss/Protocols/RawGps/GpsSubFrameFactory.cs` and the concrete models under that directory. The factory dispatches supported IDs; it does not guarantee that every almanac page or navigation extension is decoded into dedicated fields.

## Specification message catalog

Source: `Protocols/Rtcm/V3/Docs/1995-SPS-signal-specification.pdf`, GPS SPS Signal Specification, 2nd edition, June 2, 1995, section 2.4.1 (PDF pp. 24-25). The legacy navigation frame has five numbered subframes. Subframes 4 and 5 carry page-dependent content; a subframe decoder does not imply dedicated decoding of every page. These are not CNAV or CNAV-2 message types.

**Status:** **Model present** means a concrete payload model exists in the current checkout; it does not certify complete field, revision, encoding, or firmware coverage. **Partial** identifies a known incomplete decoder. **Not implemented** means no matching concrete model was found. **Default** reports whether the standard registration adds that model. Models outside the default set require explicit registration. See the limitations above before transmitting messages.

Page references below are **PDF page numbers (1-based)**, including covers and front matter.

| Subframe | Content group | Status | C# model | Integration |
| --- | --- | --- | --- | --- |
| 1 | Clock and health | Model present | `GpsSubframe1` | Factory dispatch |
| 2 | Ephemeris, first part | Model present | `GpsSubframe2` | Factory dispatch |
| 3 | Ephemeris, second part | Model present | `GpsSubframe3` | Factory dispatch |
| 4 | Page-dependent navigation and almanac data | Model present (selected page layouts) | `GpsSubframe4` | Factory dispatch |
| 5 | Almanac and page-dependent data | Model present (selected page layouts) | `GpsSubFrame5` | Factory dispatch |


[All protocols](supported-protocols.md)
