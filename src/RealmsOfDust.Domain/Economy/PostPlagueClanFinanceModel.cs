namespace RealmsOfDust.Domain.Economy;

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

/// <summary>
/// Gradually restores town tariffs and bound village commerce over the first year as post-plague trade recovers.
/// Applies equally to both the player clan and all AI lord clans.
/// </summary>
public class PostPlagueClanFinanceModel : DefaultClanFinanceModel
{
    private static readonly TextObject PostPlagueTariffRecoveryText = new TextObject("{=rod_post_plague_tariffs}Post-Plague Trade Recovery");

    /// <inheritdoc />
    public override ExplainedNumber CalculateTownIncomeFromTariffs(Clan clan, Town town, bool applyWithdrawals = false)
    {
        ExplainedNumber result = base.CalculateTownIncomeFromTariffs(clan, town, applyWithdrawals);

        float penaltyFactor = GetTariffPenaltyFactor();
        if (penaltyFactor < 0f)
        {
            result.AddFactor(penaltyFactor, PostPlagueTariffRecoveryText);
        }

        return result;
    }

    /// <inheritdoc />
    public override int CalculateVillageIncome(Clan clan, Village village, bool applyWithdrawals = false)
    {
        int baseIncome = base.CalculateVillageIncome(clan, village, applyWithdrawals);
        float multiplier = GetVillageIncomeMultiplier();

        return (int)(baseIncome * multiplier);
    }

    private static float GetTariffPenaltyFactor()
    {
        int seasonIndex = CampaignSeasonHelper.GetElapsedSeasonIndex();

        switch (seasonIndex)
        {
            case 0: // Season 1 (Days 1-21): 20% trade volume (-80% penalty)
                return -0.80f;
            case 1: // Season 2 (Days 22-42): 50% trade volume (-50% penalty)
                return -0.50f;
            case 2: // Season 3 (Days 43-63): 75% trade volume (-25% penalty)
                return -0.25f;
            default: // Season 4+ (Days 64+): Full recovery (0% penalty)
                return 0f;
        }
    }

    private static float GetVillageIncomeMultiplier()
    {
        int seasonIndex = CampaignSeasonHelper.GetElapsedSeasonIndex();

        switch (seasonIndex)
        {
            case 0: // Season 1: 20% yield
                return 0.20f;
            case 1: // Season 2: 50% yield
                return 0.50f;
            case 2: // Season 3: 75% yield
                return 0.75f;
            default: // Season 4+: 100% normal yield
                return 1.0f;
        }
    }
}
