# ADS-B and native Mode S

`AdsbProtocol.RegisterAdsbProtocol` registers `AdsbParser` for receiver-framed
56-bit and 112-bit Mode S downlinks. The protocol ID remains `ADS-B`.
This is a byte-stream codec for AVR and Beast receiver output, not an RF
1090 MHz demodulator or a UAT decoder. ASTERIX CAT021 uses a separate
[ASTERIX](protocol-asterix.md) transport.

## Registration and input

```c#
using System.Text;
using Asv.Gnss;
using Asv.IO;
using R3;

var protocol = Protocol.Create(builder => builder.Protocols.RegisterAdsbProtocol());
using var parser = protocol.CreateParser(AdsbProtocol.Info.Id);
using var subscription = parser.OnMessage.Subscribe(message =>
{
    var downlink = (AdsbDfMessage)message;
    Console.WriteLine($"DF{downlink.DownlinkFormat}: {downlink.ExtendedSquitter?.Identification?.Callsign}");
});
foreach (var value in Encoding.ASCII.GetBytes("*8D4840D6202CC371C32CE0576098;"))
{
    parser.Push(value);
}
```

The example contains the published KLM1023 identification vector. A transport
frame can arrive across any number of input blocks; `Reset` discards an incomplete
candidate. The parser keeps at most 64 receiver-frame bytes. Invalid candidates
are reported through `OnError`, and subsequent complete frames can still be read.

| Input | Accepted structure |
|-------|--------------------|
| AVR `*` or `:` | Native frame as 14 or 28 hexadecimal digits, followed by `;` |
| AVR `@` or `%` | 12 hexadecimal receiver-clock digits, then the native frame and `;` |
| AVR `<` | 12 clock digits, two signal-level digits, native frame and `;` |
| Beast type `2` | `0x1A 0x32`, six clock bytes, one signal byte, seven native bytes |
| Beast type `3` | `0x1A 0x33`, six clock bytes, one signal byte, fourteen native bytes |

Beast data bytes equal to `0x1A` are escaped by doubling them, including inside
clock and signal fields. Complete Mode A/C frames (AVR four-digit records and
Beast type `1`) and Beast type `4` receiver-status records are skipped; they are
not ADS-B messages. Other Beast extension types are not decoded. AVR line endings
and noise outside candidates are ignored. Hexadecimal input is case-insensitive.
The `%` receiver marker never bypasses host-side CRC validation.

## Message model and encoding

Every five-bit DF key is registered by default to `AdsbDfMessage`. This preserves
unrecognized payload formats as native bytes rather than claiming a semantic
decoder for every DF. `Id` is the DF as `ushort`; `DownlinkFormat` exposes it as a
byte. Factory `Remove` and `Clear` are respected, with no fallback recreation.

`RawData` is message-owned `ReadOnlyMemory<byte>`. `SetRawData` copies native bytes,
validates length and applicable parity, and refreshes `ExtendedSquitter`.
`Deserialize` instead consumes **one AVR or Beast frame**, leaving the rest of the
input span untouched. Failed validation leaves the previous message data intact.
`Tags` uses the normal `ProtocolTags` contract.

`FrameFormat`, `AvrMarker`, `ReceiverTimestamp` and `SignalLevel` describe the
receiver envelope. The clock is an unmodified 48-bit receiver value, not a UTC
instant. Signal is the raw 0–255 receiver level, not calibrated dBm.

`GetByteSize` and `Serialize` produce the selected receiver envelope. AVR output
uses uppercase hex and a trailing semicolon without CR/LF. Beast output includes
all required escape bytes. Missing clock/signal fields for a selected envelope,
out-of-range timestamps and undersized output buffers are rejected. Serialization
retains native parity bits; it never silently repairs the received downlink.

## Parity and identity

| DF | Validation and identity |
|----|-------------------------|
| 17, 18 | Direct 24-bit CRC must match; corrupt frames are rejected |
| 11 | CRC XOR parity must fit the seven-bit interrogator overlay; reported as `InterrogatorParity` |
| Other DF | Address/data parity remains `AddressParityUnverified`; a nonzero remainder is not a CRC failure by itself |

