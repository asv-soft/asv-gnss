# Supported Protocols

The pages below describe the current source implementation, its default registrations, and known limitations. A registered message is not necessarily writable; encoding support is documented separately where incomplete.

## Stream protocols

| Protocol | Registration | Documentation |
|----------|--------------|---------------|
| NMEA 0183 | `RegisterNmeaProtocol` | [Coverage and usage](protocol-nmea.md) |
| RTCM v2 | `RegisterRtcmV2Protocol` | [Coverage and usage](protocol-rtcm-v2.md) |
| RTCM v3 | `RegisterRtcmV3Protocol` | [Coverage and usage](protocol-rtcm-v3.md) |
| UBX | `RegisterUbxProtocol` | [Coverage and usage](protocol-ubx.md) |
| UBX X20 HPG 2.11 | `RegisterUbxX20Protocol` | [163 typed variants and configuration keys](protocol-ubx-x20.md) |
| ASTERIX | `RegisterAsterixProtocol` | [Coverage and usage](protocol-asterix.md) |
| ADS-B / Mode S | `RegisterAdsbProtocol` | [AVR, Beast, parity and typed payloads](protocol-ads-b.md) |
| ASV | `RegisterAsvProtocol` | [Coverage and usage](protocol-asv.md) |
| GBAS | `RegisterGbasProtocol` | [Coverage and usage](protocol-gbas.md) |
| RINEX OBS/NAV/MET | `RegisterRinexProtocol` | [Versions, file API, and examples](protocol-rinex.md) |
| Septentrio Binary Format (SBF) | `RegisterSbfProtocol` | [Coverage and usage](protocol-sbf.md) |
| ComNav | `RegisterComNavProtocol` | [Coverage and usage](protocol-comnav.md) |
| Javad / GREIS | `RegisterJavadProtocol` | [303 native message models, commands, and object catalog](protocol-javad.md) |
| Raw GPS LNAV | `RegisterRawGpsProtocol` | [300-bit subframes, parity and typed fields](protocol-raw-gps.md) |
| Raw GLONASS FDMA | `RegisterRawGlonassProtocol` | [85-bit strings, Hamming checks and typed fields](protocol-raw-glonass.md) |

All registration methods are extensions on `IProtocolParserBuilder` in the `Asv.Gnss` namespace. Use them through `builder.Protocols` inside `Protocol.Create`; see [Quick Start](quick-start.md). Where a registration accepts a `configure` callback, it runs after the default messages are added and before the factory is built.

## Receiver navigation-word adapters

| Module | Entry point | Documentation |
|--------|-------------|---------------|
| Raw GPS | `GpsSubFrameFactory.Create(uint[])` | [GPS navigation subframes](protocol-raw-gps.md) |
| Raw GLONASS | `GlonassWordFactory.Create(uint[])` | [GLONASS navigation strings](protocol-raw-glonass.md) |

These compatibility entry points decode navigation words carried by receiver messages. The returned models also support the native bit-stream protocols above. Receiver uint layouts and native transmitted bits are different representations; consult each protocol page before converting between them. Raw navigation parsers require explicit registration on a suitable navigation stream and are not automatic mixed-file detectors.

## Specification coverage tables

Each protocol page includes a specification catalog with message IDs, implementation status, C# model names, default registration, and PDF references where available. The catalogs use these local specification baselines:

| Protocol | Catalog baseline |
|----------|------------------|
| [NMEA](protocol-nmea.md) | Historical sentence guide plus u-blox 6 and M8 NMEA additions. |
| [RTCM v2](protocol-rtcm-v2.md) | RTCM 10402.3 with Amendment 1. |
| [RTCM v3](protocol-rtcm-v3.md) | RTCM 10403.3 with Amendment 1; older local editions are superseded for the catalog. |
| [UBX](protocol-ubx.md) | UBX-13003221 R23 message overview. |
| [UBX X20](protocol-ubx-x20.md) | ZED-X20P X20 HPG 2.11 JSON and UBXDOC-304424225-21617 R01; protocol 50.11. |
| [SBF](protocol-sbf.md) | SBF guide v1.11.0 and AsteRx-U firmware v4.6.1 block catalogs. |
| [ComNav](protocol-comnav.md) | OEM Board Reference Manual v1.8 native logs and commands. |
| [Javad / GREIS](protocol-javad.md) | GREIS v4.2.01: 303 native IDs, including measurement families and Appendix A.4 compatibility messages. |
| [ASTERIX](protocol-asterix.md) | Implemented category list; CAT020 edition 1.9 and CAT021 editions 2.1/2.7 item lists. |
| [Raw GPS](protocol-raw-gps.md) | Legacy GPS LNAV; headers, AODO and navigation pages checked against IS-GPS-200N, preserving the original decoder contracts. |
| [Raw GLONASS](protocol-raw-glonass.md) | Legacy navigation strings from ICD edition 5.1 (2008). |
| [RINEX](protocol-rinex.md) | IGS RINEX 2.11, 3.04 and 4.02; OBS/NAV/MET coverage and implementation limits documented separately. |
| [ASV](protocol-asv.md) | Source inventory; no local protocol PDF. |
| [ADS-B](protocol-ads-b.md) | Jetvision AVR/Beast framing and published 1090MHz Riddle downlink/type-code definitions; typed and raw coverage listed separately. |

**Model present** records a concrete model in source, while **Default** separately records registration by the standard builder. Neither means that every payload revision or optional field is supported. Known incomplete decoders are marked **Partial**. Reserved identifiers and proprietary ranges retain their specification designation.

The catalogs describe the named local editions, not every future revision or every message mentioned in integration examples. Receiver integration manuals, port maps, and the NovAtel OEM7 manual are not treated as additional ComNav wire specifications. For ASTERIX categories without a local category PDF, only source coverage is reported. Check concrete payload methods before relying on encoding or a particular field.
