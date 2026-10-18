namespace RealmsOfDust.Engine;

using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using RealmsOfDust.Core.Interfaces;
using RealmsOfDust.Core.Models;

/// <summary>
/// Coordinates the narrative presentation and restoration of the player's ancestral realm
/// during the Campaign Mode prologue.
/// </summary>
public sealed class StoryModeReclamationCoordinator
{
    private readonly IPlayerRealmInitializer _playerInitializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="StoryModeReclamationCoordinator"/> class.
    /// </summary>
    /// <param name="playerInitializer">The player realm initializer.</param>
    public StoryModeReclamationCoordinator(IPlayerRealmInitializer playerInitializer)
    {
        _playerInitializer = playerInitializer;
    }

    /// <summary>
    /// Displays the opening narrative modal explaining the Great Plague and Radagos' usurpation.
    /// </summary>
    /// <param name="capitalName">The name of the ancestral capital.</param>
    /// <param name="strongholdName">The name of the ancestral fortress.</param>
    public static void ShowPrologueIntroInquiry(string capitalName, string strongholdName)
    {
        TextObject title = new TextObject("{=rod_prologue_title}The Great Contagion & The Usurper");
        TextObject desc = new TextObject(
            "{=rod_prologue_desc}In the year 1084, the catastrophic Great Plague broke across Calradia, " +
            "devastating armies, collapsing trade, and fracturing the land into warring realms.\r\n\r\n" +
            "While your noble house was stricken by the sickness, the mercenary captain Radagos betrayed your bloodline. " +
            "He slaughtered your weakened garrison and seized your ancestral seat of {CAPITAL} and the fortress of {STRONGHOLD}!\r\n\r\n" +
            "Having fled into the wilderness with your elder brother, you must gather loyal survivors, track down the traitorous usurper, " +
            "and reclaim your rightful crown.")
            .SetTextVariable("CAPITAL", capitalName)
            .SetTextVariable("STRONGHOLD", strongholdName);

        TextObject affirmText = new TextObject("{=rod_prologue_btn}Continue");

        InformationManager.ShowInquiry(new InquiryData(
            title.ToString(),
            desc.ToString(),
            isAffirmativeOptionShown: true,
            isNegativeOptionShown: false,
            affirmText.ToString(),
            string.Empty,
            null,
            null));
    }

    /// <summary>
    /// Executes the full ancestral realm reclamation sequence.
    /// </summary>
    /// <param name="domain">The reserved player domain.</param>
    public void ExecuteReclamation(RealmDomain domain)
    {
        if (domain == null || domain.Capital == null)
        {
            return;
        }

        // 1. Elevate player to monarch and restore ancestral holdings
        _playerInitializer.InitializePlayerRealm(domain);

        // 2. Display celebratory announcement message
        string capitalName = domain.Capital.Name?.ToString() ?? "Ancestral Seat";
        string strongholdName = domain.Stronghold?.Name?.ToString() ?? "Stronghold";

        string logMessage = $"[Realms of Dust] The usurper Radagos is broken! " +
                           $"The ancestral realm of {capitalName} is restored to your bloodline!";
        InformationManager.DisplayMessage(new InformationMessage(logMessage, Colors.Yellow));

        // 3. Show immersive modal celebration inquiry
        ShowReclamationInquiry(capitalName, strongholdName);
    }

    private static void ShowReclamationInquiry(string capitalName, string strongholdName)
    {
        TextObject title = new TextObject("{=rod_inquiry_title}Ancestral Realm Reclaimed");
        TextObject desc = new TextObject(
            "{=rod_inquiry_desc}With the usurper Radagos defeated, your ancestral realm of {CAPITAL} " +
            "and the fortress of {STRONGHOLD} are restored to your bloodline! " +
            "The survivors of the Great Plague rally to your banners once more, and the gates are opened " +
            "to welcome their rightful sovereign.\r\n\r\n" +
            "Long live the Sovereign!")
            .SetTextVariable("CAPITAL", capitalName)
            .SetTextVariable("STRONGHOLD", strongholdName);

        TextObject affirmText = new TextObject("{=rod_inquiry_btn}Claim the Throne");

        InformationManager.ShowInquiry(new InquiryData(
            title.ToString(),
            desc.ToString(),
            isAffirmativeOptionShown: true,
            isNegativeOptionShown: false,
            affirmText.ToString(),
            string.Empty,
            null,
            null));
    }

    /// <summary>
    /// Safely subscribes to StoryMode tutorial end events using reflection if available.
    /// </summary>
    /// <param name="owner">The event listener owner.</param>
    /// <param name="callback">The callback to invoke when tutorial ends.</param>
    public static void TrySubscribeStoryModeTutorialEnded(object owner, Action callback)
    {
        try
        {
            Type? storyEventsType = Type.GetType("StoryMode.StoryModeEvents, StoryMode");
            if (storyEventsType == null)
            {
                return;
            }

            PropertyInfo? instProp = storyEventsType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            object? instance = instProp?.GetValue(null);
            if (instance == null)
            {
                return;
            }

            PropertyInfo? evProp = storyEventsType.GetProperty("OnStoryModeTutorialEndedEvent", BindingFlags.Public | BindingFlags.Instance);
            object? evObj = evProp?.GetValue(instance);
            if (evObj == null)
            {
                return;
            }

            MethodInfo? addListener = evObj.GetType().GetMethod("AddNonSerializedListener", new[] { typeof(object), typeof(Action) });
            addListener?.Invoke(evObj, new[] { owner, callback });
        }
        catch
        {
            // Fail safely if StoryMode assembly is absent or incompatible
        }
    }
}
