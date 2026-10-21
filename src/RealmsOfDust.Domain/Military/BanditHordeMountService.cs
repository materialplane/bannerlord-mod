namespace RealmsOfDust.Domain.Military;

using System;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.ObjectSystem;
using RealmsOfDust.Core.Constants;

/// <summary>
/// Manages pack mount provisioning and loot transfer for merged bandit hordes.
/// Ensures large hordes do not get crippled by overburdened penalties from prisoners and loot.
/// </summary>
public static class BanditHordeMountService
{
    private static ItemObject? _cachedPackAnimal;

    /// <summary>
    /// Transfers all items and mounts from the donor party to the receiving horde.
    /// </summary>
    public static void TransferLootAndMounts(MobileParty receiver, MobileParty donor)
    {
        if (receiver?.ItemRoster == null || donor?.ItemRoster == null || donor.ItemRoster.Count == 0)
        {
            return;
        }

        try
        {
            receiver.ItemRoster.Add(donor.ItemRoster);
        }
        catch (Exception ex)
        {
            TaleWorlds.Library.Debug.Print($"[RealmsOfDust] Error transferring donor loot: {ex.Message}");
        }
    }

    /// <summary>
    /// Evaluates a bandit party's troop headcount and provisions cheap pack animals
    /// to maintain reasonable cargo mobility without granting cavalry combat advantages.
    /// </summary>
    public static void EnsureHordeMobilityMounts(MobileParty party)
    {
        if (party?.MemberRoster == null || !party.IsBandit || party.ItemRoster == null)
        {
            return;
        }

        int troopCount = party.MemberRoster.TotalManCount;
        if (troopCount < RealmsOfDustConstants.BanditHordeMobilityThreshold)
        {
            return;
        }

        int targetPackMounts = (int)Math.Ceiling(troopCount / (float)RealmsOfDustConstants.BanditMenPerPackMount);
        int currentPackMounts = party.ItemRoster.NumberOfPackAnimals;

        if (currentPackMounts >= targetPackMounts)
        {
            return;
        }

        ItemObject? packAnimal = ResolvePackAnimalItem();
        if (packAnimal == null)
        {
            return;
        }

        int needed = targetPackMounts - currentPackMounts;
        party.ItemRoster.AddToCounts(packAnimal, needed);
    }

    private static ItemObject? ResolvePackAnimalItem()
    {
        if (_cachedPackAnimal != null)
        {
            return _cachedPackAnimal;
        }

        try
        {
            _cachedPackAnimal = MBObjectManager.Instance?.GetObject<ItemObject>("sumpter_horse")
                                ?? MBObjectManager.Instance?.GetObject<ItemObject>("mule");
        }
        catch
        {
            _cachedPackAnimal = null;
        }

        return _cachedPackAnimal;
    }
}
