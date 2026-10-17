namespace RealmsOfDust.Domain.Allocation;

using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

/// <summary>
/// Provides spatial distance calculations and proximity-based sorting for settlements.
/// </summary>
public static class ProximityCalculator
{
    /// <summary>
    /// Calculates the squared Euclidean distance between two settlements on the campaign map.
    /// Using squared distance avoids expensive square root operations for sorting.
    /// </summary>
    /// <param name="a">The origin settlement.</param>
    /// <param name="b">The target settlement.</param>
    /// <returns>The squared distance between both settlements.</returns>
    public static float GetDistanceSquared(Settlement a, Settlement b)
    {
        if (a == null || b == null)
        {
            return float.MaxValue;
        }

        Vec2 posA = a.GetPosition2D;
        Vec2 posB = b.GetPosition2D;
        return posA.DistanceSquared(posB);
    }

    /// <summary>
    /// Finds the closest settlement from a candidates collection to a reference origin.
    /// </summary>
    /// <param name="origin">The reference settlement.</param>
    /// <param name="candidates">The available candidate settlements.</param>
    /// <returns>The closest settlement, or null if candidates is empty.</returns>
    public static Settlement? FindClosest(Settlement origin, IEnumerable<Settlement> candidates)
    {
        if (origin == null || candidates == null)
        {
            return null;
        }

        Settlement? closest = null;
        float minDistanceSq = float.MaxValue;

        foreach (Settlement candidate in candidates)
        {
            float distSq = GetDistanceSquared(origin, candidate);
            if (distSq < minDistanceSq)
            {
                minDistanceSq = distSq;
                closest = candidate;
            }
        }

        return closest;
    }

    /// <summary>
    /// Orders a collection of settlements by distance relative to a reference origin.
    /// </summary>
    /// <param name="origin">The reference settlement.</param>
    /// <param name="candidates">The settlements to order.</param>
    /// <returns>Ordered list from nearest to farthest.</returns>
    public static IReadOnlyList<Settlement> OrderByProximity(
        Settlement origin,
        IEnumerable<Settlement> candidates)
    {
        if (origin == null || candidates == null)
        {
            return System.Array.Empty<Settlement>();
        }

        return candidates
            .OrderBy(c => GetDistanceSquared(origin, c))
            .ToList();
    }
}
