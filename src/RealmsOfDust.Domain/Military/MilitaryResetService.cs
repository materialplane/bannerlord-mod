namespace RealmsOfDust.Domain.Military;

using RealmsOfDust.Core.Interfaces;
using RealmsOfDust.Core.Models;

/// <summary>
/// Orchestrates the complete Day 1 military reset, garrison seeding,
/// and civic baseline initialization across the campaign map.
/// </summary>
public class MilitaryResetService : IMilitaryResetService
{
    private readonly PartyRosterResetService _partyResetService;
    private readonly GarrisonSeedingService _garrisonService;
    private readonly SettlementCivicInitializer _civicInitializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="MilitaryResetService"/> class.
    /// </summary>
    public MilitaryResetService()
    {
        _partyResetService = new PartyRosterResetService();
        _garrisonService = new GarrisonSeedingService();
        _civicInitializer = new SettlementCivicInitializer();
    }

    /// <inheritdoc />
    public void ResetFieldParties()
    {
        _partyResetService.ResetAllFieldParties();
    }

    /// <inheritdoc />
    public void ResetSingleLordParty(TaleWorlds.CampaignSystem.Party.MobileParty party)
    {
        _partyResetService.ResetSingleLordParty(party);
    }

    /// <inheritdoc />
    public void SeedGarrisons(MapAllocationResult allocation)
    {
        _garrisonService.SeedAllGarrisons(allocation);
    }

    /// <inheritdoc />
    public void InitializeCivicBaselines()
    {
        _civicInitializer.InitializeAllCivicBaselines();
    }
}
