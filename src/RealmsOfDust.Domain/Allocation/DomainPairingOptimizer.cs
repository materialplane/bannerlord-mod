namespace RealmsOfDust.Domain.Allocation;

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem.Settlements;

/// <summary>
/// Optimizes Town-to-Castle geographic pairings globally using 2-opt distance minimization,
/// ensuring contiguous, compact realm domains without distant enclaves.
/// </summary>
public static class DomainPairingOptimizer
{
    /// <summary>
    /// Computes optimal Town-to-Castle pairings minimizing global travel distance.
    /// Matched castles are removed from the availableCastles collection.
    /// </summary>
    /// <param name="towns">All map towns to pair.</param>
    /// <param name="availableCastles">The pool of available castles.</param>
    /// <returns>List of optimal (Town, Castle) pairs.</returns>
    public static List<(Settlement Town, Settlement Castle)> PairTownsAndCastles(
        List<Settlement> towns,
        List<Settlement> availableCastles)
    {
        if (towns == null || availableCastles == null || towns.Count == 0)
        {
            return new List<(Settlement, Settlement)>();
        }

        // 1. Initial greedy assignment
        List<Settlement> castlePool = new(availableCastles);
        Dictionary<Settlement, Settlement> assignments = new(towns.Count);

        foreach (Settlement town in towns)
        {
            Settlement? closest = ProximityCalculator.FindClosest(town, castlePool);
            if (closest != null)
            {
                assignments[town] = closest;
                castlePool.Remove(closest);
            }
        }

        // 2. 2-opt pairwise swap optimization to eliminate cross-continent enclave lines
        bool improved = true;
        int maxIterations = 50;
        int iteration = 0;

        while (improved && iteration < maxIterations)
        {
            improved = false;
            iteration++;

            for (int i = 0; i < towns.Count; i++)
            {
                Settlement townA = towns[i];
                if (!assignments.TryGetValue(townA, out Settlement? castleA))
                {
                    continue;
                }

                for (int j = i + 1; j < towns.Count; j++)
                {
                    Settlement townB = towns[j];
                    if (!assignments.TryGetValue(townB, out Settlement? castleB))
                    {
                        continue;
                    }

                    float currentCost = ProximityCalculator.GetDistanceSquared(townA, castleA) +
                                        ProximityCalculator.GetDistanceSquared(townB, castleB);

                    float swappedCost = ProximityCalculator.GetDistanceSquared(townA, castleB) +
                                        ProximityCalculator.GetDistanceSquared(townB, castleA);

                    if (swappedCost + 1.0f < currentCost)
                    {
                        assignments[townA] = castleB;
                        assignments[townB] = castleA;
                        castleA = castleB;
                        improved = true;
                    }
                }
            }
        }

        // 3. Finalize pairs and remove assigned castles from availableCastles
        List<(Settlement, Settlement)> result = new(towns.Count);
        foreach (KeyValuePair<Settlement, Settlement> kvp in assignments)
        {
            availableCastles.Remove(kvp.Value);
            result.Add((kvp.Key, kvp.Value));
        }

        return result;
    }
}
