namespace RealmsOfDust.Core.Interfaces;

using System.Collections.Generic;
using RealmsOfDust.Core.Models;

/// <summary>
/// Service contract for resetting field armies, seeding skeleton garrisons,
/// and initializing civic stability across all fortifications.
/// </summary>
public interface IMilitaryResetService
{
    /// <summary>
    /// Resets all mobile lord and player parties to their 8-recruit starting retinue.
    /// </summary>
    void ResetFieldParties();

    /// <summary>
    /// Seeds standardized skeleton garrisons in all towns, castles, and frontier holds.
    /// </summary>
    /// <param name="allocation">The active map allocation result.</param>
    void SeedGarrisons(MapAllocationResult allocation);

    /// <summary>
    /// Resets an individual mobile lord party to its starter retinue.
    /// </summary>
    /// <param name="party">The mobile lord party to reset.</param>
    void ResetSingleLordParty(TaleWorlds.CampaignSystem.Party.MobileParty party);

    /// <summary>
    /// Sets initial loyalty, security, food reserves, and wall levels across all fortifications.
    /// </summary>
    void InitializeCivicBaselines();
}
