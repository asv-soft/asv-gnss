# Raw GLONASS navigation

This module decodes GLONASS navigation strings 1–15 from receiver-provided `uint[]` navigation words. It is a payload decoder, not an independent byte-stream protocol registered with `Asv.IO`.

## Decode navigation words

```csharp
using Asv.Gnss;

// navigationWords is a uint[] obtained from a compatible receiver payload.
GlonassWordBase decoded = GlonassWordFactory.Create(navigationWords);
```

Supply the word layout expected by the factory, rather than arbitrary RF samples or bytes from a receiver port. The module does not perform signal acquisition or tracking.

## Coverage

Strings 1–5 have dedicated classes. Even strings 6–14 use `GlonassWordEven`; odd strings 7–15 use `GlonassWordOdd`. Unknown IDs throw an exception.

## Receiver integration

The decoder is used by the corresponding raw-navigation messages in [UBX](protocol-ubx.md), [SBF](protocol-sbf.md), [ComNav](protocol-comnav.md), and [ASV](protocol-asv.md). For example, `UbxRxmSfrbx` exposes decoded `GlonassWord` data when applicable. Keep the satellite and time context supplied by the enclosing message alongside the decoded navigation content.

## Implementation reference

See `src/Asv.Gnss/Protocols/RawGlonass/GlonassWordFactory.cs` and the concrete models under that directory. The factory dispatches supported IDs; it does not guarantee that every almanac page or navigation extension is decoded into dedicated fields.

## Specification message catalog

Source: `Protocols/Rtcm/V3/Docs/ICD_GLONASS_5.1_(2008)_en.pdf`, GLONASS ICD edition 5.1 (2008), navigation-message structure. The legacy frame contains strings 1-15. Strings 6-15 use repeated almanac layouts and frame-dependent content; special fifth-frame contents are not established as fully decoded merely because the factory accepts the string number. This table covers legacy L1/L2 strings, not modern CDMA message types.

**Status:** **Model present** means a concrete payload model exists in the current checkout; it does not certify complete field, revision, encoding, or firmware coverage. **Partial** identifies a known incomplete decoder. **Not implemented** means no matching concrete model was found. **Default** reports whether the standard registration adds that model. Models outside the default set require explicit registration. See the limitations above before transmitting messages.

Page references below are **PDF page numbers (1-based)**, including covers and front matter.

| String number | Status | C# model | Integration |
| --- | --- | --- | --- |
| 1 | Model present | `GlonassWord1` | Factory dispatch |
| 2 | Model present | `GlonassWord2` | Factory dispatch |
| 3 | Model present | `GlonassWord3` | Factory dispatch |
| 4 | Model present | `GlonassWord4` | Factory dispatch |
| 5 | Model present | `GlonassWord5` | Factory dispatch |
| 6 | Shared even/odd model | `GlonassWordEven` | Factory dispatch |
| 7 | Shared even/odd model | `GlonassWordOdd` | Factory dispatch |
| 8 | Shared even/odd model | `GlonassWordEven` | Factory dispatch |
| 9 | Shared even/odd model | `GlonassWordOdd` | Factory dispatch |
| 10 | Shared even/odd model | `GlonassWordEven` | Factory dispatch |
| 11 | Shared even/odd model | `GlonassWordOdd` | Factory dispatch |
| 12 | Shared even/odd model | `GlonassWordEven` | Factory dispatch |
| 13 | Shared even/odd model | `GlonassWordOdd` | Factory dispatch |
| 14 | Shared even/odd model | `GlonassWordEven` | Factory dispatch |
| 15 | Shared even/odd model | `GlonassWordOdd` | Factory dispatch |


[All protocols](supported-protocols.md)
