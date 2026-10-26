# GNSS endpoint devices implementation plan

> **For agentic workers:** Use the approved design and the broad code-testing workflow; execute tests before implementation and review the complete change once integration passes.

**Goal:** Implement one-device-per-endpoint discovery/address lookup and separate typed microservices for all implemented GNSS protocols.

**Architecture:** Canonical `gnss-endpoint` addresses share exact EndpointId values. A generic endpoint-scoped microservice base exposes typed receive streams, while concrete per-family services provide distinct lookup types. GnssDevice owns service and link lifetimes.

**Tech Stack:** C#/.NET10, Asv.IO3.7.4, R3, xUnitv3.

**Spec:** `docs/superpowers/specs/2026-10-26-gnss-endpoint-devices-design.md` (approved user direction).

## Global constraints

- One receiver per endpoint; preserve existing GnssDeviceId.AsString and pendingdependency3.7.4.
- English comments/docs, no new native protocols or GNSS calculations.
- No commits, push, tags, dependencypublication or hardware access.

## Review focus

- Default DeviceId equality only compares class; all endpoint checks must use exact EndpointId values.
- Microservice constructors subscribe before derived initialization; filters must not accept other endpoints or protocols.
- First discovery packet may predate service construction; later traffic must route correctly and first packet must mark liveness.
- Pure lookup must not change catalog or keepalive; controlledtime verifies expiry.
- Disposal releases owned services/timers without closing sharedconnections.

### Task1: Factory and lifecycle

Modify `Devices/Client/GnssDeviceFactory.cs`, `Devices/GnssDeviceId.cs`, `Devices/Client/GnssDevice.cs`; add `Devices/GnssDeviceSource.cs`. Tests in `Asv.Gnss.Test/Devices/GnssDeviceFactoryTest.cs` and explorer integration.

- [x] Write failing address extraction,12familydiscovery, mismatch/invalidinput and lookup-purity tests.
- [x] Observe RED before implementation.
- [x] Implement `TryGetSourceAddress(IProtocolMessage,out DeviceAddress)` and `TryGetDeviceAddress(DeviceId,out DeviceAddress)` with shared helper, match create/update endpoints and normalpriority.
- [x] Passcontext/extenders, firstkeepalive, context.TimeProvider and sync/asyncownedlinkdisposal.
- [x] Verify tests pass.

### Task2: Separate protocol microservices

Add `Microservices/GnssProtocolMicroserviceClient.cs`, concrete clients under `Microservices/{RtcmV2,Sbf,ComNav,Javad,Asv,Gbas,Rinex}/Client`; adapt existing Nmea/RtcmV3/Ubx clients.

Interfaces: `IGnssProtocolMicroserviceClient<TMessage> : IMicroserviceClient`, `Observable<TMessage> OnMessage`, `Observable<T> Filter<T>() where T:TMessage`. Generic base constructor `(IMicroserviceContext,GnssDeviceId,ProtocolInfo)` retains endpoint scope; no newly invented command API. New concrete clients only add nativefamily type/identity.

- [x] Write failing type/endpoint routing and disposal tests for every concreteclient.
- [x] Observe RED, implement commonbase and concretefamilyclasses, and retain existing3serviceAPIs.
- [x] Wire12services into GnssDevice and verify unique TypeName and lookup types.

### Task3: Integration and documentation

- [x] Verify mixedfamilies share endpoint device and differentendpoints separate through DeviceExplorer.
- [x] Update README and Writerside devices/microservices with addressfamily, supportedfamilytable, typed subscriptions, offlineRINEX scope and ownership.
- [x] Run focused GNSS device/service tests, Release solutionbuild and fullsuite once integration is stable; inspect each result.
- [x] Obtain independent final review and fix proven defects with focused regressiontests. Record exact test names/commands/results in `.testagent/status.md`.

User correction: raw GPS and raw GLONASS are explicitly excluded from receiver discovery and microservices.
