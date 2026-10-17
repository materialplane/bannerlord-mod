namespace RealmsOfDust.Domain.Factions;

using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

/// <summary>
/// Enforces the Day 1 mutual peace baseline across all sovereign kingdoms.
/// </summary>
public static class DiplomacySeeder
{
    /// <summary>
    /// Establishes peace between all pairs of active sovereign kingdoms.
    /// </summary>
    public static void EstablishMutualPeace()
    {
        List<Kingdom> kingdoms = Kingdom.All.Where(k => !k.IsEliminated).ToList();

        for (int i = 0; i < kingdoms.Count; i++)
        {
            for (int j = i + 1; j < kingdoms.Count; j++)
            {
                Kingdom k1 = kingdoms[i];
                Kingdom k2 = kingdoms[j];

                if (k1.IsAtWarWith(k2))
                {
                    MakePeaceAction.Apply(k1, k2);
                }
            }
        }
    }
}
