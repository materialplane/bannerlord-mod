namespace RealmsOfDust.Domain.Economy;

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

/// <summary>
/// Dampens post-plague civic instability across towns during Year 1 (Days 1-84).
/// Provides a civic reconstruction buffer against harsh culture-clash and low-garrison
/// security penalties, preventing micro-realms from immediately collapsing into rebellions.
/// </summary>
public class PostPlagueSettlementLoyaltyModel : DefaultSettlementLoyaltyModel
{
    private static readonly TextObject PostPlagueCivicSolidarityText =
        new TextObject("{=rod_post_plague_loyalty}Post-Plague Reconstruction Accord");

    /// <inheritdoc />
    public override ExplainedNumber CalculateLoyaltyChange(Town town, bool includeDescriptions = false)
    {
        ExplainedNumber result = base.CalculateLoyaltyChange(town, includeDescriptions);

        if (town == null)
        {
            return result;
        }

        try
        {
            int seasonIndex = CampaignSeasonHelper.GetElapsedSeasonIndex();

            // Active during Year 1 (Seasons 0-3 / Days 1-84)
            if (seasonIndex < 4)
            {
                // Soften the harsh -3.0 culture difference penalty by +2.0 so loyalty doesn't free-fall
                if (town.Settlement?.OwnerClan?.Culture != null &&
                    town.Settlement.OwnerClan.Culture != town.Settlement.Culture)
                {
                    result.Add(2.0f, includeDescriptions ? PostPlagueCivicSolidarityText : null);
                }
                else if (result.ResultNumber < 0f)
                {
                    // Buffer same-culture capitals against early low-garrison security / food deficit
                    result.Add(1.0f, includeDescriptions ? PostPlagueCivicSolidarityText : null);
                }
            }
        }
        catch
        {
            // Defensive fail-safe during save game deserialization
        }

        return result;
    }
}
