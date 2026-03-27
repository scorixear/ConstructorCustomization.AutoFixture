---
name: Pre-Release Package Review
description: "Use when: preparing for a NuGet package release, validating package health, versioning, dependencies, metadata, and CI/CD before publishing."
argument-hint: "Base branch or diff target (default: main)"
agent: "NuGet Package Maintainer"
---

You are about to perform a package maintenance or release operation for the ConstructorCustomization.AutoFixture NuGet package.

Follow this guided checklist for repository health, release, and maintenance:

1. **Repository Health**
    - Confirm all intended changes are merged to `main`
    - Ensure CI is green (all workflows pass)
    - Check that all dependencies are up to date in `Directory.Packages.props`
    - Run `dotnet restore` and `dotnet test` to verify build and test health

2. **Versioning**
    - Determine the correct SemVer bump (patch, minor, major) based on the change type
    - Ensure version is NOT set manually in any project file; it must flow from GitVersion
    - For major releases, ensure breaking changes and migration notes are documented

3. **Package Metadata**
    - Confirm all metadata (`Description`, `PackageTags`, `Authors`, etc.) is in `Directory.Build.props`
    - Ensure no metadata is set in `.csproj` files

4. **Multi-Targeting & Polyfills**
    - Verify target frameworks: `net8.0`, `net10.0`, `netstandard2.1`
    - Ensure any new APIs used are compatible with all targets, or polyfilled as needed

5. **Examples Projects**
    - Ensure all example projects restore from the local `.nupkg`, not nuget.org
    - Use `scripts/rebuild-package-and-examples.ps1` to refresh examples

6. **Release Process**
    - Walk through the full checklist in `release-process.md`
    - For release: create a new GitHub Release with tag `vX.Y.Z` targeting `main`
    - Publishing the release triggers the `publish-nuget.yml` workflow
    - Monitor the workflow and confirm the package appears on nuget.org

7. **CI/CD & Workflow**
    - Confirm `publish-nuget.yml` and `pr-build-test.yml` are present and unmodified in required ways
    - Surface and investigate any CI/CD failures

Return a summary with:

- Blockers (must fix before proceeding)
- Warnings (should fix)
- Passed checks
- A short release/maintenance-readiness summary

If you need to perform a specific maintenance task (e.g., dependency update, metadata change), specify it as the argument.
