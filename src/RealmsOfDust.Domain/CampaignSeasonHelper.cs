namespace RealmsOfDust.Domain;

using System;
using TaleWorlds.CampaignSystem;

/// <summary>
/// Helper to determine the zero-indexed campaign season relative to Day 1.
/// In Bannerlord, campaigns start on Summer 1, Year 1084.
/// Season 0 = Summer 1084 (First 21 days)
/// Season 1 = Autumn 1084 (Days 22-42)
/// Season 2 = Winter 1084 (Days 43-63)
/// Season 3+ = Spring 1085 onwards (Year 2+)
/// </summary>
public static class CampaignSeasonHelper
{
    private const int StartYear = 1084;
    private const int StartSeasonOffset = (int)CampaignTime.Seasons.Summer; // Summer is season 1 in TaleWorlds enum

    /// <summary>
    /// Computes how many full 21-day seasons have passed since the campaign started on Summer 1, 1084.
    /// </summary>
    /// <returns>Zero-indexed season count (0 for the first season, 1 for the second, etc.).</returns>
    public static int GetElapsedSeasonIndex()
    {
        if (Campaign.Current == null)
        {
            return 0;
        }

        try
        {
            CampaignTime now = CampaignTime.Now;
            int yearsElapsed = Math.Max(0, now.GetYear - StartYear);
            int currentSeason = (int)now.GetSeasonOfYear;

            // Total absolute seasons from Year 1084 Spring (season 0)
            int totalSeasonsFromBase = (yearsElapsed * 4) + currentSeason;

            // Elapsed seasons relative to Summer 1084
            int elapsed = totalSeasonsFromBase - StartSeasonOffset;
            return Math.Max(0, elapsed);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Checks if the current campaign date is in the second half of a 21-day season (Day 11 to 21).
    /// </summary>
    public static bool IsSecondHalfOfSeason()
    {
        if (Campaign.Current == null)
        {
            return false;
        }

        try
        {
            // Bannerlord seasons are 21 days (GetDayOfSeason is 0 to 20)
            return CampaignTime.Now.GetDayOfSeason >= 10;
        }
        catch
        {
            return false;
        }
    }
}
