# HardHeim

**Odin's world, just brutal.**

HardHeim is a Valheim difficulty mod built with BepInEx and Jötunn. It makes
progression more demanding through heavier copper ore, a custom death penalty
for skills, and dark crypts that require a handheld light source.

## Features
- **Copper Ore Weight:** Configure the weight of copper ore on the server.
- **Surtling Core Weight:** Configure the weight of Surtling Cores on the server (defaults to 150 kg).
- **Crypt Surtling Core Scarcity:** Burial Chambers contain at most 1 Surtling Core (configurable spawn chance, defaults to 30%).
- **Custom Death Penalty:** Skills below level 50 use percentage-based loss;
	skills from level 50 use fixed reductions, with a smaller reduction at level
	60 and above.
- **Dark Crypts and Caves:** Ambient lighting, environmental light, fog, and
	dungeon light sources are disabled indoors, making handheld lighting
	necessary for exploration.
- **Enhanced Handheld Lights:** Player torches and Dvergr lanterns have adjusted
	light intensity and range while inside dark interiors.
- **Lighting Note:** Camera zoom can affect the perceived brightness and range
	of handheld lights because Valheim adjusts light detail based on distance.
- **Server Synchronization:** Configuration is synchronized to clients, and
	every player must have the mod installed.

## Installation
The easiest way to install HardHeim is using a mod manager like **r2modman** or the **Thunderstore Mod Manager**. 

### Manual Installation
1. Ensure you have **BepInEx** and **Jötunn** installed.
2. Download the `HardHeim.zip` archive.
3. Extract the contents and move `HardHeim.dll` into your `Valheim/BepInEx/plugins/` folder.

## Configuration
After launching the game once, edit the generated configuration file:

```text
BepInEx/config/com.valheim.hardheim.cfg
```

- `Ore Weights -> CopperOre`: accepts `0.1` to `1000` (default `50`).
- `Ore Weights -> SurtlingCore`: accepts `0.1` to `1000` (default `150`).
- `Crypt Loot -> SurtlingCoreChance`: percent chance `0` to `100` (default `30`).

Death penalty behavior is built in and is not currently configurable.
