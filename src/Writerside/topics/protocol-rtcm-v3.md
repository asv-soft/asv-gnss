# RTCM v3

Binary GNSS corrections, reference-station information, ephemerides, and multiple-signal observations.

## Connect the parser

```csharp
using Asv.Gnss;
using Asv.IO;

var protocol = Protocol.Create(builder =>
{
    builder.Protocols.RegisterRtcmV3Protocol();
});
```

The stream parser is `RtcmV3MessageParser` and its message model is `RtcmV3MessageBase`.
See [Quick Start](quick-start.md) for router, port, and subscription setup. Registration selects decoders; the receiver must already be configured to emit the required data.

## Default message coverage

Defaults include station and antenna messages, GPS/GLONASS legacy observations, ephemerides, GLONASS code-phase biases, and MSM3/4/5/7 variants. The table below is the exact registered set; MSM support differs by constellation and subtype.

| Message model |
|---------------|
| `RtcmV3Message1004` |
| `RtcmV3Message1005` |
| `RtcmV3Message1006` |
| `RtcmV3Message1007` |
| `RtcmV3Message1008` |
| `RtcmV3Message1012` |
| `RtcmV3Message1013` |
| `RtcmV3Message1019` |
| `RtcmV3Message1020` |
| `RtcmV3Message1029` |
| `RtcmV3Message1030` |
| `RtcmV3Message1031` |
| `RtcmV3Message1032` |
| `RtcmV3Message1033` |
| `RtcmV3Message1042` |
| `RtcmV3Message1046` |
| `RtcmV3Message1230` |
| `RtcmV3Msm3Msg1073` |
| `RtcmV3Msm3Msg1083` |
| `RtcmV3Msm3Msg1093` |
| `RtcmV3Msm3Msg1123` |
| `RtcmV3Msm4Msg1074` |
| `RtcmV3Msm4Msg1084` |
| `RtcmV3Msm4Msg1094` |
| `RtcmV3Msm4Msg1104` |
| `RtcmV3Msm4Msg1114` |
| `RtcmV3Msm4Msg1124` |
| `RtcmV3Msm5Msg1075` |
| `RtcmV3Msm5Msg1085` |
| `RtcmV3Msm5Msg1095` |
| `RtcmV3Msm5Msg1125` |
| `RtcmV3Msm7Msg1077` |
| `RtcmV3Msm7Msg1087` |
| `RtcmV3Msm7Msg1097` |
| `RtcmV3Msm7Msg1107` |
| `RtcmV3Msm7Msg1117` |
| `RtcmV3Msm7Msg1127` |
| `RtcmV3Msg4094` |

## Usage and limitations

Registration does not imply that every model can be transmitted. Types derived from `RtcmV3LegacyMessageBase` do not implement payload serialization or size calculation. Check the concrete message before encoding. The standalone `RtcmV3MessageFactory` has a smaller set than `RegisterRtcmV3Protocol`; use the registration method for the defaults below.

## Implementation reference

Coverage above follows `Protocols/Rtcm/V3/RtcmV3Protocol.cs` under `src/Asv.Gnss`. Inspect concrete message classes for field names, units, and supported payload revisions.

## Specification message catalog

Source: `Protocols/Rtcm/V3/Docs/c10403.3 - DGNSS - with Amendment1 (2).pdf`, RTCM 10403.3 with Amendment 1, Table 3.2-1, PDF pp. 35-41. This is the newest local message summary and supersedes the older v3.1/v3.2 lists for catalog purposes. Explicit reserved and experimental ranges are retained. Proprietary assignments identify vendors, not standardized payloads.

**Status:** **Model present** means a concrete payload model exists in the current checkout; it does not certify complete field, revision, encoding, or firmware coverage. **Partial** identifies a known incomplete decoder. **Not implemented** means no matching concrete model was found. **Default** reports whether the standard registration adds that model. Models outside the default set require explicit registration. See the limitations above before transmitting messages.

