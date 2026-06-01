# Quick Start

The library is designed to work with `Asv.IO`.
Create a protocol, register GNSS protocol implementations, create a router, and attach one or more ports.

## Register protocols

```csharp
using Asv.Gnss;
using Asv.IO;
using R3;

var protocol = Protocol.Create(builder =>
{
    builder.Protocols.RegisterNmeaProtocol();
    builder.Protocols.RegisterRtcmV2Protocol();
    builder.Protocols.RegisterRtcmV3Protocol();
    builder.Protocols.RegisterUbxProtocol();
    builder.Protocols.RegisterAsterixProtocol();
    builder.Features.RegisterBroadcastAllFeature();
    builder.Features.RegisterEndpointIdTagFeature();
});

var router = protocol.CreateRouter("GNSS");
router.AddPort("serial:COM4?br=115200&enabled=true");

using var subscription = router.OnRxMessage.Subscribe(packet =>
{
    Console.WriteLine(packet);
});
```

## Connection strings

`Asv.IO` ports are configured with connection strings.

| Example | Meaning |
|---------|---------|
| `tcp://127.0.0.1:9002` | TCP client port that connects to a remote endpoint. |
| `tcps://127.0.0.1:9002` | TCP server port that listens for incoming clients. |
| `udp://127.0.0.1:1234?rhost=127.0.0.1&rport=1235` | UDP port with remote host and port. |
| `serial:COM4?br=115200` | Windows serial port. |
| `serial:/dev/ttyACM0?br=115200` | Linux or macOS serial port. |

## Device discovery

Register the GNSS device factory when the application needs receiver-level discovery and microservices.

```csharp
var explorer = DeviceExplorer.Create(router, builder =>
{
    builder.Factories.RegisterGnssDevice();
});
```

## UBX helper API

The UBX microservice includes helper methods for common u-blox workflows.

```csharp
await ubx.GetMonVer(cancel);
await ubx.GetNavPvt(cancel);
await ubx.SetSurveyInMode(minDuration: 60, positionAccuracyLimit: 2, cancel: cancel);
await ubx.SetupRtcmMSM4Rate(msgRate: 1, cancel: cancel);
await ubx.TurnOffNmea(cancel);
await ubx.RebootReceiver(cancel);
```
