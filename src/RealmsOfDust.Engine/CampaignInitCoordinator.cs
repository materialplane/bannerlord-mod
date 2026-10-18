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
    /// Executes the initialization pipeline.
    /// In Sandbox mode, immediately elevates the player to monarch.
    /// In Campaign mode, elevates the 52 AI realms and returns the reserved player domain for deferred reclamation.
    /// </summary>
    /// <param name="reservedPlayerDomain">Outputs the reserved player domain.</param>
    /// <returns>The computed MapAllocationResult.</returns>
    public MapAllocationResult ExecuteFullInitialization(out RealmDomain reservedPlayerDomain)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        // 1. Phase 03: Proximity Allocation & Bound Village Rebinding
        MapAllocationResult allocation = _allocationService.AllocateMap();

        // 2. Phase 05: Player Domain Selection
        reservedPlayerDomain = _playerInitializer.SelectPlayerDomain(allocation.Domains);
        bool isCampaignStoryMode = Campaign.Current?.GameMode == CampaignGameMode.Campaign;

        if (!isCampaignStoryMode)
        {
            _playerInitializer.InitializePlayerRealm(reservedPlayerDomain);
        }

        // 3. Phase 04: Elevation of Noble Clans to Kingdoms & Mutual Day 1 Peace
        _elevationService.ElevateRealms(allocation.Domains, reservedPlayerDomain);
        _elevationService.EstablishInitialPeace();

        // 4. Phase 06: Military Wipe, Retinue Reset & Garrison Seeding
        _militaryResetService.ResetFieldParties();
        _militaryResetService.SeedGarrisons(allocation);
        _militaryResetService.InitializeCivicBaselines();

        stopwatch.Stop();
        LogInitializationSuccess(allocation, stopwatch.ElapsedMilliseconds, isCampaignStoryMode, reservedPlayerDomain);

        return allocation;
    }

    private void LogInitializationSuccess(
        MapAllocationResult allocation,
        long elapsedMs,
        bool isCampaignMode,
        RealmDomain reservedDomain)
    {
        string message = $"[Realms of Dust] Continental partition complete in {elapsedMs}ms: " +
                         $"{allocation.Domains.Count} Sovereign Realms, " +
                         $"{allocation.FrontierHolds.Count} Frontier Strongholds.";

        InformationManager.DisplayMessage(new InformationMessage(message, Colors.Cyan));

        if (reservedDomain?.Capital != null)
        {
            if (isCampaignMode)
            {
                string narrativeMsg = $"[Realms of Dust] The Great Plague has shattered Calradia. The usurper Radagos has seized your ancestral seat of " +
                                      $"{reservedDomain.Capital.Name}! Defeat him in the wilderness to reclaim your throne.";
                InformationManager.DisplayMessage(new InformationMessage(narrativeMsg, Colors.Yellow));
            }
            else
            {
                string narrativeMsg = $"[Realms of Dust] Sovereign realm established at {reservedDomain.Capital.Name}! " +
                                      $"Your crown and ancestral holdings are secured.";
                InformationManager.DisplayMessage(new InformationMessage(narrativeMsg, Colors.Yellow));
            }
        }
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
