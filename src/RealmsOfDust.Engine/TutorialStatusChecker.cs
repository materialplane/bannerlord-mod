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
    /// Resolves a StoryMode type safely across AppDomain assemblies.
    /// </summary>
    /// <param name="typeFullName">The full name or assembly-qualified name of the type.</param>
    /// <returns>The resolved Type, or null if not found.</returns>
    public static Type? FindStoryModeType(string typeFullName)
    {
        Type? type = Type.GetType(typeFullName);
        if (type != null)
        {
            return type;
        }

        string cleanName = typeFullName.Contains(",") ? typeFullName.Split(',')[0].Trim() : typeFullName;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (string.Equals(asm.GetName().Name, "StoryMode", StringComparison.OrdinalIgnoreCase))
            {
                Type? found = asm.GetType(cleanName);
                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Determines whether the player has skipped, completed, or bypassed the tutorial phase.
    /// Returns true for Sandbox mode or when StoryMode tutorial is marked completed/skipped.
    /// </summary>
    /// <returns>True if the tutorial is not active or has ended; otherwise, false.</returns>
    public static bool IsTutorialSkippedOrCompleted()
    {
        if (Campaign.Current?.GameMode != CampaignGameMode.Campaign)
        {
            return true;
        }

        try
        {
            // 1. Check TutorialPhase status
            Type? tutType = FindStoryModeType("StoryMode.StoryModePhases.TutorialPhase");
            if (tutType != null)
            {
                PropertyInfo? instProp = tutType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                object? inst = instProp?.GetValue(null);
                if (inst != null)
                {
                    PropertyInfo? compProp = tutType.GetProperty("IsCompleted", BindingFlags.Public | BindingFlags.Instance);
                    PropertyInfo? skipProp = tutType.GetProperty("IsSkipped", BindingFlags.Public | BindingFlags.Instance);
                    if ((bool)(compProp?.GetValue(inst) ?? false) || (bool)(skipProp?.GetValue(inst) ?? false))
                    {
                        return true;
                    }
                }
            }

            // 2. Check FirstPhase status or instance
            Type? firstPhaseType = FindStoryModeType("StoryMode.StoryModePhases.FirstPhase");
            if (firstPhaseType != null)
            {
                PropertyInfo? instProp = firstPhaseType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                if (instProp?.GetValue(null) != null)
                {
                    return true;
                }
            }

            // 3. Fallback: check post-tutorial main quests
            if (Campaign.Current?.QuestManager != null)
            {
                foreach (var quest in Campaign.Current.QuestManager.Quests)
                {
                    if (quest.StringId == "investigate_neretzes_folly" || quest.StringId == "rebuild_player_clan")
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

    /// <summary>
    /// Determines whether the tutorial was explicitly skipped rather than completed via hideout.
    /// </summary>
    /// <returns>True if TutorialPhase was skipped; otherwise, false.</returns>
    public static bool IsTutorialSkipped()
    {
        try
        {
            Type? tutType = FindStoryModeType("StoryMode.StoryModePhases.TutorialPhase");
            if (tutType != null)
            {
                PropertyInfo? instProp = tutType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                object? inst = instProp?.GetValue(null);
                if (inst != null)
                {
                    PropertyInfo? skipProp = tutType.GetProperty("IsSkipped", BindingFlags.Public | BindingFlags.Instance);
                    return (bool)(skipProp?.GetValue(inst) ?? false);
                }
            }
        }
        catch
        {
            // Fail safely
        }

        return false;
    }
}
