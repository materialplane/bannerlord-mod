# AGENTS.md — Realms of Dust (Bannerlord Overhaul)

## 1. Project Overview & Vision

**Mod Name:** Realms of Dust  
**Target Platform:** *Mount & Blade II: Bannerlord* (v1.2.x+ / .NET 6)  
**Parent Franchise / Universe:** *Realms of Dust*  

### Core Gameplay Concept
*Realms of Dust* transforms the continent of Calradia from massive historical empires into a fractured, cutthroat continent of balanced micro-realms. Every single realm—including the player's custom realm—starts on identical footing:

1. **Equal Starting Domain:**
   * **1 Capital (Town):** Major economic center, marketplace, and civic seat.
   * **1 Stronghold (Castle):** Fortified military garrison and defensive bastion.
   * **Equal Bound Villages:** Equal village allocation tied to food, recruit, and resource production.
2. **Zero Starting Troops (Ground Zero):**
   * All party rosters, garrisons, and initial militias start completely wiped (0 troops).
   * Lords and the player start as lone captains on horseback who must recruit, train, and supply their armies from scratch.
3. **The Conquest Objective:**
   * Build up warbands, secure trade and production lines, forge or break alliances, and wage total war until one realm unifies the map.

---

## 2. Professional AAA Engineering Mandates

To ensure long-term stability, maintainability, and clean expansion, all AI agents, engineers, and contributors must strictly adhere to the following architectural rules:

### A. The 200-Line Code File Mandate
* **Strict Size Limit:** No single source code file (`.cs`) should exceed **200 lines of code**.
* **Single Responsibility Principle (SRP):** Each class must do exactly one thing. If a file begins to grow beyond 150 lines, proactively refactor and extract helper services, models, validators, or handlers into dedicated files.
* **No "God Classes":** Never bundle initialization logic, UI handling, game algorithms, and state persistence into a single class. Split them into specialized domain components.

### B. Relational & Data-Driven Architecture
* **Structured, Normalized Data:** Avoid fragile, unindexed flat-file blobs (e.g., loose, ad-hoc JSON or untyped text files). 
* **Bannerlord Native Persistence:** State persistence must leverage TaleWorlds' native relational save system (`CampaignBehaviorBase.SyncData`, `SaveableTypeDefiner`, and `[SaveableField]`) so that all entity relationships (Settlements, Clans, Kingdoms, Parties) maintain referential integrity across save and load cycles.
* **External Storage (if applicable):** If auxiliary data or external telemetry is required, use structured, relational databases (e.g., SQLite or MariaDB) with formal schemas, foreign keys, and indexed queries.

### C. Clean Architecture & Separation of Concerns
Code must be organized into distinct functional layers:
1. **Core / Domain:** Pure data models, enums, value objects, and domain interfaces.
2. **Services / Business Logic:** Allocation algorithms, troop reset services, economy seeding, diplomatic handlers.
3. **Behaviors (`CampaignBehaviorBase`):** Event listeners that subscribe to TaleWorlds campaign lifecycle hooks (e.g., `OnNewGameCreated`, `OnSettlementOwnerChanged`) and delegate immediately to domain services.
4. **Patches (Harmony):** Surgical, minimal hooks used strictly when TaleWorlds provides no public API or event. Harmony patches must never contain business logic; they must call clean service methods.

### D. Code Quality, Defensive Programming & Null Safety
* **Defensive Null Checking:** Bannerlord's campaign state is dynamic; parties and heroes can be eliminated or invalidated at any tick. Always validate references (`hero?.Clan`, `settlement?.Town?.GarrisonParty`) defensively.
* **Clear Naming Conventions:** Use explicit, descriptive PascalCase for classes, interfaces (`ISettlementAllocator`), and methods. Avoid cryptic abbreviations.
* **No Magic Strings or Numbers:** All tunable constants (starting denars, grace period days, prosperity baselines) must reside in dedicated configuration models or constants classes.
* **Documentation & Comment Preservation:** Every public class and interface must include concise XML summary comments documenting its purpose.

---

## 3. Standard Directory Layout

```
bannerlord-mod/
├── AGENTS.md                  # This architectural mandate
├── README.md                  # Project overview and build instructions
├── docs/                      # Game design and player-facing specs
├── mediawiki/                 # Local documentation wiki (bannerlord-mod.localhost)
└── src/
    ├── RealmsOfDust.Core/     # Pure models, interfaces, constants
    ├── RealmsOfDust.Domain/   # Allocation algorithms, reset services, logic
    ├── RealmsOfDust.Engine/   # Campaign behaviors, TaleWorlds hooks
    └── RealmsOfDust.Main/     # SubModule.xml, entry point, dependency wiring
```

---

## 4. Workflow Rules for Autonomous Agents

1. **Plan First:** Align on the player-facing rule and math before writing code.
2. **Review File Limits:** Before creating or modifying a file, check line counts. Split files proactively.
3. **Verify Builds Cleanly:** Never leave broken references, unresolved namespaces, or unhandled exceptions.
4. **Preserve Integrity:** Keep all existing documentation and user preferences intact across iterations.
5. **Chat Conciseness Mandate:** The chat interface is strictly for very short summaries. Never output unsolicited walls of text or long design explanations. In-depth information belongs on the local wiki (`bannerlord-mod.localhost`); provide expanded details in chat only if explicitly requested by the user.
