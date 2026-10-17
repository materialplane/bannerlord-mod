namespace RealmsOfDust.Core.Constants;

/// <summary>
/// Central constants and tuning baselines for Realms of Dust.
/// All values mirror the official wiki documentation.
/// </summary>
public static class RealmsOfDustConstants
{
    // --- Economic Baselines (Post-Plague Depletion) ---
    public const int DefaultStartingDenars = 2500;
    public const float DefaultStartingInfluence = 50f;
    public const float DefaultStartingLoyalty = 70f;
    public const float DefaultStartingSecurity = 60f;
    public const float TownProsperityBaseline = 1500f;
    public const float CastleProsperityBaseline = 800f;
    public const float VillageHearthsBaseline = 150f;
    public const int FoodStocksDaySupply = 30;

    // --- Garrison Headcounts ---
    public const int TownGarrisonTotal = 20;
    public const int TownGarrisonInfantry = 10;
    public const int TownGarrisonRanged = 10;

    public const int CastleGarrisonTotal = 15;
    public const int CastleGarrisonInfantry = 8;
    public const int CastleGarrisonRanged = 7;

    public const int FrontierGarrisonTotal = 15;

    // --- Field Parties & Clan Tiers ---
    public const int LordStartingRetinueCount = 8;
    public const int PlayerStartingClanTier = 2;
    public const int AiStartingClanTier = 3;

    // --- Domain Sizing ---
    public const int VillagesPerDomain = 4;
    public const int VillagesPerTown = 2;
    public const int VillagesPerCastle = 2;

    // --- Grace Tuning ---
    public const int BanditGracePeriodDays = 14;
    public const int BanditMaxPartySize = 12;

    // --- Post-Plague Depleted Manpower & Tax Recovery ---
    public const int SeasonLengthDays = 21;
    public const int EarlyMaxVolunteerIndex = 2; // Only first 2 slots available early
}
