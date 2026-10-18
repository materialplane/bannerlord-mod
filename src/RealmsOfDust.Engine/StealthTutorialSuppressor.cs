namespace RealmsOfDust.Engine;

using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

/// <summary>
/// Suppresses the native TaleWorlds stealth tutorial inquiry and quests so they never interrupt campaign flow.
/// </summary>
public static class StealthTutorialSuppressor
{
    private static bool _isSubscribed;

    /// <summary>
    /// Registers the inquiry filter on the TaleWorlds InformationManager.
    /// </summary>
    public static void Initialize()
    {
        if (_isSubscribed) return;
        InformationManager.OnShowInquiry += OnInquiryShown;
        _isSubscribed = true;
    }

    /// <summary>
    /// Cancels the stealth tutorial quest if it attempts to start.
    /// </summary>
    public static void TrySuppressQuest(QuestBase? quest)
    {
        if (quest == null) return;
        if (IsStealthQuest(quest))
        {
            quest.CompleteQuestWithCancel();
        }
    }

    /// <summary>
    /// Scours the campaign quest manager and cancels any active stealth tutorial quests.
    /// </summary>
    public static void ScourActiveStealthQuests()
    {
        var quests = Campaign.Current?.QuestManager?.Quests;
        if (quests == null) return;

        foreach (var quest in quests.ToList())
        {
            if (quest != null && IsStealthQuest(quest))
            {
                quest.CompleteQuestWithCancel();
            }
        }
    }

    private static bool IsStealthQuest(QuestBase quest)
    {
        string name = quest.GetType().Name;
        string id = quest.StringId ?? string.Empty;
        return name.IndexOf("VillagersInNeed", StringComparison.OrdinalIgnoreCase) >= 0 ||
               id.IndexOf("villagers_in_need", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static void OnInquiryShown(InquiryData data, bool isAffirmative, bool isNegative)
    {
        if (data == null) return;

        string title = data.TitleText ?? string.Empty;
        if (title.IndexOf("Stealth", StringComparison.OrdinalIgnoreCase) >= 0 ||
            title.IndexOf("DhMge68x", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            InformationManager.HideInquiry();
            ScourActiveStealthQuests();
        }
    }
}
