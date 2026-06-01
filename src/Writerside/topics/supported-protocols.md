# Supported Protocols

%product% contains protocol modules under `src/Asv.Gnss/Protocols` and parser modules under `src/Asv.Gnss/Parsers`.

## GNSS and receiver protocols

| Protocol | Notes |
|----------|-------|
| NMEA 0183 | Message parsing and serialization for common navigation sentences. |
| RTCM v2 | Legacy RTCM messages and stream parsing. |
| RTCM v3 | Modern RTCM messages, ephemeris, RTK, GLONASS bias, and MSM families. |
| UBX | u-blox binary messages and receiver microservice helpers. |
| Raw GPS | Raw GPS parser support. |
| Raw GLONASS | Raw GLONASS parser support. |
| SBF | Septentrio Binary Format parser support. |
| ComNav | ComNav ASCII and binary parser support. |
| Javad | Receiver-control parser support. |

## Aviation and surveillance protocols

| Protocol | Notes |
|----------|-------|
| ASTERIX | Category message parsing for surveillance data. |
| ADS-B | ADS-B protocol support. |
| ASV | ASV-specific protocol support. |

## NMEA sentences

The library includes support for commonly used NMEA 0183 sentences:

- `GBS`
- `GGA`
- `GLL`
- `GSA`
- `GST`
- `GSV`
- `RMC`
- `VTG`
- `ZDA`

## ASTERIX categories

The test resources and implementation cover a broad set of ASTERIX categories, including:

- `001`
- `002`
- `004`
- `010`
- `020`
- `021`
- `034`
- `048`
- `247`

Additional categories exist in the test resources and codebase. Check `src/Asv.Gnss/Protocols/Asterix` when you need exact category coverage for a release.
