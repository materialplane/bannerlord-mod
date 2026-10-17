namespace RealmsOfDust.Engine;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using RealmsOfDust.Core.Interfaces;
using RealmsOfDust.Domain.Military;

/// <summary>
/// Campaign behavior listening to TaleWorlds lifecycle events and delegating
/// to Realms of Dust initialization and runtime services.
/// </summary>
public sealed class RealmsOfDustCampaignBehavior : CampaignBehaviorBase
{
    private readonly CampaignInitCoordinator _coordinator;
    private readonly IBanditScalingService _banditScalingService;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="RealmsOfDustCampaignBehavior"/> class.
    /// </summary>
    public RealmsOfDustCampaignBehavior()
    {
        _coordinator = new CampaignInitCoordinator();
        _banditScalingService = new BanditScalingService();
    }

    /// <inheritdoc />
    public override void RegisterEvents()
    {
        CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, OnCharacterCreationIsOver);
        CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, OnMobilePartyCreated);
        CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, OnDailyTickParty);
    }

    /// <inheritdoc />
    public override void SyncData(IDataStore dataStore)
    {
        dataStore.SyncData("_rod_isInitialized", ref _isInitialized);
    }

    private void OnCharacterCreationIsOver()
    {
        RunInitialization("OnCharacterCreationIsOver");
    }

    private void RunInitialization(string sourceEvent)
    {
        if (_isInitialized)
        {
            return;
        }

        try
        {
            System.IO.File.AppendAllText(
                @"C:\ProgramData\Mount and Blade II Bannerlord\logs\realms_of_dust.log",
                $"[{System.DateTime.Now}] Starting {sourceEvent} execution...\n");

            _coordinator.ExecuteFullInitialization();
            _isInitialized = true;

            System.IO.File.AppendAllText(
                @"C:\ProgramData\Mount and Blade II Bannerlord\logs\realms_of_dust.log",
                $"[{System.DateTime.Now}] {sourceEvent} completed successfully.\n");
        }
        catch (System.Exception ex)
        {
            System.IO.File.AppendAllText(
                @"C:\ProgramData\Mount and Blade II Bannerlord\logs\realms_of_dust.log",
                $"[{System.DateTime.Now}] {sourceEvent} Exception: {ex}\n");
            throw;
        }
    }

    private void OnMobilePartyCreated(MobileParty party)
    {
        if (party.IsBandit)
        {
            _banditScalingService.ScaleBanditParty(party);
            return;
        }

        if (party.IsLordParty &&
            party.ActualClan != Clan.PlayerClan &&
            party.ActualClan?.IsBanditFaction == false &&
            _banditScalingService.IsGracePeriodActive)
        {
            _coordinator.ResetSingleLordParty(party);
        }
    }

    private void OnDailyTickParty(MobileParty party)
    {
        if (party.IsBandit)
        {
            _banditScalingService.ScaleBanditParty(party);
        }
    }
}
