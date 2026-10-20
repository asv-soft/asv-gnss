# ComNav

ComNav receiver binary logs, ASCII logs, text commands, and simple command responses.

## Connect the parser

```csharp
using Asv.Gnss;
using Asv.IO;

var protocol = Protocol.Create(builder =>
{
    builder.Protocols.RegisterComNavProtocol();
});
```

The stream parser is `ComNavBinaryParser / ComNavAsciiParser / ComNavSimpleAnswerParser` and its message model is `ComNavBinaryMessageBase / ComNavAsciiMessageBase / ComNavSimpleAnswerMessageBase`.
See [Quick Start](quick-start.md) for router, port, and subscription setup. Registration selects decoders; the receiver must already be configured to emit the required data.

## Default message coverage

`RegisterComNavProtocol` enables three parsers. Register them separately with `RegisterComNavBinaryProtocol`, `RegisterComNavAsciiProtocol`, or `RegisterComNavSimpleAnswerProtocol` when only one stream format is needed.

| Message model |
|---------------|
| `ComNavBinaryPsrPosPacket` |
| `ComNavBinaryPsrDopPacket` |
| `ComNavBinaryBestPosPacket` |
| `ComNavBinaryRawGpsSubFramePacket` |
| `ComNavBinaryRawGloEphemPacket` |
| `ComNavBinaryObservationInfo` |
| `ComNavLogListAMessage` |
| `ComNavComConfigAMessage` |
| `ComNavPsrPosAMessage` |
| `ComNavSimpleOkMessage` |
| `ComNavSimpleErrorMessage` |
| `ComNavSimpleTransmitMessage` |

## Usage and limitations

Incoming log models and outgoing ASCII commands have different capabilities. ASCII log serialization and size calculation are unimplemented in their base class. Several binary payload encoders are unimplemented; `ComNavBinaryObservationInfo` decodes only selected RANGE fields. Command models live in `Parsers/ComNav/Ascii/Commands`; their deserialization is unsupported. Receiving OK or ERROR is separate from receiving an ASCII or binary log.

## Implementation reference

Coverage above follows `Parsers/ComNav/ComNavProtocol.cs` under `src/Asv.Gnss`. The message tables follow `ComNavBinaryFactory`, `ComNavAsciiFactory`, and `ComNavSimpleAnswerFactory`. Inspect concrete message classes for field names, units, and supported payload revisions.

## Specification message catalog

Source: `Parsers/ComNav/Docs/ComNav_OEM_Board_Reference_Manual_V1.8.pdf`, CNT-OEM-RM001 revision 1.8, Table 19 (PDF pp. 79-82). All 72 ComNav-native log entries are listed, including repeated numeric IDs with different names as printed in the manual. NMEA, RTCM and BINEX transported inside logs do not become independent ComNav payload implementations. `Parsers/ComNav/Docs/OEM7_Commands_Logs_Manual (Novatel).pdf` is a NovAtel OEM7 manual, not a ComNav conformance specification; shared names or IDs do not imply OEM7 support. The local port-map PDF is not a protocol catalog.

**Status:** **Model present** means a concrete payload model exists in the current checkout; it does not certify complete field, revision, encoding, or firmware coverage. **Partial** identifies a known incomplete decoder. **Not implemented** means no matching concrete model was found. **Default** reports whether the standard registration adds that model. Models outside the default set require explicit registration. See the limitations above before transmitting messages.

Page references below are **PDF page numbers (1-based)**, including covers and front matter.

