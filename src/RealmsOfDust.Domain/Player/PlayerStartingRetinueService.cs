namespace RealmsOfDust.Domain.Player;

using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using RealmsOfDust.Core.Constants;

/// <summary>
/// Seeds the player's party with a difficulty-scaled cultural starter retinue on Day 1.
/// Prevents instant ambush wipes while honoring the player's difficulty setting.
/// </summary>
public static class PlayerStartingRetinueService
{
    /// <summary>
    /// Seeds the player's mobile party with basic levies and veteran anchors based on difficulty.
    /// </summary>
    /// <param name="playerParty">The player's mobile party.</param>
    public static void SeedPlayerRetinue(MobileParty? playerParty)
    {
        if (playerParty?.MemberRoster == null)
        {
            return;
        }

        TroopRoster roster = playerParty.MemberRoster;
        CultureObject? culture = playerParty.ActualClan?.Culture ?? Hero.MainHero?.Culture;
        CharacterObject? basicTroop = playerParty.ActualClan?.BasicTroop ?? culture?.BasicTroop;

        if (basicTroop == null)
        {
            return;
        }

        // Determine cultural troop progression
        CharacterObject? tier2Troop = ResolveUpgradeTarget(basicTroop, 2);
        CharacterObject? tier3Troop = tier2Troop != null ? ResolveUpgradeTarget(tier2Troop, 3) : null;

        // Fall back gracefully if higher tiers are missing
        tier2Troop ??= basicTroop;
        tier3Troop ??= tier2Troop;

        var (t1Count, t2Count, t3Count) = ResolveCountsByDifficulty();

        if (t1Count > 0)
        {
            roster.AddToCounts(basicTroop, t1Count, false, 0, 0, true, -1);
        }
        if (t2Count > 0)
        {
            roster.AddToCounts(tier2Troop, t2Count, false, 0, 0, true, -1);
        }
        if (t3Count > 0)
        {
            roster.AddToCounts(tier3Troop, t3Count, false, 0, 0, true, -1);
        }
    }

    private static (int t1, int t2, int t3) ResolveCountsByDifficulty()
    {
        CampaignOptions.Difficulty difficulty = CampaignOptions.PlayerTroopsReceivedDamage;

        return difficulty switch
        {
            CampaignOptions.Difficulty.VeryEasy => (
                RealmsOfDustConstants.RetinueEasyT1Count,
                RealmsOfDustConstants.RetinueEasyT2Count,
                RealmsOfDustConstants.RetinueEasyT3Count),

            CampaignOptions.Difficulty.Easy => (
                RealmsOfDustConstants.RetinueMediumT1Count,
                RealmsOfDustConstants.RetinueMediumT2Count,
                RealmsOfDustConstants.RetinueMediumT3Count),

            _ => (
                RealmsOfDustConstants.RetinueHardT1Count,
                RealmsOfDustConstants.RetinueHardT2Count,
                0)
        };
    }

    private static CharacterObject? ResolveUpgradeTarget(CharacterObject baseTroop, int desiredTier)
    {
        if (baseTroop.UpgradeTargets == null || baseTroop.UpgradeTargets.Length == 0)
        {
            return null;
        }

        CharacterObject? fallback = null;
        foreach (CharacterObject target in baseTroop.UpgradeTargets)
        {
            if (target != null && target.Tier == desiredTier)
            {
                // Prioritize infantry with shields over skirmishers/archers for the frontline
                if (target.IsInfantry)
                {
                    return target;
                }
                fallback ??= target;
            }
        }

        return fallback ?? baseTroop.UpgradeTargets[0];
    }
}
