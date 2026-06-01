# Publishing

The repository includes `publish_nuget.bat` for publishing `%package%` packages.

## Prepare a package

Build a Release package whose version matches the current Git tag or `git describe --tags` output.

```bat
build.bat
```

Or pack the library directly:

```bash
dotnet pack src/Asv.Gnss/Asv.Gnss.csproj -c Release
```

## Publish

Run the publishing script from the repository root:

```bat
publish_nuget.bat
```

The script is intended for Windows and publishes the `%package%` package to NuGet.org and GitHub Packages.

## Version source

Shared version properties are stored in `src/Directory.Build.props`:

```xml
<ProductVersion>2.2.0-dev.0</ProductVersion>
<TargetFrameworksValue>net10.0</TargetFrameworksValue>
<DotNetVersion>10.0.0</DotNetVersion>
```

Keep package version, Git tags, and release artifacts aligned before publishing.
