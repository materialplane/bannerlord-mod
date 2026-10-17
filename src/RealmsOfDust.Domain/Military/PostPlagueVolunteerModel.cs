namespace RealmsOfDust.Domain.Military;

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using RealmsOfDust.Core.Constants;

/// <summary>
/// Throttles village recruit generation and unlocks recruitment slots gradually
/// as village manpower recovers from the Great Plague across the first year.
/// </summary>
public class PostPlagueVolunteerModel : DefaultVolunteerModel
{
    /// <inheritdoc />
    public override int MaximumIndexHeroCanRecruitFromHero(Hero buyerHero, Hero sellerHero, int useValueAsRelation = -101)
    {
        int baseIndex = base.MaximumIndexHeroCanRecruitFromHero(buyerHero, sellerHero, useValueAsRelation);
        int maxAllowedSlot = GetAllowedRecruitmentSlotIndex();

        return Math.Min(baseIndex, maxAllowedSlot);
    }

    /// <inheritdoc />
    public override float GetDailyVolunteerProductionProbability(Hero hero, int index, Settlement settlement)
    {
        float baseProb = base.GetDailyVolunteerProductionProbability(hero, index, settlement);
        float multiplier = GetVolunteerProductionMultiplier();

        return baseProb * multiplier;
    }

    private static int GetAllowedRecruitmentSlotIndex()
    {
        int seasonIndex = CampaignSeasonHelper.GetElapsedSeasonIndex();

        switch (seasonIndex)
        {
            case 0: // Spring Year 1: Only first 2 slots unlocked
                return 2;
            case 1: // Summer Year 1: First 3 slots unlocked
                return 3;
            case 2: // Autumn Year 1: First 4 slots unlocked
                return 4;
            default: // Winter Year 1 & Year 2+: Full 6 slots accessible
                return 6;
        }
    }

    private static float GetVolunteerProductionMultiplier()
    {
        int seasonIndex = CampaignSeasonHelper.GetElapsedSeasonIndex();

        switch (seasonIndex)
        {
            case 0: // Spring: 35% volunteer replenishment speed
                return 0.35f;
            case 1: // Summer: 55% replenishment speed
                return 0.55f;
            case 2: // Autumn: 75% replenishment speed
                return 0.75f;
            default: // Winter onwards: 100% normal production
                return 1.0f;
        }
    }
}