| Log ID | Log name | Status | C# models (binary / ASCII) | Default | PDF page |
| --- | --- | --- | --- | --- | --- |
| 71 | BD2EPHEM | Not implemented | - | No | 79 |
| 72 | BD3EPHEM | Not implemented | - | No | 79 |
| 157 | BD3RAWNAVSUBFRAME | Not implemented | - | No | 79 |
| 741 | BD2RAWALM | Not implemented | - | No | 79 |
| 412 | BD2RAWEPHEM | Not implemented | - | No | 79 |
| 1695 | BDSRAWNAVSUBFRAME | Not implemented | - | No | 79 |
| 42 | BESTPOS | Model present | `ComNavBinaryBestPosPacket` | Yes | 80 |
| 99 | BESTVEL | Not implemented | - | No | 80 |
| 241 | BESTXYZ | Not implemented | - | No | 80 |
| 110 | BINEX00DATA | Not implemented | - | No | 80 |
| 81 | BINEX0101DATA | Not implemented | - | No | 80 |
| 82 | BINEX0102DATA | Not implemented | - | No | 80 |
| 85 | BINEX0105DATA | Not implemented | - | No | 80 |
| 114 | BINEX7D00DATA | Not implemented | - | No | 80 |
| 115 | BINEX7E00DATA | Not implemented | - | No | 80 |
| 120 | BINEX7F05DATA | Not implemented | - | No | 80 |
| 317 | COMCONFIG | Model present | `ComNavComConfigAMessage` | Yes | 80 |
| 723 | GLOEPHEMERIS | Not implemented | - | No | 80 |
| 792 | GLORAWEPHEM | Model present | `ComNavBinaryRawGloEphemPacket` | Yes | 80 |
| 71 | GPSEPHEM | Not implemented | - | No | 80 |
| 1122 | GALEPHEMERIS | Not implemented | - | No | 80 |
| 1413 | GALFNAVRAWPAGE | Not implemented | - | No | 80 |
| 1414 | GALINAVRAWWORD | Not implemented | - | No | 80 |
| 712 | GPSEPHEM | Not implemented | - | No | 80 |
| 971 | HEADING | Not implemented | - | No | 80 |
| 8 | IONUTC | Not implemented | - | No | 80 |
| 5 | LOGLIST | Model present | `ComNavLogListAMessage` | Yes | 80 |
| 925 | M925 | Not implemented | - | No | 80 |
| 181 | MARKPOS | Not implemented | - | No | 81 |
| 231 | MARKTIME | Not implemented | - | No | 81 |
| 106 | METEODATA | Not implemented | - | No | 81 |
| 108 | METEODATAEXT | Not implemented | - | No | 81 |
| 174 | PSRDOP | Model present | `ComNavBinaryPsrDopPacket` | Yes | 81 |
| 47 | PSRPOS | Model present | `ComNavBinaryPsrPosPacket`; `ComNavPsrPosAMessage` | Yes | 81 |
| 100 | PSRVEL | Not implemented | - | No | 81 |
| 243 | PSRXYZ | Not implemented | - | No | 81 |
| 901 | QXWZSDKINFOB | Not implemented | - | No | 81 |
| 1330 | QZSSRAWSUBFRAM | Not implemented | - | No | 81 |
| 1331 | QZSSRAWEPHEM | Not implemented | - | No | 81 |
| 43 | RANGE | Partial | `ComNavBinaryObservationInfo` | Yes | 81 |
| 140 | RANGECMP | Not implemented | - | No | 81 |
| 74 | RAWALM | Not implemented | - | No | 81 |
| 25 | RAWGPSSUBFRAME | Model present | `ComNavBinaryRawGpsSubFramePacket` | Yes | 81 |
| 41 | RAWEPHEM | Not implemented | - | No | 81 |
| 973 | RAWSBASFRAME | Not implemented | - | No | 81 |
| 175 | REFSTATION | Not implemented | - | No | 81 |
| 396 | RTCMDATA1 | Not implemented | - | No | 81 |
| 911 | SATMSG | Not implemented | - | No | 81 |
| 48 | SATVIS | Not implemented | - | No | 81 |
| 270 | SATXYZ | Not implemented | - | No | 81 |
| 976 | SBAS0 | Not implemented | - | No | 81 |
| 977 | SBAS1 | Not implemented | - | No | 81 |
| 982 | SBAS2 | Not implemented | - | No | 81 |
| 987 | SBAS3 | Not implemented | - | No | 81 |
| 992 | SBAS4 | Not implemented | - | No | 81 |
| 994 | SBAS5 | Not implemented | - | No | 81 |
| 995 | SBAS6 | Not implemented | - | No | 82 |
| 996 | SBAS7 | Not implemented | - | No | 82 |
| 997 | SBAS9 | Not implemented | - | No | 82 |
| 978 | SBAS10 | Not implemented | - | No | 82 |
| 979 | SBAS12 | Not implemented | - | No | 82 |
| 980 | SBAS17 | Not implemented | - | No | 82 |
| 981 | SBAS18 | Not implemented | - | No | 82 |
| 983 | SBAS24 | Not implemented | - | No | 82 |
| 984 | SBAS25 | Not implemented | - | No | 82 |
| 985 | SBAS26 | Not implemented | - | No | 82 |
| 986 | SBAS27 | Not implemented | - | No | 82 |
| 975 | SBAS28 | Not implemented | - | No | 82 |
| 1003 | SBAS63 | Not implemented | - | No | 82 |
| 101 | TIME | Not implemented | - | No | 82 |
| 83 | TRACKSTAT | Not implemented | - | No | 82 |
| 37 | VERSION | Not implemented | - | No | 82 |

### ASCII commands

All 45 commands in the manual's command contents (PDF pp. 12-13). A command model provides outbound text construction; it is not an incoming log decoder and is not registered in the incoming message factories. Individual command variants and every SET option are not implied.

