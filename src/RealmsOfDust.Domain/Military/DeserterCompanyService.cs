namespace RealmsOfDust.Domain.Military;

using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using RealmsOfDust.Core.Constants;

/// <summary>
/// Domain service managing rogue deserter companies.
/// Enforces Season 0 suppression (Days 1-21 grace), 25-troop merge eligibility,
/// 40-troop company locks, and a strict 60-man hard cap.
/// </summary>
public class DeserterCompanyService
{
    private const string DeserterClanStringId = "deserters";

    /// <summary>
    /// Evaluates and enforces deserter company rules across all active parties.
    /// </summary>
    public void ProcessDeserterParties()
    {
        var deserterParties = GetActiveDeserterParties();
        if (deserterParties.Count == 0)
        {
            return;
        }

        int seasonIndex = CampaignSeasonHelper.GetElapsedSeasonIndex();

        // Season 0 Grace: Suppress all deserters during Days 1-21
        if (seasonIndex < 1)
        {
            SuppressSeason0Deserters(deserterParties);
            return;
        }

        // Season 1+: Enforce hard caps and custom merger rules
        EnforceHardCaps(deserterParties);
        ProcessMergers(deserterParties);
    }

    private static List<MobileParty> GetActiveDeserterParties()
    {
        var list = new List<MobileParty>();
        var allParties = MobileParty.All;
        if (allParties == null)
        {
            return list;
        }

        for (int i = 0; i < allParties.Count; i++)
        {
            MobileParty party = allParties[i];
            if (party != null && party.IsActive && party.ActualClan?.StringId == DeserterClanStringId)
            {
                list.Add(party);
            }
        }

        return list;
    }

    private static void SuppressSeason0Deserters(List<MobileParty> deserters)
    {
        for (int i = 0; i < deserters.Count; i++)
        {
            MobileParty party = deserters[i];
            if (party != null && party.IsActive && party.MapEvent == null && party.SiegeEvent == null)
            {
                DestroyPartyAction.Apply(null, party);
            }
        }
    }

    private static void EnforceHardCaps(List<MobileParty> deserters)
    {
        for (int i = 0; i < deserters.Count; i++)
        {
            MobileParty party = deserters[i];
            if (party == null || !party.IsActive || party.MemberRoster == null)
            {
                continue;
            }

            int count = party.MemberRoster.TotalManCount;
            if (count > RealmsOfDustConstants.DeserterCompanyHardCap)
            {
                int excess = count - RealmsOfDustConstants.DeserterCompanyHardCap;
                party.MemberRoster.RemoveNumberOfNonHeroTroopsRandomly(excess);
            }

            BanditHordeMountService.EnsureHordeMobilityMounts(party);
        }
    }

    private static void ProcessMergers(List<MobileParty> deserters)
    {
        var mergedAway = new HashSet<MobileParty>();

        for (int i = 0; i < deserters.Count; i++)
        {
            MobileParty partyA = deserters[i];
            if (mergedAway.Contains(partyA) || !IsEligibleToMerge(partyA))
            {
                continue;
            }

            for (int j = i + 1; j < deserters.Count; j++)
            {
                MobileParty partyB = deserters[j];
                if (mergedAway.Contains(partyB) || !IsEligibleToMerge(partyB))
                {
                    continue;
                }

                if (partyA.MemberRoster.TotalManCount >= RealmsOfDustConstants.DeserterCompanyLockThreshold ||
                    partyB.MemberRoster.TotalManCount >= RealmsOfDustConstants.DeserterCompanyLockThreshold)
                {
                    continue;
                }

                int totalCount = partyA.MemberRoster.TotalManCount + partyB.MemberRoster.TotalManCount;
                if (totalCount > RealmsOfDustConstants.DeserterCompanyHardCap)
                {
                    continue;
                }

                if (partyA.Position.DistanceSquared(partyB.Position) <= RealmsOfDustConstants.BanditMergeProximitySquared)
                {
                    if (MBRandom.RandomFloat > RealmsOfDustConstants.BanditMergeChancePerHour)
                    {
                        continue;
                    }

                    MobileParty receiver = partyA.MemberRoster.TotalManCount >= partyB.MemberRoster.TotalManCount ? partyA : partyB;
                    MobileParty donor = receiver == partyA ? partyB : partyA;

                    receiver.MemberRoster.Add(donor.MemberRoster);
                    if (donor.PrisonRoster != null && donor.PrisonRoster.Count > 0)
                    {
                        receiver.PrisonRoster?.Add(donor.PrisonRoster);
                    }

                    BanditHordeMountService.TransferLootAndMounts(receiver, donor);
                    BanditHordeMountService.EnsureHordeMobilityMounts(receiver);

                    DestroyPartyAction.Apply(null, donor);
                    mergedAway.Add(donor);

                    if (mergedAway.Contains(partyA))
                    {
                        break;
                    }
                }
            }
        }
    }

    private static bool IsEligibleToMerge(MobileParty party)
    {
        return party != null &&
               party.IsActive &&
               party.MapEvent == null &&
               party.SiegeEvent == null &&
               !party.IsDisbanding &&
               party.MemberRoster != null &&
               party.MemberRoster.TotalManCount <= RealmsOfDustConstants.DeserterMergeEligibilityThreshold;
    }
}
