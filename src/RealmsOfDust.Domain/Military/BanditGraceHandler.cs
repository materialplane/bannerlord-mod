namespace RealmsOfDust.Domain.Military;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using RealmsOfDust.Core.Constants;
using RealmsOfDust.Core.Interfaces;

/// <summary>
/// Suppresses oversized bandit spawns during the first 14 days of the campaign,
/// ensuring early caravans and micro warbands are not overwhelmed.
/// </summary>
public class BanditGraceHandler : IBanditGraceService
{
    private CampaignTime _graceExpiryTime;
    private bool _initialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="BanditGraceHandler"/> class.
    /// Defers time calculations until campaign map time tracking is active.
    /// </summary>
    public BanditGraceHandler()
    {
    }

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
                if (!_initialized)
                {
                    _graceExpiryTime = CampaignTime.DaysFromNow(RealmsOfDustConstants.BanditGracePeriodDays);
                    _initialized = true;
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
    public void EnforcePartyLimits(MobileParty party)
    {
        if (party == null || !party.IsBandit || !IsGracePeriodActive)
        {
            return;
        }

        TroopRoster memberRoster = party.MemberRoster;
        if (memberRoster == null)
        {
            return;
        }

        int totalTroops = memberRoster.TotalManCount;
        int excess = totalTroops - RealmsOfDustConstants.BanditMaxPartySize;

        if (excess > 0)
        {
            memberRoster.RemoveNumberOfNonHeroTroopsRandomly(excess);
        }
    }
}