| Command | Purpose | Status | C# model | PDF section |
| --- | --- | --- | --- | --- |
| ASSIGN | Assign a channel to a PRN | Not implemented | - | 3.2.1 |
| BD2ECUTOFF | Set BD2 satellite elevation cut-off | Not implemented | - | 3.2.2 |
| CLOCKOFFSET | Adjust for delay in 1 PPS output | Not implemented | - | 3.2.3 |
| COM | Set baud rate | Not implemented | - | 3.2.4 |
| DGPSTXID | DGPS transmit ID | Command model present | `ComNavDgpsTxIdCommand` | 3.2.5 |
| DYNAMICS | Tune board parameters | Not implemented | - | 3.2.6 |
| ECUTOFF | Set satellite elevation cut-off angle | Not implemented | - | 3.2.7 |
| ERASEFLASH | Erase files restored in flash | Not implemented | - | 3.2.8 |
| FIX | Constrain to fixed height and position | Command model present | `ComNavFixCommand` | 3.2.9 |
| FRESET | Reset to the factory default | Not implemented | - | 3.2.10 |
| HEADINGOFFSET | Add heading and pitch offset values | Not implemented | - | 3.2.11 |
| INTERFACEMODE | Set receive or transmit modes for ports | Not implemented | - | 3.2.12 |
| LOCKOUT | Prevent the board from using a satellite | Not implemented | - | 3.2.13 |
| LOCKOUTSYSTEM | Prevent the receiver from using a system | Command model present | `ComNavSetLockoutSystemCommand` | 3.2.14 |
| LOG | Request logs from board | Command model present | `ComNavAsciiLogCommand` | 3.2.15 |
| MAGVAR | Set a magnetic variation correction | Not implemented | - | 3.2.16 |
| MARKCONTROL | Mark message control | Not implemented | - | 3.2.17 |
| MAXVECLENERR | Set the directional flypoint detection threshold | Not implemented | - | 3.2.18 |
| NMEATALKER | NMEA message talker identifier control | Not implemented | - | 3.2.19 |
| PPSCONTROL | Control the PPS output style | Command model present | `ComNavSetPpsControlCommand` | 3.2.20 |
| PPMADJUST | Adjust PPM or not | Not implemented | - | 3.2.21 |
| READFLASH | Read files from flash | Not implemented | - | 3.2.22 |
| REFAUTOSETUP | Set base station self-starting | Not implemented | - | 3.2.23 |
| RESET | Perform a hardware reset | Not implemented | - | 3.2.24 |
| RTKCOMMAND | Reset or set the RTK filter to its defaults | Not implemented | - | 3.2.25 |
| RTKDYNAMICS | Set RTK dynamic mode | Not implemented | - | 3.2.26 |
| RTKELEVMASK | Set the RTK elevation mask angle | Not implemented | - | 3.2.27 |
| RTKFIXHOLDTIME | Set maximum age of RTK fixed data | Not implemented | - | 3.2.28 |
| RTKOBSMODE | Set the observation mode of rover receiver | Not implemented | - | 3.2.29 |
| RTKREFMODE | Set the RTK ref-station position mode | Not implemented | - | 3.2.30 |
| RTKSOLUTION | Set RTK solution mode | Not implemented | - | 3.2.31 |
| RTKSOURCE | Set RTK correction source | Not implemented | - | 3.2.32 |
| RTKTIMEOUT | Set maximum age of RTK data | Not implemented | - | 3.2.33 |
| RTKQUALITY | Set RTK quality level | Not implemented | - | 3.2.34 |
| SAVECONFIG | Save current configuration | Command model present | `ComNavSaveConfigCommand` | 3.2.35 |
| SBASCONTROL | Control the usage of SBAS corrections | Not implemented | - | 3.2.36 |
| SBASECUTOFF | Set SBAS satellite elevation cut-off | Not implemented | - | 3.2.37 |
| SBASTIMEOUT | Set SBAS corrections time out | Not implemented | - | 3.2.38 |
| SET | configure settings | Command model present | `ComNavAsciiSetCommand` | 3.2.39 |
| UNDULATION | Choose undulation | Not implemented | - | 3.2.40 |
| UNLOCKOUT | Reinstate a satellite in the solution | Not implemented | - | 3.2.41 |
| UNLOCKOUTALL | Reinstate a satellite in the solution | Command model present | `ComNavUnLockoutAllSystemCommand` | 3.2.42 |
| UNLOCKOUTSYSTEM | Reinstate previously locked out system | Command model present | `ComNavSetUnLockoutSystemCommand` | 3.2.43 |
| UNLOG | Remove a log from logging control | Not implemented | - | 3.2.44 |
| UNLOGALL | Remove all logs from logging control | Command model present | `ComNavUnLogAllCommand` | 3.2.45 |

### Simple command responses

These textual reply models are registered separately from log messages.

| Response | C# model | Default |
| --- | --- | --- |
| OK! | `ComNavSimpleOkMessage` | Yes |
| Error! | `ComNavSimpleErrorMessage` | Yes |
| Command transmited | `ComNavSimpleTransmitMessage` | Yes |


[All protocols](supported-protocols.md)
