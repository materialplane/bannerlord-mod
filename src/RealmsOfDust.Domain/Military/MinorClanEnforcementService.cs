namespace RealmsOfDust.Domain.Military;

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;

/// <summary>
/// Enforces active runtime size limits, prisoner caps, and single-warband restrictions
/// on noble realms and minor factions throughout the post-plague recovery period.
/// </summary>
public class MinorClanEnforcementService
{
    /// <summary>
    /// Evaluates and enforces seasonal party size, prisoner caps, and warband counts on a lord party.
    /// Safely trims any existing over-bloated AI rosters and secondary minor clan warbands.
    /// </summary>
    /// <param name="party">The mobile party to evaluate.</param>
    public void EnforceDailyLordPartyLimits(MobileParty party)
    {
        if (party == null || !party.IsActive || !party.IsLordParty)
        {
            return;
        }

        // Never delete troops automatically from the player's party
        if (party.ActualClan == Clan.PlayerClan)
        {
            return;
        }

        Clan? clan = party.ActualClan;
        int seasonIndex = CampaignSeasonHelper.GetElapsedSeasonIndex();

        // 1. Single-warband mandate for minor clans during Year 1 recovery (seasons 0-3)
        if (clan != null && clan.IsMinorFaction && seasonIndex < 4 && clan.Leader != null)
        {
            MobileParty? leaderParty = clan.Leader.PartyBelongedTo;
            if (leaderParty != null && party != leaderParty)
            {
                DestroyPartyAction.Apply(null, party);
                return;
            }
        }

        // 2. Enforce seasonal party member cap on AI field armies
        int maxPartyCap = PostPlaguePartySizeLimitModel.GetPostPlaguePartySizeCap(seasonIndex);
        TroopRoster? memberRoster = party.MemberRoster;
        if (maxPartyCap > 0 && memberRoster != null && memberRoster.TotalManCount > maxPartyCap)
        {
            int excessMembers = memberRoster.TotalManCount - maxPartyCap;
            int availableRegulars = memberRoster.TotalRegulars;
            int toRemove = Math.Min(excessMembers, availableRegulars);

            if (toRemove > 0)
            {
                memberRoster.RemoveNumberOfNonHeroTroopsRandomly(toRemove);
            }
        }

        // 3. Enforce seasonal prisoner headcount cap
        int maxPrisonerCap = PostPlaguePartySizeLimitModel.GetPostPlaguePrisonerCap(seasonIndex);
        TroopRoster? prisonRoster = party.PrisonRoster;
        if (maxPrisonerCap > 0 && prisonRoster != null && prisonRoster.TotalManCount > maxPrisonerCap)
        {
            int excessPrisoners = prisonRoster.TotalManCount - maxPrisonerCap;
            int availablePrisoners = prisonRoster.TotalRegulars;
            int toRemovePrisoners = Math.Min(excessPrisoners, availablePrisoners);

            if (toRemovePrisoners > 0)
            {
                prisonRoster.RemoveNumberOfNonHeroTroopsRandomly(toRemovePrisoners);
            }
        }
    }

    /// <summary>
    /// Backwards-compatible alias for minor clan daily limits.
    /// </summary>
    public void EnforceDailyMinorClanLimits(MobileParty party) => EnforceDailyLordPartyLimits(party);
}
