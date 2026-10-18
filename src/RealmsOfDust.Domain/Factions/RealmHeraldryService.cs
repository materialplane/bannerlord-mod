namespace RealmsOfDust.Domain.Factions;

using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;

/// <summary>
/// Generates and applies rich, distinct, vibrant heraldic color schemes for sovereign realms,
/// eliminating the washed-out placeholder beige (0x967E7E) from vanilla Imperial clans.
/// </summary>
public class RealmHeraldryService
{
    private const uint FadedBeigeRgb = 0x00967E7E;

    private static readonly MethodInfo? KingdomColorSetMethod =
        typeof(Kingdom).GetProperty("Color")?.GetSetMethod(true);
    private static readonly MethodInfo? KingdomColor2SetMethod =
        typeof(Kingdom).GetProperty("Color2")?.GetSetMethod(true);
    private static readonly MethodInfo? KingdomPrimaryBannerColorSetMethod =
        typeof(Kingdom).GetProperty("PrimaryBannerColor")?.GetSetMethod(true);
    private static readonly MethodInfo? KingdomSecondaryBannerColorSetMethod =
        typeof(Kingdom).GetProperty("SecondaryBannerColor")?.GetSetMethod(true);

    private static readonly (uint primary, uint secondary)[] ImperialPalette = new[]
    {
        (0xFF793191, 0xFFFCDE90), // Royal Tyrian Purple & Bright Gold
        (0xFF382188, 0xFFDEA940), // Deep Indigo & Goldenrod
        (0xFF591645, 0xFFFFAD54), // Imperial Plum & Sunset Orange
        (0xFF8D291A, 0xFFF7BF46), // Imperial Scarlet & Bright Gold
        (0xFFB57A1E, 0xFF39223F), // Byzantine Amber & Dark Violet
        (0xFF7739A7, 0xFFF1C232), // Vibrant Amethyst & Saffron
        (0xFFA62B2B, 0xFFF3F3F3), // Imperial Crimson & Silver
        (0xFF2E4DA7, 0xFFF4D32E), // Imperial Cobalt & Sun Gold
        (0xFF1E5938, 0xFFEFC990), // Imperial Malachite & Cream
        (0xFF9E2B25, 0xFFE3A834), // Roman Rust Red & Imperial Ochre
        (0xFF4A100C, 0xFFFFD700), // Dark Burgundy & Radiant Gold
        (0xFF3A6298, 0xFFFFE9D4), // Steel Blue & Ivory
    };

    private static readonly (uint primary, uint secondary)[] KhuzaitPalette = new[]
    {
        (0xFF429081, 0xFFEFC990), // Emerald Teal & Cream
        (0xFF287D84, 0xFFF7E3A1), // Turquoise & Pale Gold
        (0xFF3A6298, 0xFFF3F3F3), // Steppe Cobalt & Silver
        (0xFFD48E28, 0xFF1B3B36), // Amber Gold & Dark Jade
        (0xFF1F5F5B, 0xFFFFD275), // Deep Spruce & Sun Gold
        (0xFF5AA4AD, 0xFFFFF1D0), // Sky Turquoise & Warm White
    };

    private static readonly (uint primary, uint secondary)[] SturgiaPalette = new[]
    {
        (0xFF224277, 0xFFCEDAE7), // Navy Sapphire & Ice Frost Blue
        (0xFF1C2A50, 0xFF949CCC), // Midnight Sea & Silver Frost
        (0xFF334A52, 0xFFA6D5DB), // Slate Steel & Cyan
        (0xFF7A1B1B, 0xFFEAEAEA), // Nordic Crimson & Pure Silver
        (0xFF141F36, 0xFFF7BF46), // Deep Night & Gold
        (0xFF2E6171, 0xFFD4E7C5), // Arctic Seafoam & Pale Lime
    };

    private static readonly (uint primary, uint secondary)[] VlandiaPalette = new[]
    {
        (0xFF8D291A, 0xFFF7BF46), // Lion Scarlet & Sun Gold
        (0xFF1C1A18, 0xFFF4D32E), // Onyx Black & Radiant Gold
        (0xFF681426, 0xFFE3E3E3), // Deep Burgundy & Silver
        (0xFF1D386B, 0xFFF7BF46), // Fleur-de-lis Royal Blue & Gold
        (0xFF5C2017, 0xFFECBA44), // Crimson Terracotta & Amber
        (0xFF2B3A42, 0xFFFFCC00), // Steel Charcoal & Bright Gold
    };

