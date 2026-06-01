# Build and Test

Use the solution file in `src/Asv.Gnss.sln`.

## Restore

```bash
dotnet restore src/Asv.Gnss.sln
```

## Build

```bash
dotnet build src/Asv.Gnss.sln
```

## Test

```bash
dotnet test src/Asv.Gnss.sln
```

The test project uses xUnit v3 and includes binary protocol resources under `src/Asv.Gnss.Test/Resources`.

## Pack

Build the library NuGet package:

```bash
dotnet pack src/Asv.Gnss/Asv.Gnss.csproj -c Release
```

`Asv.Gnss.csproj` has `GeneratePackageOnBuild` enabled, so Release builds also produce a package.

## Windows build script

On Windows, the repository build script can be used from the repository root:

```bat
build.bat
```

The script reads the version from `git describe --tags`, applies it with `dotnet-setversion`, builds projects, and runs `dotnet pack`.
