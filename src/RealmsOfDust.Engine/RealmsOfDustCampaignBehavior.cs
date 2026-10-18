namespace RealmsOfDust.Engine;

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using RealmsOfDust.Core.Interfaces;
using RealmsOfDust.Core.Models;
using RealmsOfDust.Domain.Factions;
using RealmsOfDust.Domain.Military;
using RealmsOfDust.Domain.Player;

/// <summary>
/// Campaign behavior listening to TaleWorlds lifecycle events and delegating
/// to Realms of Dust initialization and runtime services.
/// </summary>
public sealed class RealmsOfDustCampaignBehavior : CampaignBehaviorBase
{
    private readonly CampaignInitCoordinator _coordinator;
    private readonly IBanditScalingService _banditScalingService;
    private readonly StoryModeReclamationCoordinator _reclamationCoordinator;
    private readonly MinorClanEnforcementService _minorClanEnforcementService;
    private readonly RealmHeraldryService _realmHeraldryService;
    private bool _isInitialized;
    private bool _isPlayerRealmReclaimed;
    private string _reservedCapitalId = string.Empty;
    private string _reservedStrongholdId = string.Empty;
    private RealmDomain? _reservedPlayerDomain;

    /// <summary>
    /// Initializes a new instance of the <see cref="RealmsOfDustCampaignBehavior"/> class.
    /// </summary>
    public RealmsOfDustCampaignBehavior()
    {
        _coordinator = new CampaignInitCoordinator();
        _banditScalingService = new BanditScalingService();
        _reclamationCoordinator = new StoryModeReclamationCoordinator(new PlayerRealmInitializer());
        _minorClanEnforcementService = new MinorClanEnforcementService();
        _realmHeraldryService = new RealmHeraldryService();
    }

    /// <inheritdoc />
    public override void RegisterEvents()
    {
        CampaignEvents.OnCharacterCreationIsOverEvent.AddNonSerializedListener(this, OnCharacterCreationIsOver);
        CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, OnGameLoaded);
        CampaignEvents.MobilePartyCreated.AddNonSerializedListener(this, OnMobilePartyCreated);
        CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener(this, OnDailyTickParty);
        CampaignEvents.OnQuestCompletedEvent.AddNonSerializedListener(this, OnQuestCompleted);
        CampaignEvents.OnQuestStartedEvent.AddNonSerializedListener(this, OnQuestStarted);

        StoryModeReclamationCoordinator.TrySubscribeStoryModeTutorialEnded(this, OnStoryModeTutorialEnded);
        StealthTutorialSuppressor.Initialize();
    }

    /// <inheritdoc />
    public override void SyncData(IDataStore dataStore)
    {
        dataStore.SyncData("_rod_isInitialized", ref _isInitialized);
        dataStore.SyncData("_rod_isPlayerRealmReclaimed", ref _isPlayerRealmReclaimed);
        dataStore.SyncData("_rod_reservedCapitalId", ref _reservedCapitalId);
        dataStore.SyncData("_rod_reservedStrongholdId", ref _reservedStrongholdId);
    }

    private void OnCharacterCreationIsOver()
    {
        if (_isInitialized)
        {
            return;
        }

        _coordinator.ExecuteFullInitialization(out _reservedPlayerDomain);
        _isInitialized = true;

        bool isCampaign = Campaign.Current?.GameMode == CampaignGameMode.Campaign;
        if (isCampaign)
        {
            _reservedCapitalId = _reservedPlayerDomain?.Capital?.StringId ?? string.Empty;
            _reservedStrongholdId = _reservedPlayerDomain?.Stronghold?.StringId ?? string.Empty;

            if (_reservedPlayerDomain?.Capital != null)
            {
                StoryModeReclamationCoordinator.ShowPrologueIntroInquiry(
                    _reservedPlayerDomain.Capital.Name.ToString(),
                    _reservedPlayerDomain.Stronghold?.Name?.ToString() ?? "Stronghold");
            }
        }
        else
        {
            _isPlayerRealmReclaimed = true;
        }
    }

    private void OnGameLoaded(CampaignGameStarter starter)
    {
        _realmHeraldryService.FixupExistingFadedBanners();
    }

    private void OnQuestCompleted(QuestBase quest, QuestBase.QuestCompleteDetails detail)
    {
        if (quest != null &&
            quest.StringId == "find_hideout_tutorial_quest" &&
            detail == QuestBase.QuestCompleteDetails.Success)
        {
            TriggerReclamation();
        }
    }

    private void OnQuestStarted(QuestBase quest)
    {
        StealthTutorialSuppressor.TrySuppressQuest(quest);
    }

    private void OnStoryModeTutorialEnded()
    {
        StealthTutorialSuppressor.ScourActiveStealthQuests();
        TriggerReclamation();
    }

    private void TriggerReclamation()
    {
        if (_isPlayerRealmReclaimed)
        {
            return;
        }

        _reservedPlayerDomain ??= ReservedDomainResolver.Resolve(_reservedCapitalId, _reservedStrongholdId);
        if (_reservedPlayerDomain != null)
        {
            _isPlayerRealmReclaimed = true;
            _reclamationCoordinator.ExecuteReclamation(_reservedPlayerDomain);
        }
    }

    private void OnMobilePartyCreated(MobileParty party)
    {
        if (party.IsBandit)
        {
            _banditScalingService.ScaleBanditParty(party);
            return;
        }

        if (party.IsLordParty)
        {
            _minorClanEnforcementService.EnforceDailyLordPartyLimits(party);

            if (party.ActualClan != Clan.PlayerClan &&
                party.ActualClan?.IsBanditFaction == false &&
                _banditScalingService.IsGracePeriodActive)
            {
                _coordinator.ResetSingleLordParty(party);
            }
        }
    }

    private void OnDailyTickParty(MobileParty party)
    {
        if (party.IsBandit)
        {
            _banditScalingService.ScaleBanditParty(party);
            return;
        }

        if (party.IsLordParty)
        {
            _minorClanEnforcementService.EnforceDailyLordPartyLimits(party);
        }
    }
}
