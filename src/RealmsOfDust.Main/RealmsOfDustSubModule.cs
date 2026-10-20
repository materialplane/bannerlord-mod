namespace RealmsOfDust.Main;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using RealmsOfDust.Domain.Economy;
using RealmsOfDust.Domain.Military;
using RealmsOfDust.Engine;

/// <summary>
/// Entry point for the Realms of Dust campaign overhaul mod.
/// Inherits from TaleWorlds.MountAndBlade.MBSubModuleBase.
/// </summary>
public sealed class RealmsOfDustSubModule : MBSubModuleBase
{
    /// <inheritdoc />
    protected override void OnSubModuleLoad()
    {
        base.OnSubModuleLoad();
        try
        {
            IntroVideoBypassService.Instance.EnsureRegistered();
        }
        catch (System.Exception ex)
        {
            TaleWorlds.Library.Debug.Print($"[RealmsOfDust] SubModule load notice: {ex.Message}");
        }
    }

    /// <inheritdoc />
    protected override void OnApplicationTick(float dt)
    {
        base.OnApplicationTick(dt);
        IntroVideoBypassService.Instance.Tick();
    }

    /// <inheritdoc />
    protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
    {
        base.InitializeGameStarter(game, starterObject);

        try
        {
            if (starterObject is CampaignGameStarter campaignGameStarter)
            {
                campaignGameStarter.AddBehavior(new RealmsOfDustCampaignBehavior());
                campaignGameStarter.AddModel(new PostPlagueSettlementTaxModel());
                campaignGameStarter.AddModel(new PostPlagueVolunteerModel());
                campaignGameStarter.AddModel(new PostPlagueClanFinanceModel());
                campaignGameStarter.AddModel(new PostPlaguePartySizeLimitModel());
                campaignGameStarter.AddModel(new RealmsOfDustBanditDensityModel());
                campaignGameStarter.AddModel(new PostPlagueSettlementLoyaltyModel());
            }
        }
        catch (System.Exception ex)
        {
            System.IO.File.AppendAllText(
                @"C:\ProgramData\Mount and Blade II Bannerlord\logs\realms_of_dust.log",
                $"[{System.DateTime.Now}] InitializeGameStarter Exception: {ex}\n");
            throw;
        }
    }
}
