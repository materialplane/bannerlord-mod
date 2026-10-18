namespace RealmsOfDust.Domain.Military;

using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using RealmsOfDust.Core.Interfaces;

/// <summary>
/// Pre-activates all hideouts across Calradia on Day 1 campaign initialization,
/// seeding garrison defender parties so native bandit patrols begin immediately.
/// </summary>
public sealed class HideoutActivationService : IHideoutActivationService
{
    private const int MinPartiesToInfest = 2;

    /// <inheritdoc />
    public void ActivateAllHideouts()
    {
        if (Hideout.All == null || Hideout.All.Count == 0)
        {
            return;
        }

        int activatedCount = 0;
        foreach (Hideout hideout in Hideout.All)
        {
            Settlement? settlement = hideout.Settlement;
            if (settlement == null)
            {
                continue;
            }

            Clan? banditClan = ResolveBanditClanForHideout(settlement);
            if (banditClan == null)
            {
                continue;
            }

            int existingDefenders = GetDefenderCount(settlement);
            int partiesNeeded = MinPartiesToInfest - existingDefenders;

            for (int i = 0; i < partiesNeeded; i++)
            {
                SpawnDefenderBanditParty(hideout, settlement, banditClan);
            }

            settlement.IsVisible = true;
            activatedCount++;
        }
    }

    private static Clan? ResolveBanditClanForHideout(Settlement settlement)
    {
        CultureObject? hideoutCulture = settlement.Culture;

        foreach (Clan banditClan in Clan.BanditFactions)
        {
            if (banditClan.Culture == hideoutCulture)
            {
                return banditClan;
            }
        }

        // Fallback to first available bandit clan if culture match not found
        foreach (Clan banditClan in Clan.BanditFactions)
        {
            if (banditClan.DefaultPartyTemplate != null)
            {
                return banditClan;
            }
        }

        return null;
    }

    private static int GetDefenderCount(Settlement settlement)
    {
        if (settlement.Parties == null)
        {
            return 0;
        }

        int count = 0;
        foreach (MobileParty party in settlement.Parties)
        {
            if (party.IsBandit)
            {
                count++;
            }
        }

        return count;
    }

    private static void SpawnDefenderBanditParty(Hideout hideout, Settlement settlement, Clan banditClan)
    {
        PartyTemplateObject? template = banditClan.DefaultPartyTemplate;
        if (template == null)
        {
            return;
        }

        string partyId = $"{banditClan.StringId}_hideout_def_{Guid.NewGuid():N}";
        CampaignVec2 spawnPos = settlement.GatePosition;

        MobileParty party = BanditPartyComponent.CreateBanditParty(
            partyId,
            banditClan,
            hideout,
            false,
            template,
            spawnPos);

        if (party == null)
        {
            return;
        }

        party.ActualClan = banditClan;
        EnterSettlementAction.ApplyForParty(party, settlement);
    }
}
