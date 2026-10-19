# AGENTS.md — Realms of Dust (Bannerlord Overhaul & IP Hub)

## 0. Crash Course for AI Agents (Read This First)

Welcome to the **Realms of Dust** ecosystem. The developer is managing two distinct but connected projects under this IP:
1. **The Commercial Standalone Game (The Primary Asset):** A tactical, 2D top-down, turn-based 4-lane WeGo mercenary RPG (inspired by classic mercenary grit, 4-man squad management, threat meters, and visor duel mini-games).
   * **Full Design Bible:** Read `design/09_STANDALONE_GAME_SPEC.txt` immediately to understand this game.
2. **The Bannerlord Overhaul Mod (The Community & Conquest Funnel):** An overhaul mod on Steam Workshop (ID: `3822706398`) for *Mount & Blade II: Bannerlord* (v1.2.x+ / .NET 6).
   * **Its Purpose:** Fixes the unyielding 6-faction blob by fracturing Calradia into 20+ equal micro-realms (1 Town, 1 Castle, equal villages, 0 starting troops).
   * **Active To-Do & Roadmap:** Read `design/08_DIPLOMACY_INTEGRATION.txt` for the current feature pipeline.
   * **Version & Patch Notes:** Read `CHANGELOG.md` and check `src/Directory.Build.props`.

### Release & Testing Protocol
* **Quick-Test Features (e.g. Starting Retinues):** Verified directly by the dev in minutes and shipped to Steam.
* **Long-Play Systems (e.g. Diplomacy, Truces, Patrols):** Must NOT be pushed untested to Steam. They are released as pre-release ZIP builds to the Discord community (`discord-bot/`) for multi-week testing.
* **Steam Workshop Status:** Never touch or push files via SteamCMD while an item is in Steam's moderation queue.

---

## 1. Project Overview & Core Mod Vision

**Mod Name:** Realms of Dust  
**Target Platform:** *Mount & Blade II: Bannerlord* (v1.2.x+ / .NET 6)  
**Parent Franchise / Universe:** *Realms of Dust* (See `design/09_STANDALONE_GAME_SPEC.txt`)  

### Core Gameplay Rules:
1. **Equal Starting Domain:** 1 Town (Capital), 1 Castle (Stronghold), equal bound villages.
2. **Ground Zero Reset:** 0 starting troops for all lords and settlements on Day 1.
3. **Bandit Ecology:** Day 1 pre-activated hideouts, roaming hordes with merge logic and seasonal caps.

---

## 2. Professional AAA Engineering Mandates

All code must strictly adhere to these architectural rules:

### A. The 200-Line Code File Mandate
* **Strict Size Limit:** No single source code file (`.cs`) may exceed **200 lines of code**.
* **Single Responsibility Principle (SRP):** Split files proactively before hitting 150 lines. No "God Classes."

### B. Relational & Native Persistence
* Never use loose untyped flat files for campaign state.
* Use TaleWorlds' native relational save system (`CampaignBehaviorBase.SyncData`, `SaveableTypeDefiner`, `[SaveableField]`).

### C. Clean Architecture
1. **RealmsOfDust.Core:** Pure models, interfaces, constants, configs.
2. **RealmsOfDust.Domain:** Allocation algorithms, reset services, military logic.
3. **RealmsOfDust.Engine:** Campaign behaviors, TaleWorlds event hooks.
4. **RealmsOfDust.Main:** SubModule.xml, entry point, dependency wiring.

### D. Defensive Coding & Quality
* Always null-check dynamic campaign entities (`hero?.Clan`, `settlement?.Town`).
* No magic strings or numbers; all tuning values reside in `RealmsOfDustConstants` or config models.
* XML summary documentation on all public classes and interfaces.

---

## 3. Standard Directory Layout & Key File Map

```
bannerlord-mod/
├── AGENTS.md                  # This master crash course & engineering mandate
├── CHANGELOG.md               # Semantic version history and public patch notes
├── README.md                  # Project overview
├── design/
│   ├── 08_DIPLOMACY_INTEGRATION.txt # Active Feature Roadmap & To-Do Checklist
│   └── 09_STANDALONE_GAME_SPEC.txt  # Standalone RPG Bible (Combat, World, Vision)
├── discord-bot/               # Community bot & beta testing infrastructure
├── src/
│   ├── Directory.Build.props  # Centralized build configuration & Version tag
│   ├── RealmsOfDust.Core/     # Pure models, interfaces, constants
│   ├── RealmsOfDust.Domain/   # Allocation algorithms, reset services, logic
│   ├── RealmsOfDust.Engine/   # Campaign behaviors, TaleWorlds hooks
│   └── RealmsOfDust.Main/     # SubModule.xml, entry point, dependency wiring
└── steamcmd/                  # Workshop publishing scripts (upload.bat, item.vdf)
```

---

## 4. Workflow Rules for Autonomous Agents

1. **Orientation First:** Check `CHANGELOG.md` and `design/08_DIPLOMACY_INTEGRATION.txt` before touching any code.
2. **Review File Limits:** Never allow any `.cs` file to exceed 200 lines. Proactively refactor and split.
3. **Verify Builds Cleanly:** Run `dotnet build` from `src/` to confirm zero errors or broken references.
4. **Git Commit & Release Guard (MANDATORY):** Direct `git push` or arbitrary manual commits are strictly forbidden. The ONLY permitted way to commit and push releases is by executing `ship.bat <VERSION> "<MESSAGE>"`. The script enforces version number bumps across `Directory.Build.props` and `SubModule.xml`, runs clean builds, deploys, and pushes automatically.
5. **Chat Conciseness Mandate:** The chat interface is strictly for short, direct summaries. Never output unsolicited walls of text. Detailed design documentation belongs in `design/`.
6. **Human Gamer Tone (CRITICAL):** When writing dev logs, Discord messages, or player-facing notes, NEVER use overly formal or robotic AI buzzwords (e.g. "retinue", "ecosystem", "mitigate", "paradigm", "testbed"). Write casually like an indie gamer dev sharing thoughts with friends (e.g. "starter troops", "getting wrecked by throwing axes", "nerfed early bandit stacks").


