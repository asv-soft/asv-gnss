# Quick Start

The library is designed to work with `Asv.IO`.
Create a protocol, register GNSS protocol implementations, create a router, and attach one or more ports.

## Register protocols

```c#
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

```c#
var explorer = DeviceExplorer.Create(router, builder =>
{
    builder.Factories.RegisterGnssDevice();
});
```

## UBX helper API

The UBX microservice includes helper methods for common u-blox workflows.

```c#
await ubx.GetMonVer(cancel);
await ubx.GetNavPvt(cancel);
await ubx.SetSurveyInMode(minDuration: 60, positionAccuracyLimit: 2, cancel: cancel);
await ubx.SetupRtcmMSM4Rate(msgRate: 1, cancel: cancel);
await ubx.TurnOffNmea(cancel);
await ubx.RebootReceiver(cancel);
```

## Visit UBX payload fields

All existing UBX messages implement `Asv.IO.IVisitable`. Their `Accept` methods expose typed payload properties with field names, English descriptions, units where applicable, and numeric enum metadata. Satellite records, port configurations, coordinates, arrays, and lists are visited through the corresponding nested Asv.IO types.

```c#
using Asv.Gnss;
using Asv.IO;

var message = new UbxCfgRate { RateHz = 1, NavRate = 1 };
IVisitable payload = message;

Console.WriteLine(payload.PrintValues());
payload.Accept(new MeasurementRateVisitor());
Console.WriteLine(message.RateHz); // 5

sealed class MeasurementRateVisitor : DoubleType.IVisitor
{
    public void Visit(Field field, DoubleType type, ref double value)
    {
        if (field.Name == nameof(UbxCfgRate.RateHz))
        {
            value = 5;
        }
    }

    public void VisitUnknown(Field field, IFieldType type)
    {
    }
}
```

A read-only visitor preserves values, absent properties, collection references, and existing serialized packets. Empty poll messages expose no payload fields; poll messages with parameters expose those parameters. An external `UbxMessageBase` subclass can override the virtual `Accept` method to describe its own payload.

Native scalar values and collection elements are passed by `ref` to their backing storage, so edits are visible during the visitor callback. Repeated traversal of initialized collections reuses their storage; changing a variable array's size allocates a replacement array. List sizes must be changed through `BeginList` before visiting elements; callbacks must keep the collection structure stable during element traversal. Enum values and converted coordinate models use typed conversion steps.

Traversal reflects the existing object model and converted property units. It is separate from UBX binary serialization and does not automatically recompute related count fields, flags, or derived values after an edit. In `UbxRxmSfrbx`, the visitor exposes the payload identifiers, channel, version, reserved fields, and `RawData` words; the separately decoded `GpsSubFrame` and `GlonassWord` models are not traversed.

## Read a RINEX file

Use the contextual file reader for RINEX observation, navigation or meteorological data:

```c#
using Asv.Gnss;
using System.Linq;

var records = await RinexFile.ReadAsync("station.rnx.gz");
var epochs = records.OfType<RinexObservationEpochMessage>();
foreach (var epoch in epochs)
{
    Console.WriteLine($"{epoch.Epoch}: {epoch.Satellites.Count} satellites");
}
RinexFile.Write("station-copy.rnx", records);
```

Gzip input is detected automatically. For incremental parsing, register `RegisterRinexProtocol()` and call `RinexMessageParser.Complete()` at EOF. See [RINEX coverage and examples](protocol-rinex.md) for version coverage, header context, time systems and canonical writing.

## Receiver identity and typed streams

Each exact endpoint represents one receiver, even when it sends several GNSS protocols. See [GNSS devices and microservices](gnss-devices.md) for typed subscriptions, canonical addresses, discovery exclusions, and disposal rules.
