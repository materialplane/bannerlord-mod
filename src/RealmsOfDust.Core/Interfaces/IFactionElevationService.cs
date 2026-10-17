namespace RealmsOfDust.Core.Interfaces;

using System.Collections.Generic;
using RealmsOfDust.Core.Models;
using TaleWorlds.CampaignSystem;

/// <summary>
/// Service contract for elevating noble clans to independent sovereign kingdoms
/// and establishing baseline diplomacy.
/// </summary>
public interface IFactionElevationService
{
    /// <summary>
    /// Elevates ruling noble clans to independent kingdoms and assigns their starting domains.
    /// </summary>
    /// <param name="domains">The collection of balanced sovereign domains.</param>
    /// <param name="playerDomain">The specific domain allocated to the player character.</param>
    void ElevateRealms(IReadOnlyList<RealmDomain> domains, RealmDomain playerDomain);

    /// <summary>
    /// Establishes mutual peace between all sovereign kingdoms on Day 1.
    /// </summary>
    void EstablishInitialPeace();
}
