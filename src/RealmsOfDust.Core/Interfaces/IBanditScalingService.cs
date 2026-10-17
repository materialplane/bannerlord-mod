namespace RealmsOfDust.Core.Interfaces;

using TaleWorlds.CampaignSystem.Party;
using RealmsOfDust.Core.Models;

/// <summary>
/// Service contract for dynamically scaling outlaw and bandit parties across campaign seasons.
/// </summary>
public interface IBanditScalingService
{
    /// <summary>
    /// Gets whether the initial early-game grace window is currently active.
    /// </summary>
    bool IsGracePeriodActive { get; }

    /// <summary>
    /// Gets the current target min/max party size range based on elapsed campaign time.
    /// </summary>
    BanditScaleRange GetCurrentTargetRange();

    /// <summary>
    /// Scales and balances a bandit party according to the current campaign season.
    /// </summary>
    /// <param name="party">The mobile bandit party to scale.</param>
    void ScaleBanditParty(MobileParty party);
}
