namespace RealmsOfDust.Engine;

using TaleWorlds.SaveSystem;

/// <summary>
/// Registers Realms of Dust custom domain models and structures with the TaleWorlds save system.
/// Uses a unique root saveBaseId to guarantee save/load integrity across versions.
/// </summary>
public sealed class RealmsOfDustSaveDefiner : SaveableTypeDefiner
{
    // Unique save identifier offset for Realms of Dust to prevent mod collisions
    private const int SaveBaseId = 948_200;

    /// <summary>
    /// Initializes a new instance of the <see cref="RealmsOfDustSaveDefiner"/> class.
    /// </summary>
    public RealmsOfDustSaveDefiner() : base(SaveBaseId)
    {
    }

    /// <inheritdoc />
    protected override void DefineClassTypes()
    {
        // Custom models persist within TaleWorlds CampaignBehaviorBase.SyncData
    }

    /// <inheritdoc />
    protected override void DefineContainerDefinitions()
    {
    }
}
