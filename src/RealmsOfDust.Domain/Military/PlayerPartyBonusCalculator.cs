namespace RealmsOfDust.Domain.Military;

using System;
using System.Linq;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

/// <summary>
/// Calculates personal party size bonuses granted by the player's skills and perks.
/// Used during post-plague recovery to allow personal skill investment to scale army size.
/// </summary>
public static class PlayerPartyBonusCalculator
{
    /// <summary>
    /// Computes total bonus troop capacity earned from Steward skill and combat/leadership perks.
    /// </summary>
    /// <param name="party">The mobile party being evaluated.</param>
    /// <returns>Integer bonus capacity beyond the baseline cap.</returns>
    public static int CalculatePersonalBonus(MobileParty? party)
    {
        if (party == null)
        {
            return 0;
        }

        float totalBonus = 0f;

        // 1. Steward Quartermaster skill bonus
        if (DefaultSkillEffects.StewardPartySizeBonus != null)
        {
            ExplainedNumber stewardExplained = new ExplainedNumber(0f);
            SkillHelper.AddSkillBonusForParty(DefaultSkillEffects.StewardPartySizeBonus, party, ref stewardExplained);
            totalBonus += Math.Max(0f, stewardExplained.ResultNumber);
        }

        // 2. Personal leadership and combat perks on the party leader
        Hero? leader = party.LeaderHero;
        if (leader != null)
        {
            totalBonus += GetLeaderCombatPerkBonuses(leader);
            totalBonus += GetLeadershipPerkBonuses(leader);
        }

        return (int)Math.Floor(totalBonus);
    }

    private static float GetLeaderCombatPerkBonuses(Hero leader)
    {
        float perkBonus = 0f;

        if (DefaultPerks.OneHanded.Prestige != null && leader.GetPerkValue(DefaultPerks.OneHanded.Prestige))
        {
            perkBonus += DefaultPerks.OneHanded.Prestige.SecondaryBonus;
        }
        if (DefaultPerks.TwoHanded.Hope != null && leader.GetPerkValue(DefaultPerks.TwoHanded.Hope))
        {
            perkBonus += DefaultPerks.TwoHanded.Hope.SecondaryBonus;
        }
        if (DefaultPerks.Athletics.ImposingStature != null && leader.GetPerkValue(DefaultPerks.Athletics.ImposingStature))
        {
            perkBonus += DefaultPerks.Athletics.ImposingStature.SecondaryBonus;
        }
        if (DefaultPerks.Bow.MerryMen != null && leader.GetPerkValue(DefaultPerks.Bow.MerryMen))
        {
            perkBonus += DefaultPerks.Bow.MerryMen.PrimaryBonus;
        }
        if (DefaultPerks.Tactics.HordeLeader != null && leader.GetPerkValue(DefaultPerks.Tactics.HordeLeader))
        {
            perkBonus += DefaultPerks.Tactics.HordeLeader.PrimaryBonus;
        }
        if (DefaultPerks.Scouting.MountedScouts != null && leader.GetPerkValue(DefaultPerks.Scouting.MountedScouts))
        {
            perkBonus += DefaultPerks.Scouting.MountedScouts.SecondaryBonus;
        }

        return perkBonus;
    }

    private static float GetLeadershipPerkBonuses(Hero leader)
    {
        float bonus = 0f;

        if (DefaultPerks.Leadership.Authority != null && leader.GetPerkValue(DefaultPerks.Leadership.Authority))
        {
            bonus += DefaultPerks.Leadership.Authority.SecondaryBonus;
        }
        if (DefaultPerks.Leadership.UpliftingSpirit != null && leader.GetPerkValue(DefaultPerks.Leadership.UpliftingSpirit))
        {
            bonus += DefaultPerks.Leadership.UpliftingSpirit.SecondaryBonus;
        }
        if (DefaultPerks.Leadership.TalentMagnet != null && leader.GetPerkValue(DefaultPerks.Leadership.TalentMagnet))
        {
            bonus += DefaultPerks.Leadership.TalentMagnet.PrimaryBonus;
        }
        if (DefaultPerks.Leadership.LeaderOfMasses != null && leader.GetPerkValue(DefaultPerks.Leadership.LeaderOfMasses))
        {
            int townCount = leader.Clan?.Settlements?.Count(s => s != null && s.IsTown) ?? 0;
            bonus += townCount * DefaultPerks.Leadership.LeaderOfMasses.PrimaryBonus;
        }
        if (DefaultPerks.Leadership.UltimateLeader != null && leader.GetPerkValue(DefaultPerks.Leadership.UltimateLeader))
        {
            int maxSkill = Campaign.Current?.Models?.CharacterDevelopmentModel?.MaxSkillRequiredForEpicPerkBonus ?? 250;
            int leadershipValue = leader.GetSkillValue(DefaultSkills.Leadership);
            if (leadershipValue > maxSkill)
            {
                bonus += (leadershipValue - maxSkill) * DefaultPerks.Leadership.UltimateLeader.PrimaryBonus;
            }
        }

        return bonus;
    }
}
