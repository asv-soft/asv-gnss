# RTCM v2

Legacy differential GNSS correction and navigation messages.

## Connect the parser

```csharp
using Asv.Gnss;
using Asv.IO;

var protocol = Protocol.Create(builder =>
{
    builder.Protocols.RegisterRtcmV2Protocol();
});
```

The stream parser is `RtcmV2Parser` and its message model is `RtcmV2MessageBase`.
With the standard Asv.IO builder, registration also installs `RtcmV2Encoder` for outgoing streams.
See [Quick Start](quick-start.md) for router, port, and subscription setup. The receiver must already be configured to emit the required data.

## Default message coverage

The default registration covers message types 1, 9, 14, 15, 17, 21, and 31.

| Message model |
|---------------|
| `RtcmV2Message1` |
| `RtcmV2Message9` |
| `RtcmV2Message14` |
| `RtcmV2Message15` |
| `RtcmV2Message17` |
| `RtcmV2Message21` |
| `RtcmV2Message31` |

## Encode and transmit

The project references the NuGet packages `Asv.IO` and `Asv.Common` 3.7.0. `RtcmV2Encoder` implements the Asv.IO `IProtocolEncoder` contract: it adds GPS parity, data inversion, and six-of-eight transport framing to normalized message data. Standard ports use the registered encoder automatically when sending RTCM v2 messages.

For direct encoding, create one encoder for each outgoing stream and reuse it in transmission order:

```csharp
var encoder = new RtcmV2Encoder();
var message = new RtcmV2Message1
{
    ReferenceStationId = 42,
    SequenceNumber = 0,
    ZCount = 948.0,
    Udre = 1.0,
    ObservationItems =
    [
        new DObservationItem(NavigationSystemEnum.SYS_GPS)
        {
            Prn = 24,
            Prc = -7.46,
            Rrc = 0,
            Iod = 42,
            Udre = SatUdreEnum.LessOne
        }
    ]
};

byte[] wire = message.EncodeToArray(encoder);
```

The previous word's D29/D30 bits affect the next word, including across message boundaries. Encoder instances are not thread-safe. Serialize encoding and sending in the same order, and create a new instance when starting a new independent stream. `GetMaxByteSize` does not advance state; rejected encoding leaves state unchanged. A failed transport write can leave the remote stream incomplete: restart or resynchronize that stream before continuing.

`RtcmV2MessageBase.Serialize` and `GetByteSize` operate on normalized 24-bit data words, matching `Deserialize`; they exclude parity and transport framing. Use the encoder for receiver-ready bytes. A normalized packet contains 6 to 99 bytes; its transport representation contains 10 to 165 bytes. Parsed messages retain unmodeled and fill bits when the normalized packet size remains unchanged.

## Model details and limitations

Types 1, 9, and 31 encode 40-bit satellite corrections and select the fine or coarse correction scale as needed. PRN 32 is encoded as zero. NaN correction values encode an unavailable correction; out-of-range finite values are rejected.

Type 14 uses `GpsTime` as GPS time, without an additional UTC conversion. Its week is encoded modulo 1024, and `LeapSeconds` supplies the six-bit GPS-minus-UTC offset. Set `ZCount` separately to the seconds within the hour, in 0.6-second increments. The decoded header includes reference-station ID, sequence number, modified Z-count, GPS time, and station health exposed through the existing `Udre` property. GPS hour and week reconstruction use the current time, so historical replay requires care.

Type 15 preserves the existing API conversion formulas: `IonosphericDelay = rawUnsigned14 * 0.001` and `IonoRateOfChange = rawSigned14 * 0.05`. These legacy model values are not the specification's SI scaling; convert explicitly when interpreting them physically. Records include the two reserved leading bits and occupy 36 bits each.

Type 17 uses the specification's 480-bit payload layout. `WeekNumberRaw` supplies the ten-bit week, `Toc` supplies the GPS clock-reference time, and `A` is the semi-major axis (the encoder writes its square root). Existing raw-value properties such as `Cus` and `Toes` retain their public representation. The corrected payload layout replaces the former extra skipped bits in the decoder.

Type 21 exposes its unmodeled payload as `RawData`, consisting of complete three-byte data words. It supports opaque payload replay and transport encoding; individual high-accuracy correction fields are not modeled.

## Implementation reference

Coverage above follows `Protocols/Rtcm/V2/RtcmV2Protocol.cs` under `src/Asv.Gnss`. Inspect concrete message classes for field names, units, and supported payload revisions.