    private static readonly (uint primary, uint secondary)[] BattaniaPalette = new[]
    {
        (0xFF284E19, 0xFFB4F0F1), // Highland Forest & Pale Mint
        (0xFF34671E, 0xFFE7ECD6), // Moss Green & Pale Bone
        (0xFF1C3D18, 0xFFF1C232), // Deep Hunter & Saffron
        (0xFF5FBD72, 0xFF1A381F), // Vivid Spring Green & Dark Forest
        (0xFF2D3F1D, 0xFFBFCBB0), // Ancient Pine & Sage
        (0xFF3D6B35, 0xFFFFEAA7), // Emerald Glen & Warm Cream
    };

    private static readonly (uint primary, uint secondary)[] AseraiPalette = new[]
    {
        (0xFFB57A1E, 0xFF4E1A13), // Desert Copper & Mahogany
        (0xFFC47B2B, 0xFF2A150C), // Saffron Amber & Dark Walnut
        (0xFF1A6868, 0xFFEFC990), // Oasis Turquoise & Sand Gold
        (0xFF965228, 0xFFF4D32E), // Terracotta Dust & Sun Gold
        (0xFF7A3E1D, 0xFFFFD166), // Desert Sunset & Radiant Yellow
        (0xFF1E1915, 0xFFDEA940), // Bedouin Onyx & Desert Gold
    };

    /// <summary>
    /// Resolves vibrant primary and secondary heraldic colors for a realm based on culture and fief name.
    /// </summary>
    public (uint primary, uint secondary) ResolveHeraldry(Clan clan, Settlement? capital)
    {
        string cultureId = clan.Culture?.StringId ?? "empire";
        (uint primary, uint secondary)[] palette = cultureId switch
        {
            "khuzait" => KhuzaitPalette,
            "sturgia" => SturgiaPalette,
            "vlandia" => VlandiaPalette,
            "battania" => BattaniaPalette,
            "aserai" => AseraiPalette,
            _ => ImperialPalette,
        };

        string seedKey = (capital?.StringId ?? clan.StringId) + "_" + clan.StringId;
        int hash = Math.Abs(seedKey.GetHashCode());
        return palette[hash % palette.Length];
    }

    /// <summary>
    /// Applies heraldic colors to both the kingdom entity and the ruling clan banner.
    /// </summary>
    public void ApplyHeraldry(Kingdom kingdom, Clan clan, Settlement? capital)
    {
        if (kingdom == null || clan == null)
        {
            return;
        }

        var (primary, secondary) = ResolveHeraldry(clan, capital);

        KingdomColorSetMethod?.Invoke(kingdom, new object[] { primary });
        KingdomColor2SetMethod?.Invoke(kingdom, new object[] { secondary });
        KingdomPrimaryBannerColorSetMethod?.Invoke(kingdom, new object[] { primary });
        KingdomSecondaryBannerColorSetMethod?.Invoke(kingdom, new object[] { secondary });

        clan.Color = primary;
        clan.Color2 = secondary;
        clan.UpdateBannerColor(primary, secondary);
        clan.Banner?.ChangeBackgroundColor(primary, secondary);
        clan.Banner?.ChangeIconColors(secondary);
        kingdom.Banner?.ChangeBackgroundColor(primary, secondary);
        kingdom.Banner?.ChangeIconColors(secondary);
    }

    /// <summary>
    /// Scans active AI kingdoms and fixes any faded placeholder beige banners (0x967E7E).
    /// </summary>
    public void FixupExistingFadedBanners()
    {
        foreach (Kingdom kingdom in Kingdom.All)
        {
            Clan? rulerClan = kingdom.RulingClan;
            if (rulerClan == null || rulerClan == Clan.PlayerClan)
            {
                continue;
            }

            uint bannerColor = (rulerClan.Banner?.GetPrimaryColor() ?? 0) & 0x00FFFFFF;
            if (bannerColor == FadedBeigeRgb || bannerColor == 0 || kingdom.PrimaryBannerColor == 0)
            {
                Settlement? cap = kingdom.InitialHomeSettlement ?? rulerClan.HomeSettlement;
                ApplyHeraldry(kingdom, rulerClan, cap);
            }
        }
    }
}
