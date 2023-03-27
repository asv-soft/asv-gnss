# asv-gnss 
GNSS library for parsing RTCMv2, RTCMv3, NMEA and control recievers througt SBF, ComNav, UBX protocols for .NET

[![Deploy Nuget for Windows](https://github.com/asv-soft/asv-gnss/actions/workflows/nuget_windows.yml/badge.svg)](https://github.com/asv-soft/asv-gnss/actions/workflows/nuget_windows.yml)

## Install via nuget

```bash
Install-Package Asv.Gnss -Version X.X.X
```
## Example usage

```csharp

// create connection with default parsers: Nmea,RTCMv2,RTCMv3,ComNav,Ubx,Sbf
var connection = GnssFactory.CreateDefault("tcp://10.10.5.28:64101");
// for TCP client connection:               tcp://127.0.0.1:9002
// for TCP server(listen) connection:       tcp://127.0.0.1:9002?srv=true
// for UDP connection:                      udp://127.0.0.1:1234?rhost=127.0.0.1&rport=1235
// for COM\serial port connection Linux:    serial:/dev/ttyACM0?br=115200
// for COM\serial port connection Windows:  serial:COM4?br=115200
connection
    .Filter<RtcmV2Message1>()
    .Subscribe(_ => { /* do something with RTCM Differential GPS Corrections (Fixed) message */ });
connection
    .Filter<RtcmV3Message1006>()
    .Subscribe(_ => { /* do something with RTCM 1006 Stationary RTK Reference Station ARP */ });
    
// you can control GNSS receivers through connection too


// for example configures the SinoGNSS receiver to fix the height at the last calculated value
ComNavAsciiCommandHelper.FixAuto(connection);
// enable send NMEA GPGLL messages through 1 sec 
ComNavAsciiCommandHelper.LogCommand(connection, ComNavMessageEnum.GPGLL, period: 1, trigger: ComNavTriggerEnum.ONTIME).Wait();


// UBlox devices 
var device = new UbxDevice("serial:COM10?br=115200");
await device.SetupByDefault();
await device.SetSurveyInMode(minDuration: 60, positionAccuracyLimit: 2);
device.Connection.Filter<RtcmV3Msm4>().Subscribe(_ => { /* do something with RTCM */ });

```


