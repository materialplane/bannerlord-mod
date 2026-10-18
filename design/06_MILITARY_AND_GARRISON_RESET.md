# Phase 06: Military Roster & Garrison Reset Service

## 1. Objective
Execute the Day 1 military wipe: reset mobile parties down to an 8-recruit retinue, populate fortifications with standardized skeleton garrisons, initialize civic security/loyalty, and enforce the 14-day bandit suppression grace.

---

## 2. Key Components
1. **`PartyRosterResetService.cs`**:
   * Clears `MobileParty.MemberRoster` and `PrisonRoster` for all lord parties and the player.
   * Adds exactly 8 Tier-1 culture-specific recruits (`CharacterObject.Culture.BasicTroop`).
   * Stations secondary adult clan members inside the capital, disabling extra warbands.
2. **`GarrisonSeedingService.cs`**:
   * Sets Town garrisons to 20 Tier-1 guards (10 melee, 10 ranged).
   * Sets Castle garrisons to 15 Tier-1 guards (8 melee, 7 ranged).
   * Sets Frontier Strongholds to 15 Tier-1 neutral guards.
3. **`SettlementCivicInitializer.cs`**:
   * Sets 70 Loyalty (`Town.Loyalty = 70f`) and 60 Security (`Town.Security = 60f`).
   * Seeds 30-day food granary stocks and tier 1 walls.
4. **`BanditGraceHandler.cs`**:
   * Intercepts bandit spawn party sizes, capping them at 8–12 troops for in-game Days 1–14.

---

## 3. Verification Criteria
* All field parties and settlement garrisons match wiki numbers exactly.
* Towns have 70 Loyalty, 60 Security, and 30-day food reserves.
* All files stay under 200 lines.
