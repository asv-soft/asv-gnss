# NMEA 0183

ASCII navigation sentences for position, time, satellite status, velocity, and error estimates.

## Connect the parser

```c#
using Asv.Gnss;
using Asv.IO;

var protocol = Protocol.Create(builder =>
{
    builder.Protocols.RegisterNmeaProtocol();
});
```

The stream parser is `NmeaMessageParser` and its message model is `NmeaMessageBase`.
See [Quick Start](quick-start.md) for router, port, and subscription setup. Registration selects decoders; the receiver must already be configured to emit the required data.

## Default message coverage

The registration method installs GBS, GGA, GLL, GSA, GST, GSV, RMC, VTG, and ZDA. Talker identifiers are represented separately by `NmeaTalkerId`; the sentence type is `NmeaMessageId`.

| Message model |
|---------------|
| `NmeaMessageGbs` |
| `NmeaMessageGga` |
| `NmeaMessageGll` |
| `NmeaMessageGsa` |
| `NmeaMessageGst` |
| `NmeaMessageGsv` |
| `NmeaMessageRmc` |
| `NmeaMessageVtg` |
| `NmeaMessageZda` |

## Usage and limitations

`NmeaProtocol` handles `$`/`!`, comma-separated fields, `*` checksum notation, and CR/LF terminators. Coordinates are converted from degrees/minutes to signed decimal degrees. Empty or invalid numeric tokens can become `double.NaN` or nullable values; check validity before using them. The standalone `NmeaMessageFactory` registers only GBS; prefer the registration method for the full default set.

## Implementation reference

Coverage above follows `Protocols/Nmea/NmeaProtocol.cs` under `src/Asv.Gnss`. Inspect concrete message classes for field names, units, and supported payload revisions.

## Specification message catalog

Sources: `Protocols/Nmea/Docs/NMEA0183.pdf`, sentence and proprietary-message lists on PDF pp. 4-24; `Protocols/Nmea/Docs/u-blox6_ReceiverDescrProtSpec_(GPS.G6-SW-10018)_Public.pdf` and `Protocols/Ubx/Docs/u-blox8-M8_ReceiverDescrProtSpec_UBX-13003221.pdf`, NMEA message definitions. The first guide is an unofficial historical summary, not a complete current NMEA standard. Vendor additions are included without treating talker IDs or manufacturer codes as sentences. Names and descriptions follow these editions, including historical terminology.

**Status:** **Model present** means a concrete payload model exists in the current checkout; it does not certify complete field, revision, encoding, or firmware coverage. **Partial** identifies a known incomplete decoder. **Not implemented** means no matching concrete model was found. **Default** reports whether the standard registration adds that model. Models outside the default set require explicit registration. See the limitations above before transmitting messages.

Page references below are **PDF page numbers (1-based)**, including covers and front matter.

