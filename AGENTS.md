# AGENTS.md — ProphetsWay.Utilities

<!-- ═══════════════════════════════════════════════════════════════════════
     BEGIN SHARED BLOCK
     Generated from prophets-pipelines/conventions/AGENTS.shared.md
     DO NOT EDIT BY HAND — run /sync-agents-md to regenerate.
     ═══════════════════════════════════════════════════════════════════════ -->

## About This Codebase

`ProphetsWay.*` is a family of small, focused .NET libraries by G. Gordon Nasseri, published to
NuGet under the `ProphetsWay.` prefix and hosted at `github.com/ProphetManX`. Each library lives
in its own repository with its own version line, changelog, and pipeline.

### The Two Families

| Family | Repos | Purpose |
|---|---|---|
| **Utility** | Utilities, Logger, Hasher | Standalone helpers with no dependency on each other |
| **Data Access** | BaseDataAccess, EFTools | A layered DAL-decoupling paradigm; EFTools implements BaseDataAccess |

`ProphetsWay.Example` is a reference implementation, not a published package.
`prophets-pipelines` holds shared Azure DevOps YAML templates and this conventions file.

## Naming

**Display vs. codified.** The organization name is written two ways, and the distinction matters:

- **Display name** — `Prophet's Way`, with the apostrophe. Used in `<Company>`, prose, README text, and anything a human reads.
- **Codified** — `ProphetsWay`, no apostrophe or space. Used in namespaces, package IDs, assembly names, repo names, and the Azure DevOps org.

| Thing | Rule | Example |
|---|---|---|
| Repository | `ProphetsWay.<Library>` | `ProphetsWay.Logger` |
| Package ID | matches repository | `ProphetsWay.Logger` |
| Assembly name | matches repository | `ProphetsWay.Logger` |
| Library project folder | matches repository | `ProphetsWay.Logger/` |
| Test project | `<Library>.Tests` — **plural** | `ProphetsWay.Logger.Tests` |
| Example project | `<Library>.Example` | `ProphetsWay.Logger.Example` |
| `<Company>` | display name | `Prophet's Way` |
| `<Authors>` | `G. Gordon Nasseri` | |
| `<Product>` | library name without prefix | `Logger` |

### Namespaces

The rule is **family-dependent**. Do not "correct" one family to match the other.

- **Utility family** shares one root namespace regardless of assembly name:
  `ProphetsWay.Utilities`, with sub-namespaces for areas (`ProphetsWay.Utilities.LoggerDestinations`).
  A consumer adds one `using ProphetsWay.Utilities;` and reaches every utility library.
  This is why `ProphetsWay.Logger.dll` declares `namespace ProphetsWay.Utilities` — intentional, not a bug.
- **Data Access family** uses per-library namespaces: `ProphetsWay.BaseDataAccess`, `ProphetsWay.EFTools`
  (plus key-type sub-namespaces `.Guid`, `.Int`, `.Long`). These are an architectural paradigm, not
  utilities, and are kept separately addressable.
- **Test projects always use their own namespace**, `<AssemblyName>.Tests` — never the shared root.

## Target Frameworks

```xml
<!-- default for a published library -->
<TargetFrameworks>netstandard2.0;net10.0</TargetFrameworks>

<!-- only when a framework-conditional dependency or API requires it -->
<TargetFrameworks>netstandard2.0;net48;net10.0</TargetFrameworks>

<!-- test projects — netstandard2.0 is not a valid test target -->
<TargetFrameworks>net48;net10.0</TargetFrameworks>
```

.NET ships every November: **even-numbered = LTS (3 years), odd-numbered = STS (18 months)**.
.NET 10 is the current LTS (Nov 2025 → ~Nov 2028). **.NET 8 and .NET 9 both go end of life on
10 November 2026** — `net8.0`/`net9.0` are now debt, as are `netcoreapp*`, `net5.0`–`net7.0`,
and anything below `net48`.

1. **LTS only.** Never target an STS release in a published library — an 18-month window means
   re-cutting the list every year and stranding someone each time. Never target a preview.
2. **`netstandard2.0` is permanent.** It is an API contract, not a runtime, so it cannot expire.
   It is the reach floor: consumable by .NET Framework 4.6.1+ (painless from 4.7.2 up) and by every
   .NET Core/5+ runtime. It is also the last .NET Standard version Framework supports —
   `netstandard2.1` deliberately excluded it.
