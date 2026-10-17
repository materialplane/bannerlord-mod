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
