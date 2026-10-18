namespace RealmsOfDust.Engine;

using System;
using System.Reflection;
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

        ShowModalInquiry(title, desc, "{=rod_prologue_btn}Continue");
    }

    /// <summary>
    /// Executes the full ancestral realm reclamation sequence.
    /// </summary>
    /// <param name="domain">The reserved player domain.</param>
    /// <param name="isSkipped">True if the tutorial was resolved off-screen; otherwise, false.</param>
    public void ExecuteReclamation(RealmDomain domain, bool isSkipped = false)
    {
        if (domain?.Capital == null)
        {
            return;
        }

        _playerInitializer.InitializePlayerRealm(domain);

        string capitalName = domain.Capital.Name?.ToString() ?? "Ancestral Seat";
        string strongholdName = domain.Stronghold?.Name?.ToString() ?? "Stronghold";

        if (isSkipped)
        {
            string log = $"[Realms of Dust] Having routed Radagos during your flight, you arrive at {capitalName} to claim your sovereign throne!";
            InformationManager.DisplayMessage(new InformationMessage(log, Colors.Yellow));
            ShowSkipReclamationInquiry(capitalName, strongholdName);
        }
        else
        {
            string log = $"[Realms of Dust] The usurper Radagos is broken! The ancestral realm of {capitalName} is restored to your bloodline!";
            InformationManager.DisplayMessage(new InformationMessage(log, Colors.Yellow));
            ShowReclamationInquiry(capitalName, strongholdName);
        }
    }

    private static void ShowSkipReclamationInquiry(string capitalName, string strongholdName)
    {
        TextObject title = new TextObject("{=rod_skip_title}The Usurper Broken");
        TextObject desc = new TextObject(
            "{=rod_skip_desc}Having routed the traitor Radagos during your flight and recovered your family's ancient bronze chest " +
            "from the medicine man, you rally your loyal survivors and march to your ancestral seat.\r\n\r\n" +
            "The gates of {CAPITAL} swing open to welcome their rightful sovereign! The fortress of {STRONGHOLD} " +
            "and all bound villages reaffirm their fealty to your bloodline.\r\n\r\n" +
            "Long live the Sovereign!")
            .SetTextVariable("CAPITAL", capitalName)
            .SetTextVariable("STRONGHOLD", strongholdName);

        ShowModalInquiry(title, desc, "{=rod_inquiry_btn}Claim the Throne");
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

        ShowModalInquiry(title, desc, "{=rod_inquiry_btn}Claim the Throne");
    }

    private static void ShowModalInquiry(TextObject title, TextObject desc, string affirmKey)
    {
        TextObject affirmText = new TextObject(affirmKey);
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
            Type? storyEventsType = TutorialStatusChecker.FindStoryModeType("StoryMode.StoryModeEvents");
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
