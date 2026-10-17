namespace RealmsOfDust.Engine;

using System;
using System.Diagnostics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using RealmsOfDust.Core.Interfaces;
using RealmsOfDust.Core.Models;
using RealmsOfDust.Domain.Allocation;
using RealmsOfDust.Domain.Factions;
using RealmsOfDust.Domain.Military;
using RealmsOfDust.Domain.Player;

/// <summary>
/// Coordinates the deterministic multi-phase initialization sequence of Realms of Dust on Day 1.
/// </summary>
public sealed class CampaignInitCoordinator
{
    private readonly ISettlementAllocationService _allocationService;
    private readonly IFactionElevationService _elevationService;
    private readonly IPlayerRealmInitializer _playerInitializer;
    private readonly IMilitaryResetService _militaryResetService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CampaignInitCoordinator"/> class.
    /// </summary>
    public CampaignInitCoordinator()
    {
        _allocationService = new SettlementAllocationService();
        _elevationService = new FactionElevationService();
        _playerInitializer = new PlayerRealmInitializer();
        _militaryResetService = new MilitaryResetService();
    }

    /// <summary>
    /// Executes the full initialization pipeline in deterministic order.
    /// </summary>
    /// <returns>The computed MapAllocationResult.</returns>
    public MapAllocationResult ExecuteFullInitialization()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        // 1. Phase 03: Proximity Allocation & Bound Village Rebinding
        MapAllocationResult allocation = _allocationService.AllocateMap();

        // 2. Phase 05: Player Domain Selection & Sovereign Realm Setup
        RealmDomain playerDomain = _playerInitializer.SelectPlayerDomain(allocation.Domains);
        _playerInitializer.InitializePlayerRealm(playerDomain);

        // 3. Phase 04: Elevation of Noble Clans to Kingdoms & Mutual Day 1 Peace
        _elevationService.ElevateRealms(allocation.Domains, playerDomain);
        _elevationService.EstablishInitialPeace();

        // 4. Phase 06: Military Wipe, Retinue Reset & Garrison Seeding
        _militaryResetService.ResetFieldParties();
        _militaryResetService.SeedGarrisons(allocation);
        _militaryResetService.InitializeCivicBaselines();

        stopwatch.Stop();
        LogInitializationSuccess(allocation, stopwatch.ElapsedMilliseconds);

        return allocation;
    }

    private void LogInitializationSuccess(MapAllocationResult allocation, long elapsedMs)
    {
        string message = $"[Realms of Dust] Continental partition complete in {elapsedMs}ms: " +
                         $"{allocation.Domains.Count} Sovereign Realms, " +
                         $"{allocation.FrontierHolds.Count} Frontier Strongholds.";

        InformationManager.DisplayMessage(new InformationMessage(message, Colors.Cyan));
    }

    /// <summary>
    /// Resets an individual newly spawned lord party to its starter retinue.
    /// </summary>
    /// <param name="party">The mobile party to reset.</param>
    public void ResetSingleLordParty(TaleWorlds.CampaignSystem.Party.MobileParty party)
    {
        _militaryResetService.ResetSingleLordParty(party);
    }
}