| Sentence | Description | Status | C# model | Default | PDF reference |
| --- | --- | --- | --- | --- | --- |
| AAM | Waypoint Arrival Alarm | Not implemented | - | No | Guide p. 4 |
| ALM | GPS Almanac Data | Not implemented | - | No | Guide p. 4 |
| APA | Autopilot Sentence "A" | Not implemented | - | No | Guide p. 4 |
| APB | Autopilot Sentence "B" | Not implemented | - | No | Guide p. 5 |
| ASD | Autopilot System Data | Not implemented | - | No | Guide p. 5 |
| BEC | Bearing & Distance to Waypoint – Dead Reckoning | Not implemented | - | No | Guide p. 5 |
| BOD | Bearing – Waypoint to Waypoint | Not implemented | - | No | Guide p. 6 |
| BWC | Bearing and Distance to Waypoint – Latitude, N/S, Longitude, E/W, UTC, Status | Not implemented | - | No | Guide p. 6 |
| BWR | Bearing and Distance to Waypoint – Rhumb Line Latitude, N/S, Longitude, E/W, | Not implemented | - | No | Guide p. 6 |
| BWW | Bearing – Waypoint to Waypoint | Not implemented | - | No | Guide p. 7 |
| DBK | Depth Below Keel | Not implemented | - | No | Guide p. 7 |
| DBS | Depth Below Surface | Not implemented | - | No | Guide p. 7 |
| DBT | Depth Below Transducer | Not implemented | - | No | Guide p. 7 |
| DCN | Decca Position | Not implemented | - | No | Guide p. 7 |
| DPT | Heading – Deviation & Variation | Not implemented | - | No | Guide p. 8 |
| DSC | Digital Selective Calling Information | Not implemented | - | No | Guide p. 8 |
| DSE | Extended DSC | Not implemented | - | No | Guide p. 8 |
| DSI | DSC Transponder Initiate | Not implemented | - | No | Guide p. 8 |
| DSR | DSC Transponder Response | Not implemented | - | No | Guide p. 8 |
| DTM | Datum Reference | Not implemented | - | No | Guide p. 8 |
| FSI | Frequency Set Information | Not implemented | - | No | Guide p. 8 |
| GBQ | Poll a standard message (Talker ID GB) | Not implemented | - | No | M8 p. 162 |
| GBS | GPS Satellite Fault Detection | Model present | `NmeaMessageGbs` | Yes | Guide p. 8 |
| GGA | Global Positioning System Fix Data. Time, Position and fix related data | Model present | `NmeaMessageGga` | Yes | Guide p. 9 |
| GLC | Geographic Position, Loran-C | Not implemented | - | No | Guide p. 9 |
| GLL | Geographic Position – Latitude/Longitude | Model present | `NmeaMessageGll` | Yes | Guide p. 9 |
| GLQ | Poll a standard message (Talker ID GL) | Not implemented | - | No | M8 p. 166 |
| GNQ | Poll a standard message (Talker ID GN) | Not implemented | - | No | M8 p. 166 |
| GNS | GNSS fix data | Not implemented | - | No | M8 p. 167 |
| GPQ | Poll message | Not implemented | - | No | u-blox 6 p. 70 |
| GRS | GPS Range Residuals | Not implemented | - | No | Guide p. 10 |
| GSA | GPS DOP and active satellites | Model present | `NmeaMessageGsa` | Yes | Guide p. 10 |
| GST | GPS Pseudorange Noise Statistics | Model present | `NmeaMessageGst` | Yes | Guide p. 10 |
| GSV | Satellites in view | Model present | `NmeaMessageGsv` | Yes | Guide p. 10 |
| GTD | Geographic Location in Time Differences | Not implemented | - | No | Guide p. 10 |
| GXA | TRANSIT Position – Latitude/Longitude, Location and Time of TRANSIT Fix at Waypoint | Not implemented | - | No | Guide p. 10 |
| HDG | Heading – Deviation & Variation | Not implemented | - | No | Guide p. 11 |
| HDM | Heading – Magnetic | Not implemented | - | No | Guide p. 11 |
| HDT | Heading – True | Not implemented | - | No | Guide p. 11 |
| HSC | Heading Steering Command | Not implemented | - | No | Guide p. 11 |
| LCD | Loran-C Signal Data | Not implemented | - | No | Guide p. 12 |
| MSK | MSK Receiver Interface (for DGPS Beacon Receivers) | Not implemented | - | No | Guide p. 12 |
| MSS | MSK Receiver Signal Status | Not implemented | - | No | Guide p. 12 |
| MTW | Water Temperature | Not implemented | - | No | Guide p. 12 |
| MWD | Wind Direction & Speed | Not implemented | - | No | Guide p. 12 |
| MWV | Wind Speed and Angle | Not implemented | - | No | Guide p. 13 |
| OLN | Omega Lane Numbers | Not implemented | - | No | Guide p. 13 |
| OSD | Own Ship Data | Not implemented | - | No | Guide p. 13 |
| PGRMC | Sensor Configuration Information | Not implemented | - | No | Guide p. 21 |
| PGRME | Estimated Position Error | Not implemented | - | No | Guide p. 21 |
| PGRMF | Position Fix Sentence | Not implemented | - | No | Guide p. 22 |
| PGRMI | Sensor Initialisation Information | Not implemented | - | No | Guide p. 22 |
| PGRMM | Map Datum | Not implemented | - | No | Guide p. 22 |
| PGRMO | Output Sentence Enable/Disable | Not implemented | - | No | Guide p. 23 |
| PGRMT | Sensor Status Information | Not implemented | - | No | Guide p. 23 |
| PGRMV | 3D Velocity | Not implemented | - | No | Guide p. 23 |
| PGRMZ | Altitude Information | Not implemented | - | No | Guide p. 24 |
| PSLIB | Differental GPS Beacon Receiver Control | Not implemented | - | No | Guide p. 24 |
| PUBX,00 | POSITION | Not implemented | - | No | M8 p. 180 |
| PUBX,03 | SVSTATUS | Not implemented | - | No | M8 p. 182 |
| PUBX,04 | TIME | Not implemented | - | No | M8 p. 183 |
| PUBX,05 | Poll a PUBX,05 message | Not implemented | - | No | u-blox 6 p. 88 |
| PUBX,06 | Poll a PUBX,06 message | Not implemented | - | No | u-blox 6 p. 91 |
| PUBX,40 | RATE | Not implemented | - | No | M8 p. 181 |
| PUBX,41 | CONFIG | Not implemented | - | No | M8 p. 179 |
| RMA | Recommended Minimum Navigation Information | Not implemented | - | No | Guide p. 13 |
| RMB | Recommended Minimum Navigation Information | Not implemented | - | No | Guide p. 14 |
| RMC | Recommended Minimum Navigation Information | Model present | `NmeaMessageRmc` | Yes | Guide p. 14 |
| ROO | Waypoints in Active Route | Not implemented | - | No | Guide p. 13 |
| ROT | Rate Of Turn | Not implemented | - | No | Guide p. 14 |
| RPM | Revolutions | Not implemented | - | No | Guide p. 14 |
| RSA | Rudder Sensor Angle | Not implemented | - | No | Guide p. 15 |
| RSD | RADAR System Data | Not implemented | - | No | Guide p. 15 |
| RTE | Routes | Not implemented | - | No | Guide p. 15 |
| SFI | Scanning Frequency Information | Not implemented | - | No | Guide p. 15 |
| STN | Multiple Data ID | Not implemented | - | No | Guide p. 15 |
| THS | True Heading and Status | Not implemented | - | No | u-blox 6 p. 76 |
| TLL | Target Latitude and Longitude | Not implemented | - | No | Guide p. 16 |
| TRF | TRANSIT Fix Data | Not implemented | - | No | Guide p. 16 |
| TTM | Tracked Target Message | Not implemented | - | No | Guide p. 16 |
| TXT | Text Transmission | Not implemented | - | No | u-blox 6 p. 77 |
| VBW | Dual Ground/Water Speed | Not implemented | - | No | Guide p. 16 |
| VDR | Set and Drift | Not implemented | - | No | Guide p. 16 |
| VHW | Water Speed and Heading | Not implemented | - | No | Guide p. 17 |
| VLW | Distance Traveled through Water | Not implemented | - | No | Guide p. 17 |
| VPW | Speed – Measured Parallel to Wind | Not implemented | - | No | Guide p. 17 |
| VTG | Track Made Good and Ground Speed | Model present | `NmeaMessageVtg` | Yes | Guide p. 17 |
| VWR | Relative Wind Speed and Angle | Not implemented | - | No | Guide p. 18 |
| WCV | Waypoint Closure Velocity | Not implemented | - | No | Guide p. 18 |
| WDC | Distance to Waypoint – Great Circle | Not implemented | - | No | Guide p. 18 |
| WDR | Distance to Waypoint – Rhumb Line | Not implemented | - | No | Guide p. 18 |
| WNC | Distance – Waypoint to Waypoint | Not implemented | - | No | Guide p. 18 |
| WPL | Waypoint Location | Not implemented | - | No | Guide p. 18 |
| XDR | Cross Track Error – Dead Reckoning | Not implemented | - | No | Guide p. 19 |
| XTE | Cross-Track Error – Measured | Not implemented | - | No | Guide p. 19 |
| XTR | Cross Track Error – Dead Reckoning | Not implemented | - | No | Guide p. 19 |
| ZDA | Time & Date – UTC, Day, Month, Year and Local Time Zone | Model present | `NmeaMessageZda` | Yes | Guide p. 19 |
| ZDL | Time and Distance to Variable Point | Not implemented | - | No | Guide p. 19 |
| ZFO | UTC & Time from Origin Waypoint | Not implemented | - | No | Guide p. 20 |
| ZTG | UTC & Time to Destination Waypoint | Not implemented | - | No | Guide p. 20 |


[All protocols](supported-protocols.md)
