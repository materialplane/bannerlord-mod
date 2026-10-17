namespace RealmsOfDust.Core.Interfaces;

using TaleWorlds.CampaignSystem.Party;

/// <summary>
/// Service contract for enforcing the early-game outlaw suppression grace period.
/// </summary>
public interface IBanditGraceService
{
    /// <summary>
    /// Gets whether the campaign is currently within the active bandit grace period.
    /// </summary>
    bool IsGracePeriodActive { get; }

    /// <summary>
    /// Evaluates and enforces party size constraints on newly spawned bandit parties.
    /// </summary>
    /// <param name="party">The newly created or updated mobile party.</param>
    void EnforcePartyLimits(MobileParty party);
}
