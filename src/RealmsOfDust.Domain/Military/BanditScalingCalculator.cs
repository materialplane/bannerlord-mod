namespace RealmsOfDust.Domain.Military;

using System;
using TaleWorlds.CampaignSystem;
using RealmsOfDust.Core.Models;

/// <summary>
/// Calculates seasonal bandit party headcount ranges, widening over time.
/// </summary>
public static class BanditScalingCalculator
{
    /// <summary>
    /// Computes the min/max headcount range for the specified elapsed season index.
    /// Season 0 (Spring Year 1): 8 - 15
    /// Season 1 (Summer Year 1): 10 - 28
    /// Season 2 (Autumn Year 1): 15 - 38
    /// Season 3+ (Winter & Year 2+): 15 - 50
    /// </summary>
    /// <param name="seasonIndex">Total seasons elapsed since campaign start.</param>
    /// <returns>A BanditScaleRange representing the allowed troop count bounds.</returns>
    public static BanditScaleRange CalculateRangeForSeason(int seasonIndex)
    {
        switch (seasonIndex)
        {
            case 0:
                return new BanditScaleRange(8, 15);
            case 1:
                return new BanditScaleRange(10, 28);
            case 2:
                return new BanditScaleRange(15, 38);
            default:
                return new BanditScaleRange(15, 50);
        }
    }

    /// <summary>
    /// Picks a random target headcount within the specified range using the provided RNG.
    /// </summary>
    public static int PickTargetCount(BanditScaleRange range, Random random)
    {
        if (range.MinCount >= range.MaxCount)
        {
            return range.MinCount;
        }

        return random.Next(range.MinCount, range.MaxCount + 1);
    }
}
