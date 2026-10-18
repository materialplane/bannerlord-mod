# Phase 04: Faction Elevation & World Sovereignty

## 1. Objective
Transform Calradia's noble clans into independent sovereign kingdoms, normalize AI noble stats, seed initial treasuries, establish mutual Day 1 peace, and preserve minor mercenary clans.

---

## 2. Key Components
1. **`FactionElevationService.cs`**:
   * Creates an independent `Kingdom` object for each ruling noble clan.
   * Promotes the clan leader to sovereign monarch.
   * Assigns settlement ownership via `ChangeOwnerOfSettlementAction.ApplyByDefault`.
2. **`ClanNormalizer.cs`**:
   * Normalizes AI ruling clans to Tier 3 (`Clan.Tier = 3`).
   * Seeds 10,000 denars into `Clan.Leader.Gold`.
   * Sets 50 influence (`Clan.Influence = 50f`).
   * Preserves minor clans (`clan.IsMinorFaction`) as unaligned sellswords.
3. **`DiplomacySeeder.cs`**:
   * Ensures all sovereign kingdoms start with no active war declarations (`MakePeaceAction.Apply`).

---

## 3. Verification Criteria
* Exactly 52 AI sovereign kingdoms created on vanilla map.
* Minor clans remain independent without fiefs.
* No war declarations active on Day 1.
* All classes strictly under 200 lines.
