# Realms of Dust — Engineering Roadmap

This document outlines the phased, modular implementation plan for *Realms of Dust*. Each phase is completely isolated, testable, and strictly adheres to the AAA engineering mandates in [`AGENTS.md`](../AGENTS.md) (under 200 lines per file, clean separation of concerns, and zero magic numbers).

---

## Phase Index

| Phase | Specification Document | Description | Status |
| :--- | :--- | :--- | :--- |
| **01** | [`01_PROJECT_SCAFFOLDING.md`](./01_PROJECT_SCAFFOLDING.md) | Module directory structure, `.csproj` targeting .NET 6, `SubModule.xml`, and entry point wiring. | Complete |
| **02** | [`02_CORE_CONFIG_AND_INTERFACES.md`](./02_CORE_CONFIG_AND_INTERFACES.md) | Pure domain models, constants, configuration models, and service interfaces. | Complete |
| **03** | [`03_MAP_ALLOCATION_SERVICE.md`](./03_MAP_ALLOCATION_SERVICE.md) | Proximity-based pairing algorithm (Towns, Castles, 4 Villages, Frontier Strongholds) with War Sails scaling. | Complete |
| **04** | [`04_FACTION_ELEVATION_SERVICE.md`](./04_FACTION_ELEVATION_SERVICE.md) | Elevating 52 noble clans to kingdoms, setting Tier 3, 50 influence, 10k denars, and Day 1 peace. | Complete |
| **05** | [`05_PLAYER_REALM_INITIALIZER.md`](./05_PLAYER_REALM_INITIALIZER.md) | Culture-based randomized starting capital, Tier 2 elevation, domain grant, and initial positioning. | Complete |
| **06** | [`06_MILITARY_AND_GARRISON_RESET.md`](./06_MILITARY_AND_GARRISON_RESET.md) | 8-recruit lord retinue wipe, 20/15-guard skeleton garrisons, 70/60 stability, and 14-day bandit grace. | Complete |
| **07** | [`07_CAMPAIGN_BEHAVIOR_AND_PERSISTENCE.md`](./07_CAMPAIGN_BEHAVIOR_AND_PERSISTENCE.md) | Lifecycle hooks (`OnNewGameCreated`), `SaveableTypeDefiner`, and end-to-end integration verification. | Complete |

---

## Execution Rule
* We will execute **one phase at a time**.
* Each phase must compile cleanly and pass verification before proceeding to the next.
