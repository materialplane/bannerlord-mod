# Phase 07: Campaign Behavior & Persistence Integration

## 1. Objective
Bind all domain services to TaleWorlds campaign lifecycle events via a custom `CampaignBehaviorBase`, configure native savegame persistence with `SaveableTypeDefiner`, and perform end-to-end verification.

---

## 2. Key Components
1. **`RealmsOfDustCampaignBehavior.cs`**:
   * Inherits `CampaignBehaviorBase`.
   * Subscribes to `CampaignEvents.OnNewGameCreatedEvent`.
   * Delegates initialization sequence:
     1. Map Allocation Service (Phase 03)
     2. Faction Elevation Service (Phase 04)
     3. Player Realm Initializer (Phase 05)
     4. Military Reset Service (Phase 06)
2. **`RealmsOfDustSaveDefiner.cs`**:
   * Inherits `SaveableTypeDefiner`.
   * Enforces referential integrity across save/load cycles without loose flat files.
3. **`CampaignInitCoordinator.cs`**:
   * Ensures deterministic execution order and logs execution benchmarks to TaleWorlds debug output.

---

## 3. Verification Criteria
* Campaign behavior registers cleanly on game startup.
* New sandbox game boots successfully, executes initialization once, and saves state cleanly.
* Reloading a saved game preserves all custom kingdoms, fiefs, and party compositions.
* All files remain strictly under 200 lines.
