# ASTERIX

Surveillance records grouped by ASTERIX category.

## Connect the parser

```csharp
using Asv.Gnss;
using Asv.IO;

var protocol = Protocol.Create(builder =>
{
    builder.Protocols.RegisterAsterixProtocol();
});
```

The stream parser is `AsterixMessageParser` and its message model is `AsterixMessage`.
See [Quick Start](quick-start.md) for router, port, and subscription setup. Registration selects decoders; the receiver must already be configured to emit the required data.

## Default message coverage

The default categories are 001, 002, 004, 010, 020, 021, 034, 048, and 247. Category IDs use `byte`; each category model contains its supported data-item implementations.

| Message model |
|---------------|
| `AsterixMessageI002` |
| `AsterixMessageI004` |
| `AsterixMessageI034` |
| `AsterixMessageI001` |
| `AsterixMessageI010` |
| `AsterixMessageI020` |
| `AsterixMessageI021` |
| `AsterixMessageI048` |
| `AsterixMessageI247` |

## Usage and limitations

Category registration is not a claim of complete coverage of every edition, UAP, or optional data item. Match the sender's category edition and field layout against the corresponding `Messages/Ixxx` source directory. Test recordings and excluded raw-message files do not add runtime category support.

## Implementation reference

Coverage above follows `Protocols/Asterix/AsterixProtocol.cs` under `src/Asv.Gnss`. Inspect concrete message classes for field names, units, and supported payload revisions.

## Specification message catalog

The message-level registration unit is an ASTERIX category. The following table lists all categories registered in `AsterixProtocol`; the local PDF set provides category specifications only for CAT020 and CAT021. No complete catalog of all EUROCONTROL categories is available in this checkout.

**Status:** **Model present** means a concrete payload model exists in the current checkout; it does not certify complete field, revision, encoding, or firmware coverage. **Partial** identifies a known incomplete decoder. **Not implemented** means no matching concrete model was found. **Default** reports whether the standard registration adds that model. Models outside the default set require explicit registration. See the limitations above before transmitting messages.

Page references below are **PDF page numbers (1-based)**, including covers and front matter.

| Category | C# model | Status | Default | Local PDF coverage |
| --- | --- | --- | --- | --- |
| 001 | `AsterixMessageI001` | Model present | Yes | No category specification PDF in this checkout |
| 002 | `AsterixMessageI002` | Model present | Yes | No category specification PDF in this checkout |
| 004 | `AsterixMessageI004` | Model present | Yes | No category specification PDF in this checkout |
| 010 | `AsterixMessageI010` | Model present | Yes | No category specification PDF in this checkout |
| 020 | `AsterixMessageI020` | Model present | Yes | CAT020 ed. 1.9 |
| 021 | `AsterixMessageI021` | Model present | Yes | CAT021 ed. 2.1 and 2.7 |
| 034 | `AsterixMessageI034` | Model present | Yes | No category specification PDF in this checkout |
| 048 | `AsterixMessageI048` | Model present | Yes | No category specification PDF in this checkout |
| 247 | `AsterixMessageI247` | Model present | Yes | No category specification PDF in this checkout |

### CAT020 and CAT021 data-item catalogs

The tables below list every standard data item defined by the local category PDFs. They do not expand bit fields inside those items. A matching item number means a model exists, not that all revisions of that item are compatible. In particular, `AsterixRecordI021` declares the edition 2.1 mapping; CAT021 edition 2.7 support must not be inferred from a shared item identifier. C# FRN values are reported exactly as implemented, independently of the printed PDF UAP.

### CAT020

Sources: `Protocols/Asterix/Messages/I020/Docs/cat020-asterix-mlt-messages-part-14.pdf` (edition 1.9).

