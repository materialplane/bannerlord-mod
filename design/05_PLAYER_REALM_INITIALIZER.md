# Phase 05: Player Realm Initializer

## 1. Objective
Establish the player's custom sovereign realm upon campaign initialization, randomly select their starting capital based on their chosen character creation culture, and grant their full domain assets.

---

## 2. Key Components
1. **`CultureCapitalPicker.cs`**:
   * Queries all Capitals matching `CharacterCreationManager.PlayerCulture`.
   * Randomly selects one capital using a deterministic campaign seed.
2. **`PlayerRealmInitializer.cs`**:
   * Elevates player clan to sovereign Kingdom status on Day 1.
   * Sets player clan to Tier 2 (`Clan.PlayerClan.Tier = 2`).
   * Grants 10,000 denars to player treasury and 50 influence.
   * Grants ownership of the selected Capital, its paired Castle, and 4 bound villages.
3. **`PlayerPartyPositioner.cs`**:
   * Teleports `MobileParty.MainParty` directly outside the assigned starting capital.

---

## 3. Verification Criteria
* Player begins as ruler of a Tier 2 kingdom.
* Assigned Capital matches character creation culture, randomized across available culture towns.
* Player owns 1 Town, 1 Castle, and 4 Villages.
* All files stay under 200 lines.