3. **`net48` is conditional, not default.** `netstandard2.0` already reaches .NET Framework 4.8, so
   an explicit `net48` target earns its place only when the repo has a framework-conditional
   *dependency* or needs an API `netstandard2.0` does not expose. `ProphetsWay.EFTools` qualifies —
   its EF6 branch is keyed on `net4*`. Most repos do not. Justify it per repo.
   (.NET Framework 4.8 is the final Framework version; it ships as a Windows component and inherits
   the OS lifecycle, so it has no standalone EOL date.)
4. **Carry exactly one modern TFM** unless something concrete requires two. Every extra target
   multiplies build time.
5. **Test projects name runtimes directly**, since `netstandard2.0` cannot be a test target. A
   `net48` test target is how .NET Framework behavior is *verified*, which is distinct from a
   library merely *supporting* it: `Activator.CreateInstance<T>()` wraps a throwing constructor on
   .NET Framework and does not on .NET Core, so `ProphetsWay.Example.Tests` must keep `net48` or its
   exception-passthrough regression guard stops guarding anything.
6. **Canonical dotted monikers** — `net10.0`, never `net100`. The undotted form parses, but it is
   non-standard and inconsistent across the repos.
7. **`LangVersion`:** `netstandard2.0` defaults to C# 7.3, and that constraint applies to all shared
   code in a multi-targeted project. This is why nullable reference types do not work in these
   libraries regardless of what a csproj claims.
8. **Dropping `net8.0`/`net9.0` is not a breaking change** while `netstandard2.0` remains — those
   consumers still install and still resolve an asset.
9. **Adding a TFM is a MINOR bump, never a patch.** A new target silently repoints existing
   consumers to a *different assembly* — a .NET 10 consumer that resolved the `netstandard2.0` asset
   starts binding the `net10.0` one, a different compilation with different BCL bindings and no
   netstandard shims. A patch must be safe to take without reading the notes.

## Packaging Metadata

Required in every **published** library's `.csproj`. If a repo is not published to NuGet, these
are optional — but they become mandatory the moment publishing is on the table.

```xml
<PackageId>ProphetsWay.Thing</PackageId>
<Product>Thing</Product>
<Authors>G. Gordon Nasseri</Authors>
<Company>Prophet's Way</Company>
<Description>...</Description>
<RepositoryType>git</RepositoryType>
<RepositoryUrl>https://github.com/ProphetManX/ProphetsWay.Thing</RepositoryUrl>
<PackageLicenseExpression>MIT</PackageLicenseExpression>
<PackageRequireLicenseAcceptance>true</PackageRequireLicenseAcceptance>
<PackageIcon>profile.png</PackageIcon>
<PackageReadmeFile>README.md</PackageReadmeFile>
<PackageTags>...</PackageTags>
```

Paired with the item group that actually packs those files — declaring `PackageIcon` or
`PackageReadmeFile` without the matching `<None Pack="true">` packs nothing and fails the build:

```xml
<ItemGroup>
  <None Include="..\CHANGELOG.md" Link="CHANGELOG.md" Pack="true" PackagePath="" />
  <None Include="..\README.md" Link="README.md" Pack="true" PackagePath="" />
  <Content Include="..\profile.png" Link="profile.png" Pack="true" PackagePath="" />
</ItemGroup>
```

**An empty self-closing element is not a value.** `<PackageId />` silently falls back to
`AssemblyName` and leaves the nuget.org listing without a license, readme, or source link.
Treat empty stubs as missing.

Versioning is owned by the pipeline. Leave `<Version />`, `<AssemblyVersion />`,
`<FileVersion />`, and `<InformationalVersion />` empty in the csproj — `app-variables.yml`
supplies them at build time.

## Testing

- **xUnit** — the test framework. Do not introduce NUnit or MSTest.
- **Shouldly** — assertion style. Prefer `result.ShouldBe(...)` over `Assert.Equal`.
  FluentAssertions 8.x requires a paid commercial license; do not add it to any project.
- **coverlet.collector** — coverage.
- **Moq** — only where a test genuinely needs a mock; most of these libraries do not.
- Test class names mirror the type under test: `HasherTests`, `FileDestinationTests`.
- Tests requiring a local database set `LocalTestsOnly: 'yes'` in `app-variables.yml` so CI skips them.

## Pipelines

Every repo consumes the shared templates in `prophets-pipelines` via two root files:

| File | Purpose |
|---|---|
| `app-variables.yml` | Per-repo values — `Major`/`Minor`/`Patch`, `TargetProject`, `Product`, `RepoName`, `PostTargetToNuGet`, `LocalTestsOnly` |
| `local-pipeline.yml` | Thin wrapper pulling `prophets-pipelines` stage templates |

