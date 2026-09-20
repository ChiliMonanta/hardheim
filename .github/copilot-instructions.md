# GitHub Copilot Instructions - HardHeim

## Project Context
*   **Project:** HardHeim is a hardcore Valheim mod built with BepInEx (5.4) and Jötunn (2.30).
*   **Target:** Managed .NET Framework net462 assembly running inside Mono runtime.
*   **Mod Code Location:** All mod-specific source code and the `.csproj` are located strictly inside the `/hardheim` directory.
*   **Dev Environment:** Running in a Linux Dev Container. The tool `ilspycmd` is globally installed and available in the container for decompiling Unity/Valheim assemblies if needed.

## Documentation Rules
For user-visible features or configuration changes, update the documentation in the same change:

1.  **Main Documentation (`/README.md`):** 
    *   Document the behavior and configuration options with technical precision.
2.  **Thunderstore Package (`/hardheim/thunderstore/README.md`):**
    *   Add a brief end-user description.
3.  **Thunderstore Changelog (`/hardheim/thunderstore/CHANGELOG.md`):**
    *   Add a concise bullet under `## v0.0.DEV`; do not create a new version unless explicitly asked.

Skip documentation for internal refactors, build changes, and minor fixes unless they change user-facing behavior.

## Copilot Efficiency Constraints
*   Keep responses and diffs concise; avoid boilerplate and unrelated refactors.
*   Prefer existing Jötunn and BepInEx APIs and local project patterns.
*   Do not guess Valheim/Unity APIs. Inspect the assemblies with `ilspycmd` when needed.
