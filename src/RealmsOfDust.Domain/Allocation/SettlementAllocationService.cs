namespace RealmsOfDust.Domain.Allocation;

using System;
using System.Collections.Generic;
using System.Linq;
using RealmsOfDust.Core.Constants;
using RealmsOfDust.Core.Interfaces;
using RealmsOfDust.Core.Models;
using TaleWorlds.CampaignSystem.Settlements;

/// <summary>
/// Dynamically partitions all settlements on the active campaign map into
/// balanced sovereign domains and unaligned frontier strongholds.
/// Fully compatible with both standard Calradia and expanded maps like War Sails.
/// </summary>
public sealed class SettlementAllocationService : ISettlementAllocationService
{
    public MapAllocationResult AllocateMap()
    {
        List<Settlement> towns = Settlement.All
            .Where(s => s.IsTown && !s.IsCastle)
            .OrderBy(s => s.StringId)
            .ToList();

        List<Settlement> availableCastles = Settlement.All
            .Where(s => s.IsCastle)
            .ToList();

        List<Settlement> availableVillages = Settlement.All
            .Where(s => s.IsVillage)
            .ToList();

        List<RealmDomain> domains = new(towns.Count);

        // 1. Optimally pair each Town with its closest contiguous Castle
        List<(Settlement Town, Settlement Castle)> pairs =
            DomainPairingOptimizer.PairTownsAndCastles(towns, availableCastles);

        // 2. Allocate 4 bound villages per domain (2 to Town, 2 to Castle)
        foreach (var (town, castle) in pairs)
        {
            List<Settlement> domainVillages = new(RealmsOfDustConstants.VillagesPerDomain);

            // Assign 2 closest villages to Town
            var townVillages = ProximityCalculator.OrderByProximity(town, availableVillages)
                .Take(RealmsOfDustConstants.VillagesPerTown)
                .ToList();

            foreach (Settlement v in townVillages)
            {
                availableVillages.Remove(v);
                VillageRebinder.Rebind(v, town);
                domainVillages.Add(v);
            }

            // Assign 2 closest villages to Castle
            var castleVillages = ProximityCalculator.OrderByProximity(castle, availableVillages)
                .Take(RealmsOfDustConstants.VillagesPerCastle)
                .ToList();

            foreach (Settlement v in castleVillages)
            {
                availableVillages.Remove(v);
                VillageRebinder.Rebind(v, castle);
                domainVillages.Add(v);
            }

            domains.Add(new RealmDomain(town, castle, domainVillages));
        }

        // 3. Leftover Castles become Frontier Strongholds
        List<FrontierHold> frontierHolds = new(availableCastles.Count);
        Dictionary<Settlement, List<Settlement>> frontierMap = availableCastles
            .ToDictionary(c => c, _ => new List<Settlement>());

        // 4. Distribute remaining villages to closest Frontier Stronghold
        foreach (Settlement village in availableVillages)
        {
            Settlement? closestFrontier = ProximityCalculator.FindClosest(village, availableCastles);
            if (closestFrontier != null)
            {
                VillageRebinder.Rebind(village, closestFrontier);
                frontierMap[closestFrontier].Add(village);
            }
        }

        foreach (KeyValuePair<Settlement, List<Settlement>> kvp in frontierMap)
        {
            frontierHolds.Add(new FrontierHold(kvp.Key, kvp.Value));
        }

        return new MapAllocationResult(domains, frontierHolds);
    }
}