Page references below are **PDF page numbers (1-based)**, including covers and front matter.

| Type | Message | Status | C# model | Default | PDF page |
| --- | --- | --- | --- | --- | --- |
| 1-100 | Experimental messages | Not implemented | - | No | 35 |
| 1001 | L1-Only GPS RTK Observables | Not implemented | - | No | 35 |
| 1002 | Extended L1-Only GPS RTK Observables | Not implemented | - | No | 35 |
| 1003 | L1&L2 GPS RTK Observables | Not implemented | - | No | 35 |
| 1004 | Extended L1&L2 GPS RTK Observables | Model present | `RtcmV3Message1004` | Yes | 35 |
| 1005 | Stationary RTK Reference Station ARP | Model present | `RtcmV3Message1005` | Yes | 35 |
| 1006 | Stationary RTK Reference Station ARP with Antenna Height | Model present | `RtcmV3Message1006` | Yes | 35 |
| 1007 | Antenna Descriptor | Model present | `RtcmV3Message1007` | Yes | 35 |
| 1008 | Antenna Descriptor & Serial Number | Model present | `RtcmV3Message1008` | Yes | 35 |
| 1009 | L1-Only GLONASS RTK Observables | Not implemented | - | No | 35 |
| 1010 | Extended L1-Only GLONASS RTK Observables | Not implemented | - | No | 35 |
| 1011 | L1&L2 GLONASS RTK Observables | Not implemented | - | No | 35 |
| 1012 | Extended L1&L2 GLONASS RTK Observables | Model present | `RtcmV3Message1012` | Yes | 35 |
| 1013 | System Parameters | Model present | `RtcmV3Message1013` | Yes | 35 |
| 1014 | Network Auxiliary Station Data | Not implemented | - | No | 35 |
| 1015 | GPS Ionospheric Correction Differences | Not implemented | - | No | 36 |
| 1016 | GPS Geometric Correction Differences | Not implemented | - | No | 36 |
| 1017 | GPS Combined Geometric and Ionospheric Correction Differences | Not implemented | - | No | 36 |
| 1018 | Reserved: alternative ionospheric corrections | Reserved | - | No | 36 |
| 1019 | GPS Ephemerides | Model present | `RtcmV3Message1019` | Yes | 36 |
| 1020 | GLONASS Ephemerides | Model present | `RtcmV3Message1020` | Yes | 36 |
| 1021 | Helmert / Abridged Molodenski Transformation Parameters | Not implemented | - | No | 36 |
| 1022 | Molodenski-Badekas Transformation | Not implemented | - | No | 36 |
| 1023 | Residuals, Ellipsoidal Grid Representation | Not implemented | - | No | 36 |
| 1024 | Residuals, Plane Grid Representation | Not implemented | - | No | 36 |
| 1025 | Projection Parameters, Projection Types other than Lambert Conic Conformal (2 SP) and Oblique Mercator | Not implemented | - | No | 36 |
| 1026 | Projection Parameters, Projection Type LCC2SP (Lambert Conic Conformal (2 SP)) | Not implemented | - | No | 36 |
| 1027 | Projection Parameters, Projection Type OM (Oblique Mercator) | Not implemented | - | No | 36 |
| 1028 | Reserved: global to plate-fixed transformation | Reserved | - | No | 36 |
| 1029 | Unicode Text String | Model present | `RtcmV3Message1029` | Yes | 37 |
| 1030 | GPS Network RTK Residual Message Message | Model present | `RtcmV3Message1030` | Yes | 37 |
| 1032 | Physical Reference Station Position Message | Model present | `RtcmV3Message1032` | Yes | 37 |
| 1033 | Receiver and Antenna Descriptors | Model present | `RtcmV3Message1033` | Yes | 37 |
| 1034 | GPS Network FKP Gradient | Not implemented | - | No | 37 |
| 1035 | GLONASS Network FKP Gradient | Not implemented | - | No | 37 |
| 1037 | GLONASS Ionospheric Correction | Not implemented | - | No | 37 |
| 1038 | GLONASS Geometric Correction | Not implemented | - | No | 37 |
| 1039 | GLONASS Combined Geometric and Ionospheric Correction Differences | Not implemented | - | No | 37 |
| 1041 | NavIC/IRNSS Satellite Ephemeris | Not implemented | - | No | 37 |
| 1042 | BDS Satellite Ephemeris Data | Model present | `RtcmV3Message1042` | Yes | 37 |
| 1044 | QZSS Ephemerides | Not implemented | - | No | 37 |
| 1045 | Galileo F/NAV Satellite Ephemeris Data | Not implemented | - | No | 37 |
| 1046 | Galileo I/NAV Satellite Ephemeris Data | Model present | `RtcmV3Message1046` | Yes | 38 |
| 1057 | SSR GPS Orbit Correction | Not implemented | - | No | 38 |
| 1058 | SSR GPS Clock Correction | Not implemented | - | No | 38 |
| 1059 | SSR GPS Code Bias | Not implemented | - | No | 38 |
| 1060 | SSR GPS Combined Orbit and Clock Corrections | Not implemented | - | No | 38 |
| 1061 | SSR GPS URA | Not implemented | - | No | 38 |
| 1062 | SSR GPS High Rate Clock Correction | Not implemented | - | No | 38 |
| 1063 | SSR GLONASS Orbit Correction | Not implemented | - | No | 38 |
| 1064 | SSR GLONASS Clock Correction | Not implemented | - | No | 38 |
| 1065 | SSR GLONASS Code Bias | Not implemented | - | No | 38 |
| 1066 | SSR GLONASS Combined Orbit and Clock Corrections | Not implemented | - | No | 38 |
| 1067 | SSR GLONASS URA | Not implemented | - | No | 38 |
| 1068 | SSR GLONASS High Rate Clock Correction | Not implemented | - | No | 38 |
| 1070 | Reserved MSM | Reserved | - | No | 38 |
| 1071 | GPS MSM1 | Not implemented | - | No | 38 |
| 1072 | GPS MSM2 | Not implemented | - | No | 38 |
| 1073 | GPS MSM3 | Model present | `RtcmV3Msm3Msg1073` | Yes | 38 |
| 1074 | GPS MSM4 | Model present | `RtcmV3Msm4Msg1074` | Yes | 39 |
| 1075 | GPS MSM5 | Model present | `RtcmV3Msm5Msg1075` | Yes | 39 |
| 1076 | GPS MSM6 | Not implemented | - | No | 39 |
| 1077 | GPS MSM7 | Model present | `RtcmV3Msm7Msg1077` | Yes | 39 |
| 1078 | Reserved MSM | Reserved | - | No | 39 |
| 1079 | Reserved MSM | Reserved | - | No | 39 |
| 1080 | Reserved MSM | Reserved | - | No | 39 |
| 1081 | GLONASS MSM1 | Not implemented | - | No | 39 |
| 1082 | GLONASS MSM2 | Not implemented | - | No | 39 |
| 1083 | GLONASS MSM3 | Model present | `RtcmV3Msm3Msg1083` | Yes | 39 |
| 1084 | GLONASS MSM4 | Model present | `RtcmV3Msm4Msg1084` | Yes | 39 |
| 1085 | GLONASS MSM5 | Model present | `RtcmV3Msm5Msg1085` | Yes | 39 |
| 1086 | GLONASS MSM6 | Not implemented | - | No | 39 |
| 1087 | GLONASS MSM7 | Model present | `RtcmV3Msm7Msg1087` | Yes | 39 |
| 1088 | Reserved MSM | Reserved | - | No | 39 |
| 1089 | Reserved MSM | Reserved | - | No | 39 |
| 1090 | Reserved MSM | Reserved | - | No | 39 |
| 1091 | Galileo MSM1 | Not implemented | - | No | 39 |
| 1092 | Galileo MSM2 | Not implemented | - | No | 39 |
| 1093 | Galileo MSM3 | Model present | `RtcmV3Msm3Msg1093` | Yes | 39 |
| 1094 | Galileo MSM4 | Model present | `RtcmV3Msm4Msg1094` | Yes | 40 |
| 1095 | Galileo MSM5 | Model present | `RtcmV3Msm5Msg1095` | Yes | 40 |
| 1096 | Galileo MSM6 | Not implemented | - | No | 40 |
| 1097 | Galileo MSM7 | Model present | `RtcmV3Msm7Msg1097` | Yes | 40 |
| 1098 | Reserved MSM | Reserved | - | No | 40 |
| 1099 | Reserved MSM | Reserved | - | No | 40 |
| 1100 | Reserved MSM | Reserved | - | No | 40 |
| 1101 | SBAS MSM1 | Not implemented | - | No | 40 |
| 1102 | SBAS MSM2 | Not implemented | - | No | 40 |
| 1103 | SBAS MSM3 | Not implemented | - | No | 40 |
| 1104 | SBAS MSM4 | Model present | `RtcmV3Msm4Msg1104` | Yes | 40 |
| 1105 | SBAS MSM5 | Not implemented | - | No | 40 |
| 1106 | SBAS MSM6 | Not implemented | - | No | 40 |
| 1107 | SBAS MSM7 | Model present | `RtcmV3Msm7Msg1107` | Yes | 40 |
| 1108 | Reserved MSM | Reserved | - | No | 40 |
| 1109 | Reserved MSM | Reserved | - | No | 40 |
| 1110 | Reserved MSM | Reserved | - | No | 40 |
| 1111 | QZSS MSM1 | Not implemented | - | No | 40 |
| 1112 | QZSS MSM2 | Not implemented | - | No | 40 |
| 1113 | QZSS MSM3 | Not implemented | - | No | 40 |
| 1114 | QZSS MSM4 | Model present | `RtcmV3Msm4Msg1114` | Yes | 41 |
| 1115 | QZSS MSM5 | Not implemented | - | No | 41 |
| 1116 | QZSS MSM6 | Not implemented | - | No | 41 |
| 1117 | QZSS MSM7 | Model present | `RtcmV3Msm7Msg1117` | Yes | 41 |
| 1121 | BeiDou MSM1 | Not implemented | - | No | 41 |
| 1122 | BeiDou MSM2 | Not implemented | - | No | 41 |
| 1123 | BeiDou MSM3 | Model present | `RtcmV3Msm3Msg1123` | Yes | 41 |
| 1124 | BeiDou MSM4 | Model present | `RtcmV3Msm4Msg1124` | Yes | 41 |
| 1125 | BeiDou MSM5 | Model present | `RtcmV3Msm5Msg1125` | Yes | 41 |
| 1126 | BeiDou MSM6 | Not implemented | - | No | 41 |
| 1127 | BeiDou MSM7 | Model present | `RtcmV3Msm7Msg1127` | Yes | 41 |
| 1230 | GLONASS L1 and L2 Code-Phase Biases | Model present | `RtcmV3Message1230` | Yes | 41 |
| 1118-1120 | Reserved MSM | Reserved | - | No | 41 |
| 1128-1229 | Reserved MSM range (including NavIC/IRNSS reservations) | Reserved | - | No | 41 |
| 4001-4095 | Proprietary allocation range; not a common payload definition | Vendor-specific | - | No | 41 |

### Additional implementation entries

These models exist in source but are not individually defined by the standard message summary. Their presence does not establish vendor interoperability.

| Type | C# model | Default |
| --- | --- | --- |
| 1031 | `RtcmV3Message1031` | Yes |
| 4094 | `RtcmV3Msg4094` | Yes |


[All protocols](supported-protocols.md)