## Specification message catalog

Source: `Protocols/Rtcm/V2/Docs/c10402.3 + Amend 1 - RTCM VER 2-3 (3).pdf`, RTCM 10402.3 with Amendment 1, Chapter 4.3 and its contents on PDF p. 8. All named types and undefined/reserved ranges in that contents list are represented. Type 34 covers partial differential corrections for N > 1 and a null frame for N <= 1, as confirmed by the message summary on PDF p. 50. The amended type 27 is included.

**Status:** **Model present** means a concrete payload model exists in the current checkout; it does not certify complete field, revision, encoding, or firmware coverage. **Partial** identifies a known incomplete decoder. **Not implemented** means no matching concrete model was found. **Default** reports whether the standard registration adds that model. Models outside the default set require explicit registration. See the limitations above before transmitting messages.

Page references below are **PDF page numbers (1-based)**, including covers and front matter.

| Type | Message / source designation | Status | C# model | Default |
| --- | --- | --- | --- | --- |
| 1 | Differential GPS Corrections (Fixed) | Model present | `RtcmV2Message1` | Yes |
| 2 | Delta Differential GPS Corrections (Fixed) | Not implemented | - | No |
| 3 | GPS Reference Station Parameters (Fixed) | Not implemented | - | No |
| 4 | Reference Station Datum Message (Tentative) | Not implemented | - | No |
| 5 | GPS Constellation Health (Fixed) | Not implemented | - | No |
| 6 | GPS Null Frame (Fixed) | Not implemented | - | No |
| 7 | DGPS Radiobeacon Almanac (Fixed) | Not implemented | - | No |
| 8 | Pseudolite Almanac (Tentative) | Not implemented | - | No |
| 9 | GPS Partial Correction Set (Fixed) | Model present | `RtcmV2Message9` | Yes |
| 10 | P-Code Differential Corrections (Reserved) | Not implemented | - | No |
| 11 | C/A Code L2 Corrections (Reserved) | Not implemented | - | No |
| 12 | Pseudolite Station Parameters (Reserved) | Not implemented | - | No |
| 13 | Ground Transmitter Parameters (Tentative) | Not implemented | - | No |
| 14 | GPS Time of Week (Fixed) | Model present | `RtcmV2Message14` | Yes |
| 15 | Ionospheric Delay Message (Fixed) | Model present | `RtcmV2Message15` | Yes |
| 16 | GPS Special Message (Fixed) | Not implemented | - | No |
| 17 | GPS Ephemerides (Tentative) | Model present | `RtcmV2Message17` | Yes |
| 18 | RTK Uncorrected Carrier Phases (Fixed*) | Not implemented | - | No |
| 19 | RTK Uncorrected Pseudoranges (Fixed*) | Not implemented | - | No |
| 20 | RTK Carrier Phase Corrections (Fixed*) | Not implemented | - | No |
| 21 | High-Accuracy Pseudorange Corrections (Fixed*) | Partial: opaque payload | `RtcmV2Message21` | Yes |
| 22 | Extended Reference Station Parameters (Tent) | Not implemented | - | No |
| 23 | Antenna Type Definition Record (Tentative) | Not implemented | - | No |
| 24 | Antenna Reference Point (ARP) (Tentative) | Not implemented | - | No |
| 27 | Extended Radiobeacon Almanac (Fixed) | Not implemented | - | No |
| 31 | Differential GLONASS Corrections (Tentative) | Model present | `RtcmV2Message31` | Yes |
| 32 | GLONASS Reference Station Parameters (Tent) | Not implemented | - | No |
| 33 | GLONASS Constellation Health (Tentative) | Not implemented | - | No |
| 34 | GLONASS Partial Differential Correction Set (N > 1) / Null Frame (N <= 1) | Not implemented | - | No |
| 35 | GLONASS Radiobeacon Almanac | Not implemented | - | No |
| 36 | GLONASS Special Message | Not implemented | - | No |
| 37 | GNSS System Time Offset | Not implemented | - | No |
| 59 | Proprietary Message (Fixed) | Not implemented | - | No |
| 0 | Undefined | Not implemented | - | No |
| 25-26 | Undefined | Not implemented | - | No |
| 28-30 | Undefined | Not implemented | - | No |
| 38-58 | Undefined | Not implemented | - | No |
| 60-63 | Reserved multipurpose use | Not implemented | - | No |


[All protocols](supported-protocols.md)
