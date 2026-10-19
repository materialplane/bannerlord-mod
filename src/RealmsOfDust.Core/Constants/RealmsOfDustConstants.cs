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

    // --- Universal Post-Plague Mobile Army & Prisoner Caps ---
    public const int PostPlaguePartyCapSeason0 = 25;
    public const int PostPlaguePartyCapSeason1 = 35;
    public const int PostPlaguePartyCapSeason2 = 50;
    public const int PostPlaguePartyCapSeason3 = 70;

    public const int PostPlaguePrisonerCapSeason0 = 10;
    public const int PostPlaguePrisonerCapSeason1 = 15;
    public const int PostPlaguePrisonerCapSeason2 = 25;
    public const int PostPlaguePrisonerCapSeason3 = 40;

    // Backwards-compatible aliases
    public const int MinorClanPartyCapSeason0 = PostPlaguePartyCapSeason0;
    public const int MinorClanPartyCapSeason1 = PostPlaguePartyCapSeason1;
    public const int MinorClanPartyCapSeason2 = PostPlaguePartyCapSeason2;
    public const int MinorClanPartyCapSeason3 = PostPlaguePartyCapSeason3;

    public const int MinorClanPrisonerCapSeason0 = PostPlaguePrisonerCapSeason0;
    public const int MinorClanPrisonerCapSeason1 = PostPlaguePrisonerCapSeason1;
    public const int MinorClanPrisonerCapSeason2 = PostPlaguePrisonerCapSeason2;
    public const int MinorClanPrisonerCapSeason3 = PostPlaguePrisonerCapSeason3;

    // --- Bandit Horde Merging (Brigand Coalitions) ---
    public const int BanditHordeCapSeason0 = 20; // Summer (Days 1-21) - Player cap: 25
    public const int BanditHordeCapSeason1 = 30; // Autumn (Days 22-42) - Player cap: 35
    public const int BanditHordeCapSeason2 = 45; // Winter (Days 43-63) - Player cap: 50
    public const int BanditHordeCapSeason3 = 60; // Spring / Year 2+ (Days 64+) - Player cap: 70
    public const float BanditMergeProximitySquared = 16f; // Within 4.0 map distance units
    public const float BanditMergeChancePerHour = 0.30f; // 30% chance to merge when within proximity
    public const float BanditLooterSpawnMultiplier = 1.30f; // +30% looter population cap
    public const int BanditHideoutRoamingCap = 4; // 4 roaming parties per hideout (up from vanilla 3)

    // --- Player Starting Retinue (Anti-Ambush & Difficulty Scaling) ---
    // Realistic (Hardest / Bannerlord Difficulty): 10x T2 (Shielded infantry line)
    public const int RetinueHardT1Count = 0;
    public const int RetinueHardT2Count = 10;

    // Easy (Medium / Warrior Difficulty): 6x T1, 3x T2, 1x T3
    public const int RetinueMediumT1Count = 6;
    public const int RetinueMediumT2Count = 3;
    public const int RetinueMediumT3Count = 1;

    // VeryEasy (Easiest / Freebooter Difficulty): 4x T1, 4x T2, 4x T3
    public const int RetinueEasyT1Count = 4;
    public const int RetinueEasyT2Count = 4;
    public const int RetinueEasyT3Count = 4;
}


