# ASV

ASV-specific heartbeat, GNSS observations, raw navigation data, PVT, and GBAS data messages.

## Connect the parser

```csharp
using Asv.Gnss;
using Asv.IO;

var protocol = Protocol.Create(builder =>
{
    builder.Protocols.RegisterAsvProtocol();
});
```

The stream parser is `AsvMessageParser` and its message model is `AsvMessageBase`.
See [Quick Start](quick-start.md) for router, port, and subscription setup. Registration selects decoders; the receiver must already be configured to emit the required data.

## Default message coverage

Frames use sync bytes `AA 44`, a 10-byte header, a payload of up to 1012 bytes, and a 2-byte CRC. The message ID is a `ushort`.

| Message model |
|---------------|
| `AsvMessageHeartBeat` |
| `AsvMessageGbasVdbSend` |
| `AsvMessageGbasVdbSendV2` |
| `AsvMessageGbasMonDevSendV2` |
| `AsvMessageGpsObservations` |
| `AsvMessageGloObservations` |
| `AsvMessageGpsRawCa` |
| `AsvMessageGloRawCa` |
| `AsvMessagePvtGeo` |

## Usage and limitations

Use these models with endpoints that implement this ASV wire format. GBAS message transport does not itself validate aviation requirements or establish equipment interoperability. Raw GPS and GLONASS payload decoding is described on the corresponding raw-navigation pages.

## Implementation reference

Coverage above follows `Protocols/Asv/AsvProtocol.cs` under `src/Asv.Gnss`. Inspect concrete message classes for field names, units, and supported payload revisions.

## Specification message catalog

No ASV protocol PDF is included in this repository. The table therefore lists every concrete message in `Protocols/Asv/Messages`, using the source IDs and names, rather than claiming a PDF-wide inventory. `AsvProtocol` is the registration source.

**Status:** **Model present** means a concrete payload model exists in the current checkout; it does not certify complete field, revision, encoding, or firmware coverage. **Partial** identifies a known incomplete decoder. **Not implemented** means no matching concrete model was found. **Default** reports whether the standard registration adds that model. Models outside the default set require explicit registration. See the limitations above before transmitting messages.

Page references below are **PDF page numbers (1-based)**, including covers and front matter.

| Message ID | Name | Status | C# model | Default |
| --- | --- | --- | --- | --- |
| 0x0000 | HeartBeat | Model present | `AsvMessageHeartBeat` | Yes |
| 0x0100 | GbasVdbSend | Model present | `AsvMessageGbasVdbSend` | Yes |
| 0x0102 | GbasVdbSendV2 | Model present | `AsvMessageGbasVdbSendV2` | Yes |
| 0x0103 | GbasCuSendV2 | Model present | `AsvMessageGbasMonDevSendV2` | Yes |
| 0x0110 | GpsObservation | Model present | `AsvMessageGpsObservations` | Yes |
| 0x0111 | GloObservation | Model present | `AsvMessageGloObservations` | Yes |
| 0x0112 | GpsRawCa | Model present | `AsvMessageGpsRawCa` | Yes |
| 0x0113 | GloRawCa | Model present | `AsvMessageGloRawCa` | Yes |
| 0x0114 | PvtGeo | Model present | `AsvMessagePvtGeo` | Yes |


[All protocols](supported-protocols.md)