`Major`/`Minor`/`Patch` are bumped **by hand** in `app-variables.yml` as work proceeds. The
pipeline appends build metadata to produce alpha/beta/release packages.

## Repo Layout

```
ProphetsWay.Thing/
├─ AGENTS.md                 ← this file
├─ README.md                 ← packed into the nupkg
├─ CHANGELOG.md              ← packed into the nupkg
├─ LICENSE                   ← MIT
├─ profile.png               ← NuGet icon
├─ app-variables.yml
├─ local-pipeline.yml
├─ ProphetsWay.Thing.sln
├─ ProphetsWay.Thing/        ← library
├─ ProphetsWay.Thing.Tests/  ← xUnit
└─ docs/                     ← agent-generated analysis
  ├─ repo-profile.md
  ├─ purpose-and-scope.md
  ├─ nuget-extraction-proposal.md
  └─ feature-requests.md    ← durable request and decision index
```

These artifacts are generated by agents and committed. `feature-requests.md` becomes applicable once
the first request is captured; an empty repo need not carry a placeholder.

**`docs/architecture.md` and per-project `docs/requirements.md` are `n/a` for a utility or reference
library** — its architecture lives in `AGENTS.md`, the README, and XML `<remarks>`. They apply to
multi-project **application** solutions only. Do not report either as missing from a library repo.

## Solution Layout

For multi-project application solutions, the rule is **base name = contracts, suffix = swappable
implementation**. Business logic has one implementation and needs no split; a DAL has many and does.

| Project | Contains |
|---|---|
| `<Solution>.Core` | Domain models, business logic interfaces, and their implementation |
| `<Solution>.DataAccess` | DAL contracts only — interfaces and entities |
| `<Solution>.DataAccess.<Provider>` | One DAL implementation: `.MSSQL`, `.PostgreSQL`, `.MySQL`, `.NoDB`, `.EF` |
| `<Solution>.Database` | The `.sqlproj` database project |
| `<Solution>.Api` | Service endpoints |
| `<Solution>.Web` | Web UI |
| `<Solution>.Win` | Desktop UI |
| `<Project>.Tests` | xUnit tests for that project — `<Solution>.Core.Tests` |

**The suffix list is open.** A new provider or UI technology gets a new suffix following the same
shape (`.DataAccess.Cosmos`, `.Mobile`, `.Cli`). Do not invent a new *pattern* — extend this one.

A contracts project must never reference an implementation project, and must never expose a type
from a specific technology (`DbContext`, `SqlConnection`, `HttpContext`) in its public surface.
That rule is what makes the DAL swappable, and it is the whole point of the paradigm.

### Database Projects

New `.sqlproj` projects use the **`Microsoft.Build.Sql`** SDK — SDK-style, cross-platform, and
buildable with `dotnet build`:

```xml
<Project Sdk="Microsoft.Build.Sql/<version>">
```

The legacy SSDT format (`ToolsVersion="4.0"`, the 2003 MSBuild namespace, `TargetFrameworkVersion`,
plus `.dbmdl`/`.jfm` sidecar files) requires Visual Studio on Windows and cannot be built by the
.NET CLI. Existing legacy projects are **debt to migrate** — the `.sql` files carry over unchanged;
the project header and sidecars are what change.

## Code Style

- Tabs for indentation in `.csproj` and `.cs`.
- Braces on their own line (Allman).
- Interfaces prefixed `I`. Abstract bases prefixed `Base` or `Root`.
- Public API surface gets XML doc comments; internals do not need them.
- No `.editorconfig` exists yet — style is convention, not enforced. Match surrounding code.

## Rules for Agents

- **Never edit `.cs`, `.csproj`, `.sln`, or `.yml`** unless the human explicitly asks in that turn.
  Propose changes as fenced snippets labeled `PROPOSED — not applied`.
