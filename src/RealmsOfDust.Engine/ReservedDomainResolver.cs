namespace RealmsOfDust.Engine;

using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;
using RealmsOfDust.Core.Models;

/// <summary>
/// Reconstructs a reserved RealmDomain from persisted settlement identifiers upon game load.
/// </summary>
public static class ReservedDomainResolver
{
    /// <summary>
    /// Resolves the RealmDomain from capital and stronghold string IDs.
    /// </summary>
    /// <param name="capitalId">The string ID of the capital.</param>
    /// <param name="strongholdId">The string ID of the stronghold.</param>
    /// <returns>The reconstructed RealmDomain, or null if settlements are not found.</returns>
    public static RealmDomain? Resolve(string capitalId, string strongholdId)
    {
        Settlement? capital = !string.IsNullOrEmpty(capitalId) ? Settlement.Find(capitalId) : null;
        Settlement? stronghold = !string.IsNullOrEmpty(strongholdId) ? Settlement.Find(strongholdId) : null;

        if (capital == null)
        {
            capital = Settlement.All.Find(s => s.IsTown && (s.OwnerClan == null || s.OwnerClan == TaleWorlds.CampaignSystem.Clan.PlayerClan || s.OwnerClan.Leader == null));
        }

        if (stronghold == null && capital != null)
        {
            stronghold = Settlement.All.Find(s => s.IsCastle && (s.OwnerClan == null || s.OwnerClan == TaleWorlds.CampaignSystem.Clan.PlayerClan || s.OwnerClan.Leader == null));
        }

        if (capital == null || stronghold == null)
        {
            return null;
        }

        List<Settlement> villages = new List<Settlement>();
        if (capital.BoundVillages != null)
        {
            foreach (var v in capital.BoundVillages)
            {
                if (v.Settlement != null)
                {
                    villages.Add(v.Settlement);
                }
            }
        }

        if (stronghold.BoundVillages != null)
        {
            foreach (var v in stronghold.BoundVillages)
            {
                if (v.Settlement != null)
                {
                    villages.Add(v.Settlement);
                }
            }
        }

        return new RealmDomain(capital, stronghold, villages);
    }
}
