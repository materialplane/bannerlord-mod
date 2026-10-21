namespace RealmsOfDust.Domain.Military;

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;
using RealmsOfDust.Core.Constants;

/// <summary>
/// Party speed model enforcing a minimum speed floor for merged bandit hordes.
/// Prevents large infantry-heavy coalitions from crawling at an immobile snail pace,
/// while keeping them slow enough for healthy caravans and player scouts to outrun.
/// </summary>
public class PostPlaguePartySpeedModel : DefaultPartySpeedCalculatingModel
{
    private static readonly TextObject BrigandMobilityText =
        new TextObject("{=rod_brigand_mobility}Brigand Mobility");

    /// <inheritdoc />
    public override ExplainedNumber CalculateFinalSpeed(MobileParty mobileParty, ExplainedNumber finalSpeed)
    {
        ExplainedNumber result = base.CalculateFinalSpeed(mobileParty, finalSpeed);

        if (mobileParty == null || !mobileParty.IsBandit || mobileParty.MemberRoster == null)
        {
            return result;
        }

        try
        {
            if (mobileParty.MemberRoster.TotalManCount >= RealmsOfDustConstants.BanditHordeMobilityThreshold)
            {
                float currentSpeed = result.ResultNumber;
                if (currentSpeed > 0f && currentSpeed < RealmsOfDustConstants.BanditHordeMinSpeedFloor)
                {
                    float boost = RealmsOfDustConstants.BanditHordeMinSpeedFloor - currentSpeed;
                    result.Add(boost, BrigandMobilityText);
                }
            }
        }
        catch (Exception ex)
        {
            TaleWorlds.Library.Debug.Print($"[RealmsOfDust] Speed calculation notice: {ex.Message}");
        }

        return result;
    }
}