- **Exception — the TDD agents.** `Interface Architect`, `Test Designer`, `Implementer`, and
  `Refactorer` exist to write code; invoking one *is* the explicit ask. Each is restricted to one
  kind of file, and those restrictions are load-bearing:

  | Agent | May write |
  |---|---|
  | `Interface Architect` | Interfaces and their supporting types — never tests, never implementations |
  | `API Designer` | HTTP contracts and `docs/api/` — never implementations |
  | `Test Designer` | `*Tests.cs` only |
  | `Implementer` | Implementation `.cs` only — **never** a test file |
  | `Refactorer` | Implementation `.cs` only, behavior-preserving — **never** a test file |
  | `Modernizer` | `.csproj` / `.sqlproj` build and packaging config — never versions, never namespaces |
  | `Pipeline Engineer` | `.yml` / `.yaml` only — never versions, secrets, project files, or Markdown |
  | `Changelog Author` | `CHANGELOG.md` only |
  | `Threat Modeler`, `Security Reviewer` | `docs/security/` only — read-only on source |

  If an agent edits a test to make it pass, the workflow has failed. Report it rather than
  accepting the green build.
- **Never bump a version** in `app-variables.yml`. That is a human decision.
- **Never invent an Azure DevOps `definitionId`.** Badge URLs must be copied from a file that
  already exists in the repo. If one is missing, ask.
- **Feature requests are shared-capture, single-owner triage.** The owner or any agent may append a
  `Proposed` entry to `docs/feature-requests.md`, but must read the index first and extend an existing
  entry instead of duplicating it. Only `Purpose Refiner` may change status. Never delete or renumber
  entries; rejected requests remain with their reasoning, and new numbers increase monotonically.
- **A namespace change is a binary-breaking change.** Never make one casually; it requires a major
  version bump and a CHANGELOG entry.
- **Affirming an inherited claim is not verifying it.** Before restating any existing claim in a
  README, `AGENTS.md`, or doc as still accurate, open the artifact it describes. A claim that has
  survived several passes has been *copied* several times, not *checked* several times. Say which
  file you opened.
- **When a hygiene fix has a teaching cost, genericize rather than delete** — replace the
  machine-specific value, keep the artifact — and record the declined option in the
  `docs/feature-requests.md` entry so it is not re-proposed later as unfinished work.
- Respect the family split above. `ProphetsWay.EFTools` living outside `ProphetsWay.Utilities`
  is correct, not drift.
- Deviations from these conventions are listed per-repo below. They are known, not overlooked —
  do not re-report them as discoveries.

<!-- ═══════════════════════════════════════════════════════════════════════
     END SHARED BLOCK
     ═══════════════════════════════════════════════════════════════════════ -->

---

## This Repo

**Family:** Utility · **Published:** **probably yes, historically** — but not by anything in this repo.
`CHANGELOG.md` v1.0.0 calls itself "the first public release of the library," and `README.md` carries
Alpha / Beta / Release Azure DevOps *release-pipeline* badges. Neither proves a current nuget.org
listing, and no packaging step exists in-repo. Treat as published-and-abandoned until the owner says
otherwise.

A grab-bag of small standalone helpers accumulated over time. It owns the canonical
`ProphetsWay.Utilities` namespace that the rest of the Utility family shares.

> **This repo is the least maintained in the workspace.** It is the primary candidate for the
> purpose-refinement and NuGet-extraction workflow. Its own `<Description>` reads, in full,
> "A collection of small utility classes I've made over the years." — which is a symptom, not a
> description.

### Key Types

Four source files, four public types. All are `public static` extension-method holders; there are no
other public types in the library.

| Type | Role |
|---|---|
| `Duplicator` | Deep-copies an object by round-tripping it through `BinaryFormatter`. **One public member**, `DuplicateObjectSerial<T>` — the reflection-based `DuplicateObject<T>` is commented out in the source with a note that the owner is not confident in it |
| `Parser` | `string` → typed value conversion: `GetValue<T>` plus the dictionary shortcut `GetValueFromKey<T>`. Handles enums and `Nullable<>` |
| `Serializer` | Binary (`BinaryFormatter`) and XML serialize/deserialize, to and from streams, strings, and files |
| `Streamer` | `ReadToEnd` and `Streamify` — two members, byte array ⇄ stream |

These four have **no relationship to each other**. That is exactly the cohesion problem the
`Purpose Refiner` agent exists to evaluate — each may deserve its own package, or the set may
deserve a sharper unifying purpose.

### Layout

| Project | Role |
|---|---|
| `ProphetsWay.Utilities/` | The library |
| `ProphetsWay.Utilities.Tests/` | xUnit tests — four classes, one per library type: `DuplicatorTests`, `ParserTests`, `SerializerTests`, `StreamerTests`. See deviations 10 and 12 |

`Utilities.sln` contains exactly these two projects plus a Solution Items folder.

### Known Deviations

This repo has the most drift of any in the workspace. Do not treat these as new discoveries.

