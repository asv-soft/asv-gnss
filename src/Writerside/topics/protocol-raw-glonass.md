# Raw GLONASS navigation

`RegisterRawGlonassProtocol` registers legacy GLONASS FDMA strings 1–15 as ordinary `Asv.IO` messages. `GlonassWordBase` implements `IProtocolMessage<byte>` and `IVisitable`: messages have protocol metadata, typed fields, span serialization and visitor support. The streaming parser validates the eight Hamming check bits before publishing a string.

## Register the protocol

```c#
using Asv.Gnss;
using Asv.IO;

var protocol = Protocol.Create(builder => builder.Protocols.RegisterRawGlonassProtocol());
using var parser = protocol.CreateParser(RawGlonassProtocol.Info.Id);
```

The registration accepts a factory configuration callback after the fifteen default IDs have been added. Keep a separate navigation stream per satellite and retain satellite identity, reception time and frame context from the receiver or demodulator.

**Use this parser explicitly on a GLONASS navigation stream.** An 85-bit string has no distinctive byte sync marker, and Hamming validation alone cannot reliably identify GLONASS inside arbitrary receiver traffic. The parser is not automatically added to mixed-file protocol detection.

## Native bits and byte representation

One string contains 85 transmitted navigation bits, including the idle bit and eight Hamming check bits. `GlonassWordParser.Push(byte)` receives MSB-first bits and searches across byte boundaries. It accepts contiguous 85-bit strings and individually padded strings. Input must already have navigation data separated from signal modulation and time marks.

`Serialize(ref Span<byte>)` and `Deserialize(ref ReadOnlySpan<byte>)` use 11 bytes: three leading zero padding bits followed by ICD bits 85 through 1, MSB-first. The idle bit 85 is zero; the string number occupies the first byte's low nibble. The final byte contains the Hamming check bits. To create a continuous navigation bit stream, concatenate only the 85 meaningful bits from each message.

The span API validates the padding, idle bit, string number and Hamming checks. Corrupt strings are rejected without silent correction. Serialization encodes edited typed fields, preserves unmodeled and reserved bits and regenerates the check bits. Sign-magnitude fields retain their sign representation, including unedited negative zero.

A parser publishes decoded messages immediately. `Push` returns `true` only when a string ends on the last bit of the supplied byte. If a string ends earlier in the byte, the message is still published and the parser keeps the following bits while returning `false`. The normal `Asv.IO` dispatcher therefore does not discard the start of the next string. `Reset()` discards incomplete framing state.

## Receiver compatibility

The existing receiver-oriented API remains available:

```c#
using Asv.Gnss;

// navigationWords contains the receiver's three uint navigation words.
GlonassWordBase decoded = GlonassWordFactory.Create(navigationWords);
```

The uint layout is 85 bits left-aligned across three words. The factory converts it to the existing 11-byte representation. Existing `Create(uint[])` and `Deserialize(byte[])` calls retain their permissive receiver-oriented validation; strict Hamming validation belongs to the new span API and stream parser. This preserves receivers whose enclosing messages carry separate validity flags.

The decoder remains usable from the corresponding receiver messages, including `UbxRxmSfrbx.GlonassWord`. Keep satellite and frame context from those enclosing messages alongside the decoded content. The shared `GlonassWordEven` and `GlonassWordOdd` classes remain available for existing callers.

## Almanac health and fifth-frame layouts

All odd almanac strings (7, 9, 11, 13 and 15) expose `ln`, the health flag at ICD bit 9. It is decoded, serialized and editable through `IVisitor`, including the existing shared `GlonassWordOdd` model.

Strings 14 and 15 have different contents in the fifth frame. Their concrete models expose `FrameNumber`: 0 means unknown, 1–4 select the almanac layout, and 5 selects the special layout. The default remains 0 with the existing almanac behavior. A standalone string does not contain its frame number; the parser does not guess it.

| Model with FrameNumber=5 | Fields | Wire meaning |
| --- | --- | --- |
| `GlonassWord14` | `B1`, `B2`, `KP` | UT1 correction coefficients and UTC leap-second notification |
| `GlonassWord15` | `ReservedData`, `ln` | 71 reserved bits and the health flag |

`B1` is in seconds (11-bit sign-magnitude, resolution 2^-10); `B2` is in seconds per mean solar day (10-bit sign-magnitude, resolution 2^-16). `KP` retains all four two-bit codes: 0 no correction, 1 +1 second, 3 -1 second, and 2 the ICD's pending-decision state. The remaining bits of string 14 are retained without interpretation.

