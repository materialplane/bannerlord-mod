namespace RealmsOfDust.Engine;

using TaleWorlds.CampaignSystem;
using RealmsOfDust.Domain.Military;

/// <summary>
/// Campaign behavior enforcing Realms of Dust deserter rules.
/// Hooks hourly ticks and game load to maintain Season 0 grace
/// and the 60-man deserter company limits.
/// </summary>
public sealed class RealmsOfDustDeserterBehavior : CampaignBehaviorBase
{
    private readonly DeserterCompanyService _deserterService;

    public RealmsOfDustDeserterBehavior()
    {
        _deserterService = new DeserterCompanyService();
    }

    public override void RegisterEvents()
    {
        CampaignEvents.HourlyTickEvent.AddNonSerializedListener(this, OnHourlyTick);
        CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
    }

    public override void SyncData(IDataStore dataStore)
    {
        // Stateless service; persistence handled natively by MobileParty
    }

    private void OnHourlyTick()
    {
        _deserterService.ProcessDeserterParties();
    }

    private void OnGameLoaded(CampaignGameStarter starter)
    {
        _deserterService.ProcessDeserterParties();
    }
}