| Item | Name | Status | C# model | C# FRN | Record dispatch | PDF reference |
| --- | --- | --- | --- | --- | --- | --- |
| I020/010 | Data Source Identifier | Model present | `AsterixFieldI020Frn001Type010` | 1 | Yes | ed. 1.9 p. 20 |
| I020/020 | Target Report Descriptor | Model present | `AsterixFieldI020Frn002Type020` | 2 | Yes | ed. 1.9 p. 21 |
| I020/030 | Warning/Error Conditions | Model present | `AsterixFieldI020Frn024Type030` | 24 | Yes | ed. 1.9 p. 23 |
| I020/041 | Position in WGS-84 Coordinates | Model present | `AsterixFieldI020Frn004Type041` | 4 | Yes | ed. 1.9 p. 24 |
| I020/042 | Position in Cartesian Coordinates | Model present | `AsterixFieldI020Frn005Type042` | 5 | Yes | ed. 1.9 p. 25 |
| I020/050 | Mode-2 Code in Octal Representation | Model present | `AsterixFieldI020Frn026Type050` | 26 | Yes | ed. 1.9 p. 26 |
| I020/055 | Mode-1 Code in Octal Representation | Model present | `AsterixFieldI020Frn025Type055` | 25 | Yes | ed. 1.9 p. 27 |
| I020/070 | Mode-3/A Code in Octal Representation | Model present | `AsterixFieldI020Frn008Type070` | 8 | Yes | ed. 1.9 p. 28 |
| I020/090 | Flight Level in Binary Representation | Model present | `AsterixFieldI020Frn010Type090` | 10 | Yes | ed. 1.9 p. 29 |
| I020/100 | Mode-C Code | Model present | `AsterixFieldI020Frn011Type100` | 11 | Yes | ed. 1.9 p. 30 |
| I020/105 | Geometric Height (WGS-84) | Model present | `AsterixFieldI020Frn015Type105` | 15 | Yes | ed. 1.9 p. 31 |
| I020/110 | Measured Height (Local Cartesian Coordinates) | Model present | `AsterixFieldI020Frn014Type110` | 14 | Yes | ed. 1.9 p. 31 |
| I020/140 | Time of Day | Model present | `AsterixFieldI020Frn003Type140` | 3 | Yes | ed. 1.9 p. 32 |
| I020/161 | Track Number | Model present | `AsterixFieldI020Frn006Type161` | 6 | Yes | ed. 1.9 p. 32 |
| I020/170 | Track Status | Model present | `AsterixFieldI020Frn007Type170` | 7 | Yes | ed. 1.9 p. 33 |
| I020/202 | Calculated Track Velocity in Cartesian Coordinates | Model present | `AsterixFieldI020Frn009Type202` | 9 | Yes | ed. 1.9 p. 34 |
| I020/210 | Calculated Acceleration | Model present | `AsterixFieldI020Frn016Type210` | 16 | Yes | ed. 1.9 p. 35 |
| I020/220 | Target Address | Model present | `AsterixFieldI020Frn012Type220` | 12 | Yes | ed. 1.9 p. 35 |
| I020/230 | Communications/ACAS Capability and Flight Status | Model present | `AsterixFieldI020Frn022Type230` | 22 | Yes | ed. 1.9 p. 36 |
| I020/245 | Target Identification | Model present | `AsterixFieldI020Frn013Type245` | 13 | Yes | ed. 1.9 p. 38 |
| I020/250 | Mode S MB Data | Model present | `AsterixFieldI020Frn021Type250` | 21 | Yes | ed. 1.9 p. 39 |
| I020/260 | ACAS Resolution Advisory Report | Model present | `AsterixFieldI020Frn023Type260` | 23 | Yes | ed. 1.9 p. 41 |
| I020/300 | Vehicle Fleet Identification | Model present | `AsterixFieldI020Frn017Type300` | 17 | Yes | ed. 1.9 p. 42 |
| I020/310 | Pre-programmed Message | Model present | `AsterixFieldI020Frn018Type310` | 18 | Yes | ed. 1.9 p. 43 |
| I020/400 | Contributing Devices | Model present | `AsterixFieldI020Frn020Type400` | 20 | Yes | ed. 1.9 p. 44 |
| I020/500 | Position Accuracy | Model present | `AsterixFieldI020Frn019Type500` | 19 | Yes | ed. 1.9 p. 46 |
| RE | Reserved expansion | Container model | `AsterixFieldI020Frn027TypeRe` | 27 | Yes | UAP / separate appendix |
| SP | Special purpose | Not implemented | - | - | No | UAP / separate appendix |

### CAT021

Sources: `Protocols/Asterix/Messages/I021/Docs/part12-cat021-asterix-ads-b-messages-ed2.1.pdf` (edition 2.1), `Protocols/Asterix/Messages/I021/Docs/asterix-adsbtr-cat021-part12-v2-7.pdf` (edition 2.7).

