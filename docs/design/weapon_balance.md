# Weapon Balance & Early Game Hardcore Progression

This document outlines the design philosophy, data analysis, and mathematical justification for the early-game weapon rebalancing implemented in this mod. 

## 🎯 Design Philosophy & Goals
The core objective of this mod is to **increase the difficulty of Valheim's early game (Meadows and Black Forest)** without altering enemy stats. Altering enemy health or damage scales poorly into later biomes and destroys the vanilla progression curve. Instead, by tuning starting weapons, we achieve the following:

1. **Eliminate Early Game Power Trips:** Prevent players from effortlessly "one-shotting" early passive wildlife (Boars/Deer) in open combat using standard low-tier blunt clubs.
2. **Encourage Tactical Combat:** Force players to manage stamina carefully, use stealth modifiers, and utilize the correct damage types.
3. **Preserve Utility Efficiency:** Nerf combat damage while keeping tool efficiency intact (e.g., the Stone Axe still chops trees at normal speed).

---

## ⚔️ Vanilla Baseline: Weapons vs. Enemies

To understand the modifications, we must analyze Valheim's vanilla health-to-damage ratios. The table below outlines baseline enemy Health Points (HP) alongside Quality Level 1 weapon stats.

| Category | Creature / Weapon (`Prefab`) | Base HP | Damage (Quality Level 1) | Damage Type / Trait |
| :--- | :--- | :---: | :---: | :--- |
| **WEAPON** | **Torch** (`Torch`) | — | **19** | Blunt (4) + Fire (15) |
| **WEAPON** | **Club** (`Club`) | — | **12** | Blunt |
| **WEAPON** | **Flint Knife** (`KnifeFlint`) | — | **12** | Slash/Pierce (4x Backstab) |
| **WEAPON** | **Stone Axe** (`AxeStone`) | — | **15** | Slash (Chopping damage) |
| **WEAPON** | **Flint Spear** (`SpearFlint`) | — | **20** | Pierce |
| **WEAPON** | **Crude Bow** (`BowCrude`) | — | **22** | Pierce (+ Arrow's base damage) |
| 🟩 **MEADOWS** | **Neck** | **5** | — | Vulnerable to Fire |
| 🟩 **MEADOWS** | **Deer** | **10** | — | Flees immediately when startled |
| 🟩 **MEADOWS** | **Boar** | **10** | — | Frightened by Fire |
| 🟩 **MEADOWS** | **Greyling** | **20** | — | Vulnerable to Fire |
| 🟩 **MEADOWS** | **Eikthyr (Boss)** | **500** | — | First biome boss |
| 🌲 **BLACK FOREST** | **Greydwarf** | **40** | — | Vulnerable to Fire |
| 🌲 **BLACK FOREST** | **Skeleton** | **40** | — | Weak to Blunt / Resistant to Pierce |
| 🌲 **BLACK FOREST** | **Greydwarf Shaman** | **60** | — | Heals other nearby enemies |
| 🌲 **BLACK FOREST** | **Ghost** | **60** | — | Resistant to Blunt/Slash/Pierce |
| 🌲 **BLACK FOREST** | **Rancid Remains** | **100** | — | Poisonous skeleton / Weak to Blunt |
| 🌲 **BLACK FOREST** | **Greydwarf Brute** | **150** | — | Heavy hitting, high health Greydwarf |
| 🌲 **BLACK FOREST** | **Troll** | **600** | — | Weak to Pierce / Resistant to Blunt |
| 🌲 **BLACK FOREST** | **Bear** | **800** | — | Vulnerable to Fire / Resistant to Blunt/Pierce/Frost |
| 🌲 **BLACK FOREST** | **The Elder (Boss)** | **2500** | — | Black Forest boss / Vulnerable to Fire |

---

## 📊 Proposed Weapon Balance (Tier 1 Nerfs)

To implement a meaningful challenge, the tier 1 weapons have been downscaled linearly. This prevents players from skipping a nerfed weapon class to abuse another easily accessible early-game tool.

| Weapon | Vanilla Damage | Proposed Damage | Gameplay Impact & Balancing Intent |
| :--- | :---: | :---: | :--- |
| **Club** | 12 | **8** | Requires 2 hits to kill a Boar or Deer. Takes 5 hits to defeat a standard Greydwarf (instead of 4). |
| **Flint Knife** | 12 | **8** | Can still one-shot Boars and Deer via a *stealth attack* due to the 4x Backstab multiplier (8 x 4 = 32 damage), but requires 2 hits in open combat. |
| **Stone Axe** | 15 | **9** | Slightly stronger than the club, but still requires 2 hits for Boars and Deer. Encourages players to save flint for the spear rather than relying on the axe for early combat. |
| **Flint Spear** | 20 | **12** | Sits right at the threshold to barely one-shot a Boar or Deer with a clean hit. Still requires 4 well-aimed stabs to bring down a standard Greydwarf. |
| **Crude Bow** | 22 | **14** | Combined with a basic wood arrow (+11), the total damage lands at **25**. This keeps ranged hunting viable for deer but severely nerfs the DPS when cheesing early boss fights. |

---

## 🔍 Key Combat Shifts (Before vs. After)

By comparing the two tables, the actual gameplay progression alters dramatically:

* **Hunting (Boar & Deer):** In vanilla, a basic wooden club (12 Dmg) kills a 10 HP Boar in one swing. With this mod, the Club (8 Dmg) and Stone Axe (9 Dmg) leave the creature alive, giving it time to fight back or escape. Stealth (Knife) or resource investment (Spear/Bow) becomes the mandatory meta for efficient hunting.
* **Early Defenses (Greylings & Greydwarfs):** A Greydwarf (40 HP) can no longer be systematically staggered and beaten down quickly. Fighting a group of them with a nerfed Flint Spear (12 Dmg) means landing 4 precise strikes while actively dodging, elevating the tension of entering the Black Forest.
