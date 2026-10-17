namespace RealmsOfDust.Domain.Economy;

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;
using RealmsOfDust.Core.Constants;

/// <summary>
/// Gradually restores town tax revenue over the first year as post-plague civic infrastructure recovers.
/// Year 1 progresses from 25% -> 50% -> 75% -> 100% across the 4 seasons.
/// </summary>
public class PostPlagueSettlementTaxModel : DefaultSettlementTaxModel
{
    private static readonly TextObject PostPlagueRecoveryText = new TextObject("{=rod_post_plague}Post-Plague Recovery");

    /// <inheritdoc />
    public override ExplainedNumber CalculateTownTax(Town town, bool includeDescriptions = false)
    {
        ExplainedNumber result = base.CalculateTownTax(town, includeDescriptions);

        float penaltyFactor = GetPlagueRecoveryPenaltyFactor();
        if (penaltyFactor < 0f)
        {
            result.AddFactor(penaltyFactor, includeDescriptions ? PostPlagueRecoveryText : null);
        }

        return result;
    }

    private static float GetPlagueRecoveryPenaltyFactor()
    {
        int seasonIndex = CampaignSeasonHelper.GetElapsedSeasonIndex();

        switch (seasonIndex)
        {
            case 0: // Spring Year 1: 25% yield (-75% factor)
                return -0.75f;
            case 1: // Summer Year 1: 50% yield (-50% factor)
                return -0.50f;
            case 2: // Autumn Year 1: 75% yield (-25% factor)
                return -0.25f;
            default: // Winter Year 1 & Year 2+: 100% normal yield
                return 0f;
        }
    }
}
