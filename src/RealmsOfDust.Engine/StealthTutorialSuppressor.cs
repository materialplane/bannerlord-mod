namespace RealmsOfDust.Engine;

using System;
using TaleWorlds.Library;

/// <summary>
/// Suppresses the native TaleWorlds stealth tutorial inquiry so it never interrupts the campaign flow.
/// </summary>
public static class StealthTutorialSuppressor
{
    private static bool _isSubscribed;

    /// <summary>
    /// Registers the inquiry filter on the TaleWorlds InformationManager.
    /// </summary>
    public static void Initialize()
    {
        if (_isSubscribed)
        {
            return;
        }

        InformationManager.OnShowInquiry += OnInquiryShown;
        _isSubscribed = true;
    }

    private static void OnInquiryShown(InquiryData data, bool isAffirmative, bool isNegative)
    {
        if (data == null)
        {
            return;
        }

        string title = data.TitleText ?? string.Empty;
        if (title.IndexOf("Stealth", StringComparison.OrdinalIgnoreCase) >= 0 ||
            title.IndexOf("DhMge68x", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            InformationManager.HideInquiry();
        }
    }
}
