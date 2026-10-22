# UBX X20 HPG 2.11

The X20 profile models the UBX interface in the official **ZED-X20P X20 HPG 2.11**
JSON, whose reported protocol version is **50.11**. It contains **163 payload
variants**, **101 class/ID pairs** across **11 UBX classes**, and **826 configuration
keys** in **38 groups**. The upstream JSON marks its message and configuration
content as filtered for this receiver/firmware profile. This coverage does not
assert support for every u-blox product or firmware release.

Sources: [pinned official JSON](https://github.com/u-blox/u-blox-X20-interface-description-json/blob/02c3c9302cacf1b2fb17eeb10a6a4e0b7ef2379e/HPG/ZED-X20P/u-blox-X20-HPG-2.11.json),
[interface description, UBXDOC-304424225-21617 R01](https://content.u-blox.com/sites/default/files/documents/u-blox-X20-HPG-2.11_InterfaceDescription_UBXDOC-304424225-21617.pdf),
and [upstream usage notice](https://github.com/u-blox/u-blox-X20-interface-description-json/blob/02c3c9302cacf1b2fb17eeb10a6a4e0b7ef2379e/README.md).
The source also describes NMEA, RTCM and SPARTN; the X20 registration below adds
the UBX models. Other stream protocols keep their separate registrations.

## Register the exact X20 profile

```c#
using Asv.Gnss;
using Asv.IO;

var protocol = Protocol.Create(builder =>
{
    builder.Protocols.RegisterUbxX20Protocol();
});
```

Register one UBX profile per protocol builder. `RegisterUbxX20Protocol` and
`RegisterUbxProtocol` share `UbxProtocol.Info`; registering both does not create
two independent UBX wire protocols. The X20 registration selects the generated
`UbxX20...` models by payload length, discriminator fields and subtype.

The normal `RegisterUbxProtocol` preserves the existing concrete models for IDs
already handled by its legacy factory and adds X20 decoding for new IDs. For
example, a default NAV-PVT remains `UbxNavPvt`; choose the explicit X20 profile to
receive `UbxX20NavPvtData1` and its native fields. Custom legacy registrations
also keep precedence for their IDs. See [UBX](protocol-ubx.md) for the legacy API.

The default parser keeps its **8192-byte complete-frame limit**. The explicit
X20 profile uses a bounded **65543-byte complete-frame buffer**, allowing the UBX
maximum payload of 65535 bytes plus eight framing/checksum bytes. Recognizing a
new ID in the default profile does not increase its buffer limit.

## Parse a complete frame

After creating `protocol` above, feed bytes to a parser or attach a router/port as
shown in [Quick Start](quick-start.md). This independent frame contains
`UBX-NAV-EOE` with `iTOW = 1000 ms`:

```c#
using R3;

using var parser = protocol.CreateParser(UbxProtocol.Info.Id);
using var subscription = parser.OnMessage.Subscribe(message =>
{
    if (message is UbxX20NavEoeData0 epoch)
        Console.WriteLine($"{epoch.Name}: {epoch.ITOW} ms");
});

byte[] frame =
[
    0xB5, 0x62, 0x01, 0x61, 0x04, 0x00,
    0xE8, 0x03, 0x00, 0x00, 0x51, 0x70,
];
foreach (var value in frame)
    parser.Push(value);
```

The parser checks framing, length and checksum, selects the payload variant and
deserializes it before publishing. Subscribe to `parser.OnError` when the
application needs diagnostics for damaged frames or unsupported variants.
Registration does not configure receiver output or confirm command acceptance;
use receiver acknowledgements and the applicable integration manual.

## Create and serialize a specific variant

`UbxX20Protocol.Messages` exposes the full variant catalog, including message
direction, concrete type and a constructor delegate. `CreateMessage(id, variant)`
selects an exact documented variant for transmission or direct deserialization.
The variant name is the source identifier, not necessarily a version-byte value:
for example, CFG-VALGET `DATA0` carries wire version 1.

```c#
var poll = UbxX20Protocol.CreateMessage(0x0A04, "POLL0");
byte[] bytes = new byte[poll.GetByteSize()];
Span<byte> destination = bytes;
poll.Serialize(ref destination);
```

`UbxX20MessageFactory.Create(id)` creates its default model for that ID.
`Create(id, payload)` **only selects and constructs** a model from payload
length/version/subtype; it does not deserialize the payload, check its full
structure, or verify a UBX checksum. A returned model still has its initial
property values. Use the stream parser for complete received frames, or call a
specific model's `Deserialize` with a complete UBX frame. Unsupported selector
combinations return null; malformed selected payloads are rejected during
deserialization.

## Native values and visitors

Generated numeric properties retain their **native wire scale**. XML field
documentation and visitor metadata record units and scale factors. For example,
`UbxX20NavPosllhData0.Lon = 123456789` represents longitude `12.3456789` degrees;
`Height = 1000` represents one metre because the native field is millimetres.
No implicit conversion to the legacy models' convenience properties takes place.

All generated messages and nested records implement `IVisitable`. `Accept`
visits each canonical wire field once, with native type metadata. Scalar and
array-element edits use their backing storage by `ref`; a callback can observe
the edited property immediately. Repeated traversal of initialized collections
reuses storage; changing a variable array's length allocates replacement storage.

Convenience bit-field properties and `...Ascii` views are aliases and are not
visited separately. The canonical integer bit field or raw `byte[]` is the
authoritative field. ASCII views stop at the first NUL and use ASCII decoding;
the raw array retains padding, NUL bytes and bytes outside ASCII for exact
serialization.

Counts represented by a separate scalar, such as RAWX `NumMeas`, are read-only
properties derived from the corresponding array length. Their visitor metadata
contains `readOnly = true`; attempting to edit a derived count through a visitor
throws. Resize or replace the array to change the count. A count packed inside a
bit field remains part of that canonical field: update its count alias and the
array together. Serialization checks their consistency. Fixed arrays must keep
their declared size; incomplete records, inconsistent counts, unsupported
discriminators and trailing payload bytes are rejected.

Firmware restrictions still apply to exposed bit aliases. The NAV-STATUS
`fixStat.indoorDet` and `fixStat.selectiveL5` fields are documented only for
protocol versions **52.00 and later**, although they appear in the source for
this **50.11** profile. Their accessors preserve the bits; their presence does
not establish receiver support. Do not interpret them as supported features on
50.11 firmware.

## Configuration keys and values

`UbxX20ConfigurationCatalog` exposes all source groups, key IDs, names, native
types, scales, units, descriptions and named constants. Use `TryGet(uint, out
item)` or the exact case-sensitive name, such as `CFG-RATE-MEAS`.
`UbxX20ConfigurationKeys` provides named IDs. An empty named-constant value in
the JSON is preserved as unspecified text; it is not silently converted to zero.

Use exact CLR native types for values. `CFG-RATE-MEAS` is a `U2`, so 1000 must
be a `ushort`. Its source scale is `0.001 s`, giving a one-second measurement
period. The codec retains that raw value and does not apply the scale:

```c#
UbxX20ConfigurationValue[] values =
[
    new(UbxX20ConfigurationKeys.Rate.Meas, (ushort)1000),
    new(UbxX20ConfigurationKeys.Signal.GpsEna, true),
];

var set = new UbxX20CfgValsetData0
{
    Layers = (byte)UbxX20ConfigurationLayers.Ram,
    CfgData = UbxX20ConfigurationCodec.EncodeValues(values),
};

var decoded = UbxX20ConfigurationCodec.DecodeValues(set.CfgData);
```

For a received `UbxX20CfgValgetData0`, pass its deserialized `CfgData` property
to the same `DecodeValues` method.

| Native type | CLR value |
| --- | --- |
| `L` | `bool` |
| `U1`, `E1`, `X1` | `byte` |
| `U2`, `E2`, `X2` | `ushort` |
| `U4`, `E4`, `X4` | `uint` |
| `U8`, `X8` | `ulong` |
| `I1`, `I2`, `I4`, `I8` | `sbyte`, `short`, `int`, `long` |
| `R4`, `R8` | `float`, `double` |

Unknown concrete keys with valid size/group/item fields decode to raw `byte[]`
values. Size alone cannot determine whether their contents are signed integers,
floating-point values or bit fields. Reserved key fields, invalid storage sizes,
incomplete key/value pairs and messages exceeding **64 entries** are rejected.
Accepted ranges, hardware support and cross-key dependencies remain receiver
responsibilities.

`EncodeKeys`/`DecodeKeys` handle VALGET-poll and VALDEL key lists. These lists
also accept documented wildcards: a valid group with item `0xffff`, or group
`0xfff` in bits 16 through 27 to select all groups. Wildcard storage bits are not
interpreted as value sizes; reserved bit 31 must remain clear. Use
`UbxX20ConfigurationKeys.Rate.All` or `UbxX20ConfigurationCodec.AllKeys` for the
canonical selectors. Wildcards cannot appear in key-value data.

`CreateValSet`, `CreateValDelete`, `CreateValGetRequest` and
`CreateValGetResponse` return complete CFG-VAL **payload bytes**, including the
four-byte configuration header, without UBX framing or checksum. For a generated
message's `CfgData` property use `EncodeValues`, which returns only the key/value
data. Assigning a complete helper payload to `CfgData` would duplicate its header.

```c#
byte[] pollPayload = UbxX20ConfigurationCodec.CreateValGetRequest(
    [UbxX20ConfigurationKeys.Rate.All],
    UbxX20ConfigurationLayer.Default,
    position: 0);

byte[] transactionPayload = UbxX20ConfigurationCodec.CreateValSet(
    values,
    UbxX20ConfigurationLayers.Ram | UbxX20ConfigurationLayers.Bbr,
    UbxX20ConfigurationTransaction.Begin);
```

VALSET accepts RAM, BBR and Flash flags. VALDEL accepts BBR and Flash only.
VALGET uses one numeric layer: RAM 0, BBR 1, Flash 2, Default 7; these values
differ from write-layer flags. A null transaction selects VALSET/VALDEL version
0; an explicit action selects version 1, including `None = 0`. Empty entry lists
allow transaction management. These helpers validate individual messages rather
than track receiver transaction state. Keep the same layers across a transaction,
follow its action sequence and handle receiver ACK/NAK responses. Read large
wildcard results in pages using the VALGET position field.

## Documented UBX variant catalog

The following table matches every UBX variant in the pinned source and
`UbxX20Protocol.Messages`. Direction values are the unmodified manufacturer
metadata; they are not runtime permissions. A row certifies a typed codec for
that documented layout, not hardware verification or acceptance by another
firmware version. Variable and optional groups are represented by their models;
the table does not infer a single payload size for those layouts.

<!-- X20-VARIANT-CATALOG -->

| Class / ID | Message | Variant | C# model | Direction |
| --- | --- | --- | --- | --- |
| `0x05 / 0x01` | `UBX-ACK-ACK` | `DATA0` | `UbxX20AckAckData0` | `output` |
| `0x05 / 0x00` | `UBX-ACK-NAK` | `DATA0` | `UbxX20AckNakData0` | `output` |
| `0x06 / 0x09` | `UBX-CFG-CFG` | `DATA1` | `UbxX20CfgCfgData1` | `command` |
| `0x06 / 0x41` | `UBX-CFG-OTP` | `WRITEFILEJU_A4` | `UbxX20CfgOtpWritefilejuA4` | `set` |
| `0x06 / 0x04` | `UBX-CFG-RST` | `DATA3` | `UbxX20CfgRstData3` | `command` |
| `0x06 / 0x8C` | `UBX-CFG-VALDEL` | `DATA0` | `UbxX20CfgValdelData0` | `set` |
| `0x06 / 0x8C` | `UBX-CFG-VALDEL` | `DATA1` | `UbxX20CfgValdelData1` | `set` |
| `0x06 / 0x8B` | `UBX-CFG-VALGET` | `POLLID` | `UbxX20CfgValgetPollid` | `pollrequest` |
| `0x06 / 0x8B` | `UBX-CFG-VALGET` | `DATA0` | `UbxX20CfgValgetData0` | `polled` |
| `0x06 / 0x8A` | `UBX-CFG-VALSET` | `DATA0` | `UbxX20CfgValsetData0` | `set` |
| `0x06 / 0x8A` | `UBX-CFG-VALSET` | `DATA1` | `UbxX20CfgValsetData1` | `set` |
| `0x04 / 0x04` | `UBX-INF-DEBUG` | `DATA0` | `UbxX20InfDebugData0` | `output` |
| `0x04 / 0x00` | `UBX-INF-ERROR` | `DATA0` | `UbxX20InfErrorData0` | `output` |
| `0x04 / 0x02` | `UBX-INF-NOTICE` | `DATA0` | `UbxX20InfNoticeData0` | `output` |
| `0x04 / 0x03` | `UBX-INF-TEST` | `DATA0` | `UbxX20InfTestData0` | `output` |
| `0x04 / 0x01` | `UBX-INF-WARNING` | `DATA0` | `UbxX20InfWarningData0` | `output` |
| `0x13 / 0x60` | `UBX-MGA-ACK-DATA0` | `DATA0` | `UbxX20MgaAckData0` | `output` |
| `0x13 / 0x03` | `UBX-MGA-BDS-EPH` | `EPH` | `UbxX20MgaBdsEph` | `input` |
| `0x13 / 0x03` | `UBX-MGA-BDS-ALM` | `ALM` | `UbxX20MgaBdsAlm` | `input` |
| `0x13 / 0x03` | `UBX-MGA-BDS-HEALTH` | `HEALTH` | `UbxX20MgaBdsHealth` | `input` |
| `0x13 / 0x03` | `UBX-MGA-BDS-UTC` | `UTC` | `UbxX20MgaBdsUtc` | `input` |
| `0x13 / 0x03` | `UBX-MGA-BDS-IONO` | `IONO` | `UbxX20MgaBdsIono` | `input` |
| `0x13 / 0x80` | `UBX-MGA-DBD` | `POLL` | `UbxX20MgaDbdPoll` | `pollrequest` |
| `0x13 / 0x80` | `UBX-MGA-DBD` | `DATA0` | `UbxX20MgaDbdData0` | `input_output` |
| `0x13 / 0x02` | `UBX-MGA-GAL-EPH` | `EPH` | `UbxX20MgaGalEph` | `input` |
| `0x13 / 0x02` | `UBX-MGA-GAL-ALM` | `ALM` | `UbxX20MgaGalAlm` | `input` |
| `0x13 / 0x02` | `UBX-MGA-GAL-TIMEOFFSET` | `TIMEOFFSET` | `UbxX20MgaGalTimeoffset` | `input` |
| `0x13 / 0x02` | `UBX-MGA-GAL-UTC` | `UTC` | `UbxX20MgaGalUtc` | `input` |
| `0x13 / 0x02` | `UBX-MGA-GAL-OSNMA_PUBKEY` | `OSNMA_PUBKEY` | `UbxX20MgaGalOsnmaPubkey` | `input` |
| `0x13 / 0x02` | `UBX-MGA-GAL-OSNMA_MERKLE` | `OSNMA_MERKLE` | `UbxX20MgaGalOsnmaMerkle` | `input` |
| `0x13 / 0x06` | `UBX-MGA-GLO-EPH` | `EPH` | `UbxX20MgaGloEph` | `input` |
| `0x13 / 0x06` | `UBX-MGA-GLO-ALM` | `ALM` | `UbxX20MgaGloAlm` | `input` |
| `0x13 / 0x06` | `UBX-MGA-GLO-TIMEOFFSET` | `TIMEOFFSET` | `UbxX20MgaGloTimeoffset` | `input` |
| `0x13 / 0x00` | `UBX-MGA-GPS-EPH` | `EPH` | `UbxX20MgaGpsEph` | `input` |
| `0x13 / 0x00` | `UBX-MGA-GPS-ALM` | `ALM` | `UbxX20MgaGpsAlm` | `input` |
| `0x13 / 0x00` | `UBX-MGA-GPS-HEALTH` | `HEALTH` | `UbxX20MgaGpsHealth` | `input` |
| `0x13 / 0x00` | `UBX-MGA-GPS-UTC` | `UTC` | `UbxX20MgaGpsUtc` | `input` |
| `0x13 / 0x00` | `UBX-MGA-GPS-IONO` | `IONO` | `UbxX20MgaGpsIono` | `input` |
| `0x13 / 0x40` | `UBX-MGA-INI-POS_XYZ` | `POS_XYZ` | `UbxX20MgaIniPosXyz` | `input` |
| `0x13 / 0x40` | `UBX-MGA-INI-POS_LLH` | `POS_LLH` | `UbxX20MgaIniPosLlh` | `input` |
| `0x13 / 0x40` | `UBX-MGA-INI-TIME_UTC` | `TIME_UTC` | `UbxX20MgaIniTimeUtc` | `input` |
| `0x13 / 0x40` | `UBX-MGA-INI-TIME_GNSS` | `TIME_GNSS` | `UbxX20MgaIniTimeGnss` | `input` |
| `0x13 / 0x40` | `UBX-MGA-INI-CLKD` | `CLKD` | `UbxX20MgaIniClkd` | `input` |
| `0x13 / 0x40` | `UBX-MGA-INI-FREQ` | `FREQ` | `UbxX20MgaIniFreq` | `input` |
| `0x13 / 0x05` | `UBX-MGA-QZSS-EPH` | `EPH` | `UbxX20MgaQzssEph` | `input` |
| `0x13 / 0x05` | `UBX-MGA-QZSS-ALM` | `ALM` | `UbxX20MgaQzssAlm` | `input` |
| `0x13 / 0x05` | `UBX-MGA-QZSS-HEALTH` | `HEALTH` | `UbxX20MgaQzssHealth` | `input` |
| `0x0A / 0x36` | `UBX-MON-COMMS` | `POLL0` | `UbxX20MonCommsPoll0` | `other` |
| `0x0A / 0x36` | `UBX-MON-COMMS` | `DATA0` | `UbxX20MonCommsData0` | `periodic_polled` |
| `0x0A / 0x28` | `UBX-MON-GNSS` | `POLL0` | `UbxX20MonGnssPoll0` | `other` |
| `0x0A / 0x28` | `UBX-MON-GNSS` | `DATA1` | `UbxX20MonGnssData1` | `polled` |
| `0x0A / 0x37` | `UBX-MON-HW3` | `POLL0` | `UbxX20MonHw3Poll0` | `other` |
| `0x0A / 0x37` | `UBX-MON-HW3` | `DATA0` | `UbxX20MonHw3Data0` | `periodic_polled` |
| `0x0A / 0x27` | `UBX-MON-PATCH` | `POLL0` | `UbxX20MonPatchPoll0` | `pollrequest` |
| `0x0A / 0x27` | `UBX-MON-PATCH` | `DATA0` | `UbxX20MonPatchData0` | `polled` |
| `0x0A / 0x35` | `UBX-MON-PMP` | `DATA0` | `UbxX20MonPmpData0` | `periodic` |
| `0x0A / 0x3B` | `UBX-MON-POST` | `POLL0` | `UbxX20MonPostPoll0` | `other` |
| `0x0A / 0x3B` | `UBX-MON-POST` | `DATA0` | `UbxX20MonPostData0` | `polled` |
| `0x0A / 0x38` | `UBX-MON-RF` | `POLL0` | `UbxX20MonRfPoll0` | `other` |
| `0x0A / 0x38` | `UBX-MON-RF` | `DATA0` | `UbxX20MonRfData0` | `periodic_polled` |
| `0x0A / 0x21` | `UBX-MON-RXR` | `VER0` | `UbxX20MonRxrVer0` | `output` |
| `0x0A / 0x31` | `UBX-MON-SPAN` | `POLL0` | `UbxX20MonSpanPoll0` | `other` |
| `0x0A / 0x31` | `UBX-MON-SPAN` | `DATA0` | `UbxX20MonSpanData0` | `periodic_polled` |
| `0x0A / 0x39` | `UBX-MON-SYS` | `POLL0` | `UbxX20MonSysPoll0` | `other` |
| `0x0A / 0x39` | `UBX-MON-SYS` | `DATA0` | `UbxX20MonSysData0` | `periodic_polled` |
| `0x0A / 0x04` | `UBX-MON-VER` | `POLL0` | `UbxX20MonVerPoll0` | `pollrequest` |
| `0x0A / 0x04` | `UBX-MON-VER` | `DATA0` | `UbxX20MonVerData0` | `polled` |
| `0x01 / 0x22` | `UBX-NAV-CLOCK` | `DATA0` | `UbxX20NavClockData0` | `periodic_polled` |
| `0x01 / 0x36` | `UBX-NAV-COV` | `POLL0` | `UbxX20NavCovPoll0` | `other` |
| `0x01 / 0x36` | `UBX-NAV-COV` | `DATA0` | `UbxX20NavCovData0` | `periodic_polled` |
| `0x01 / 0x04` | `UBX-NAV-DOP` | `DATA0` | `UbxX20NavDopData0` | `periodic_polled` |
| `0x01 / 0x61` | `UBX-NAV-EOE` | `DATA0` | `UbxX20NavEoeData0` | `periodic` |
| `0x01 / 0x39` | `UBX-NAV-GEOFENCE` | `DATA0` | `UbxX20NavGeofenceData0` | `periodic_polled` |
| `0x01 / 0x13` | `UBX-NAV-HPPOSECEF` | `DATA0` | `UbxX20NavHpposecefData0` | `periodic_polled` |
| `0x01 / 0x14` | `UBX-NAV-HPPOSLLH` | `DATA0` | `UbxX20NavHpposllhData0` | `periodic_polled` |
| `0x01 / 0x34` | `UBX-NAV-ORB` | `DATA0` | `UbxX20NavOrbData0` | `periodic_polled` |
| `0x01 / 0x01` | `UBX-NAV-POSECEF` | `DATA0` | `UbxX20NavPosecefData0` | `periodic_polled` |
| `0x01 / 0x02` | `UBX-NAV-POSLLH` | `DATA0` | `UbxX20NavPosllhData0` | `periodic_polled` |
| `0x01 / 0x07` | `UBX-NAV-PVT` | `DATA1` | `UbxX20NavPvtData1` | `periodic_polled` |
| `0x01 / 0x3C` | `UBX-NAV-RELPOSNED` | `POLL0` | `UbxX20NavRelposnedPoll0` | `other` |
| `0x01 / 0x3C` | `UBX-NAV-RELPOSNED` | `DATA1` | `UbxX20NavRelposnedData1` | `periodic_polled` |
| `0x01 / 0x35` | `UBX-NAV-SAT` | `DATA0` | `UbxX20NavSatData0` | `periodic_polled` |
| `0x01 / 0x32` | `UBX-NAV-SBAS` | `DATA0` | `UbxX20NavSbasData0` | `periodic_polled` |
| `0x01 / 0x43` | `UBX-NAV-SIG` | `DATA0` | `UbxX20NavSigData0` | `periodic_polled` |
| `0x01 / 0x42` | `UBX-NAV-SLAS` | `DATA0` | `UbxX20NavSlasData0` | `periodic_polled` |
| `0x01 / 0x03` | `UBX-NAV-STATUS` | `DATA1` | `UbxX20NavStatusData1` | `periodic_polled` |
| `0x01 / 0x3B` | `UBX-NAV-SVIN` | `POLL0` | `UbxX20NavSvinPoll0` | `other` |
| `0x01 / 0x3B` | `UBX-NAV-SVIN` | `DATA0` | `UbxX20NavSvinData0` | `periodic_polled` |
| `0x01 / 0x24` | `UBX-NAV-TIMEBDS` | `DATA0` | `UbxX20NavTimebdsData0` | `periodic_polled` |
| `0x01 / 0x25` | `UBX-NAV-TIMEGAL` | `POLL0` | `UbxX20NavTimegalPoll0` | `other` |
| `0x01 / 0x25` | `UBX-NAV-TIMEGAL` | `DATA0` | `UbxX20NavTimegalData0` | `periodic_polled` |
| `0x01 / 0x23` | `UBX-NAV-TIMEGLO` | `POLL0` | `UbxX20NavTimegloPoll0` | `other` |
| `0x01 / 0x23` | `UBX-NAV-TIMEGLO` | `DATA0` | `UbxX20NavTimegloData0` | `periodic_polled` |
| `0x01 / 0x20` | `UBX-NAV-TIMEGPS` | `DATA0` | `UbxX20NavTimegpsData0` | `periodic_polled` |
| `0x01 / 0x26` | `UBX-NAV-TIMELS` | `DATA0` | `UbxX20NavTimelsData0` | `periodic_polled` |
| `0x01 / 0x63` | `UBX-NAV-TIMENAVIC` | `POLL0` | `UbxX20NavTimenavicPoll0` | `other` |
| `0x01 / 0x63` | `UBX-NAV-TIMENAVIC` | `DATA0` | `UbxX20NavTimenavicData0` | `periodic_polled` |
| `0x01 / 0x27` | `UBX-NAV-TIMEQZSS` | `POLL0` | `UbxX20NavTimeqzssPoll0` | `other` |
| `0x01 / 0x27` | `UBX-NAV-TIMEQZSS` | `DATA0` | `UbxX20NavTimeqzssData0` | `periodic_polled` |
| `0x01 / 0x64` | `UBX-NAV-TIMETRUSTED` | `DATA0` | `UbxX20NavTimetrustedData0` | `periodic_polled` |
| `0x01 / 0x21` | `UBX-NAV-TIMEUTC` | `DATA0` | `UbxX20NavTimeutcData0` | `periodic_polled` |
| `0x01 / 0x11` | `UBX-NAV-VELECEF` | `DATA0` | `UbxX20NavVelecefData0` | `periodic_polled` |
| `0x01 / 0x12` | `UBX-NAV-VELNED` | `DATA0` | `UbxX20NavVelnedData0` | `periodic_polled` |
| `0x29 / 0x22` | `UBX-NAV2-CLOCK` | `DATA0` | `UbxX20Nav2ClockData0` | `periodic_polled` |
| `0x29 / 0x36` | `UBX-NAV2-COV` | `POLL0` | `UbxX20Nav2CovPoll0` | `other` |
| `0x29 / 0x36` | `UBX-NAV2-COV` | `DATA0` | `UbxX20Nav2CovData0` | `periodic_polled` |
| `0x29 / 0x04` | `UBX-NAV2-DOP` | `DATA0` | `UbxX20Nav2DopData0` | `periodic_polled` |
| `0x29 / 0x61` | `UBX-NAV2-EOE` | `DATA0` | `UbxX20Nav2EoeData0` | `periodic` |
| `0x29 / 0x01` | `UBX-NAV2-POSECEF` | `DATA0` | `UbxX20Nav2PosecefData0` | `periodic_polled` |
| `0x29 / 0x02` | `UBX-NAV2-POSLLH` | `DATA0` | `UbxX20Nav2PosllhData0` | `periodic_polled` |
| `0x29 / 0x07` | `UBX-NAV2-PVT` | `DATA1` | `UbxX20Nav2PvtData1` | `periodic_polled` |
| `0x29 / 0x35` | `UBX-NAV2-SAT` | `DATA0` | `UbxX20Nav2SatData0` | `periodic_polled` |
| `0x29 / 0x32` | `UBX-NAV2-SBAS` | `DATA0` | `UbxX20Nav2SbasData0` | `periodic_polled` |
| `0x29 / 0x43` | `UBX-NAV2-SIG` | `DATA0` | `UbxX20Nav2SigData0` | `periodic_polled` |
| `0x29 / 0x42` | `UBX-NAV2-SLAS` | `DATA0` | `UbxX20Nav2SlasData0` | `periodic_polled` |
| `0x29 / 0x03` | `UBX-NAV2-STATUS` | `DATA1` | `UbxX20Nav2StatusData1` | `periodic_polled` |
| `0x29 / 0x3B` | `UBX-NAV2-SVIN` | `POLL0` | `UbxX20Nav2SvinPoll0` | `other` |
| `0x29 / 0x3B` | `UBX-NAV2-SVIN` | `DATA0` | `UbxX20Nav2SvinData0` | `periodic_polled` |
| `0x29 / 0x24` | `UBX-NAV2-TIMEBDS` | `DATA0` | `UbxX20Nav2TimebdsData0` | `periodic_polled` |
| `0x29 / 0x25` | `UBX-NAV2-TIMEGAL` | `POLL0` | `UbxX20Nav2TimegalPoll0` | `other` |
| `0x29 / 0x25` | `UBX-NAV2-TIMEGAL` | `DATA0` | `UbxX20Nav2TimegalData0` | `periodic_polled` |
| `0x29 / 0x23` | `UBX-NAV2-TIMEGLO` | `POLL0` | `UbxX20Nav2TimegloPoll0` | `other` |
| `0x29 / 0x23` | `UBX-NAV2-TIMEGLO` | `DATA0` | `UbxX20Nav2TimegloData0` | `periodic_polled` |
| `0x29 / 0x20` | `UBX-NAV2-TIMEGPS` | `DATA0` | `UbxX20Nav2TimegpsData0` | `periodic_polled` |
| `0x29 / 0x26` | `UBX-NAV2-TIMELS` | `POLL0` | `UbxX20Nav2TimelsPoll0` | `other` |
| `0x29 / 0x26` | `UBX-NAV2-TIMELS` | `DATA0` | `UbxX20Nav2TimelsData0` | `periodic_polled` |
| `0x29 / 0x63` | `UBX-NAV2-TIMENAVIC` | `POLL0` | `UbxX20Nav2TimenavicPoll0` | `other` |
| `0x29 / 0x63` | `UBX-NAV2-TIMENAVIC` | `DATA0` | `UbxX20Nav2TimenavicData0` | `periodic_polled` |
| `0x29 / 0x27` | `UBX-NAV2-TIMEQZSS` | `POLL0` | `UbxX20Nav2TimeqzssPoll0` | `other` |
| `0x29 / 0x27` | `UBX-NAV2-TIMEQZSS` | `DATA0` | `UbxX20Nav2TimeqzssData0` | `periodic_polled` |
| `0x29 / 0x21` | `UBX-NAV2-TIMEUTC` | `DATA0` | `UbxX20Nav2TimeutcData0` | `periodic_polled` |
| `0x29 / 0x11` | `UBX-NAV2-VELECEF` | `DATA0` | `UbxX20Nav2VelecefData0` | `periodic_polled` |
| `0x29 / 0x12` | `UBX-NAV2-VELNED` | `DATA0` | `UbxX20Nav2VelnedData0` | `periodic_polled` |
| `0x02 / 0x34` | `UBX-RXM-COR` | `DATA0` | `UbxX20RxmCorData0` | `output` |
| `0x02 / 0x14` | `UBX-RXM-MEASX` | `POLL0` | `UbxX20RxmMeasxPoll0` | `other` |
| `0x02 / 0x14` | `UBX-RXM-MEASX` | `DATA1` | `UbxX20RxmMeasxData1` | `periodic_polled` |
| `0x02 / 0x72` | `UBX-RXM-PMP` | `DATA2` | `UbxX20RxmPmpData2` | `input_output` |
| `0x02 / 0x41` | `UBX-RXM-PMREQ` | `0` | `UbxX20RxmPmreq0` | `command` |
| `0x02 / 0x41` | `UBX-RXM-PMREQ` | `1` | `UbxX20RxmPmreq1` | `command` |
| `0x02 / 0x73` | `UBX-RXM-QZSSL6` | `DATA2` | `UbxX20RxmQzssl6Data2` | `input` |
| `0x02 / 0x15` | `UBX-RXM-RAWX` | `DATA1` | `UbxX20RxmRawxData1` | `periodic_polled` |
| `0x02 / 0x59` | `UBX-RXM-RLM` | `SHORT` | `UbxX20RxmRlmShort` | `output` |
| `0x02 / 0x59` | `UBX-RXM-RLM` | `LONG` | `UbxX20RxmRlmLong` | `output` |
| `0x02 / 0x13` | `UBX-RXM-SFRBX` | `POLL0` | `UbxX20RxmSfrbxPoll0` | `other` |
| `0x02 / 0x13` | `UBX-RXM-SFRBX` | `DATA2` | `UbxX20RxmSfrbxData2` | `output` |
| `0x02 / 0x36` | `UBX-RXM-SPARTNKEY` | `POLL0` | `UbxX20RxmSpartnkeyPoll0` | `pollrequest` |
| `0x02 / 0x36` | `UBX-RXM-SPARTNKEY` | `DATA0` | `UbxX20RxmSpartnkeyData0` | `input_output` |
| `0x27 / 0x0A` | `UBX-SEC-OSNMA` | `DATA2` | `UbxX20SecOsnmaData2` | `periodic_polled` |
| `0x27 / 0x09` | `UBX-SEC-SIG` | `POLL0` | `UbxX20SecSigPoll0` | `other` |
| `0x27 / 0x09` | `UBX-SEC-SIG` | `DATA1` | `UbxX20SecSigData1` | `periodic_polled` |
| `0x27 / 0x09` | `UBX-SEC-SIG` | `DATA2` | `UbxX20SecSigData2` | `periodic_polled` |
| `0x27 / 0x10` | `UBX-SEC-SIGLOG` | `POLL0` | `UbxX20SecSiglogPoll0` | `other` |
| `0x27 / 0x10` | `UBX-SEC-SIGLOG` | `DATA1` | `UbxX20SecSiglogData1` | `periodic_polled` |
| `0x27 / 0x03` | `UBX-SEC-UNIQID` | `POLL0` | `UbxX20SecUniqidPoll0` | `other` |
| `0x27 / 0x03` | `UBX-SEC-UNIQID` | `DATA1` | `UbxX20SecUniqidData1` | `output` |
| `0x0D / 0x03` | `UBX-TIM-TM2` | `DATA1` | `UbxX20TimTm2Data1` | `periodic_polled` |
| `0x0D / 0x01` | `UBX-TIM-TP` | `DATA0` | `UbxX20TimTpData0` | `periodic_polled` |
| `0x0D / 0x06` | `UBX-TIM-VRFY` | `DATA1` | `UbxX20TimVrfyData1` | `periodic_polled` |
| `0x09 / 0x14` | `UBX-UPD-SOS` | `POLL0` | `UbxX20UpdSosPoll0` | `pollrequest` |
| `0x09 / 0x14` | `UBX-UPD-SOS` | `BACKUP` | `UbxX20UpdSosBackup` | `command` |
| `0x09 / 0x14` | `UBX-UPD-SOS` | `CLEAR` | `UbxX20UpdSosClear` | `command` |
| `0x09 / 0x14` | `UBX-UPD-SOS` | `ACK` | `UbxX20UpdSosAck` | `output` |
| `0x09 / 0x14` | `UBX-UPD-SOS` | `RESTORED` | `UbxX20UpdSosRestored` | `output` |
