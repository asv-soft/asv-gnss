# Console Tools

The `%shell%` project contains diagnostic commands implemented with `ConsoleAppFramework`.
Run commands from the repository root.

## RTCM v3 stream

Read an RTCM v3 stream from a connection string:

```bash
dotnet run --project src/Asv.Gnss.Shell -- rtcm --cs "tcp://172.16.0.1:30"
```

The command registers the RTCM v3 protocol, opens the input port, and also opens a local TCP server port:

```text
tcps://127.0.0.1:7341?enabled=true
```

Use `tcps://host:port` when the shell should listen for incoming TCP clients instead of connecting to a remote endpoint.

## ASTERIX stream statistics

Parse an ASTERIX `.stream` file and print frame statistics:

```bash
dotnet run --project src/Asv.Gnss.Shell -- asterix-stream --file "path/to/input.stream"
```

## ASTERIX KML export

Export CAT021 tracks from an ASTERIX stream to KML:

```bash
dotnet run --project src/Asv.Gnss.Shell -- asterix-stream --file "path/to/input.stream" --kml "tracks.kml" --min-track-points 2
```

The KML export is best-effort: invalid or incomplete CAT021 records are skipped while stream validation and category statistics still run.

## NMEA command status

`NmeaCommand` exists in the project, but it is currently not registered in `Program.cs`.
Register `app.Add<NmeaCommand>();` before using the `nmea` command from the shell.