`ParitySyndrome` contains computed payload CRC XOR transmitted parity, and
`Parity` preserves the transmitted 24-bit field. No aircraft-address cache or bit
error correction is hidden in the parser. `IcaoAddress` is exposed only for DF11,
DF17 and DF18 control value 0. Relay addresses whose IMF semantics have not been decoded, anonymous/non-ICAO DF18 addresses and an
unverified parity-derived value are never presented as an ICAO identity.

## Typed ADS-B payloads

DF17 and DF18 control values 0 or 1 provide typed nested ADS-B models.
DF18 control values 2, 5 and 6 provide `AdsbExtendedSquitter` with its type code
and raw message element only: relays repurpose flag positions for IMF and other
semantics, so ordinary ADS-B flag meanings are not applied to those records.
Other DF18 control layouts remain raw. All message models
and decoded payload models implement `IVisitable` with cached typed metadata,
English descriptions, units and read-only markers.

| Type code | Model | Decoded data |
|-----------|-------|--------------|
| 1–4 | `AdsbIdentification` | Callsign and category; type code retains the category set |
| 5–8 | `AdsbSurfacePosition` | Movement, speed/track availability, speed in knots, track in degrees and raw CPR |
| 9–18 | `AdsbAirbornePosition` | Surveillance/quality bits, raw altitude, Q=1 pressure altitude in feet and raw CPR |
| 20–22 | `AdsbAirbornePosition` | GNSS height in meters and raw CPR |
| 19, subtypes 1–2 | `AdsbVelocity` | East/north velocity, ground speed, track and vertical-rate information |
| 19, subtypes 3–4 | `AdsbVelocity` | Heading, indicated/true airspeed, vertical-rate information |
| Other type codes | `AdsbExtendedSquitter` | Type code and unmodified seven-byte `MessageElement` hex |

Unavailable values use nullable numeric fields rather than zero. Surface movement
code 124 indicates a **lower bound** on speed through `GroundSpeedIsLowerBound`.
The raw version-dependent quality bit is retained; it is not automatically
converted into an ADS-B-version-specific integrity estimate.

The implementation does not reconstruct geographic coordinates from CPR pairs,
track aircraft over time, or decode Q=0 Gillham pressure altitude. For Q=0 the
original `AltitudeCode` remains available and `BarometricAltitudeFeet` is null.
Emergency, target-state and operational-status message elements are preserved raw
rather than advertised as typed decoders.

## File tools and inspector fields

The shell `ProtocolFileReader` registers ADS-B by default, so `asv-gnss file`
accepts AVR and Beast recordings. Other applications can enable the same parser
through `RegisterAdsbProtocol`; consumers pinned to an older package need an
updated library build. Visitors expose receiver metadata, DF, explicit address,
parity status, original bytes and decoded message-element fields without mutating
the received data.

## References and validation

- [Jetvision Mode-S Beast data output formats](https://wiki.jetvision.de/wiki/Mode-S_Beast:Data_Output_Formats)
- [The 1090 Megahertz Riddle: Mode S parity and identity](https://mode-s.org/1090mhz/content/mode-s/1-basics.html)
- [Aircraft identification](https://mode-s.org/1090mhz/content/ads-b/2-identification.html)
- [Airborne position](https://mode-s.org/1090mhz/content/ads-b/3-airborne-position.html)
- [Surface position](https://mode-s.org/1090mhz/content/ads-b/4-surface-position.html)
- [Airborne velocity](https://mode-s.org/1090mhz/content/ads-b/5-airborne-velocity.html)

`AdsbProtocolTest`, `AdsbMessageTest` and `AdsbPayloadTest` cover published external
vectors, independently constructed CRC/parity cases, streaming fragmentation,
escaping, recovery, transport round trips, unavailable values, typed visitors,
metadata reuse and unchanged raw storage. The shell integration has a mixed
AVR/Beast fixture in `ProtocolFileReaderTest`.
