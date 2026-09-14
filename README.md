# HardHeim

HardHeim is a Valheim mod built with BepInEx and Jötunn. Its purpose is to
make survival more demanding by changing gameplay rules that are normally
fixed in vanilla Valheim.

The current implementation focuses on mining progression and death penalties:
the weight of copper ore can be configured by the server, and skill loss on
death is adjusted for higher skill levels. The configuration is synchronized
to clients, and all players are required to have the mod installed.

Download and follow releases on [Thunderstore](https://thunderstore.io/c/valheim/p/Dudes/HardHeim/).

## Current Features

- Configurable copper ore weight.
- Custom death penalty scaling based on skill level.
- Server-side configuration synchronization through Jötunn.
- Network compatibility requiring the mod on every client.
- Configuration Manager support through the local development packages.

The default copper ore weight is `50`. The generated BepInEx configuration can
be changed after the first launch:

```text
BepInEx/config/com.valheim.heavyminer.cfg
```

The configuration value is named `CopperOre` and accepts values from `0.1` to
`1000`. Death penalty behavior is currently built in and has no separate
configuration option.

## Architecture

HardHeim is a managed .NET Framework `net462` assembly. It runs inside the
Mono runtime used by the Windows version of Valheim and is loaded by BepInEx.
The mod uses Jötunn for Valheim integration and configuration
synchronization.

The runtime dependency chain is:

```text
HardHeim
├── BepInEx 5.4.23.3
├── Jötunn 2.30.0
│   └── YamlDotNet and JotunnBuildTask dependencies
├── UnityEngine assemblies from the local Valheim installation
└── Valheim assemblies from valheim_Data/Managed
```

The build also creates the libraries needed to build Jötunn and BepInEx from
source. The important development-time chain is:

```text
Jötunn
└── JotunnBuildTask
		└── Mono.Cecil 0.10.4

BepInEx and Jötunn
├── HarmonyX 2.9.0
└── MonoMod.Utils / MonoMod.RuntimeDetour 22.1.29.1
		├── Mono.Cecil 0.10.4
		└── MonoMod.Common source compiled into the MonoMod assemblies
```

MonoMod.Common is not distributed as a separate assembly. Its source is
compiled into the relevant MonoMod outputs.

### Repository layout

```text
/
├── build.sh                 # Builds dependencies, mod, and deployment zips
├── setup.sh                 # Clones dependencies at pinned revisions
├── install-local.sh         # Installs selected zips into the local Valheim tree
├── hardheim/
│   ├── HardHeim.csproj      # Mod project
│   ├── HeavyMiner.cs        # Mod implementation
│   └── thunderstore/        # Thunderstore package metadata and documentation
├── dependencies/            # Local source checkouts and wrapper projects
├── .locals-packages/        # Locally produced NuGet packages (not committed)
└── dist/                    # Generated deployment packages (not committed)
```

The Valheim installation is expected at:

```text
dependencies/valheim-steam/
├── valheim_Data/Managed/    # Original Unity and Valheim assemblies
└── BepInEx/                 # Local test installation
```

Valheim assemblies are build inputs only. They are not replaced or modified
by the mod build. Jötunn's `publicized_assemblies` directory is a temporary
build artifact and is removed by `build.sh` after Jötunn has been built.

## Security and Supply Chain

The project is designed to keep third-party build inputs local and auditable:

- Third-party dependencies are cloned at pinned Git commit revisions by
	`setup.sh`.
- BepInEx, HarmonyX, MonoMod, Mono.Cecil, Jötunn, and Unity Doorstop are built
	locally from source.
- Prebuilt third-party DLLs are not required or committed for the build.
- The local NuGet source is `.locals-packages`, which is populated by the
	build. NuGet access is restricted by `nuget.config` to approved package
	families and explicitly allowed build/test packages.
- Valheim and Unity assemblies come from the locally mounted game installation;
	they are not downloaded by this repository.
- Generated binaries should be treated as build artifacts. Before a release,
	record their SHA-256 values and the Valheim Steam build/depot information
	used for the build.

Only install release packages from a source you trust. The mod executes inside
the Valheim/BepInEx process and therefore has the same privileges as the game
process. Review dependency source revisions and generated package contents
before distributing them.

## Local Development

### Prerequisites

The only local prerequisite is VS Code with the Dev Containers support. Open
the repository in the provided devcontainer; it includes the Linux
environment, .NET 8 SDK, Git, MinGW-w64, and the other build tools required by
the project. The devcontainer also mounts the local Valheim installation at
`dependencies/valheim-steam`.

Initialize the pinned source dependencies:

```bash
./setup.sh
```

The setup script checks out the revisions documented in
[`hardheim.spec`](hardheim.spec), including the MonoMod.Common submodule.

Build all local dependencies, the mod, Unity Doorstop, and deployment
packages:

```bash
./build.sh
```

The build produces packages in `dist/`, including:

```text
dist/
├── BepInEx-windows.zip
├── BepInEx-linux.zip
├── Jotunn.zip
├── ConfigurationManager.zip
└── HardHeim.zip
```

`HardHeim.zip` contains the mod DLL and the package metadata from
`hardheim/thunderstore/`. It does not contain the development dependency
source tree or the local Valheim assemblies.

## Local Deployment

`install-local.sh` extracts generated packages into the mounted Valheim
directory. For a complete Windows test installation:

```bash
./install-local.sh --all-windows
```

To install only HardHeim after rebuilding it:

```bash
./install-local.sh --hardheim
```

For a Linux test installation, use:

```bash
./install-local.sh --all-linux
```

The intended plugin layout is:

```text
dependencies/valheim-steam/BepInEx/plugins/
└── HardHeim.dll
```

The `HardHeim.zip` build artifact contains `HardHeim.dll` at the zip root,
which is the expected Thunderstore package layout. `install-local.sh` extracts
that package into `BepInEx/plugins/HardHeim` for local testing.

BepInEx and Jötunn are installed as separate packages. The game installation
and its original `valheim_Data/Managed` assemblies remain outside the release
package and are not modified by the HardHeim deployment step.

## Release Package

The Thunderstore package metadata is maintained in
`hardheim/thunderstore/manifest.json`. Its declared dependencies are:

- `denikson-BepInExPack_Valheim-5.4.2333`
- `ValheimModding-Jotunn-2.30.0`

For end users, BepInEx must already be installed in the Valheim directory.
Install the release package with a mod manager, or extract `HardHeim.zip` and
place `HardHeim.dll` in `BepInEx/plugins/`.
