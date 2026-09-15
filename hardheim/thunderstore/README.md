# HardHeim

**Odin's world, just brutal.**

HardHeim is a Valheim difficulty mod built with BepInEx and Jötunn. It makes
progression more demanding through heavier copper ore, a custom death penalty
for skills, and dark crypts that require a handheld light source.

## Features
- **Copper Ore Weight:** Configure the weight of copper ore on the server.
- **Custom Death Penalty:** Skills below level 50 use percentage-based loss;
	skills from level 50 use fixed reductions, with a smaller reduction at level
	60 and above.
- **Dark Crypts and Caves:** Ambient lighting, environmental light, fog, and
	dungeon light sources are disabled indoors, making handheld lighting
	necessary for exploration.
- **Enhanced Handheld Lights:** Player torches and Dvergr lanterns have greater
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
BepInEx/config/com.valheim.heavyminer.cfg
```

The `CopperOre` value accepts values from `0.1` to `1000` and defaults to
`50`. Death penalty behavior is built in and is not currently configurable.
