# Installation

Install the `%package%` package into an SDK-style .NET project.

## NuGet

Use the .NET CLI:

```bash
dotnet add package Asv.Gnss
```

Or use the NuGet Package Manager console:

```powershell
Install-Package Asv.Gnss
```

## Source checkout

Clone the repository and restore dependencies:

```bash
git clone https://github.com/asv-soft/asv-gnss.git
cd asv-gnss
dotnet restore src/Asv.Gnss.sln
```

The target framework and shared dependency versions are defined in `src/Directory.Build.props`.

## SDK check

Verify that the expected SDK is available:

```bash
dotnet --version
```

The current repository configuration targets `net10.0`.
