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

## Coverage and limitations

This protocol covers legacy GLONASS L1/L2 FDMA strings, not modern CDMA messages or RF demodulation. Strings 1–5 have dedicated layouts. Strings 6–15 have individually registered classes that reuse the existing even/odd almanac layouts. Special contents of strings 14 and 15 in the fifth frame require external frame context and are not promoted to a complete typed model by this registration. All raw bits are retained for reserialization.

Existing constructor defaults are unchanged. Initialize the necessary fields before serializing a new model; for example, string 4 needs a supported `Accuracy`, and almanac strings need representable inclination or frequency values. String 1's `P1` now reads ICD bits 77–78; the former decoder incorrectly read bits belonging to the time field.

## Specification message catalog

Source: local `Protocols/Rtcm/V3/Docs/ICD_GLONASS_5.1_(2008)_en.pdf`, GLONASS ICD edition 5.1 (2008), sections 4.3.3 and 4.7. These define the 85-bit string and the Hamming equations. Registration covers string numbers, not every frame-dependent interpretation.

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
