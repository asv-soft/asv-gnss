# Publishing

The repository includes `publish_nuget.bat` for publishing `%package%` packages.

## Prepare a package

Build a Release package whose version matches the current Git tag or `git describe --tags` output.

```batch
build.bat
```

Or pack the library directly:

```bash
dotnet pack src/Asv.Gnss/Asv.Gnss.csproj -c Release
```

## Publish

Run the publishing script from the repository root:

```batch
publish_nuget.bat
```

The script is intended for Windows and publishes the `%package%` package to NuGet.org and GitHub Packages.

## Version source

Shared version properties are stored in `src/Directory.Build.props`:

```xml
<ProductVersion>2.2.0</ProductVersion>
<TargetFramework>net10.0</TargetFramework>
```

Keep package version, Git tags, and release artifacts aligned before publishing.


## Publish documentation

The `Publish documentation` workflow checks changes to `src/Writerside/**`, `src/Directory.Build.props`, and `.github/workflows/docs.yml` in pull requests. Matching pushes to `main` publish the site to [Asv.Gnss documentation](https://asv.me/asv-gnss/). The workflow can also be started manually from GitHub Actions; only runs on `main` deploy.

Before building, the workflow reads `ProductVersion` from `src/Directory.Build.props` and sets the `version` attribute of the `a.tree` instance in its temporary checkout of `writerside.cfg`. Writerside displays that version in the site header, including development and release-candidate suffixes. Local previews display the version stored in `writerside.cfg`; keep that preview value aligned with `ProductVersion` when preparing a release. CI always replaces it with the current `ProductVersion`.

The pipeline builds the HTML website with the Writerside Docker builder, checks the build report, and publishes the generated artifact through GitHub Pages. Failed builds or documentation checks prevent deployment. Documentation publication does not require a release tag and does not publish NuGet packages.

The repository must use **Settings → Pages → Source → GitHub Actions**. In the repository's About settings, **Use your GitHub Pages website** keeps the Website link synchronized with the Pages address. Publication uses the built-in `GITHUB_TOKEN`; no separate deployment secret is required.
