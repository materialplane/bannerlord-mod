namespace RealmsOfDust.Domain.Military;

using System;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using RealmsOfDust.Core.Constants;

/// <summary>
/// Initializes civic loyalty, security, food granary reserves, wall levels,
/// and post-plague baseline prosperity/hearths across all fortifications and villages.
/// </summary>
public class SettlementCivicInitializer
{
    private const string WallBuildingSubstring = "fortifications";

    /// <summary>
    /// Applies standardized civic baselines across all towns, castles, and bound villages.
    /// </summary>
    public void InitializeAllCivicBaselines()
    {
        foreach (Town town in Town.AllTowns)
        {
            ApplyTownCivicSettings(town);
        }

        foreach (Town castle in Town.AllCastles)
        {
            ApplyCastleCivicSettings(castle);
        }

        foreach (Settlement settlement in Settlement.All)
        {
            if (settlement.IsVillage && settlement.Village != null)
            {
                settlement.Village.Hearth = RealmsOfDustConstants.VillageHearthsBaseline;
            }
        }
    }

    private void ApplyTownCivicSettings(Town town)
    {
        town.Prosperity = RealmsOfDustConstants.TownProsperityBaseline;
        town.Loyalty = RealmsOfDustConstants.DefaultStartingLoyalty;
        town.Security = RealmsOfDustConstants.DefaultStartingSecurity;
        SetFoodStocks(town);
        SetWallsToTierOne(town);
    }

    private void ApplyCastleCivicSettings(Town castle)
    {
        castle.Prosperity = RealmsOfDustConstants.CastleProsperityBaseline;
        castle.Loyalty = RealmsOfDustConstants.DefaultStartingLoyalty;
        castle.Security = RealmsOfDustConstants.DefaultStartingSecurity;
        SetFoodStocks(castle);
        SetWallsToTierOne(castle);
    }

    private void SetFoodStocks(Town town)
    {
        int maxCapacity = town.FoodStocksUpperLimit();
        float targetFood = RealmsOfDustConstants.FoodStocksDaySupply;
        if (targetFood > maxCapacity && maxCapacity > 0)
        {
            targetFood = maxCapacity;
        }

        town.FoodStocks = targetFood;
    }

    private void SetWallsToTierOne(Town town)
    {
        if (town.Buildings == null)
        {
            return;
        }

        foreach (Building building in town.Buildings)
        {
            string buildingId = building.BuildingType?.StringId ?? string.Empty;
            if (buildingId.IndexOf(WallBuildingSubstring, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (building.CurrentLevel == 0)
                {
                    building.CurrentLevel = 1;
                }
            }
        }
    }
}
