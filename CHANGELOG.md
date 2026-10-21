# Changelog & Patch Notes — Realms of Dust

All notable changes, gameplay balance updates, and bug fixes for **Realms of Dust** are documented here.
This project follows [Semantic Versioning](https://semver.org/) (`vMAJOR.MINOR.PATCH`).

---

## [v1.0.0] — 2026-10-19 (Initial Steam Workshop Release)

### Core Mechanics & World Balance
* **Ground Zero Reset:** 
  * All initial party rosters, garrisons, and militias across Calradia wiped on campaign start.
  * Lords and monarchs start as solo commanders on horseback.
* **Micro-Realm Allocation:** 
  * Every realm begins on identical footing with exactly 1 Capital (Town), 1 Stronghold (Castle), and equal bound village distribution.
* **Bandit Overhaul & Pre-Activated Hideouts:**
  * All hideouts across the continent pre-activated on Day 1.
  * Roaming bandit parties periodically merge into larger warbands (+30% density).
  * Seasonal horde caps established (Summer: 20, Autumn: 30, Winter: 45, Spring: 60).
* **Groundwork & Compatibility:**
  * Clean multi-tier .NET 6 / `netstandard2.0` architecture.
  * Native TaleWorlds campaign persistence (`CampaignBehaviorBase.SyncData`).
  * Singleplayer launcher category tag configured in `SubModule.xml`.

---

## [v1.1.0] — 2026-10-19

### Gameplay & Balance Updates
* **Difficulty-Scaled Starting Retinue (Anti-Ambush):**
  * Solved Day-1 spawn ambushes by granting the player an immediate cultural escort retinue.
  * **Realistic (Hardest / Bannerlord):** 10x Tier-2 Shielded Infantry (holds tight shield wall against pebble volleys).
  * **Easy (Medium / Warrior):** 6x Tier-1, 3x Tier-2, 1x Tier-3.
  * **Very Easy (Easiest / Freebooter):** 4x Tier-1, 4x Tier-2, 4x Tier-3.
* **Early Horde Balancing:**
  * Capped Season 0 (Summer / Days 1–21) merged bandit hordes to a maximum of 15 troops to ensure fair early-game scaling.

---

## [v1.2.0] — 2026-10-19

### Gameplay & Balance Updates
* **2-Phase Seasonal Bandit Horde Merging:**
  * Summer (Days 1–10: Cap 15, Days 11–21: Cap 30).
  * Autumn (Days 22–31: Cap 30, Days 32–42: Cap 40).
  * Winter (Days 43–52: Cap 45, Days 53–63: Cap 55).
  * Spring / Year 2+ (Days 64–73: Cap 65, Days 74+: Cap 75).
  * Second-half pressure ensures roaming brigand stacks maintain tactical challenge ahead of the player cap.

---

## [v1.3.1] — 2026-10-20

### Gameplay & Balance Updates
* **Culture-Strict Domain Assignment:**
  * AI clan assignment now strictly prioritizes noble clans matching the capital's culture, preventing mismatched rulers from suffering instant -3 daily loyalty culture shock.
* **Post-Plague Reconstruction Accord (Settlement Loyalty):**
  * Added `PostPlagueSettlementLoyaltyModel` to buffer town loyalty during Year 1 (Days 1–84).
  * Softens cross-culture loyalty penalty by +2.0 and provides a +1.0 civic stabilization buffer for capitals recovering from depleted garrisons and early food shortages.

---

## [v1.3.0] — 2026-10-19

### Gameplay & Balance Updates
* **Difficulty-Aware Personal Skill Party Size Unlocks:**
  * **Realistic (Hardest / Bannerlord):** Party size is strictly clamped to 25 during Season 0 (Summer). Personal bonuses from Steward skill and combat/leadership perks unlock starting in Season 1 (Autumn 35-man bump), stacking directly on top of the seasonal baseline.
  * **Easy / Very Easy (Warrior / Freebooter):** Personal Steward and perk bonuses are unlocked from Day 1, allowing early army expansion beyond the baseline 25.
* **Year 1 Sovereign Bonus Suppression:**
  * The +20 King/Sovereign faction leader bonus is suppressed during Year 1 (Days 1–84) for all realms to reflect post-plague crown rebuilding.
  * Sovereign kingdom bonuses fully unlock in Year 2+ (Days 85+).

---

## [v1.4.0] — 2026-10-20

### Gameplay & Balance Updates
* **Bandit Horde Mobility & Pack Mount Logistics:**
  * Merged bandit hordes (20+ troops) now automatically receive cheap pack mounts (1 per 5 bandits) to carry captured loot and prisoners without getting weighed down.
  * Absorbs all donor party inventory and mounts during mergers so loot isn't lost to the void.
* **Brigand Mobility Speed Floor:**
  * Added `PostPlaguePartySpeedModel` to guarantee merged hordes never drop below a 3.6 map speed floor.
  * Stops massive 40–75 man infantry coalitions from crawling at an immobile 2.2 pace, while keeping them slow enough for healthy caravans (4.6–5.2) and player scouting parties to pull away.

---

## [v1.5.0] — 2026-10-21

### Gameplay & Balance Updates
* **Rogue Deserter Companies & Season 0 Grace Period:**
  * Deserters are completely suppressed during Season 0 (Summer / Days 1–21) to give players and AI realms breathing room to rebuild and clear starter bandit hideouts without getting wiped by rogue soldiers.
  * Deserter companies become active starting in Season 1 (Autumn / Day 22+).
* **Anti-Doomstack Merge Rules:**
  * Only small splinter bands (25 or fewer troops) are eligible to merge into another party.
  * Once a deserter company reaches 40+ troops, they become locked and stop absorbing smaller bands, ensuring the map always has a healthy variety of small and large targets.
  * Enforces a strict 60-man hard cap on all deserter companies (overriding vanilla's 120-man blob).
* **Tactical Outlaws & Military Loot:**
  * Deserters fight in disciplined shield walls and spear braces, dropping legitimate military armor, weapons, and recruitable Tier 3–4 veterans.
  * Zero diplomatic consequences, war declarations, or clan feuds from fighting them.

---

## [Unreleased] — In Development

### Next in the Chamber
* **Settlement Realm Guards (Patrols):** Enable Guard House projects for Castles and synchronize patrol sizes with seasonal horde caps.
