# Septentrio Binary Format (SBF)

Septentrio receiver binary blocks for PVT, quality, status, observations, and raw navigation data.

## Connect the parser

```c#
using Asv.Gnss;
using Asv.IO;

var protocol = Protocol.Create(builder =>
{
    builder.Protocols.RegisterSbfProtocol();
});
```

The stream parser is `SbfBinaryParser` and its message model is `SbfMessageBase`.
See [Quick Start](quick-start.md) for router, port, and subscription setup. Registration selects decoders; the receiver must already be configured to emit the required data.

## Default message coverage

The default set includes GPS and GLONASS raw C/A blocks, MeasEpoch revision 1, PVTGeodetic revision 2, DOP, ReceiverStatus revision 1, QualityInd, and GPSNav. Block revision is part of the model contract.

| Message model |
|---------------|
| `SbfPacketGpsRawCa` |
| `SbfPacketGloRawCa` |
| `SbfPacketMeasEpochRev1` |
| `SbfPacketPvtGeodeticRev2` |
| `SbfPacketDOP` |
| `SbfPacketReceiverStatusRev1` |
| `SbfPacketQualityInd` |
| `SbfPacketGpsNav` |

## Usage and limitations

This is a decoding interface: `SbfMessageBase.Serialize` and `GetByteSize` throw `NotImplementedException`. `SbfPacketMeasEpochRev1` is explicitly marked incomplete in the factory. Do not infer support for other block revisions from a matching block name.

## Implementation reference

Coverage above follows `Parsers/SBF/SbfMessageFactory.cs` under `src/Asv.Gnss`. Inspect concrete message classes for field names, units, and supported payload revisions.

## Specification message catalog

Sources: `Parsers/SBF/Docs/SBF Reference Guide.pdf`, v1.11.0, block catalog on PDF pp. 9-10; `Parsers/SBF/Docs/asterx-u_firmware_v4.6.1_reference_guide (1).pdf`, firmware v4.6.1, Appendix B on PDF pp. 440-443. The table is the union by numeric block number; different block numbers remain separate even when their names match. The older guide's block-version column is not the payload revision encoded by the C# model. The model column gives the implemented payload revision; other revisions are not implied.

**Status:** **Model present** means a concrete payload model exists in the current checkout; it does not certify complete field, revision, encoding, or firmware coverage. **Partial** identifies a known incomplete decoder. **Not implemented** means no matching concrete model was found. **Default** reports whether the standard registration adds that model. Models outside the default set require explicit registration. See the limitations above before transmitting messages.

Page references below are **PDF page numbers (1-based)**, including covers and front matter.

