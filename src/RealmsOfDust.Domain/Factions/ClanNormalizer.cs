namespace RealmsOfDust.Domain.Factions;

using System.Reflection;
using TaleWorlds.CampaignSystem;

/// <summary>
/// Normalizes clan assets, authority tier, and influence to standard baselines.
/// </summary>
public static class ClanNormalizer
{
    private static readonly FieldInfo? TierField = typeof(Clan).GetField(
        "_tier",
        BindingFlags.Instance | BindingFlags.NonPublic);

    /// <summary>
    /// Sets a clan's tier, treasury, and influence to standardized starting amounts.
    /// </summary>
    /// <param name="clan">The clan to normalize.</param>
    /// <param name="targetTier">The target clan tier (e.g. Tier 2 for player, Tier 3 for AI).</param>
    /// <param name="startingDenars">The starting treasury gold.</param>
    /// <param name="startingInfluence">The starting kingdom influence.</param>
    public static void Normalize(Clan clan, int targetTier, int startingDenars, float startingInfluence)
    {
        if (clan == null)
        {
            return;
        }

        // 1. Clan Tier & Renown Baseline
        if (TierField != null)
        {
            TierField.SetValue(clan, targetTier);
        }

        clan.Renown = targetTier switch
        {
            1 => 0f,
            2 => 50f,
            3 => 150f,
            4 => 350f,
            _ => 50f
        };

        // 2. Leader Treasury
        if (clan.Leader != null)
        {
            clan.Leader.Gold = startingDenars;
        }

        // 3. Kingdom Influence
        clan.Influence = startingInfluence;
    }
}
