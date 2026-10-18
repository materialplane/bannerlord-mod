namespace RealmsOfDust.Domain.Military;

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;
using RealmsOfDust.Core.Constants;

/// <summary>
/// Enforces universal post-plague recovery caps on all mobile field armies and prisoner sizes.
/// In Year 1, lord warbands scale progressively across the 4 seasons (25 -> 35 -> 50 -> 70),
/// guaranteeing that no faction or king can snowball into massive 90+ doomstacks early.
/// </summary>
public class PostPlaguePartySizeLimitModel : DefaultPartySizeLimitModel
{
    private static readonly TextObject PostPlagueArmyText = new TextObject("{=rod_post_plague_army}Post-Plague Manpower Cap");
    private static readonly TextObject PostPlaguePrisonerText = new TextObject("{=rod_post_plague_prisoner}Post-Plague Prisoner Cap");

    /// <inheritdoc />
    public override ExplainedNumber GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = false)
    {
        ExplainedNumber result = base.GetPartyMemberSizeLimit(party, includeDescriptions);

        if (party?.MobileParty != null && party.MobileParty.IsLordParty)
        {
            int seasonIndex = CampaignSeasonHelper.GetElapsedSeasonIndex();
            int maxCap = GetPostPlaguePartySizeCap(seasonIndex);

            if (maxCap > 0 && result.ResultNumber > maxCap)
            {
                result.LimitMax(maxCap, includeDescriptions ? PostPlagueArmyText : null);
            }
        }

        return result;
    }

    /// <inheritdoc />
    public override ExplainedNumber GetPartyPrisonerSizeLimit(PartyBase party, bool includeDescriptions = false)
    {
        ExplainedNumber result = base.GetPartyPrisonerSizeLimit(party, includeDescriptions);

        if (party?.MobileParty != null && party.MobileParty.IsLordParty)
        {
            int seasonIndex = CampaignSeasonHelper.GetElapsedSeasonIndex();
            int maxPrisonerCap = GetPostPlaguePrisonerCap(seasonIndex);

            if (maxPrisonerCap > 0 && result.ResultNumber > maxPrisonerCap)
            {
                result.LimitMax(maxPrisonerCap, includeDescriptions ? PostPlaguePrisonerText : null);
            }
        }

        return result;
    }

    /// <summary>
    /// Checks whether the specified mobile party belongs to a minor or mercenary faction.
    /// </summary>
    public static bool IsMinorFactionParty(MobileParty party)
    {
        return party.ActualClan?.IsMinorFaction == true || party.MapFaction?.IsMinorFaction == true;
    }

    /// <summary>
    /// Computes the maximum party size allowed for lord armies based on elapsed seasons.
    /// Returns -1 when the post-plague recovery period has completed (Season 4+ / Year 2+).
    /// </summary>
    public static int GetPostPlaguePartySizeCap(int seasonIndex)
    {
        return seasonIndex switch
        {
            0 => RealmsOfDustConstants.PostPlaguePartyCapSeason0, // Summer Year 1: 25
            1 => RealmsOfDustConstants.PostPlaguePartyCapSeason1, // Autumn Year 1: 35
            2 => RealmsOfDustConstants.PostPlaguePartyCapSeason2, // Winter Year 1: 50
            3 => RealmsOfDustConstants.PostPlaguePartyCapSeason3, // Spring Year 1: 70
            _ => -1,                                             // Year 2+: Uncapped vanilla limit
        };
    }

    /// <summary>
    /// Computes the maximum prisoner capacity allowed for lord armies based on elapsed seasons.
    /// Returns -1 when the post-plague recovery period has completed (Season 4+ / Year 2+).
    /// </summary>
    public static int GetPostPlaguePrisonerCap(int seasonIndex)
    {
        return seasonIndex switch
        {
            0 => RealmsOfDustConstants.PostPlaguePrisonerCapSeason0, // Summer Year 1: 10
            1 => RealmsOfDustConstants.PostPlaguePrisonerCapSeason1, // Autumn Year 1: 15
            2 => RealmsOfDustConstants.PostPlaguePrisonerCapSeason2, // Winter Year 1: 25
            3 => RealmsOfDustConstants.PostPlaguePrisonerCapSeason3, // Spring Year 1: 40
            _ => -1,                                                // Year 2+: Uncapped vanilla limit
        };
    }

    /// <summary>
    /// Backwards-compatible alias for minor clan queries.
    /// </summary>
    public static int GetMinorFactionPartySizeCap(int seasonIndex) => GetPostPlaguePartySizeCap(seasonIndex);

    /// <summary>
    /// Backwards-compatible alias for minor clan queries.
    /// </summary>
    public static int GetMinorFactionPrisonerCap(int seasonIndex) => GetPostPlaguePrisonerCap(seasonIndex);
}
