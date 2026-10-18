# Phase 03: Dynamic Map Allocation Service

## 1. Objective
Implement the runtime proximity-pairing algorithm in `RealmsOfDust.Domain`. Discovers all map settlements dynamically via TaleWorlds APIs, ensuring native compatibility with both base Calradia and expansion maps like *War Sails*.

---

## 2. Key Components
1. **`ProximityCalculator.cs`**:
   * Euclidean distance calculations using `Settlement.GetPosition2D`.
   * Fast spatial nearest-neighbor sorting for fief pairing.
2. **`SettlementAllocationService.cs`**:
   * Queries `Settlement.All` dynamically.
   * Matches each Town to its closest unclaimed Castle.
   * Identifies surplus Castles as Frontier Strongholds (14 in vanilla).
   * Distributes exactly 4 bound villages per Town/Castle domain (2 to Town, 2 to Castle).
   * Assigns remaining villages (~48 in vanilla) to the Frontier Strongholds.
3. **`VillageRebinder.cs`**:
   * Sets `Village.Bound` to the designated parent Town or Castle.

---

## 3. Verification Criteria
* Handles dynamic town/castle counts without hardcoded IDs.
* Every settlement on the active map is accounted for with zero orphan settlements.
* Compiles cleanly under 200 lines per file.
