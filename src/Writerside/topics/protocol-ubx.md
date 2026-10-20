# UBX

u-blox binary receiver configuration, acknowledgements, monitoring, navigation results, and raw navigation words.

## Connect the parser

```csharp
using Asv.Gnss;
using Asv.IO;

var protocol = Protocol.Create(builder =>
{
    builder.Protocols.RegisterUbxProtocol();
});
```

The stream parser is `UbxMessageParser` and its message model is `UbxMessageBase`.
See [Quick Start](quick-start.md) for router, port, and subscription setup. Registration selects decoders; the receiver must already be configured to emit the required data.

## Default message coverage

The default set includes ACK, CFG, INF, MON, NAV, and RXM messages. UBX uses sync bytes `B5 62`; message identity combines class and message ID into a `ushort`.

| Message model |
|---------------|
| `UbxAckAck` |
| `UbxAckNak` |
| `UbxCfgAnt` |
| `UbxCfgCfg` |
| `UbxCfgMsg` |
| `UbxCfgNav5` |
| `UbxCfgPrt` |
| `UbxCfgRate` |
| `UbxCfgRst` |
| `UbxCfgTMode3` |
| `UbxInfDebug` |
| `UbxInfError` |
| `UbxInfNotice` |
| `UbxInfTest` |
| `UbxMonHw` |
| `UbxMonMsgpp` |
| `UbxMonRxbuf` |
| `UbxMonTxbuf` |
| `UbxMonVer` |
| `UbxNavDop` |
| `UbxNavPvt` |
| `UbxNavSat` |
| `UbxNavSbas` |
| `UbxNavStatus` |
| `UbxNavSvin` |
| `UbxRxmSfrbx` |

## Usage and limitations

Receiver firmware determines which commands and payload versions are accepted. Registration does not configure receiver output. Use the receiver microservice for request/response workflows and acknowledgements. The standalone `UbxMessageFactory` is empty; use `RegisterUbxProtocol`. Existing payload models implement `IVisitable`; see [Quick Start](quick-start.md) for field traversal and its editing constraints.

## Implementation reference

Coverage above follows `Protocols/Ubx/UbxProtocol.cs` under `src/Asv.Gnss`. Inspect concrete message classes for field names, units, and supported payload revisions.

## Specification message catalog

Source: `Protocols/Ubx/Docs/u-blox8-M8_ReceiverDescrProtSpec_UBX-13003221.pdf`, UBX-13003221 R23, section 32.7 (PDF pp. 190-195). Each mnemonic is listed once; poll/output variants and payload versions share a row. MGA subtype mnemonics remain separate even when class/ID is shared.

**Status:** **Model present** means a concrete payload model exists in the current checkout; it does not certify complete field, revision, encoding, or firmware coverage. **Partial** identifies a known incomplete decoder. **Not implemented** means no matching concrete model was found. **Default** reports whether the standard registration adds that model. Models outside the default set require explicit registration. See the limitations above before transmitting messages.

Page references below are **PDF page numbers (1-based)**, including covers and front matter.

