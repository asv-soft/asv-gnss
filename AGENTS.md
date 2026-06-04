# Repository Instructions

## Comment and Documentation Language

- Write all code comments in English.
- Write all XML documentation, Markdown documentation, README content, and other developer-facing documentation in English.
- Do not use Russian or mixed-language comments or documentation.
- Keep terminology consistent across code, comments, and documentation.
- Use clear English names for types, members, variables, files, modules, and public APIs.

## Comment Quality

- Prefer self-explanatory code over excessive comments.
- Add comments only when they explain intent, constraints, assumptions, tradeoffs, or non-obvious behavior.
- Do not add comments that only restate what the code already makes obvious.
- Keep comments concise, accurate, and aligned with the current implementation.
- Update or remove comments when the code changes so documentation never becomes misleading.

## Architecture and Design

- Keep the architecture clean, modular, and easy to maintain.
- Follow SOLID principles in design and implementation.
- Give each class, service, and module a single, well-defined responsibility.
- Prefer composition over inheritance unless inheritance is clearly justified.
- Minimize coupling and keep related behavior cohesive.
- Separate domain logic from UI, infrastructure, persistence, and framework-specific concerns.
- Depend on abstractions at system boundaries when this improves testability, extensibility, or clarity.
- Keep public APIs explicit, stable, and easy to understand.
- Eliminate duplicated logic through extraction or refactoring instead of copying behavior.
- Avoid god objects, hidden side effects, and unclear ownership of responsibilities.

## Coding Workflow

- State assumptions explicitly before implementing when requirements are unclear.
- Ask for clarification when multiple interpretations are plausible and the choice is risky.
- Prefer the simplest implementation that solves the requested problem.
- Do not add speculative features, abstractions, configurability, or error handling.
- Make surgical changes: touch only what is needed for the request.
- Match the existing style, even when a different style would also work.
- Do not refactor or reformat adjacent code unless the task requires it.
- Remove only imports, variables, functions, and files made unused by your own changes.
- Mention unrelated dead code or issues instead of changing them.
- Define verifiable success criteria for non-trivial work.
- Verify changes with focused tests or builds whenever practical.

## Dev Feed Version Updates

Package and dependency versions are centralized in `src/Directory.Build.props`.

When asked to update `Asv.Common` or shared ASV dependencies for the dev feed:

1. Check the working tree first and keep the change scoped to version metadata unless the task explicitly asks for more.
2. Update `AsvCommonVersion` in `src/Directory.Build.props` to the requested version.
3. Increment the numeric `ProductVersion` dev suffix by one, for example `2.2.0-dev.2` to `2.2.0-dev.3`.
4. Confirm the dev release workflow tag pattern in `.github/workflows/release-debug-action.yml`; the tag must be `v<ProductVersion>`, because the workflow trims the leading `v` before publishing.
5. Run focused validation when practical:

```powershell
dotnet restore src\Asv.Gnss.sln
dotnet build src\Asv.Gnss.sln -c Release --no-restore
dotnet test src\Asv.Gnss.sln --no-restore
```

6. Commit only the version metadata change, create an annotated tag, and push both `main` and the tag:

```powershell
git add src\Directory.Build.props
git commit -m "Bump Asv.Common to <version>"
git tag -a v<ProductVersion> -m "v<ProductVersion>"
git push origin main
git push origin v<ProductVersion>
```

The tag push starts the `Deploy debug Nuget for Windows` workflow and publishes the dev package to the GitHub Packages feed.
