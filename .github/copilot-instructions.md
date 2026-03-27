# ConstructorCustomization.AutoFixture — Copilot Instructions

This is a NuGet library targeting `net8.0`, `net10.0`, and `netstandard2.1`. The following conventions are non-negotiable and must be followed in all code contributions.

## Project Structure

- **Package metadata** (`Description`, `PackageTags`, `Authors`, `PackageLicenseExpression`, etc.) belongs in `Directory.Build.props` — never in individual `.csproj` files.
- **Dependency versions** belong exclusively in `Directory.Packages.props` (Central Package Management). Never set `Version` on a `PackageReference` in a `.csproj`.
- **Package version** is never set manually in any project file. It is computed from git tags by GitVersion at build time.
- New dependencies: add `PackageVersion` to `Directory.Packages.props` AND `PackageReference` (without `Version`) to the relevant `.csproj`.

## Versioning

- Versioning follows Semantic Versioning via GitVersion (v6.x configuration in `GitVersion.yml`).
- Removing or renaming `public` types, members, or constructor parameters is a **breaking change** and requires a major version bump.
- In `GitVersion.yml`: use `tag`, never `label`; do not use `is-main-branch`.

## Multi-Target Compatibility

All library code must compile and run correctly on `netstandard2.1`. If a language feature or API is unavailable on netstandard2.1, add a polyfill under `Polyfills/` (see existing examples: `IsExternalInit.cs`, `CallerArgumentExpressionAttribute.cs`).

## Testing

- Test runner: NUnit 4 via `dotnet test` from the repo root.
- All test fixture classes must be `public` (not `internal`).
- Every new `public` API surface must have corresponding test coverage.
- Coverage baseline: ≥96% line, ≥93% branch. Do not submit changes that reduce coverage without justification.
- Run `dotnet test` before opening a PR.

## CI/CD

- The `publish-nuget.yml` workflow triggers on `release.published` targeting `main` only — do not alter this guard.
- The `pr-build-test.yml` workflow must always include a `dotnet test` step — do not remove or skip it.
- Publish is triggered by creating a GitHub Release with a `vX.Y.Z` tag. See `release-process.md` for the full checklist.

## Examples Projects

- Projects under `Examples/` restore from the **local** `.nupkg`, not from nuget.org.
- Do not change `Examples/nuget.config` to point at nuget.org.
- Use `scripts/rebuild-package-and-examples.ps1` to regenerate the local pack before working on examples.

## Code Quality

- Nullable reference types are enabled — all `public` API members must have correct nullability annotations.
- XML documentation comments are required on all `public` types and members (`GenerateDocumentationFile` is enabled; missing docs become build warnings).
- `InternalsVisibleTo` is granted only to `ConstructorCustomization.AutoFixture.Tests` — do not widen this.
