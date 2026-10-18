# Phase 02: Core Config, Constants & Service Interfaces

## 1. Objective
Define the strongly-typed configuration models, constants classes, and domain service interfaces in `RealmsOfDust.Core`. Eliminates all magic numbers from the codebase.

---

## 2. Key Components
1. **`RealmsOfDustConstants.cs`**:
   * `DefaultStartingDenars = 10000`
   * `DefaultStartingInfluence = 50`
   * `DefaultStartingLoyalty = 70f`
   * `DefaultStartingSecurity = 60f`
   * `TownProsperityBaseline = 3000f`
   * `CastleProsperityBaseline = 1200f`
   * `VillageHearthsBaseline = 300f`
   * `TownGarrisonCount = 20` (10 melee, 10 ranged)
   * `CastleGarrisonCount = 15` (8 melee, 7 ranged)
   * `FrontierGarrisonCount = 15`
   * `LordStartingRetinueCount = 8`
   * `PlayerStartingClanTier = 2`
   * `AiStartingClanTier = 3`
   * `BanditGracePeriodDays = 14`
   * `BanditMaxPartySize = 12`
2. **Data Models / DTOs**:
   * `RealmDomain.cs`: Record of Town, Castle, and bound Villages list.
   * `FrontierHold.cs`: Record of unaligned Castle and its bound Villages.
3. **Domain Interfaces**:
   * `ISettlementAllocationService.cs`: Generates balanced domains.
   * `IFactionElevationService.cs`: Manages kingdom creation.
   * `IPlayerRealmInitializer.cs`: Handles player realm selection.
   * `IMilitaryResetService.cs`: Wipes armies and sets garrisons.

---

## 3. Verification Criteria
* All configuration models mirror the exact stats on the wiki.
* Every class strictly adheres to the Single Responsibility Principle and stays under 200 lines.
