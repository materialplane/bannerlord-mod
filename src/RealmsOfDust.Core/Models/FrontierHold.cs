namespace RealmsOfDust.Core.Models;

using System.Collections.Generic;
using TaleWorlds.CampaignSystem.Settlements;

/// <summary>
/// Represents an unaligned Frontier Stronghold (Castle) and its bound villages.
/// </summary>
public sealed class FrontierHold
{
    /// <summary>
    /// Gets the unaligned Castle fortification.
    /// </summary>
    public Settlement Stronghold { get; }

    /// <summary>
    /// Gets the bound villages assigned to this frontier stronghold.
    /// </summary>
    public IReadOnlyList<Settlement> BoundVillages { get; }

    public FrontierHold(Settlement stronghold, IReadOnlyList<Settlement> boundVillages)
    {
        Stronghold = stronghold ?? throw new System.ArgumentNullException(nameof(stronghold));
        BoundVillages = boundVillages ?? throw new System.ArgumentNullException(nameof(boundVillages));
    }
}
