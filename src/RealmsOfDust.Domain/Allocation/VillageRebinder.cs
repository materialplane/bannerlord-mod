namespace RealmsOfDust.Domain.Allocation;

using System;
using System.Reflection;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

/// <summary>
/// Safely manages referential binding between Villages and their parent Fortifications (Towns or Castles).
/// </summary>
public static class VillageRebinder
{
    private static readonly PropertyInfo? BoundProp = typeof(Village).GetProperty(
        "Bound",
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

    private static readonly FieldInfo? BoundVillagesField = typeof(Settlement).GetField(
        "_boundVillages",
        BindingFlags.Instance | BindingFlags.NonPublic);

    /// <summary>
    /// Rebinds a village to a designated parent settlement (Town or Castle).
    /// </summary>
    /// <param name="villageSettlement">The settlement representing the village.</param>
    /// <param name="newParent">The new parent Town or Castle settlement.</param>
    public static void Rebind(Settlement villageSettlement, Settlement newParent)
    {
        if (villageSettlement?.Village == null || newParent == null)
        {
            return;
        }

        Village village = villageSettlement.Village;
        Settlement? oldParent = village.Bound;

        // 1. Remove village from old parent's internal _boundVillages list
        if (oldParent != null && BoundVillagesField != null)
        {
            if (BoundVillagesField.GetValue(oldParent) is MBList<Village> oldList)
            {
                oldList.Remove(village);
            }
        }

        // 2. Assign village.Bound to newParent
        if (BoundProp != null)
        {
            BoundProp.SetValue(village, newParent);
        }

        // 3. Add village to new parent's internal _boundVillages list
        if (BoundVillagesField != null)
        {
            if (BoundVillagesField.GetValue(newParent) is MBList<Village> newList)
            {
                if (!newList.Contains(village))
                {
                    newList.Add(village);
                }
            }
        }
    }
}
