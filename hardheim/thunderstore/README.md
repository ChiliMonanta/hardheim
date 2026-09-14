# HardHeim

**Odin's world, just brutal.**

HardHeim is a Valheim difficulty mod built with BepInEx and Jötunn. It makes
progression more demanding through heavier copper ore and a custom death
penalty for skills.

## Features
- **Copper Ore Weight:** Configure the weight of copper ore on the server.
- **Custom Death Penalty:** Skills below level 50 use percentage-based loss;
	skills from level 50 use fixed reductions, with a smaller reduction at level
	60 and above.
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
