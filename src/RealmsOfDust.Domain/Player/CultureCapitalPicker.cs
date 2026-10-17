namespace RealmsOfDust.Domain.Player;

using System;
using System.Collections.Generic;
using System.Linq;
using RealmsOfDust.Core.Models;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

/// <summary>
/// Selects a randomized starting domain for the player matching their chosen culture.
/// </summary>
public static class CultureCapitalPicker
{
    /// <summary>
    /// Picks a domain whose Capital matches the player's culture, or falls back to any domain.
    /// </summary>
    /// <param name="availableDomains">All domains available on the active map.</param>
    /// <param name="playerCulture">The culture chosen by the player during character creation.</param>
    /// <returns>The selected RealmDomain.</returns>
    public static RealmDomain PickRandomDomain(
        IReadOnlyList<RealmDomain> availableDomains,
        CultureObject? playerCulture)
    {
        if (availableDomains == null || availableDomains.Count == 0)
        {
            throw new ArgumentException("No available domains to pick from.", nameof(availableDomains));
        }

        // 1. Filter domains matching the player's chosen culture
        List<RealmDomain> cultureMatches = availableDomains
            .Where(d => playerCulture != null && d.Capital.Culture == playerCulture)
            .ToList();

        // 2. If culture matches exist, pick randomly among them
        if (cultureMatches.Count > 0)
        {
            int index = MBRandom.RandomInt(cultureMatches.Count);
            return cultureMatches[index];
        }

        // 3. Fallback: Pick randomly from all available domains
        int fallbackIndex = MBRandom.RandomInt(availableDomains.Count);
        return availableDomains[fallbackIndex];
    }
}