`ReservedData` returns a copy of nine bytes: one leading zero padding bit followed by ICD bits 80 through 10, MSB-first. Assign the edited array back to update those bits; assignment is allowed only in frame 5. The health bit is edited separately through `ln`. Native serialization regenerates Hamming checks.

Select context before populating a new message. For received messages, context can be supplied before or after deserialization:

```c#
var message = new GlonassWord14 { FrameNumber = 5 };
ReadOnlySpan<byte> remaining = capturedString;
message.Deserialize(ref remaining);
Console.WriteLine($"B1={message.B1} s, B2={message.B2} s/day, KP={message.KP}");
```

Changing between the almanac and fifth-frame layouts on a decoded message first preserves the active field edits, then reinterprets the same payload bits. Assigning the same layout does not reset edits. The selected context remains in effect when the object is reused for another deserialization. Visitors expose `FrameNumber` first and then only the selected layout; visitors can also change that context. Inactive almanac properties do not overwrite a fifth-frame payload.

The old `GlonassWordFactory.Create(uint[])` intentionally continues returning the shared even/odd types. To apply frame context to such receiver data, deserialize its `RawData` into a concrete `GlonassWord14` or `GlonassWord15` configured with `FrameNumber=5`. The ordinary protocol factory already returns the concrete classes.

## Coverage and limitations

This protocol covers legacy GLONASS L1/L2 FDMA strings, not modern CDMA messages or RF demodulation. Strings 1–5 have dedicated layouts. Strings 6–15 have individually registered classes, with even/odd almanac content and explicit fifth-frame layouts for strings 14/15. `RawData` stores the backing payload and is updated by layout changes or `ReservedData` assignments. Typed fields determine modeled bits during serialization, while unmodeled bits are preserved.

Existing constructor defaults are unchanged. Initialize the necessary fields before serializing a new model; for example, string 4 needs a supported `Accuracy`, and almanac strings need representable inclination or frequency values. String 1's `P1` now reads ICD bits 77–78; the former decoder incorrectly read bits belonging to the time field.

## Recorded-stream tests

`Asv.Gnss.Test/Resources/RawNavigation` includes a complete recorded GLONASS superframe from GPSD's 2023-06-28 Moscow capture: slot 12, five frames of 15 strings. The 797-byte native stream contains 6,375 received navigation and Hamming bits, followed by one zero padding bit. The unchanged source UBX capture, provenance, license and offline extraction script are retained alongside it. Extraction removes receiver framing and padding without rebuilding check bits.

`RawNavigationRecordedStreamTest` compares every selected bit against the original receiver packets, independently checks all 584 GLONASS strings in the source capture, and replays all 75 selected strings at several read sizes. It verifies exact serialization, truncated-stream handling and fifth-frame B1/B2/KP and reserved data. Frame numbers come from captured receiver metadata and remain external context; they are not added to the native stream. The recording contains demodulated 85-bit strings without GLONASS relative/bi-binary encoding or time marks. See the [fixture documentation](https://github.com/asv-soft/asv-gnss/tree/main/src/Asv.Gnss.Test/Resources/RawNavigation) for sources and offline reproduction.

## Specification message catalog

Source: local `Protocols/Rtcm/V3/Docs/ICD_GLONASS_5.1_(2008)_en.pdf`, GLONASS ICD edition 5.1 (2008), sections 4.3.2–4.3.3 and 4.7. Tables 4.6 and 4.9 define health and almanac fields; tables 4.11/4.12 define the fifth-frame strings. Section 4.7 defines the Hamming equations. Frame-dependent interpretation requires the explicit context described above.

| String number | C# model | Default registration |
| --- | --- | --- |
| 1 | `GlonassWord1` | Yes |
| 2 | `GlonassWord2` | Yes |
| 3 | `GlonassWord3` | Yes |
| 4 | `GlonassWord4` | Yes |
| 5 | `GlonassWord5` | Yes |
| 6 | `GlonassWord6` | Yes |
| 7 | `GlonassWord7` | Yes |
| 8 | `GlonassWord8` | Yes |
| 9 | `GlonassWord9` | Yes |
| 10 | `GlonassWord10` | Yes |
| 11 | `GlonassWord11` | Yes |
| 12 | `GlonassWord12` | Yes |
| 13 | `GlonassWord13` | Yes |
| 14 | `GlonassWord14` | Yes |
| 15 | `GlonassWord15` | Yes |

[All protocols](supported-protocols.md)
