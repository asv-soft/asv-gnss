# GNSS devices and microservices

`GnssDeviceFactory` implements `IClientDeviceFactory` using one logical receiver per endpoint. Different protocol messages from the same endpoint identify the same receiver. Endpoint values use ordinal, case-sensitive comparison without normalization; the stable display identity remains `Gnss receiver:<endpoint>`.

## Registration and discovery

Register `RegisterEndpointIdTagFeature()` on the protocol builder so incoming messages carry the transport endpoint tag, then register the factory:

```c#
using Asv.Gnss;
using Asv.IO;

using var explorer = DeviceExplorer.Create(router, builder =>
{
    builder.Factories.RegisterGnssDevice();
});
```

Only supported messages with a nonblank endpoint tag participate. Raw GPS subframes, raw GLONASS words, ASTERIX, and ADS-B do not create GNSS devices or receiver microservices. Their existing parsers remain available.

The factory exposes canonical `DeviceAddress` values with family `gnss-endpoint` and the exact endpoint as value. `TryGetSourceAddress` extracts this address from a supported message; `TryGetDeviceAddress` extracts it from a `GnssDeviceId`. These hooks only describe addresses. Explorer lookup does not create receivers or renew their link lifetime.

Creation and updates validate the protocol and endpoint. The first accepted discovery packet marks the receiver connected; subsequent accepted traffic renews its link indicator. Rejected traffic cannot keep another receiver alive. The link indicator uses the context's `TimeProvider` and a ten-second timeout with periodic checks.

## Separate protocol clients

Each initialized receiver exposes these clients, even before it has received traffic for every family:

| Family | Client interface | Native message base |
|---|---|---|
| NMEA | `INmeaMicroserviceClient` | `NmeaMessageBase` |
| RTCM v2 | `IRtcmV2MicroserviceClient` | `RtcmV2MessageBase` |
| RTCM v3 | `IRtcmV3MicroserviceClient` | `RtcmV3MessageBase` |
| UBX, including X20 | `IUbxMicroserviceClient` | `UbxMessageBase` |
| SBF | `ISbfMicroserviceClient` | `SbfMessageBase` |
| ComNav binary | `IComNavBinaryMicroserviceClient` | `ComNavBinaryMessageBase` |
| ComNav ASCII | `IComNavAsciiMicroserviceClient` | `ComNavAsciiMessageBase` |
| ComNav simple answer | `IComNavSimpleAnswerMicroserviceClient` | `ComNavSimpleAnswerMessageBase` |
| Javad GREIS | `IJavadMicroserviceClient` | `JavadMessageBase` |
| ASV | `IAsvMicroserviceClient` | `AsvMessageBase` |
| GBAS | `IGbasMicroserviceClient` | `GbasMessageBlockBase` |
| RINEX | `IRinexMicroserviceClient` | `RinexMessageBase` |

All clients implement `IGnssProtocolMicroserviceClient<TMessage>`. `OnMessage` publishes native messages from their own endpoint and family. `Filter<T>()` selects a native subtype while retaining endpoint isolation. Existing NMEA/RTCM v3 reactive properties and UBX command helpers remain available.

For an initialized device, subscribe once and retain the subscription for the consumer's lifetime:

```c#
using R3;

var sbf = device.GetRequiredMicroservice<ISbfMicroserviceClient>();
using var subscription = sbf.OnMessage.Subscribe(message =>
{
    Console.WriteLine(message.Name);
});
```

The streams publish traffic received after subscription; the discovery packet may arrive before the services are initialized. New clients preserve native data and do not introduce position conversions or receiver commands.

RINEX participates only when records are explicitly delivered through the connection with an endpoint tag. Reading a file with `RinexFile.Read` does not automatically discover a receiver. See [RINEX files](protocol-rinex.md) for file parsing and EOF handling.

## Ownership

The explorer initializes discovered devices and forwards its context and configured extenders. A device owns its protocol clients and link timer. Synchronous and asynchronous disposal release those resources while leaving the shared connection under its caller's ownership. Dispose consumer subscriptions when their consumers stop.