| Block ID | Block name | Status | C# model / payload revision | Default | PDF reference |
| --- | --- | --- | --- | --- | --- |
| 4000 | MeasExtra | Not implemented | - | No | Guide p. 9; AsteRx-U p. 440 |
| 4001 | DOP | Model present | `SbfPacketDOP`; revision 0 | Yes | Guide p. 10; AsteRx-U p. 442 |
| 4002 | GALNav | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 4003 | GALAlm | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 4004 | GLONav | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 4005 | GLOAlm | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 4006 | PVTCartesian | Not implemented | - | No | Guide p. 10; AsteRx-U p. 441 |
| 4007 | PVTGeodetic | Model present | `SbfPacketPvtGeodeticRev2`; revision 2 | Yes | Guide p. 10; AsteRx-U p. 442 |
| 4008 | PVTSatCartesian | Not implemented | - | No | Guide p. 10 |
| 4009 | PVTResiduals | Not implemented | - | No | Guide p. 10 |
| 4011 | RAIMStatistics | Not implemented | - | No | Guide p. 10 |
| 4012 | SatVisibility | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 4013 | ChannelStatus | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 4014 | ReceiverStatus | Model present | `SbfPacketReceiverStatusRev1`; revision 1 | Yes | Guide p. 10; AsteRx-U p. 442 |
| 4015 | Commands | Not implemented | - | No | Guide p. 10; AsteRx-U p. 443 |
| 4017 | GPSRawCA | Model present | `SbfPacketGpsRawCa`; revision 0 | Yes | Guide p. 9; AsteRx-U p. 440 |
| 4018 | GPSRawL2C | Model present | `SbfPacketGpsRawL2C`; revision 0 | No | Guide p. 9; AsteRx-U p. 440 |
| 4019 | GPSRawL5 | Model present | `SbfPacketGpsRawL5`; revision 0 | No | Guide p. 9; AsteRx-U p. 440 |
| 4020 | GEORawL1 | Model present | `SbfPacketGeoRawL1`; revision 0 | No | Guide p. 9; AsteRx-U p. 440 |
| 4021 | GEORawL5 | Model present | `SbfPacketGeoRawL5`; revision 0 | No | AsteRx-U p. 440 |
| 4022 | GALRawFNAV | Model present | `SbfPacketGalRawFnav`; revision 0 | No | Guide p. 9; AsteRx-U p. 440 |
| 4023 | GALRawINAV | Model present | `SbfPacketGalRawInav`; revision 0 | No | Guide p. 9; AsteRx-U p. 440 |
| 4024 | GALRawCNAV | Model present | `SbfPacketGalRawCnav`; revision 0 | No | Guide p. 9; AsteRx-U p. 440 |
| 4026 | GLORawCA | Model present | `SbfPacketGloRawCa`; revision 0 | Yes | Guide p. 9; AsteRx-U p. 440 |
| 4027 | MeasEpoch | Partial | `SbfPacketMeasEpochRev1`; revision 1 | Yes | Guide p. 9; AsteRx-U p. 440 |
| 4028 | BaseVectorGeod | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 4030 | GALIon | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 4031 | GALUtc | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 4032 | GALGstGps | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 4034 | GALSARRLM | Not implemented | - | No | AsteRx-U p. 441 |
| 4036 | GLOTime | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 4037 | ExtEventPVTCartesian | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 4038 | ExtEventPVTGeodetic | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 4040 | BBSamples | Not implemented | - | No | Guide p. 10; AsteRx-U p. 443 |
| 4043 | BaseVectorCart | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 4044 | PosCart | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 4045 | IntPVAAGeod | Not implemented | - | No | Guide p. 10 |
| 4046 | IQCorr | Not implemented | - | No | Guide p. 9 |
| 4047 | CMPRaw | Model present | `SbfPacketBdsRaw`; revision 0 | No | Guide p. 9; AsteRx-U p. 440 |
| 4049 | RTCMDatum | Not implemented | - | No | AsteRx-U p. 442 |
| 4050 | ExtSensorMeas | Not implemented | - | No | Guide p. 10 |
| 4051 | BluetoothStatus | Not implemented | - | No | AsteRx-U p. 443 |
| 4052 | PosLocal | Not implemented | - | No | AsteRx-U p. 442 |
| 4053 | NTRIPClientStatus | Not implemented | - | No | AsteRx-U p. 442 |
| 4054 | WiFiAPStatus | Not implemented | - | No | AsteRx-U p. 443 |
| 4055 | CellularStatus | Not implemented | - | No | AsteRx-U p. 443 |
| 4056 | ExtSensorStatus | Not implemented | - | No | Guide p. 10 |
| 4057 | ExtSensorSetup | Not implemented | - | No | Guide p. 10 |
| 4058 | IPStatus | Not implemented | - | No | AsteRx-U p. 442 |
| 4059 | DiskStatus | Not implemented | - | No | AsteRx-U p. 443 |
| 4060 | IntPVCart | Not implemented | - | No | Guide p. 10 |
| 4061 | IntPVGeod | Not implemented | - | No | Guide p. 10 |
| 4062 | IntPosCovCart | Not implemented | - | No | Guide p. 10 |
| 4063 | IntVelCovCart | Not implemented | - | No | Guide p. 10 |
| 4064 | IntPosCovGeod | Not implemented | - | No | Guide p. 10 |
| 4065 | IntVelCovGeod | Not implemented | - | No | Guide p. 10 |
| 4066 | QZSRawL1CA | Model present | `SbfPacketQzsRawL1Ca`; revision 0 | No | AsteRx-U p. 440 |
| 4067 | QZSRawL2C | Model present | `SbfPacketQzsRawL2C`; revision 0 | No | AsteRx-U p. 441 |
| 4068 | QZSRawL5 | Model present | `SbfPacketQzsRawL5`; revision 0 | No | AsteRx-U p. 441 |
| 4069 | QZSRawL6 | Model present | `SbfPacketQzsRawL6`; revision 0 | No | AsteRx-U p. 441 |
| 4070 | IntAttEuler | Not implemented | - | No | Guide p. 10 |
| 4072 | IntAttCovEuler | Not implemented | - | No | Guide p. 10 |
| 4075 | ASCIIIn | Not implemented | - | No | AsteRx-U p. 443 |
| 4076 | PVTSupport | Not implemented | - | No | AsteRx-U p. 442 |
| 4079 | PVTSupportA | Not implemented | - | No | AsteRx-U p. 442 |
| 4081 | BDSNav | Not implemented | - | No | AsteRx-U p. 441 |
| 4082 | QualityInd | Model present | `SbfPacketQualityInd`; revision 0 | Yes | AsteRx-U p. 443 |
| 4084 | RxComponents | Not implemented | - | No | AsteRx-U p. 443 |
| 4085 | UHFStatus | Not implemented | - | No | AsteRx-U p. 443 |
| 4090 | InputLink | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 4091 | OutputLink | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 4092 | RFStatus | Not implemented | - | No | AsteRx-U p. 443 |
| 4093 | NAVICRaw | Model present | `SbfPacketNavicRaw`; revision 0 | No | AsteRx-U p. 441 |
| 4094 | PosProjected | Not implemented | - | No | AsteRx-U p. 442 |
| 4095 | QZSNav | Not implemented | - | No | AsteRx-U p. 441 |
| 4096 | WiFiClientStatus | Not implemented | - | No | AsteRx-U p. 443 |
| 4097 | EncapsulatedOutput | Not implemented | - | No | AsteRx-U p. 443 |
| 4103 | RxMessage | Not implemented | - | No | AsteRx-U p. 443 |
| 4105 | DynDNSStatus | Not implemented | - | No | AsteRx-U p. 443 |
| 4109 | Meas3Ranges | Not implemented | - | No | AsteRx-U p. 440 |
| 4110 | Meas3CN0HiRes | Not implemented | - | No | AsteRx-U p. 440 |
| 4111 | Meas3Doppler | Not implemented | - | No | AsteRx-U p. 440 |
| 4112 | Meas3PP | Not implemented | - | No | AsteRx-U p. 440 |
| 4113 | Meas3MP | Not implemented | - | No | AsteRx-U p. 440 |
| 4116 | QZSAlm | Not implemented | - | No | AsteRx-U p. 441 |
| 4119 | BDSAlm | Not implemented | - | No | AsteRx-U p. 441 |
| 4120 | BDSIon | Not implemented | - | No | AsteRx-U p. 441 |
| 4121 | BDSUtc | Not implemented | - | No | AsteRx-U p. 441 |
| 4122 | NTRIPServerStatus | Not implemented | - | No | AsteRx-U p. 442 |
| 4201 | LBandTrackerStatus | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 4202 | LBAS1DecoderStatus | Not implemented | - | No | Guide p. 10 |
| 4203 | LBAS1Messages | Not implemented | - | No | Guide p. 10 |
| 4204 | LBandBeams | Not implemented | - | No | AsteRx-U p. 442 |
| 4217 | ExtEventBaseVectGeod | Not implemented | - | No | AsteRx-U p. 442 |
| 4218 | BDSRawB1C | Model present | `SbfPacketBdsRawB1C`; revision 0 | No | AsteRx-U p. 440 |
| 4219 | BDSRawB2a | Model present | `SbfPacketBdsRawB2A`; revision 0 | No | AsteRx-U p. 440 |
| 4237 | ExtEventAttEuler | Not implemented | - | No | AsteRx-U p. 442 |
| 4243 | CosmosStatus | Not implemented | - | No | AsteRx-U p. 443 |
| 5889 | MeasEpoch | Not implemented | - | No | Guide p. 9 |
| 5890 | ShortMeasEpoch | Not implemented | - | No | Guide p. 9 |
| 5891 | GPSNav | Model present | `SbfPacketGpsNav`; revision 1 | Yes | Guide p. 9; AsteRx-U p. 441 |
| 5892 | GPSAlm | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5893 | GPSIon | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5894 | GPSUtc | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5895 | GPSRaw | Not implemented | - | No | Guide p. 9 |
| 5896 | GEONav | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5897 | GEOAlm | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5898 | GEORaw | Not implemented | - | No | Guide p. 9 |
| 5902 | ReceiverSetup | Not implemented | - | No | Guide p. 10; AsteRx-U p. 443 |
| 5903 | PVTCartesian | Not implemented | - | No | Guide p. 9 |
| 5904 | PVTGeodetic | Not implemented | - | No | Guide p. 9 |
| 5905 | PosCovCartesian | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5906 | PosCovGeodetic | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5907 | VelCovCartesian | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5908 | VelCovGeodetic | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5909 | DOP | Not implemented | - | No | Guide p. 10 |
| 5910 | PVTResiduals | Not implemented | - | No | Guide p. 10 |
| 5911 | xPPSOffset | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5912 | TrackingStatus | Not implemented | - | No | Guide p. 10 |
| 5913 | ReceiverStatus | Not implemented | - | No | Guide p. 10 |
| 5914 | ReceiverTime | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5915 | RAIMStatistics | Not implemented | - | No | Guide p. 10 |
| 5917 | GEOServiceLevel | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5918 | GEONetworkTime | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5919 | DiffCorrIn | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5920 | DiffCorrEpoch | Not implemented | - | No | Guide p. 10 |
| 5921 | EndOfPVT | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5922 | EndOfMeas | Not implemented | - | No | Guide p. 9; AsteRx-U p. 440 |
| 5924 | ExtEvent | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5925 | GEOMT00 | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5926 | GEOPRNMask | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5927 | GEOFastCorr | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5928 | GEOIntegrity | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5929 | GEOFastCorrDegr | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5930 | GEODegrFactors | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5931 | GEOIGPMask | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5932 | GEOLongTermCor | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5933 | GEOIonoDelay | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5934 | GEOClockEphCovMatrix | Not implemented | - | No | Guide p. 9; AsteRx-U p. 441 |
| 5935 | GEOCorrections | Not implemented | - | No | Guide p. 10 |
| 5936 | Comment | Not implemented | - | No | Guide p. 10; AsteRx-U p. 443 |
| 5938 | AttEuler | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5939 | AttCovEuler | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5942 | AuxAntPositions | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5943 | EndOfAtt | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5944 | GenMeasEpoch | Not implemented | - | No | Guide p. 9 |
| 5947 | CNAVRaw | Not implemented | - | No | Guide p. 9 |
| 5949 | BaseStation | Not implemented | - | No | Guide p. 10; AsteRx-U p. 442 |
| 5950 | BaseLine | Not implemented | - | No | AsteRx-U p. 442 |
| 6000 | SystemInfo | Not implemented | - | No | AsteRx-U p. 443 |


[All protocols](supported-protocols.md)
