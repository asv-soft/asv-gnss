# Architecture

%product% follows the `Asv.IO` protocol and router model.
The application composes protocol parsers, features, ports, device discovery, and microservices at startup.

## Runtime flow

```text
Port -> Router -> Protocol parser -> Message packet -> Subscriber or device microservice
```

The usual setup sequence is:

1. Create a `Protocol` with the required GNSS protocol registrations.
2. Enable router features such as broadcast and endpoint tagging.
3. Create a router.
4. Add serial, TCP, UDP, or server ports.
5. Subscribe to router messages or create a `DeviceExplorer`.

## Source layout

```text
src/
  Asv.Gnss/
    Devices/
    Microservices/
    Parsers/
    Protocols/
  Asv.Gnss.Shell/
  Asv.Gnss.Test/
```

| Directory | Responsibility |
|-----------|----------------|
| `Devices` | GNSS device factory registration and device-level integration. |
| `Microservices` | Receiver-facing client helpers and service abstractions. |
| `Parsers` | Parser modules that are not modeled as the main `Protocols` tree. |
| `Protocols` | Protocol definitions, factories, message models, and serializers. |
| `Asv.Gnss.Shell` | Diagnostic command-line workflows. |
| `Asv.Gnss.Test` | Protocol fixtures and regression tests. |

## Extension registration

Protocol registration is exposed through extension methods, for example:

```csharp
builder.Protocols.RegisterNmeaProtocol();
builder.Protocols.RegisterRtcmV2Protocol();
builder.Protocols.RegisterRtcmV3Protocol();
builder.Protocols.RegisterUbxProtocol();
builder.Protocols.RegisterAsterixProtocol();
```

This keeps application startup explicit: only the protocols you register are added to the runtime pipeline.
