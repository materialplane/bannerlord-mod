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

## [Unreleased] — In Development

### Planned for v1.1.0
* **Difficulty-Scaled Starting Retinue:** Seed player party on Day 1 with cultural levies and veterans (Tier 1–3) to prevent instant looter ambushes upon spawning.
* **Bandit Horde Mobility:** Inject pack/sumpter mounts into merged hordes to prevent heavy loot over-encumbrance and sluggish campaign map crawling.
* **Settlement Realm Guards (Patrols):** Enable Guard House projects for Castles and synchronize patrol sizes with seasonal horde caps.
