namespace RealmsOfDust.Domain.Factions;

using System;
using System.Collections.Generic;
using System.Linq;
using RealmsOfDust.Core.Constants;
using RealmsOfDust.Core.Interfaces;
using RealmsOfDust.Core.Models;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;

/// <summary>
/// Elevates noble clans to independent sovereign kingdoms, assigns domain fiefs,
/// and establishes initial continental peace.
/// </summary>
public sealed class FactionElevationService : IFactionElevationService
{
    public void ElevateRealms(IReadOnlyList<RealmDomain> domains, RealmDomain playerDomain)
    {
        if (domains == null)
        {
            return;
        }

        HashSet<Clan> assignedClans = new();
        if (Clan.PlayerClan != null)
        {
            assignedClans.Add(Clan.PlayerClan);
        }

        foreach (RealmDomain domain in domains)
        {
            // Skip the player's assigned domain (handled by IPlayerRealmInitializer)
            if (domain == playerDomain || domain.Capital == playerDomain?.Capital)
            {
                continue;
            }

            Clan? rulerClan = ResolveClanForDomain(domain, assignedClans);
            if (rulerClan == null)
            {
                continue;
            }

            assignedClans.Add(rulerClan);
            Kingdom kingdom = CreateRealmKingdom(rulerClan, domain);

            // Establish Kingdom Sovereignty
            ChangeKingdomAction.ApplyByCreateKingdom(rulerClan, kingdom, false);

            // Transfer Fief Ownership
            TransferDomainOwnership(rulerClan, domain);

            // Clear residual town gold reserves
            if (domain.Capital.Town != null)
            {
                domain.Capital.Town.ChangeGold(-domain.Capital.Town.Gold);
            }
            if (domain.Stronghold.Town != null)
            {
                domain.Stronghold.Town.ChangeGold(-domain.Stronghold.Town.Gold);
            }

            // Normalize Clan Assets & Tier
            ClanNormalizer.Normalize(
                rulerClan,
                RealmsOfDustConstants.AiStartingClanTier,
                RealmsOfDustConstants.DefaultStartingDenars,
                RealmsOfDustConstants.DefaultStartingInfluence);
        }
    }

    public void EstablishInitialPeace()
    {
        DiplomacySeeder.EstablishMutualPeace();
    }

    private static Clan? ResolveClanForDomain(RealmDomain domain, HashSet<Clan> assignedClans)
    {
        // 1. Prefer the historical owner clan of the town if available and unassigned
        Clan? candidate = domain.Capital.OwnerClan;
        if (candidate != null && !candidate.IsMinorFaction && !assignedClans.Contains(candidate))
        {
            return candidate;
        }

        // 2. Fall back to any unassigned, living noble clan
        return Clan.All.FirstOrDefault(c =>
            !c.IsMinorFaction &&
            !c.IsEliminated &&
            !assignedClans.Contains(c) &&
            c != Clan.PlayerClan);
    }

    private static Kingdom CreateRealmKingdom(Clan clan, RealmDomain domain)
    {
        string kingdomId = "rod_kingdom_" + clan.StringId;
        Kingdom kingdom = Kingdom.CreateKingdom(kingdomId);

        TextObject name = new(domain.Capital.Name.ToString());
        TextObject informalName = new(domain.Capital.Name.ToString());
        TextObject textDesc = new(string.Empty);

        var (primaryColor, secondaryColor) = ResolveColors(clan);

        kingdom.InitializeKingdom(
            name,
            informalName,
            clan.Culture,
            clan.Banner,
            primaryColor,
            secondaryColor,
            domain.Capital,
            textDesc,
            textDesc,
            textDesc);

        clan.Color = primaryColor;
        clan.Color2 = secondaryColor;

        return kingdom;
    }

    private const uint InvalidColor = 0xDEADBEEF;

    private static (uint primary, uint secondary) ResolveColors(Clan clan)
    {
        uint primary = 0;
        uint secondary = 0;

        // 1. Prefer original kingdom heraldic colors if available
        if (clan.Kingdom != null && clan.Kingdom.Color != 0 && clan.Kingdom.Color != InvalidColor)
        {
            primary = clan.Kingdom.Color;
            secondary = clan.Kingdom.Color2;
        }

        // 2. Fall back to native culture colors (guaranteed valid in TaleWorlds)
        if ((primary == 0 || primary == InvalidColor) && clan.Culture != null)
        {
            primary = clan.Culture.Color;
            secondary = clan.Culture.Color2;
        }

        // 3. Fall back to clan's existing colors if valid
        if ((primary == 0 || primary == InvalidColor) && clan.Color != 0 && clan.Color != InvalidColor)
        {
            primary = clan.Color;
            secondary = clan.Color2;
        }

        // 4. Hard fallback to vibrant imperial crimson and gold
        if (primary == 0 || primary == InvalidColor)
        {
            primary = 0xFF9E2B25;
            secondary = 0xFFE3A834;
        }

        if ((primary & 0xFF000000) == 0) primary |= 0xFF000000;
        if ((secondary & 0xFF000000) == 0) secondary |= 0xFF000000;

        return (primary, secondary);
    }

    private static void TransferDomainOwnership(Clan clan, RealmDomain domain)
    {
        if (clan.Leader == null)
        {
            return;
        }

        ChangeOwnerOfSettlementAction.ApplyByDefault(clan.Leader, domain.Capital);
        ChangeOwnerOfSettlementAction.ApplyByDefault(clan.Leader, domain.Stronghold);

        domain.Capital.Party?.SetVisualAsDirty();
        domain.Stronghold.Party?.SetVisualAsDirty();
        foreach (var village in domain.BoundVillages)
        {
            village.Party?.SetVisualAsDirty();
        }
    }
}
