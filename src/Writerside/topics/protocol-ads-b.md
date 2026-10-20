# ADS-B status

`AdsbProtocol.RegisterAdsbProtocol` and `AdsbParser` exist, but they are scaffolding. `AdsbParser.Push` and `Reset` throw `NotImplementedException`; `AdsbDfMessage` serialization, deserialization, sizing, and tag access are also unimplemented. No default messages are registered.

Do not register this parser in a working stream pipeline. For ADS-B reports transported in ASTERIX CAT021, use [ASTERIX](protocol-asterix.md); this is a separate transport and decoder.

See `src/Asv.Gnss/Protocols/ADS-B`.

## Specification message catalog

No native Mode S / ADS-B message-catalog PDF is present under `Protocols/ADS-B`. The local CAT021 documents describe ASTERIX reports and are covered on the [ASTERIX page](protocol-asterix.md). They do not define the input framing of `AdsbParser`. Consequently a complete native ADS-B downlink-format/type-code catalog cannot be established from this PDF set.

| Module | Status | C# type | Default payloads |
| --- | --- | --- | --- |
| Native ADS-B parser | Not implemented (Push/Reset throw) | `AdsbParser` | None |
| Native ADS-B payload | Not implemented | `AdsbDfMessage` | None |
| ASTERIX CAT021 reports | Separate implemented transport | `AsterixMessageI021` | Registered by RegisterAsterixProtocol |


[All protocols](supported-protocols.md)
