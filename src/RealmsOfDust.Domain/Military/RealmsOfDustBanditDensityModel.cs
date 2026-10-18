namespace RealmsOfDust.Domain.Military;

using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using RealmsOfDust.Core.Constants;

/// <summary>
/// Custom bandit density model that scales roaming bandit and looter populations across Calradia.
/// Increases roaming party quotas around hideouts and expands the continental looter population cap.
/// </summary>
public class RealmsOfDustBanditDensityModel : DefaultBanditDensityModel
{
    /// <inheritdoc />
    public override int NumberOfMaximumBanditPartiesAroundEachHideout =>
        RealmsOfDustConstants.BanditHideoutRoamingCap;

    /// <inheritdoc />
    public override int GetMaxSupportedNumberOfLootersForClan(Clan clan)
    {
        int baseCount = base.GetMaxSupportedNumberOfLootersForClan(clan);
        return (int)Math.Round(baseCount * RealmsOfDustConstants.BanditLooterSpawnMultiplier);
    }
}
