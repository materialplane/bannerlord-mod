namespace RealmsOfDust.Domain.Player;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

/// <summary>
/// Positions the player party at their assigned sovereign seat on Day 1.
/// </summary>
public static class PlayerPartyPositioner
{
    /// <summary>
    /// Teleports the player party directly outside their starting capital gate.
    /// </summary>
    /// <param name="capital">The assigned starting Capital.</param>
    public static void PositionAtCapital(Settlement capital)
    {
        if (capital == null || MobileParty.MainParty == null)
        {
            return;
        }

        MobileParty.MainParty.Position = capital.GatePosition;
        MobileParty.MainParty.Party?.SetVisualAsDirty();
        MobileParty.MainParty.SetMoveModeHold();

        if (Campaign.Current != null && PartyBase.MainParty != null)
        {
            Campaign.Current.CameraFollowParty = PartyBase.MainParty;
        }
    }
}
