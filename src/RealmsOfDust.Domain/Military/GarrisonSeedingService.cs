namespace RealmsOfDust.Domain.Military;

using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using RealmsOfDust.Core.Constants;
using RealmsOfDust.Core.Models;

/// <summary>
/// Seeds standardized defensive garrisons in Towns, Castles, and Frontier Strongholds.
/// </summary>
public class GarrisonSeedingService
{
    /// <summary>
    /// Seeds garrisons across all allocated realm domains and neutral frontier strongholds.
    /// </summary>
    /// <param name="allocation">The map allocation result containing realms and frontier holds.</param>
    public void SeedAllGarrisons(MapAllocationResult allocation)
    {
        if (allocation == null)
        {
            return;
        }

        // 1. Seed realm capitals (Towns) and strongholds (Castles)
        foreach (RealmDomain realm in allocation.Domains)
        {
            if (realm.Capital != null)
            {
                SeedTownGarrison(realm.Capital);
            }

            if (realm.Stronghold != null)
            {
                SeedCastleGarrison(realm.Stronghold);
            }
        }

        // 2. Seed neutral frontier strongholds
        foreach (FrontierHold hold in allocation.FrontierHolds)
        {
            if (hold.Stronghold != null)
            {
                SeedFrontierGarrison(hold.Stronghold);
            }
        }
    }

    private void SeedTownGarrison(Settlement settlement)
    {
        MobileParty? garrison = EnsureGarrisonParty(settlement);
        if (garrison == null)
        {
            return;
        }

        TroopRoster roster = garrison.MemberRoster;
        roster.Clear();
        garrison.PrisonRoster?.Clear();

        CultureObject culture = settlement.Culture;
        (CharacterObject? melee, CharacterObject? ranged) = ResolveGarrisonTroops(culture);

        if (melee != null)
        {
            roster.AddToCounts(melee, RealmsOfDustConstants.TownGarrisonInfantry, false, 0, 0, true, -1);
        }

        if (ranged != null)
        {
            roster.AddToCounts(ranged, RealmsOfDustConstants.TownGarrisonRanged, false, 0, 0, true, -1);
        }
    }

    private void SeedCastleGarrison(Settlement settlement)
    {
        MobileParty? garrison = EnsureGarrisonParty(settlement);
        if (garrison == null)
        {
            return;
        }

        TroopRoster roster = garrison.MemberRoster;
        roster.Clear();
        garrison.PrisonRoster?.Clear();

        CultureObject culture = settlement.Culture;
        (CharacterObject? melee, CharacterObject? ranged) = ResolveGarrisonTroops(culture);

        if (melee != null)
        {
            roster.AddToCounts(melee, RealmsOfDustConstants.CastleGarrisonInfantry, false, 0, 0, true, -1);
        }

        if (ranged != null)
        {
            roster.AddToCounts(ranged, RealmsOfDustConstants.CastleGarrisonRanged, false, 0, 0, true, -1);
        }
    }

    private void SeedFrontierGarrison(Settlement settlement)
    {
        MobileParty? garrison = EnsureGarrisonParty(settlement);
        if (garrison == null)
        {
            return;
        }

        TroopRoster roster = garrison.MemberRoster;
        roster.Clear();
        garrison.PrisonRoster?.Clear();

        CultureObject culture = settlement.Culture;
        CharacterObject? basicTroop = culture?.BasicTroop;

        if (basicTroop != null)
        {
            roster.AddToCounts(basicTroop, RealmsOfDustConstants.FrontierGarrisonTotal, false, 0, 0, true, -1);
        }
    }

    private MobileParty? EnsureGarrisonParty(Settlement settlement)
    {
        MobileParty? garrison = settlement.Town?.GarrisonParty;
        if (garrison == null)
        {
            string partyId = "garrison_rod_" + settlement.StringId;
            garrison = GarrisonPartyComponent.CreateGarrisonParty(partyId, settlement);
        }

        return garrison;
    }

    private (CharacterObject? melee, CharacterObject? ranged) ResolveGarrisonTroops(CultureObject culture)
    {
        CharacterObject? basic = culture?.BasicTroop;
        if (basic == null)
        {
            return (null, null);
        }

        CharacterObject melee = basic;
        CharacterObject ranged = basic;

        // Inspect upgrade targets for basic troop to branch into ranged/melee
        if (basic.UpgradeTargets != null && basic.UpgradeTargets.Length > 0)
        {
            foreach (CharacterObject target in basic.UpgradeTargets)
            {
                if (target.IsRanged)
                {
                    ranged = target;
                }
                else if (target.IsInfantry)
                {
                    melee = target;
                }
            }
        }

        return (melee, ranged);
    }
}