| Item | Name | Status | C# model | C# FRN | Record dispatch | PDF reference |
| --- | --- | --- | --- | --- | --- | --- |
| I021/008 | Aircraft Operational Status | Model present | `AsterixFieldI021Frn041Type008` | 41 | Yes | ed. 2.1 p. 19; ed. 2.7 p. 20 |
| I021/010 | Data Source Identification | Model present | `AsterixFieldI021Frn001Type010` | 1 | Yes | ed. 2.1 p. 20; ed. 2.7 p. 21 |
| I021/015 | Service Identification | Model present | `AsterixFieldI021Frn004Type015` | 4 | Yes | ed. 2.1 p. 20; ed. 2.7 p. 21 |
| I021/016 | Service Management | Model present | `AsterixFieldI021Frn039Type016` | 39 | Yes | ed. 2.1 p. 21; ed. 2.7 p. 22 |
| I021/020 | Emitter Category | Model present | `AsterixFieldI021Frn034Type020` | 34 | Yes | ed. 2.1 p. 22; ed. 2.7 p. 23 |
| I021/040 | Target Report Descriptor | Model present | `AsterixFieldI021Frn002Type040` | 2 | Yes | ed. 2.1 p. 23; ed. 2.7 p. 24 |
| I021/070 | Mode 3/A Code in Octal Representation | Model present | `AsterixFieldI021Frn021Type070` | 21 | Yes | ed. 2.1 p. 26; ed. 2.7 p. 31 |
| I021/071 | Time of Applicability for Position | Model present | `AsterixFieldI021Frn005Type071` | 5 | Yes | ed. 2.1 p. 26; ed. 2.7 p. 32 |
| I021/072 | Time of Applicability for Velocity | Model present | `AsterixFieldI021Frn009Type072` | 9 | Yes | ed. 2.1 p. 27; ed. 2.7 p. 33 |
| I021/073 | Time of Message Reception for Position | Model present | `AsterixFieldI021Frn013Type073` | 13 | Yes | ed. 2.1 p. 28; ed. 2.7 p. 34 |
| I021/074 | Time of Message Reception of Position–High Precision19 | Model present | `AsterixFieldI021Frn014Type074` | 14 | Yes | ed. 2.1 p. 8; ed. 2.1 p. 29; ed. 2.7 p. 35 |
| I021/075 | Time of Message Reception for Velocity | Model present | `AsterixFieldI021Frn015Type075` | 15 | Yes | ed. 2.1 p. 30; ed. 2.7 p. 36 |
| I021/076 | Time of Message Reception of Velocity–High Precision 21 | Model present | `AsterixFieldI021Frn017Type076` | 17 | Yes | ed. 2.1 p. 8; ed. 2.1 p. 31; ed. 2.7 p. 37 |
| I021/077 | Time of ASTERIX Report Transmission | Model present | `AsterixFieldI021Frn031Type077` | 31 | Yes | ed. 2.1 p. 32; ed. 2.7 p. 38 |
| I021/080 | Target Address | Model present | `AsterixFieldI021Frn012Type080` | 12 | Yes | ed. 2.1 p. 33; ed. 2.7 p. 39 |
| I021/090 | Quality Indicators | Model present | `AsterixFieldI021Frn019Type090` | 19 | Yes | ed. 2.1 p. 34; ed. 2.7 p. 40 |
| I021/110 | Trajectory Intent | Model present | `AsterixFieldI021Frn038Type110` | 38 | Yes | ed. 2.1 p. 37; ed. 2.7 p. 51 |
| I021/130 | Position in WGS-84 Co-ordinates | Model present | `AsterixFieldI021Frn006Type130` | 6 | Yes | ed. 2.1 p. 41; ed. 2.7 p. 56 |
| I021/131 | High-Resolution Position in WGS-84 Co-ordinates | Model present | `AsterixFieldI021Frn007Type131` | 7 | Yes | ed. 2.1 p. 42; ed. 2.7 p. 57 |
| I021/132 | Message Amplitude | Model present | `AsterixFieldI021Frn043Type132` | 43 | Yes | ed. 2.1 p. 43; ed. 2.7 p. 58 |
| I021/140 | Geometric Height | Model present | `AsterixFieldI021Frn018Type140` | 18 | Yes | ed. 2.1 p. 43; ed. 2.7 p. 59 |
| I021/145 | Flight Level | Model present | `AsterixFieldI021Frn023Type145` | 23 | Yes | ed. 2.1 p. 44; ed. 2.7 p. 60 |
| I021/146 | Selected Altitude | Model present | `AsterixFieldI021Frn036Type146` | 36 | Yes | ed. 2.1 p. 45; ed. 2.7 p. 61 |
| I021/148 | Final State Selected Altitude | Model present | `AsterixFieldI021Frn037Type148` | 37 | Yes | ed. 2.1 p. 46; ed. 2.7 p. 62 |
| I021/150 | Air Speed | Model present | `AsterixFieldI021Frn010Type150` | 10 | Yes | ed. 2.1 p. 47; ed. 2.7 p. 63 |
| I021/151 | True Airspeed | Model present | `AsterixFieldI021Frn011Type151` | 11 | Yes | ed. 2.1 p. 47; ed. 2.7 p. 64 |
| I021/152 | Magnetic Heading | Model present | `AsterixFieldI021Frn025Type152` | 25 | Yes | ed. 2.1 p. 48; ed. 2.7 p. 64 |
| I021/155 | Barometric Vertical Rate | Model present | `AsterixFieldI021Frn027Type155` | 27 | Yes | ed. 2.1 p. 48; ed. 2.7 p. 65 |
| I021/157 | Geometric Vertical Rate | Model present | `AsterixFieldI021Frn028Type157` | 28 | Yes | ed. 2.1 p. 49; ed. 2.7 p. 66 |
| I021/160 | Airborne Ground Vector | Model present | `AsterixFieldI021Frn029Type160` | 29 | Yes | ed. 2.1 p. 50; ed. 2.7 p. 67 |
| I021/161 | Track Number | Model present | `AsterixFieldI021Frn003Type161` | 3 | Yes | ed. 2.1 p. 51; ed. 2.7 p. 68 |
| I021/165 | Track Angle Rate | Model present | `AsterixFieldI021Frn030Type165` | 30 | Yes | ed. 2.1 p. 51; ed. 2.7 p. 68 |
| I021/170 | Target Identification | Model present | `AsterixFieldI021Frn033Type170` | 33 | Yes | ed. 2.1 p. 52; ed. 2.7 p. 69 |
| I021/200 | Target Status | Model present | `AsterixFieldI021Frn026Type200` | 26 | Yes | ed. 2.1 p. 53; ed. 2.7 p. 70 |
| I021/210 | MOPS Version | Model present | `AsterixFieldI021Frn020Type210` | 20 | Yes | ed. 2.1 p. 54; ed. 2.7 p. 72 |
| I021/220 | Met Information | Model present | `AsterixFieldI021Frn035Type220` | 35 | Yes | ed. 2.1 p. 55; ed. 2.7 p. 74 |
| I021/230 | Roll Angle | Model present | `AsterixFieldI021Frn022Type230` | 22 | Yes | ed. 2.1 p. 57; ed. 2.7 p. 76 |
| I021/250 | Mode S MB Data | Model present | `AsterixFieldI021Frn044Type250` | 44 | Yes | ed. 2.1 p. 58; ed. 2.7 p. 77 |
| I021/260 | ACAS Resolution Advisory Report | Model present | `AsterixFieldI021Frn045Type260` | 45 | Yes | ed. 2.1 p. 59; ed. 2.7 p. 79 |
| I021/271 | Surface Capabilities and Characteristics | Model present | `AsterixFieldI021Frn042Type271` | 42 | Yes | ed. 2.1 p. 60; ed. 2.7 p. 81 |
| I021/295 | Data Ages | Model present | `AsterixFieldI021Frn047Type295` | 47 | Yes | ed. 2.1 p. 62; ed. 2.7 p. 83 |
| I021/400 | Receiver ID | Model present | `AsterixFieldI021Frn046Type400` | 46 | Yes | ed. 2.1 p. 72; ed. 2.7 p. 93 |
| RE | Reserved expansion | Container model | `AsterixFieldI021Frn054TypeRe` | 54 | Yes | UAP / separate appendix |
| SP | Special purpose | Container model | `AsterixFieldI021Frn055TypeSp` | 55 | Yes | UAP / separate appendix |

The local reserved-expansion appendices (`appendixcat020pt14aed12.pdf` and `appendixcat021pt12aed11.pdf`) describe subfields rather than additional message categories. A container model in the table is not a claim that those appendices are fully decoded. The ASTERIX Part 1 PDF defines common framing, not another category catalog.


[All protocols](supported-protocols.md)
