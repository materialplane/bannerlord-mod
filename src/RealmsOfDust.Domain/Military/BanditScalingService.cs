namespace RealmsOfDust.Domain.Military;

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using RealmsOfDust.Core.Constants;
using RealmsOfDust.Core.Interfaces;
using RealmsOfDust.Core.Models;

/// <summary>
/// Scales bandit parties across campaign seasons, enforcing early-game bounds
/// and ramping to wide 15-50 warbands in later seasons.
/// </summary>
public class BanditScalingService : IBanditScalingService, IBanditGraceService
{
    private readonly Random _random = new Random();
    private CampaignTime _graceExpiryTime;
    private bool _graceInitialized;

    /// <inheritdoc />
    public bool IsGracePeriodActive
    {
        get
        {
            if (Campaign.Current == null)
            {
                return false;
            }

            try
            {
                if (!_graceInitialized)
                {
                    _graceExpiryTime = CampaignTime.DaysFromNow(RealmsOfDustConstants.BanditGracePeriodDays);
                    _graceInitialized = true;
                }

                return CampaignTime.Now < _graceExpiryTime;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <inheritdoc />
    public BanditScaleRange GetCurrentTargetRange()
    {
        int seasonIndex = CampaignSeasonHelper.GetElapsedSeasonIndex();
        return BanditScalingCalculator.CalculateRangeForSeason(seasonIndex);
    }

    /// <inheritdoc />
    public void ScaleBanditParty(MobileParty party)
    {
        if (party == null || !party.IsBandit || party.IsBanditBossParty)
        {
            return;
        }

        TroopRoster memberRoster = party.MemberRoster;
        if (memberRoster == null)
        {
            return;
        }

        BanditScaleRange range = GetCurrentTargetRange();
        int seasonIndex = CampaignSeasonHelper.GetElapsedSeasonIndex();
        int hordeCap = BanditHordeMergeService.GetHordeCapForSeason(seasonIndex);
        int currentCount = memberRoster.TotalManCount;

        if (currentCount > hordeCap)
        {
            int excess = currentCount - hordeCap;
            memberRoster.RemoveNumberOfNonHeroTroopsRandomly(excess);
        }
        else if (currentCount < range.MinCount)
        {
            int target = BanditScalingCalculator.PickTargetCount(range, _random);
            int needed = target - currentCount;
            if (needed > 0)
            {
                AddTroopsToParty(memberRoster, needed);
            }
        }
    }

    /// <inheritdoc />
    public void EnforcePartyLimits(MobileParty party)
    {
        ScaleBanditParty(party);
    }

    private void AddTroopsToParty(TroopRoster memberRoster, int countToAdd)
    {
        CharacterObject? templateTroop = null;
        foreach (var element in memberRoster.GetTroopRoster())
        {
            if (element.Character != null && !element.Character.IsHero)
            {
                templateTroop = element.Character;
                break;
            }
        }

        if (templateTroop != null)
        {
            memberRoster.AddToCounts(templateTroop, countToAdd, false, 0, 0, true, -1);
        }
    }
}
