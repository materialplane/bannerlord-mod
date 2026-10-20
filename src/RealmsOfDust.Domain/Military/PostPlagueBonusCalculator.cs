namespace RealmsOfDust.Domain.Military;

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

/// <summary>
/// Safely calculates personal party size bonuses (Steward skill + perks) for the player.
/// Designed with defensive fail-safes so it never crashes during save game deserialization.
/// </summary>
public static class PostPlagueBonusCalculator
{
    /// <summary>
    /// Computes the effective post-plague party cap including difficulty-unlocked personal bonuses.
    /// </summary>
    public static int CalculateEffectivePartyLimit(MobileParty party, int baseCap, int seasonIndex)
    {
        if (!party.IsMainParty)
        {
            return baseCap;
        }

        bool isHardMode = true;
        try
        {
            isHardMode = CampaignOptions.PlayerTroopsReceivedDamage == CampaignOptions.Difficulty.Realistic;
        }
        catch
        {
            // Fallback during save deserialization
        }

        bool bonusesActive = !isHardMode || seasonIndex >= 1;
        if (!bonusesActive)
        {
            return baseCap;
        }

        return baseCap + CalculatePersonalBonus(party);
    }

    private static int CalculatePersonalBonus(MobileParty party)
    {
        Hero? leader = party.LeaderHero;
        if (leader == null)
        {
            return 0;
        }

        float bonus = 0f;

        // Steward: Quartermaster party size bonus (0.25 troops per skill level in vanilla)
        if (DefaultSkills.Steward != null)
        {
            try
            {
                int stewardLevel = leader.GetSkillValue(DefaultSkills.Steward);
                bonus += stewardLevel * 0.25f;
            }
            catch
            {
                // Fallback
            }
        }

        // Perks: safely queried when Campaign and perks are initialized
        try
        {
            if (Campaign.Current?.DefaultPerks != null)
            {
                if (DefaultPerks.Bow.MerryMen != null && leader.GetPerkValue(DefaultPerks.Bow.MerryMen))
                    bonus += DefaultPerks.Bow.MerryMen.PrimaryBonus;
                if (DefaultPerks.Tactics.HordeLeader != null && leader.GetPerkValue(DefaultPerks.Tactics.HordeLeader))
                    bonus += DefaultPerks.Tactics.HordeLeader.PrimaryBonus;
                if (DefaultPerks.Leadership.Authority != null && leader.GetPerkValue(DefaultPerks.Leadership.Authority))
                    bonus += DefaultPerks.Leadership.Authority.SecondaryBonus;
                if (DefaultPerks.Leadership.UpliftingSpirit != null && leader.GetPerkValue(DefaultPerks.Leadership.UpliftingSpirit))
                    bonus += DefaultPerks.Leadership.UpliftingSpirit.SecondaryBonus;
                if (DefaultPerks.Leadership.TalentMagnet != null && leader.GetPerkValue(DefaultPerks.Leadership.TalentMagnet))
                    bonus += DefaultPerks.Leadership.TalentMagnet.PrimaryBonus;
            }
        }
        catch
        {
            // Defensive guard during save deserialization
        }

        return (int)Math.Floor(bonus);
    }
}
