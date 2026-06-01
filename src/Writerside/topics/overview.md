# Asv.Gnss

%product% is a .NET library for parsing GNSS, aviation, and receiver-control protocols.
The repository ships the `%package%` NuGet package and a `%shell%` console project for RTCM v3 and ASTERIX stream diagnostics.

Use %product% when an application needs to receive, decode, route, or serialize navigation data from receivers, serial ports, TCP/UDP endpoints, log files, or protocol streams.

## Main capabilities

- NMEA 0183 parsing and serialization.
- RTCM v2 and RTCM v3 parsing, including ephemeris, stationary RTK, GLONASS bias, and MSM messages.
- UBX support for u-blox receivers, including helper APIs for common configuration tasks.
- ASTERIX message parsing and stream diagnostics for surveillance data.
- Protocol registration for `Asv.IO` routers, ports, endpoint tags, and broadcast flows.
- GNSS device discovery and microservices for receiver-facing workflows.

## Projects

| Project | Purpose |
|---------|---------|
| `Asv.Gnss` | Library with protocols, message models, parsers, device factory, and microservices. |
| `Asv.Gnss.Shell` | Console diagnostics for RTCM v3 streams and ASTERIX stream files. |
| `Asv.Gnss.Test` | xUnit v3 tests and binary protocol test resources. |

## Requirements

- %dotnet%.
- Windows, Linux, or macOS for the library.
- Windows for the repository batch scripts `build.bat` and `publish_nuget.bat`.
- Visual Studio, JetBrains Rider, or another IDE that supports SDK-style .NET projects.

<seealso>
    <category ref="gnss">
        <a href="https://github.com/asv-soft/asv-gnss">Repository</a>
        <a href="https://github.com/asv-soft/asv-gnss/issues">Issues</a>
        <a href="https://docs.asv.me/">ASV documentation</a>
    </category>
</seealso>
