# Phase 3 · Project 1: Housekeeping Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make the build incremental and offline-capable, centralize project settings and package versions, turn on analyzers, trimming checks and warnings-as-errors, update the Tailwind npm packages, and fix lifecycle logging levels. No public API or visual change.

**Architecture:**
- One shared MSBuild targets file replaces the four always-on `npm install` + `npm run buildcss` PostBuild targets with an incremental target that runs before the build.
- `src/Directory.Build.props` holds the library settings that are duplicated in three csproj files today.
- `Directory.Packages.props` holds every package version.
- Logging changes are limited to `LoggerExtensions` and one AutoComplete line.

**Tech Stack:** .NET 10 SDK, MSBuild, npm, Tailwind CSS 4, bUnit/xUnit v3.

**Spec:** `docs/superpowers/specs/2026-11-11-phase3-reorganization-design.md`, section "Project 1: Housekeeping".

## Global Constraints

- No public API change and no visual change. This project can ship alone as 0.3.0 (`+semver: minor`).
- `.github/workflows/publish-nuget.yml` stays unchanged and must keep working: `dotnet restore`, `dotnet build`, `dotnet test`, `dotnet pack` on ubuntu, with node available.
- Every `min.css` keeps every class selector it has today (Task 6 checks this).
- Package versions stay as they are today; Task 4 only moves them.
- Settings move into `src/Directory.Build.props`, not the repo root. Tests and samples keep their own settings, so warnings-as-errors and package metadata reach only `src/` projects. *(Ruling against the spec's "repo root": a root props file would also apply them to tests and samples.)*
- Test command: `dotnet test --project tests/Maviray.Blazor.Components.Material.Tests/Maviray.Blazor.Components.Material.Tests.csproj`, optionally with `-- --filter-class "*<Class>"`.
- One commit per task on `feature/ongoing-development`, ending with the `Co-Authored-By` trailer.

## Review Focus

- A fresh clone without `node_modules` must still build: `npm ci` runs once. Pinned in Task 2.
- A second build with no source changes must skip the CSS step. Pinned in Task 2.
- `dotnet build -p:SkipTailwindBuild=true` must succeed offline and without `node_modules`, using the committed CSS. Pinned in Task 2.
- Packed `.nupkg` files must keep their metadata, icon, readme, license and static web assets (`css/*.css`, `js/*.js`). Pinned in Task 3.
- A new NuGet vulnerability advisory must not break every build once warnings are errors; audit warnings stay warnings. Pinned in Task 5.

---

### Task 1: Lifecycle logs at Debug, no unconditional AutoComplete log

**Files:**
- Modify: `src/Maviray.Blazor.Components.Core/Extensions/LoggerExtensions.cs` (both `LogDebugLifeCycle` overloads)
- Modify: `src/Maviray.Blazor.Components.Material/Components/Inputs/MaviAutoComplete.razor` (the `Logger?.Information(...)` line in `PerformSearch`'s `finally`)
- Create: `tests/Maviray.Blazor.Components.Material.Tests/TestUtilities/CapturingLoggerProvider.cs`
- Test: `tests/Maviray.Blazor.Components.Material.Tests/Components/LoggingTests.cs`

**Interfaces:**
- Produces: `CapturingLoggerProvider : ILoggerProvider` (test utility) with `IReadOnlyList<(string Category, LogLevel Level, string Message)> Entries`. Register it with `Services.AddSingleton<ILoggerProvider>(provider)`.
- Both `LogDebugLifeCycle` overloads log at `LogLevel.Debug` and return early when `!logger.IsEnabled(LogLevel.Debug)`.

- [ ] **Step 1: Write the failing tests:**
  - `LifecycleLogging_WritesDebugEntries`:
    - register `MaviComponentOptions { EnableLifecycleLogging = true }` as `IMaviComponentOptions` (the last registration wins);
    - render `MaviInputString` bound to `string.Empty`;
    - expect at least one entry whose category starts with `Maviray.`, and every `Maviray.` entry at `LogLevel.Debug`.
  - `AutoCompleteSearch_WritesNoInformationLogs`:
    - default options;
    - render `MaviAutoComplete<string>` with `DebounceDelay = 0` and `OnSearch` returning `["apple"]`;
    - `InputAsync("a")`;
    - expect no `Maviray.` entry at `LogLevel.Information` or above.
- [ ] **Step 2: Run** `--filter-class "*LoggingTests"`. Expected: both fail. The first finds Information entries; the second finds "PerformSearch finally block…".
- [ ] **Step 3: Implement** the Debug level with the early return, and delete the AutoComplete line.
- [ ] **Step 4: Run the full suite.** Expected: all pass.
- [ ] **Step 5: Commit** `fix: log lifecycle traces at Debug; drop unconditional AutoComplete log`.

### Task 2: Incremental Tailwind build

**Files:**
- Create: `build/Tailwind.targets`
- Modify: the PostBuild target is removed from, and `<Import Project="…/build/Tailwind.targets" />` plus `TailwindCssOutput` are added to, each of:
  - `src/Maviray.Blazor.Components.Material/…csproj`
  - `src/Maviray.Blazor.Components.Tailwind/…csproj`
  - `samples/Maviray.Blazor.Components.Samples.Material/…csproj`
  - `samples/Maviray.Blazor.Components.Samples.Material.Client/…csproj`

**Interfaces (`build/Tailwind.targets`):**
- **Required property** `TailwindCssOutput`: the project's generated CSS, relative to the project. Today's values:
  - `wwwroot/css/maviray.material.min.css`
  - `wwwroot/css/maviray.tailwind.min.css`
  - the two sample apps' `wwwroot/css/maviray.showcase.*.min.css`

  Read the exact names from each `package.json` `buildcss` script.
- **Optional property** `SkipTailwindBuild`: `true` skips both targets.
- **Target `TailwindNpmInstall`:** runs `npm ci` when `node_modules` doesn't exist. It runs before `TailwindBuildCss`.
- **Target `TailwindBuildCss`:**
  - `BeforeTargets="BeforeBuild"`, so the CSS exists before static web assets are collected.
  - `Inputs`: the project's `**/*.razor`, `**/*.cs`, `**/*.cshtml`, `wwwroot/**/*.css` (excluding `$(TailwindCssOutput)`), `package.json` and `package-lock.json`, excluding `bin/`, `obj/` and `node_modules/`.
  - `Outputs`: `$(TailwindCssOutput)`.
  - Runs `npm run buildcss`, then `<Touch>`es the output so the next build sees it as up to date.
- Use forward-slash or MSBuild path functions, so the same file works on Windows and in the ubuntu CI.

- [ ] **Step 1: Record the baseline.** `dotnet build Maviray.Blazor.Components.slnx -v:n > before.log`, then `dotnet build … -v:n > before2.log`. Expected: `npm install` and `npm run buildcss` appear in both logs (they run on every build).
- [ ] **Step 2: Implement** `build/Tailwind.targets` and the four csproj edits.
- [ ] **Step 3: Verify incrementality.** Build twice with `-v:n`. Expected:
  - the first build runs `npm run buildcss` once per project;
  - the second shows `Skipping target "TailwindBuildCss" because all output files are up-to-date` for all four, and no npm command.
- [ ] **Step 4: Verify that a source change rebuilds the CSS.** Touch `src/Maviray.Blazor.Components.Material/Components/Inputs/MaviInputString.razor` and build. Expected: Material reruns `buildcss`; the projects whose own inputs didn't change skip it; `git status` shows no `min.css` change.
- [ ] **Step 5: Verify a fresh clone and offline use.**
  - Rename `src/Maviray.Blazor.Components.Material/node_modules` away and build. Expected: `npm ci` runs, then `buildcss`, and the build succeeds.
  - Then remove it again and build with `-p:SkipTailwindBuild=true`. Expected: success, no npm command, `min.css` unchanged.
  - Restore `node_modules` with `npm ci`.
- [ ] **Step 6: Run the full suite.** Expected: all pass, and `git status` shows only the intended files changed.
- [ ] **Step 7: Commit** `build: incremental Tailwind CSS target; npm ci only when node_modules is missing`.

### Task 3: Shared library settings in `src/Directory.Build.props`

**Files:**
- Create: `src/Directory.Build.props`
- Modify: the three `src/*/*.csproj` keep only `PackageId`, `Description`, `PackageTags`, their references and project-specific items. `AddRazorSupportForMvc` is removed.

**Interfaces:** `src/Directory.Build.props` defines, for all `src` projects:
- `TargetFramework` `net10.0`, `Nullable` and `ImplicitUsings` `enable`;
- `Version` `1.0.0`, as in the csproj files today; CI overrides it with `/p:Version` and `/p:PackageVersion`;
- `Authors`, `Company`, `Copyright`, `RepositoryUrl`, `PackageProjectUrl`, `RepositoryType`, `PackageLicenseFile`, `PackageReadmeFile`, `PackageIcon`, `GeneratePackageOnBuild` `false`, `IsPackable` `true`, `GenerateEmbeddedFilesManifest` `true`, with today's values;
- the `None` items packing `icon.png`, `README.md` and `LICENSE.txt` from `$(MSBuildThisFileDirectory)..\`.

- [ ] **Step 1: Record the baseline packages.**
  - `dotnet pack` each `src` project with `-o <scratch>/before -p:PackageVersion=0.0.0-check`.
  - Extract each `.nuspec` and the file list (`unzip -l`).
- [ ] **Step 2: Implement** the props file and trim the three csproj files.
- [ ] **Step 3: Pack again** into `<scratch>/after` and diff against the baseline. Expected:
  - The nuspec metadata, dependencies and file lists are identical.
  - The only allowed difference: a `Microsoft.AspNetCore.App` framework reference disappearing because of `AddRazorSupportForMvc`. Record it if it appears.
- [ ] **Step 4: Run the full suite and build the solution.** Expected: all pass, 0 warnings.
- [ ] **Step 5: Commit** `build: share library settings in src/Directory.Build.props`.

### Task 4: Central Package Management

**Files:**
- Create: `Directory.Packages.props` with `ManagePackageVersionsCentrally` `true` and one `PackageVersion` per package used anywhere in the solution, at today's versions.
- Modify: every `*.csproj` in `src/`, `tests/` and `samples/` drops `Version="…"` from its `PackageReference`s. `PrivateAssets` and `IncludeAssets` stay.

- [ ] **Step 1: Record the baseline.** `dotnet list Maviray.Blazor.Components.slnx package --include-transitive > before.txt`.
- [ ] **Step 2: Implement.**
- [ ] **Step 3: Verify.** `dotnet restore`, then `dotnet list … package --include-transitive > after.txt` and diff. Expected: identical resolved versions.
- [ ] **Step 4: Run the full suite and build the solution.** Expected: all pass.
- [ ] **Step 5: Commit** `build: central package management`.

### Task 5: Analyzers, warnings as errors, trimming checks

**Files:**
- Modify: `src/Directory.Build.props` (from Task 3) adds:
  - `AnalysisLevel` `latest`, with the default analysis mode;
  - `TreatWarningsAsErrors` `true`;
  - `WarningsNotAsErrors` `NU1901;NU1902;NU1903;NU1904`, so vulnerability-audit warnings stay warnings;
  - `IsTrimmable` `true`.

  Measured on 2026-11-11: the default mode and the trim analyzer report 0 warnings for Core and Material. If warnings appear, fix them in code; suppress only with a justification comment.

- [ ] **Step 1: Implement** the properties.
- [ ] **Step 2: Build the solution.** Expected: 0 warnings and 0 errors in `src/`.
- [ ] **Step 3: Check trimming with the real trimmer.**
  - `dotnet publish samples/Maviray.Blazor.Components.Samples.Material.Client -c Release -o <scratch>/publish`, then grep its output for `IL2` / `IL3` warnings that mention `Maviray`.
  - Expected: none. If any appear, annotate the code: `[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]` on `TableDataCollectionFactory.FromDataSource<T>`'s `T`; field access on enum types for display names. Re-publish until none remain.
- [ ] **Step 4: Prove warnings now fail the build.**
  - Temporarily add an unused local variable in `src/Maviray.Blazor.Components.Core/Utilities/CssManager.cs`. Expected: the build fails with CS0168 or CS0219 as an error.
  - Revert it, and build. Expected: success.
- [ ] **Step 5: Run the full suite.** Expected: all pass.
- [ ] **Step 6: Commit** `build: analyzers, warnings as errors and trimming checks for src`.

### Task 6: Update the Tailwind npm packages

**Files:**
- Modify: `package.json` and `package-lock.json` in the four projects from Task 2:
  - `tailwindcss` and `@tailwindcss/cli` `^4.3.5`
  - `@tailwindcss/typography` `^0.5.20`
  - `@tailwindcss/forms` stays at `^0.5.11`, already the latest.
- Modify: the four regenerated `*.min.css` files.
- Create (scratch, not committed): a script that lists every class selector in a CSS file.

- [ ] **Step 1: Record the baseline.** Save each project's current class-selector list (sorted, unique) to `<scratch>/<project>-before.txt`.
- [ ] **Step 2: Update.** Run `npm install tailwindcss@^4.3.5 @tailwindcss/cli@^4.3.5 @tailwindcss/typography@^0.5.20` in each project, then build the solution.
- [ ] **Step 3: Compare.** Produce `<scratch>/<project>-after.txt` and diff.
  - Expected: no selector removed. Additions are allowed and get recorded.
  - If any selector is missing, find out why: Tailwind changelog, or a class-detection change. Fix it with an `@source` or literal-class change, or stop and report.
- [ ] **Step 4: Run the full suite.** Expected: all pass.
- [ ] **Step 5: Hand the user a short visual check** in the sample app, since the generated CSS changed: one page each of inputs, buttons, dialog, table and the editor.
- [ ] **Step 6: Commit** `build: update Tailwind to 4.3.5 and typography to 0.5.20`.
