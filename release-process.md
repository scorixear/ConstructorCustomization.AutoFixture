# Release Process

This document describes how to ship a new **major**, **minor**, or **patch** release for this package to both GitHub and nuget.org.

## Current Automation (Repository Setup)

- PR validation workflow: [.github/workflows/pr-build-test.yml](.github/workflows/pr-build-test.yml)
- Automated publish workflow: [.github/workflows/publish-nuget.yml](.github/workflows/publish-nuget.yml)
- Versioning rules: [GitVersion.yml](GitVersion.yml)
- NuGet target feed: `https://api.nuget.org/v3/index.json`
- Supported frameworks: `net8.0`, `net10.0`, `netstandard2.1`
- Authentication: repository secret `NUGET_API_KEY`

### What happens automatically when a GitHub Release is published

1. The **Publish NuGet Package** workflow triggers on `release.published`.
2. The workflow runs only when the release target is `main`.
3. The project is built, tested, packed, and pushed to nuget.org.
4. Symbol packages (`.snupkg`) are pushed to nuget.org.
5. The workflow checks out the release tag and publishes that exact build.

## Versioning Model

- Branch prerelease labels:
  - `develop` -> `alpha`
  - `main` -> `beta`
- Final stable release versions come from Git tags in the form `vX.Y.Z`.

Examples:
- `v1.0.0` -> stable `1.0.0`
- `v1.3.0` -> stable `1.3.0`
- `v1.3.5` -> stable `1.3.5`

## Prerequisites

1. GitHub repository secret `NUGET_API_KEY` is configured.
2. Changes are merged and CI is green in GitHub Actions.
3. You are ready to release from the commit that should become the package.

## Choose the Next Version

Use Semantic Versioning:

- **Patch**: backward-compatible bug fix
  - Example: `1.4.2` -> `1.4.3`
- **Minor**: backward-compatible feature
  - Example: `1.4.2` -> `1.5.0`
- **Major**: breaking change
  - Example: `1.4.2` -> `2.0.0`

## Release Steps (Git + GitHub + NuGet)

1. Make sure your release commit is on `main` (recommended for stable releases).
2. In GitHub, create a new **Release** from `main` using a tag `vX.Y.Z`.

Publishing the release automatically:
- Triggers **Publish NuGet Package** in GitHub Actions.
- Builds, tests, packs, and pushes the package (and symbols) to nuget.org.

3. Monitor the run under **Actions** → **Publish NuGet Package** and verify:
   - SemVer and NuGet version in the workflow summary.
  - Package visible on nuget.org.

## Quick Checklists

### Patch Release

1. Confirm fix merged to `main`.
2. Create/publish release `vA.B.(C+1)` from `main` — everything else is automatic.

### Minor Release

1. Confirm feature set merged to `main`.
2. Create/publish release `vA.(B+1).0` from `main` — everything else is automatic.

### Major Release

1. Confirm breaking changes documented.
2. Create/publish release `v(A+1).0.0` from `main` — everything else is automatic.
3. Add migration notes in the release description.

## Troubleshooting

- Wrong version generated:
  - Confirm workflow was run against the correct ref (tag, not branch).
  - Confirm tag format is exactly `vX.Y.Z`.
- Publish fails with auth error:
  - Recreate/rotate `NUGET_API_KEY` secret in GitHub.
- Package already exists:
  - Publishing uses `--skip-duplicate`; verify whether this is expected.
