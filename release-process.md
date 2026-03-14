# Release Process

This document describes how to ship a new **major**, **minor**, or **patch** release for this package to both GitHub and nuget.org.

## Current Automation (Repository Setup)

- PR validation workflow: [.github/workflows/pr-build-test.yml](.github/workflows/pr-build-test.yml)
- Manual publish workflow: [.github/workflows/publish-nuget.yml](.github/workflows/publish-nuget.yml)
- Versioning rules: [GitVersion.yml](GitVersion.yml)
- NuGet target feed: `https://api.nuget.org/v3/index.json`

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
2. Create and push a tag for the target release version.

```bash
git checkout main
git pull

git tag vX.Y.Z
git push origin vX.Y.Z
```

3. In GitHub, open **Actions** -> **Publish NuGet Package**.
4. Click **Run workflow**.
5. Select the release tag as the workflow ref (for example `v1.5.0`).
6. Run the workflow.
7. Verify in the workflow summary:
   - SemVer
   - NuGet version
8. Verify package published on nuget.org.

## GitHub Release (Recommended)

After the package is published:

1. Open **Releases** in GitHub.
2. Create a release from the same tag (`vX.Y.Z`).
3. Add release notes (breaking changes, new features, fixes).
4. Publish the release.

## Quick Checklists

### Patch Release

1. Confirm fix merged.
2. Tag `vA.B.(C+1)`.
3. Run publish workflow on the tag.
4. Create GitHub Release.

### Minor Release

1. Confirm feature set merged.
2. Tag `vA.(B+1).0`.
3. Run publish workflow on the tag.
4. Create GitHub Release.

### Major Release

1. Confirm breaking changes are documented.
2. Tag `v(A+1).0.0`.
3. Run publish workflow on the tag.
4. Create GitHub Release with migration notes.

## Troubleshooting

- Wrong version generated:
  - Confirm workflow was run against the correct ref (tag, not branch).
  - Confirm tag format is exactly `vX.Y.Z`.
- Publish fails with auth error:
  - Recreate/rotate `NUGET_API_KEY` secret in GitHub.
- Package already exists:
  - Publishing uses `--skip-duplicate`; verify whether this is expected.
