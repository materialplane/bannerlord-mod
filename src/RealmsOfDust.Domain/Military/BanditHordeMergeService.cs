namespace RealmsOfDust.Domain.Military;

using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using RealmsOfDust.Core.Constants;

/// <summary>
/// Merges roaming bandit parties into combined hordes when in close proximity.
/// Allows cross-faction bandit cooperation (Looters, Forest Bandits, Sea Raiders, etc.)
/// up to the seasonal horde troop limit.
/// </summary>
public class BanditHordeMergeService
{
    /// <summary>
    /// Gets the maximum allowed troop count for merged bandit hordes based on season and phase.
    /// By the second half of every season, merged hordes exceed the player's party cap by +5.
    /// </summary>
    public static int GetHordeCapForSeason(int seasonIndex, bool isLateSeason = false)
    {
        return seasonIndex switch
        {
            0 => isLateSeason ? RealmsOfDustConstants.BanditHordeCapSeason0Late : RealmsOfDustConstants.BanditHordeCapSeason0Early,
            1 => isLateSeason ? RealmsOfDustConstants.BanditHordeCapSeason1Late : RealmsOfDustConstants.BanditHordeCapSeason1Early,
            2 => isLateSeason ? RealmsOfDustConstants.BanditHordeCapSeason2Late : RealmsOfDustConstants.BanditHordeCapSeason2Early,
            _ => isLateSeason ? RealmsOfDustConstants.BanditHordeCapSeason3Late : RealmsOfDustConstants.BanditHordeCapSeason3Early
        };
    }

    /// <summary>
    /// Evaluates all active bandit parties on the campaign map and merges eligible nearby parties.
    /// </summary>
    public void ProcessBanditMergers()
    {
        var banditParties = MobileParty.AllBanditParties;
        if (banditParties == null || banditParties.Count < 2)
        {
            return;
        }

        int seasonIndex = CampaignSeasonHelper.GetElapsedSeasonIndex();
        bool isLateSeason = CampaignSeasonHelper.IsSecondHalfOfSeason();
        int hordeCap = GetHordeCapForSeason(seasonIndex, isLateSeason);

        var eligible = new List<MobileParty>(banditParties.Count);
        for (int i = 0; i < banditParties.Count; i++)
        {
            MobileParty party = banditParties[i];
            if (IsEligibleForMerging(party, hordeCap))
            {
                eligible.Add(party);
            }
        }

        var mergedAway = new HashSet<MobileParty>();

        for (int i = 0; i < eligible.Count; i++)
        {
            MobileParty partyA = eligible[i];
            if (mergedAway.Contains(partyA) || !IsEligibleForMerging(partyA, hordeCap))
            {
                continue;
            }

            for (int j = i + 1; j < eligible.Count; j++)
            {
                MobileParty partyB = eligible[j];
                if (mergedAway.Contains(partyB) || !IsEligibleForMerging(partyB, hordeCap))
                {
                    continue;
                }

                if (partyA.Position.DistanceSquared(partyB.Position) <= RealmsOfDustConstants.BanditMergeProximitySquared)
                {
                    if (TaleWorlds.Core.MBRandom.RandomFloat > RealmsOfDustConstants.BanditMergeChancePerHour)
                    {
                        continue;
                    }

                    bool merged = TryMergeParties(partyA, partyB, hordeCap, mergedAway);
                    if (merged && mergedAway.Contains(partyA))
                    {
                        break;
                    }
                }
            }
        }
    }

    private static bool IsEligibleForMerging(MobileParty party, int hordeCap)
    {
        return party != null &&
               party.IsActive &&
               party.IsBandit &&
               !party.IsBanditBossParty &&
               !party.IsDisbanding &&
               party.MapEvent == null &&
               party.SiegeEvent == null &&
               party.MemberRoster != null &&
               party.MemberRoster.TotalManCount < hordeCap;
    }

    private static bool TryMergeParties(MobileParty partyA, MobileParty partyB, int hordeCap, HashSet<MobileParty> mergedAway)
    {
        MobileParty receiver = partyA.MemberRoster.TotalManCount >= partyB.MemberRoster.TotalManCount ? partyA : partyB;
        MobileParty donor = receiver == partyA ? partyB : partyA;

        int receiverCount = receiver.MemberRoster.TotalManCount;
        int donorCount = donor.MemberRoster.TotalManCount;
        int spaceAvailable = hordeCap - receiverCount;

        if (spaceAvailable <= 0)
        {
            return false;
        }

        if (donorCount <= spaceAvailable)
        {
            AbsorbFullParty(receiver, donor);
            mergedAway.Add(donor);
            return true;
        }

        AbsorbPartialParty(receiver, donor, spaceAvailable);
        return false;
    }

    private static void AbsorbFullParty(MobileParty receiver, MobileParty donor)
    {
        receiver.MemberRoster.Add(donor.MemberRoster);
        if (donor.PrisonRoster != null && donor.PrisonRoster.Count > 0)
        {
            receiver.PrisonRoster?.Add(donor.PrisonRoster);
        }

        DestroyPartyAction.Apply(null, donor);
    }

    private static void AbsorbPartialParty(MobileParty receiver, MobileParty donor, int countToTransfer)
    {
        var elements = donor.MemberRoster.GetTroopRoster();
        int transferred = 0;

        for (int k = elements.Count - 1; k >= 0 && transferred < countToTransfer; k--)
        {
            var element = elements[k];
            if (element.Character == null)
            {
                continue;
            }

            int canTake = Math.Min(element.Number, countToTransfer - transferred);
            if (canTake > 0)
            {
                receiver.MemberRoster.AddToCounts(element.Character, canTake, false, element.WoundedNumber > 0 ? Math.Min(element.WoundedNumber, canTake) : 0, element.Xp);
                donor.MemberRoster.AddToCounts(element.Character, -canTake);
                transferred += canTake;
            }
        }
    }
}
