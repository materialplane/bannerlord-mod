namespace RealmsOfDust.Domain.Military;

using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using RealmsOfDust.Core.Constants;

/// <summary>
/// Resets mobile lord and player field armies to an 8-recruit retinue on Day 1.
/// Also stations excess adult clan members inside the capital to prevent extra warbands.
/// </summary>
public class PartyRosterResetService
{
    /// <summary>
    /// Resets all mobile parties belonging to noble clans and the player.
    /// </summary>
    public void ResetAllFieldParties()
    {
        // 1. Reset clan primary parties and station secondary members in capital
        foreach (Clan clan in Clan.All)
        {
            if (clan.IsEliminated || clan.IsBanditFaction || clan == Clan.PlayerClan)
            {
                continue;
            }

            ResetClanParties(clan);
        }

        // 2. Comprehensive sweep of all active lord parties currently on the map
        foreach (MobileParty lordParty in MobileParty.AllLordParties)
        {
            if (lordParty.ActualClan == Clan.PlayerClan || lordParty.ActualClan?.IsBanditFaction == true)
            {
                continue;
            }

            ResetSingleLordParty(lordParty);
        }

        // 3. Sweep all pre-existing bandit parties on Day 1
        BanditScalingService banditService = new BanditScalingService();
        foreach (MobileParty party in MobileParty.All)
        {
            if (party.IsBandit)
            {
                banditService.ScaleBanditParty(party);
            }
        }
    }

    /// <summary>
    /// Resets an individual mobile lord party to its starter retinue.
    /// </summary>
    public void ResetSingleLordParty(MobileParty party)
    {
        if (party == null || !party.IsLordParty)
        {
            return;
        }

        ResetPartyRoster(party, party.LeaderHero?.CharacterObject);
    }

    private void ResetClanParties(Clan clan)
    {
        Hero? leader = clan.Leader;
        MobileParty? primaryParty = clan.Leader?.PartyBelongedTo;

        // Reset the primary warband
        if (primaryParty != null && primaryParty.IsLordParty)
        {
            ResetPartyRoster(primaryParty, leader?.CharacterObject);
        }

        // Station secondary adult members in the capital if available
        Settlement? capital = clan.HomeSettlement;
        if (capital == null)
        {
            return;
        }

        foreach (Hero hero in clan.Heroes)
        {
            if (hero == leader || !hero.IsAlive || hero.IsChild)
            {
                continue;
            }

            MobileParty? separateParty = hero.PartyBelongedTo;
            if (hero.CurrentSettlement != capital)
            {
                TeleportHeroAction.ApplyImmediateTeleportToSettlement(hero, capital);
            }

            if (separateParty != null && separateParty != primaryParty)
            {
                DestroyPartyAction.Apply(null, separateParty);
            }
        }
    }

    private void ResetPartyRoster(MobileParty party, CharacterObject? leaderCharacter)
    {
        TroopRoster memberRoster = party.MemberRoster;
        TroopRoster prisonRoster = party.PrisonRoster;

        if (memberRoster == null)
        {
            return;
        }

        // 1. Wipe captured prisoners
        prisonRoster?.Clear();

        // 2. Clear all regular non-hero troops
        int regularCount = memberRoster.TotalRegulars;
        if (regularCount > 0)
        {
            memberRoster.RemoveNumberOfNonHeroTroopsRandomly(regularCount);
        }

        // 3. Ensure leader hero is in roster
        if (leaderCharacter != null && memberRoster.FindIndexOfTroop(leaderCharacter) < 0)
        {
            memberRoster.AddToCounts(leaderCharacter, 1, false, 0, 0, true, -1);
        }

        // 4. Seed exactly 8 culture-specific basic recruits
        CultureObject? culture = party.ActualClan?.Culture ?? leaderCharacter?.Culture;
        CharacterObject? basicTroop = culture?.BasicTroop;

        if (basicTroop != null)
        {
            memberRoster.AddToCounts(
                basicTroop,
                RealmsOfDustConstants.LordStartingRetinueCount,
                false,
                0,
                0,
                true,
                -1);
        }
    }
}
