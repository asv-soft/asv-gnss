# Asv.Gnss

[![NuGet version](https://img.shields.io/nuget/v/Asv.Gnss)](https://www.nuget.org/packages/Asv.Gnss)
[![Build and tests](https://github.com/asv-soft/asv-gnss/actions/workflows/code-quality.yml/badge.svg?branch=main)](https://github.com/asv-soft/asv-gnss/actions/workflows/code-quality.yml)
[![Documentation](https://github.com/asv-soft/asv-gnss/actions/workflows/docs.yml/badge.svg?branch=main)](https://asv.me/asv-gnss/)
[![GitHub Packages](https://img.shields.io/badge/GitHub-Packages-blue?logo=github)](https://github.com/asv-soft/asv-gnss/pkgs/nuget/Asv.Gnss)

[![Build and Publish Release Version](https://github.com/asv-soft/asv-gnss/actions/workflows/release-action.yml/badge.svg)](https://github.com/asv-soft/asv-gnss/actions/workflows/release-action.yml)
[![Deploy debug Nuget for Windows](https://github.com/asv-soft/asv-gnss/actions/workflows/release-debug-action.yml/badge.svg)](https://github.com/asv-soft/asv-gnss/actions/workflows/release-debug-action.yml)

`Asv.Gnss` is a .NET library for parsing GNSS, aviation, receiver-control protocols, and RINEX data files. The project is distributed as the `Asv.Gnss` NuGet package and also includes console tools for mixed-protocol file inspection, RTCM v3, and ASTERIX stream diagnostics.

## Documentation

Read the [Asv.Gnss documentation](https://asv.me/asv-gnss/) for installation, quick-start examples, supported protocols, architecture, console tools, and publishing instructions.

## Features

- Read and write RINEX 2.10/2.11, 3.00–3.05, and 4.00–4.02 observation, navigation, and meteorological records, with stream registration and gzip file reading. See [RINEX coverage and examples](src/Writerside/topics/protocol-rinex.md).

- Parse and serialize NMEA 0183 messages: `GBS`, `GGA`, `GLL`, `GSA`, `GST`, `GSV`, `RMC`, `VTG`, `ZDA`.
- Parse RTCM v2 and RTCM v3, including legacy messages, ephemeris, stationary RTK, GLONASS bias, and MSM3/MSM4/MSM5/MSM7 messages.
- Parse and serialize the 163 UBX message variants in the u-blox ZED-X20P X20 HPG 2.11 interface, including `NAV2`, `MGA`, `SEC`, `TIM`, and configuration values for 826 keys. Existing UBX registrations retain their legacy model types. See [X20 coverage and examples](src/Writerside/topics/protocol-ubx-x20.md).
- Parse ASTERIX categories `001`, `002`, `004`, `010`, `020`, `021`, `034`, `048`, and `247`.
- Parse and serialize 303 native Javad GREIS message IDs, with receiver commands, response labels, DTP file-transfer codecs, and the GREIS 4.2.01 object catalog. See [Javad / GREIS coverage](src/Writerside/topics/protocol-javad.md).
- Parse and serialize native GPS LNAV subframes and GLONASS FDMA strings with parity validation, typed visitors, and bit-stream protocol registration, including GPS NMCT/special-message/UTC pages and explicit GLONASS fifth-frame layouts.
- Parse and serialize ADS-B / Mode S AVR and Beast receiver streams, with CRC/parity status and typed identification, airborne/surface position fields and velocity. See [ADS-B coverage and limits](src/Writerside/topics/protocol-ads-b.md).
- Additional protocols and parsers: ASV, SBF, and ComNav.
- Integration with `Asv.IO`: protocol registration, routers, ports, GNSS device discovery, and NMEA/RTCM/UBX microservices.

## Requirements

- .NET SDK 10.0.
- Windows, Linux, or macOS for the library. The local publishing batch scripts require Windows.
- Visual Studio, JetBrains Rider, or another IDE with SDK-style .NET project support.

Check the installed SDK version:

```bash
dotnet --version
```

## Installation

Using the .NET CLI:

```bash
dotnet add package Asv.Gnss
```

Using NuGet Package Manager:

```powershell
Install-Package Asv.Gnss
```

## Quick Start

The example below creates an `Asv.IO` protocol, registers GNSS protocols, and opens a port:

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

Connection string examples supported by the `Asv.IO` transport layer:

```text
tcp://127.0.0.1:9002
tcps://127.0.0.1:9002
udp://127.0.0.1:1234?rhost=127.0.0.1&rport=1235
serial:COM4?br=115200
serial:/dev/ttyACM0?br=115200
```

TCP uses separate schemes for client and server ports:

- `tcp://host:port` creates a TCP client port that connects to a remote endpoint.
- `tcps://host:port` creates a TCP server port that listens for incoming clients.

Register the GNSS device factory:

```csharp
var explorer = DeviceExplorer.Create(router, builder =>
{
    builder.Factories.RegisterGnssDevice();
});
```

## RINEX Files

Read a plain or gzip-compressed OBS/NAV/MET file, inspect typed records, and write it back:

```csharp
using Asv.Gnss;
using System.Linq;

var records = await RinexFile.ReadAsync("station.rnx.gz");
foreach (var epoch in records.OfType<RinexObservationEpochMessage>())
{
    Console.WriteLine($"{epoch.Epoch}: {epoch.Satellites.Count} satellites");
}
RinexFile.Write("station-copy.rnx", records);
```

For incremental input, register `builder.Protocols.RegisterRinexProtocol()`, obtain a
`RinexMessageParser`, feed bytes through `Push`, and call `Complete()` at EOF. The parser
publishes a header followed by typed records. Time tags retain the file's time system;
blank observations remain distinct from zero. See [the complete RINEX guide](src/Writerside/topics/protocol-rinex.md)
for coverage, canonical writing, event handling, and unsupported file families.

Inspect a RINEX file in the console viewer:

```bash
dotnet run --project src/Asv.Gnss.Shell -- inspect -f "station.rnx"
```

## UBX Helper API

The UBX microservice provides helper methods for u-blox devices:

```csharp
await ubx.GetMonVer(cancel);
await ubx.GetNavPvt(cancel);
await ubx.SetSurveyInMode(minDuration: 60, positionAccuracyLimit: 2, cancel: cancel);
await ubx.SetupRtcmMSM4Rate(msgRate: 1, cancel: cancel);
await ubx.TurnOffNmea(cancel);
await ubx.RebootReceiver(cancel);
```

## Console Tools

The `Asv.Gnss.Shell` project contains diagnostic commands.

Read an RTCM v3 stream:

```bash
dotnet run --project src/Asv.Gnss.Shell -- rtcm --cs "tcp://172.16.0.1:30"
```

Use `tcps://0.0.0.0:7341` when the shell should listen for incoming TCP clients instead of connecting as a TCP client.

Parse an ASTERIX stream file:

```bash
dotnet run --project src/Asv.Gnss.Shell -- asterix-stream --file "path/to/input.stream"
```

Export CAT021 tracks from an ASTERIX stream to KML:

```bash
dotnet run --project src/Asv.Gnss.Shell -- asterix-stream --file "path/to/input.stream" --kml "tracks.kml" --min-track-points 2
```

The `nmea` command exists in the codebase, but it is currently not registered in `Program.cs`.

## Build and Test

Restore dependencies:

```bash
dotnet restore src/Asv.Gnss.sln
```

Build the solution:

```bash
dotnet build src/Asv.Gnss.sln
```

Run tests:

```bash
dotnet test src/Asv.Gnss.sln
```

Build the library NuGet package:

```bash
dotnet pack src/Asv.Gnss/Asv.Gnss.csproj -c Release
```

On Windows, you can use the root build script:

```bat
build.bat
```

The script reads the version from `git describe --tags`, applies it with `dotnet-setversion`, builds the projects, and runs `dotnet pack`.

## Repository Structure

```text
asv-gnss/
  README.md
  LICENSE
  build.bat
  publish_nuget.bat
  src/
    Asv.Gnss.sln
    Directory.Build.props
    Asv.Gnss/
      Devices/
      Microservices/
      Parsers/
      Protocols/
    Asv.Gnss.Shell/
    Asv.Gnss.Test/
```

Main projects:

- `Asv.Gnss` - protocol, message, parser, GNSS device factory, and microservice library.
- `Asv.Gnss.Shell` - console commands for working with data streams and ASTERIX stream files.
- `Asv.Gnss.Test` - xUnit v3 protocol tests and binary RTCM/ASTERIX test resources.

Product version, target framework, and shared dependency versions are defined in `src/Directory.Build.props`.

## Publishing

The `publish_nuget.bat` script publishes the `Asv.Gnss` package to NuGet.org and GitHub Packages:

```bat
publish_nuget.bat
```

Before publishing, build a Release package whose version matches `git describe --tags`.

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).

## Links

- Repository: <https://github.com/asv-soft/asv-gnss>
- Issues: <https://github.com/asv-soft/asv-gnss/issues>
- Asv.Gnss documentation: [asv.me/asv-gnss](https://asv.me/asv-gnss/)
- ASV documentation: <https://docs.asv.me/>
- Telegram: <https://t.me/asvsoft>
