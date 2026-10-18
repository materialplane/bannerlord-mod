namespace RealmsOfDust.Engine;

using System;
using System.Reflection;
using TaleWorlds.CampaignSystem;

/// <summary>
/// Detects whether the StoryMode tutorial phase has been skipped, completed, or bypassed.
/// </summary>
public static class TutorialStatusChecker
{
    /// <summary>
    /// Determines whether the player has skipped, completed, or bypassed the tutorial phase.
    /// Returns true for Sandbox mode or when StoryMode tutorial is marked completed/skipped.
    /// </summary>
    /// <returns>True if the tutorial is not active or has ended; otherwise, false.</returns>
    public static bool IsTutorialSkippedOrCompleted()
    {
        // 1. Non-Campaign game modes (e.g. Sandbox) never have a tutorial phase
        if (Campaign.Current?.GameMode != CampaignGameMode.Campaign)
        {
            return true;
        }

        try
        {
            // 2. Check TutorialPhase.Instance status via reflection
            Type? tutType = Type.GetType("StoryMode.StoryModePhases.TutorialPhase, StoryMode");
            if (tutType != null)
            {
                PropertyInfo? instProp = tutType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                object? inst = instProp?.GetValue(null);
                if (inst != null)
                {
                    PropertyInfo? compProp = tutType.GetProperty("IsCompleted", BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo? skipProp = tutType.GetProperty("IsSkipped", BindingFlags.Public | BindingFlags.Instance);
                    bool isCompleted = (bool)(compProp?.GetValue(inst) ?? false);
                    bool isSkipped = (bool)(skipProp?.GetValue(inst) ?? false);
                    if (isCompleted || isSkipped)
                    {
                        return true;
                    }
                }
            }

            // 3. Check MainStoryLine.FirstPhase on StoryModeManager
            Type? smmType = Type.GetType("StoryMode.StoryModeManager, StoryMode");
            if (smmType != null)
            {
                PropertyInfo? currProp = smmType.GetProperty("Current", BindingFlags.Public | BindingFlags.Static);
                object? smm = currProp?.GetValue(null);
                if (smm != null)
                {
                    PropertyInfo? mslProp = smmType.GetProperty("MainStoryLine", BindingFlags.Public | BindingFlags.Instance);
                    object? msl = mslProp?.GetValue(smm);
                    if (msl != null)
                    {
                        PropertyInfo? fpProp = msl.GetType().GetProperty("FirstPhase", BindingFlags.Public | BindingFlags.Instance);
                        if (fpProp?.GetValue(msl) != null)
                        {
                            return true;
                        }
                    }
                }
            }

            // 4. Fallback: check if post-tutorial main quest (Neretzes' Folly) is active
            if (Campaign.Current?.QuestManager != null)
            {
                foreach (var quest in Campaign.Current.QuestManager.Quests)
                {
                    if (quest.StringId == "investigate_neretzes_folly" ||
                        quest.StringId == "rebuild_player_clan")
                    {
                        return true;
                    }
                }
            }
        }
        catch
        {
            // Fail safely if StoryMode types are unavailable
        }

        return false;
    }
}