| # | Deviation | Severity |
|---|---|---|
| 1 | **All packaging metadata is empty stubs** — `<PackageId />`, `<RepositoryUrl />`, `<PackageLicenseUrl />`, `<PackageTags />`, `<Product />` are all self-closing with no value | **High.** If this is published, the nuget.org listing has no license, no readme, no icon, and no source link. `PackageId` silently falls back to `AssemblyName`. |
| 2 | **No pipeline file in the repo** — no `app-variables.yml`, no `local-pipeline.yml`, no `.yml` of any kind | **High.** *But CI is not simply absent:* `README.md` badges point at an Azure DevOps build definition named `Utilities CI` (`definitionId=10`) and a **classic** release pipeline (`definitionId=6`, environments 12/13/14 for Alpha/Beta/Release). Classic pipelines are defined in the portal, not in source — so the build config exists somewhere no agent can read or review. That is worse than nothing, not equivalent to it. |
| 3 | No `profile.png` | Medium. Every other library packs a NuGet icon. |
| 4 | `<Company>` is `ProphetsWay` — **and is then blanked** | Low → Medium. `<Company>ProphetsWay</Company>` should be the display form `Prophet's Way`, but see deviation 10: a later `<Company />` in the same `PropertyGroup` overrides it, so the effective value is empty. |
| 5 | No `<Product>` value | Low. `<Product />` is an empty stub. Should be `Utilities`. |
| 6 | `PackageReadmeFile` / CHANGELOG are not packed | Medium. Confirmed — the library csproj has **no `<ItemGroup>` at all**, and no `PackageReadmeFile` or `PackageIcon` property either. |
| 7 | TFMs are `net45;net451;net452;net46;net461;net471;netstandard2.0;netcoreapp2.2;netcoreapp2.1;netcoreapp2.0;` — **10 targets**, with a trailing semicolon and the `netcoreapp` entries in descending order | **High.** Every target except `netstandard2.0` is end-of-life. Newest target is `netcoreapp2.2`, retired in 2019. No `net8.0`/`net9.0`/`net10.0`. |
| 8 | `<LangVersion>7.1</LangVersion>` is pinned | Medium. Blocks nullable reference types, pattern matching improvements, records, and file-scoped namespaces. |
| 9 | csproj indentation is 2-space for elements and 4-space for properties inside `<PropertyGroup>` | Cosmetic. Convention is tabs; there are no tabs anywhere in either csproj. |
| 10 | **`<Authors>` and `<Company>` are each declared twice — the second declaration is an empty stub that wins** | **Medium, and non-obvious.** `<Authors>G. Gordon Nasseri</Authors>` and `<Company>ProphetsWay</Company>` appear first; the "Assembly Override Properties" comment block below re-declares `<Authors />` and `<Company />`. MSBuild takes the **last** assignment, so both resolve to empty. Reading the top of the file and stopping gives the wrong answer. |
| 11 | **`Serializer` and `Duplicator` are built on `BinaryFormatter`** | **High — this blocks the modernization work item.** `BinaryFormatter` is obsolete since .NET 5 (`SYSLIB0011`), throws by default in .NET 8, and is **removed outright in .NET 9+**. `SerializeAsByteArr<T>`, `DeserializeFromByteArr<T>`, and everything downstream of them — including `Duplicator`'s only public member — cannot be retargeted to a modern LTS as written. It is also an insecure-deserialization vector on untrusted input. Step 3 of *Suggested Order of Work* is therefore a rewrite, not a TFM edit. |
| 12 | **Test project is a single EOL TFM and uses `FluentAssert`, not `FluentAssertions`** | Medium. `ProphetsWay.Utilities.Tests.csproj` targets `netcoreapp2.2` alone — retired 2019 — so the suite exercises **none** of the library's ten targets on a supported runtime. Its assertion package is `FluentAssert` 1.0.0.11, a different package from FluentAssertions and unused anywhere else in the workspace. `Microsoft.NET.Test.Sdk` is 16.0.1, `xunit` 2.4.1, and there is no `coverlet.collector`. |

### Suggested Order of Work

1. Run `Purpose Refiner` first — decide what this library **is** before investing in packaging it.
2. Then fix packaging metadata and add a pipeline.
3. Then modernize TFMs and drop the `LangVersion` pin. **Read deviation 11 first** — `BinaryFormatter`
   is removed in .NET 9+, so this step is a rewrite of `Serializer`/`Duplicator`, not a TFM edit.
4. README last, once the purpose is settled.