| Class / ID | Message | Status | C# model | Default | PDF pages |
| --- | --- | --- | --- | --- | --- |
| 0x05 / 0x01 | ACK-ACK | Model present | `UbxAckAck` | Yes | 196 |
| 0x05 / 0x00 | ACK-NAK | Model present | `UbxAckNak` | Yes | 196 |
| 0x0B / 0x30 | AID-ALM | Not implemented | - | No | 197, 198 |
| 0x0B / 0x33 | AID-AOP | Not implemented | - | No | 199, 200 |
| 0x0B / 0x31 | AID-EPH | Not implemented | - | No | 201, 202 |
| 0x0B / 0x02 | AID-HUI | Not implemented | - | No | 203 |
| 0x0B / 0x01 | AID-INI | Not implemented | - | No | 205 |
| 0x06 / 0x13 | CFG-ANT | Model present | `UbxCfgAnt` | Yes | 208 |
| 0x06 / 0x93 | CFG-BATCH | Not implemented | - | No | 209 |
| 0x06 / 0x09 | CFG-CFG | Model present | `UbxCfgCfg` | Yes | 210 |
| 0x06 / 0x06 | CFG-DAT | Not implemented | - | No | 212, 213 |
| 0x06 / 0x70 | CFG-DGNSS | Not implemented | - | No | 214 |
| 0x06 / 0x61 | CFG-DOSC | Not implemented | - | No | 214 |
| 0x06 / 0x56 | CFG-ESFALG | Not implemented | - | No | 216 |
| 0x06 / 0x4C | CFG-ESFA | Not implemented | - | No | 217 |
| 0x06 / 0x4D | CFG-ESFG | Not implemented | - | No | 218 |
| 0x06 / 0x82 | CFG-ESFWT | Not implemented | - | No | 218 |
| 0x06 / 0x60 | CFG-ESRC | Not implemented | - | No | 221 |
| 0x06 / 0x69 | CFG-GEOFENCE | Not implemented | - | No | 223 |
| 0x06 / 0x3E | CFG-GNSS | Not implemented | - | No | 224 |
| 0x06 / 0x5C | CFG-HNR | Not implemented | - | No | 227 |
| 0x06 / 0x02 | CFG-INF | Not implemented | - | No | 227, 228 |
| 0x06 / 0x39 | CFG-ITFM | Not implemented | - | No | 229 |
| 0x06 / 0x47 | CFG-LOGFILTER | Not implemented | - | No | 230 |
| 0x06 / 0x01 | CFG-MSG | Model present | `UbxCfgMsg` | Yes | 232, 233 |
| 0x06 / 0x24 | CFG-NAV5 | Model present | `UbxCfgNav5` | Yes | 233 |
| 0x06 / 0x23 | CFG-NAVX5 | Not implemented | - | No | 236, 238, 241 |
| 0x06 / 0x17 | CFG-NMEA | Not implemented | - | No | 243, 245, 248 |
| 0x06 / 0x1E | CFG-ODO | Not implemented | - | No | 251 |
| 0x06 / 0x3B | CFG-PM2 | Not implemented | - | No | 252, 254, 256 |
| 0x06 / 0x86 | CFG-PMS | Not implemented | - | No | 259 |
| 0x06 / 0x00 | CFG-PRT | Model present | `UbxCfgPrt` | Yes | 260, 263, 265, 268 |
| 0x06 / 0x57 | CFG-PWR | Not implemented | - | No | 270 |
| 0x06 / 0x08 | CFG-RATE | Model present | `UbxCfgRate` | Yes | 271 |
| 0x06 / 0x34 | CFG-RINV | Not implemented | - | No | 272 |
| 0x06 / 0x04 | CFG-RST | Model present | `UbxCfgRst` | Yes | 273 |
| 0x06 / 0x11 | CFG-RXM | Not implemented | - | No | 275 |
| 0x06 / 0x16 | CFG-SBAS | Not implemented | - | No | 276 |
| 0x06 / 0x88 | CFG-SENIF | Not implemented | - | No | 278 |
| 0x06 / 0x8D | CFG-SLAS | Not implemented | - | No | 279 |
| 0x06 / 0x62 | CFG-SMGR | Not implemented | - | No | 280 |
| 0x06 / 0x64 | CFG-SPT | Not implemented | - | No | 283 |
| 0x06 / 0x3D | CFG-TMODE2 | Not implemented | - | No | 283 |
| 0x06 / 0x71 | CFG-TMODE3 | Model present | `UbxCfgTMode3` | Yes | 285 |
| 0x06 / 0x31 | CFG-TP5 | Not implemented | - | No | 287, 288, 289 |
| 0x06 / 0x53 | CFG-TXSLOT | Not implemented | - | No | 291 |
| 0x06 / 0x1B | CFG-USB | Not implemented | - | No | 292 |
| 0x10 / 0x14 | ESF-ALG | Not implemented | - | No | 294 |
| 0x10 / 0x15 | ESF-INS | Not implemented | - | No | 295 |
| 0x10 / 0x02 | ESF-MEAS | Not implemented | - | No | 297 |
| 0x10 / 0x03 | ESF-RAW | Not implemented | - | No | 298 |
| 0x10 / 0x10 | ESF-STATUS | Not implemented | - | No | 299 |
| 0x28 / 0x01 | HNR-ATT | Not implemented | - | No | 302 |
| 0x28 / 0x02 | HNR-INS | Not implemented | - | No | 303 |
| 0x28 / 0x00 | HNR-PVT | Not implemented | - | No | 304 |
| 0x04 / 0x04 | INF-DEBUG | Model present | `UbxInfDebug` | Yes | 307 |
| 0x04 / 0x00 | INF-ERROR | Model present | `UbxInfError` | Yes | 307 |
| 0x04 / 0x02 | INF-NOTICE | Model present | `UbxInfNotice` | Yes | 308 |
| 0x04 / 0x03 | INF-TEST | Model present | `UbxInfTest` | Yes | 308 |
| 0x04 / 0x01 | INF-WARNING | Not implemented | - | No | 309 |
| 0x21 / 0x11 | LOG-BATCH | Not implemented | - | No | 310 |
| 0x21 / 0x07 | LOG-CREATE | Not implemented | - | No | 313 |
| 0x21 / 0x03 | LOG-ERASE | Not implemented | - | No | 314 |
| 0x21 / 0x0E | LOG-FINDTIME | Not implemented | - | No | 314, 315 |
| 0x21 / 0x08 | LOG-INFO | Not implemented | - | No | 316 |
| 0x21 / 0x10 | LOG-RETRIEVEBATCH | Not implemented | - | No | 318 |
| 0x21 / 0x0F | LOG-RETRIEVEPOSEXTRA | Not implemented | - | No | 319 |
| 0x21 / 0x0B | LOG-RETRIEVEPOS | Not implemented | - | No | 319 |
| 0x21 / 0x0D | LOG-RETRIEVESTRING | Not implemented | - | No | 320 |
| 0x21 / 0x09 | LOG-RETRIEVE | Not implemented | - | No | 321 |
| 0x21 / 0x04 | LOG-STRING | Not implemented | - | No | 322 |
| 0x13 / 0x60 | MGA-ACK-DATA0 | Not implemented | - | No | 323 |
| 0x13 / 0x20 | MGA-ANO | Not implemented | - | No | 324 |
| 0x13 / 0x03 | MGA-BDS-EPH | Not implemented | - | No | 325 |
| 0x13 / 0x03 | MGA-BDS-ALM | Not implemented | - | No | 326 |
| 0x13 / 0x03 | MGA-BDS-HEALTH | Not implemented | - | No | 327 |
| 0x13 / 0x03 | MGA-BDS-UTC | Not implemented | - | No | 328 |
| 0x13 / 0x03 | MGA-BDS-IONO | Not implemented | - | No | 328 |
| 0x13 / 0x80 | MGA-DBD | Not implemented | - | No | 329 |
| 0x13 / 0x21 | MGA-FLASH-DATA | Not implemented | - | No | 330 |
| 0x13 / 0x21 | MGA-FLASH-STOP | Not implemented | - | No | 331 |
| 0x13 / 0x21 | MGA-FLASH-ACK | Not implemented | - | No | 331 |
| 0x13 / 0x02 | MGA-GAL-EPH | Not implemented | - | No | 332 |
| 0x13 / 0x02 | MGA-GAL-ALM | Not implemented | - | No | 334 |
| 0x13 / 0x02 | MGA-GAL-TIMEOFFSET | Not implemented | - | No | 335 |
| 0x13 / 0x02 | MGA-GAL-UTC | Not implemented | - | No | 335 |
| 0x13 / 0x06 | MGA-GLO-EPH | Not implemented | - | No | 336 |
| 0x13 / 0x06 | MGA-GLO-ALM | Not implemented | - | No | 337 |
| 0x13 / 0x06 | MGA-GLO-TIMEOFFSET | Not implemented | - | No | 338 |
| 0x13 / 0x00 | MGA-GPS-EPH | Not implemented | - | No | 339 |
| 0x13 / 0x00 | MGA-GPS-ALM | Not implemented | - | No | 341 |
| 0x13 / 0x00 | MGA-GPS-HEALTH | Not implemented | - | No | 342 |
| 0x13 / 0x00 | MGA-GPS-UTC | Not implemented | - | No | 342 |
| 0x13 / 0x00 | MGA-GPS-IONO | Not implemented | - | No | 343 |
| 0x13 / 0x40 | MGA-INI-POS_XYZ | Not implemented | - | No | 344 |
| 0x13 / 0x40 | MGA-INI-POS_LLH | Not implemented | - | No | 345 |
| 0x13 / 0x40 | MGA-INI-TIME_UTC | Not implemented | - | No | 345 |
| 0x13 / 0x40 | MGA-INI-TIME_GNSS | Not implemented | - | No | 347 |
| 0x13 / 0x40 | MGA-INI-CLKD | Not implemented | - | No | 348 |
| 0x13 / 0x40 | MGA-INI-FREQ | Not implemented | - | No | 349 |
| 0x13 / 0x40 | MGA-INI-EOP | Not implemented | - | No | 350 |
| 0x13 / 0x05 | MGA-QZSS-EPH | Not implemented | - | No | 350 |
| 0x13 / 0x05 | MGA-QZSS-ALM | Not implemented | - | No | 352 |
| 0x13 / 0x05 | MGA-QZSS-HEALTH | Not implemented | - | No | 353 |
| 0x0A / 0x32 | MON-BATCH | Not implemented | - | No | 354 |
| 0x0A / 0x28 | MON-GNSS | Not implemented | - | No | 355 |
| 0x0A / 0x0B | MON-HW2 | Not implemented | - | No | 357 |
| 0x0A / 0x09 | MON-HW | Model present | `UbxMonHw` | Yes | 358 |
| 0x0A / 0x02 | MON-IO | Not implemented | - | No | 359 |
| 0x0A / 0x06 | MON-MSGPP | Model present | `UbxMonMsgpp` | Yes | 360 |
| 0x0A / 0x27 | MON-PATCH | Not implemented | - | No | 360, 361 |
| 0x0A / 0x07 | MON-RXBUF | Model present | `UbxMonRxbuf` | Yes | 362 |
| 0x0A / 0x21 | MON-RXR | Not implemented | - | No | 362 |
| 0x0A / 0x2E | MON-SMGR | Not implemented | - | No | 363 |
| 0x0A / 0x2F | MON-SPT | Not implemented | - | No | 366 |
| 0x0A / 0x08 | MON-TXBUF | Model present | `UbxMonTxbuf` | Yes | 370 |
| 0x0A / 0x04 | MON-VER | Model present | `UbxMonVer` | Yes | 371 |
| 0x01 / 0x60 | NAV-AOPSTATUS | Not implemented | - | No | 373 |
| 0x01 / 0x05 | NAV-ATT | Not implemented | - | No | 374 |
| 0x01 / 0x22 | NAV-CLOCK | Not implemented | - | No | 375 |
| 0x01 / 0x36 | NAV-COV | Not implemented | - | No | 375 |
| 0x01 / 0x31 | NAV-DGPS | Not implemented | - | No | 376 |
| 0x01 / 0x04 | NAV-DOP | Model present | `UbxNavDop` | Yes | 377 |
| 0x01 / 0x3D | NAV-EELL | Not implemented | - | No | 378 |
| 0x01 / 0x61 | NAV-EOE | Not implemented | - | No | 379 |
| 0x01 / 0x39 | NAV-GEOFENCE | Not implemented | - | No | 379 |
| 0x01 / 0x13 | NAV-HPPOSECEF | Not implemented | - | No | 380 |
| 0x01 / 0x14 | NAV-HPPOSLLH | Not implemented | - | No | 381 |
| 0x01 / 0x28 | NAV-NMI | Not implemented | - | No | 383 |
| 0x01 / 0x09 | NAV-ODO | Not implemented | - | No | 386 |
| 0x01 / 0x34 | NAV-ORB | Not implemented | - | No | 387 |
| 0x01 / 0x01 | NAV-POSECEF | Not implemented | - | No | 390 |
| 0x01 / 0x02 | NAV-POSLLH | Not implemented | - | No | 390 |
| 0x01 / 0x07 | NAV-PVT | Model present | `UbxNavPvt` | Yes | 391 |
| 0x01 / 0x3C | NAV-RELPOSNED | Not implemented | - | No | 394 |
| 0x01 / 0x10 | NAV-RESETODO | Not implemented | - | No | 396 |
| 0x01 / 0x35 | NAV-SAT | Model present | `UbxNavSat` | Yes | 396 |
| 0x01 / 0x32 | NAV-SBAS | Model present | `UbxNavSbas` | Yes | 398 |
| 0x01 / 0x42 | NAV-SLAS | Not implemented | - | No | 400 |
| 0x01 / 0x06 | NAV-SOL | Not implemented | - | No | 402 |
| 0x01 / 0x03 | NAV-STATUS | Model present | `UbxNavStatus` | Yes | 403 |
| 0x01 / 0x30 | NAV-SVINFO | Not implemented | - | No | 405 |
| 0x01 / 0x3B | NAV-SVIN | Model present | `UbxNavSvin` | Yes | 407 |
| 0x01 / 0x24 | NAV-TIMEBDS | Not implemented | - | No | 409 |
| 0x01 / 0x25 | NAV-TIMEGAL | Not implemented | - | No | 410 |
| 0x01 / 0x23 | NAV-TIMEGLO | Not implemented | - | No | 411 |
| 0x01 / 0x20 | NAV-TIMEGPS | Not implemented | - | No | 412 |
| 0x01 / 0x26 | NAV-TIMELS | Not implemented | - | No | 413 |
| 0x01 / 0x21 | NAV-TIMEUTC | Not implemented | - | No | 415 |
| 0x01 / 0x11 | NAV-VELECEF | Not implemented | - | No | 416 |
| 0x01 / 0x12 | NAV-VELNED | Not implemented | - | No | 417 |
| 0x02 / 0x61 | RXM-IMES | Not implemented | - | No | 418 |
| 0x02 / 0x14 | RXM-MEASX | Not implemented | - | No | 421 |
| 0x02 / 0x41 | RXM-PMREQ | Not implemented | - | No | 422, 423 |
| 0x02 / 0x15 | RXM-RAWX | Not implemented | - | No | 424, 428 |
| 0x02 / 0x59 | RXM-RLM | Not implemented | - | No | 431, 432 |
| 0x02 / 0x32 | RXM-RTCM | Not implemented | - | No | 433 |
| 0x02 / 0x13 | RXM-SFRBX | Model present | `UbxRxmSfrbx` | Yes | 434, 435 |
| 0x02 / 0x20 | RXM-SVSI | Not implemented | - | No | 436 |
| 0x27 / 0x03 | SEC-UNIQID | Not implemented | - | No | 438 |
| 0x0D / 0x11 | TIM-DOSC | Not implemented | - | No | 439 |
| 0x0D / 0x16 | TIM-FCHG | Not implemented | - | No | 439 |
| 0x0D / 0x17 | TIM-HOC | Not implemented | - | No | 440 |
| 0x0D / 0x13 | TIM-SMEAS | Not implemented | - | No | 441 |
| 0x0D / 0x04 | TIM-SVIN | Not implemented | - | No | 443 |
| 0x0D / 0x03 | TIM-TM2 | Not implemented | - | No | 444 |
| 0x0D / 0x12 | TIM-TOS | Not implemented | - | No | 445 |
| 0x0D / 0x01 | TIM-TP | Not implemented | - | No | 447 |
| 0x0D / 0x15 | TIM-VCOCAL | Not implemented | - | No | 449, 450, 451 |
| 0x0D / 0x06 | TIM-VRFY | Not implemented | - | No | 452 |
| 0x09 / 0x14 | UPD-SOS | Not implemented | - | No | 453, 454, 455 |


[All protocols](supported-protocols.md)
