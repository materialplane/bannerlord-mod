namespace RealmsOfDust.Domain.Player;

using System;
using System.Collections.Generic;
using RealmsOfDust.Core.Constants;
using RealmsOfDust.Core.Interfaces;
using RealmsOfDust.Core.Models;
using RealmsOfDust.Domain.Factions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

/// <summary>
/// Initializes the player's sovereign kingdom, domain holdings, and starting assets.
/// </summary>
public sealed class PlayerRealmInitializer : IPlayerRealmInitializer
{
    public RealmDomain SelectPlayerDomain(IReadOnlyList<RealmDomain> availableDomains)
    {
        CultureObject? playerCulture = Hero.MainHero?.Culture ?? CharacterObject.PlayerCharacter?.Culture ?? Clan.PlayerClan?.Culture;
        return CultureCapitalPicker.PickRandomDomain(availableDomains, playerCulture);
    }

    public void InitializePlayerRealm(RealmDomain playerDomain)
    {
        if (playerDomain == null || Clan.PlayerClan == null || Hero.MainHero == null)
        {
            return;
        }

        // 1. Create Player Sovereign Kingdom
        Kingdom playerKingdom = CreatePlayerKingdom(Clan.PlayerClan, playerDomain);
        ChangeKingdomAction.ApplyByCreateKingdom(Clan.PlayerClan, playerKingdom, false);

        // 2. Transfer Fief Ownership to Player (cascades automatically to bound villages)
        ChangeOwnerOfSettlementAction.ApplyByDefault(Hero.MainHero, playerDomain.Capital);
        ChangeOwnerOfSettlementAction.ApplyByDefault(Hero.MainHero, playerDomain.Stronghold);

        playerDomain.Capital.Party?.SetVisualAsDirty();
        playerDomain.Stronghold.Party?.SetVisualAsDirty();
        foreach (var v in playerDomain.BoundVillages)
        {
            v.Party?.SetVisualAsDirty();
        }

        // 3. Clear residual town gold reserves so they don't dump into clan wallet
        if (playerDomain.Capital.Town != null)
        {
            playerDomain.Capital.Town.ChangeGold(-playerDomain.Capital.Town.Gold);
        }
        if (playerDomain.Stronghold.Town != null)
        {
            playerDomain.Stronghold.Town.ChangeGold(-playerDomain.Stronghold.Town.Gold);
        }

        // 4. Strictly normalize Player Hero and Clan Treasury to 2,500 denars
        Hero.MainHero.Gold = RealmsOfDustConstants.DefaultStartingDenars;
        ClanNormalizer.Normalize(
            Clan.PlayerClan,
            RealmsOfDustConstants.PlayerStartingClanTier,
            RealmsOfDustConstants.DefaultStartingDenars,
            RealmsOfDustConstants.DefaultStartingInfluence);

        // 5. Position Player Party at Starting Capital
        PlayerPartyPositioner.PositionAtCapital(playerDomain.Capital);
    }

    private static Kingdom CreatePlayerKingdom(Clan playerClan, RealmDomain domain)
    {
        string kingdomId = "rod_kingdom_player";
        Kingdom kingdom = Kingdom.CreateKingdom(kingdomId);

        TextObject name = new TextObject("{=rod_player_kingdom}Realm of {SETTLEMENT}")
            .SetTextVariable("SETTLEMENT", domain.Capital.Name);
        TextObject informalName = new TextObject(domain.Capital.Name.ToString());
        TextObject textDesc = new TextObject(string.Empty);

        var (primaryColor, secondaryColor) = ResolvePlayerColors(playerClan);

        kingdom.InitializeKingdom(
            name,
            informalName,
            playerClan.Culture,
            playerClan.Banner,
            primaryColor,
            secondaryColor,
            domain.Capital,
            textDesc,
            textDesc,
            textDesc);

        playerClan.Color = primaryColor;
        playerClan.Color2 = secondaryColor;
        playerClan.UpdateBannerColor(primaryColor, secondaryColor);

        return kingdom;
    }

    private const uint InvalidColor = 0xDEADBEEF;

    private static (uint primary, uint secondary) ResolvePlayerColors(Clan clan)
    {
        uint primary = 0;
        uint secondary = 0;

        // 1. Prefer player's custom chosen banner colors
        if (clan.Banner != null)
        {
            primary = clan.Banner.GetPrimaryColor();
            secondary = clan.Banner.GetFirstIconColor();
            if (secondary == 0 || secondary == InvalidColor)
            {
                secondary = clan.Banner.GetSecondaryColor();
            }
        }

        // 2. Fall back to culture colors if banner colors are unset
        if (primary == 0 || primary == InvalidColor)
        {
            if (clan.Culture != null)
            {
                primary = clan.Culture.Color;
                secondary = clan.Culture.Color2;
            }
        }

        if (primary == 0 || primary == InvalidColor)
        {
            primary = 0xFF9E2B25;
            secondary = 0xFFE3A834;
        }

        if ((primary & 0xFF000000) == 0) primary |= 0xFF000000;
        if ((secondary & 0xFF000000) == 0) secondary |= 0xFF000000;

        return (primary, secondary);
    }
}
